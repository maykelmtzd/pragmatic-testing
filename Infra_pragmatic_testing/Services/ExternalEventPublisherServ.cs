using System;
using System.Collections.Generic;
using System.Text;
using Infra_pragmatic_testing.Configurations;
using Infra_pragmatic_testing.ExternalEvents;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Infra_pragmatic_testing.Services
{
	public class ExternalEventPublisherServ
	{
		private readonly EventGridGateway _eventGridGateway;
		private readonly IOptions<EventGridSettings> _eventGridSettings;
		private readonly AsyncRetryPolicy _retryPolicy;
		private readonly ILogger _logger;

		public ExternalEventPublisherServ(EventGridGateway eventGridGateway, IOptions<EventGridSettings> eventGridSettings, AsyncRetryPolicy retryPolicy, ILogger logger)
		{
			_eventGridGateway = eventGridGateway;
			_eventGridSettings = eventGridSettings;
			_retryPolicy = retryPolicy;
			_logger = logger;
		}

        public async void PublishAsync(ExternalEvent externalEvent)
        {
            var eventGridEvent = MapToEventGridEvent(externalEvent);
            try
            {
                var topicHostName = new Uri(_eventGridSettings.Value.InvoiceManagementTopicEndpoint).Host;

                await _retryPolicy.ExecuteAsync(async ctx =>
                {
                    _logger.Log(LogLevel.Debug,
                        $"Publishing event:{eventGridEvent.EventType} to EventGrid. RetryCount:{ctx["retryCount"]} {DateTime.Now}");
                    await _eventGridGateway.PublishEventsWithHttpMessagesAsync(topicHostName, new List<EventGridEvent> { eventGridEvent });
                }, new Context { { "retryCount", 1 } });

            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while publishing to event grid", ex);
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
