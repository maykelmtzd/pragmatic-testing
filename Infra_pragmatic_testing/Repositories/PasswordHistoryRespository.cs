using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Repositories;
using Infra_pragmatic_testing.Database;

namespace Infra_pragmatic_testing.Repositories
{
	public class PasswordHistoryRespository : IPasswordHistoryRepository
	{
		public PasswordHistory GetPasswordHistory(string userName)
		{
			return SimpleInMemoryDb.GetPasswordHistory(userName);
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
			SimpleInMemoryDb.UpdatePasswordHistory(passwordHistory);
		}
	}
}
