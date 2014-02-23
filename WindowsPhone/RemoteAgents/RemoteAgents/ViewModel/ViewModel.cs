using RemoteAgents.WindowsPhone.Model;
using System.Threading.Tasks;

namespace RemoteAgents.WindowsPhone.ViewModel
{
    public class ViewModelImpl
    {
        private static readonly string uri = "http://gumartinm.name/spring-mainapp/CurrentDateService.json";
        private readonly CallRemoteProcedure remoteProcedure = new CallRemoteProcedure();

        async public Task<string> getCurrentDate()
        {
            return await remoteProcedure.callRemoteService<string>(uri, "getCurrentDate");
        }
    }
}
