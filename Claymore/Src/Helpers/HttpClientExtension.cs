using Claymore.Src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Helpers;

public static class HttpClientExtension
{
    public async static Task<HttpClient> AddRequestHeaders(this HttpClient client, ClaymoreSyntaxResolver resolver, List<Header> headers) {

        foreach (var header in headers)
        {
            var headerValue = await resolver.FindAndReplace(header.value);
            client.DefaultRequestHeaders.Add(header.key, headerValue ?? "");
        }

        return client;
    }
}