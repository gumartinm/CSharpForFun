using RemoteAgents.WindowsPhone.ViewModel;
using System.Threading.Tasks;

namespace RemoteAgents.WindowsPhone.View
{
    public class ViewImpl
    {
        private static readonly ViewModelImpl vm = new ViewModelImpl();

        async public Task<string> getCurrentDate()
        {
            return await vm.getCurrentDate();
        }
    }
}
