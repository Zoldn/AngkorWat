using AngkorWat.Components;
using Newtonsoft.Json;
using OperationsResearch.Pdlp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO.HTTP
{
    public class HttpHelper
    {
        public static string ApiKey { get; private set; }
        public static HttpClient Client { get; private set; }
        static HttpHelper()
        {
            ApiKey = string.Empty;
            Client = new HttpClient();
            
        }
        public static void SetApiKey(string key)
        {
            ApiKey = key;
            Client.DefaultRequestHeaders.Add("X-Auth-Token", ApiKey);
        }

        public static async Task<string> Get(string url)
        {
            var response = await Client.GetStringAsync(url);

            return response;
        }

        public static async Task<T?> Get<T>(string url)
        {
            var response = await Client.GetStringAsync(url);

            var ret = JsonConvert.DeserializeObject<T>(response);

            return ret;
        }

        public static async Task<string> Post<TInput>(string url, TInput dataToSend)
        {
            var json = JsonConvert.SerializeObject(dataToSend);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(url, data);

            Console.WriteLine(response.StatusCode);

            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        public static async Task<TOutput?> Post<TInput, TOutput>(string url, TInput dataToSend)
        {
            var json = JsonConvert.SerializeObject(dataToSend);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(url, data);

            Console.WriteLine(response.StatusCode);

            string result = response.Content.ReadAsStringAsync().Result;

            var ret = JsonConvert.DeserializeObject<TOutput>(result);

            return ret;
        }

        public static async Task<(TOutput?, HttpStatusCode)> PostWithStatus<TInput, TOutput>(string url, TInput dataToSend)
        {
            var json = JsonConvert.SerializeObject(dataToSend);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(url, data);

            Console.WriteLine(response.StatusCode);

            string result = response.Content.ReadAsStringAsync().Result;

            var ret = JsonConvert.DeserializeObject<TOutput>(result);

            return (ret, response.StatusCode);
        }

        public static async Task<string> PostMultipart(string url)
        {
            using MultipartFormDataContent multipartContent = new();

            //multipartContent.Add(new StringContent("John", Encoding.UTF8, MediaTypeNames.Text.Plain), "first_name");
            //multipartContent.Add(new StringContent("Doe", Encoding.UTF8, MediaTypeNames.Text.Plain), "last_name");

            using var response = await Client.PostAsync(url, multipartContent);

            string result = response.Content.ReadAsStringAsync().Result;

            //Console.WriteLine(response.StatusCode);

            return result;
        }

        public static async Task<string> PostMultipartWithContent(string url,
            Dictionary<string, string> content)
        {
            using MultipartFormDataContent multipartContent = new("----WebKitFormBoundary7MA4YWxkTrZu0gW");

            foreach (var (key, value) in content)
            {
                multipartContent.Add(
                    new StringContent(value, Encoding.UTF8, MediaTypeNames.Text.Plain), key);
            }

            //multipartContent.Add(new StringContent("John", Encoding.UTF8, MediaTypeNames.Text.Plain), "first_name");
            //multipartContent.Add(new StringContent("Doe", Encoding.UTF8, MediaTypeNames.Text.Plain), "last_name");

            using var response = await Client.PostAsync(url, multipartContent);

            string result = response.Content.ReadAsStringAsync().Result;

            //Console.WriteLine(response.StatusCode);

            return result;
        }
    }
}
