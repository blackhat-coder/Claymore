using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Models;

public class ProgressDisplayContext
{
    public ProgressContext progressContext { get; set; }
    public ProgressTask progressTask { get; set; }
    public int incrementBy { get; set; }
}
