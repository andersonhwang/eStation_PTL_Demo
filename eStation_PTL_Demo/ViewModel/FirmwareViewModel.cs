using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Helper;
using eStation_PTL_Demo.Model;
using Microsoft.Win32;
using Serilog;
using System.IO;
using System.Windows.Input;

namespace eStation_PTL_Demo.ViewModel
{
    public class FirmwareViewModel : DialogViewModelBase
    {
        private OtaFirmware firmware = new();
        public OtaFirmware Firmware
        {
            get => firmware;
            set
            {
                if (firmware != value)
                {
                    firmware = value;
                    NotifyPropertyChanged(nameof(Firmware));
                }
            }
        }

        /// <summary>
        /// Command - Browse
        /// </summary>
        public ICommand CmdBrowse { get; set; }
        /// <summary>
        /// Command - OTA
        /// </summary>
        public ICommand CmdOTA { get; set; }

        /// <summary>
        /// Default cosntructor
        /// </summary>
        public FirmwareViewModel()
        {
            CmdBrowse = new MyCommand(DoBrowse, (obj) => true);
            CmdOTA = new MyAsyncCommand(DoOTA, CanOTA);
        }

        /// <summary>
        /// Can OTA
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanOTA(object arg) => File.Exists(Firmware.Path) && Firmware.Version > 0;

        /// <summary>
        /// Do browse
        /// </summary>
        /// <param name="obj"></param>
        private void DoBrowse(object obj)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Multiselect = false,
                    RestoreDirectory = true,
                    DefaultDirectory = Path.GetDirectoryName(Environment.ProcessPath),
                    DefaultExt = Firmware.Type switch
                    {
                        1 => ".tar",
                        2 => ".bin",
                        _ => ".tar"
                    },
                    Filter = "Firmware|*.tar|Binary|*.bin"
                };

                if (dialog.ShowDialog() != true || !File.Exists(dialog.FileName)) return;

                Firmware.Path = dialog.FileName;
                Firmware.Bytes = File.ReadAllBytes(dialog.FileName);
                Firmware.MD5 = FileHelper.GetBytesMd5(Firmware.Bytes);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Load_Firmware_Error");
            }
        }

        /// <summary>
        /// Do OTA
        /// </summary>
        /// <param name="obj"></param>
        private async Task DoOTA(object obj)
            => await SendService.Instance.Send(new OTA
            {
                Type = Firmware.Type,
                Firmware = Firmware.Bytes,
                Version = Firmware.Version,
                MD5 = Firmware.MD5,
                Factory = Firmware.Factory
            });
    }
}
