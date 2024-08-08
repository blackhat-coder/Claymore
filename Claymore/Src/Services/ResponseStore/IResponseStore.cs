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
        Task StoreResponse(string id, object response);
        Task StoreResponse(string id, string response);
    }
}
