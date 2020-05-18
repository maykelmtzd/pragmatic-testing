using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Infra_pragmatic_testing.Configurations;
using Infra_pragmatic_testing.ExternalEvents;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Infra_pragmatic_testing.Services
{
    /// <summary>
    /// This interface(IExternalEventPublisherServ) is not necessary. Why:
    /// 
    /// 1- It's not at the edge of the hexagon/microservice. We don't need to mock it, it's an implementation detail.
    /// IEventGridGateway is a valuable interface, it's at the very edge of the microservice.
    /// 
    /// 2- We don't have more than one concrete implementation for it. So it's just a header interface.
    /// </summary>
    public class ExternalEventPublisherServ : IExternalEventPublisherServ
    {
        private readonly IEventGridGateway _eventGridGateway;
        private readonly IOptions<EventGridSettings> _eventGridSettings;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILogger<ExternalEventPublisherServ> _logger;

        public ExternalEventPublisherServ(IEventGridGateway eventGridGateway, 
            IOptions<EventGridSettings> eventGridSettings, AsyncRetryPolicy retryPolicy, ILogger<ExternalEventPublisherServ> logger)
        {
            _eventGridGateway = eventGridGateway;
            _eventGridSettings = eventGridSettings;
            _retryPolicy = retryPolicy;
            _logger = logger;
        }

        public async Task PublishAsync(ExternalEvent externalEvent)
        {
            var eventGridEvent = MapToEventGridEvent(externalEvent);
            try
            {
                //TODO Try to read the endpoint from configuration
                //var topicHostName = new Uri(_eventGridSettings.Value.InvoiceManagementTopicEndpoint).Host;
                var topicHostName = new Uri("http://someendpoint").Host;

                await _retryPolicy.ExecuteAsync(async ctx =>
                {
                    _logger.Log(LogLevel.Debug,
                        $"Publishing event:{eventGridEvent.EventType} to EventGrid. RetryCount:{ctx["retryCount"]} {DateTime.Now}");
                    _eventGridGateway.PublishEventsWithHttpMessagesAsync(topicHostName, new List<EventGridEvent> { eventGridEvent });
                }, new Context { { "retryCount", 1 } });

            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while publishing to event grid", ex);
                throw;
            }
        }

        internal EventGridEvent MapToEventGridEvent(ExternalEvent externalEvent)
        {
            return new EventGridEvent()
            {
                Id = Guid.NewGuid().ToString(),
                EventType = externalEvent.EventType,
                Data = externalEvent.Data,
                EventTime = DateTime.Now,
                Subject = externalEvent.Subject,
                DataVersion = externalEvent.DataVersion
            };
        }
    }
}
