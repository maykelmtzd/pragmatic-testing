using System;
using System.Collections.Generic;
using System.Text;

namespace Core_pragmatic_testing.Entities
{
	//Potential value object
	public class Password
	{
		public string PasswordText { get; private set; }
		public Password(string password)
		{
			PasswordText = password;
		}
	}
}
