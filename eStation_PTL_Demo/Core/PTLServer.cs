using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Model;

namespace eStation_PTL_Demo.Core
{
    internal abstract class PTLServer
    {
        /// <summary>
        /// Connection information
        /// </summary>
        public ConnInfo Connection { get; protected set; }
        protected readonly object locker = new();
        public readonly Dictionary<string, Ap> Clients = [];
        protected readonly Action<string, ApStatus> ApStatusHandler;
        protected readonly Action<string> ApDataHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">Connection information</param>
        /// <param name="actStatus">AP status handler</param>
        /// <param name="actData">AP data handler</param>
        public PTLServer(ConnInfo info, Action<string, ApStatus> actStatus, Action<string> actData)
        {
            Connection = info;
            ApStatusHandler = actStatus;
            ApDataHandler = actData;
        }

        /// <summary>
        /// Run method
        /// </summary>
        /// <returns>The result</returns>
        public abstract bool Run();

        /// <summary>
        /// Stop server
        /// </summary>
        /// <returns>The result</returns>
        public abstract bool Stop();

        /// <summary>
        /// Send data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract Task<SendResult> Send(string topic, string payload);

        /// <summary>
        /// Get AP config
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <returns>AP config</returns>
        public ApConfigB GetApConfig(string id)
        {
            if(Clients.ContainsKey(id)) return Clients[id].ConfigB;
            return new ApConfigB();
        }

        /// <summary>
        /// Update AP config
        /// </summary>
        /// <param name="apInfor">AP Infor</param>
        public void UpdateApConfig(ApInfor apInfor)
        {
            if(Clients.ContainsKey(apInfor.ID)) Clients[apInfor.ID].ConfigB = apInfor.ConfigB;
        }
    }
}
