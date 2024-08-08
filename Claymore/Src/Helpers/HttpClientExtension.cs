using Claymore.Src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Helpers
{
    public static class HttpClientExtension
    {
        public static HttpClient AddRequestHeaders(this HttpClient client, List<Header> headers) {

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.key, header.value);
            }

            return client;
        }
    }
}
