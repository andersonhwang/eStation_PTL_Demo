using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Model;
using eStation_PTL_Demo.ViewModel;
using System.Windows.Controls;

namespace eStation_PTL_Demo.View
{
    /// <summary>
    /// Interaction logic for ucDebug.xaml
    /// </summary>
    public partial class ucDebug : UserControl
    {
        public int DebugType { get; set; } = 0;

        public ucDebug()
        {
            InitializeComponent();
        }

        public void SetDebugInfo(int debugType)
        {
            DebugType = debugType;
            var context = DataContext as DebugViewModel;
            if (context is null) return;
            switch (DebugType)
            {
                case 0: context.SetInfo(new DebugInfo(DebugType, "Debug->Request", SendService.Instance.DebugRequest)); break;
                case 1: context.SetInfo(new DebugInfo(DebugType, "Debug->Response", SendService.Instance.DebugResponse)); break;
                default: break;
            }
        }
    }
}
