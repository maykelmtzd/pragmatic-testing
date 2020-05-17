using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.DomainEvents;
using Core_pragmatic_testing.Factories;
using Core_pragmatic_testing.PasswordRules;

namespace Core_pragmatic_testing.Entities
{
	/// <summary>
	/// The code from PasswordHistory class is repeated in this class. This is just for the sake of showing both approaches.
	/// In a real production code base you would only have one implementation: PasswordHistory or PasswordHistoryWithDomainEvents.
	/// </summary>
	public class PasswordHistoryWithDomainEvents : DomainEventPublisher
	{
		public string UserName { get; private set; }

		//TODO Make the command return CUrrentPassword and PreviusPassword so the Controller can return it back to the
		//API caller
		public Password CurrentPassword { get; private set; }
		public List<Password> PreviousPasswords { get; private set; }
		private IPasswordRuleSet PasswordRules { get; }

		public PasswordHistoryWithDomainEvents(string userName,
			Password currentPassword,
			List<Password> previousPasswords,
			IPasswordRuleSet passwordRules)
		{
			UserName = userName;
			CurrentPassword = currentPassword;
			PreviousPasswords = previousPasswords;
			PasswordRules = passwordRules;
		}

		/// <summary>
		/// We could change this to apply the rules when creating the password: Password.Create(newPassword) 
		/// There are rules that depend on previous  passwords creation time(createdAt field on Password obj) so not all could be apply at Password.Create(newPassword)
		/// We could validate that the current password in Dto is equal to the current one coming from the DB
		/// </summary>
		/// <param name="newPassword"></param>
		/// <param name="isHighProfileUser"></param>
		/// <returns></returns>
		public bool CreateNewPassword(Password newPassword)
		{
			if (PasswordWasNotPreviouslyUsed(newPassword) && AllRulesComply(newPassword, PasswordRules.GetPasswordRules()))
			{
				PreviousPasswords.Add(CurrentPassword);
				CurrentPassword = newPassword;
				AddEvent(new NewPasswordCreatedEvent(this));
				return true;
			}

			return false;
		}

		private bool PasswordWasNotPreviouslyUsed(Password newPassword)
		{
			return CurrentPassword != newPassword && !PreviousPasswords.Contains(newPassword);
		}

		private bool AllRulesComply(Password newPassword, HashSet<IPasswordRule> passwordRules)
		{
			var pswHistory = new List<Password>(PreviousPasswords);
			pswHistory.Add(CurrentPassword);

			foreach (var passwordRule in passwordRules)
			{
				if (!passwordRule.Comply(newPassword, pswHistory))
				{
					return false;
				}
			}

			return true;
		}
	}
}
