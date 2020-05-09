using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Rest.Azure;

namespace Infra_pragmatic_testing.Services
{
	/// <summary>
	/// Creates a wrapper over a third party library client.
	/// Don't mock what you don't own: https://github.com/testdouble/contributing-tests/wiki/Don't-mock-what-you-don't-own
	/// </summary>
	public class EventGridGateway
	{
		private readonly EventGridClient _eventGridClient;

		public EventGridGateway(EventGridClient eventGridClient)
		{
			_eventGridClient = eventGridClient;
		}

		public Task<AzureOperationResponse> PublishEventsWithHttpMessagesAsync(string topicHostname, IList<EventGridEvent> events)
		{
			return _eventGridClient.PublishEventsWithHttpMessagesAsync(topicHostname, events);
		}
	}
}
