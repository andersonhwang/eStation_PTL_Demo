using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Model;
using Serilog;
using System.Collections.Concurrent;
using System.Text.Json;

namespace eStation_PTL_Demo.Core
{
    /// <summary>
    /// Send service
    /// </summary>
    public class SendService
    {
        private PTLServer Server;
        private static SendService instance = new();
        protected readonly ConcurrentQueue<string> RecvQueue = [];

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

        /// <summary>
        /// Run send service
        /// </summary>
        /// <param name="info">Connection information</param>
        public bool Run(ConnInfo info)
        {
            try
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            if (!RecvQueue.TryDequeue(out var data))
                            {
                                await Task.Delay(200);
                                continue;
                            }

                            var entity = JsonSerializer.Deserialize<BaseEntity>(data);
                            if (entity is null) continue;
                            switch (entity.Code)
                            {
                                case 0x01:
                                    var infor = JsonSerializer.Deserialize<ApInfor>(data);
                                    if (infor is null) continue;
                                    Server?.UpdateApConfig(infor);
                                    ApInforHandler?.Invoke(infor);
                                    ApStatusHandler?.Invoke(ApStatus.Online);
                                    break;
                                case 0x02:
                                    var heartbeat = JsonSerializer.Deserialize<ApHeartbeat>(data);
                                    if (heartbeat is null) continue;
                                    ApHeartbeatHandler?.Invoke(heartbeat);
                                    break;
                                case 0x03:
                                    var response = JsonSerializer.Deserialize<TaskResponse>(data);
                                    if (response is null) continue;
                                    TaskResponseHandler?.Invoke(response);
                                    break;
                                case 0x04:
                                    var result = JsonSerializer.Deserialize<TaskResult>(data);
                                    if (result is null) continue;
                                    TaskResultHandler?.Invoke(result);
                                    break;
                                default: break;
                            }
                            DebugResponseHandler?.Invoke(new DebugItem(entity.Code, data));
                        }
                        catch
                        {
                            // Not important
                        }
                    }
                });

                Server = info.ConnType switch
                {
                    ConnType.MQTT => new MQTT(info, ProcessApStatus, ProcessApData),
                    ConnType.WebSocket => new WebSocket(info, ProcessApStatus, ProcessApData),
                    _ => new MQTT(info, ProcessApStatus, ProcessApData),
                };
                return Server.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Send_Service_Error");
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
            DebugRequestHandler?.Invoke(new DebugItem(t.Code, JsonSerializer.Serialize(t)));
            return await Server.Send(t);
        }

        /// <summary>
        /// Get AP config
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <returns>AP config</returns>
        public ApConfigB GetConfig(string id) => Server.GetApConfig(id);
    }
}
