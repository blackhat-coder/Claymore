// See https://aka.ms/new-console-template for more information
using Claymore.Src;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Net.Http.Json;

var httpClient = new HttpClient();
string filename = "C:\\Users\\aboh.israel\\Documents\\Codes\\Claymore\\Claymore\\claymore.json";

var config = ConfigurationReader.Read(new ConfigurationReaderOptions { file = filename });

var workers = new ClaymoreWorkers(httpClient);

await workers.Run();