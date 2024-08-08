using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Services.ResponseStore
{
    public class FileResponseStore(ILogger<FileResponseStore> _logger) : IResponseStore
    {
        public Task StoreResponse(string id, object response)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("Please provide a valid Id");

                var line = $"{id},{response}";
                using (StreamWriter sw = new StreamWriter("response.txt"))
                {
                    sw.WriteLine(line);
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Storing response, msg: {ex.Message}");
                return Task.CompletedTask;
            }
        }

        public Task StoreResponse(string id, string response)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("Please provide a valid Id");

                var line = $"{id},{response}";
                using (StreamWriter sw = new StreamWriter("response.txt"))
                {
                    sw.WriteLine(line);
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Storing response, msg: {ex.Message}");
                return Task.CompletedTask;
            }
        }
    }
}
