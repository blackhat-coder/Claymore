using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Helpers
{
    public static class HttpResponseHeaderExtension
    {
        public static string ToJsonString(this HttpResponseHeaders headers)
        {
            var headerDictionary = new Dictionary<string, string>();
            foreach(var header in headers)
            {
                string headerValue = string.Join(", ", header.Value);
                headerDictionary.Add(header.Key, headerValue);
            };

            string jsonString = JsonConvert.SerializeObject(headerDictionary, Formatting.Indented);
            return jsonString;
        }
    }
}
