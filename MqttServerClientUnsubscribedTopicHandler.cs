using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;

namespace MQTTServer
{
    public class MqttServerClientUnsubscribedTopicHandler : IMqttServerClientUnsubscribedTopicHandler
    {
        private readonly ILogger _logger;

        public MqttServerClientUnsubscribedTopicHandler(ILogger<MqttServerClientUnsubscribedTopicHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task HandleClientUnsubscribedTopicAsync(MqttServerClientUnsubscribedTopicEventArgs eventArgs)
        {
            _logger.LogInformation(eventArgs.ClientId + " unsubscribed to " + eventArgs.TopicFilter);

            return Task.CompletedTask;
        }
    }
}
