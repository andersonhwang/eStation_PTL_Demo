using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Helper;
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
                var builder = new MqttServerOptionsBuilder()
                        .WithDefaultEndpoint();
                if (Connection.Security)
                {
                    builder = builder
                        .WithEncryptedEndpoint()
                        .WithEncryptedEndpointPort(Connection.Port)
                        .WithEncryptionCertificate(FileHelper.GetCertificate(Connection.Certificate, Connection.Key))
                        .WithEncryptionSslProtocol(SslProtocols.Tls12);
                }
                else
                {
                    builder = builder
                        .WithDefaultEndpointPort(Connection.Port);
                }
                var options = builder.Build();
                mqttServer = new MqttFactory().CreateMqttServer(options);
                mqttServer.ClientConnectedAsync += ClientConnectedAsync;
                mqttServer.ClientDisconnectedAsync += ClientDisconnectedAsync;
                mqttServer.ValidatingConnectionAsync += ValidatingConnectionAsync;
                mqttServer.InterceptingPublishAsync += InterceptingPublishAsync;
                mqttServer.StartAsync();
                Log.Information("MQTT_RUN_OK");
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
        private Task ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            ApStatusHandler(arg.ClientId, ApStatus.Connecting);
            Log.Information($"{arg.ClientId}({arg.Endpoint}) Connected");
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
            Log.Information($"{arg.ClientId}({arg.Endpoint}) Disconnected");
            if (Clients.TryGetValue(arg.ClientId, out Ap? value)) value.Status = ApStatus.Offline;
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
                ApStatusHandler(arg.ClientId, ApStatus.Online);
                if (Clients.TryGetValue(arg.ClientId, out Ap? value)) value.Status = ApStatus.Online;
                else Clients.Add(arg.ClientId, new Ap { ID = arg.ClientId, Status = ApStatus.Online, IP = arg.Endpoint });
            }

            Log.Information($"{arg.ClientId}({arg.Endpoint}) validating connection:{arg.ReasonCode}");
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
                    .WithTopic($"/estation/{client.Key}/recv")
                    .WithPayload(JsonSerializer.Serialize(t))
                    .Build();

                Log.Information(Convert.ToHexString(mqtt.PayloadSegment.ToArray()) + " " + mqtt.Topic);
                await mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(mqtt));
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
