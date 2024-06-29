using Claymore.Src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Claymore.Src;

public static class ConfigurationReader
{
    private static string? _configFile;
    private static JsonConfig? _config;
    public static JsonConfig? Config { get { return _config; } private set { } }
    public static string? ConfigFile { 
        get { 
            return _configFile; 
        } 
        set {
            _configFile = value;
        } 
    }

    public static JsonConfig? Read()
    {
        if(_configFile == null)
            throw new ArgumentNullException("Couldn't find config file");

        JsonConfig config = new JsonConfig();

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        using (StreamReader reader = new StreamReader(_configFile))
        {
            string jsonConfig = reader.ReadToEnd();
            config = JsonSerializer.Deserialize<JsonConfig>(jsonConfig, options);
            _config = config;
        }
        
        return config;
    }

    public static JsonConfig? Read(ConfigurationReaderOptions options)
    {
        if (string.IsNullOrEmpty(options.file))
            throw new ArgumentNullException("");

        _configFile = options.file;
        return Read();
    }

    public static int NumEndpoints()
    {
        if (_config == null)
            Read();

        return _config.endpointsInfo.Count;
    }

    public static IEnumerable<string> Endpoints()
    {
        if (_config == null)
            Read();

        return _config!.endpointsInfo.Select(e => e.endpoint).ToList();
    }
}

public class ConfigurationReaderOptions
{
    public string file { get; set; }
}