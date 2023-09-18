namespace TaskManager.Infrastructure.Utilities
{
    public interface IHttpClientWrapper
    {
        Task<T> SendPostEmailAsync<T>(string baseUrl, object body);
    }
}
