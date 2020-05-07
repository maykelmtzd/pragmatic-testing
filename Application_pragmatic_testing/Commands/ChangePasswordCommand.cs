using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application_pragmatic_testing.Dtos;
using Application_pragmatic_testing.Responses;
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
			protected override ChangePasswordResponse Handle(ChangePasswordCommand command)
			{
				// Make some call to Credential service. Maybe to get some result that I could pass to the domain object, like isHighProfileUser.

				// User repo to load PasswordManagerObject who has a list of oldPassword and a list of rules for the newPassword to comply with.

				// Make call to passwordManagerObject passing isHighProfileUser to determine if password can be change.

				// If it can be changed, call Credential service ChangePasswordMethod

				// if passwordChanged call save in the DB

				// Logging, should I use logger in the command or use the logger in each service.

				// raise the event passwordChanged, or just call a service (eventPublisher) that publishes the event.
				// The EventPublisher would have the real EventGridClient third party class  wrapped in a Gateway class.

				return new ChangePasswordResponse()
				{
					UserName = command.ChangePasswordDto.UserName,
					Success = true
				};
			}
		}
	}
}
