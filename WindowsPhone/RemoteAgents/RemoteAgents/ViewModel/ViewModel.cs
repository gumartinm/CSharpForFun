using GumartinM.JsonRPC4Mono;
using System.Threading.Tasks;

namespace RemoteAgents.WindowsPhone.ViewModel
{
    public class ViewModelImpl
    {
        private static readonly string uri = "http://gumartinm.name/spring-mainapp/CurrentDateService.json";
        private readonly JsonRpcHttpAsyncClient _remoteProcedure = new JsonRpcHttpAsyncClient();

        async public Task<string> GetCurrentDateAsync()
        {
            return await _remoteProcedure.PostRemoteServiceAsync<string>(uri, "getCurrentDate");
        }
    }
}
