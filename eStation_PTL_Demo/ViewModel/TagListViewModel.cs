using ClosedXML.Excel;
using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Helper;
using eStation_PTL_Demo.Model;
using Serilog;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace eStation_PTL_Demo.ViewModel
{
    internal class TagListViewModel : ViewModelBase
    {
        private readonly object _locker = new();
        private readonly string TagListPath = "TagList.txt";
        private Random? Random;
        private CancellationTokenSource? _tokenSource;
        private CancellationToken _token;
        private TagHeader header = new(string.Empty);
        private ObservableCollection<Tag> tags = [];
        private Dictionary<int, List<TagRecord>> autoTestRecords = [];
        private Dictionary<string, int> retryDic = [];
        private bool isStopping = false;
        private string fileName = string.Empty;
        /// <summary>
        /// Tag header
        /// </summary>
        public TagHeader Header
        {
            get => header;
            set
            {
                if (header != null)
                {
                    header.PropertyChanged -= Header_PropertyChanged;
                }

                header = value;

                if (header != null)
                {
                    header.PropertyChanged += Header_PropertyChanged;
                }

                NotifyPropertyChanged(nameof(Header));
            }
        }
        /// <summary>
        /// Tags list
        /// </summary>
        public ObservableCollection<Tag> Tags
        {
            get => tags;
            set
            {
                if (tags != null)
                {
                    tags.CollectionChanged -= Tags_CollectionChanged;
                }

                tags = value;

                if (tags != null)
                {
                    tags.CollectionChanged += Tags_CollectionChanged;
                    SendService.Instance.OnTagsChanged([.. tags]);
                }

                NotifyPropertyChanged(nameof(Tags));
            }
        }
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
        /// Command - quick send
        /// </summary>
        public IAsyncCommand CmdQuickSend { get; private set; }
        /// <summary>
        /// Command - menu
        /// </summary>
        public ICommand CmdMenu { get; private set; }
        /// <summary>
        /// Command - register test
        /// </summary>
        public ICommand CmdRegisterTest { get; private set; }

        /// <summary>
        /// Default cosntructor
        /// </summary>
        public TagListViewModel()
        {
            CmdSelectTags = new MyCommand(SelectTags, CanSelect);
            CmdRandomTags = new MyCommand(RandomTags, CanSelect);
            CmdSend = new MyAsyncCommand(SendTags, CanSend);
            CmdAutoSend = new MyCommand(AutoSendTags, CanSend);
            CmdQuickSend = new MyAsyncCommand(QuickSendTags, CanSend);
            CmdMenu = new MyCommand(TagListMenu, CanMenu);
            CmdRegisterTest = new MyCommand(OpenRegisterTest, CanSend);

            // 初始化完成属性后，添加监听
            header.PropertyChanged += Header_PropertyChanged;
            // 注册集合变更事件
            Tags.CollectionChanged += Tags_CollectionChanged;

            LoadTags();

            SendService.Instance.Register(UpdateTaskResult);

            // 初始通知SendService当前状态
            SendService.Instance.OnAutoRegisterChanged(Header.AutoRegister);
            SendService.Instance.OnTagsChanged([.. Tags]);
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
            foreach (Tag tag in Tags)
            {
                switch (parameter.ToString())
                {
                    case "A": tag.Select = Header.Select; break;
                    case "R": if (tag.Select) tag.R = Header.R; break;
                    case "G": if (tag.Select) tag.G = Header.G; break;
                    case "B": if (tag.Select) tag.B = Header.B; break;
                    case "Beep": if (tag.Select) tag.Beep = Header.Beep; break;
                    case "Flashing": if (tag.Select) tag.Flashing = Header.Flashing; break;
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
                    Flashing = tag.Flashing,
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

        private async Task BatchTestSendOrder(List<Tag> tags)
        {
            var items = new List<OrderItem>();
            foreach (var tag in tags)
            {
                if (!tag.Select) continue;
                items.Add(new OrderItem
                {
                    TagID = tag.TagID,
                    Beep = tag.Beep,
                    Flashing = tag.Flashing,
                    Color = GetColor(tag.R, tag.G, tag.B)
                });
                tag.Status = TagStatus.Sending;
                tag.LastSend = DateTime.Now;
                tag.SendCount++;
                ResetTag(tag, true);
                if (items.Count > 29)
                {
                    await SendService.Instance.Send(new Order
                    {
                        Time = header.Time,
                        Items = [.. items],
                    });
                    items.Clear();
                    await Task.Delay(TimeSpan.FromSeconds(Header.Wait));
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
                await Task.Delay(TimeSpan.FromSeconds(Header.Wait));
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
                _tokenSource?.Cancel();
                Header.AutoTest = false;
            }
            else
            {
                if (Header.Round < 1)
                {
                    MsgHelper.Error("The rounds of auto test cannot be less than 1 round.");
                    return;
                }

                if (!Tags.Any(x => x.Select))
                {
                    MsgHelper.Error("Please select tag to test.");
                    return;
                }

                autoTestRecords.Clear();

                // 重新创建 token source
                _tokenSource = new CancellationTokenSource();
                _token = _tokenSource.Token;

                Header.CurrenRound = 1;
                var selectedTags = Tags.Where(x => x.Select).ToList();

                Task.Run(async () =>
                {
                    var r = false;
                    var g = false;
                    var b = false;
                    var beep = false;
                    var flashing = false;
                    while (!_token.IsCancellationRequested && IsConnect && Header.CurrenRound <= Header.Round)
                    {
                        if (!IsConnect)
                        {
                            break;
                        }
                        //重置重试记录
                        retryDic.Clear();

                        // 确保本轮的记录列表存在（加锁，避免 UpdateTaskResult 在读取前写入导致 KeyNotFound）
                        lock (_locker)
                        {
                            if (!autoTestRecords.ContainsKey(Header.CurrenRound))
                            {
                                autoTestRecords[Header.CurrenRound] = [];
                            }
                        }

                        if (Header.Random)
                        {
                            r = false;
                            g = false;
                            b = false;
                            beep = false;
                            flashing = false;

                            Random = new Random(DateTime.Now.Millisecond);
                            var random = Random.Next(0x20);
                            if ((random & 0x10) == 0x10) r = true;
                            if ((random & 0x08) == 0x08) g = true;
                            if ((random & 0x04) == 0x04) b = true;
                            if ((random & 0x02) == 0x02) beep = true;
                            if ((random & 0x01) == 0x01) flashing = true;

                            if(!Header.RandomBeep)
                                beep = false;
                            if (!r && !g && !b)
                                r = true;

                            foreach (var tag in selectedTags)
                            {
                                tag.R = r;
                                tag.G = g;
                                tag.B = b;
                                tag.Beep = beep;
                                tag.Flashing = flashing;
                            }
                        }

                        isStopping = false;
                        await BatchTestSendOrder(selectedTags);
                        if (Header.RetryNumber > 0)
                        {
                            var retry = 0;
                            while (!_token.IsCancellationRequested)
                            {
                                HashSet<string> receivedIds;
                                lock (_locker)
                                {
                                    if (!autoTestRecords.TryGetValue(Header.CurrenRound, out var list))
                                        list = [];
                                    receivedIds = [.. list.Where(x => x.Tag != null).Select(t => t.Tag!.TagID)];
                                }

                                var faileds = selectedTags
                                    .Where(t => !receivedIds.Contains(t.TagID))
                                    .ToList();

                                if (faileds.Count == 0) break;
                                if (retry >= Header.RetryNumber) break;
                                foreach (var failed in faileds)
                                {
                                    if(retryDic.TryGetValue(failed.TagID, out int value))
                                        retryDic[failed.TagID] = ++value;
                                    else
                                        retryDic[failed.TagID] = 1;
                                }

                                await BatchTestSendOrder(faileds);
                                retry++;
                            }
                        }
                        await Task.Delay(TimeSpan.FromSeconds(Header.CurrenRound != Header.Round ? Header.Time * 5 : Header.Wait));
                        isStopping = true;

                        foreach (var retry in retryDic)
                        {
                            var tag = autoTestRecords[Header.CurrenRound].FirstOrDefault(x => x.Tag?.TagID == retry.Key);
                            if (tag != null)
                            {
                                tag.RetryNumber = retry.Value;
                            }
                            else
                            {
                                autoTestRecords[Header.CurrenRound].Add(new TagRecord { RetryNumber = retry.Value, Tag = new Tag(retry.Key) });
                            }
                        }

                        if (Header.CurrenRound != Header.Round)
                            Header.CurrenRound++;
                        else
                            break;
                    }

                    Header.AutoTest = false;
                    var saveDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Excel files (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv",
                        DefaultExt = ".xlsx",
                        FileName = $"AutoTest_{DateTime.Now:yyyyMMdd_HHmmss}"
                    };

                    if (saveDialog.ShowDialog() != true) return;
                    fileName = saveDialog.FileName;
                    ExportAutoTestRecords();
                });
                Header.AutoTest = true;
            }
        }

        /// <summary>
        /// Quick send tags
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private async Task QuickSendTags(object parameter)
        {
            var lst = new List<OrderItem>();
            foreach (var tag in Tags)
            {
                if (!tag.Select) continue;
                lst.Add(new OrderItem
                {
                    TagID = tag.TagID,
                    Beep = tag.Beep,
                    Flashing = tag.Flashing,
                    Color = GetColor(tag.R, tag.G, tag.B)
                });
                tag.Status = TagStatus.Sending;
                tag.LastSend = DateTime.Now;
                tag.SendCount++;
                ResetTag(tag, true);
                if (lst.Count >= 10)
                {
                    await SendService.Instance.Send(new Order
                    {
                        Time = header.Time,
                        Items = [.. lst]
                    });
                    lst.Clear();
                    await Task.Delay(200);
                }
            }
            if (lst.Count > 0)
            {
                await SendService.Instance.Send(new Order
                {
                    Time = header.Time,
                    Items = [.. lst]
                });
                lst.Clear();
                await Task.Delay(200);

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
                    tag.R = false;
                    tag.G = false;
                    tag.B = false;
                    tag.Beep = false;
                    tag.Flashing = false;

                    var r = Random.Next(0x20);
                    if ((r & 0x10) == 0x10) tag.R = true;
                    if ((r & 0x08) == 0x08) tag.G = true;
                    if ((r & 0x04) == 0x04) tag.B = true;
                    if ((r & 0x02) == 0x02) tag.Beep = true;
                    if ((r & 0x01) == 0x01) tag.Flashing = true;
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
                    ExportTagsToExcel();
                    break;
            }
        }

        /// <summary>
        /// Export tags to Excel (CSV format)
        /// </summary>
        private void ExportTagsToExcel()
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv",
                    DefaultExt = ".xlsx",
                    FileName = $"TagList_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() != true) return;

                var csv = new StringBuilder();

                // 添加表头
                csv.AppendLine("TagID,Group,Status,Version,RfPower,Battery,Speed,Heartbeat,TurnOff,Type,SendCount,ReceiveCount,HeartbeatCount,KeyCount,LastSend,LastReceive,LastHeartbeat,LastKey");

                // 添加数据行
                foreach (var tag in Tags)
                {
                    csv.AppendLine($"{EscapeCsvField(tag.TagID)}," +                                  
                                  $"{tag.Group}," +
                                  $"{tag.Status}," +
                                  $"{tag.Version}," +
                                  $"{tag.RfPower}," +
                                  $"{tag.Battery}," +
                                  $"{tag.Speed}," +
                                  $"{tag.Heartbeat}," +
                                  $"{tag.TurnOff}," +
                                  $"{tag.Type}," +
                                  $"{tag.SendCount}," +
                                  $"{tag.ReceiveCount}," +
                                  $"{tag.HeartbeatCount}," +
                                  $"{tag.KeyCount}," +
                                  $"{EscapeCsvField(tag.LastSend?.ToString("yyyy-MM-dd HH:mm:ss.fff"))}," +
                                  $"{EscapeCsvField(tag.LastReceive?.ToString("yyyy-MM-dd HH:mm:ss.fff"))}," +
                                  $"{EscapeCsvField(tag.LastHeartbeat?.ToString("yyyy-MM-dd HH:mm:ss.fff"))}," +
                                  $"{EscapeCsvField(tag.LastKey?.ToString("yyyy-MM-dd HH:mm:ss.fff"))}");
                }

                File.WriteAllText(saveDialog.FileName, csv.ToString(), Encoding.UTF8);

                MsgHelper.Infor($"Export completed! File saved to: {saveDialog.FileName}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Export_Tags_Error");
                MsgHelper.Error($"Export failed: {ex.Message}");
            }
        }


        private void ExportAutoTestRecords()
        {
            try
            {
                using var workbook = new XLWorkbook();
                var heartCountDic = new Dictionary<string, int>();
                List<string> failedTags = new();
                int row = 2;
                int successCount = 0;
                foreach (var round in autoTestRecords)
                {
                    var sheet = workbook.Worksheets.Add($"Round_{round.Key}");
                    // 添加表头
                    sheet.Cell(1, 1).Value = "TagID";
                    sheet.Cell(1, 2).Value = "Group";
                    sheet.Cell(1, 3).Value = "Status";
                    sheet.Cell(1, 4).Value = "Version";
                    sheet.Cell(1, 5).Value = "RfPower";
                    sheet.Cell(1, 6).Value = "Battery";
                    sheet.Cell(1, 7).Value = "Type";
                    sheet.Cell(1, 8).Value = "LastSendTime";
                    sheet.Cell(1, 9).Value = "LastRecvTime";
                    sheet.Cell(1, 10).Value = "HeartbeatCount";
                    sheet.Cell(1, 11).Value = "RetryNumber";
                    sheet.Cell(1, 12).Value = "SuccessRate";
                    sheet.Cell(1, 13).Value = "RoundSuccessRate";
                    sheet.Cell(1, 14).Value = "FailedTag";

                    row = 2;
                    successCount = 0;
                    failedTags.Clear();
                    foreach (var tag in tags)
                    {
                        if (!tag.Select) continue;

                        if (round.Key == 1)
                            heartCountDic[tag.TagID] = 0;

                        var record = round.Value.FirstOrDefault(x => x.Tag?.TagID == tag.TagID);
                        if (record != null && record.Tag?.LastReceive != null)
                        {
                            heartCountDic[tag.TagID]++;
                            successCount++;
                        }
                        else
                        {
                            failedTags.Add(tag.TagID);
                        }

                        sheet.Cell(row, 1).Value = tag.TagID;
                        sheet.Cell(row, 2).Value = record?.Tag?.Group;
                        sheet.Cell(row, 3).Value = record?.Tag?.Status.ToString();
                        sheet.Cell(row, 4).Value = record?.Tag?.Version;
                        sheet.Cell(row, 5).Value = record?.Tag?.RfPower;
                        sheet.Cell(row, 6).Value = record?.Tag?.Battery;
                        sheet.Cell(row, 7).Value = record?.Tag?.Type;
                        sheet.Cell(row, 8).Value = record?.Tag?.LastSend;
                        sheet.Cell(row, 9).Value = record?.Tag?.LastReceive;
                        sheet.Cell(row, 10).Value = heartCountDic[tag.TagID];
                        sheet.Cell(row, 11).Value = record?.RetryNumber;
                        sheet.Cell(row, 12).Value = $"{((double)heartCountDic[tag.TagID] * 100 / round.Key):F2}%";
                        row++;
                    }
                    sheet.Cell(2, 13).Value = $"{((double)(successCount * 100 / tags.Count(x => x.Select))):F2}%";
                    sheet.Cell(2, 14).Value = string.Join(",", failedTags);
                    sheet.Columns().AdjustToContents();
                }

                workbook.SaveAs(fileName);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MsgHelper.Infor($"Auto Test completed! Records saved to: {fileName}");
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Export_Auto_Test_Records_Error");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MsgHelper.Error($"Export failed: {ex.Message}");
                });
            }
        }


    /// <summary>
    /// Escape CSV field
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    private string EscapeCsvField(string? field)
        {
            if (string.IsNullOrEmpty(field)) return "";

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
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
                    if (string.IsNullOrWhiteSpace(id)) continue;
                    //if (!Regex.IsMatch(id, "^[0-9A-F]{12}$")) continue;
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

                        tag.RfPower = item.RfPower;
                        tag.Battery = item.Voltage;
                        tag.Version = item.Version;
                        tag.Group = item.Group;
                        tag.Type = item.Type;
                        tag.Speed = item.SleepInterval;
                        tag.Heartbeat = item.HeartbeatInterval;
                        tag.TurnOff = item.TurnOffInterval;

                        switch (item.DataType)
                        {
                            case 0: 
                                tag.HeartbeatCount++; tag.LastHeartbeat = DateTime.Now; tag.Status = TagStatus.Heartbeat;
                                break;
                            case 1:
                                tag.ReceiveCount++; tag.LastReceive = DateTime.Now;
                                if ((tag.Status != TagStatus.Success) && (item.RfPower != -256)) Header.SuccsessCount++;
                                tag.Status = item.RfPower == -256 ? TagStatus.Fail : TagStatus.Success;
                                if (Header.AutoTest && !isStopping && tag.Select)
                                {
                                    autoTestRecords[Header.CurrenRound].Add(new TagRecord { Tag = tag.Clone() });
                                }
                                break;
                            case 2: tag.KeyCount++; tag.LastKey = DateTime.Now; tag.Status = TagStatus.Key; break;
                            case 3: tag.Status = TagStatus.GroupControl; tag.LastReceive = DateTime.Now; break;
                            case 4: tag.Status = TagStatus.Duplicate; tag.LastReceive = DateTime.Now; break;
                            default: break;
                        }
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
            tag.Flashing = false;
            tag.LastSend =
            tag.LastHeartbeat =
            tag.LastKey = null;
            tag.KeyCount =
            tag.HeartbeatCount =
            tag.SendCount =
            tag.ReceiveCount = 0;
        }

        /// <summary>
        /// 处理Header属性变更
        /// </summary>
        private void Header_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TagHeader.AutoRegister))
            {
                // 通知SendService自动注册属性已变更
                SendService.Instance.OnAutoRegisterChanged(Header.AutoRegister);
            }
        }

        /// <summary>
        /// 处理Tags集合变更
        /// </summary>
        private void Tags_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // 通知SendService标签集合已变更
            SendService.Instance.OnTagsChanged([.. Tags]);
        }

        private RegisterTestWindow? registerTestWindow;
        private void OpenRegisterTest(object obj)
        {
            try
            {
                if (registerTestWindow == null || !registerTestWindow.IsVisible)
                {
                    registerTestWindow = new RegisterTestWindow(IsConnect, Header.AutoRegister);
                    registerTestWindow.Owner = Application.Current.MainWindow;
                    registerTestWindow.Closed += (_, __) => registerTestWindow = null;
                    registerTestWindow.Show();
                }
                else
                {
                    if (registerTestWindow.WindowState == WindowState.Minimized)
                        registerTestWindow.WindowState = WindowState.Normal;
                    registerTestWindow.Activate();
                    registerTestWindow.Topmost = true; // bring to front
                    registerTestWindow.Topmost = false;
                    registerTestWindow.Focus();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Open_Register_Test_Window_Error");
            }
        }
    }
}
