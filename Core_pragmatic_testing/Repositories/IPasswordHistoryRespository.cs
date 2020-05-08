using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;

namespace Core_pragmatic_testing.Repositories
{
	public interface IPasswordHistoryRepository
	{
		PasswordHistory GetPasswordHistory(string userName);
	}
}
