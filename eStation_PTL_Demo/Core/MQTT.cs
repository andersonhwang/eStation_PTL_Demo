using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Entity.V_1;
using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Helper;
using eStation_PTL_Demo.Model;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Serilog;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                        .WithEncryptionSslProtocol(SslProtocols.Tls12)
                        .WithClientCertificate(ValidateClientCertificate);
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
        /// Stop MQTT service
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            try
            {
                if (mqttServer != null)
                {
                    mqttServer.StopAsync().GetAwaiter().GetResult();
                    Log.Information("MQTT_STOP_OK");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MQTT_STOP_ERR");
                return false;
            }
        }

        private static bool ValidateClientCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None || sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
            {
                return true;
            }

            // 处理自签名证书：如果错误仅为链接错误，认为是自签名证书，允许通过
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
            {
                var x509Certificate = new X509Certificate2(certificate);

                // 检查证书是否自签名（颁发者和主题相同）
                bool isSelfSigned = x509Certificate.Subject == x509Certificate.Issuer;

                if (isSelfSigned)
                {
                    Log.Information($"接受自签名证书: {x509Certificate.Subject}");
                    return true;
                }

                // 如果有证书链，尝试使用自定义策略验证
                if (chain != null)
                {
                    // 创建一个新链进行验证
                    X509Chain customChain = new();

                    // 配置链验证策略
                    customChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck; // 不检查吊销
                    customChain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                    customChain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;

                    // 进行链验证
                    bool isValid = customChain.Build(x509Certificate);

                    // 记录验证结果
                    if (isValid)
                    {
                        Log.Information($"证书链验证通过: {x509Certificate.Subject}, 链长度: {customChain.ChainElements.Count}");
                        return true;
                    }
                    else
                    {
                        var errors = string.Join(", ", customChain.ChainStatus.Select(s => s.StatusInformation));
                        Log.Warning($"证书链验证失败: {errors}, 链长度: {customChain.ChainElements.Count}");
                    }
                }
            }

            return false;
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
                var clientID = arg.ClientId;
                if (Clients.TryGetValue(clientID,out var ap))
                {
                    var connectType = ap.ConnType;
                    if (ap.ConnType == -1)
                    {
                        var connTypeStr = GetPropFromConfigB(Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment), "ConnType");
                        var encryptedStr = GetPropFromConfigB(Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment), "Encrypted");
                        if (!string.IsNullOrWhiteSpace(connTypeStr))
                            ap.ConnType = Int32.Parse(connTypeStr);
                        if (!string.IsNullOrWhiteSpace(encryptedStr))
                            ap.Encrypted = bool.Parse(encryptedStr);

                        connectType = ap.ConnType;
                    }
                    var message = new Message
                    {
                        ConnType = connectType,
                        Data = connectType switch
                        {
                            0 => JsonSerializer.Serialize(new ApData
                            {
                                Id = arg.ClientId,
                                Topic = arg.ApplicationMessage.Topic,
                                PayloadSegment = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment)
                            }),
                            _ => Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment)
                        }
                    };
                    ApDataHandler(JsonSerializer.Serialize(message));
                }
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Send data to client
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="t">Data object</param>
        /// <returns>Result</returns>
        public override async Task<SendResult> Send(string topic, string payload)
        {
            try
            {
                if (Clients.Count == 0) return SendResult.NotExist;
                var client = Clients.FirstOrDefault(x=>x.Value.Status == ApStatus.Online);
                if (client.Value == null) return SendResult.Offline;

                var mqtt = new MqttApplicationMessageBuilder()
                    .WithTopic($"/estation/{client.Key}/{topic}")
                    .WithPayload(payload)
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

        private static string? GetPropFromConfigB(string jsonString, string prop)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonString);

                if (doc.RootElement.TryGetProperty("ConfigB", out JsonElement configElement2))
                {
                    if (configElement2.TryGetProperty(prop, out JsonElement nestedConnTypeElement))
                    {
                        return nestedConnTypeElement.GetRawText();
                    }
                }
            }
            catch (JsonException)
            {
            }

            return null;
        }

        public static Color TransColor(int color)
        {
            // color: 0-7, 低三位分别为 B(1), G(2), R(4)
            return new Color
            {
                R = (color & 0b100) != 0,
                G = (color & 0b010) != 0,
                B = (color & 0b001) != 0
            };
        }
    }
}
