using Claymore.Src.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Services.ResponseStore;

public class DataStore : IResponseStore
{
    private DataContext _dataContext { get; set; }

    public DataStore(DataContext dataContext) {
        _dataContext = dataContext;
    }

    public Task StoreTaskAsync(string workerId, string headers, object response, bool success, int elapsedTime)
    {



        return Task.CompletedTask;
    }

    public Task<string> GetAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetResponseBodyAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetResponseHeaderAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GetTaskStatus(string id)
    {
        throw new NotImplementedException();
    }

    public Task StoreResponseAsync(string id, string headers, object response, bool success)
    {
        throw new NotImplementedException();
    }

    public Task StoreResponseAsync(string id, string headers, string response, bool success)
    {
        throw new NotImplementedException();
    }
}
