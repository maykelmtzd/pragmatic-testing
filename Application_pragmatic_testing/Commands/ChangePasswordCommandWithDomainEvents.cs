using System.Threading;
using System.Threading.Tasks;
using Application_pragmatic_testing.Dtos;
using Application_pragmatic_testing.ExternalServices;
using Application_pragmatic_testing.Responses;
using Core_pragmatic_testing.Entities;
using Core_pragmatic_testing.Repositories;
using Infra_pragmatic_testing.ExternalEvents;
using Infra_pragmatic_testing.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application_pragmatic_testing.Commands
{
	public class ChangePasswordCommandWithDomainEvents : IRequest<ChangePasswordResponse>
	{
		public ChangePasswordCommandWithDomainEvents(ChangePasswordDto changePasswordDto)
		{
			ChangePasswordDto = changePasswordDto;
		}

		public ChangePasswordDto ChangePasswordDto { get; }

		public class ChangePasswordWithDomainEventsHandler : IRequestHandler<ChangePasswordCommandWithDomainEvents, ChangePasswordResponse>
		{
			//IUserBehaviorService is NOT valuable, it's an internal service, not at the end/edge of the hexagon. It only has one concrete implementation.
			private readonly IUserBehaviorService _userBehaviourService;

			//IExternalEventPublisherServ is NOT valuable, it's an internal service, not at the end/edge of the hexagon. It only has one concrete implementation.
			private readonly IExternalEventPublisherServ _externalEventPublisherServ;

			private readonly IPasswordHistoryRepository _passwordHistoryRepo;
			private readonly ILogger<ChangePasswordWithDomainEventsHandler> _logger;


			public ChangePasswordWithDomainEventsHandler(IUserBehaviorService userBehaviourService,
				IPasswordHistoryRepository passwordHistoryRepo,
				ILogger<ChangePasswordWithDomainEventsHandler> logger,
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
			/// </summary>
			/// <param name="command"></param>
			/// <returns></returns>
			public async Task<ChangePasswordResponse> Handle(ChangePasswordCommandWithDomainEvents command, CancellationToken cancellationToken)
			{
				//Translate Dto  information into Domain objects format(Some form of validation takes place).
				var userName = command.ChangePasswordDto.UserName;
				var newPassword = new Password(command.ChangePasswordDto.NewPassword);

				//call external dependencies to retrieve information needed to execute business logic.
				var isHighProfileUser = _userBehaviourService.IsHighProfileUser(userName);

				//Load domain object(usually aggregate) in memory
				var passwordHistoryUsingDomainEvents = _passwordHistoryRepo.GetPasswordHistoryUsingDomainEvents(userName, isHighProfileUser);

				//Call operation on aggregate which mutates some state.
				//Use strategy pattern: Pass the isHighProfile to the repo so the passwordHistory is created
				//with the right object, HightProfileUserRulesFactory or RegularUserRulesFactory.
				// I could add another object that perform some algorithm and only has on concrete implementation. FindPlainEnglishWords()
				var passwordWasNotChanged = !passwordHistoryUsingDomainEvents.CreateNewPassword(newPassword);

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
				_passwordHistoryRepo.UpdatePasswordHistory(passwordHistoryUsingDomainEvents);

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

				//var passwordChangedExternalEvent = PasswordChangedData.CreateExternalEvent(userName, newPassword);

				//await _externalEventPublisherServ.PublishAsync(passwordChangedExternalEvent);

				return new ChangePasswordResponse()
				{
					UserName = command.ChangePasswordDto.UserName,
					Success = true
				};
			}
		}
	}
}
