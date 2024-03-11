using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Uploadify.Client.Application.Infrastructure.Components.Models;

public class BasePageComponent<TViewModel> : ComponentBase, IDisposable where TViewModel : BaseViewModel
{
    public readonly CancellationTokenSource CancellationTokenSource = new();

    [Inject] public required TViewModel Model { get; set; }
    [Inject] public required IStringLocalizer Localizer { get; set; }

    protected override bool ShouldRender()
    {
        return !Model.IsBusy;
    }

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += OnModelPropertyChanged;
        return Model.OnViewModelInitialized();
    }

    protected override Task OnParametersSetAsync()
    {
        return Model.OnViewModelParametersSet();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            return Model.OnViewModelAfterRender();
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private async void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        await InvokeAsync(StateHasChanged);
    }

    void IDisposable.Dispose()
    {
        Model.PropertyChanged -= OnModelPropertyChanged;
    }
}
