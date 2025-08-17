using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Services.ResponseStore
{
    public interface IResponseStore
    {
        /// <summary>
        /// Id should carry the identifier of the response
        /// response should carry the response
        /// Id can be the ThreadId-name where name is the name of the endpoint.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task StoreResponseAsync(string id, string headers, object response, bool success);
        Task StoreResponseAsync(string id, string headers, string response, bool success);
        Task<string> GetAsync(string id);
        Task<string> GetResponseHeaderAsync(string id);
        Task<string> GetResponseBodyAsync(string id);
        Task<bool> GetTaskStatus(string id);
        Task StoreTaskAsync(string workerId, string headers, object response, bool success, int elapsedTime);
    }
}
