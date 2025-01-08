using eStation_PTL_Demo.Model;
using eStation_PTL_Demo.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;

namespace eStation_PTL_Demo.Core
{
    internal class Worker
    {
        static readonly object _locker = new();
        static readonly Worker instance = new();
        readonly Queue<byte[]> ReceiveQueue = new();
        readonly List<byte> ReceiveBuffer = [];
        TcpClient? Client;
        IPEndPoint Point;
        /// <summary>
        /// AP is online
        /// </summary>
        public bool IsOnline { get { if (Client is null) return false; return Client.IsConnect; } }
        public Func<bool, int> FuncToken { get; private set; }
        public Action<ApInfor>? ApInforHandler { get; private set; }
        public Action<PtlInfor>? TagsHandler { get; private set; }

        public delegate void ApStatusDelegate(ApStatus status, int cmd, int count);
        public delegate void ApMsgDelegate(PddMsg msg);

        public event ApStatusDelegate? ApStatusEvent;
        public event ApMsgDelegate? ApMsgEvent;

        public static Worker Instance
        {
            get { return instance; }
        }
        public void Register(Func<bool, int> token) => FuncToken = token;
        public void Register(Action<ApInfor> ap) => ApInforHandler = ap;
        public void Register(Action<PtlInfor> tags) => TagsHandler = tags;
        public void Register(ApStatusDelegate status) => ApStatusEvent += status;
        public void Register(ApMsgDelegate msg) => ApMsgEvent += msg;

        /// <summary>
        /// Run the worker
        /// </summary>
        /// <param name="ap">AP result handler</param>
        /// <param name="tags">Tags result handler</param>
        public void Run()
        {
            Run(ProcessResult);
        }

