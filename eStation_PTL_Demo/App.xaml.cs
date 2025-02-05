using Serilog;
using System.Windows;

namespace eStation_PTL_Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Log, static configure
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("Logs/.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:HH:mm:ss.fff}[{Level:u1}]{Message} {NewLine}{Exception}",
                    retainedFileCountLimit: 10,
                    fileSizeLimitBytes: 1024 * 1024 * 512) // 256MB
                .CreateLogger();
            Log.Information("======================================");
            Log.Information("Start eStation.PTL.Demo...");
        }
    }
}