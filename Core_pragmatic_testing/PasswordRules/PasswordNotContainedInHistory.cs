using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core_pragmatic_testing.Entities;

namespace Core_pragmatic_testing.PasswordRules
{
	public class NewPasswordContainOthersInHistory : IPasswordRule
	{
		private const int RelevantHistory = 3;
		public bool Comply(Password newPassword, IReadOnlyList<Password> relevantHistory)
		{
			var newPswText = newPassword.PasswordText;

			return relevantHistory.OrderByDescending(psw => psw.CreatedAt)
				.Take(RelevantHistory)
				.Select(psw => psw.PasswordText)
				.Where(historyPsw => newPswText.Contains(historyPsw)).Count() == 0;
		}
	}
}
