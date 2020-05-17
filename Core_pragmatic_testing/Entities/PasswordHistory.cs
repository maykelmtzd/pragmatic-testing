using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Factories;
using Core_pragmatic_testing.PasswordRules;

namespace Core_pragmatic_testing.Entities
{
	/// <summary>
	/// This class is inheriting from DomainEventPublisher to facilitate the implementation of examples for both approaches:
	/// 1- Using domain events.
	/// 2- Not using domain events.
	/// 
	/// In real production code if we decide that domain events are not needed we would need to inherit from DomainEventPublisher.
	/// </summary>
	public class PasswordHistory : DomainEventPublisher
	{
		public string UserName { get; protected set; }

		//TODO Make the command return CUrrentPassword and PreviusPassword so the Controller can return it back to the
		//API caller
		public Password CurrentPassword { get; protected set; }
		public List<Password> PreviousPasswords { get; protected set; }
		protected IPasswordRuleSet PasswordRules { get;  private set; }

		public PasswordHistory(string userName,
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
		public virtual bool CreateNewPassword(Password newPassword)
		{
			if (PasswordWasNotPreviouslyUsed(newPassword) && AllRulesComply(newPassword, PasswordRules.GetPasswordRules()))
			{
				PreviousPasswords.Add(CurrentPassword);
				CurrentPassword = newPassword;
				return true;
			}

			return false;
		}

		protected bool PasswordWasNotPreviouslyUsed(Password newPassword)
		{
			return CurrentPassword != newPassword && !PreviousPasswords.Contains(newPassword);
		}

		protected bool AllRulesComply(Password newPassword, HashSet<IPasswordRule> passwordRules)
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
