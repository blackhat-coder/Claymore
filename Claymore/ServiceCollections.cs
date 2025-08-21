using Claymore.Src;
using Claymore.Src.Persistence;
using Claymore.Src.Persistence.Repository;
using Claymore.Src.Services.TextGeneration;
using Microsoft.EntityFrameworkCore;
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
        services.AddPooledDbContextFactory<DataContext>(
            options => options.UseSqlite("Data Source=C:\\Users\\aboh.israel\\OneDrive\\Documents\\Codes\\Claymore\\db\\Claymore.db"));
        services.AddScoped<DataContextFactory>();

        services.AddLogging(builder => builder.AddConsole());
        services.AddHttpClient();

        services.AddScoped<IDataGenerator, DataGenerator>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<ClaymoreWorkers>();

        return services;
    }
}