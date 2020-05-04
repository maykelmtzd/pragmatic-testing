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
			protected override ChangePasswordResponse Handle(ChangePasswordCommand request)
			{
				return new ChangePasswordResponse()
				{
					UserName = request.ChangePasswordDto.UserName,
					Success = true
				};
			}
		}
	}
}
