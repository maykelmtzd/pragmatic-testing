using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application_pragmatic_testing.Commands;
using Application_pragmatic_testing.Dtos;
using Application_pragmatic_testing.ExternalServices;
using Application_pragmatic_testing.Responses;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Repositories;
using FluentAssertions;
using Infra_pragmatic_testing.Constants;
using Infra_pragmatic_testing.ExternalEvents;
using Infra_pragmatic_testing.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Pragmatic_testing_tests.Core.Builders;
using Xunit;

namespace Pragmatic_testing_tests.Application.TooMuchMocking
{
	public class ChangePasswordCommandTests
	{
		private readonly ChangePasswordCommand.ChangePasswordHandler _changePasswordHandler;
		private readonly Mock<IUserBehaviourService> _credentialService;
		private readonly Mock<IPasswordHistoryRepository> _passwordHistoryRepo;
		private readonly Mock<ILogger<ChangePasswordCommand.ChangePasswordHandler>> _logger;
		private readonly Mock<IExternalEventPublisherServ> _externalEventPublisher;

		public ChangePasswordCommandTests()
		{
			_credentialService = new Mock<IUserBehaviourService>();
			_passwordHistoryRepo = new Mock<IPasswordHistoryRepository>();
			_logger = new Mock<ILogger<ChangePasswordCommand.ChangePasswordHandler>>();
			_externalEventPublisher = new Mock<IExternalEventPublisherServ>();
			_changePasswordHandler = new ChangePasswordCommand.ChangePasswordHandler(_credentialService.Object, _passwordHistoryRepo.Object, _logger.Object,
				_externalEventPublisher.Object);
		}

		[Fact]
		public async Task When_change_password_handler_receive_a_valid_dto_and_it_complies_with_business_rules_it_returns_success_and_response_body_and_publishes()
		{
			var changePasswordDto = new ChangePasswordDto()
			{
				UserName = "Username1",
				CurrentPassword = "OldPassword",
				NewPassword = "newPassword",
				NewPasswordConfirmation = "newPassword"
			};

			var changePasswordCommand = new ChangePasswordCommand(changePasswordDto);

			_passwordHistoryRepo.Setup(mock => mock.GetPasswordHistory(changePasswordDto.UserName)).Returns
				(
					new PasswordHistoryBuilder().withUserName(changePasswordDto.UserName).Build() 
				);

			var changePasswordResponse = await _changePasswordHandler.Handle(changePasswordCommand, CancellationToken.None);

			var passwordChangedExternalEvent = PasswordChangedData.CreateExternalEvent
				(
					changePasswordDto.UserName, new Password(changePasswordDto.NewPassword)
				);

			//It's better to pass the specific paramenter and not use It.IsAny<ExternalEvent>()
			//_externalEventPublisher.Verify(mock => mock.PublishAsync(It.IsAny<ExternalEvent>()));
			_externalEventPublisher.Verify(publisher => publisher.PublishAsync(passwordChangedExternalEvent));

			changePasswordResponse.UserName.Should().Be(changePasswordDto.UserName);
			changePasswordResponse.Success.Should().BeTrue();
		}

		[Fact]
		public async Task When_change_password_handler_receive_a_valid_dto_and_it_does_not_comply_with_business_rules_it_returns_failure_and_response_body_and_does_not_publish()
		{
			//This requires better use of builders or fixtures, the property values for the Dto should be in one place
			//So they match in all tests.
			var changePasswordDto = new ChangePasswordDto()
			{
				UserName = "Username1",
				CurrentPassword = "currentPassword",
				NewPassword = "previousPassword1",
				NewPasswordConfirmation = "previousPassword1"
			};

			var changePasswordCommand = new ChangePasswordCommand(changePasswordDto);

			_passwordHistoryRepo.Setup(mock => mock.GetPasswordHistory(changePasswordDto.UserName)).Returns
				(
					new PasswordHistoryBuilder().withUserName(changePasswordDto.UserName).Build()
				);

			var changePasswordResponse = await _changePasswordHandler.Handle(changePasswordCommand, CancellationToken.None);

			_externalEventPublisher.Verify(publisher => publisher.PublishAsync(It.IsAny<ExternalEvent>()), Times.Never);

			changePasswordResponse.UserName.Should().Be(changePasswordDto.UserName);
			changePasswordResponse.Success.Should().BeFalse();
		}

	}
}
