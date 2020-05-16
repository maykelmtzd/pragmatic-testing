using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Factories;
using Core_pragmatic_testing.PasswordRules;
using FluentAssertions;
using Xunit;

namespace Pragmatic_testing_tests.Core.Entities
{
	/// <summary>
	/// State verification approach
	/// </summary>
	public class PasswordHistoryTests
	{
		private const string CurrentPassword = "currentPassword1";
		private const string UsedPassword = "password4";
		private List<Password> _previousPasswords;
		private IPasswordRuleSet _passwordRules;

		private PasswordHistory _passwordHistory;
		public PasswordHistoryTests()
		{
			_previousPasswords = _previousPasswords = new List<Password>()
			{
				new Password("password1", new DateTime(2020, 04, 04)),
				new Password(UsedPassword, new DateTime(2020, 01, 01)),
				new Password("password2", new DateTime(2020, 03, 03)),
				new Password("password3", new DateTime(2020, 02, 02))
			};

			_passwordRules = new HighProfileUserPasswordRules();

			_passwordHistory = new PasswordHistory("UserName1", new Password(CurrentPassword), _previousPasswords, _passwordRules);
		}

		[Fact]
		public void Should_not_create_new_password_if_password_is_corrently_in_use()
		{
			_passwordHistory.CreateNewPassword(new Password(CurrentPassword))
				.Should().BeFalse();
		}

		[Fact]
		public void Should_not_create_new_password_if_password_was_previously_used()
		{
			_passwordHistory.CreateNewPassword(new Password(UsedPassword))
				.Should().BeFalse();
		}

		[Fact]
		public void Should_not_create_new_password_if_it_does_not_comply_with_any_of_the_password_rules()
		{
			var itContainsUsedPassword = $"A{_previousPasswords[0].PasswordText}B!";
			_passwordHistory.CreateNewPassword(new Password(itContainsUsedPassword))
				.Should().BeFalse();

			var itDoesNotAddNewNonLetterCharacter = $"AB1cd22";
			_passwordHistory.CreateNewPassword(new Password(itDoesNotAddNewNonLetterCharacter))
				.Should().BeFalse();
		}

		[Fact]
		public void Should_create_new_password_if_it_was_not_used_and_it_comply_with_all_password_rules()
		{
			_passwordHistory.CreateNewPassword(new Password("newPassword!"))
				.Should().BeTrue();
		}
	}
}
