using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Factories;
using Core_pragmatic_testing.PasswordRules;

namespace Core_pragmatic_testing.Entities
{
	public class PasswordHistory
	{
		private readonly IPasswordRulesFactory _passwordRulesFactory;

		public PasswordHistory(string userName,
			Password currentPassword,
			List<Password> previousPasswords,
			IPasswordRulesFactory passwordRulesFactory)
		{
			UserName = userName;
			CurrentPassword = currentPassword;
			PreviousPasswords = previousPasswords;
			_passwordRulesFactory = passwordRulesFactory;
		}

		public string UserName { get; private set; }

		public Password CurrentPassword { get; private set; }

		public List<Password> PreviousPasswords { get; private set; }


		/// <summary>
		/// We could change this to apply the rules when creating the password: Password.Create(newPassword) 
		/// There are rules that depend on previous  passwords creation time(createdAt field on Password obj) so not all could be apply at Password.Create(newPassword)
		/// We could validate that the current password in Dto is equal to the current one coming from the DB
		/// </summary>
		/// <param name="newPassword"></param>
		/// <param name="isHighProfileUser"></param>
		/// <returns></returns>
		public bool CreateNewPassword(Password newPassword, bool isHighProfileUser)
		{
			var passwordRules = _passwordRulesFactory.CreatePasswordRules(isHighProfileUser);

			if (PasswordWasNotPreviouslyUsed(newPassword) && AllRulesComply(newPassword, passwordRules))
			{
				PreviousPasswords.Add(CurrentPassword);
				CurrentPassword = newPassword;
				return true;
			}

			return false;
		}

		private bool PasswordWasNotPreviouslyUsed(Password newPassword)
		{
			return CurrentPassword != newPassword && !PreviousPasswords.Contains(newPassword);
		}

		private bool AllRulesComply(Password newPassword, IReadOnlyList<IPasswordRule> passwordRules)
		{
			foreach (var passwordRule in passwordRules)
			{
				if (!passwordRule.Comply(newPassword))
				{
					return false;
				}
			}

			return true;
		}
	}
}
