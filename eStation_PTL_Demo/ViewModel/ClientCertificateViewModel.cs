using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Helper;
using eStation_PTL_Demo.Model;
using Microsoft.Win32;
using Serilog;

namespace eStation_PTL_Demo.ViewModel
{
    public class ClientCertificateViewModel : DialogViewModelBase
    {
        private byte[] pfx = [];
        public byte[] Pfx
        {
            get => pfx;
            set
            {
                if (pfx != value)
                {
                    pfx = value;
                    NotifyPropertyChanged(nameof(Pfx));
                }
            }
        }

        private string pfxPath = string.Empty;
        public string PfxPath { get => pfxPath; set { pfxPath = value; NotifyPropertyChanged(nameof(PfxPath)); } }

        private string pfxPassword = string.Empty;
        public string PfxPassword { get => pfxPassword; set { pfxPassword = value; NotifyPropertyChanged(nameof(PfxPassword)); } }

        private byte[] crt = [];
        public byte[] Crt
        {
            get => crt;
            set
            {
                if (crt != value)
                {
                    crt = value;
                    NotifyPropertyChanged(nameof(Crt));
                }
            }
        }

        private string crtPath = string.Empty;
        public string CrtPath { get => crtPath; set { crtPath = value; NotifyPropertyChanged(nameof(CrtPath)); } }

        /// <summary>
        /// Command - Browse
        /// </summary>
        public ICommand CmdPfxBrowse { get; set; }

        /// <summary>
        /// Command - Browse
        /// </summary>
        public ICommand CmdCrtBrowse { get; set; }

        /// <summary>
        /// Command - Certificate
        /// </summary>
        public ICommand CmdCertificate { get; set; }

        /// <summary>/// <summary>
        /// Default cosntructor
        /// </summary>
        public ClientCertificateViewModel()
        {
            CmdPfxBrowse = new MyCommand(DoPfxBrowse, (obj) => true);
            CmdCrtBrowse = new MyCommand(DoCrtBrowse, (obj) => true);
            CmdCertificate = new MyAsyncCommand(DoCertificate, CanCertificate);
        }

        /// <summary>
        /// Can Certificate
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanCertificate(object arg) => Pfx.Length > 0 && !string.IsNullOrEmpty(PfxPassword);

        /// <summary>
        /// Do browse
        /// </summary>
        /// <param name="obj"></param>
        private (byte[] bytes,string path) DoBrowse(object obj,string description, string ext)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Multiselect = false,
                    RestoreDirectory = true,
                    DefaultDirectory = Path.GetDirectoryName(Environment.ProcessPath),
                    DefaultExt = ext,
                    Filter = $"{description}|*.{ext}"
                };

                if (dialog.ShowDialog() != true || !File.Exists(dialog.FileName)) return ([], "");

                return (File.ReadAllBytes(dialog.FileName),dialog.FileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Load_Firmware_Error");
                return ([], "");
            }
        }

        private void DoPfxBrowse(object obj)
        {
            var (bytes, path) = DoBrowse(obj, "PFX", "pfx");
            Pfx = bytes;
            PfxPath = path;
        }

        private void DoCrtBrowse(object obj)
        {
            var (bytes, path) = DoBrowse(obj, "CRT", "crt");
            Crt = bytes;
            CrtPath = path;
        }

        /// <summary>
        /// Do Certificate
        /// </summary>
        /// <param name="obj"></param>
        private async Task DoCertificate(object obj)
        {
            await SendService.Instance.Send(new CertificateOrder
            {
                Certificate = Pfx,
                CertificateMD5 = FileHelper.GetBytesMd5(Pfx),
                Password = PfxPassword,
                CAChain = Crt,
                CAChainMD5 = FileHelper.GetBytesMd5(Crt)
            });
            DialogResult = true;
        }
    }
}
