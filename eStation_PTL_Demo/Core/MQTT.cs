using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStation_PTL_Demo.Core
{
    internal class MQTT
    {
        private readonly MqttServer _theFactory = new();
        private MqttServer _theServer;
    }
}
