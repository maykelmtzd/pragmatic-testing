using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core_pragmatic_testing.Entities;

namespace Core_pragmatic_testing.PasswordRules
{
	public class PasswordNotContainedInPrevious : IPasswordRule
	{
		public bool Comply(Password newPassword, IReadOnlyList<Password> relevantHistory)
		{
			var newPswText = newPassword.PasswordText;

			return relevantHistory.Select(psw => psw.PasswordText)
				.Where(historyPsw => historyPsw.Contains(newPswText)).Count() == 0;
		}
	}
}
