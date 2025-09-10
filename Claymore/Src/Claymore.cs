using Claymore.Src.Enums;
using Claymore.Src.Helpers;
using Claymore.Src.Models;
using Claymore.Src.Persistence;
using Claymore.Src.Persistence.Repository;
using Claymore.Src.Services.TextGeneration;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Claymore.Src;

public class ClaymoreWorkers
{
    private readonly HttpClient _httpClient;
    private ILogger<ClaymoreWorkers> _logger;
    private readonly IGenericRepository<TaskResult> _taskRepository;
    private readonly DataContextFactory _dataContextFactory;
    private IDataGenerator _dataGenerator;

    private static int _workerCounter = 0;

    private object _progressLockObj = new object();

    public ClaymoreWorkers(ILogger<ClaymoreWorkers> logger, IHttpClientFactory httpClientFactory, IGenericRepository<TaskResult> taskRepository
        , IDataGenerator dataGenerator, DataContextFactory dataContextFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _dataGenerator = dataGenerator;
        _taskRepository = taskRepository;
        _dataContextFactory = dataContextFactory;
    }

    public async System.Threading.Tasks.Task<Dictionary<string, List<TaskStatistic>>?> Run(ProgressContext ctx, Dictionary<string, ProgressTask?> progressTasks)
    {
        try
        {
            var tasks = ConfigurationReader.Config.tasks;

            // Loop through the requests
            foreach(var task in tasks.OrderBy(x => x.order))
            {
                progressTasks.TryGetValue(task.name, out var progressTask);

                int incrementBy = task.workers != 0 ? 100 / task.workers : 100;

                if (task.method == HttpConfigMethod.GET) {
                    List<Task> allTasks = new();

                    for(int i=0; i<task.workers; i++) {
                        allTasks.Add(ExectuteGet(task, new ProgressDisplayContext { incrementBy = incrementBy, progressTask = progressTask!, progressContext = ctx} ));
                    }

                    await Task.WhenAll(allTasks);
                }
                
                if (task.method == HttpConfigMethod.POST) {
                    List<Task> allTasks = new();

                    for(int i=0; i<task.workers; i++){
                        allTasks.Add(ExecutePost(task, new ProgressDisplayContext { incrementBy = incrementBy, progressTask = progressTask!, progressContext = ctx }));
                    }

                    await Task.WhenAll(allTasks);
                }

                if (task.method == HttpConfigMethod.PUT) {
                    List<Task> allTasks = new();
                    for(int i=0; i<task.workers; i++) {
                        allTasks.Add(ExectutePut(task, new ProgressDisplayContext { incrementBy = incrementBy, progressTask = progressTask!, progressContext = ctx }));
                    }

                    await Task.WhenAll(allTasks);
                }

                if (task.method == HttpConfigMethod.DELETE) {
                    List<Task> allTasks = new();

                    for(int i=0; i<task.workers; i++) {
                        allTasks.Add(ExecuteDelete(task, new ProgressDisplayContext { incrementBy = incrementBy, progressTask = progressTask!, progressContext = ctx }));
                    }
                }

                _workerCounter = 0;
            }

            var statistics = await CalculateTaskStatistics();
            return statistics;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return null;
        }
    }

