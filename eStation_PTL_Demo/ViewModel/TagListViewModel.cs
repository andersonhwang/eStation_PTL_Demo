using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Helper;
using eStation_PTL_Demo.Model;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace eStation_PTL_Demo.ViewModel
{
    internal class TagListViewModel : ViewModelBase
    {
        private readonly object _locker = new();
        private readonly string TagListPath = "TagList.txt";
        private Random Random;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;
        private TagHeader header = new(string.Empty);
        private ObservableCollection<Tag> tags = [];
        /// <summary>
        /// Tag header
        /// </summary>
        public TagHeader Header { get => header; set { header = value; NotifyPropertyChanged(nameof(Header)); } }
        /// <summary>
        /// Tags list
        /// </summary>
        public ObservableCollection<Tag> Tags { get => tags; set { tags = value; NotifyPropertyChanged(nameof(Tags)); } }
        /// <summary>
        /// Command - select tags
        /// </summary>
        public ICommand CmdSelectTags { get; set; }
        /// <summary>
        /// Command - random tags
        /// </summary>
        public ICommand CmdRandomTags { get; set; }
        /// <summary>
        /// Command - send
        /// </summary>
        public ICommand CmdSend { get; private set; }
        /// <summary>
        /// Command - auto send
        /// </summary>
        public ICommand CmdAutoSend { get; private set; }
        /// <summary>
        /// Command - menu
        /// </summary>
        public ICommand CmdMenu { get; private set; }

        /// <summary>
        /// Default cosntructor
        /// </summary>
        public TagListViewModel()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

            CmdSelectTags = new MyCommand(SelectTags, CanSelect);
            CmdRandomTags = new MyCommand(RandomTags, CanSelect);
            CmdSend = new MyAsyncCommand(SendTags, CanSend);
            CmdAutoSend = new MyCommand(AutoSendTags, CanSend);
            CmdMenu = new MyCommand(TagListMenu, CanMenu);

            LoadTags();

            SendService.Instance.Register(UpdateTaskResult);
        }

        /// <summary>
        /// Can select
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanSelect(object parameter) => true;

        /// <summary>
        /// Can edit
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanMenu(object parameter) => true;

        /// <summary>
        /// Select tags
        /// </summary>
        /// <param name="parameter"></param>
        public void SelectTags(object parameter)
        {
            foreach (Tag tag in tags)
            {
                switch (parameter.ToString())
                {
                    case "A": tag.Select = Header.Select; break;
                    case "R": if (tag.Select) tag.R = Header.R; break;
                    case "G": if (tag.Select) tag.G = Header.G; break;
                    case "B": if (tag.Select) tag.B = Header.B; break;
                    case "Beep": if (tag.Select) tag.Beep = Header.Beep; break;
                    case "Blink": if (tag.Select) tag.Blink = Header.Blink; break;
                }
            }
        }

        /// <summary>
        /// Send tags
        /// </summary>
        /// <param name="parameter"></param>
        private async Task SendTags(object parameter)
        {
            var code = parameter.ToString();
            switch (code)
            {
                case "E":   // Emtpy
                    await SendService.Instance.Send(new Order
                    {
                        Time = header.Time,
                    });
                    break;
                case "B":   // Bind
                    await SendBind();
                    break;
                case "C":   // Order
                    await SendOrder();
                    break;
                case "R":   // Reset
                    Reset();
                    break;
            }

            if (code == "E" || code == "R") return;

            Header.RoundCount++;
            Header.SendCount += Tags.Count(x => x.Select);
        }

        /// <summary>
        /// Send
        /// </summary>
        private async Task SendBind()
        {
            var items = new List<BindItem>();
            foreach (var tag in Tags)
            {
                if (!tag.Select) continue;
                items.Add(new BindItem
                {
                    TagID = tag.TagID,
                    Group = (byte)Header.Group,
                });
                tag.Status = TagStatus.Sending;
                tag.LastSend = DateTime.Now;
                tag.SendCount++;
                ResetTag(tag, true);
                if (items.Count > 59)
                {
                    await SendService.Instance.Send(new Bind
                    {
                        Items = [.. items],
                    });
                    items.Clear();
                }
            }
            if (items.Count > 0)
            {
                await SendService.Instance.Send(new Bind
                {
                    Items = [.. items],
                });
                items.Clear();
            }
        }

        /// <summary>
        /// Send
        /// </summary>
        private async Task SendOrder()
        {
            var items = new List<OrderItem>();
            foreach (var tag in Tags)
            {
                if (!tag.Select) continue;
                items.Add(new OrderItem
                {
                    TagID = tag.TagID,
                    Beep = tag.Beep,
                    Flashing = tag.Blink,
                    Color = GetColor(tag.R, tag.G, tag.B)
                });
                tag.Status = TagStatus.Sending;
                tag.LastSend = DateTime.Now;
                tag.SendCount++;
                ResetTag(tag, true);
                if (items.Count > 59)
                {
                    await SendService.Instance.Send(new Order
                    {
                        Time = header.Time,
                        Items = [.. items],
                    });
                    items.Clear();
                }
            }
            if (items.Count > 0)
            {
                await SendService.Instance.Send(new Order
                {
                    Time = header.Time,
                    Items = [.. items],
                });
                items.Clear();
            }
        }

        /// <summary>
        /// Auto send tags
        /// </summary>
        /// <param name="parameter"></param>
        private void AutoSendTags(object parameter)
        {
            if (Header.AutoTest)
            {
                _tokenSource.Cancel();
                Header.AutoTest = false;
            }
            else
            {
                if (Header.Speed < 2)
                {
                    MsgHelper.Error("The speed of auto test cannot be less than 2 seconds.");
                    return;
                }

                if (!Tags.Any(x => x.Select))
                {
                    MsgHelper.Error("Please select tag to test.");
                    return;
                }

                Task.Run(async () =>
                {
                    while (!_token.IsCancellationRequested)
                    {
                        if (Header.AutoMode != 0) Header.Group++;
                        if (Header.Group > 0xFF) Header.Group = 0;

                        switch (Header.AutoMode)
                        {
                            case 0:
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        await Task.Delay(TimeSpan.FromSeconds(Header.Speed));
                    }

                });
                Header.AutoTest = true;
            }
        }

        /// <summary>
        /// Reset
        /// </summary>
        private void Reset()
        {
            Header.RoundCount = 0;
            Header.SendCount = 0;
            Header.SuccsessCount = 0;
            foreach (var tag in Tags)
            {
                ResetTag(tag);
            }
        }

        /// <summary>
        /// Random tags
        /// </summary>
        /// <param name="parameter"></param>
        public void RandomTags(object parameter)
        {
            Random = new Random(DateTime.Now.Millisecond);
            foreach (var tag in Tags)
            {
                if (tag.Select)
                {
                    var r = Random.Next(0x20);
                    //if ((r & 0x10) == 0x10) tag.R = true;
                    //if ((r & 0x08) == 0x08) tag.G = true;
                    //if ((r & 0x04) == 0x04) tag.B = true;
                    //if ((r & 0x02) == 0x02) tag.Beep = true;
                    //if ((r & 0x01) == 0x01) tag.Blink = true;
                }
            }
        }

        /// <summary>
        /// Tag list menu
        /// </summary>
        /// <param name="parameter"></param>
        public void TagListMenu(object parameter)
        {
            switch (parameter.ToString())
            {
                case "E":
                    var dialog = new TagList(Tags.Select(x => x.TagID).ToList());
                    dialog.ShowDialog();
                    LoadTags();
                    break;
                case "R":
                    Reset();
                    break;
                case "P":

                    break;
            }
        }

        /// <summary>
        /// Load tags
        /// </summary>
        private void LoadTags()
        {
            // Create if not exist
            if (!File.Exists(TagListPath))
            {
                using var fs = File.Create(TagListPath);
                var title = Encoding.UTF8.GetBytes(string.Empty);
                fs.Write(title, 0, title.Length);
            }

            // Read
            var lines = File.ReadAllLines(TagListPath);
            lock (_locker)
            {
                Tags.Clear();
                foreach (var line in lines)
                {
                    var id = line.Trim().ToUpper();
                    if (!Regex.IsMatch(id, "^[0-9A-F]{12}$")) continue;
                    if (Tags.Any(x => x.TagID.Equals(id))) continue;
                    Tags.Add(new Tag(id));
                }
            }
        }

        /// <summary>
        /// Update task result
        /// </summary>
        /// <param name="result">Task result</param>
        public void UpdateTaskResult(TaskResult result)
        {
            lock (_locker)
            {
                try
                {
                    foreach (var item in result.Items)
                    {
                        //if (Header.OnlyData && (infor.reason != 1 && infor.reason != 4)) return;
                        var tag = Tags.FirstOrDefault(x => x.TagID.Equals(item.TagID));
                        if (tag is null)
                        {
                            if (Header.AutoRegister)
                            {
                                tag = new Tag(item.TagID);
                                Application.Current.Dispatcher.Invoke(() => Tags.Add(tag));
                            }
                            else return;
                        }
                        switch (item.DataType)
                        {
                            case 0: tag.HeartbeatCount++; tag.LastHeartbeat = DateTime.Now; tag.Status = TagStatus.Heartbeat; break;
                            case 1:
                                tag.ReceiveCount++; tag.LastReceive = DateTime.Now;
                                if ((tag.Status != TagStatus.Success) && (item.RfPower != -256)) Header.SuccsessCount++;
                                tag.Status = item.RfPower == -256 ? TagStatus.Fail : TagStatus.Success;
                                break;
                            case 2: tag.KeyCount++; tag.LastKey = DateTime.Now; tag.Status = TagStatus.Key; break;
                            case 3: tag.Status = TagStatus.GroupControl; tag.LastReceive = DateTime.Now; break;
                            case 4: tag.Status = TagStatus.Duplicate; tag.LastReceive = DateTime.Now; break;
                            default: break;
                        }
                        tag.RfPower = item.RfPower;
                        tag.Battery = item.Voltage;
                        tag.Version = item.Version;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Update_Task_Result_Error");
                }
            }
        }

        /// <summary>
        /// Reset tags
        /// </summary>
        /// <param name="tag">Tag to reset</param>
        /// <param name="onlyReceive">Only reset receive flags</param>
        private void ResetTag(Tag tag, bool onlyReceive = false)
        {
            tag.LastReceive = null;
            tag.Battery = null;
            tag.RfPower = null;
            tag.Status = TagStatus.Init;

            if (onlyReceive) { return; }

            tag.Select =
                tag.R =
                tag.G =
                tag.B =
            tag.Beep =
            tag.Blink = false;
            tag.LastSend =
            tag.LastHeartbeat =
            tag.LastKey = null;
            tag.KeyCount =
            tag.HeartbeatCount =
            tag.SendCount =
            tag.ReceiveCount = 0;
        }
    }
}
