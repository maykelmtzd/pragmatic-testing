﻿using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.PasswordRules;

namespace Core_pragmatic_testing.Factories
{
	/// <summary>
	/// The code in RegularUserPasswordRules is pretty trivial. Let's not write tests for it. 
	/// 
	/// The code in RegularUserPasswordRules is tested at a higher level through PasswordHistory.
	/// It is tested in:
	/// 1- PasswordHistoryTests.Should_ignore_non_letter_characters_rule_if_it_is_a_regular_user()
	/// 2- PasswordHistoryTests.Should_not_ignore_new_pasword_contained_in_history_if_it_is_a_regular_user()
	/// </summary>
	public class RegularUserPasswordRules : IPasswordRuleSet
	{
		public HashSet<IPasswordRule> GetPasswordRules()
		{
			return new HashSet<IPasswordRule> { new NewPasswordContainOthersInHistory() };
		}
	}
}