        /// <summary>
        /// Task Runner
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="speed">Wait speed, default is 1s</param>
        void Run(Action action, int speed = 100)
        {
            int errorCount = 0;
            Exception previous = new();

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        action();
                        if (ReceiveQueue.Count != 0 || ReceiveBuffer.Count != 0) continue;
                        await Task.Delay(TimeSpan.FromMilliseconds(speed));
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Equals(previous.Message))
                        {
                            previous = ex;
                            errorCount = 0;
                            Log.Error(ex, "Error_Producer");
                        }
                        else
                        {
                            if (++errorCount > 1000)
                            {
                                Log.Error(ex, "Error_Producer_Long");
                                errorCount = 0;
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Connect to AP
        /// </summary>
        /// <param name="point">AP address</param>
        public void Connect(ConnInfo infor)
        {
            try
            {
                Point = point;
                ApStatusEvent?.Invoke(ApStatus.Connecting, 0, 0);
                Client = new TcpClient(ReceiveData, ClientError);
                Client.TryConenct(point);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Connect to {point} failed.");
                ApStatusEvent?.Invoke(ApStatus.ConnectError, 0, 0);
            }
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        public void Disconnect()
        {
            ApStatusEvent?.Invoke(ApStatus.Offline, -1, 0);
        }

        /// <summary>
        /// Auto test
        /// </summary>
        /// <param name="list"></param>
        public void GenTask(List<string> list)
        {

        }

        /// <summary>
        /// Send data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void SendData<T>(T t, int cmd, int count) where T : PddBaseToekn
        {
            t.token = FuncToken(true);
            var data = PddProtocol.PackageData(t);
            Client?.Send(data);
            ApStatusEvent?.Invoke(ApStatus.Working, cmd, count);
        }

        public void Query()
        {
            var data = PddProtocol.PackageData(new ApQuery());
            Client?.Send(data);
        }

        /// <summary>
        /// Receive data
        /// </summary>
        /// <param name="bytes"></param>
        public void ReceiveData(byte[] bytes)
            => ReceiveQueue.Enqueue(bytes);

        /// <summary>
        /// Client error
        /// </summary>
        /// <param name="error">Error message</param>
        public void ClientError(string error)
        {
            try
            {
                ApStatusEvent?.Invoke(ApStatus.Offline, -1, 0);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Re-connect error");
            }
        }

        /// <summary>
        /// Process result
        /// </summary>
        public void ProcessResult()
        {
            try
            {
                if (ReceiveQueue.Count > 0)
                {
                    var buffer = ReceiveQueue.Dequeue();
                    ReceiveBuffer.AddRange(buffer);
                    //Log.Information(Convert.ToHexString(buffer));
                }

                if (ReceiveBuffer.Count == 0) return;

                var i = FindMagic(ReceiveBuffer);
                if (i == -1 || i + 10 > ReceiveBuffer.Count) return; // Not find/enough

                var size = ByteHelper.Byte2Int([.. ReceiveBuffer[(i + 4)..(i + 8)]]);
                if (size + i > ReceiveBuffer.Count) return; // Not enough

                var data = ReceiveBuffer[i..(size - 2)];
                if (!CrcHelper.Crc16(data).SequenceEqual(ReceiveBuffer[(size - 2)..size])) goto DROP;

                byte[] payload = [.. data[(i + 8)..]];
                var t = PddProtocol.UnpackageData<PddBase>(payload);
                if (t == null) goto DROP;

                switch (t.command)
                {
                    case 0x8000:
                        var request = PddProtocol.UnpackageData<ApRequest>(payload);
                        if (request is null) goto DROP;
                        var response = PddProtocol.GenResponse(request);
                        Log.Information("Conn_Request:" + JsonSerializer.Serialize(request));
                        Client?.Send(response);
                        ApInforHandler?.Invoke(new ApInfor { sn = request.sn, manufacturer = request.manufacturer, version = request.version });
                        ApStatusEvent?.Invoke(ApStatus.Online, 0x8000, 0);
                        break;
                    case 0x8001:
                        var apInfor = PddProtocol.UnpackageData<ApInfor>(payload);
                        if (apInfor == null) goto DROP;
                        ApInforHandler?.Invoke(apInfor); // Just update last heartbeat
                        //Log.Information("Heartbaet:" + JsonSerializer.Serialize(apInfor));
                        break;
                    case 0x8002:
                        var apMsg = PddProtocol.UnpackageData<PddMsg>(payload);
                        if (apMsg == null) goto DROP;
                        ApMsgEvent?.Invoke(apMsg);
                        Log.Information("AP_Message:" + JsonSerializer.Serialize(apMsg));
                        break;
                    case 0x8003:
                        var pddLog = PddProtocol.UnpackageData<PddLog>(payload);
                        if (pddLog == null) goto DROP;
                        Log.Information("AP_Debug:" + JsonSerializer.Serialize(pddLog));
                        break;
                    case 0x8011:
                        var ptlInfor = PddProtocol.UnpackageData<PtlInfor>(payload);
                        if (ptlInfor == null) goto DROP;
                        TagsHandler?.Invoke(ptlInfor);
                        Log.Information(JsonSerializer.Serialize(ptlInfor));
                        if (/*ptlInfor.Items.Count() > 0 && */ptlInfor.reason == 1) ApStatusEvent?.Invoke(ApStatus.Receive, 0x8011, 1); // Just update last receive
                        break;
                }

            DROP:
                // Drop
                var j = FindMagic(ReceiveBuffer, i + 4);
                if (j > 0) ReceiveBuffer.RemoveRange(0, j);
                else ReceiveBuffer.Clear();
            }
            catch (Exception ex)
            {
                Log.Information($"Error: {ex.Message}");
                Log.Information(Convert.ToHexString(ReceiveBuffer.ToArray()));
                ReceiveBuffer.Clear();
            }
        }

        /// <summary>
        /// Find magic start index
        /// </summary>
        /// <param name="bytes">Bytes to find</param>
        /// <param name="offset">Offset</param>
        /// <returns>Magic start index</returns>
        static int FindMagic(List<byte> bytes, int offset = 0)
        {
            if (bytes.Count < 10) return -1;
            for (int i = offset, j = bytes.Count - 4; i < j; i++)
            {
                if (bytes[i..(i + 4)].SequenceEqual(PddProtocol.Magic)) return i;
            }
            return -1;
        }

        /// <summary>
        /// Get color
        /// </summary>
        /// <param name="blink">Blink</param>
        /// <param name="beep">Beep</param>
        /// <param name="color">Color</param>
        /// <returns></returns>
        static byte GetColor(bool blink, bool beep, int color)
            => (byte)((beep ? 0x08 : 0x00) | (color == 0 ? 0x00 : (blink ? 0xC0 : 0x40)) | color);
    }
}
