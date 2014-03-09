using GumartinM.JsonRPC4NET;
using System.Threading.Tasks;

namespace RemoteAgents.WindowsPhone.ViewModel
{
    public class ViewModelImpl
    {
        private static readonly string uri = "http://gumartinm.name/spring-mainapp/CurrentDateService.json";
        private static readonly string uriSetWriteText = "http://gumartinm.name/spring-mainapp/WriteTextService.json";
        private readonly JsonRpcHttpAsyncClient _remoteClient = new JsonRpcHttpAsyncClient();

        async public Task<string> GetCurrentDateAsync()
        {
            return await _remoteClient.PostRemoteServiceAsync<string>(uri, "getCurrentDate");
        }

        async public Task SetWriteTextAsync(params object[] parameters)
        {
            await _remoteClient.PostRemoteServiceAsync(uriSetWriteText, "setWriteText", parameters);
        }

    }
}
