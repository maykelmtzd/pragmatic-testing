using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using MediatR;

namespace Core_pragmatic_testing.DomainEvents
{
	public class NewPasswordCreatedDomainEvent : INotification
	{
		public PasswordHistoryUsingDomainEvents Aggregate { get; }

		public NewPasswordCreatedDomainEvent(PasswordHistoryUsingDomainEvents aggregate) => Aggregate = aggregate ?? throw new ArgumentNullException();
	}
}
