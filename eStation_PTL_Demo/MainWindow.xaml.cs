using System.Windows;

namespace eStation_PTL_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            debugRequest.SetDebugInfo(0);
            debugResponse.SetDebugInfo(1);
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}