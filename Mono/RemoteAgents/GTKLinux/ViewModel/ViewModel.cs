using System;
using System.Threading.Tasks;
using Example.RemoteAgents.GTKLinux.Model;

namespace Example.RemoteAgents.GTKLinux.ViewModel
{
  public class ViewModelImpl
  {
    private static readonly string uri = "http://gumartinm.name/spring-mainapp/CurrentDateService.json";
    private static readonly CallRemoteProcedure remoteProcedure = new CallRemoteProcedure();

    async public Task<string> getCurrentDate()
    {
      return await remoteProcedure.callRemoteService<string>(uri, "getCurrentDate");
    }
  }
}

