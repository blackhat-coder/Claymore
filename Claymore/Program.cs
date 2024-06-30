// See https://aka.ms/new-console-template for more information
using Claymore;
using Claymore.Src;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO.Enumeration;
using System.Net.Http.Json;


// App startup
var serviceCollection = new ServiceCollection();
var services = serviceCollection.AddAppServices();

var serviceProvider = services.BuildServiceProvider();

var httpClient = new HttpClient();
string filename = "C:\\Users\\aboh.israel\\Documents\\Codes\\Claymore\\Claymore\\claymore.json";

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var workers = serviceProvider.GetRequiredService<ClaymoreWorkers>();

ConfigurationReader.Init(options => { options.file = filename; options.logger = logger; });
var config = ConfigurationReader.Read();

await workers.Run();