using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using Infra_pragmatic_testing.Constants;

namespace Infra_pragmatic_testing.ExternalEvents
{
	public class PasswordChangedData
	{
		private const string dataVersion = "1.0";
		public string UserName { get; set; }
		public string NewPassword { get; set; }

		public static ExternalEvent CreateExternalEvent(string userName, Password newPassword)
		{
			return new ExternalEvent()
			{
				Data = new PasswordChangedData()
				{
					UserName = userName,
					NewPassword = newPassword.PasswordText
				},
				DataVersion = dataVersion,
				EventType = EventTypes.PasswordChanged
			};
		}
	}
}
