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
		private const int RelevantHistory = 2;
		public bool Comply(Password newPassword, IReadOnlyList<Password> passwordHistory)
		{
			var allPreviousNonLetterCharacters = passwordHistory.OrderByDescending(psw => psw.CreatedAt)
				.Take(RelevantHistory)
				.Select(psw => psw.PasswordText)
				.Aggregate
				(
					new HashSet<char>(), (charSet, pswText) => 
					{
						var charList = new List<char>(charSet);
						charList.AddRange(ExtractNonLetterCharacters(pswText));
						return new HashSet<char>(charList);
					}
				);

			//TODO add extension method to char: char.NotContainedIn(list) instead of NotContainedIn(char, list)
			return ExtractNonLetterCharacters(newPassword.PasswordText)
				.Where(@char => NotContainedIn(@char, allPreviousNonLetterCharacters))
				.Count() > 0;
		}

		private bool NotContainedIn(char character, HashSet<char> allPreviousNonLetterCharacters)
		{
			return !allPreviousNonLetterCharacters.Contains(character);
		}

		private HashSet<char> ExtractNonLetterCharacters(string passwordText)
		{
			return new HashSet<char>(Regex.Replace(passwordText, "[a-zA-Z]", ""));
		}
	}
}
