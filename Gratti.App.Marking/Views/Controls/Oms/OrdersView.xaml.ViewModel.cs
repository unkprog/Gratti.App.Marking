using System.Collections.Generic;
using Gratti.App.Marking.Api.Model;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Views.Models;
using System.Reactive;
using ReactiveUI;
using System.Reactive.Linq;

namespace Gratti.App.Marking.Views.Controls.Oms.Models
{
    public partial class OrdersViewModel : CertViewModel
    {
        public OrdersViewModel()
        {
            RefreshCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => Refresh()); });
            PrintAllAvalaibleOrdersInfoCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => PrintAllOrderInfo()); });
            PrintOneCurrentOrderInfoCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => PrintCurrentOrderInfo(1)); });
            PrintAllAvalaibleCurrentOrderInfoCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => PrintCurrentOrderInfo(-1)); });
            CreateOrderCommand = ReactiveCommand.Create(CreateOrder);
            App.Self.MainVM.RunAsync(() => Refresh());
        }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Unit, Unit> PrintAllAvalaibleOrdersInfoCommand { get; }
        
        public ReactiveCommand<Unit, Unit> PrintOneCurrentOrderInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> PrintAllAvalaibleCurrentOrderInfoCommand { get; }

        public ReactiveCommand<Unit, Unit> CreateOrderCommand { get; }

        private List<OrderInfoModel> orders;
        public List<OrderInfoModel> Orders
        {
            get { return orders; }
            set
            {
                this.RaiseAndSetIfChanged(ref orders, value);
                this.RaisePropertyChanged("IsAvalaibleOrders");
            }
        }

        public bool IsAvalaibleOrders
        {
            get { return this.Orders != null && this.Orders.Count > 0 && this.Orders.Exists((f)=> f.Buffers.Exists((b)=> b.AvailableCodes > 0)); }
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
            SyncThread(() => App.Self.MainVM.TextBusy = "Получение списка заказов...");
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

        private void PrintAllOrderInfo()
        {
            Log("Сохрание кодов маркировки...");
            if (this.Orders == null || this.Orders.Count < 1)
            {
                App.Self.MainVM.Info("Нет заказов для сохранения...");
                return;
            }

            foreach (OrderInfoModel order in this.Orders)
            {
                SyncThread(() => App.Self.MainVM.TextBusy = "Сохрание КМ по заказу " + CurrentOrderInfo.OrderId + "...");

                foreach (BufferInfoModel buffer in order.Buffers)
                {
                    if (buffer.AvailableCodes > 0)
                    {
                        CodesModel codes = App.Self.OmsApi.GetCodes(App.Self.Auth.OmsToken, Api.GroupEnum.lp, CurrentOrderInfo.OrderId, buffer.Gtin, buffer.AvailableCodes);
                        foreach (string dmcode in codes.Codes)
                        {
                            DataMatrixModel model = new DataMatrixModel(dmcode) { ProductGroup = Api.GroupEnum.lp.ToString() };

                            model.Barcode = buffer.ProductInfo?.RawOrigin;
                            if (string.IsNullOrEmpty(model.Barcode))
                                model.Barcode = CurrentOrderInfo.ProductionOrderId;

                            SaveCisTrue(App.Self.Auth.Profile.SqlConnectionString, model);
                        }
                        Log("Сохранено кодов маркировки: " + codes.Codes.Count.ToString() + " шт...");
                    }
                }
            }
            Log("Сохрание кодов маркировки завершено...");
            Refresh();
        }

        private void CreateOrder()
        {
            App.Self.MainVM.Content = new Oms.OrderNewView();
        }

        private void PrintCurrentOrderInfo(int aCount)
        {
            if (CurrentOrderInfo == null)
            {
                App.Self.MainVM.Info("Заказ не выбран...");
                return;
            }

            Log("Сохрание КМ по заказу " + CurrentOrderInfo.OrderId + "...");
            SyncThread(() => App.Self.MainVM.TextBusy = "Сохрание КМ по заказу " + CurrentOrderInfo .OrderId + "...");

            int dCount = aCount;
            foreach (BufferInfoModel buffer in CurrentOrderInfo.Buffers)
            {
                if (buffer.AvailableCodes > 0 && (dCount == -1 || dCount > 0))
                {
                    CodesModel codes = App.Self.OmsApi.GetCodes(App.Self.Auth.OmsToken, Api.GroupEnum.lp, CurrentOrderInfo.OrderId, buffer.Gtin, dCount == -1 || buffer.AvailableCodes < dCount? buffer.AvailableCodes : dCount);
                    foreach (string dmcode in codes.Codes)
                    {
                        DataMatrixModel model = new DataMatrixModel(dmcode) { ProductGroup = Api.GroupEnum.lp.ToString() };

                        model.Barcode = buffer.ProductInfo?.RawOrigin;
                        if (string.IsNullOrEmpty(model.Barcode))
                            model.Barcode = CurrentOrderInfo.ProductionOrderId;

                        SaveCisTrue(App.Self.Auth.Profile.SqlConnectionString, model);
                        dCount--;

                        if (dCount != -1 && dCount == 0)
                            break;
                    }
                    Log("Сохранено кодов маркировки: " + codes.Codes.Count.ToString() + " шт...");
                }
                if (dCount != -1 && dCount == 0)
                    break;
            }

            if (dCount != -1 && dCount > 0)
                App.Self.MainVM.Info("Сохранено " + (aCount - dCount).ToString() + " из заданных " + aCount.ToString() + " ...");

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
