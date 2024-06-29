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
    public List<EndpointInfo> endpointsInfo { get; set; }
}

public class EndpointInfo
{
    public string endpoint { get; set; }
    public int priority { get; set; }
    public HttpConfigMethod method { get; set; }
    public List<Header> headers { get; set; }
    public string payload { get; set; }
}

public class Header
{
    public string key { get; set; }
}