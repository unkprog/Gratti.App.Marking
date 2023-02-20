using ReactiveUI;
using System.Reactive;
using Gratti.App.Marking.Views.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Gratti.App.Marking.Api.Model;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using Gratti.App.Marking.Core.Extensions;

namespace Gratti.App.Marking.Views.Controls.Oms.Models
{
    public partial class OrderNewViewModel : CertViewModel
    {
        public OrderNewViewModel()
        {
            order = new OrderNewModel() { CreateMethodType = OrderNewModel.CreateMethodTypeEnum.SELF_MADE, ReleaseMethodType = OrderNewModel.ReleaseMethodTypeEnum.REMAINS };
            product = new OrderNewProductModel() { Quantity = 1, CisType = OrderNewProductModel.CisTypeEnum.UNIT  };
            order.Products.Add(product);
            CreateOrderCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => CreateOrder()); });
            CancelCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => Cancel()); });
            SelectGTINCommand = ReactiveCommand.Create(() => { App.Self.MainVM.RunAsync(() => SelectGTIN()); });
        }

        public ReactiveCommand<Unit, Unit> SelectGTINCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateOrderCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        //public ReactiveCommand<Unit, Unit> PrintOneCurrentOrderInfoCommand { get; }
        //public ReactiveCommand<Unit, Unit> PrintAllAvalaibleCurrentOrderInfoCommand { get; }

        //public ReactiveCommand<Unit, Unit> CheckCurrentOrderInfoCommand { get; }


        private OrderNewModel order;
        public OrderNewModel Order
        {
            get { return order; }
            set { this.RaiseAndSetIfChanged(ref order, value); }
        }

        private OrderNewProductModel product;
        public OrderNewProductModel Product
        {
            get { return product; }
            set { this.RaiseAndSetIfChanged(ref product, value); }
        }

        public IEnumerable<EnumExtensions.ValueDisplayName> CisTypeValues { get { return EnumExtensions.GetAllValuesDisplayName(typeof(OrderNewProductModel.CisTypeEnum)); } }

        private void CreateOrder()
        {
            JsonSerializerOptions opts = new JsonSerializerOptions();
            opts.Converters.Add(new JsonStringEnumConverter());
            opts.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            string content = JsonSerializer.Serialize(order, order.GetType(), opts);
            OrderResultModel orderResult = App.Self.OmsApi.PostOrder(App.Self.Auth.OmsToken, Api.GroupEnum.lp, Order, Utils.Certificate.SignByCertificate(App.Self.Auth.GetCertificate(), content));
        }

        private void Cancel()
        {
            SyncThread(() => App.Self.MainVM.Content = new Oms.OrdersView());
        }

        private void SelectGTIN()
        {

        }

    }
}
