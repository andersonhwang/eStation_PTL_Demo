using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Entity.V_1;
using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Helper;
using eStation_PTL_Demo.Model;
using Serilog;

namespace eStation_PTL_Demo.Core
{
    /// <summary>
    /// Send service
    /// </summary>
    public class SendService
    {
        private PTLServer? Server;
        private static SendService instance = new();
        protected readonly ConcurrentQueue<string> RecvQueue = [];
        private CancellationTokenSource? processingCts;
        private bool isRunning = false;
        private bool isAutoRegister = false;
        private ObservableCollection<Tag> tags = [];

        private SendService()
        {
            AutoRegisterChangedHandler += HandleAutoRegisterChanged;
            TagsChangedHandler += HandleTagsChanged;
        }

        // 添加服务运行状态属性
        public bool IsRunning
        {
            get => isRunning;
            private set
            {
                isRunning = value;
                // 触发状态改变事件
                ServiceStatusChangedHandler?.Invoke(isRunning);
            }
        }

        public int Sequence = 0; // Just for demo

        public string DebugRequest { get; set; } = "-";
        public string DebugResponse { get; set; } = "-";

        /// <summary>
        /// Instance of send service
        /// </summary>
        public static SendService Instance => instance;

        /// <summary>
        /// AP status delegate
        /// </summary>
        /// <param name="status">AP status</param>
        public delegate void ApStatusDelegate(ApStatus status);
        public event ApStatusDelegate? ApStatusHandler;
        /// <summary>
        /// AP infor delegate
        /// </summary>
        /// <param name="infor">AP information</param>
        public delegate void ApInforDelegate(ApInfor infor);
        public event ApInforDelegate? ApInforHandler;
        /// <summary>
        /// AP heartbeat delegate
        /// </summary>
        /// <param name="heartbeat">AP heartbeat</param>
        public delegate void ApHeartbeatDelegate(ApHeartbeat heartbeat);
        public event ApHeartbeatDelegate? ApHeartbeatHandler;
        /// <summary>
        /// Task response delegate
        /// </summary>
        /// <param name="response">Task response</param>
        public delegate void TaskResponseDelegate(TaskResponse response);
        public event TaskResponseDelegate? TaskResponseHandler;
        /// <summary>
        /// Task result delegate
        /// </summary>
        /// <param name="result"></param>
        public delegate void TaskResultDelegate(TaskResult result);
        public event TaskResultDelegate? TaskResultHandler;
        /// <summary>
        /// Debug request delegate
        /// </summary>
        /// <param name="item">Debug item</param>
        public delegate void DebugRequestDelegate(DebugItem item);
        public event DebugRequestDelegate? DebugRequestHandler;
        /// <summary>
        /// Debug response delegate
        /// </summary>
        /// <param name="item">Debug item</param>
        public delegate void DebugResponseDelegate(DebugItem item);
        public event DebugResponseDelegate? DebugResponseHandler;

        // 添加服务状态改变事件
        public delegate void ServiceStatusChangedDelegate(bool isRunning);
        public event ServiceStatusChangedDelegate? ServiceStatusChangedHandler;

        /// <summary>
        /// AutoRegister变更代理
        /// </summary>
        /// <param name="isAutoRegister">是否自动注册标签</param>
        public delegate void AutoRegisterChangedDelegate(bool isAutoRegister);
        public event AutoRegisterChangedDelegate? AutoRegisterChangedHandler;

        /// <summary>
        /// Tags集合变更代理
        /// </summary>
        /// <param name="tags">标签集合</param>
        public delegate void TagsChangedDelegate(ObservableCollection<Tag> tags);
        public event TagsChangedDelegate? TagsChangedHandler;


