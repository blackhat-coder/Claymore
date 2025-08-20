using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Models;

/// <summary>
/// A task represents a unit of work done by a worker. eg. Hitting a particular endpoint
/// </summary>
public class TaskResult
{
    public string Id { get; set; }
    public string WorkerId { get; set; }
    public string EndpointName { get; set; }
    public int Order { get; set; }
    public string? ResponseHeader { get; set; }
    public string? ResponseBody { get; set; }
    public bool Success { get; set; }
    public long ElapsedTime { get; set; }
}