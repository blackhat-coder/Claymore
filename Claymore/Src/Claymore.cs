using Claymore.Src.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src;

public class ClaymoreWorkers
{
    private readonly HttpClient _httpClient;
    private Stopwatch _stopWatch;

    public ClaymoreWorkers(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _stopWatch = new Stopwatch();
    }
    public async Task Run()
    {
        try
        {
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
            // Console.WriteLine the error or write the error to the error output;
            Console.WriteLine(ex.Message);
        }
    }
}
