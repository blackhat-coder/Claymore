using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Services.TextGeneration;

public interface IDataGenerator
{
    Task<string> GenerateName();
    Task<string> GenerateEmail();
    Task<string> GenerateString(int length);
    Task<int> GenerateNumber(int length);
}
