using Claymore.Src.Models;
using Microsoft.Extensions.Logging;
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
    /// <summary>
    /// 
    /// </summary>
    private static string? _configFile;

    /// <summary>
    /// 
    /// </summary>
    private static JsonConfig? _config;

    /// <summary>
    /// 
    /// </summary>
    private static ILogger _logger;

    /// <summary>
    /// 
    /// </summary>
    public static JsonConfig? Config { get { return _config; } private set { } }

    /// <summary>
    /// Set to True if ConfigurationReader has _configFile & _logger
    /// </summary>
    private static bool _initialized = false;

    public static string? ConfigFile { 
        get { 
            return _configFile; 
        } 
        set {
            _configFile = value;
        } 
    }

    public static void Init(Action<ConfigurationReaderOptions> options)
    {
        var readerOptions = new ConfigurationReaderOptions();
        options(readerOptions);

        _configFile = readerOptions.file;
        _logger = readerOptions.logger;

        _initialized = true;
    }

    public static JsonConfig? Read()
    {
        try
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Plese call Init() to set neccessary configurations");
            }

            if (_configFile == null)
                throw new InvalidOperationException("Couldn't find config file");

            JsonConfig config = new JsonConfig();

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());

            using (StreamReader reader = new StreamReader(_configFile))
            {
                string jsonConfig = reader.ReadToEnd();
                config = JsonSerializer.Deserialize<JsonConfig>(jsonConfig, options);
                _config = config;
            }

            _logger.LogInformation("Successfully read Config File");
            return config;
        }
        catch (Exception ex)
        {
            throw new Exception("Exception while Reading Config File", ex);
        }
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
    public ILogger logger { get; set; }
}