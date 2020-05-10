using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Rest.Azure;

namespace Infra_pragmatic_testing.Services
{
	/// <summary>
	/// IEventGridGateway is a valuable interface: 
	/// 
	/// 1- Even though we only have one concrete implementation for it, it's at the very edge of the microservice,
	/// so we might need to mock it.
	/// 
	/// 2- It defines exactly what our microservice/hexagon needs from the external API. The microservice is not exposed
	/// to every possible functionality the external API could have.
	/// 
	/// 3- It's a good marker for the microservice/hexagon integration point(port, in Ports and Adapters architecture terminology )
	/// This integration points are critical to the microservice success. They are the source of a lot of integration issues due to
	/// network connectivity, security, etc. It's important to have them clearly defined and isolated.
	/// </summary>
	public interface IEventGridGateway
	{
		Task<AzureOperationResponse> PublishEventsWithHttpMessagesAsync(string topicHostname, IList<EventGridEvent> events);
	}
}