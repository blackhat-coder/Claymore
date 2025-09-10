using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Models;

public class TaskStatistic
{
    public string status { get; set; }
    public string method { get; set; }
    public string endpoint { get; set; }
    public string name { get; set; }
    public double workers { get; set; }
    public double mean { get; set; }
    public double max { get; set; }
    public double min { get; set; }
}
