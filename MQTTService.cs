using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Server;

namespace MQTTServer
{
    public class MQTTService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly MQTTConfig _config;
        private IMqttServer _mqttServer;
        private readonly IMqttServerClientSubscribedTopicHandler _subscribedHandler;
        private readonly IMqttServerClientUnsubscribedTopicHandler _unsubscribedHandler;

        public MQTTService(
            ILogger<MQTTService> logger,
            IOptions<MQTTConfig> config,
            IMqttServerClientSubscribedTopicHandler subscribedHandler,
            IMqttServerClientUnsubscribedTopicHandler unsubscribedHandler)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config.Value;
            _subscribedHandler = subscribedHandler ?? throw new ArgumentNullException(nameof(subscribedHandler));
            _unsubscribedHandler = unsubscribedHandler ?? throw new ArgumentNullException(nameof(unsubscribedHandler));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting MQTT Daemon on port " + _config.Port);

            //Building the config
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(1000)
                .WithDefaultEndpointPort(_config.Port);


            //Getting an MQTT Instance
            _mqttServer = new MqttFactory().CreateMqttServer();

            //Wiring up all the events...

            _mqttServer.ClientSubscribedTopicHandler = _subscribedHandler;

            _mqttServer.ClientUnsubscribedTopicHandler = _unsubscribedHandler;

            _mqttServer.UseClientConnectedHandler(
                e => _logger.LogInformation(e.ClientId + " Connected."));

            _mqttServer.UseClientDisconnectedHandler(
                e => _logger.LogInformation(e.ClientId + " Disonnected."));

            _mqttServer.UseApplicationMessageReceivedHandler(
                e => _logger.LogInformation(e.ClientId + " published message to topic " + e.ApplicationMessage.Topic));

            //Now, start the server -- Notice this is resturning the MQTT Server's StartAsync, which is a task.
            return _mqttServer.StartAsync(optionsBuilder.Build());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping MQTT Daemon.");
            return _mqttServer.StopAsync();
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");

        }
    }
}
