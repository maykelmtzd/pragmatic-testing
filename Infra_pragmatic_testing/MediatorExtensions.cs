using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core_pragmatic_testing.Entities;
using MediatR;

namespace Infra_pragmatic_testing
{
	internal static class MediatorExtensions
	{
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DomainEventPublisher ctx)
        {
            var domainEvents = ctx.Events;
            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}
