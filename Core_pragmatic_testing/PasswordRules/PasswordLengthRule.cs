using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;

namespace Core_pragmatic_testing.PasswordRules
{
	public class PasswordLengthRule : IPasswordRule
	{
		private const int MaximumPasswordLength = 25;
		private const int MinimumPasswordLength = 5;

		public bool Comply(Password password)
		{
			return password.PasswordText.Length > MinimumPasswordLength && password.PasswordText.Length < MaximumPasswordLength;
		}
	}
}
