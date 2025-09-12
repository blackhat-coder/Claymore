using Claymore.Src.Enums;
using Claymore.Src.Models;
using Claymore.Src.Persistence.Repository;
using Claymore.Src.Services.TextGeneration;
using Claymore.Src.Utils;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Claymore.Src.Helpers;

public class ClaymoreSyntaxResolver(IGenericRepository<TaskResult> _taskRepository, IDataGenerator _dataGenerator)
{
    private string? _workerId { get; set; }

    private readonly SyntaxValidator _syntaxValidator = new SyntaxValidator();

    /// <summary>
    /// Sets the worker Id for this instance
    /// </summary>
    /// <param name="workerId"></param>
    public void SetWorkerId(string workerId)
    {
        _workerId = workerId;
    }

    /// <summary>
    /// Takes the input string, checks for a valid syntax, and replaces the placeholder with
    /// the corresponding value from the stored response
    /// </summary>
    /// <param name="input">The string containing the syntax placeholder to be replaced</param>
    /// <returns>The modified string with the placeholder replaced by the actual value, or null if not found/invalid</returns>
    public async Task<string?> FindAndReplace(string input)
    {
        var result = input;
        var tokenPatterns = RegexPatterns.GetAllPatterns();

        foreach (var pattern in tokenPatterns)
        {
            var matchesCount = pattern.Matches(input).Count;

            for(int i=0; i<matchesCount; i++)
            //foreach(Match match in matches)
            {
                var matches = pattern.Matches(result);
                var match = matches[(matchesCount - 1) - i];

                string token = match.ToString();
                string? replacement = null;

                if (pattern == RegexPatterns.ReplacementPattern)
                {
                    replacement = await ResolveReplacementToken(token);
                }
                else if (pattern == RegexPatterns.StringTokenPattern)
                {
                    replacement = await ResolveStringToken(token);
                }
                else if (pattern == RegexPatterns.NameTokenPattern)
                {
                    replacement = await ResolveNameToken(token);
                }
                else if (pattern == RegexPatterns.NumberTokenPattern)
                {
                    replacement = await ResolveNumberToken(token);
                }
                else if (pattern == RegexPatterns.EmailTokenPattern)
                {
                    replacement = await ResolveEmailToken(token);
                }else if (pattern == RegexPatterns.BooleanTokenPattern)
                {
                    replacement = await ResolveBooleanToken(token);
                }

                if (replacement != null)
                {
                    result = result.ReplaceUsingIndex(match.Index, token.Length, replacement);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Resolves and replaces replacement tokens in the input string.
    /// </summary>
    /// <param name="syntaxValidator"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task<string?> ResolveReplacementToken(string input)
    {
        // Parse the input to extract name, part and property
        (string name, string part, string property)? parsedResponse = _syntaxValidator.ParseReplacementToken(input);
        if (parsedResponse == null) { return null; }

        if (!string.IsNullOrEmpty(parsedResponse?.name))
        {
            string name = parsedResponse?.name!;
            // Use the current thread Id to build the search name

            // Handle ResponseBody replacement
            if (parsedResponse?.part == ClaymoreConstants.ResponseBody)
            {
                string? storedResponse = (await _taskRepository.GetFirstOrDefault(x => x.EndpointName == name && x.WorkerId == _workerId))?.ResponseBody;
                // parse the body and get the preperty parsedResponse.property
                if (storedResponse != null && parsedResponse?.property != null)
                {
                    var propertyValue = GetValueFromJson(storedResponse, parsedResponse?.property);

                    if (propertyValue != null)
                    {
                        // replace some text in input with the propertyValue
                        string newresponse = Regex.Replace(input, _syntaxValidator.RegexValidatorPattern, propertyValue);
                        return newresponse;
                    }
                }
            }

            // Handle ResponseHeader replacement
            if (parsedResponse?.part == ClaymoreConstants.ResponseHeader)
            {
                string? headerResponse = (await _taskRepository.GetFirstOrDefault(x => x.EndpointName == name && x.WorkerId == _workerId))?.ResponseHeader;

                if (headerResponse != null && parsedResponse?.property != null)
                {
                    string? headerValue = GetValueFromJson(headerResponse, parsedResponse?.property);

                    if (headerValue != null)
                    {
                        string newResponse = Regex.Replace(input, _syntaxValidator.RegexValidatorPattern, headerValue);
                        return newResponse;
                    }
                }
            }
        }

        return null;
    }


    /// <summary>
    /// Resolves and replaces boolean tokens in the input string.
    /// </summary>
    private async Task<string?> ResolveBooleanToken(string input)
    {
        var parsedResponse = _syntaxValidator.ParseBooleanToken(input);
        if(parsedResponse == null) { return null; }

        var boolean = (new Random().Next(2) == 0).ToString();
        return Regex.Replace(input, RegexPatterns.BooleanTokenPattern.ToString(), boolean);
    }

    /// <summary>
    /// Resolves and replaces string tokens in the input string.
    /// </summary>
    private async Task<string?> ResolveStringToken(string input)
    {
        // call TextGenerator
        (string token, int length)? parsedResponse = _syntaxValidator.ParseStringToken(input);
        if (parsedResponse == null) { return null; }
        
        int len = parsedResponse?.length ?? 0;
        string generatedString = await _dataGenerator.GenerateString(len);
        return Regex.Replace(input, RegexPatterns.StringTokenPattern.ToString(), generatedString);
    }

    /// <summary>
    /// Resolves and replaces name tokens in the input string.
    /// </summary>
    private async Task<string?> ResolveNameToken(string input)
    {
        string generatedName = await _dataGenerator.GenerateName();
        return Regex.Replace(input, RegexPatterns.NameTokenPattern.ToString(), generatedName);
    }

    /// <summary>
    /// Resolves and replaces number tokens in the input string.
    /// </summary>
    private async Task<string?> ResolveNumberToken(string input)
    {
        (string token, int length)? parsedResponse = _syntaxValidator.ParseNumberToken(input);
        if (parsedResponse == null) { return null; }

        int len = parsedResponse?.length ?? 0;
        int generatedNumber = await _dataGenerator.GenerateNumber(len);
        return Regex.Replace(input, RegexPatterns.NumberTokenPattern.ToString(), generatedNumber.ToString());
    }

    /// <summary>
    /// Resolves and replaces email tokens in the input string.
    /// </summary>
    private async Task<string?> ResolveEmailToken(string input)
    {
        string generatedEmail = await _dataGenerator.GenerateEmail();
        string newResponse = Regex.Replace(input, RegexPatterns.EmailTokenPattern.ToString(), generatedEmail);
        return newResponse;
    }


    /// <summary>
    /// Extracts the value of a specified property from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <param name="property">The name of the property to extract.</param>
    /// <returns>The value of the specified property, or null if not found.</returns>
    private string? GetValueFromJson(string json, string property)
    {
        try
        {
            var jsonObject = JObject.Parse(json);
            var token = jsonObject.SelectToken($"$..{property}");

            return token?.ToString();
        }
        catch (Exception ex) {
            return null;
        }
    }
}


public class SyntaxValidator
{
    private static readonly Regex pattern = RegexPatterns.ReplacementPattern;

    public string RegexValidatorPattern { get { return pattern.ToString(); } }

    /// <summary>
    /// Validates the syntax of the input string and identifies the token type.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public (bool, TokenType) ValidateSyntax(string input)
    {
        if (pattern.IsMatch(input))
        {
            return (true, TokenType.ReplacementToken);
        }else if (RegexPatterns.StringTokenPattern.IsMatch(input))
        {
            return (true, TokenType.StringToken);
        }else if (RegexPatterns.NameTokenPattern.IsMatch(input))
        {
            return (true, TokenType.NameToken);
        }else if (RegexPatterns.NumberTokenPattern.IsMatch(input))
        {
            return (true, TokenType.NumberToken);
        }else if (RegexPatterns.EmailTokenPattern.IsMatch(input))
        {
            return (true, TokenType.EmailToken);
        }

        return (false, default);
    }

    public (string name, string part, string property)? ParseReplacementToken(string input)
    {
        var match = pattern.Match(input);
        if (match.Success)
        {
            var name = match.Groups["name"].Value;
            var part = match.Groups["part"].Value;
            var property = match.Groups["property"].Value;

            return (name, part, property);
        }

        return null;
    }

    public string? ParseBooleanToken(string input)
    {
        var match = RegexPatterns.BooleanTokenPattern.Match(input);
        if (match.Success)
        {
            var token = match.Groups[ClaymoreConstants.token].Value;
            return token;
        }

        return null;
    }

    public (string token, int length)? ParseStringToken(string input)
    {
        var match = RegexPatterns.StringTokenPattern.Match(input);
        if (match.Success)
        {
            var token = match.Groups[ClaymoreConstants.token].Value;
            var length = match.Groups[ClaymoreConstants.length].Value;
            return (token, int.Parse(length));
        }

        return null;
    }

    public string? ParseNameToken(string input)
    {
        var match = RegexPatterns.NameTokenPattern.Match(input);
        if (match.Success)
        {
            var token = match.Groups[ClaymoreConstants.token].Value;
            return token;
        }

        return null;
    }

    public (string token, int length)? ParseNumberToken(string input)
    {
        var match = RegexPatterns.NumberTokenPattern.Match(input);
        if (match.Success)
        {
            var token = match.Groups[ClaymoreConstants.token].Value;
            var length = match.Groups[ClaymoreConstants.length].Value;
            return (token, int.Parse(length));
        }

        return null;
    }

    public string? ParseEmailToken(string input)
    {
        var match = RegexPatterns.EmailTokenPattern.Match(input);
        if (match.Success)
        {
            var token = match.Groups[ClaymoreConstants.token].Value;
            return token;
        }

        return null;
    }
}