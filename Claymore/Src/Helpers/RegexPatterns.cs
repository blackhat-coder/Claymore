using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Claymore.Src.Helpers;

public static class RegexPatterns
{
    /// REPLACEMENT TOKENS

    /// <summary>
    /// Matches a placeholder with format: $name.ResponseBody.property or $name.ResponseHeader.property
    /// </summary>
    private static readonly Regex _replacementPattern = new Regex(@"\$(?<name>[a-zA-Z0-9_]+)\.(?<part>ResponseBody|ResponseHeader)\.(?<property>[a-zA-Z0-9_]+)");
    public static Regex ReplacementPattern { get { return _replacementPattern; } }


    /// GENERATION TOKENS

    /// <summary>
    /// Matches a boolean token placeholder: $bool
    /// </summary>
    private static readonly Regex _booleanTokenPattern = new Regex(@"(?<token>\$bool)", RegexOptions.Compiled);
    public static Regex BooleanTokenPattern { get { return _booleanTokenPattern; } }

    /// <summary>
    /// Matches a string token placeholder: $string
    /// </summary>
    private static readonly Regex _stringTokenPattern = new Regex(@"(?<token>\$string)\[(?<length>\d+)\]", RegexOptions.Compiled);
    public static Regex StringTokenPattern {  get { return _stringTokenPattern; } }

    /// <summary>
    /// Matches a name token placeholder: $name
    /// </summary>
    private static readonly Regex _nameTokenPattern = new Regex(@"(?<token>(?<=^|\s)\$name(?=\s|$))", RegexOptions.Compiled);
    public static Regex NameTokenPattern {  get { return _nameTokenPattern; } }

    /// <summary>
    /// Matches a number token placeholder with length: $number[length]
    /// </summary>
    private static readonly Regex _numberTokenPattern = new Regex(@"(?<token>\$number)\[(?<length>\d+)\]", RegexOptions.Compiled);
    public static Regex NumberTokenPattern {  get { return _numberTokenPattern; } }

    /// <summary>
    /// Matches an email address token placeholder: $email
    /// </summary>
    private static readonly Regex _emailTokenPattern = new Regex(@"(?<token>(?<=^|\s)\$email(?=\s|$))", RegexOptions.Compiled);
    public static Regex EmailTokenPattern { get {return _emailTokenPattern; } }

    public static List<Regex> GetAllPatterns()
    {
        return new List<Regex>
        {
            _replacementPattern,
            _booleanTokenPattern,
            _stringTokenPattern,
            _numberTokenPattern,
            _emailTokenPattern,
            _nameTokenPattern,
        };
    }
}
