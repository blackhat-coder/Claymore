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
                    var resolver = new ClaymoreSyntaxResolver(_responseStore);
                    await _httpClient.AddRequestHeaders(resolver, endpointInfo.headers);

                    _stopWatch.Start();

                    var response = await _httpClient.GetAsync(endpointInfo.endpoint);
                    _stopWatch.Stop();

                    var respContent = await response.Content.ReadAsStringAsync();
                    await _responseStore.StoreResponseAsync($"{Thread.CurrentThread.ManagedThreadId}_{endpointInfo.name}", response.Headers.ToJsonString(), respContent);
                }
                
                if (endpointInfo.method == HttpConfigMethod.POST)
                {
                    var resolver = new ClaymoreSyntaxResolver(_responseStore);
                    
                    string stringPayload = Convert.ToString(endpointInfo.payload);
                    string payload = (await resolver.FindAndReplace(stringPayload)) ?? "";
                    StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                    await _httpClient.AddRequestHeaders(resolver, endpointInfo.headers);

                    _stopWatch.Start();
                    
                    var response = await _httpClient.PostAsync(endpointInfo.endpoint, content);

                    _stopWatch.Stop();

                    var respContent = await response.Content.ReadAsStringAsync();
                    await _responseStore.StoreResponseAsync($"{Thread.CurrentThread.ManagedThreadId}_{endpointInfo.name}", response.Headers.ToJsonString(), respContent);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
