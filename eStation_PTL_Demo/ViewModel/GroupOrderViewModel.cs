using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Model;
using System.Windows.Input;

namespace eStation_PTL_Demo.ViewModel
{
    public class GroupOrderViewModel : ViewModelBase
    {
        private TagGroup group = new(string.Empty);
        public TagGroup Group
        {
            get => group;
            set { group = value; NotifyPropertyChanged(nameof(Group)); }
        }

        public ICommand CmdGroupSend { get; private set; }
        public ICommand CmdGroupStop { get; private set; }

        public GroupOrderViewModel()
        {
            CmdGroupSend = new MyAsyncCommand(GroupSend, CanSend);
            CmdGroupStop = new MyAsyncCommand(GroupStop, CanSend);
        }

        /// <summary>
        /// Group stop
        /// </summary>
        /// <param name="parameter"></param>
        public async Task GroupSend(object parameter)
        {
            await SendService.Instance.Send(new GroupOrder
            {
                Time = Group.Time,
                Beep = Group.Beep,
                Flashing = Group.Flashing,
                Color = GetColor(Group.R, Group.G, Group.B),
                StartGroup = Group.StartGroup,
                EndGroup = Group.EndGroup,
                StartID = Group.StartID,
                EndID = Group.EndID,
            });
        }

        /// <summary>
        /// Group send
        /// </summary>
        /// <param name="parameter"></param>
        public async Task GroupStop(object parameter)
        {
            await SendService.Instance.Send(new GroupOrder
            {
                Time = -1,
                Beep = false,
                Flashing = false,
                Color = GetColor(false, false, false),
                StartGroup = Group.StartGroup,
                EndGroup = Group.EndGroup,
                StartID = Group.StartID,
                EndID = Group.EndID,
            });
        }
    }
}
