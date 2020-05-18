using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using MediatR;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.DependencyInjection;
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
		private readonly Mock<IEventGridGateway> _eventGridGateway;
		private readonly ISimpleInMemoryDb _simpleInMemoryDb;
		private readonly ServiceProvider _serviceProvider;

		public ChangePasswordCommandTests()
		{
			_eventGridGateway = new Mock<IEventGridGateway>();
			_serviceProvider = ConfigureDi(_eventGridGateway);
			_simpleInMemoryDb = _serviceProvider.GetRequiredService<ISimpleInMemoryDb>();
			_changePasswordHandler = _serviceProvider.GetRequiredService<ChangePasswordCommand.ChangePasswordHandler>();
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
			eventList[0].EventType = Infra_pragmatic_testing.Constants.EventTypes.PasswordChanged;

			return true;
		}

		private void AssertNewPasswordWasSavedInDB(ChangePasswordDto changePasswordCommandDto)
		{
			var simpleInMemoryDb = _serviceProvider.GetRequiredService<ISimpleInMemoryDb>();
			var savedPasswordHistoryDto = simpleInMemoryDb.GetPasswordHistoryDto(changePasswordCommandDto.UserName);


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

		/// <summary>
		/// This method could be place in its own class so it could be reused by several integration tests.
		/// </summary>
		/// <param name="eventGridGatewayMock"></param>
		/// <returns></returns>
		private ServiceProvider ConfigureDi(Mock<IEventGridGateway> eventGridGatewayMock)
		{
			var services = new ServiceCollection();

			services.AddMediatR(typeof(ChangePasswordCommandTests), typeof(ChangePasswordCommand));

			services.AddTransient<IExternalEventPublisherServ, ExternalEventPublisherServ>();
			services.AddTransient<ChangePasswordCommand.ChangePasswordHandler, ChangePasswordCommand.ChangePasswordHandler>();
			services.AddTransient<IUserBehaviorService, UserBehaviorService>();

			var userBehaviorGateway = new Mock<IUserBehaviorGateway>();
			userBehaviorGateway.Setup(mock => mock.IsPlatinumUser(It.IsAny<string>())).Returns("false");
			services.AddTransient(_ => userBehaviorGateway.Object);

			services.AddTransient<IPasswordHistoryRepository, PasswordHistoryRespository>();
			services.AddSingleton<ISimpleInMemoryDb>(SimpleInMemoryDb.InitializeDbWithDefaultSeedData());

			var policy = Policy
			   .Handle<Exception>()
			   .WaitAndRetryAsync(retryCount: 3,
								  retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
								  (_, __, retryCount, context) => { context["retryCount"] = retryCount; });
			services.AddSingleton(policy);
			
			services.AddTransient(_ => eventGridGatewayMock.Object);

			var eventGridClient = new EventGridClient(new TopicCredentials("SomeTopicKey"));
			services.AddSingleton<IEventGridClient>(eventGridClient);

			services.AddSingleton(Options.Create(new EventGridSettings()
			{
				InvoiceManagementTopicEndpoint = "http://someendpoint",
				InvoiceManagementTopicKey = "SomeTopicKey"
			}));

			services.AddLogging();

			return services.BuildServiceProvider();
		}
	}
}