        /// <summary>
        /// Run send service
        /// </summary>
        /// <param name="info">Connection information</param>
        public bool Run(ConnInfo info)
        {
            try
            {
                // 如果已经在运行，停止
                if (IsRunning)
                {
                    Stop();
                    return false;
                }

                processingCts = new CancellationTokenSource();
                var token = processingCts.Token;

                Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            if (!RecvQueue.TryDequeue(out var msg))
                            {
                                await Task.Delay(200, token);
                                continue;
                            }

                            var message = JsonSerializer.Deserialize<Message>(msg);
                            if (message?.ConnType == 0)
                            {
                                var item = JsonSerializer.Deserialize<ApData>(message.Data);
                                if (item is null) continue;
                                var alias = item.Topic switch
                                {
                                    _ when item.Topic.EndsWith("info") => 0x10,
                                    _ when item.Topic.EndsWith("message") => 0x11,
                                    _ when item.Topic.EndsWith("result") => 0x12,
                                    _ when item.Topic.EndsWith("heartbeat") => 0x13,
                                    _ when item.Topic.EndsWith("response") => 0x03,
                                    _ => 0
                                };
                                switch (alias)
                                {
                                    case 0x03:
                                        var response = JsonSerializer.Deserialize<TaskResponse>(item.PayloadSegment);
                                        if (response is null) continue;
                                        TaskResponseHandler?.Invoke(response);
                                        break;
                                    case 0x10:
                                        var infor = JsonSerializer.Deserialize<ApInfor>(item.PayloadSegment);
                                        if (infor is null) continue;
                                        infor.ID = infor.MAC;
                                        Server?.UpdateApConfig(infor);
                                        ApInforHandler?.Invoke(infor);
                                        ApStatusHandler?.Invoke(ApStatus.Online);
                                        break;
                                    case 0x11:
                                        DebugResponseHandler?.Invoke(new DebugItem(alias, item.PayloadSegment));
                                        break;
                                    case 0x12:
                                        var resultV1 = JsonSerializer.Deserialize<TaskResultV1>(item.PayloadSegment);
                                        if (resultV1 is null) continue;
                                        var tagList = tags.Select(y => y.TagID).ToHashSet();
                                        var filterItems = isAutoRegister ? resultV1.Results : [.. resultV1.Results.Where(x => tagList.Contains(x.TagID))];
                                        if (filterItems.Length == 0) continue;
                                        var result = new TaskResult
                                        {
                                            Items =
                                            [..
                                            filterItems.Select(x=>new TagResult {
                                                TagID = x.TagID,
                                                DataType = x.ResultType switch{
                                                    0xFE => 1, // 通信
                                                    0xFD => 2, // 按键
                                                    _ => 0, // 心跳
                                                },
                                                Version = Int32.TryParse(x.Version,out var version) ? version : 0,
                                                Color = x.Colors?.Length>0 ? GetColor(x.Colors[0]) : 0,
                                                Group = x.Group,
                                                RfPower = x.RfPowerRecv,
                                                Voltage = x.Battery
                                            })
                                            ]
                                        };
                                        TaskResultHandler?.Invoke(result);
                                        break;
                                    case 0x13:
                                        var heartbeat = JsonSerializer.Deserialize<ApHeartbeat>(item.PayloadSegment);
                                        if (heartbeat is null) continue;
                                        ApHeartbeatHandler?.Invoke(heartbeat);
                                        break;
                                    default:
                                        break;
                                }
                                DebugResponseHandler?.Invoke(new DebugItem(alias, JsonSerializer.Serialize(item.PayloadSegment, JsonOptions.Default)));
                            }
                            else if (message?.ConnType == 1)
                            {
                                var entity = JsonSerializer.Deserialize<BaseEntity>(message.Data);
                                if (entity is null) continue;
                                switch (entity.Code)
                                {
                                    case 0x01:
                                        var infor = JsonSerializer.Deserialize<ApInfor>(message.Data);
                                        if (infor is null) continue;
                                        Server?.UpdateApConfig(infor);
                                        ApInforHandler?.Invoke(infor);
                                        ApStatusHandler?.Invoke(ApStatus.Online);
                                        break;
                                    case 0x02:
                                        var heartbeat = JsonSerializer.Deserialize<ApHeartbeat>(message.Data);
                                        if (heartbeat is null) continue;
                                        ApHeartbeatHandler?.Invoke(heartbeat);
                                        break;
                                    case 0x03:
                                        var response = JsonSerializer.Deserialize<TaskResponse>(message.Data);
                                        if (response is null) continue;
                                        TaskResponseHandler?.Invoke(response);
                                        break;
                                    case 0x04:
                                        var result = JsonSerializer.Deserialize<TaskResult>(message.Data);
                                        if (result is null) continue;
                                        var tagList = tags.Select(y => y.TagID).ToHashSet();
                                        var filterItems = isAutoRegister ? result.Items : [.. result.Items.Where(x => tagList.Contains(x.TagID))];
                                        if (filterItems.Length == 0) continue;
                                        result.Items = filterItems;
                                        TaskResultHandler?.Invoke(result);
                                        break;
                                    default: break;
                                }
                                DebugResponseHandler?.Invoke(new DebugItem(entity.Code, message.Data));
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // 正常取消
                            break;
                        }
                        catch
                        {
                            // 其他异常，不重要
                            if (token.IsCancellationRequested)
                                break;
                        }
                    }
                }, token);

