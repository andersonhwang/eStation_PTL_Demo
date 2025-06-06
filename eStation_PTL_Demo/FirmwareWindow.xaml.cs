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
    }
}
