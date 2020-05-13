﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Factories;
using Core_pragmatic_testing.Repositories;
using Infra_pragmatic_testing.Database;

namespace Infra_pragmatic_testing.Repositories
{
	public class PasswordHistoryRespository : IPasswordHistoryRepository
	{
		private readonly ISimpleInMemoryDb _simpleInMemoryDbGateway;
		public PasswordHistoryRespository(ISimpleInMemoryDb simpleInMemoryDbGateway)
		{
			_simpleInMemoryDbGateway = simpleInMemoryDbGateway;
		}
		public PasswordHistory GetPasswordHistory(string userName)
		{
			var passwordHistoryDto = _simpleInMemoryDbGateway.GetPasswordHistoryDto(userName);
			return ConvertToPasswordHistoryDomainObj(passwordHistoryDto);
		}

		private PasswordHistory ConvertToPasswordHistoryDomainObj(PasswordHistoryDto passwordHistoryDto)
		{
			return new PasswordHistory
				(
					userName: passwordHistoryDto.UserName,
					currentPassword: new Password(passwordHistoryDto.CurrentPassword),
					previousPasswords: passwordHistoryDto.PreviousPasswords.Select(strPsw => new Password(strPsw)).ToList(),
					new PasswordRulesFactory()
				);
		}

		/// <summary>
		/// This methods bubles up any DB exceptions. It would be catch by a global handler at the application level.
		/// We probably want the application to log the exception, this would be tested as an integration test.
		/// We observe the result outside the hexagon.
		/// </summary>
		/// <param name="passwordHistory"></param>
		public void UpdatePasswordHistory(PasswordHistory passwordHistory)
		{
			//this could through a DB exception
			_simpleInMemoryDbGateway.UpdatePasswordHistoryDto
				(
					ConvertToPasswordHistoryDto(passwordHistory)
				);
		}

		private PasswordHistoryDto ConvertToPasswordHistoryDto(PasswordHistory passwordHistory)
		{
			return new PasswordHistoryDto()
			{
				UserName = passwordHistory.UserName,
				CurrentPassword = passwordHistory.CurrentPassword.PasswordText,
				PreviousPasswords = passwordHistory.PreviousPasswords.Select(psw => psw.PasswordText).ToList()
			};
		}
	}
}
