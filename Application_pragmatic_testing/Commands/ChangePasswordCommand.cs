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

		public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
		{
			//IUserBehaviorService is NOT valuable, it's an internal service, not at the end/edge of the hexagon. It only has one concrete implementation.
			private readonly IUserBehaviorService _userBehaviourService;

			//IExternalEventPublisherServ is NOT valuable, it's an internal service, not at the end/edge of the hexagon. It only has one concrete implementation.
			private readonly IExternalEventPublisherServ _externalEventPublisherServ;

			private readonly IPasswordHistoryRepository _passwordHistoryRepo;
			private readonly ILogger<ChangePasswordHandler> _logger;
			

			public ChangePasswordHandler(IUserBehaviorService userBehaviourService, 
				IPasswordHistoryRepository passwordHistoryRepo,
				ILogger<ChangePasswordHandler> logger,
				IExternalEventPublisherServ externalEventPublisherServ)
			{
				_userBehaviourService = userBehaviourService;
				_passwordHistoryRepo = passwordHistoryRepo;
				_logger = logger;
				_externalEventPublisherServ = externalEventPublisherServ;
			}

			/// <summary>
			/// The Handler method was not defined as async just to remove some complexity which is not very relevant 
			/// for showing when unit tests are or are not valuable.
			/// 
			/// //Translate Dto  information into Domain objects format(Some form of validation takes place).
			/// 
			/// call external dependencies to retrieve information needed to execute business logic.
			/// 
			/// Load domain object(usually aggregate) in memory
			/// 
			/// //Call operation on aggregate which mutates some state.
			/// Use strategy pattern: Pass the isHighProfile to the repo so the passwordHistory is created
			/// with the right object, HightProfileUserRulesFactory or RegularUserRulesFactory.
			///  I could add another object that perform some algorithm and only has on concrete implementation. FindPlainEnglishWords()
			/// 
			/// </summary>
			/// <param name="command"></param>
			/// <returns></returns>
			public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
			{
				var userName = command.ChangePasswordDto.UserName;
				var newPassword = new Password(command.ChangePasswordDto.NewPassword);

				var isHighProfileUser = _userBehaviourService.IsHighProfileUser(userName);
				var passwordHistory = _passwordHistoryRepo.GetPasswordHistory(userName, isHighProfileUser);
				
				var passwordWasNotChanged = !passwordHistory.CreateNewPassword(newPassword);

				if (passwordWasNotChanged)
				{
					return new ChangePasswordResponse()
					{
						UserName = command.ChangePasswordDto.UserName,
						Success = false
					};
				}

				_logger.LogInformation($"Saving new password for user {userName}");
				//this could through an exception, see comments on UpdatePasswordHistory method.
				await _passwordHistoryRepo.UpdatePasswordHistoryAsync(passwordHistory);

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
				await _externalEventPublisherServ.PublishAsync(passwordChangedExternalEvent);

				return new ChangePasswordResponse()
				{
					UserName = command.ChangePasswordDto.UserName,
					Success = true
				};
			}
		}
	}
}
