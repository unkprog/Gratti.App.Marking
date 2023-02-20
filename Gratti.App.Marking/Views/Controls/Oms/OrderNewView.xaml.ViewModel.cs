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
using Gratti.App.Marking.Api;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Utils;

namespace Gratti.App.Marking.Views.Controls.Oms.Models
{
    public partial class OrderNewViewModel : CertViewModel
    {
        public OrderNewViewModel()
        {
            Group = GroupEnum.lp;
            order = new OrderNewModel()
            {
                CreateMethodType = OrderNewModel.CreateMethodTypeEnum.SELF_MADE,
                ReleaseMethodType = OrderNewModel.ReleaseMethodTypeEnum.PRODUCTION
            };
            product = new OrderNewProductModel()
            {
                Quantity = 1,
                CisType = OrderNewProductModel.CisTypeEnum.UNIT,
                SerialNumberType = OrderNewProductModel.SerialNumerTypeEnum.OPERATOR,
                TemplateId = 10
            };
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

        private GroupEnum group;
        public GroupEnum Group
        {
            get { return group; }
            set { this.RaiseAndSetIfChanged(ref group, value); }
        }

        public IEnumerable<EnumExtensions.ValueDisplayName> GroupValues { get { return EnumExtensions.GetAllValuesDisplayName(typeof(GroupEnum)); } }

        public IEnumerable<EnumExtensions.ValueDisplayName> ReleaseMethodTypeValues { get { return EnumExtensions.GetAllValuesDisplayName(typeof(OrderNewModel.ReleaseMethodTypeEnum)); } }
        public IEnumerable<EnumExtensions.ValueDisplayName> CreateMethodTypeValues { get { return EnumExtensions.GetAllValuesDisplayName(typeof(OrderNewModel.CreateMethodTypeEnum)); } }
        public IEnumerable<EnumExtensions.ValueDisplayName> CisTypeValues { get { return EnumExtensions.GetAllValuesDisplayName(typeof(OrderNewProductModel.CisTypeEnum)); } }

        private string VerifyCreateOrder()
        {
            string result = string.Empty;

            var appendResult = new Action<string>((msg) =>
            {
                result = string.Concat(result, string.IsNullOrEmpty(result) ? string.Empty : Environment.NewLine, msg);
            });

            if (string.IsNullOrEmpty(Product.Gtin))
                appendResult("Укажите код товара (GTIN)");
            if (Product.Quantity < 1)
                appendResult("Укажите количество кодов");

            return result;
        }

        private void CreateOrder()
        {
            string errorMessage = VerifyCreateOrder();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                App.Self.MainVM.Error(errorMessage, "Создание заказа");
                return;
            }

            JsonSerializerOptions opts = new JsonSerializerOptions();
            opts.Converters.Add(new JsonStringEnumConverter());
            opts.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            string content = JsonSerializer.Serialize(order, order.GetType(), opts);
            OrderResultModel orderResult = App.Self.OmsApi.PostOrder(App.Self.Auth.OmsToken, Api.GroupEnum.lp, Order, Utils.Certificate.SignByCertificate(App.Self.Auth.GetCertificate(), content));
            SyncThread(() => App.Self.MainVM.Content = new Oms.OrdersView());
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
