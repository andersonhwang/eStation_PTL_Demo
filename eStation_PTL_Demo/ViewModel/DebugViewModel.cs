using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Helper;
using eStation_PTL_Demo.Model;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace eStation_PTL_Demo.ViewModel
{
    public class DebugViewModel : ViewModelBase
    {
        private int index = -1;
        private DebugInfo info;
        private ObservableCollection<DebugItem> items = [];

        /// <summary>
        /// Select index
        /// </summary>
        public int Index { get { return index; } set { index = value; NotifyPropertyChanged(nameof(Index)); } }
        /// <summary>
        /// Debug info
        /// </summary>
        public DebugInfo Info { get => info; set { info = value; NotifyPropertyChanged(nameof(Info)); } }
        /// <summary>
        /// Items
        /// </summary>
        public ObservableCollection<DebugItem> Items { get => items; set { items = value; NotifyPropertyChanged(nameof(Items)); } }

        /// <summary>
        /// Command clear
        /// </summary>
        public MyCommand CmdClear { get; set; }

        /// <summary>
        /// Command copy
        /// </summary>
        public MyCommand CmdCopy { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DebugViewModel()
        {
            CmdClear = new MyCommand(DoClear, CanClear);
            CmdCopy = new MyCommand(DoCopy, CanCopy);
        }

        /// <summary>
        /// Set information
        /// </summary>
        /// <param name="info">Debug information</param>
        public void SetInfo(DebugInfo info)
        {
            Info = info;
            if (Info.DebugType == 0) SendService.Instance.DebugRequestHandler += Instance_DebugItemHandler;
            else if(Info.DebugType == 1) SendService.Instance.DebugResponseHandler += Instance_DebugItemHandler;
        }

        /// <summary>
        /// Debug item handler
        /// </summary>
        /// <param name="item">Debug item</param>
        private void Instance_DebugItemHandler(DebugItem item)
        {
            Application.Current.Dispatcher.Invoke(() => { Items.Insert(0, item); });
        }

        /// <summary>
        /// Can clear
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool CanClear(object parameter) => true;

        /// <summary>
        /// Can clear
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool CanCopy(object parameter) => true;

        /// <summary>
        /// Do clear
        /// </summary>
        /// <param name="parameter"></param>
        private void DoClear(object parameter)
        {
            Items.Clear();
        }

        /// <summary>
        /// Do copy
        /// </summary>
        /// <param name="parameter"></param>
        private void DoCopy(object parameter)
        {
            if(Items.Count == 0) return;
            if(Index == -1)
            {
                MsgHelper.Infor("Please select an item to copy");
                return;
            }
            if(Items.Count <= Index)
            {
                return;
            }
            Clipboard.SetDataObject(Items[Index].Data);
        }
    }
}
