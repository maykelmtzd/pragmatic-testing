using System;
using System.Collections.Generic;
using System.Text;

namespace Infra_pragmatic_testing.Database
{
	public class PasswordHistoryDto
	{
		public string UserName { get; set; }
		public string CurrentPassword { get; set; }
		public List<string> PreviousPasswords { get; set; }
    }
}
