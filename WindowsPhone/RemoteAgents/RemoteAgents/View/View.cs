using RemoteAgents.WindowsPhone.ViewModel;
using System.Threading.Tasks;

namespace RemoteAgents.WindowsPhone.View
{
    public class ViewImpl
    {
        private readonly ViewModelImpl _vm = new ViewModelImpl();

        async public Task<string> GetCurrentDateAsync()
        {
            return await _vm.GetCurrentDateAsync();
        }
    }
}
