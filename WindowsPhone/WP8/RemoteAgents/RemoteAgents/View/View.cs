using RemoteAgents.WindowsPhone.ViewModel;
using System.Threading.Tasks;

namespace RemoteAgents.WindowsPhone.View
{
    public class ViewImpl
    {
        private readonly ViewModelImpl _vm = new ViewModelImpl();

        async public Task<string> GetCurrentDateAsync()
        {
            // Returning data in a diferent context. Upper layer decides.
            return await _vm.GetCurrentDateAsync().ConfigureAwait(false);
        }

        async public Task SetWriteTextAsync(string text, int number)
        {
            // Returning data in a diferent context. Upper layer decides.
            await _vm.SetWriteTextAsync(text, number).ConfigureAwait(false);
        }
    }
}
