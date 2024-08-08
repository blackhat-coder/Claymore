using Claymore.Src.Enums;
using Claymore.Src.Helpers;
using Claymore.Src.Models;
using Claymore.Src.Services.ResponseStore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src;

public class ClaymoreWorkers
{
    private readonly HttpClient _httpClient;
    private Stopwatch _stopWatch;
    private ILogger<ClaymoreWorkers> _logger;
    private IResponseStore _responseStore;

    public ClaymoreWorkers(ILogger<ClaymoreWorkers> logger, IHttpClientFactory httpClientFactory, IResponseStore responseStore)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _responseStore = responseStore;
        _stopWatch = Stopwatch.StartNew();
    }

    public async Task Run()
    {
        try
        {
            _logger.LogInformation("Trying");

            var endpointsInfo = ConfigurationReader.Config.endpointsInfo;

            // Loop through the requests
            foreach(var endpointInfo in endpointsInfo)
            {
                if (endpointInfo.method == HttpConfigMethod.GET)
                {
                    _stopWatch.Start();

                    _httpClient.AddRequestHeaders(endpointInfo.headers);

                    var response = await _httpClient.GetAsync(endpointInfo.endpoint);
                    _stopWatch.Stop();
                    var respContent = await response.Content.ReadAsStringAsync();
                    await _responseStore.StoreResponse($"{Thread.CurrentThread.ManagedThreadId}_{endpointInfo.name}", respContent);
                }
                
                if (endpointInfo.method == HttpConfigMethod.POST)
                {
                    _stopWatch.Start();
                    // we want to check the payload data
                    HttpContent content = endpointInfo.payload;
                    var response = await _httpClient.PostAsync(endpointInfo.endpoint, content);
                    var respContent = await response.Content.ReadAsStringAsync();

                    _stopWatch.Stop();
                    await _responseStore.StoreResponse($"{Thread.CurrentThread.ManagedThreadId}_{endpointInfo.name}", respContent);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
