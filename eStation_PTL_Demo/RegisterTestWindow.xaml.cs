using System;
using System.Windows;
using eStation_PTL_Demo.ViewModel;

namespace eStation_PTL_Demo
{
    public partial class RegisterTestWindow : Window
    {
        public RegisterTestWindow(bool isConnect, bool autoRegister)
        {
            InitializeComponent();
            DataContext = new RegisterTestViewModel(isConnect, autoRegister);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // 让VM感知关闭，自动停止测试
            if (DataContext is RegisterTestViewModel vm)
            {
                vm.OnWindowClosed();
            }
        }
    }
}
