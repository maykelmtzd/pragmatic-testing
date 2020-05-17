using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.DomainEvents;
using Core_pragmatic_testing.Factories;
using Core_pragmatic_testing.PasswordRules;

namespace Core_pragmatic_testing.Entities
{
	/// <summary>
	/// In a real production code base you would only have one implementation: PasswordHistory or PasswordHistoryUsingDomainEvents.
	/// </summary>
	public class PasswordHistoryUsingDomainEvents : PasswordHistory
	{
		public PasswordHistoryUsingDomainEvents(string userName,
			Password currentPassword,
			List<Password> previousPasswords,
			IPasswordRuleSet passwordRules) : base(userName, currentPassword, previousPasswords, passwordRules) { }

		public override bool CreateNewPassword(Password newPassword)
		{
			if (PasswordWasNotPreviouslyUsed(newPassword) && AllRulesComply(newPassword, PasswordRules.GetPasswordRules()))
			{
				PreviousPasswords.Add(CurrentPassword);
				CurrentPassword = newPassword;
				AddEvent(new NewPasswordCreatedDomainEvent(this));
				return true;
			}

			return false;
		}
	}
}
