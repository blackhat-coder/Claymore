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
        public Task StoreResponseAsync(string id, string headers, object response)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("Please provide a valid Id");

                var line = $"{id}|{headers}|{response}\n#";
                using (StreamWriter sw = new StreamWriter("response.txt", true))
                {
                    sw.Write(line);
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Storing response, msg: {ex.Message}");
                return Task.CompletedTask;
            }
        }

        public Task StoreResponseAsync(string id, string headers, string response)
        {
            try
            {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("Please provide a valid Id");

                var line = $"{id}|{headers}|{response}\n#";
                using (StreamWriter sw = new StreamWriter("response.txt", true))
                {
                    sw.Write(line);
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Storing response, msg: {ex.Message}");
                return Task.CompletedTask;
            }
        }

        public Task<string> GetAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("Please provide a valid Id");

                using (StreamReader sr = new StreamReader("response.txt"))
                {
                    string line;

                    while((line = sr.ReadLine()) != null)
                    {
                        var splits = line.Split("|");
                        if (splits[0] == id)
                        {
                            return Task.FromResult<string>(line);
                        }
                    }
                }

                return Task.FromResult<string>(string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Getting response, msg: {ex.Message}");
                return Task.FromResult<string>(string.Empty);
            }
        }

        public Task<string> GetResponseHeaderAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("Please provide a valid Id");

                using (StreamReader sr = new StreamReader("response.txt"))
                {
                    string line;

                    while ((line = sr.ReadToEnd()) != null)
                    {
                        var entries = line.Split("#");
                        foreach (var entry in entries)
                        {
                            var splits = entry.Split("|");
                            if (splits[0] == id)
                            {
                                return Task.FromResult<string>(splits[2]);
                            }
                        }
                    }
                }

                return Task.FromResult<string>(string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Getting response, msg: {ex.Message}");
                return Task.FromResult<string>(string.Empty);
            }
        }

        public Task<string> GetResponseBodyAsync(string id)
        {
            try
            {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("Please provide a valid Id");

                using (StreamReader sr = new StreamReader("response.txt"))
                {
                    string line;

                    while ((line = sr.ReadToEnd()) != null)
                    {
                        var entries = line.Split("#");
                        foreach (var entry in entries)
                        {
                            var splits = entry.Split("|");
                            if (splits[0] == id)
                            {
                                return Task.FromResult<string>(splits[2]);
                            }
                        }
                    }
                }

                return Task.FromResult<string>(string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Getting response, msg: {ex.Message}");
                return Task.FromResult<string>(string.Empty);
            }
        }
    }
}
