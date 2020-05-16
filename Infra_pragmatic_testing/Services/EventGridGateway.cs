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
	public class EventGridGateway : IEventGridGateway
	{
		private readonly IEventGridClient _eventGridClient;

		public EventGridGateway(IEventGridClient eventGridClient)
		{
			_eventGridClient = eventGridClient;
		}

		public AzureOperationResponse PublishEventsWithHttpMessagesAsync(string topicHostname, IList<EventGridEvent> events)
		{
			//Not making the actual call down below to simplify example.
			return new AzureOperationResponse();

			//return _eventGridClient.PublishEventsWithHttpMessagesAsync(topicHostname, events).Result;
		}
	}
}
