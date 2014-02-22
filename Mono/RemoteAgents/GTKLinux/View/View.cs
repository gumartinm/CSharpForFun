using System;
using System.Threading.Tasks;
using Example.RemoteAgents.GTKLinux.ViewModel;


namespace Example.RemoteAgents.GTKLinux.View
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

