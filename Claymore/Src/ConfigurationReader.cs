using Claymore.Src.Helpers;
using Claymore.Src.Models;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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

    private static StatusContext _ctx { get; set; }

    /// <summary>
    /// Call this static method on ConfigurationReader and pass options [ConfigurationReaderOptions] to initialize ConfigurationReader
    /// ConfigurationReaderOptions contains a file and a logger.
    /// After the configFile is read, _initialized is set to True
    /// </summary>
    /// <param name="options"></param>
    public static void Init(Action<ConfigurationReaderOptions> options, StatusContext ctx)
    {
        var readerOptions = new ConfigurationReaderOptions();
        options(readerOptions);

        _configFile = readerOptions.file;
        _logger = readerOptions.logger;

        _initialized = true;
        _ctx = ctx;
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

            AnsiConsole.MarkupLine("LOG: Read config file [bold green]success[/]...");

            _ctx.Status("[bold khaki3]Validating...[/]");
            _ctx.Spinner(Spinner.Known.SimpleDotsScrolling);
            AnsiConsole.MarkupLine("LOG: Validating config file...");

            // Performs validation on the configuration
            Thread.Sleep(1000);
            ValidateConfig();

            _ctx.Status("[bold green]Done![/]");
            AnsiConsole.MarkupLine("LOG: Validation [bold green]success![/]");
            return config;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return null;
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

        return _config?.tasks.Count ?? 0;
    }

    /// <summary>
    /// Returns the list of endpoints defined in the configuration.
    /// </summary>
    /// <returns>An enumerable collection of endpoint URLs.</returns>
    public static IEnumerable<string> Endpoints()
    {
        if (_config == null)
            Read();

        return _config?.tasks.Select(e => e.endpoint).ToList() ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// Validates the configuration to ensure that each dependency listed in the "dependsOn" field 
    /// corresponds to an existing endpoint name.
    /// </summary>
    /// <exception cref="Exception">Thrown when a dependency references a non-existent endpoint name.</exception>
    private static void ValidateConfig()
    {
        if (Config == null)
            return;

        List<string> names = _config.tasks.Select(e => e.name).ToList();

        // Check distinct names
        if(names.Distinct().Count() != names.Count)
        {
            string errorMessage = $"Duplicate name found";
            _logger?.LogError(errorMessage);
            throw new Exception(errorMessage);
        }

        // Check Depends On
        foreach (var requestInfo in Config?.tasks)
        {
            foreach(var dependsOn in requestInfo.dependsOn)
            {
                if (!names.Contains(dependsOn.name))
                {
                    string errorMesage = $"Syntax Error ({_configFile}): {requestInfo.name} depends on: {dependsOn} Not Found.";
                    _logger?.LogError(errorMesage);
                    throw new Exception(errorMesage);
                }

                if(dependsOn.condition != ClaymoreConstants.Success && dependsOn.condition != ClaymoreConstants.Error)
                {
                    string errorMessage = $"Unable to parse dependsOn condition: {dependsOn.condition}; condition should either be {ClaymoreConstants.Success} or {ClaymoreConstants.Error}";
                    _logger?.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }
            }
        }

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