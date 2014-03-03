using System;
using System.Threading.Tasks;
using GumartinM.JsonRPC4NET;
using System.ComponentModel;

namespace Example.RemoteAgents.GTKLinux.ViewModel
{
  public class ViewModelImpl
  {
    private static readonly string uri = "http://127.0.0.1:8080/spring-mainapp/CurrentDateService.json";
    private readonly JsonRpcHttpAsyncClient _remoteClient = new JsonRpcHttpAsyncClient(); 

    async public Task<string> GetCurrentDateAsync()
    {
      return await _remoteClient.PostRemoteServiceAsync<string>(uri, "getCurrentDate");
    }
  }
}

