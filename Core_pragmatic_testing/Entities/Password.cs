using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Common;

namespace Core_pragmatic_testing.Entities
{
	public class Password : ValueObject
	{
		public string PasswordText { get; }
		public DateTime CreatedAt { get; }
		public Password(string password, DateTime createdAt)
		{
			PasswordText = password;
			CreatedAt = createdAt;
		}

		public Password(string password)
		{
			PasswordText = password;
			CreatedAt = DateTime.MinValue;
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return PasswordText;
		}
	}
}