    private async Task ExectuteGet(Models.Task task, ProgressDisplayContext progressDisplayContext)
    {
        using (var dbContext = await _dataContextFactory.CreateDbContextAsync())
        {
            var workerId = $"worker-{Interlocked.Increment(ref _workerCounter)}";

            if (!(await ShouldProcessEndpoint(task, workerId)))
                return;

            var resolver = new ClaymoreSyntaxResolver(_taskRepository, _dataGenerator);
            resolver.SetWorkerId(workerId);
            await _httpClient.AddRequestHeaders(resolver, task.headers);

            var endpoint = await resolver.FindAndReplace(task.endpoint);

            long startTime = Stopwatch.GetTimestamp();
            var response = await _httpClient.GetAsync(endpoint);
            var delta = Stopwatch.GetElapsedTime(startTime);

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
                ElapsedTime = delta.Milliseconds
            }, dbContext);
            await _taskRepository.SaveChanges(dbContext);
        }

        lock (_progressLockObj)
        {
            progressDisplayContext.progressTask.Increment(progressDisplayContext.incrementBy);
            progressDisplayContext.progressContext.Refresh();
        }
    }

    private async Task ExecutePost(Models.Task task, ProgressDisplayContext progressDisplayContext)
    {
        using (var dbContext = await _dataContextFactory.CreateDbContextAsync())
        {
            var workerId = $"worker-{Interlocked.Increment(ref _workerCounter)}";

            if (!(await ShouldProcessEndpoint(task, workerId)))
                return;

            var resolver = new ClaymoreSyntaxResolver(_taskRepository, _dataGenerator);
            resolver.SetWorkerId(workerId);

            string stringPayload = Convert.ToString(task.payload);
            string payload = (await resolver.FindAndReplace(stringPayload)) ?? "";
            StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            await _httpClient.AddRequestHeaders(resolver, task.headers);

            var endpoint = await resolver.FindAndReplace(task.endpoint);

            long startTime = Stopwatch.GetTimestamp();
            var response = await _httpClient.PostAsync(endpoint, content);
            var delta = Stopwatch.GetElapsedTime(startTime);

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
                ElapsedTime = delta.Milliseconds
            }, dbContext);
            await _taskRepository.SaveChanges(dbContext);
        }
        lock (_progressLockObj)
        {
            progressDisplayContext.progressTask.Increment(progressDisplayContext.incrementBy);
            progressDisplayContext.progressContext.Refresh();
        }
    }

    private async Task ExectutePut(Models.Task task, ProgressDisplayContext progressDisplayContext)
    {
        using (var dbContext = await _dataContextFactory.CreateDbContextAsync())
        {
            var workerId = $"worker-{Interlocked.Increment(ref _workerCounter)}";

            if (!(await ShouldProcessEndpoint(task, workerId)))
                return;

            var resolver = new ClaymoreSyntaxResolver(_taskRepository, _dataGenerator);
            resolver.SetWorkerId(workerId);

            string stringPayload = Convert.ToString(task.payload);
            string payload = (await resolver.FindAndReplace(stringPayload)) ?? "";
            StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            await _httpClient.AddRequestHeaders(resolver, task.headers);

            var endpoint = await resolver.FindAndReplace(task.endpoint);

            long startTIme = Stopwatch.GetTimestamp();
            var response = await _httpClient.PutAsync(endpoint, content);
            var delta = Stopwatch.GetElapsedTime(startTIme);

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
                ElapsedTime = delta.Milliseconds
            }, dbContext);
            await _taskRepository.SaveChanges(dbContext);
        }
        lock (_progressLockObj)
        {
            progressDisplayContext.progressTask.Increment(progressDisplayContext.incrementBy);
            progressDisplayContext.progressContext.Refresh();
        }
    }

    private async Task ExecuteDelete(Models.Task task, ProgressDisplayContext progressDisplayContext)
    {
        using (var dbContext = await _dataContextFactory.CreateDbContextAsync())
        {
            var workerId = $"worker-{Interlocked.Increment(ref _workerCounter)}";

            if (!(await ShouldProcessEndpoint(task, workerId)))
                return;

            var resolver = new ClaymoreSyntaxResolver(_taskRepository, _dataGenerator);
            resolver.SetWorkerId(workerId);
            await _httpClient.AddRequestHeaders(resolver, task.headers);

            var endpoint = await resolver.FindAndReplace(task.endpoint);

            long startTime = Stopwatch.GetTimestamp();
            var response = await _httpClient.DeleteAsync(endpoint);
            var delta = Stopwatch.GetElapsedTime(startTime);

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
                ElapsedTime = delta.Milliseconds
            }, dbContext);
            await _taskRepository.SaveChanges(dbContext);
        }
        lock (_progressLockObj)
        {
            progressDisplayContext.progressTask.Increment(progressDisplayContext.incrementBy);
            progressDisplayContext.progressContext.Refresh();
        }
    }

    /// <summary>
    /// Returns a boolean indicating if the task should be run by the worker
    /// Takes TaskInfo
    /// </summary>
    /// <returns></returns>
    private async Task<bool> ShouldProcessEndpoint(Models.Task task, string workerId)
    {
        var response = new List<bool>();

        var dependsOn = task.dependsOn;

        foreach(var x in dependsOn)
        {
            bool successCondition = x.condition == ClaymoreConstants.Success ? true : false;
            var success = (await _taskRepository.GetFirstOrDefault(task => task.WorkerId == workerId && task.EndpointName == x.name))?.Success;
            var sRun = success == successCondition;
            if (!sRun)
                response.Add(false);
        }

        return response.Count == 0;
    }

    private async Task<Dictionary<string, List<TaskStatistic>>> CalculateTaskStatistics()
    {
        var result = new Dictionary<string, List<TaskStatistic>>();
        var tasksConfigs = ConfigurationReader.Config.tasks;

        // Loop through the requests
        foreach (var task in tasksConfigs.OrderBy(x => x.order))
        {
            var taskStatistic = new List<TaskStatistic>();
            var tasks = await _taskRepository.GetAll(x => x.EndpointName == task.name);
            taskStatistic.Add(new TaskStatistic
            {
                status = ClaymoreConstants.Success,
                method = task.method.ToString(),
                endpoint = new Uri(task.endpoint).PathAndQuery,
                name = task.name,
                workers = tasks.Where(x => x.Success).Count(),
                mean = tasks.Where(x => x.Success).Select(x => x.ElapsedTime).DefaultIfEmpty(0).Average(),
                max = tasks.Where(x => x.Success).Select(x => x.ElapsedTime).DefaultIfEmpty(0).Max(),
                min = tasks.Where(x => x.Success).Select(x => x.ElapsedTime).DefaultIfEmpty(0).Min(),
            });
            taskStatistic.Add(new TaskStatistic
            {
                status = ClaymoreConstants.Failure,
                method = task.method.ToString(),
                endpoint = new Uri(task.endpoint).PathAndQuery,
                name = task.name,
                workers = tasks.Where(x => !x.Success).Count(),
                mean = tasks.Where(x => !x.Success).Select(x => x.ElapsedTime).DefaultIfEmpty(0).Average(),
                max = tasks.Where(x => !x.Success).Select(x => x.ElapsedTime).DefaultIfEmpty(0).Max(),
                min = tasks.Where(x => !x.Success).Select(x => x.ElapsedTime).DefaultIfEmpty(0).Min(),
            });

            result.Add(task.name, taskStatistic);
        }

        return result;
    }
}
