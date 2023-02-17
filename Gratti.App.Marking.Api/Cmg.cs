using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Gratti.App.Marking.Api.Model;
using Gratti.App.Marking.Extensions;

namespace Gratti.App.Marking.Api
{
    public class Cmg
    {
        public Cmg(string baseUrl, string apiKey)
        {
            this.baseUrl = baseUrl;
            this.apiKey = apiKey;
        }

        private string baseUrl;
        private string apiKey;

        private T Get<T>(string method, string query = "")
        {
            T result;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(string.Concat(baseUrl, "/v3", method, "?apikey=", apiKey, query));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                result = client.GetJson<T>("");
            }
            return result;
        }

        public ProductModel ProductByGtin(string gtin)
        {
            return Get<ProductModel>("/product", string.Concat("&gtin=", gtin));
        }
    }
}