                Server = info.ConnType switch
                {
                    ConnType.MQTT => new MQTT(info, ProcessApStatus, ProcessApData),
                    ConnType.WebSocket => new WebSocket(info, ProcessApStatus, ProcessApData),
                    _ => new MQTT(info, ProcessApStatus, ProcessApData),
                };

                IsRunning = true;

                return Server.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Send_Service_Error");
                return false;
            }
        }

        /// <summary>
        /// Stop send service
        /// </summary>
        public bool Stop()
        {
            try
            {
                // 取消处理任务
                processingCts?.Cancel();
                processingCts = null;

                // 停止服务器
                if (Server != null)
                {
                    bool result = Server.Stop();
                    IsRunning = false;
                    return result;
                }

                IsRunning = false;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Send_Service_Stop_Error");
                return false;
            }
        }

        /// <summary>
        /// Register AP status event
        /// </summary>
        /// <param name="status"></param>
        public void Register(ApStatusDelegate status) => ApStatusHandler += status;

        /// <summary>
        /// Register AP information event
        /// </summary>
        /// <param name="infor">AP status event</param>
        public void Register(ApInforDelegate infor) => ApInforHandler += infor;

        /// <summary>
        /// Register AP heartbeat event
        /// </summary>
        /// <param name="heartbeat">AP data event</param>
        public void Register(ApHeartbeatDelegate heartbeat) => ApHeartbeatHandler += heartbeat;

        /// <summary>
        /// Register task response event
        /// </summary>
        /// <param name="response">Task response event</param>
        public void Register(TaskResponseDelegate response) => TaskResponseHandler += response;

        /// <summary>
        /// Register task result event
        /// </summary>
        /// <param name="result">Task result event</param>
        public void Register(TaskResultDelegate result) => TaskResultHandler += result;

        /// <summary>
        /// Register debug response event
        /// </summary>
        /// <param name="debug">Debug response event</param>
        public void Register(DebugResponseDelegate debug) => DebugResponseHandler += debug;

        /// <summary>
        /// Register debug request event
        /// </summary>
        /// <param name="debug">Debug request event</param>
        public void Register(DebugRequestDelegate debug) => DebugRequestHandler += debug;

        /// <summary>
        /// Register service status changed event
        /// </summary>
        /// <param name="changedDelegate"></param>
        public void Register(ServiceStatusChangedDelegate changedDelegate) => ServiceStatusChangedHandler += changedDelegate;

        /// <summary>
        /// 注册Tags集合变更事件
        /// </summary>
        /// <param name="tagsChanged">Tags集合变更处理方法</param>
        public void Register(TagsChangedDelegate tagsChanged) => TagsChangedHandler += tagsChanged;

        /// <summary>
        /// 处理AutoRegister变更
        /// </summary>
        /// <param name="isAutoRegister">是否自动注册标签</param>
        public void OnAutoRegisterChanged(bool isAutoRegister)
        {
            AutoRegisterChangedHandler?.Invoke(isAutoRegister);
        }

        /// <summary>
        /// 处理Tags集合变更
        /// </summary>
        /// <param name="tags">标签集合</param>
        public void OnTagsChanged(ObservableCollection<Tag> tags)
        {
            TagsChangedHandler?.Invoke(tags);
        }

        /// <summary>
        /// AP status handler
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <param name="status">AP status</param>
        public void ProcessApStatus(string id, ApStatus status)
        {
            ApStatusHandler?.Invoke(status);
        }

        /// <summary>
        /// AP data handler
        /// </summary>
        /// <param name="data">AP data</param>
        public void ProcessApData(string data) => RecvQueue.Enqueue(data);

        private void HandleAutoRegisterChanged(bool isAutoRegister)
        {
            this.isAutoRegister = isAutoRegister;
        }

        private void HandleTagsChanged(ObservableCollection<Tag> tags)
        {
            this.tags = tags;
        }

        /// <summary>
        /// Send data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="id">Client ID</param>
        /// <param name="t">Data object</param>
        /// <returns>Send result</returns>
        public async Task<SendResult> Send<T>(T t) where T : SequenceEntity
        {
            t.Sequence = Sequence++;    // Just for demo
            var connType = Server?.Clients.FirstOrDefault(x => x.Value.Status == ApStatus.Online).Value?.ConnType;
            if (connType is null)
                return SendResult.Offline;

            var topic = "recv";
            var payload = JsonSerializer.Serialize(t);

            if (connType == 0)
            {
                topic = t.Code switch
                {
                    0x70 => "time",
                    0x81 => "config",
                    0x83 => "task",
                    0x84 => "group",
                    0x85 => "bind",
                    0x87 => "ota",
                    0x88 => "certificate",
                    _ => topic
                };
                switch (t.Code)
                {
                    case 0x70:
                        var time = t as Time;
                        if (time == null) return SendResult.Error;
                        payload = time.ApTime.ToString("yyyy-MM-dd HH:mm:ss");
                        break;
                    case 0x81:
                        var config = t as ApConfig;
                        if (config == null) return SendResult.Error;
                        payload = JsonSerializer.Serialize(config.Config);
                        break;
                    case 0x83:
                        var order = t as Order;
                        if (order == null) return SendResult.Error;
                        var orderV1 = new OrderV1
                        {
                            Time = order.Time,
                            Items = order.Items.Select(item => new TaskItemData
                            {
                                TagID = item.TagID,
                                Beep = item.Beep,
                                Colors = [TransColor(item.Color)],
                                Flashing = item.Flashing 
                            }).ToArray() ?? []
                        };
                        payload = JsonSerializer.Serialize(orderV1);
                        break;
                    case 0x84:
                        var group = t as GroupOrder;
                        if (group == null) return SendResult.Error;
                        var color = TransColor(group.Color);
                        var groupV1 = new GroupOrderV1
                        {
                            Group = group.StartGroup,
                            Times = group.Time,
                            R = color.R,
                            G = color.G,
                            B = color.B,
                            Beep = group.Beep,
                            Flashing = group.Flashing
                        };
                        payload = JsonSerializer.Serialize(groupV1);
                        break;
                    case 0x85:
                        var bind = t as Bind;
                        if (bind == null) return SendResult.Error;
                        var bindV1 = new BindV1
                        {
                            Group = bind.Items.First().Group,
                            Items = bind.Items.Select(x => x.TagID).ToArray() ?? []
                        };
                        payload = JsonSerializer.Serialize(bindV1);
                        break;
                    case 0x87:
                        var ota = t as OTA;
                        if (ota == null) return SendResult.Error;
                        var otaV1 = new OTAV1
                        {
                            Factory = ota.Factory,
                            Version = ota.Version.ToString(),
                            Firmware = ota.Firmware,
                            MD5 = ota.MD5,
                            Type = ota.Type,
                        };
                        payload = JsonSerializer.Serialize(otaV1);
                        break;
                    case 0x88:
                        var cert = t as CertificateOrder;
                        if (cert == null) return SendResult.Error;
                        var certV1 = new CertificateV1
                        {
                            CAChain = cert.CAChain,
                            CAChainMD5 = cert.CAChainMD5,
                            Certificate = cert.Certificate,
                            CertificateMD5 = cert.CertificateMD5,
                            Password = cert.Password
                        };
                        payload = JsonSerializer.Serialize(certV1);
                        break;
                }
            }
            else
            {
                if(t.Code == 0x70)
                    return SendResult.Error; // 仅1.0协议支持修改基站时间
            }

            DebugRequestHandler?.Invoke(new DebugItem(t.Code, payload));
            return await Server.Send(topic, payload);
        }

        /// <summary>
        /// Get AP config
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <returns>AP config</returns>
        public ApConfigB GetConfig(string id) => Server.GetApConfig(id);

        public static int GetColor(Color color)
        {
            var data = 0;
            data += (byte)(color.R ? 1 : 0); // Bit 2
            data <<= 1;
            data += (byte)(color.G ? 1 : 0); // Bit 1
            data <<= 1;
            data += (byte)(color.B ? 1 : 0); // Bit 0
            return data;
        }

        public static Color TransColor(int color)
        {
            // color: 0-7, 低三位分别为 B(1), G(2), R(4)
            return new Color
            {
                R = (color & 0b100) != 0,
                G = (color & 0b010) != 0,
                B = (color & 0b001) != 0
            };
        }
    }
}
