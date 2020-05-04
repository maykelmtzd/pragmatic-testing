using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Api_pragmatic_testing.Controllers
{
	[ApiController]
	public class CredentialManagementController : ControllerBase
	{

		[HttpGet("api/users")]
		public JsonResult GetUsers() 
		{
			return new JsonResult(
					new List<object>()
					{
						new { Id = 1, Name = "Name1" },
						new { Id = 2, Name = "Name2" }
					}
				);
		}
	}
}
