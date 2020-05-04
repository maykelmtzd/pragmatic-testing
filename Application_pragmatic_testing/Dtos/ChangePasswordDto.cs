using System;
using System.Collections.Generic;
using System.Text;

namespace Application_pragmatic_testing.Dtos
{
	public class ChangePasswordDto
	{
		public string UserName { get; set; }
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
		public string NewPasswordConfirmation { get; set; }
	}
}
