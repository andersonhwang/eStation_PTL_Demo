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

        public override Task<SendResult> Send(string topic, string payload)
        {
            throw new NotImplementedException();
        }

        public override bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}
