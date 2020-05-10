using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api_pragmatic_testing.Controllers;
using Application_pragmatic_testing.Commands;
using Application_pragmatic_testing.Dtos;
using Application_pragmatic_testing.Responses;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Pragmatic_testing_tests.API.TooMuchMocking
{
	/// <summary>
	/// Code in ChangePassword action is trivial. We don't need to test at the unit level. 
	/// The integration tests will cover this code.
	/// </summary>
	public class CredentialManagementControllerTests
	{
		private readonly Mock<IMediator> _mediator;
		private readonly CredentialManagementController _credentialManagementController;

		public CredentialManagementControllerTests()
		{
			_mediator = new Mock<IMediator>();
			_credentialManagementController = new CredentialManagementController(_mediator.Object);
		}

		/// <summary>
		/// This test is too complicated for the value it provided. The code tested in the Controller is trivial
		/// and it should be covered by integration tests.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task Given_change_password_endpoint_receive_valid_payload_it_should_return_200_status_code_with_user_name_in_response_body()
		{
			var changePasswordDto = new ChangePasswordDto()
			{
				CurrentPassword = "CurrentPassword",
				NewPassword = "NewPassword",
				NewPasswordConfirmation = "NewPassword",
				UserName = "UserName"
			};

			var changePasswordResponse = new ChangePasswordResponse()
			{
				UserName = changePasswordDto.UserName,
				Success = true
			};

			_mediator.Setup(mock => mock.Send(It.Is<ChangePasswordCommand>(command => command.ChangePasswordDto.Equals(changePasswordDto)),
				It.IsAny<CancellationToken>())).ReturnsAsync(changePasswordResponse);

			var actionResult = await _credentialManagementController.ChangePassword(changePasswordDto);

			var okObjectResultWhich = actionResult.Should().BeOfType<OkObjectResult>().Which;
			okObjectResultWhich.StatusCode.Should().Be(StatusCodes.Status200OK);
			okObjectResultWhich.Value.Should().BeOfType(typeof(ChangePasswordResponse));

			var response = ((OkObjectResult)actionResult).Value as ChangePasswordResponse;

			response.UserName.Should().Be(changePasswordDto.UserName);
			response.Success.Should().BeTrue();
		}
	}
}
