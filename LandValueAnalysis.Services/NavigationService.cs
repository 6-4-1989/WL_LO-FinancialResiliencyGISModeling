using LandValueAnalysis.Common;
using LandValueAnalysis.Models.EventArgs;
using Microsoft.Extensions.DependencyInjection;

namespace LandValueAnalysis.Services;

public sealed class NavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public event EventHandler<NavigationEventArgs> OnPageChanged;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Navigate<TViewModel>() where TViewModel : BaseViewModel
    {
        BaseViewModel viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        OnPageChanged?.Invoke(this, new NavigationEventArgs(viewModel));
    }
}
