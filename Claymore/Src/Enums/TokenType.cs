using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Claymore.Src.Enums;

public enum TokenType
{
    ReplacementToken,
    BooleanToken,
    StringToken,
    NameToken,
    NumberToken,
    EmailToken
}