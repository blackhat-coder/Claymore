using Claymore.Src.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src;

public class ClaymoreWorkers
{
    private readonly HttpClient _httpClient;
    private Stopwatch _stopWatch;
    private ILogger<ClaymoreWorkers> _logger;

    public ClaymoreWorkers(ILogger<ClaymoreWorkers> logger, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _stopWatch = Stopwatch.StartNew();
    }

    public async Task Run()
    {
        try
        {
            _logger.LogInformation("Trying");

            var endpointsInfo = ConfigurationReader.Config.endpointsInfo;

            foreach(var endpointInfo in endpointsInfo)
            {
                if (endpointInfo.method == HttpConfigMethod.GET)
                {
                    _stopWatch.Start();
                    var response = await _httpClient.GetAsync(endpointInfo.endpoint);
                    _stopWatch.Stop();
                    // Save this response somewhere
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
