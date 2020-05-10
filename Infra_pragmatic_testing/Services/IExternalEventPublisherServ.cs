using Infra_pragmatic_testing.ExternalEvents;

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
    public interface IExternalEventPublisherServ
	{
		void PublishAsync(ExternalEvent externalEvent);
	}
}