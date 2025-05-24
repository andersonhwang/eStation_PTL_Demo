using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;

namespace eStation_PTL_Demo_Avalonia.Android
{
    [Activity(
        Label = "eStation_PTL_Demo_Avalonia.Android",
        Theme = "@style/MyTheme.NoActionBar",
        Icon = "@drawable/icon",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
    public class MainActivity : AvaloniaMainActivity<App>
    {
        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            return base.CustomizeAppBuilder(builder)
                .WithInterFont();
        }
    }
}
