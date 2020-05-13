using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Common;
using Core_pragmatic_testing.Entities;
using Infra_pragmatic_testing.Constants;

namespace Infra_pragmatic_testing.ExternalEvents
{
	public class PasswordChangedData : ValueObject
	{
		private const string dataVersion = "1.0";
		public string UserName { get; }
		public string NewPassword { get; }

		public static ExternalEvent CreateExternalEvent(string userName, Password newPassword)
		{
			return ExternalEvent.Create
			(
				EventTypes.PasswordChanged,
				new PasswordChangedData(userName, newPassword.PasswordText),
				subject: null,
				dataVersion 
			);
		}

		private PasswordChangedData(string userName, string newPassword)
		{
			UserName = userName;
			NewPassword = newPassword;
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return UserName;
			yield return NewPassword;
		}
	}
}
