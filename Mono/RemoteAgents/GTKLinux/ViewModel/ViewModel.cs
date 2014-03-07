using System;
using System.Threading.Tasks;
using GumartinM.JsonRPC4NET;
using System.ComponentModel;

namespace Example.RemoteAgents.GTKLinux.ViewModel
{
  public class ViewModelImpl
  {
    private static readonly string uriGetCurrentDate = "http://127.0.0.1:8080/spring-mainapp/CurrentDateService.json";
    private static readonly string uriSetWriteText = "http://127.0.0.1:8080/spring-mainapp/WriteTextService.json";
    private readonly JsonRpcHttpAsyncClient _remoteClient = new JsonRpcHttpAsyncClient(); 

    async public Task<string> GetCurrentDateAsync()
    {
      return await _remoteClient.PostRemoteServiceAsync<string>(uriGetCurrentDate, "getCurrentDate");
    }

    async public Task SetWriteTextAsync(params object[] parameters)
    {
      await _remoteClient.PostRemoteServiceAsync(uriSetWriteText, "setWriteText", parameters);
    }
  }
}

