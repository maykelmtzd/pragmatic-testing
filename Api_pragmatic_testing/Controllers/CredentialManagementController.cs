using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application_pragmatic_testing.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api_pragmatic_testing.Controllers
{
	[ApiController]
	[Route("api")]
	public class CredentialManagementController : ControllerBase
	{

		[HttpGet("users")]
		public IActionResult GetUsers() 
		{
			return Ok(new List<object>() { 
					new { Id = 1, Name = "Name1" },
					new { Id = 2, Name = "Name2" }
				});
		}

		[HttpPost("credential/changePassword")]
		public IActionResult ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
		{
			return Ok(changePasswordDto);
		}
	}
}
