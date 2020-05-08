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
	}
}
