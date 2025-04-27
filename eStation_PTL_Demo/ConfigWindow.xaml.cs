using eStation_PTL_Demo.Model;
using eStation_PTL_Demo.ViewModel;
using System.Windows;

namespace eStation_PTL_Demo
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">AP config</param>
        public ConfigWindow(ApConfigB config)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;
            if (DataContext is ApConfigViewModel vm) vm.Config = config;
        }

        private void Button_Click(object sender, RoutedEventArgs e) => Close();
    }
}
