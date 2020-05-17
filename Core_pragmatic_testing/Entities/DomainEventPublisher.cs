using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Core_pragmatic_testing.Entities
{
	public abstract class DomainEventPublisher
	{
		private readonly ISet<INotification> _events = new HashSet<INotification>();

		public IEnumerable<INotification> Events => _events;

		protected void AddEvent(INotification @event)
		{
			_events.Add(@event);
		}

		public void ClearEvents() => _events.Clear();
	}
}
