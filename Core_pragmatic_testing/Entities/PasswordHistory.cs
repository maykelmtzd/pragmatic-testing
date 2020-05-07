using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Factories;
using Core_pragmatic_testing.PasswordRules;

namespace Core_pragmatic_testing.Entities
{
	public class PasswordHistory
	{
		private Password _currentPassword;
		private readonly List<Password> _previousPasswords;
		private readonly IPasswordRulesFactory _passwordRulesFactory;

		public PasswordHistory(string userName,
			Password currentPassword,
			List<Password> previousPasswords,
			IPasswordRulesFactory passwordRulesFactory)
		{
			UserName = userName;
			_currentPassword = currentPassword;
			_previousPasswords = previousPasswords;
			_passwordRulesFactory = passwordRulesFactory;
		}

		public string UserName { get; private set; }

		//We could change this to apply the rules when creating the password: Password.Create(newPassword)
		// There are rules that depend on passwordCreationTime so not all could be apply at Password.Create(newPassword)
		public bool CreateNewPassword(Password newPassword, bool isHighProfileUser)
		{
			var passwordRules = _passwordRulesFactory.CreatePasswordRules(isHighProfileUser);

			if (PasswordWasNotPreviouslyUsed(newPassword) && AllRulesComply(newPassword, passwordRules))
			{
				_previousPasswords.Add(_currentPassword);
				_currentPassword = newPassword;
				return true;
			}

			return false;
		}

		private bool PasswordWasNotPreviouslyUsed(Password newPassword)
		{
			return !_previousPasswords.Contains(newPassword);
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
