using System;
using System.Net.Http.Headers;
using System.Net.Http;
using Gratti.App.Marking.Api.Model;
using Gratti.Marking.Extensions;

namespace Gratti.App.Marking.Api
{
    public class Oms
    {
        public Oms(string baseUrl, string omsId) {
            this.baseUrl = baseUrl;
            this.omsId = omsId;
        }

        private string baseUrl;
        private string omsId;

        private T Get<T>(string clientToken, GroupEnum group, string method, string query = "")
        {
            T result;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(string.Concat(baseUrl, "/api/v2/", group.ToString(), method, "?omsId=", omsId, query));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("clientToken", clientToken);
                result = client.GetJson<T>("");
            }
            return result;
        }

        public OrdersModel GetOrders(string clientToken, GroupEnum group)
		{
            return Get<OrdersModel>(clientToken, group, "/orders");

        }

        public OrderProductInfoModel GetOrderProductInfo(string clientToken, GroupEnum group, string orderId)
        {
            return Get<OrderProductInfoModel>(clientToken, group, "/order/product", string.Concat("&orderId=", orderId));
        }


        public CodesModel GetCodes(string clientToken, GroupEnum group, string orderId, string gtin, int quantity)
        {
            return Get<CodesModel>(clientToken, group, "/codes", string.Concat("&orderId=", orderId, "&gtin=", gtin, "&quantity=", quantity));
        }
        
    }
}
