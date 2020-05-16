using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.PasswordRules;

namespace Core_pragmatic_testing.Factories
{
	public class RegularUserPasswordRules : IPasswordRuleSet
	{
		public HashSet<IPasswordRule> GetPasswordRules()
		{
			return new HashSet<IPasswordRule> { new NewPasswordContainOthersInHistory() };
		}
	}
}
