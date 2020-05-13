using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Factories;

namespace Infra_pragmatic_testing.Database
{
	public class SimpleInMemoryDb : ISimpleInMemoryDb
	{
		public void AddPasswordHistoryDto(PasswordHistoryDto passwordHistory)
		{
			if (!_passwordHistories.ContainsKey(passwordHistory.UserName))
			{
				_passwordHistories.Add(passwordHistory.UserName, passwordHistory);
				return;
			}

			throw new Exception("Db exception, Detailed Message: Key already exist");
		}

		public PasswordHistoryDto GetPasswordHistoryDto(string userName)
		{
			if (_passwordHistories.ContainsKey(userName))
				return _passwordHistories[userName];

			return null;
		}

		public void UpdatePasswordHistoryDto(PasswordHistoryDto passwordHistory)
		{
			if (_passwordHistories.ContainsKey(passwordHistory.UserName))
			{
				_passwordHistories[passwordHistory.UserName] = passwordHistory;
				return;
			}

			throw new Exception("Db exception, Detailed Message: PasswordHistory not found");
		}

		public void DeletePasswordHistoryDto(string userName)
		{
			if (_passwordHistories.ContainsKey(userName))
			{
				_passwordHistories.Remove(userName);
				return;
			}

			throw new Exception("Db exception, Detailed Message: PasswordHistory not found");
		}

		public static SimpleInMemoryDb InitializeDbWithDefaultSeedData()
		{
			return new SimpleInMemoryDb();
		}

		public SimpleInMemoryDb(Dictionary<string, PasswordHistoryDto> dbSeedData)
		{
			_passwordHistories = dbSeedData;
		}

		private Dictionary<string, PasswordHistoryDto> _passwordHistories;

		private SimpleInMemoryDb()
		{
			_passwordHistories = new Dictionary<string, PasswordHistoryDto>()
			{
				{
					"UserName1",
					new PasswordHistoryDto
					{
						UserName = "UserName1",
						CurrentPassword = "password3",
						PreviousPasswords = new List<string>() { "password1", "password2" }
					}
				},
				{
					"UserName2",
					new PasswordHistoryDto
					{
						UserName = "UserName2",
						CurrentPassword = "passwordC",
						PreviousPasswords = new List<string>() { "passwordA", "passwordB" }
					}
				},
				{
					"UserName3",
					new PasswordHistoryDto
					{
						UserName = "UserName3",
						CurrentPassword = "password3",
						PreviousPasswords = new List<string>() { "password1", "password2" }
					}
				}
			};
		}
	}
}
