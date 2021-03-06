﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Factories;
using Core_pragmatic_testing.Repositories;
using Infra_pragmatic_testing.Database;
using MediatR;

[assembly: InternalsVisibleTo("Pragmatic_testing_tests")]
/// <summary>
/// We are compromising by testing the internal method which don't belong to this class public API:
/// 1- It this case it could make sense because the repo is so thin, it only orchestrates the call to the DB plus calling the mapping method.
/// The mapping functionality is the important piece of logic that happens in memory(no external dependencies) that we would like to test.
/// 
/// 2- This is also saving us from the extra code needed to mock the repo dependencies and instanciate the repo itself to finally make the call (Act)
/// We're basically enjoying the benefits of FUNCTIONAL TESTING.
/// </summary>
namespace Infra_pragmatic_testing.Repositories
{
	public class PasswordHistoryRespository : IPasswordHistoryRepository
	{
		private readonly ISimpleInMemoryDb _simpleInMemoryDb;
		private readonly IMediator _mediator;
		public PasswordHistoryRespository(ISimpleInMemoryDb simpleInMemoryDbGateway, IMediator mediator)
		{
			_simpleInMemoryDb = simpleInMemoryDbGateway;
			_mediator = mediator;
		}
		public PasswordHistory GetPasswordHistory(string userName, bool isHighProfileUser)
		{
			var passwordHistoryDto = _simpleInMemoryDb.GetPasswordHistoryDto(userName);
			return ConvertToPasswordHistoryDomainObj(passwordHistoryDto, isHighProfileUser);
		}

		public PasswordHistoryUsingDomainEvents GetPasswordHistoryUsingDomainEvents(string userName, bool isHighProfileUser)
		{
			var passwordHistoryDto = _simpleInMemoryDb.GetPasswordHistoryDto(userName);
			return ConvertToPasswordHistoryUsingDomainEvents(passwordHistoryDto, isHighProfileUser);
		}

		/// <summary>
		/// This methods bubles up any DB exceptions. It would be catch by a global handler at the application level.
		/// We probably want the application to log the exception, this would be tested as an integration test.
		/// We observe the result outside the hexagon.
		/// Notice that we don't need to have one integration test per each integration point (DB, EventGrid, Logging, etc)
		/// we want to test. A single integration test can cover several integration points.
		/// 
		/// Using the Result<> (CSharpFunctionalExtensions) return type could be a better approach to using "void/raise exception" in this methods. 
		/// It could be in general a better strategy for the entire application.
		/// For the sake of time, given that Result<> type is not neccessary to demonstrate the pragmatic approach to unit testing, 
		/// Result<> is not used.
		/// </summary>
		/// <param name="passwordHistory"></param>
		public async Task UpdatePasswordHistoryAsync(PasswordHistory passwordHistory)
		{
			await _mediator.DispatchDomainEventsAsync(passwordHistory);

			//this could through a DB exception
			_simpleInMemoryDb.UpdatePasswordHistoryDto
				(
					ConvertToPasswordHistoryDto(passwordHistory)
				);
		}

		static internal PasswordHistoryDto ConvertToPasswordHistoryDto(PasswordHistory passwordHistory)
		{
			return new PasswordHistoryDto()
			{
				UserName = passwordHistory.UserName,
				CurrentPassword = (passwordHistory.CurrentPassword.PasswordText, passwordHistory.CurrentPassword.CreatedAt),
				PreviousPasswords = passwordHistory.PreviousPasswords.Select(psw => (psw.PasswordText, psw.CreatedAt)).ToList()
			};
		}

		static internal PasswordHistory ConvertToPasswordHistoryDomainObj(PasswordHistoryDto passwordHistoryDto, bool isHighProfileUser)
		{
			return new PasswordHistory
				(
					userName: passwordHistoryDto.UserName,
					currentPassword: new Password(passwordHistoryDto.CurrentPassword.Item1, passwordHistoryDto.CurrentPassword.Item2),
					previousPasswords: passwordHistoryDto.PreviousPasswords.Select(psw => new Password(psw.Item1, psw.Item2)).ToList(),
					isHighProfileUser ? (IPasswordRuleSet)new HighProfileUserPasswordRules() : new RegularUserPasswordRules()
				);
		}

		/// <summary>
		/// This method is not tested. It was added for the sake of showing both approaches:
		/// 1- Using domain events.
		/// 2- Not using domain events.
		/// 
		/// In real production code depending on whether we choose to use domain events or not, we would have only one version of the
		/// ConvertToPasswordHistory() method.
		/// </summary>
		/// <param name="passwordHistoryDto"></param>
		/// <param name="isHighProfileUser"></param>
		/// <returns></returns>
		static internal PasswordHistoryUsingDomainEvents ConvertToPasswordHistoryUsingDomainEvents(PasswordHistoryDto passwordHistoryDto, bool isHighProfileUser)
		{
			return new PasswordHistoryUsingDomainEvents
				(
					userName: passwordHistoryDto.UserName,
					currentPassword: new Password(passwordHistoryDto.CurrentPassword.Item1, passwordHistoryDto.CurrentPassword.Item2),
					previousPasswords: passwordHistoryDto.PreviousPasswords.Select(psw => new Password(psw.Item1, psw.Item2)).ToList(),
					isHighProfileUser ? (IPasswordRuleSet)new HighProfileUserPasswordRules() : new RegularUserPasswordRules()
				);
		}
	}
}
