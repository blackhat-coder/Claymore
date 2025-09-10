// See https://aka.ms/new-console-template for more information
using Claymore;
using Claymore.Src;
using Claymore.Src.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO.Enumeration;
using System.Net.Http.Json;


AnsiConsole.Write(new FigletText("Claymore").Centered().Color(Color.Green));

// App startup
var serviceCollection = new ServiceCollection();
var services = serviceCollection.AddAppServices();

var serviceProvider = services.BuildServiceProvider();

var httpClient = new HttpClient();

var filename = AnsiConsole.Prompt(new TextPrompt<string>("Please enter configuration filepath [bold green](.json)[/]: "));

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var workers = serviceProvider.GetRequiredService<ClaymoreWorkers>();


AnsiConsole.Status()
    .Spinner(Spinner.Known.OrangePulse)
    .Start("Reading config...", ctx =>
{
    AnsiConsole.MarkupLine("LOG: Reading config file...");
    ConfigurationReader.Init(options => { options.file = filename; options.logger = logger; }, ctx);

    Thread.Sleep(1000);
    var config = ConfigurationReader.Read();
});

Dictionary<string, List<TaskStatistic>>? resultStatistics = null;

AnsiConsole.MarkupLine("");
await AnsiConsole.Progress().StartAsync(async ctx =>
{
    var progressTasks = AddTasksToProgressContext(ctx, ConfigurationReader.Config.tasks);

    resultStatistics = await workers.Run(ctx, progressTasks);
});

DisplayStatistics(resultStatistics);


static Dictionary<string, ProgressTask?> AddTasksToProgressContext(ProgressContext progressContext, List<Claymore.Src.Models.Task> taskList)
{
    var dict = new Dictionary<string, ProgressTask?>();
    foreach(var task in taskList) {

        var url = new Uri(task.endpoint);

        string displayText = $"[green] {task.method} {url.PathAndQuery} [[workers:{task.workers}]][/]";

        var progressTask = progressContext.AddTask(displayText);

        dict.Add(task.name, progressTask);
    }
    return dict;
}

void DisplayStatistics(Dictionary<string, List<TaskStatistic>>? statistics)
{
    if (statistics != null)
    {
        foreach (var stat in statistics)
        {
            var table = new Table();
            table.AddColumn("Status");
            table.AddColumn("Endpoint");
            table.AddColumn("Name");
            table.AddColumn("Workers");
            table.AddColumn("Mean (ms)");
            table.AddColumn("Max (ms)");
            table.AddColumn("Min (ms)");
            foreach (var s in stat.Value)
            {
                table.AddRow(s.status, $"{s.method} {s.endpoint}", s.name, s.workers.ToString(), s.mean.ToString("F2"), s.max.ToString(), s.min.ToString());
            }
            AnsiConsole.Write(table);
        }
    }
}