using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Model;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace eStation_PTL_Demo.Core
{
    internal abstract class PTLServer
    {

        /// <summary>
        /// Connection information
        /// </summary>
        public ConnInfo Connection { get; protected set; }
        protected readonly object locker = new();
        protected readonly Dictionary<string, Ap> Clients = [];
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
        /// Send data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract Task<SendResult> Send<T>(T t) where T : BaseEntity;

        /// <summary>
        /// Get X509 certificate file
        /// </summary>
        /// <param name="info">Connection infor</param>
        /// <returns>X509 certificate</returns>
        protected X509Certificate2 GetCertificate2(ConnInfo info)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), info.Certificate);
            return X509Certificate2.CreateFromPemFile(path, info.Key);
        }
    }
}
