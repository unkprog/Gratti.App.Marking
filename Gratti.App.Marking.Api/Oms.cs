using System;
using System.Net.Http.Headers;
using System.Net.Http;
using Gratti.App.Marking.Api.Model;
using Gratti.App.Marking.Extensions;
using System.Collections.Generic;

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

        private R Post<T, R>(string clientToken, GroupEnum group, string method, T data, string sign = "")
        {
            R result;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(string.Concat(baseUrl, "/api/v2/", group.ToString(), method, "?omsId=", omsId));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("clientToken", clientToken);
                if (!string.IsNullOrEmpty(sign))
                {
                    client.DefaultRequestHeaders.Add("X-Signature", sign);
                }
                result = client.PostJson<T, R>("", data);
            }
            return result;
        }

        public OrdersModel GetOrders(string clientToken, GroupEnum group)
		{
            return Get<OrdersModel>(clientToken, group, "/orders");

        }

        public Dictionary<string, OrderProductInfoModel> GetOrderProductInfo(string clientToken, GroupEnum group, string orderId)
        {
            return Get<Dictionary<string, OrderProductInfoModel>>(clientToken, group, "/order/product", string.Concat("&orderId=", orderId));
        }


        public CodesModel GetCodes(string clientToken, GroupEnum group, string orderId, string gtin, int quantity)
        {
            return Get<CodesModel>(clientToken, group, "/codes", string.Concat("&orderId=", orderId, "&gtin=", gtin, "&quantity=", quantity));
        }

        public OrderResultModel PostOrder(string clientToken, GroupEnum group, OrderNewModel order, string signature = "")
        {
            return Post<OrderNewModel, OrderResultModel>(clientToken, group, "/orders", order, signature);
        }

        //TODO: Сделать периодический пинг для проверки сессии
        public string Ping(string clientToken, GroupEnum group)
        {
            return Get<string>(clientToken, group, "/ping");
        }

    }
}
