using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Factories;
using FluentAssertions;
using Infra_pragmatic_testing.Database;
using Infra_pragmatic_testing.Repositories;
using Xunit;

namespace Pragmatic_testing_tests.Infra
{
	/// <summary>
	/// We are compromising by testing the internal method which don't belong to this class public API:
	/// 1- It this case it could make sense because the repo is so thin, it only orchestrates the call to the DB plus calling the mapping method.
	/// The mapping functionality is the important piece of logic that happens in memory(no external dependencies) that we would like to test.
	/// 
	/// 2- This is also saving us from the extra code needed to mock the repo dependencies and instanciate the repo itself to finally make the call (Act)
	/// We're basically enjoying the benefits of FUNCTIONAL TESTING.
	/// </summary>
	public class PasswordHistoryRespositoryTests
	{
		private const string CurrentPassword = "currentPassword1";
		private const string UserName = "UserName1";

		[Fact]
		public void Should_extract_relevant_information_from_password_history_domain_obj_to_be_saved_in_db()
		{
			var passwordHistoryDomainObj = CreatePasswordHistoryDomainObj();
			var passwordHistoryDto = PasswordHistoryRespository.ConvertToPasswordHistoryDto(passwordHistoryDomainObj);

			passwordHistoryDto.UserName.Should().Be(UserName);
			passwordHistoryDto.CurrentPassword.Should().Be((CurrentPassword, DateTime.MinValue));

			passwordHistoryDto.PreviousPasswords.Select(psw => new Password(psw.Item1, psw.Item2)).Should()
				.BeEquivalentTo(passwordHistoryDomainObj.PreviousPasswords);
		}

		/// <summary>
		/// It's interesting how in these tests that assert on the passwordHistoryDomainObj the properties like PasswordRules are exposed (get) only
		/// for the purpose of testing... In general we should try to avoid this; we could try to test it at a higher level where it's observable through a 
		/// valid/real public API. 
		/// </summary>
		[Fact]
		public void Should_assemble_domain_obj_containing_high_profile_user_password_rules_based_on_data_comming_from_db_and_external_user_behaviour_service()
		{
			var passwordHistoryDto = CreatePasswordHistoryDto();
			var passwordHistoryDomainObj = PasswordHistoryRespository.ConvertToPasswordHistoryDomainObj(passwordHistoryDto, isHighProfileUser: true);

			passwordHistoryDomainObj.UserName.Should().Be(UserName);
			passwordHistoryDomainObj.CurrentPassword.Should().Be(new Password(CurrentPassword, DateTime.MinValue));
			passwordHistoryDomainObj.PreviousPasswords.Select(psw => (psw.PasswordText, psw.CreatedAt)).Should().BeEquivalentTo(passwordHistoryDto.PreviousPasswords);

			//Notice how we don't make the BeOfType() assertion. It could be tempting to make it public in order to facilitate unit testing. We should not do
			//this, we should test through the public API so the tests are not too brittle by relying on implementation details.

			//passwordHistoryDomainObj.PasswordRules.Should().BeOfType<HighProfileUserPasswordRules>();

			const string doesNotAddNewNonLetterCharacter = "new11Password22";
			passwordHistoryDomainObj.CreateNewPassword(new Password(doesNotAddNewNonLetterCharacter, new DateTime(2020, 05, 05)))
				.Should().BeFalse();

			const string containsOneOfThePreviousPasswords = "password2!";
			passwordHistoryDomainObj.CreateNewPassword(new Password(containsOneOfThePreviousPasswords, new DateTime(2020, 05, 05)))
				.Should().BeFalse();

			const string validPassword = "password3!";
			passwordHistoryDomainObj.CreateNewPassword(new Password(validPassword, new DateTime(2020, 05, 05)))
				.Should().BeTrue();
		}

		[Fact]
		public void Should_assemble_domain_obj_containing_regular_user_password_rules_based_on_data_comming_from_db_and_external_user_behaviour_service()
		{
			var passwordHistoryDto = CreatePasswordHistoryDto();
			var passwordHistoryDomainObj = PasswordHistoryRespository.ConvertToPasswordHistoryDomainObj(passwordHistoryDto, isHighProfileUser: false);

			passwordHistoryDomainObj.UserName.Should().Be(UserName);
			passwordHistoryDomainObj.CurrentPassword.Should().Be(new Password(CurrentPassword, DateTime.MinValue));
			passwordHistoryDomainObj.PreviousPasswords.Select(psw => (psw.PasswordText, psw.CreatedAt)).Should().BeEquivalentTo(passwordHistoryDto.PreviousPasswords);

			//Notice how we don't make the BeOfType() assertion. It could be tempting to make it public in order to facilitate unit testing. We should not do
			//this, we should test through the public API so the tests are not too brittle by relying on implementation details.

			//passwordHistoryDomainObj.PasswordRules.Should().BeOfType<RegularUserPasswordRules>();

			const string containsOneOfThePreviousPasswords = "password2!";
			passwordHistoryDomainObj.CreateNewPassword(new Password(containsOneOfThePreviousPasswords, new DateTime(2020, 05, 05)))
				.Should().BeFalse();

			const string doesNotAddNewNonLetterCharacter = "new11Password22";
			passwordHistoryDomainObj.CreateNewPassword(new Password(doesNotAddNewNonLetterCharacter, new DateTime(2020, 05, 05)))
				.Should().BeTrue();
		}

		private PasswordHistory CreatePasswordHistoryDomainObj()
		{
			var previousPasswords = new List<Password>()
			{
				new Password("password1", new DateTime(2020, 04, 04)),
				new Password("password2", new DateTime(2020, 03, 03)),
			};

			var passwordRules = new HighProfileUserPasswordRules();

			return new PasswordHistory(UserName, new Password(CurrentPassword, DateTime.MinValue), previousPasswords, passwordRules);
		}

		private PasswordHistoryDto CreatePasswordHistoryDto()
		{
			return new PasswordHistoryDto()
			{
				UserName = UserName,
				CurrentPassword = (CurrentPassword, DateTime.MinValue),
				PreviousPasswords = new List<(string, DateTime)>()
				{
					("password1", new DateTime(2020, 04, 04)),
					("password2", new DateTime(2020, 03, 03))
				}
			};
		}
	}
}
