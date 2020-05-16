using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Factories;
using Core_pragmatic_testing.PasswordRules;

namespace Pragmatic_testing_tests.Core.Builders
{
	/// <summary>
	/// Is there a dotnet library that generates this code???
	/// </summary>
	public class PasswordHistoryBuilder
	{
		private string _userName = "userName";
		private Password _currentPassword = new Password("currentPassword");
		private List<Password> _previousPasswords = new List<Password>() 
			{
				new Password("previousPassword1"),
				new Password("previousPassword2"),
			};
		private IPasswordRuleSet _passwordRules = new RegularUserPasswordRules();

		public PasswordHistory Build()
		{
			return new PasswordHistory(_userName, _currentPassword, _previousPasswords, _passwordRules);
		}

		public PasswordHistoryBuilder withUserName(string userName)
		{
			_userName = userName;
			return this;
		}

		public PasswordHistoryBuilder withCurrentPassword(Password currentPassword)
		{
			_currentPassword = currentPassword;
			return this;
		}

		public PasswordHistoryBuilder withPreviousPasswords(List<Password> previousPasswords)
		{
			_previousPasswords = previousPasswords;
			return this;
		}

		public PasswordHistoryBuilder withPasswordRules(IPasswordRuleSet passwordRules)
		{
			_passwordRules = passwordRules;
			return this;
		}
	}
}
