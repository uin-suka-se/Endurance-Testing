using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Endurance_Testing.Core
{
    public static class HttpClientProvider
    {
        private static readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddHttpClient()
            .BuildServiceProvider();

        public static HttpClient Instance => _serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
    }
}