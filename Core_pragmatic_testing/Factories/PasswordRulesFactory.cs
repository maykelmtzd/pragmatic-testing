using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.PasswordRules;

namespace Core_pragmatic_testing.Factories
{
	public class PasswordRulesFactory : IPasswordRulesFactory
	{
		public IReadOnlyList<IPasswordRule> CreatePasswordRules(bool isHighProfileUser)
		{
			if (isHighProfileUser)
				return new List<IPasswordRule> { new PasswordNotContainedInHistory(), new NewNonLetterCharacterAdded() };

			return new List<IPasswordRule> { new PasswordNotContainedInHistory() };
		}
	}
}
