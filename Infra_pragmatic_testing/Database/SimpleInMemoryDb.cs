using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Factories;

namespace Infra_pragmatic_testing.Database
{
	public static class SimpleInMemoryDb
	{
		public static void AddPasswordHistory(PasswordHistory passwordHistory)
		{
			if (!_passwordHistories.ContainsKey(passwordHistory.UserName))
			{
				_passwordHistories.Add(passwordHistory.UserName, passwordHistory);
			}
				
			throw new Exception("Db exception, Detailed Message: Key already exist");
		}

		public static PasswordHistory GetPasswordHistory(string userName)
		{
			if (_passwordHistories.ContainsKey(userName))
				return _passwordHistories[userName];

			return null;
		}

		public static void UpdatePasswordHistory(PasswordHistory passwordHistory)
		{
			if (_passwordHistories.ContainsKey(passwordHistory.UserName))
			{
				_passwordHistories[passwordHistory.UserName] = passwordHistory;
			}

			throw new Exception("Db exception, Detailed Message: PasswordHistory not found");
		}

		public static void DeletePasswordHistory(string userName)
		{
			if (_passwordHistories.ContainsKey(userName))
			{
				_passwordHistories.Remove(userName);
			}

			throw new Exception("Db exception, Detailed Message: PasswordHistory not found");
		}

		private static Dictionary<string, PasswordHistory> _passwordHistories = new Dictionary<string, PasswordHistory>()
		{
			{
				"UserName1",
				new PasswordHistory(
				"UserName1",
				new Password("password3"),
				new List<Password>() { new Password("password1"), new Password("password2") },
				new PasswordRulesFactory()
				)
			},
			{
				"UserName2",
				new PasswordHistory(
				"UserName2",
				new Password("passwordC"),
				new List<Password>() { new Password("passwordA"), new Password("passwordB") },
				new PasswordRulesFactory()
				)
			},
			{
				"UserName3",
				new PasswordHistory(
				"UserName3",
				new Password("password3"),
				new List<Password>() { new Password("password1"), new Password("password2") },
				new PasswordRulesFactory()
				)
			}
		};

	}
}
