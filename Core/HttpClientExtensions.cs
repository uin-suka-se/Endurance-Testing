using System.IO.Compression;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Endurance_Testing.Core
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> SendCompressedAsync(this HttpClient client, HttpRequestMessage request)
        {
            if (request.Content != null)
            {
                using (var contentStream = await request.Content.ReadAsStreamAsync())
                using (var compressedStream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
                    {
                        await contentStream.CopyToAsync(gzipStream);
                    }

                    compressedStream.Seek(0, SeekOrigin.Begin);
                    request.Content = new StreamContent(compressedStream);
                    request.Content.Headers.ContentEncoding.Add("gzip");
                }
            }

            return await client.SendAsync(request);
        }
    }
}
