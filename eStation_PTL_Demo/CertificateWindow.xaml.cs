using eStation_PTL_Demo.Helper;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace eStation_PTL_Demo
{
    /// <summary>
    /// Interaction logic for X509Certificate.xaml
    /// </summary>
    public partial class CertificateWindow : Window
    {
        private readonly Action<string, string> ActSave;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="save"></param>
        public CertificateWindow(Action<string, string> save)
        {
            InitializeComponent();

            ActSave = save;
            Owner = Application.Current.MainWindow;
        }

        /// <summary>
        /// Button cancel click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// Button save click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var path = txtPath.Text.Trim();
            var key = txtKey.Text;
            if (path.Length == 0)
            {
                MsgHelper.Error("Certificate_File_Is_Required");
                return;
            }
            if (!File.Exists(path))
            {
                MsgHelper.Error("File_Not_Exist:" + path);
                return;
            }
            if (string.IsNullOrEmpty(key))
            {
                MsgHelper.Error("Key_Is_Required");
                return;
            }

            var check = FileHelper.GetCertificate(path, key);
            if (check.Length == 0)
            {
                MsgHelper.Error("Invalid_Certificate_Or_Key");
                return;
            }
            ActSave(path, key);
            Close();
        }

        /// <summary>
        /// Button browse click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                DefaultDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                FileName = "server",
                DefaultExt = ".pfx",
                Filter = "Personal Information Exchange (.pfx)|*.pfx"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                txtPath.Text = dialog.FileName;
            }
        }
    }
}
