using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Model;

namespace eStation_PTL_Demo.Core
{
    internal class WebSocket : PTLServer
    {
        public WebSocket(ConnInfo info, Action<string, ApStatus> actStatus, Action<string> actData) 
            : base(info, actStatus, actData)
        {
        }

        public override bool Run()
        {
            // TODO
            return false;
        }

        public override Task<SendResult> Send<T>(T t)
        {
            throw new NotImplementedException();
        }
    }
}
