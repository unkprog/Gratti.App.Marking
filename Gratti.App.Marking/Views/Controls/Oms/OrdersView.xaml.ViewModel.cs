using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive;
using ReactiveUI;
using Gratti.App.Marking.Core.Extensions;
using Gratti.App.Marking.Api.Model;
using Gratti.App.Marking.Views.Models;

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

        private bool isAutoRefresh = true;
        public bool IsAutoRefresh
        {
            get { return isAutoRefresh; }
            set
            {
                this.RaiseAndSetIfChanged(ref isAutoRefresh, value);
                if (value)
                    StartRefreshTimer();
                else
                    CancelRefreshTimer();
            }
        }


        private bool isPrintAvalaible = true;
        public bool IsPrintAvalaible
        {
            get { return isPrintAvalaible; }
            set
            {
                this.RaiseAndSetIfChanged(ref isPrintAvalaible, value);
                this.RaisePropertyChanged("IsAvalaibleOrders");
                this.RaisePropertyChanged("OrdersView");
            }
        }
        

        private List<OrderInfoModel> orders;
        public List<OrderInfoModel> Orders
        {
            get { return orders; }
            set
            {
                this.RaiseAndSetIfChanged(ref orders, value);
                this.RaisePropertyChanged("OrdersView");
                this.RaisePropertyChanged("IsAvalaibleOrders");
            }
        }

        public IEnumerable<OrderInfoModel> OrdersView
        {
            get { return (IsPrintAvalaible ? (orders == null ? null : orders.Where(f=> f.TotalAvailableCodes > 0)) : orders); }
        }

        public bool IsAvalaibleOrders
        {
            get { return this.Orders != null && this.Orders.Count > 0 && this.Orders.Exists((f)=> f.Buffers.Exists((b)=> b.AvailableCodes > 0)); }
        }


        private OrderInfoModel currentOrderInfo;
        public OrderInfoModel CurrentOrderInfo
        {
            get { return currentOrderInfo; }
            set
            {
                this.RaiseAndSetIfChanged(ref currentOrderInfo, value);
                this.RaisePropertyChanged("IsCurrentAvalaibleOrder");
                this.RaisePropertyChanged("IsCurrentBufferInfo");
            }
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

        public bool IsCurrentAvalaibleOrder
        {
            get { return currentOrderInfo != null && currentOrderInfo.TotalAvailableCodes > 0; }
        }

        public bool IsCurrentBufferInfo
        {
            get { return currentBufferInfo != null && currentBufferInfo.AvailableCodes > 0; }
        }

        private bool isRefreshing = false;
        public void Refresh()
        {
            if (isRefreshing)
                return;

            try
            {
                isRefreshing = true;
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
                SyncThread(() =>
                {
                    this.Orders = ordersResponse.Orders;
                    if (this.OrdersView.Count() > 0)
                        this.CurrentOrderInfo = this.OrdersView.ElementAt(0);
                });
            }
            finally
            {
                isRefreshing = false;
                Log("Получено заказов: " + this.Orders?.Count.ToString() + "...");
            }
            if (cts == null && isAutoRefresh)
                StartRefreshTimer();
        }

        private void PrintAllOrderInfo()
        {
            Log("Сохрание кодов маркировки...");
            if (this.Orders == null || this.Orders.Count < 1)
            {
                App.Self.MainVM.Info("Нет заказов для сохранения...");
                return;
            }

            foreach (OrderInfoModel order in this.Orders.Where(f=> f.TotalAvailableCodes > 0))
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
            CancelRefreshTimer();
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


        private async Task TaskDelayAsync(CancellationToken ct)
        {
            int i = 0;
            while (true)
            {
               
                if (ct.IsCancellationRequested)
                {
                    break;
                }
                await Task.Delay(500);
                i++;
                if (i >= 30)
                {
                    i = 0;
                    RefreshCommand.Execute().Subscribe();
                }
            }
        }

        private CancellationTokenSource cts;

        public async Task StartRefreshTimer()
        {
            cts = new CancellationTokenSource();
            try
            {
                await TaskDelayAsync(cts.Token);
            }
            catch (Exception ex)
            {
                Log(ex.GetMessages());
            }
            cts = null;
        }

        public void CancelRefreshTimer()
        {
            cts?.Cancel();
        }


    }
}
