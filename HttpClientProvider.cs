using System.Net.Http;

namespace Endurance_Testing
{
    public static class HttpClientProvider
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static HttpClient Instance => _httpClient;
    }
}
