using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Model;
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

        /// <summary>
        /// Instance of send service
        /// </summary>
        public static SendService Instance => instance;

        /// <summary>
        /// AP status delegate
        /// </summary>
        /// <param name="infor">AP information</param>
        public delegate void ApInforDelegate(ApInfor infor);
        public event ApInforDelegate? ApInforHandler;
        /// <summary>
        /// AP data delegate
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
        /// Run send service
        /// </summary>
        /// <param name="info">Connection information</param>
        public void Run(ConnInfo info)
        {
            Server = info.ConnType switch
            {
                ConnType.MQTT => new MQTT(info, ApStatusHandler, ApDataHandler),
                ConnType.WebSocket => new WebSocket(info, ApStatusHandler, ApDataHandler),
                _ => new MQTT(info, ApStatusHandler, ApDataHandler),
            };
            Server.Run();

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
                                ApInforHandler?.Invoke(infor);
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
                    }
                    catch
                    {
                        // Not important
                    }
                }
            });
        }

        /// <summary>
        /// Register AP status event
        /// </summary>
        /// <param name="infor">AP status event</param>
        public void Register(ApInforDelegate infor) => ApInforHandler += infor;

        /// <summary>
        /// Register AP data event
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
        /// AP status handler
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <param name="status">AP status</param>
        public void ApStatusHandler(string id, ApStatus status)
        {

        }

        /// <summary>
        /// AP data handler
        /// </summary>
        /// <param name="data">AP data</param>
        public void ApDataHandler(string data) => RecvQueue.Enqueue(data);

        /// <summary>
        /// Send data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="id">Client ID</param>
        /// <param name="t">Data object</param>
        /// <returns>Send result</returns>
        public async Task<SendResult> Send<T>(T t) where T : BaseEntity
        {
            return await Server.Send(t);
        }
    }
}
