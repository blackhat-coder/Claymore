using Claymore.Src.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Claymore.Src.Models;

public class JsonConfig
{
    public List<Task> tasks { get; set; }
}

public class Task
{
    public string name { get; set; }
    public int workers { get; set; }
    public string endpoint { get; set; }
    public int order { get; set; }
    public HttpConfigMethod method { get; set; }
    public List<Header> headers { get; set; }
    public dynamic payload { get; set; }
    public List<DependsOn> dependsOn { get; set; }
}

public class DependsOn
{
    public string name { get; set; }
    public string condition { get; set; }
}

public class Header
{
    public string key { get; set; }
    public string value { get; set; }
}