using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.PasswordRules;
using FluentAssertions;
using Xunit;

namespace Pragmatic_testing_tests.Core.PasswordRules
{
	public class NewNonLetterCharacterAddedTests
	{
		private List<Password> _passwordHistory;
		private NewNonLetterCharacterAdded _newNonLetterCharacterAdded;

		/// <summary>
		/// List is not order. PasswordRule class does not rely on history being ordered when 
		/// getting out of the DB or any other service.
		/// </summary>
		public NewNonLetterCharacterAddedTests()
		{
			_passwordHistory = new List<Password>()
			{
				new Password("password1-", new DateTime(2020, 04, 04)),
				new Password("password4!", new DateTime(2020, 01, 01)),
				new Password("password2/", new DateTime(2020, 03, 03)),
				new Password("password3~", new DateTime(2020, 02, 02))
			};

			_newNonLetterCharacterAdded = new NewNonLetterCharacterAdded();
		}

		[Fact]
		public void Should_comply_if_new_non_letter_character_was_added_to_new_password_considering_last_two_passwords()
		{
			const string NewNonLetterCharacterAdded= "newPassword!";
			_newNonLetterCharacterAdded.Comply(new Password(NewNonLetterCharacterAdded, new DateTime(2020, 05, 05)), _passwordHistory)
				.Should().BeTrue();
		}

		[Fact]
		public void Should_not_comply_if_new_non_letter_character_was_not_added_to_new_password_considering_last_two_passwords()
		{
			const string NewNonLetterCharacterAdded = "newPassword12/";
			_newNonLetterCharacterAdded.Comply(new Password(NewNonLetterCharacterAdded, new DateTime(2020, 05, 05)), _passwordHistory)
				.Should().BeFalse();
		}
	}
}
