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


        public async Task<T> SendPostEmailAsync<T>(string baseUrl, object body)
        {
            try
            {
                using (var _httpClient = new System.Net.Http.HttpClient())
                {
                    _httpClient.BaseAddress = new Uri(baseUrl);

                    var json = JsonConvert.SerializeObject(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var _response = await _httpClient.PostAsync(baseUrl, content); // Await this async call

                    if (_response.IsSuccessStatusCode)
                    {
                        var _content1 = await _response.Content.ReadAsStringAsync(); // Await this async call
                        var item = JsonConvert.DeserializeObject<T>(_content1);
                        return item;
                    }
                    else
                    {
                        Console.WriteLine(_response);
                        throw new Exception(await _response.Content.ReadAsStringAsync()); // Await this async call
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
