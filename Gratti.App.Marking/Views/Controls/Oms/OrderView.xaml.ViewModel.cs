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
            OrdersModel ordersResponse = test();
            this.Orders = ordersResponse.Orders;

            PrintAllAvalaibleCurrentOrderInfoCommand = ReactiveCommand.Create(() => { Task.Run(PrintAllAvalaibleCurrentOrderInfo); });
        }

        public ReactiveCommand<Unit, Unit> PrintAllAvalaibleCurrentOrderInfoCommand { get; }

        public CertificateInfoModel Certificate { get { return Utils.Certificate.GetCertificateInfo(App.Self.Auth.Profile.SerialNumber); } }

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
            TryCatch.Invoke(() =>
            {
                //OrdersModel ordersResponse = App.Self.OmsApi.GetOrders(App.Self.Auth.OmsToken, Api.GroupEnum.lp);
                OrdersModel ordersResponse = test();
                this.Orders = ordersResponse.Orders;
            }, 
            (errorMessage) => Log(errorMessage));

        }

        private void PrintAllAvalaibleCurrentOrderInfo()
        {
            if (CurrentOrderInfo == null)
                return;
            
            foreach(BufferInfoModel buffer in CurrentOrderInfo.Buffers)
            {
                CodesModel codes = App.Self.OmsApi.GetCodes(App.Self.Auth.OmsToken, Api.GroupEnum.lp, CurrentOrderInfo.OrderId, buffer.Gtin, buffer.AvailableCodes);
                foreach(string dmcode in codes.Codes)
                {
                    SaveCisTrue( dmcode);
                }
            }
        }

        private OrdersModel test()
        {
           return new OrdersModel()
            {
                OmsId = App.Self.Auth.Profile.OmsId,
                Orders = new List<OrderInfoModel>(new OrderInfoModel[]
                    {
                        new OrderInfoModel()
                        {
                            OrderId = "123",
                            OrderStatus = OrderInfoModel.OrderStatusEnum.PENDING,
                            Buffers = new List<BufferInfoModel>(new BufferInfoModel[] {
                                new BufferInfoModel()
                                {
                                    OmsId = App.Self.Auth.Profile.OmsId,
                                    OrderId = "123",
                                    AvailableCodes = 3,
                                    BufferStatus = BufferInfoModel.BufferStatusEnum.ACTIVE,
                                    TotalCodes = 5,
                                    TotalPassed = 2
                                },
                                new BufferInfoModel()
                                {
                                    OmsId = App.Self.Auth.Profile.OmsId,
                                    OrderId = "123",
                                    AvailableCodes = 3,
                                    BufferStatus = BufferInfoModel.BufferStatusEnum.ACTIVE,
                                    TotalCodes = 5,
                                    TotalPassed = 2
                                }
                            }),
                            ProductionOrderId = "123",

                        },
                        new OrderInfoModel()
                        {
                            OrderId = "321",
                            OrderStatus = OrderInfoModel.OrderStatusEnum.PENDING,
                            Buffers = new List<BufferInfoModel>(new BufferInfoModel[] {
                                new BufferInfoModel()
                                {
                                    OmsId = App.Self.Auth.Profile.OmsId,
                                    OrderId = "321",
                                    AvailableCodes = 3,
                                    BufferStatus = BufferInfoModel.BufferStatusEnum.ACTIVE,
                                    TotalCodes = 5,
                                    TotalPassed = 2
                                },
                                new BufferInfoModel()
                                {
                                    OmsId = App.Self.Auth.Profile.OmsId,
                                    OrderId = "321",
                                    AvailableCodes = 3,
                                    BufferStatus = BufferInfoModel.BufferStatusEnum.ACTIVE,
                                    TotalCodes = 7,
                                    TotalPassed = 4
                                }
                            }),
                            ProductionOrderId = "321",

                        }
                    })
            };
        }
    }
}
