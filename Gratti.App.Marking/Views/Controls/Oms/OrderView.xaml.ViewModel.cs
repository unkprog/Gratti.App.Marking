using ReactiveUI;
using System;
using System.Collections.Generic;
using Gratti.App.Marking.Api.Model;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Views.Models;
using Gratti.App.Marking.Extensions;
using System.Reactive;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Views.Controls.Oms.Models
{
    public partial class OrdersViewModel : LogViewModel
    {
        public OrdersViewModel()
        {
            //OrdersModel ordersResponse = test();
            //this.Orders = ordersResponse.Orders;

            RefreshCommand = ReactiveCommand.Create(() => { Task.Run(() => Refresh()); });
            PrintOneCurrentOrderInfoCommand = ReactiveCommand.Create(() => { Task.Run(() => PrintOneCurrentOrderInfo()); });
            PrintAllAvalaibleCurrentOrderInfoCommand = ReactiveCommand.Create(() => { Task.Run(() => PrintAllAvalaibleCurrentOrderInfo()); });
        }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

        
            public ReactiveCommand<Unit, Unit> PrintOneCurrentOrderInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> PrintAllAvalaibleCurrentOrderInfoCommand { get; }

        public CertificateInfoModel Certificate { get { return Utils.Certificate.GetCertificateInfo(App.Self.Auth.Profile.ThumbPrint); } }

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
            App.Self.MainVM.Run(() =>
            {
                OrdersModel ordersResponse = App.Self.OmsApi.GetOrders(App.Self.Auth.OmsToken, Api.GroupEnum.lp);
                //OrdersModel ordersResponse = test();
                ordersResponse.Orders.Sort((x, y) => { return y.CreatedDateTime.CompareTo(x.CreatedDateTime); });

                foreach (OrderInfoModel order in ordersResponse.Orders)
                {
                    Dictionary<string, OrderProductInfoModel> productItems = App.Self.OmsApi.GetOrderProductInfo(App.Self.Auth.OmsToken, Api.GroupEnum.lp, order.OrderId);
                    foreach(BufferInfoModel buffer in order.Buffers)
                    {
                        OrderProductInfoModel product;
                        if (productItems.TryGetValue(buffer.Gtin, out product))
                        {
                            buffer.ProductInfo = product;
                        }
                    }
                }
                this.Orders = ordersResponse.Orders;
            });

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
            
            foreach(BufferInfoModel buffer in CurrentOrderInfo.Buffers)
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

        //private OrdersModel test()
        //{
        //   return new OrdersModel()
        //    {
        //        OmsId = App.Self.Auth.Profile.OmsId,
        //        Orders = new List<OrderInfoModel>(new OrderInfoModel[]
        //            {
        //                new OrderInfoModel()
        //                {
        //                    OrderId = "123",
        //                    OrderStatusStr = OrderInfoModel.OrderStatusEnum.PENDING,
        //                    Buffers = new List<BufferInfoModel>(new BufferInfoModel[] {
        //                        new BufferInfoModel()
        //                        {
        //                            OmsId = App.Self.Auth.Profile.OmsId,
        //                            OrderId = "123",
        //                            AvailableCodes = 3,
        //                            BufferStatus = BufferStatusEnum.ACTIVE,
        //                            TotalCodes = 5,
        //                            TotalPassed = 2
        //                        },
        //                        new BufferInfoModel()
        //                        {
        //                            OmsId = App.Self.Auth.Profile.OmsId,
        //                            OrderId = "123",
        //                            AvailableCodes = 3,
        //                            BufferStatus = BufferStatusEnum.ACTIVE,
        //                            TotalCodes = 5,
        //                            TotalPassed = 2
        //                        }
        //                    }),
        //                    ProductionOrderId = "123",

        //                },
        //                new OrderInfoModel()
        //                {
        //                    OrderId = "321",
        //                    OrderStatus = OrderInfoModel.OrderStatusEnum.PENDING,
        //                    Buffers = new List<BufferInfoModel>(new BufferInfoModel[] {
        //                        new BufferInfoModel()
        //                        {
        //                            OmsId = App.Self.Auth.Profile.OmsId,
        //                            OrderId = "321",
        //                            AvailableCodes = 3,
        //                            BufferStatus = BufferStatusEnum.ACTIVE,
        //                            TotalCodes = 5,
        //                            TotalPassed = 2
        //                        },
        //                        new BufferInfoModel()
        //                        {
        //                            OmsId = App.Self.Auth.Profile.OmsId,
        //                            OrderId = "321",
        //                            AvailableCodes = 3,
        //                            BufferStatus = BufferStatusEnum.ACTIVE,
        //                            TotalCodes = 7,
        //                            TotalPassed = 4
        //                        }
        //                    }),
        //                    ProductionOrderId = "321",

        //                }
        //            })
        //    };
        //}
    }
}
