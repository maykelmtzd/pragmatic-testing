using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core_pragmatic_testing.DomainEvents;
using Infra_pragmatic_testing.ExternalEvents;
using Infra_pragmatic_testing.Services;
using MediatR;

namespace Application_pragmatic_testing.DomainEventHandlers
{
	public class NewPasswordCreatedDomainEventHandler : INotificationHandler<NewPasswordCreatedDomainEvent>
	{
		//IExternalEventPublisherServ is NOT valuable, it's an internal service, not at the end/edge of the hexagon. It only has one concrete implementation.
		private readonly IExternalEventPublisherServ _externalEventPublisherServ;

		public NewPasswordCreatedDomainEventHandler(IExternalEventPublisherServ externalEventPublisherServ)
		{
			_externalEventPublisherServ = externalEventPublisherServ;
		}

		public Task Handle(NewPasswordCreatedDomainEvent notification, CancellationToken cancellationToken)
		{
			var userName = notification.Aggregate.UserName;
			var newPassword = notification.Aggregate.CurrentPassword;
			var passwordChangedExternalEvent = PasswordChangedData.CreateExternalEvent(userName, newPassword);

			_externalEventPublisherServ.PublishAsync(passwordChangedExternalEvent);

			return Task.CompletedTask;
		}
	}
}
