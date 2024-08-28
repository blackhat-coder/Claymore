using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Services.TextGeneration;

public interface ITextGenerator
{
    string GenerateText(string prompt);

    string FindGenerationTokens(string text);
}
