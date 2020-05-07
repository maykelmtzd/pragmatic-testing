using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;

namespace Core_pragmatic_testing.PasswordRules
{
	public class PasswordComplexCharactersRule : IPasswordRule
	{
		public bool Comply(Password password)
		{
			if (DoNotContainAtLeastOneCapitalLetter(password.PasswordText))
				return false;

			if (DoNotContainAtLeastOneNumericCharacter(password.PasswordText))
				return false;

			if (DoNotContainAtLeastOneStrangedCharacter(password.PasswordText))
				return false;

			return true;
		}

		private bool DoNotContainAtLeastOneStrangedCharacter(string passwordText)
		{
			//StrangedCharacters: ! ~ & 
			return false;
		}

		private bool DoNotContainAtLeastOneNumericCharacter(string passwordText)
		{
			return false;
		}

		private bool DoNotContainAtLeastOneCapitalLetter(string passwordText)
		{
			return false;
		}
	}
}
