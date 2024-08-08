﻿using Claymore.Src.Models;
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
    /// Holds the name of the configuration file.
    /// </summary>
    private static string? _configFile;

    /// <summary>
    /// Stores the configuration object after the configuration file has been read.
    /// </summary>
    private static JsonConfig? _config;

    /// <summary>
    /// Logger instance for logging information, warnings, and errors.
    /// </summary>
    private static ILogger _logger;

    /// <summary>
    /// Stores the configuration object after the configuration file has been read.
    /// </summary>
    public static JsonConfig? Config 
    { 
        get { return _config; } 
        private set { }
    }

    /// <summary>
    /// Indicates whether the ConfigurationReader has been initialized with necessary configurations.
    /// </summary>
    private static bool _initialized = false;

    /// <summary>
    /// Gets or sets the path to the configuration file.
    /// </summary>
    public static string? ConfigFile { 
        get { return _configFile; } 
        set { _configFile = value; } 
    }

    /// <summary>
    /// Call this static method on ConfigurationReader and pass options [ConfigurationReaderOptions] to initialize ConfigurationReader
    /// ConfigurationReaderOptions contains a file and a logger.
    /// After the configFile is read, _initialized is set to True
    /// </summary>
    /// <param name="options"></param>
    public static void Init(Action<ConfigurationReaderOptions> options)
    {
        var readerOptions = new ConfigurationReaderOptions();
        options(readerOptions);

        _configFile = readerOptions.file;
        _logger = readerOptions.logger;

        _initialized = true;
    }

    /// <summary>
    /// Reads the configuration file and returns the configuration object.
    /// </summary>
    /// <returns>The configuration object deserialized from the configuration file.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the ConfigurationReader is not initialized or the configuration file is not found.</exception>
    /// <exception cref="Exception">Thrown when there is an error reading the configuration file.</exception>
    public static JsonConfig? Read()
    {
        try
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Plese call Init() to set neccessary configurations");
            }

            if (string.IsNullOrEmpty(_configFile))
                throw new InvalidOperationException("Configuration file path is not set.");

            JsonConfig config = new JsonConfig();

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());

            using (StreamReader reader = new StreamReader(_configFile))
            {
                string jsonConfig = reader.ReadToEnd();
                config = JsonSerializer.Deserialize<JsonConfig>(jsonConfig, options)!;
                _config = config;
            }

            _logger?.LogInformation("Successfully read Config File");
            return config;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error while reading the configuration file.");
            throw new Exception("Exception while reading configuration file", ex);
        }
    }

    /// <summary>
    /// Returns the number of endpoints in the configuration.
    /// </summary>
    /// <returns>The number of endpoints defined in the configuration.</returns>
    public static int GetEndpointCount()
    {
        if (_config == null)
            Read();

        return _config?.endpointsInfo.Count ?? 0;
    }

    /// <summary>
    /// Returns the list of endpoints defined in the configuration.
    /// </summary>
    /// <returns>An enumerable collection of endpoint URLs.</returns>
    public static IEnumerable<string> Endpoints()
    {
        if (_config == null)
            Read();

        return _config?.endpointsInfo.Select(e => e.endpoint).ToList() ?? Enumerable.Empty<string>();
    }
}

/// <summary>
/// Represents the options for initializing the ConfigurationReader.
/// </summary>
public class ConfigurationReaderOptions
{
    /// <summary>
    /// Gets or sets the path to the configuration file.
    /// </summary>
    public string file { get; set; }

    /// <summary>
    /// Gets or sets the logger instance for logging.
    /// </summary>
    public ILogger logger { get; set; }
}