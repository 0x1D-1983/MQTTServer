using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;

namespace MQTTServer
{
    public class MqttServerClientSubscribedTopicHandler : IMqttServerClientSubscribedTopicHandler
    {
        private readonly ILogger _logger;

        public MqttServerClientSubscribedTopicHandler(ILogger<MqttServerClientSubscribedTopicHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task HandleClientSubscribedTopicAsync(MqttServerClientSubscribedTopicEventArgs eventArgs)
        {
            _logger.LogInformation(eventArgs.ClientId + " subscribed to " + eventArgs.TopicFilter);

            return Task.CompletedTask;
        }
    }
}
