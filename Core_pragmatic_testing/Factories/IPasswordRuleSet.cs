﻿using System.Collections.Generic;
using Core_pragmatic_testing.PasswordRules;

namespace Core_pragmatic_testing.Factories
{
	/// <summary>
	/// This interface is not valuable. We should remove it.
	/// </summary>
	public interface IPasswordRuleSet
	{
		HashSet<IPasswordRule> GetPasswordRules();
	}
}