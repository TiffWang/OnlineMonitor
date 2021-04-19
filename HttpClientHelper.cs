using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMonitor
{
    public class HttpClientHelper
    {

        private static HttpClient _client;

        static HttpClientHelper()
        {

            var handler = new HttpClientHandler { UseCookies = true };
            _client = new HttpClient(handler);

            //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", parameter);
            _client.Timeout = TimeSpan.FromSeconds(10);
            //长连接
            _client.DefaultRequestHeaders.Connection.Add("keep-alive");
            //ServicePointManager.FindServicePoint(new Uri(""));

            //HttpClient 预热
            var isSuccess = _client.SendAsync(new HttpRequestMessage
            {
                Method = new HttpMethod("HEAD"),
                RequestUri = new Uri("http://www.baidu.com/")

            }).Result.IsSuccessStatusCode;

        }

        public static TResult Get<TResult>(string url, int seconds = 10) where TResult : class, new()
        {
            return JosnToObject<TResult>(Get(url, seconds));
        }

        public static async Task<TResult> GetAsync<TResult>(string url, int seconds = 10) where TResult : class, new()
        {
            return JosnToObject<TResult>(await GetAsync(url, seconds));
        }

        public static async Task<string> GetAsync(string url, int seconds = 10)
        {
            string result = string.Empty;
            string httpUrl = GetUrl(url);

            //_client.Timeout = TimeSpan.FromSeconds(seconds);

            var response = await _client.GetAsync(httpUrl, (httpRequestMessage) =>
            {
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpRequestMessage = SetRequestMessageHeaders(httpRequestMessage);

            });

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Could not made request to " + httpUrl + "! StatusCode: " +
                                    response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static string Get(string url, int seconds = 10)
        {
            string result = string.Empty;
            string httpUrl = GetUrl(url);


            var response = _client.GetAsync(httpUrl, (httpRequestMessage) =>
            {

                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpRequestMessage = SetRequestMessageHeaders(httpRequestMessage);

            }).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Could not made request to " + httpUrl + "! StatusCode: " +
                                    response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
            }
            result = response.Content.ReadAsStringAsync().Result;
            return result;
        }

        public static HttpResponseMessage GetResponse(string url, int seconds = 10)
        {
            HttpResponseMessage result = null;
            string httpUrl = GetUrl(url);


            var response = _client.GetAsync(httpUrl, (httpRequestMessage) =>
            {

                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpRequestMessage = SetRequestMessageHeaders(httpRequestMessage);

            }).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Could not made request to " + httpUrl + "! StatusCode: " +
                                    response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
            }
            result = response;
            return result;
        }

        public static TResult Post<TResult>(string url, int seconds = 10) where TResult : class, new()
        {
            return JosnToObject<TResult>(Post(url, seconds));
        }

        public static string Post(string url, int seconds = 10)
        {
            string result = string.Empty;
            string httpUrl = GetUrl(url);


            var response = _client.PostAsync(httpUrl, (httpRequestMessage) =>
            {

                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpRequestMessage = SetRequestMessageHeaders(httpRequestMessage);

            }).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Could not made request to " + httpUrl + "! StatusCode: " +
                                    response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
            }
            result = response.Content.ReadAsStringAsync().Result;
            return result;
        }



        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <typeparam name="TResult">返回结果</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="input">传输参数如：new {id=1}</param>
        /// <param name="timeout">超时时间S</param>
        /// <returns>返回TResult</returns>
        public TResult Post<TResult>(string url, object input) where TResult : class, new()

        {
            TResult result = default(TResult);

            result = PostAsync<TResult>(url, input).Result;

            return result;
        }


        public async Task<TResult> PostAsync<TResult>(string url, object input)
          where TResult : class, new()
        {
            var httpUrl = GetUrl(url);
            var obj = input;
            var response = _client.PostAsync(httpUrl, (httpRequestMessage) =>
            {
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
               
                httpRequestMessage.Content = new StringContent(ObjectToString(obj), Encoding.UTF8, "application/json");
                
                httpRequestMessage = SetRequestMessageHeaders(httpRequestMessage);
            });

            

            return JosnToObject<TResult>(await response.Result.Content.ReadAsStringAsync());
        }





        public static HttpRequestMessage SetRequestMessageHeaders(HttpRequestMessage requestMessage)
        {
            var httpRequestMessage = requestMessage;
            //处理Cookies
            return httpRequestMessage;
        }

        private static string GetUrl(string url)
        {
            string newUlr = url;
            return newUlr;
        }

        private static TResult JosnToObject<TResult>(string obj) where TResult : class, new()
        {
            if (string.IsNullOrEmpty(obj))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<TResult>(obj);
        }

        private static string ObjectToString(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

    }



    public static class HttpClientExtension
    {

        public static Task<HttpResponseMessage> GetAsync(this HttpClient httpClient, string requestUri, Action<HttpRequestMessage> preAction)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            preAction(httpRequestMessage);

            return httpClient.SendAsync(httpRequestMessage);
        }

        public static Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string requestUri, Action<HttpRequestMessage> preAction)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
            preAction(httpRequestMessage);
            return httpClient.SendAsync(httpRequestMessage);
        }
    }

}
