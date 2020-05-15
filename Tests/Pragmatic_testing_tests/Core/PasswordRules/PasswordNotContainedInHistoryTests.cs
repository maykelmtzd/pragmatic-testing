using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.PasswordRules;
using FluentAssertions;
using Xunit;

namespace Pragmatic_testing_tests.Core.PasswordRules
{
	/// <summary>
	/// Functional verification approach, we should favor this style as much as possible:
	/// 1- Everything the function/method needs is specified in the parameters.
	/// 2- The test just verify the function/method output.
	/// </summary>
	public class PasswordNotContainedInHistoryTests
	{
		private List<Password> _passwordHistory;

		private NewPasswordContainOthersInHistory _passwordNotContainedInHistory;

		/// <summary>
		/// History list is not created in order. SUT shouldn't rely on that.
		/// </summary>
		public PasswordNotContainedInHistoryTests()
		{
			_passwordHistory = new List<Password>()
			{
				new Password("password1", new DateTime(2020, 04, 04)),
				new Password("password2", new DateTime(2020, 03, 03)),
				new Password("password4", new DateTime(2020, 01, 01)),
				new Password("password3", new DateTime(2020, 02, 02))
			};

			_passwordNotContainedInHistory = new NewPasswordContainOthersInHistory();
		}

		[Fact]
		public void Should_not_comply_if_new_password_contains_any_of_last_three_passwords_from_history()
		{
			const string ContainPasswordRecentrlyUsed = "Apassword2B";
			_passwordNotContainedInHistory.Comply(new Password(ContainPasswordRecentrlyUsed, new DateTime(2020, 05, 05)), _passwordHistory)
				.Should().BeFalse();
		}

		[Fact]
		public void Should_comply_if_password_does_not_contain_any_of_last_three_passwords_from_history()
		{
			const string PasswordContainedIsTooOld = "Apassword4B";
			_passwordNotContainedInHistory.Comply(new Password(PasswordContainedIsTooOld, new DateTime(2020, 05, 05)), _passwordHistory)
				.Should().BeTrue();
		}

	}
}
