using System;
using System.Net.Http;

namespace Endurance_Testing.Core
{
    public static class HttpClientProvider
    {
        private static readonly Lazy<HttpClient> _httpClientInstance =
            new Lazy<HttpClient>(() => new HttpClient());

        public static HttpClient Instance => _httpClientInstance.Value;
    }
}
