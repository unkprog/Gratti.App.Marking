using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gratti.Marking.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetJsonAsync<T>(this HttpClient client, string parameters = "")
        {
            T result;
            HttpResponseMessage response = await client.GetAsync(parameters);  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            var resultString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            // Parse the response body.
           
            if (response != null)
            {
                //var resultString = await response.Content.ReadAsStringAsync();  //Make sure to add a reference to System.Net.Http.Formatting.dll
                result = JsonSerializer.Deserialize<T>(resultString);
            }
            else
                result = default(T);//throw new Exception(string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));

            return result;
        }


        public static T GetJson<T>(this HttpClient client, string parameters = "")
        {
            Task<T> taskResult = Task.Run(async () =>
            {
                return await client.GetJsonAsync<T>(parameters);
            });
            taskResult.Wait();
            return taskResult.Result;

        }

        public static T ReadAsJson<T>(this HttpContent content)
        {
            var dataAsString = content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<T>(dataAsString);
        }


        public static async Task<R> PostAsJsonAsync<T, R>(this HttpClient httpClient, string url, T data)
        {
            R result;
            var dataAsString = JsonSerializer.Serialize(data);
            var message = new HttpRequestMessage
            {
                RequestUri = new Uri(httpClient.BaseAddress.OriginalString + url),
                Method = HttpMethod.Post,
                Content = new StringContent(dataAsString, Encoding.UTF8, "application/json") { Headers = { ContentType = new MediaTypeHeaderValue("application/json") } }
            };
            HttpResponseMessage response = await httpClient.SendAsync(message);
            var resultString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            // Parse the response body.
            //Make sure to add a reference to System.Net.Http.Formatting.dll

            if (response != null)
            {
                //var resultString = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<R>(resultString);
            }
            else
                result = default(R);
            //else
            //{
            //    throw new Exception(string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
            //}
            return result;
        }

        public static R PostJson<T, R>(this HttpClient client, string url, T data)
        {
            Task<R> taskResult = Task.Run(async () =>
            {
                return await client.PostAsJsonAsync<T, R>(url, data);
            });
            taskResult.Wait();
            return taskResult.Result;

        }

        public static async Task<R> PostAsJsonAsync2<R>(this HttpClient httpClient, string url, string data)
        {
            R result;
            var dataAsString = data; // JsonSerializer.Serialize(data);
            var message = new HttpRequestMessage
            {
                RequestUri = new Uri(httpClient.BaseAddress.OriginalString + url),
                Method = HttpMethod.Post,
                Content = new StringContent(dataAsString, Encoding.UTF8, "application/json") { Headers = { ContentType = new MediaTypeHeaderValue("application/json") } }
            };
            HttpResponseMessage response = await httpClient.SendAsync(message);
            var resultString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            // Parse the response body.
            //Make sure to add a reference to System.Net.Http.Formatting.dll

            if (response != null)
            {
                //var resultString = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<R>(resultString);
            }
            else
                result = default(R);
            //else
            //{
            //    throw new Exception(string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
            //}
            return result;
        }

        public static R PostJson2<R>(this HttpClient client, string url, string data)
        {
            Task<R> taskResult = Task.Run(async () =>
            {
                return await client.PostAsJsonAsync2<R>(url, data);
            });
            taskResult.Wait();
            return taskResult.Result;

        }
    }
}
