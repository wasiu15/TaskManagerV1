namespace TaskManager.Infrastructure.Utilities
{
    public interface IHttpClientWrapper
    {
        T SendPostEmailAsync<T>(string baseUrl, object body);
    }
}
