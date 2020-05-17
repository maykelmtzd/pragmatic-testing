using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using MediatR;

namespace Core_pragmatic_testing.DomainEvents
{
	public class NewPasswordCreatedEvent : INotification
	{
		public PasswordHistoryWithDomainEvents Aggregate { get; }

		public NewPasswordCreatedEvent(PasswordHistoryWithDomainEvents aggregate) => Aggregate = aggregate ?? throw new ArgumentNullException();
	}
}
