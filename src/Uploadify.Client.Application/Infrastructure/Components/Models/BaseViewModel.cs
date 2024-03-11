using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Uploadify.Client.Application.Infrastructure.Components.Models;

public class BaseViewModel : INotifyPropertyChanged
{
    public delegate Task ViewModelAfterRenderHandler();
    public delegate Task ViewModelInitializedHandler();
    public delegate Task ViewModelParametersSetHandler();

    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetValue(ref _isBusy, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event ViewModelInitializedHandler? ViewModelInitialized;
    public event ViewModelAfterRenderHandler? ViewModelAfterRender;
    public event ViewModelParametersSetHandler? ViewModelParametersSet;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = default!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void SetValue<TItem>(ref TItem field, TItem value, [CallerMemberName] string propertyName = default!)
    {
        if (EqualityComparer<TItem>.Default.Equals(field, value))
        {
            return;
        }

        field = value;

        OnPropertyChanged(propertyName);
    }

    public Task OnViewModelInitialized()
    {
        return ViewModelInitialized?.Invoke() ?? Task.CompletedTask;
    }

    public Task OnViewModelAfterRender()
    {
        return ViewModelAfterRender?.Invoke() ?? Task.CompletedTask;
    }

    public Task OnViewModelParametersSet()
    {
        return ViewModelParametersSet?.Invoke() ?? Task.CompletedTask;
    }
}
