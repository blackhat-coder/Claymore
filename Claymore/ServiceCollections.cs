using Claymore.Src;
using Claymore.Src.Services.ResponseStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Claymore;

public static class ServiceCollections
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddLogging(builder => builder.AddConsole());
        services.AddHttpClient();

        services.AddTransient<IResponseStore, FileResponseStore>();
        services.AddTransient<ClaymoreWorkers>();

        return services;
    }
}