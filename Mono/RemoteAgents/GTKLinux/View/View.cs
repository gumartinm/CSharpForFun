using System.Threading.Tasks;
using Example.RemoteAgents.GTKLinux.ViewModel;


namespace Example.RemoteAgents.GTKLinux.View
{
  public class ViewImpl
  {
    private readonly ViewModelImpl _vm = new ViewModelImpl();

    async public Task<string> GetCurrentDateAsync()
    {
      return await _vm.GetCurrentDateAsync();
    }

    async public Task SetWriteTextAsync(params object[] parameters)
    {
      await _vm.SetWriteTextAsync(parameters);
    }
  }
}

