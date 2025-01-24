using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Model;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Serilog;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;

namespace eStation_PTL_Demo.Core
{
    internal class MQTT : PTLServer
    {
        private MqttServer mqttServer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="actStatus"></param>
        /// <param name="actData"></param>
        public MQTT(ConnInfo info, Action<string, ApStatus> actStatus, Action<string> actData)
            : base(info, actStatus, actData)
        {
        }

        /// <summary>
        /// Run MQTT service
        /// </summary>
        /// <returns></returns>
        public override bool Run()
        {
            try
            {
                var builder = new MqttServerOptionsBuilder();
                if (Connection.Security)
                {
                    builder = builder
                        .WithEncryptedEndpoint()
                        .WithEncryptedEndpointPort(Connection.Port)
                        .WithEncryptionCertificate(GetCertificate2(Connection))
                        .WithEncryptionSslProtocol(SslProtocols.Tls12);
                }
                else
                {
                    builder = builder
                        .WithDefaultEndpoint()
                        .WithDefaultEndpointPort(Connection.Port);
                }
                var options = builder.Build();
                mqttServer = new MqttFactory().CreateMqttServer(options);
                mqttServer.ClientConnectedAsync += ClientConnectedAsync;
                mqttServer.ClientDisconnectedAsync += ClientDisconnectedAsync;
                mqttServer.ValidatingConnectionAsync += ValidatingConnectionAsync;
                mqttServer.InterceptingPublishAsync += InterceptingPublishAsync;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MQTT_RUN_ERR");
                return false;
            }
        }

        /// <summary>
        /// Client connected
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Task ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            ApStatusHandler(arg.ClientId, ApStatus.Connecting);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Client disconnected
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>The task</returns>
        private Task ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            ApStatusHandler(arg.ClientId, ApStatus.Offline);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Validating client connection
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>The task</returns>
        private Task ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            arg.ReasonCode = arg.UserName == Connection.UserName && arg.Password == Connection.Password
                ? MqttConnectReasonCode.Success
                : MqttConnectReasonCode.UnspecifiedError;

            if (arg.ReasonCode == MqttConnectReasonCode.Success)
            {
                mqttServer.SubscribeAsync(arg.ClientId, $"/estation/{arg.ClientId}/send");
                //Clients.Add((true, arg.ClientId));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Intercepting publish
        /// </summary>
        /// <param name="arg">Message</param>
        /// <returns>The task</returns>
        private Task InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            lock (locker)
            {
                ApDataHandler(Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment));
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Send data to client
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="t">Data object</param>
        /// <returns>Result</returns>
        public override async Task<SendResult> Send<T>(T t)
        {
            try
            {
                if (Clients.Count == 0) return SendResult.NotExist;
                var client = Clients.FirstOrDefault();
                if (client.Value.Status != ApStatus.Online) return SendResult.Offline;

                var mqtt = new MqttApplicationMessageBuilder()
                    .WithTopic("/estation/recv")
                    .WithTopicAlias(ushort.Parse(client.Key))
                    .WithPayload(JsonSerializer.Serialize(t))
                    .Build();
                await mqttServer.InjectApplicationMessage(
                    new InjectedMqttApplicationMessage(mqtt) { SenderClientId = client.Key });
                return SendResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MQTT_SEND_ERR");
                return SendResult.Error;
            }
        }
    }
}
