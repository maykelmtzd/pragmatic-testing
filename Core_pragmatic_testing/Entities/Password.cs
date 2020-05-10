using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Common;

namespace Core_pragmatic_testing.Entities
{
	//Potential value object
	public class Password : ValueObject
	{
		public string PasswordText { get; }
		public Password(string password)
		{
			PasswordText = password;
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return PasswordText;
		}
	}
}
