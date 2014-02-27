using RemoteAgents.WindowsPhone.ViewModel;
using System.Threading.Tasks;

namespace RemoteAgents.WindowsPhone.View
{
    public class ViewImpl
    {
        private readonly ViewModelImpl vm = new ViewModelImpl();

        async public Task<string> GetCurrentDateAsync()
        {
            return await vm.GetCurrentDateAsync();
        }
    }
}
