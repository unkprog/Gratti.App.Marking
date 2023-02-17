﻿using System.Collections.Generic;
using Gratti.App.Marking.Api.Model;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Views.Models;
using System.Reactive;
using ReactiveUI;
using System.Reactive.Linq;

namespace Gratti.App.Marking.Views.Controls.Oms.Models
{
    public partial class OrdersViewModel : LogViewModel
    {
        public OrdersViewModel()
        {
            RefreshCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => Refresh()); });
            PrintOneCurrentOrderInfoCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => PrintCurrentOrderInfo(1)); });
            PrintAllAvalaibleCurrentOrderInfoCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => PrintCurrentOrderInfo(-1)); });
            CheckCurrentOrderInfoCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => CheckCurrentOrderInfo()); });
            App.Self.MainVM.RunAsync(() => Refresh());
        }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Unit, Unit> PrintOneCurrentOrderInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> PrintAllAvalaibleCurrentOrderInfoCommand { get; }

        public ReactiveCommand<Unit, Unit> CheckCurrentOrderInfoCommand { get; }

        public CertificateInfoModel Certificate
        {
            get
            {
                return Utils.Certificate.GetCertificateInfo(App.Self.Auth.Profile.ThumbPrint);
            }
        }

        private List<OrderInfoModel> orders;
        public List<OrderInfoModel> Orders
        {
            get { return orders; }
            set { this.RaiseAndSetIfChanged(ref orders, value); }
        }

        private OrderInfoModel currentOrderInfo;
        public OrderInfoModel CurrentOrderInfo
        {
            get { return currentOrderInfo; }
            set { this.RaiseAndSetIfChanged(ref currentOrderInfo, value); }
        }

        private BufferInfoModel currentBufferInfo;
        public BufferInfoModel CurrentBufferInfo
        {
            get { return currentBufferInfo; }
            set
            {
                this.RaiseAndSetIfChanged(ref currentBufferInfo, value);
                this.RaisePropertyChanged("IsCurrentBufferInfo");
            }
        }

        public bool IsCurrentBufferInfo
        {
            get { return currentBufferInfo != null; }
        }

        

        public void Refresh()
        {
            Log("Получение списка заказов...");
            OrdersModel ordersResponse = App.Self.OmsApi.GetOrders(App.Self.Auth.OmsToken, Api.GroupEnum.lp);
            ordersResponse.Orders.Sort((x, y) => { return y.CreatedDateTime.CompareTo(x.CreatedDateTime); });

            foreach (OrderInfoModel order in ordersResponse.Orders)
            {
                Dictionary<string, OrderProductInfoModel> productItems = App.Self.OmsApi.GetOrderProductInfo(App.Self.Auth.OmsToken, Api.GroupEnum.lp, order.OrderId);
                foreach (BufferInfoModel buffer in order.Buffers)
                {
                    OrderProductInfoModel product;
                    if (productItems.TryGetValue(buffer.Gtin, out product))
                    {
                        buffer.ProductInfo = product;
                    }
                }
            }
            SyncThread(() => {
                this.Orders = ordersResponse.Orders;
                if (this.Orders.Count > 0)
                    this.CurrentOrderInfo = this.Orders[0];
                });
            Log("Получено заказов: " + this.Orders.Count.ToString() + "...");
        }


        private void CheckCurrentOrderInfo()
        {
            if (CurrentOrderInfo == null)
            {
                App.Self.MainVM.Info("Заказ не выбран...");
                return;
            }

            if (CurrentOrderInfo.Buffers.Count > 0)
            {
                BufferInfoModel buffer = CurrentOrderInfo.Buffers[0];
                ProductModel product = App.Self.CmgApi.ProductByGtin(buffer.ProductInfo.GTIN);
                Log("Test " + product.Name);
            }
        }

        private void PrintCurrentOrderInfo(int aCount)
        {
            if (CurrentOrderInfo == null)
            {
                App.Self.MainVM.Info("Заказ не выбран...");
                return;
            }

            Log("Сохрание кодов маркировки...");
            int iCount = 0;
            foreach (BufferInfoModel buffer in CurrentOrderInfo.Buffers)
            {
                if (buffer.AvailableCodes > 0 && (aCount == -1 || iCount < aCount))
                {
                    CodesModel codes = App.Self.OmsApi.GetCodes(App.Self.Auth.OmsToken, Api.GroupEnum.lp, CurrentOrderInfo.OrderId, buffer.Gtin, 1);
                    foreach (string dmcode in codes.Codes)
                    {
                        DataMatrixModel model = new DataMatrixModel(dmcode) { ProductGroup = Api.GroupEnum.lp.ToString() };

                        model.Barcode = buffer.ProductInfo?.RawOrigin;
                        if (string.IsNullOrEmpty(model.Barcode))
                            model.Barcode = CurrentOrderInfo.ProductionOrderId;

                        SaveCisTrue(App.Self.Auth.Profile.SqlConnectionString, model);
                        iCount++;
                        if (aCount != -1 && iCount >= aCount)
                            break;
                    }
                    Log("Сохранено кодов маркировки: " + codes.Codes.Count.ToString() + " шт...");
                }
                if (aCount != -1 && iCount >= aCount)
                    break;
            }

            if (aCount != -1 && iCount < aCount)
                App.Self.MainVM.Info("Сохранено " + iCount.ToString() + " из заданных " + (aCount == -1 ? iCount : aCount).ToString() + " ...");

            Log("Сохрание кодов маркировки завершено...");
            Refresh();
        }


        // https://xn--j1ab.xn----7sbabas4ajkhfocclk9d3cvfsa.xn--p1ai/rest/tabs/goods/175630239
        // x-csrf-token: z7u81ueznjbtabmbxlqn8vbz95yy1o8zjz826ryhhn7fufe06zlrbawe7349hj8r


//        {
//  "pageSize": 10,
//  "pageNumber": 1,
//  "sort": {
//    "field": "created",
//    "direction": "DESC"
//  },
//  "fields": [
//    "type",
//    "photo",
//    "created",
//    "gtin",
//    "name",
//    "category",
//    "packages",
//    "brand",
//    "markingCondition",
//    "status"
//  ],
//  "filter": {
//    "gtin": [
//      "04603702421225"
//    ]
//    }
//}





//        {
//  "data": [
//    {
//      "goods": {
//        "id": 175630239,
//        "goodId": 175630239,
//        "status": "published",
//        "link": "https://национальный-каталог.рф/product/4603702421225-ru-kurtka-zhenskaya",
//        "preVersion": 0,
//        "indexing": 0,
//        "isMarking": true,
//        "type": "unit",
//        "photo": {
//          "url": "",
//          "previewUrl": ""
//        },
//        "created": "2023-02-10T10:55:28+03:00",
//        "gtin": "4603702421225",
//        "name": "Куртка женская",
//        "category": "Одежда",
//        "packages": [],
//        "brand": "21208",
//        "markingCondition": "turn"
//      },
//      "draft": null
//    }
//  ],
//  "isGs1Loads": true,
//  "totalObjects": 1,
//  "totalPages": 1
//}




    }
}
