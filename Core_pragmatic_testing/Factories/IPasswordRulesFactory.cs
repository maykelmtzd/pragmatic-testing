using System.Collections.Generic;
using Core_pragmatic_testing.PasswordRules;

namespace Core_pragmatic_testing.Factories
{
	public interface IPasswordRulesFactory
	{
		IReadOnlyList<IPasswordRule> CreatePasswordRules(bool isHighProfileUser);
	}
}