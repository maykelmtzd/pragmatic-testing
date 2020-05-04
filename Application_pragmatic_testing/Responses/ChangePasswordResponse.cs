using System;
using System.Collections.Generic;
using System.Text;

namespace Application_pragmatic_testing.Responses
{
	public class ChangePasswordResponse
	{
		public string UserName { get; set; }
		public bool Success { get; set; }

		public string ErrorMessage { get; set; }
	}
}
