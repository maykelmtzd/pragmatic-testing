using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application_pragmatic_testing.Dtos;
using Application_pragmatic_testing.ExternalServices;
using Application_pragmatic_testing.Responses;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Repositories;
using Infra_pragmatic_testing.Constants;
using Infra_pragmatic_testing.ExternalEvents;
using Infra_pragmatic_testing.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application_pragmatic_testing.Commands
{
	public class ChangePasswordCommand : IRequest<ChangePasswordResponse>
	{
		public ChangePasswordCommand(ChangePasswordDto changePasswordDto)
		{
			ChangePasswordDto = changePasswordDto;
		}

		public ChangePasswordDto ChangePasswordDto { get; }

		public class ChangePasswordHandler : RequestHandler<ChangePasswordCommand, ChangePasswordResponse>
		{
			private readonly ICredentialService _credentialService;
			private readonly IPasswordHistoryRepository _passwordHistoryRepo;
			private readonly ILogger<ChangePasswordHandler> _logger;
			private readonly ExternalEventPublisherServ _externalEventPublisherServ;

			public ChangePasswordHandler(ICredentialService credentialService, 
				IPasswordHistoryRepository passwordHistoryRepo,
				ILogger<ChangePasswordHandler> logger,
				ExternalEventPublisherServ externalEventPublisherServ)
			{
				_credentialService = credentialService;
				_passwordHistoryRepo = passwordHistoryRepo;
				_logger = logger;
				_externalEventPublisherServ = externalEventPublisherServ;
			}

			protected override ChangePasswordResponse Handle(ChangePasswordCommand command)
			{
				//Translate Dto  information into Domain objects format(Some form of validation takes place).
				var userName = command.ChangePasswordDto.UserName;
				var newPassword = new Password(command.ChangePasswordDto.NewPassword);

				//call external dependencies to retrieve information needed to execute business logic.
				var isHighProfileUser = _credentialService.IsHighProfileUser(userName);

				//Load domain object(usually aggregate) in memory
				var passwordHistory = _passwordHistoryRepo.GetPasswordHistory(userName);

				//Call operation on aggregate which mutates some state.
				var wasPasswordChanged = passwordHistory.CreateNewPassword(newPassword, isHighProfileUser);

				if (wasPasswordChanged)
				{
					//this could through an exception, see comments on UpdatePasswordHistory method.
					_passwordHistoryRepo.UpdatePasswordHistory(passwordHistory);

					//log the aggregate was saved
					_logger.LogInformation($"New password for user {userName} was saved");

					#region IMS comments
					//We don't need to:
					//1- The aggregate wraps itself in a domain event

					//2- The handler maps the aggregate in the domain event to an specific parameter (Dto?) for ExternalEventPublisherServ (EventGridService)

					//3- The ExternalEventPublisherServ maps the parameter (Dto?) into a more valuable Dto that specify the data contract with 
					//the outer world(EventGrid in this case)

					//4- The ExternalEventPublisherServ uses a third party library client (EventGridClient) to send the event out. No wrapper on the client
					// is used. Don't mock what you don't own: https://github.com/testdouble/contributing-tests/wiki/Don't-mock-what-you-don't-own

					//eventGridService.PublishPasswordChangedEvent(userName, newPassword); 
					#endregion

					var passwordChangedExternalEvent = PasswordChangedData.CreateExternalEvent(userName, newPassword);

					_externalEventPublisherServ.PublishAsync(passwordChangedExternalEvent);

					return new ChangePasswordResponse()
					{
						UserName = command.ChangePasswordDto.UserName,
						Success = true
					};
				}

				return new ChangePasswordResponse()
				{
					UserName = command.ChangePasswordDto.UserName,
					Success = false
				};
			}
		}
	}
}
