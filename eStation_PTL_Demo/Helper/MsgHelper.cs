using System.Windows;

namespace eStation_PTL_Demo.Helper
{
    internal class MsgHelper
    {
        public static void Infor(string message)
        {
            try
            {
                MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }
        }

        public static void Error(string message)
        {
            try
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
        }

        public static MessageBoxResult Confirm(string message)
        {
            try
            {
                return MessageBox.Show(message, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
            catch { return MessageBoxResult.Cancel; }
        }
    }
}
