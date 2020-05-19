using System;
using System.Collections.Generic;
using System.Text;

namespace Pragmatic_testing_tests.Core.Factories
{
	/// <summary>
	/// The code in HighProfileUserPasswordRules class is pretty trivial. Let's not write tests for it. 
	/// 
	/// The code in HighProfileUserPasswordRules is tested at a higher level through PasswordHistory.
	/// It is tested in:
	/// 1- PasswordHistoryTests.Should_not_create_new_password_if_it_does_not_comply_with_any_of_the_password_rules()
	/// 2- PasswordHistoryTests.Should_create_new_password_if_it_was_not_used_and_it_comply_with_all_password_rules()
	/// </summary>
	public class HighProfileUserPasswordRulesTests
	{
	}
}
