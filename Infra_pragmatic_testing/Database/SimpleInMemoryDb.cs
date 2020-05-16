using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Factories;

namespace Infra_pragmatic_testing.Database
{
	/// <summary>
	/// Using the Result<> (CSharpFunctionalExtensions) return type could be a better approach to using "void/raise exception" in this methods. It
	/// could be in general a better strategy for the entire application.
	/// For the sake of time, given that Result<> type is not neccessary to demonstrate the pragmatic approach to unit testing, 
	/// Result<> is not used.
	/// </summary>
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
						CurrentPassword = ("password3", new DateTime(2020, 3, 3)),
						PreviousPasswords = new List<(string, DateTime)>()
						{
							("password1", new DateTime(2019, 10, 10)),
							("password2", new DateTime(2019, 12, 12))
						}
					}
				},
				{
					"UserName2",
					new PasswordHistoryDto
					{
						UserName = "UserName2",
						CurrentPassword = ("passwordC", new DateTime(2020, 3, 3)),
						PreviousPasswords = new List<(string, DateTime)>() { ("passwordA", new DateTime(2019, 10, 10)), ("passwordB", new DateTime(2019, 12, 12)) }
					}
				},
				{
					"UserName3",
					new PasswordHistoryDto
					{
						UserName = "UserName3",
						CurrentPassword = ("password3", new DateTime(2020, 3, 3)),
						PreviousPasswords = new List<(string, DateTime)>()
						{
							("password1", new DateTime(2019, 10, 10)),
							("password2", new DateTime(2019, 12, 12))
						}
					}
				}
			};
		}
	}
}
