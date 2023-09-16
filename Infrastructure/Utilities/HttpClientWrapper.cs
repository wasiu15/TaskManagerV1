using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace TaskManager.Infrastructure.Utilities
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IConfiguration _configuration;

        public HttpClientWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T SendPostEmailAsync<T>(string baseUrl, object body)
        {
            try
            {
                using (var _httpClient = new System.Net.Http.HttpClient())
                {
                    _httpClient.BaseAddress = new Uri(baseUrl);

                    var json = JsonConvert.SerializeObject(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var _response = _httpClient.PostAsync(baseUrl, content).Result;
                    if (_response.IsSuccessStatusCode)
                    {
                        var _content1 = _response.Content.ReadAsStringAsync().Result;
                        var Item = JsonConvert.DeserializeObject<T>(_content1);
                        return Item;
                    }
                    else
                    {
                        Console.WriteLine(_response);
                        throw new Exception(_response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
