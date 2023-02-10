using System.Collections.Generic;
using Gratti.App.Marking.Api.Model;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Views.Models;
using System.Reactive;
using ReactiveUI;
using Gratti.App.Marking.Extensions;
using System.Data.SqlClient;
using System;
using System.ComponentModel;
using System.Windows;

namespace Gratti.App.Marking.Views.Controls.Oms.Models
{
    public partial class OrdersViewModel : LogViewModel
    {
        public OrdersViewModel()
        {

            RefreshCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => Refresh()); });
            PrintOneCurrentOrderInfoCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => PrintOneCurrentOrderInfo()); });
            PrintAllAvalaibleCurrentOrderInfoCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => PrintAllAvalaibleCurrentOrderInfo()); });
        }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Unit, Unit> PrintOneCurrentOrderInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> PrintAllAvalaibleCurrentOrderInfoCommand { get; }

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
            set { this.RaiseAndSetIfChanged(ref currentBufferInfo, value); }
        }

        public void Refresh()
        {
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
            SyncThread(() => this.Orders = ordersResponse.Orders);
        }

        private void PrintOneCurrentOrderInfo()
        {
            if (CurrentOrderInfo == null)
                return;

            foreach (BufferInfoModel buffer in CurrentOrderInfo.Buffers)
            {
                if (buffer.AvailableCodes > 0)
                {
                    CodesModel codes = App.Self.OmsApi.GetCodes(App.Self.Auth.OmsToken, Api.GroupEnum.lp, CurrentOrderInfo.OrderId, buffer.Gtin, 1);
                    foreach (string dmcode in codes.Codes)
                    {
                        DataMatrixModel model = new DataMatrixModel(dmcode) { ProductGroup = Api.GroupEnum.lp.ToString() };
                        SaveCisTrue(App.Self.Auth.Profile.SqlConnectionString, model);
                    }
                    break;
                }
            }
            Refresh();
        }

        private void PrintAllAvalaibleCurrentOrderInfo()
        {
            if (CurrentOrderInfo == null)
                return;

            foreach (BufferInfoModel buffer in CurrentOrderInfo.Buffers)
            {
                if (buffer.AvailableCodes > 0)
                {
                    CodesModel codes = App.Self.OmsApi.GetCodes(App.Self.Auth.OmsToken, Api.GroupEnum.lp, CurrentOrderInfo.OrderId, buffer.Gtin, buffer.AvailableCodes);
                    foreach (string dmcode in codes.Codes)
                    {
                        DataMatrixModel model = new DataMatrixModel(dmcode) { ProductGroup = Api.GroupEnum.lp.ToString(), Barcode = CurrentOrderInfo.ProductionOrderId };
                        SaveCisTrue(App.Self.Auth.Profile.SqlConnectionString, model);
                    }
                }
            }
            Refresh();
        }


    }
}
