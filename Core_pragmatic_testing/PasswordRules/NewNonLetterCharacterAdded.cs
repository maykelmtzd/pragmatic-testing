using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core_pragmatic_testing.Entities;

namespace Core_pragmatic_testing.PasswordRules
{
	public class NewNonLetterCharacterAdded : IPasswordRule
	{
		public bool Comply(Password newPassword, IReadOnlyList<Password> relevantHistory)
		{
			var allPreviousNonLetterCharacters = relevantHistory.Aggregate
				(
					new StringBuilder(), (strBuilder, psw) => strBuilder.Append(ExtractNonLetterCharacters(psw.PasswordText))
				).ToString();

			return ExtractNonLetterCharacters(newPassword.PasswordText).ToCharArray()
				.Where(@char => NotContainedIn(@char, allPreviousNonLetterCharacters))
				.Count() > 0;
		}

		private bool NotContainedIn(char character, string allPreviousNonLetterCharacters)
		{
			return !allPreviousNonLetterCharacters.Contains(character);
		}

		private string ExtractNonLetterCharacters(string passwordText)
		{
			return Regex.Replace(passwordText, "[^a-zA-Z]", "");
		}
	}
}
