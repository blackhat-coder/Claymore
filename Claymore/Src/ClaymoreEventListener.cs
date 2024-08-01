using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src;

public class ClaymoreEventListener(ILogger<ClaymoreEventListener> _logger) : EventListener
{
    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        EnableEvents(eventSource, EventLevel.Informational);
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        _logger.LogInformation($"{DateTime.UtcNow:ss:fff} {eventData.ActivityId}: " +
            string.Join(' ', eventData.PayloadNames!.Zip(eventData.Payload!).Select(pair => $"{pair.First}={pair.Second}")));
    }
}
