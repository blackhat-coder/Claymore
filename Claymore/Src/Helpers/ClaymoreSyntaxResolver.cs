using Claymore.Src.Services.ResponseStore;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Claymore.Src.Helpers;

public class ClaymoreSyntaxResolver(IResponseStore _responseStore)
{

    /// <summary>
    /// Takes the input string, checks for a valid syntax, and replaces the placeholder with
    /// the corresponding value from the stored response
    /// </summary>
    /// <param name="input">The string containing the syntax placeholder to be replaced</param>
    /// <returns>The modified string with the placeholder replaced by the actual value, or null if not found/invalid</returns>
    public async Task<string?> FindAndReplace(string input)
    {
        var syntaxValidator = new SyntaxValidator();

        if (syntaxValidator.ValidateSyntax(input))
        {
            // Parse the input to extract name, part and property
            (string name, string part, string property)? parsedResponse = syntaxValidator.ParseSyntax(input);
            if (parsedResponse == null) { return null; }

            if (!string.IsNullOrEmpty(parsedResponse?.name))
            {
                // Use the current thread Id to build the search name
                string searchname = $"{Thread.CurrentThread.ManagedThreadId}_{parsedResponse?.name}";

                // Handle ResponseBody replacement
                if(parsedResponse?.part == ClaymoreConstants.ResponseBody)
                {
                    var storedResponse = await _responseStore.GetResponseBodyAsync(searchname);
                    // parse the body and get the preperty parsedResponse.property
                    if (storedResponse != null && parsedResponse?.property != null)
                    {
                        var propertyValue = GetValueFromJson(storedResponse, parsedResponse?.property);

                        if (propertyValue != null) {
                            // replace some text in input with the propertyValue
                            var newresponse = Regex.Replace(input, syntaxValidator.RegexValidatorPattern, propertyValue);
                            return newresponse;
                        }
                    }
                }
                
                // Handle ResponseHeader replacement
                if(parsedResponse?.part == ClaymoreConstants.ResponseHeader)
                {
                    var headerResponse = await _responseStore.GetResponseHeaderAsync(searchname);

                    if (headerResponse != null && parsedResponse?.property != null)
                    {
                        var headerValue = GetValueFromJson(headerResponse, parsedResponse?.property);

                        if (headerValue != null)
                        {
                            var newResponse = Regex.Replace(input, syntaxValidator.RegexValidatorPattern, headerValue);
                            return newResponse;
                        }
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Extracts the value of a specified property from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <param name="property">The name of the property to extract.</param>
    /// <returns>The value of the specified property, or null if not found.</returns>
    private string? GetValueFromJson(string json, string property)
    {
        var jsonObject = JObject.Parse(json);
        var token = jsonObject.SelectToken($"$..{property}");

        return token?.ToString();
    }
}


public class SyntaxValidator
{
    private static readonly Regex pattern = new Regex(@"\$(?<name>[a-zA-Z0-9_]+)\.(?<part>ResponseBody|ResponseHeader)\.(?<property>[a-zA-Z0-9_]+)");

    public string RegexValidatorPattern { get { return pattern.ToString(); } }

    public bool ValidateSyntax(string input)
    {
        return pattern.IsMatch(input);
    }

    public (string name, string part, string property)? ParseSyntax(string input)
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
}