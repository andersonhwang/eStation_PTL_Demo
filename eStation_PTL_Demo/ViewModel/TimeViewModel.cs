using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Helper;

namespace eStation_PTL_Demo.ViewModel
{
    internal class TimeViewModel : DialogViewModelBase
    {
        private DateTime apDateTime = DateTime.Now;

        /// <summary>
        /// AP 时间
        /// </summary>
        public DateTime ApDateTime
        {
            get => apDateTime;
            set
            {
                apDateTime = value;
                NotifyPropertyChanged(nameof(ApDateTime));
            }
        }

        /// <summary>
        /// Command - set time
        /// </summary>
        public ICommand CmdTimeSetting { get; set; }

        /// <summary>/// <summary>
        /// Default cosntructor
        /// </summary>
        public TimeViewModel()
        {
            CmdTimeSetting = new MyAsyncCommand(DoTimeSetting, (obj) => true);
        }

        /// <summary>
        /// Do Certificate
        /// </summary>
        /// <param name="obj"></param>
        private async Task DoTimeSetting(object obj)
        {
            await SendService.Instance.Send(new Time
            {
                ApTime = ApDateTime
            });
            DialogResult = true;
        }
    }
}
