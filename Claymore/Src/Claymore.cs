using Claymore.Src.Enums;
using Claymore.Src.Helpers;
using Claymore.Src.Models;
using Claymore.Src.Persistence.Repository;
using Claymore.Src.Services.TextGeneration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src;

public class ClaymoreWorkers
{
    private readonly HttpClient _httpClient;
    private Stopwatch _stopWatch;
    private ILogger<ClaymoreWorkers> _logger;
    private readonly IGenericRepository<TaskResult> _taskRepository;
    private IDataGenerator _dataGenerator;

    private static int _workerCounter = 0;

    public ClaymoreWorkers(ILogger<ClaymoreWorkers> logger, IHttpClientFactory httpClientFactory, IGenericRepository<TaskResult> taskRepository
        , IDataGenerator dataGenerator)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _stopWatch = Stopwatch.StartNew();
        _dataGenerator = dataGenerator;
        _taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task Run()
    {
        try
        {
            _logger.LogInformation("Trying");

            var tasks = ConfigurationReader.Config.tasks;

            // Loop through the requests
            foreach(var task in tasks.OrderBy(x => x.order))
            {
                if (task.method == HttpConfigMethod.GET)
                {

                    var workerId = $"worker-{Interlocked.Increment(ref _workerCounter)}";

                    if (!(await ShouldProcessEndpoint(task)))
                        continue;

                    var resolver = new ClaymoreSyntaxResolver(_taskRepository, _dataGenerator);
                    await _httpClient.AddRequestHeaders(resolver, task.headers);

                    _stopWatch.Start();

                    var response = await _httpClient.GetAsync(task.endpoint);
                    _stopWatch.Stop();

                    var respContent = await response.Content.ReadAsStringAsync();

                    await _taskRepository.Add(new TaskResult
                    {
                        Id = Guid.NewGuid().ToString(),
                        WorkerId = workerId,
                        EndpointName = task.name,
                        Order = task.order,
                        ResponseHeader = response.Headers.ToJsonString(),
                        ResponseBody = respContent,
                        Success = response.IsSuccessStatusCode,
                        ElapsedTime = _stopWatch.ElapsedMilliseconds
                    });
                    await _taskRepository.SaveChanges();
                }
                
                if (task.method == HttpConfigMethod.POST)
                {
                    var workerId = $"worker-{Interlocked.Increment(ref _workerCounter)}";

                    if (!(await ShouldProcessEndpoint(task)))
                        continue;

                    var resolver = new ClaymoreSyntaxResolver(_taskRepository, _dataGenerator);
                    
                    string stringPayload = Convert.ToString(task.payload);
                    string payload = (await resolver.FindAndReplace(stringPayload)) ?? "";
                    StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                    await _httpClient.AddRequestHeaders(resolver, task.headers);

                    _stopWatch.Start();
                    
                    var response = await _httpClient.PostAsync(task.endpoint, content);

                    _stopWatch.Stop();

                    var respContent = await response.Content.ReadAsStringAsync();

                    await _taskRepository.Add(new TaskResult
                    {
                        Id = Guid.NewGuid().ToString(),
                        WorkerId = workerId,
                        EndpointName = task.name,
                        Order = task.order,
                        ResponseHeader = response.Headers.ToJsonString(),
                        ResponseBody = respContent,
                        Success = response.IsSuccessStatusCode,
                        ElapsedTime = _stopWatch.ElapsedMilliseconds
                    });
                    await _taskRepository.SaveChanges();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    /// <summary>
    /// Returns a boolean indicating if the task should be run by the worker
    /// Takes TaskInfo
    /// </summary>
    /// <returns></returns>
    private async Task<bool> ShouldProcessEndpoint(Models.Task task)
    {
        var response = new List<bool>();

        var dependsOn = task.dependsOn;

        foreach(var x in dependsOn)
        {
            bool successCondition = x.condition == ClaymoreConstants.Success ? true : false;
            var success = (await _taskRepository.GetFirstOrDefault(task => task.WorkerId == "" && task.EndpointName == x.name))?.Success;
            var sRun = success == successCondition;
            if (!sRun)
                response.Add(false);
        }

        return response.Count == 0;
    }
}
