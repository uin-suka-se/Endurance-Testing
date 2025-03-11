using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Endurance_Testing.Core
{
    public static class HttpClientProvider
    {
        private static readonly Lazy<IServiceProvider> _serviceProvider = new Lazy<IServiceProvider>(() =>
            new ServiceCollection()
                .AddHttpClient()
                .BuildServiceProvider()
        );

        public static HttpClient Instance => _serviceProvider.Value.GetRequiredService<IHttpClientFactory>().CreateClient();
    }
}