using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application_pragmatic_testing.Commands;
using Application_pragmatic_testing.Dtos;
using Application_pragmatic_testing.ExternalServices;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Factories;
using Core_pragmatic_testing.Repositories;
using FluentAssertions;
using Infra_pragmatic_testing.Configurations;
using Infra_pragmatic_testing.Constants;
using Infra_pragmatic_testing.Database;
using Infra_pragmatic_testing.ExternalEvents;
using Infra_pragmatic_testing.Repositories;
using Infra_pragmatic_testing.Services;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Polly;
using Polly.Retry;
using Pragmatic_testing_tests.Core.Builders;
using Xunit;

namespace Pragmatic_testing_tests.Application.IntegrationTests
{
	public class ChangePasswordCommandTests
	{
		private readonly ChangePasswordCommand.ChangePasswordHandler _changePasswordHandler;
		private readonly Mock<IUserBehaviorService> _credentialService;
		private readonly PasswordHistoryRespository _passwordHistoryRepo;
		private readonly Mock<ILogger<ChangePasswordCommand.ChangePasswordHandler>> _handlerLogger;
		private readonly Mock<ILogger<ExternalEventPublisherServ>> _eventPublisherServLogger;
		private readonly Mock<IEventGridGateway> _eventGridGateway;
		private readonly AsyncRetryPolicy _asyncRetryPolicy;
		private readonly ExternalEventPublisherServ _externalEventPublisherServ;
		private readonly SimpleInMemoryDb _simpleInMemoryDb;
		private readonly IOptions<EventGridSettings> _eventGridSettingOptions;
		public ChangePasswordCommandTests()
		{
			_credentialService = new Mock<IUserBehaviorService>();

			_simpleInMemoryDb = SimpleInMemoryDb.InitializeDbWithDefaultSeedData();
			_passwordHistoryRepo = new PasswordHistoryRespository(_simpleInMemoryDb);

			_handlerLogger = new Mock<ILogger<ChangePasswordCommand.ChangePasswordHandler>>();
			_eventPublisherServLogger = new Mock<ILogger<ExternalEventPublisherServ>>();
			_eventGridGateway = new Mock<IEventGridGateway>();

			_asyncRetryPolicy = Policy
				   .Handle<Exception>()
				   .WaitAndRetryAsync(1,
									  retryAttempt => TimeSpan.FromMilliseconds(1),
									  (_, __, retryCount, context) => { context["retryCount"] = retryCount; });

			_eventGridSettingOptions = Options.Create(new EventGridSettings()
			{
				InvoiceManagementTopicEndpoint = "http://someendpoint",
				InvoiceManagementTopicKey = "SomeTopicKey"
			});

			_externalEventPublisherServ = new ExternalEventPublisherServ
				(
					_eventGridGateway.Object,
					_eventGridSettingOptions,
					_asyncRetryPolicy,
					_eventPublisherServLogger.Object
				);

			_changePasswordHandler = new ChangePasswordCommand.ChangePasswordHandler(_credentialService.Object, _passwordHistoryRepo, _handlerLogger.Object,
				_externalEventPublisherServ);
		}

		[Fact]
		public async Task When_change_password_handler_receive_a_valid_dto_and_it_complies_with_business_rules_it_returns_success_and_response_body_and_save_to_DB_and_publishes()
		{
			var changePasswordDto = new ChangePasswordDto()
			{
				UserName = "UserName1",
				CurrentPassword = "password3",
				NewPassword = "brandNewPassword",
				NewPasswordConfirmation = "brandNewPassword"
			};

			var changePasswordCommand = new ChangePasswordCommand(changePasswordDto);
			var changePasswordResponse = await _changePasswordHandler.Handle(changePasswordCommand, CancellationToken.None);

			AssertNewPasswordWasSavedInDB(changePasswordDto);

			_eventGridGateway.Verify(gateway => gateway.PublishEventsWithHttpMessagesAsync
			(
				"someendpoint", It.Is<List<EventGridEvent>>(eventList => AssertOnEventList(eventList, changePasswordDto)))
			);

			changePasswordResponse.UserName.Should().Be(changePasswordDto.UserName);
			changePasswordResponse.Success.Should().BeTrue();
		}

		private bool AssertOnEventList(List<EventGridEvent> eventList, ChangePasswordDto changePasswordDto)
		{
			eventList.Should().HaveCount(1);

			var data = eventList[0].Data;
			var passwordChangeData = data.Should().BeOfType<PasswordChangedData>().Which;
			passwordChangeData.UserName.Should().Be(changePasswordDto.UserName);
			passwordChangeData.NewPassword.Should().Be(changePasswordDto.NewPassword);

			eventList[0].DataVersion.Should().Be("1.0");
			eventList[0].EventType = EventTypes.PasswordChanged;

			return true;
		}

		private void AssertNewPasswordWasSavedInDB(ChangePasswordDto changePasswordCommandDto)
		{
			var savedPasswordHistoryDto = _simpleInMemoryDb.GetPasswordHistoryDto(changePasswordCommandDto.UserName);


			savedPasswordHistoryDto.UserName.Should().Be(changePasswordCommandDto.UserName);
			savedPasswordHistoryDto.CurrentPassword.Item1.Should().Be(changePasswordCommandDto.NewPassword);
			savedPasswordHistoryDto.PreviousPasswords.Select(t => t.Item1).Should().BeEquivalentTo(new List<string> { "password1", "password2", "password3" });
		}

		/// <summary>
		/// This requires better use of builders or fixtures, the property values for the Dto should be in one place
		//  so they match in all tests.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task When_change_password_handler_receive_a_valid_dto_and_it_does_not_comply_with_business_rules_it_returns_failure_and_response_body_and_does_not_save_or_publish()
		{
			//Password is contained in history, it doesn't comply
			var newPassword = "password1";
			var changePasswordDto = new ChangePasswordDto()
			{
				UserName = "UserName1",
				CurrentPassword = "password3",
				NewPassword = newPassword,
				NewPasswordConfirmation = newPassword
			};

			var changePasswordCommand = new ChangePasswordCommand(changePasswordDto);
			var changePasswordResponse = await _changePasswordHandler.Handle(changePasswordCommand, CancellationToken.None);

			AssertPasswordHistoryDidNotChangeInDb(changePasswordDto.UserName);

			_eventGridGateway.Verify(gateway => gateway.PublishEventsWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<List<EventGridEvent>>()), Times.Never);

			changePasswordResponse.UserName.Should().Be(changePasswordDto.UserName);
			changePasswordResponse.Success.Should().BeFalse();
		}

		private void AssertPasswordHistoryDidNotChangeInDb(string userName)
		{
			var savedPasswordHistoryDto = _simpleInMemoryDb.GetPasswordHistoryDto(userName);


			savedPasswordHistoryDto.UserName.Should().Be(userName);
			savedPasswordHistoryDto.CurrentPassword.Item1.Should().Be("password3");
			savedPasswordHistoryDto.PreviousPasswords.Select(t => t.Item1).Should().BeEquivalentTo(new List<string> { "password1", "password2" });
		}
	}
}
