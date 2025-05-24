using CommunityToolkit.Mvvm.ComponentModel;

namespace eStation_PTL_Demo_Avalonia.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _greeting = "Welcome to Avalonia!";
    }
}
