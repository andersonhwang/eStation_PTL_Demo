using System.Windows;
using System.Windows.Controls;

namespace eStation_PTL_Demo.View
{
    /// <summary>
    /// Interaction logic for ucRadiButtonList.xaml
    /// </summary>
    public partial class ucRadiButtonList : UserControl
    {
        public static readonly DependencyProperty SelectedEnumValueProperty =
        DependencyProperty.Register("SelectedEnumValue", typeof(object), typeof(ucRadiButtonList), new PropertyMetadata(null));

        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register("EnumType", typeof(Array), typeof(ucRadiButtonList), new PropertyMetadata(null));

        public object SelectedEnumValue
        {
            get { return GetValue(SelectedEnumValueProperty); }
            set { SetValue(SelectedEnumValueProperty, value); }
        }

        public Array EnumType
        {
            get { return (Array)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }

        public ucRadiButtonList()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
