using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Helper;
using Microsoft.Win32;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace eStation_PTL_Demo
{
    /// <summary>
    /// Interaction logic for LoadFirmware.xaml
    /// </summary>
    public partial class FirmwareWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FirmwareWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        /// <summary>
        /// Button OTA click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnOTA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = txtFirmware.Text.Trim();
                var version = txtVersion.Text.Trim();

                if (string.IsNullOrEmpty(path))
                {
                    MsgHelper.Error("Firmware_Is_Mandatory");
                    return;
                }

                if (string.IsNullOrEmpty(version))
                {
                    MsgHelper.Error("Version_Is_Mandatory");
                    return;
                }

                if (!File.Exists(path))
                {
                    MsgHelper.Error("Firmware_Not_Exist:" + path);
                    return;
                }

                if (!int.TryParse(version, out int result) || result == 0)
                {
                    MsgHelper.Error("Invalid_Version:" + version);
                    return;
                }

                var firmware = File.ReadAllBytes(path);
                var ota = new OTA
                {
                    Type = (rBtnAP.IsChecked ?? false) ? 0 : 1,
                    Firmware = firmware,
                    Version = result,
                    MD5 = FileHelper.GetBytesMd5(firmware)
                };

                await SendService.Instance.Send(ota);
                Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "OTA_ERR");
            }
        }

        /// <summary>
        /// Button cancel click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// Button browse click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    RestoreDirectory = true,
                    DefaultDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                    DefaultExt = (rBtnAP.IsChecked ?? false) ? ".tar" : "bin",
                    Filter = "Firmware|*.tar|Binary|*.bin"
                };

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    txtFirmware.Text = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Load_Firmware_Error");
            }
        }
    }
}
