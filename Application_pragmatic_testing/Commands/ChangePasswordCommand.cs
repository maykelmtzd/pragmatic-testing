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
using MediatR;

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

			public ChangePasswordHandler(ICredentialService credentialService, IPasswordHistoryRepository passwordHistoryRepo)
			{
				_credentialService = credentialService;
				_passwordHistoryRepo = passwordHistoryRepo;
			}

			protected override ChangePasswordResponse Handle(ChangePasswordCommand command)
			{
				// Make some call to Credential service. Maybe to get some result that I could pass to the domain object, like isHighProfileUser.

				// User repo to load PasswordManagerObject who has a list of oldPassword and a list of rules for the newPassword to comply with.

				// Make call to passwordHistoryObject passing isHighProfileUser to determine if password can be change.

				// If it can be changed, call Credential service ChangePasswordMethod

				// if passwordChanged call save in the DB

				// Logging, should I use logger in the command or use the logger in each service.

				// raise the event passwordChanged, or just call a service (eventPublisher) that publishes the event.
				// The EventPublisher would have the real EventGridClient third party class  wrapped in a Gateway class.



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

					//eventGridService.PublishPasswordChangedEvent(userName, newPassword);

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
