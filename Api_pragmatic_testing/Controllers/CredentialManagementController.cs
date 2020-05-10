
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application_pragmatic_testing.Commands;
using Application_pragmatic_testing.Dtos;
using Application_pragmatic_testing.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api_pragmatic_testing.Controllers
{
	[ApiController]
	[Route("api")]
	public class CredentialManagementController : ControllerBase
	{
		private readonly IMediator _mediator;

		public CredentialManagementController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("users")]
		public IActionResult GetUsers() 
		{
			return Ok(new List<object>() { 
					new { Id = 1, Name = "Name1" },
					new { Id = 2, Name = "Name2" }
				});
		}

		[HttpPost("credential/changePassword")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
		{
			ChangePasswordResponse changePasswordResponse = await _mediator.Send(new ChangePasswordCommand(changePasswordDto));
			return Ok(changePasswordResponse);
		}
	}
}
