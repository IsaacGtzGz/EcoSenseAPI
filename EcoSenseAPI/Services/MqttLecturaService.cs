using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Text;
using System.Text.Json;
using EcoSenseAPI.Models;
using EcoSenseAPI.Data;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EcoSenseAPI.Services
{
    public class MqttLecturaService
    {
        private readonly IMqttClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _topicLecturas = "sensores/lecturas";
        private readonly HttpClient _httpClient;

        public MqttLecturaService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();
            _httpClient = new HttpClient();

            _client.ApplicationMessageReceivedAsync += async e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var topic = e.ApplicationMessage.Topic;

                Console.WriteLine($"📥 [{topic}] = {payload}");

                try
                {
                    // Envía el JSON recibido como POST al endpoint de la API
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("https://localhost:7181/api/Lecturas/nueva", content); // Cambia el puerto si tu API usa otro

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("✅ Lectura enviada al endpoint y procesada correctamente");
                    }
                    else
                    {
                        Console.WriteLine($"❌ Error al enviar lectura al endpoint: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al procesar lectura MQTT: {ex.Message}");
                }
            };

            _client.ConnectedAsync += async e =>
            {
                Console.WriteLine("✅ Conectado al broker MQTT");
                await _client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f
                        .WithTopic(_topicLecturas)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce))
                    .Build());
            };
        }

        public async Task InitAsync()
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("6861ddd63ae7462298cb0ef32349bcff.s1.eu.hivemq.cloud", 8883)
                .WithCredentials("Rayret", "Jasso24#")
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    AllowUntrustedCertificates = true,
                    IgnoreCertificateChainErrors = true,
                    IgnoreCertificateRevocationErrors = true
                })
                .Build();

            await _client.ConnectAsync(options, CancellationToken.None);
        }
    }
}