using LandValueAnalysis.Common;

namespace LandValueAnalysis.Models.EventArgs;

public sealed class NavigationEventArgs : System.EventArgs
{
    public BaseViewModel ViewModel { get; }
    
    public NavigationEventArgs(BaseViewModel baseViewModel)
    {
        ViewModel = baseViewModel;
    }
}
