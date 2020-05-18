using System;
using System.Collections.Generic;
using System.Text;
using Application_pragmatic_testing.Configurations;
using Microsoft.Extensions.Options;

namespace Application_pragmatic_testing.ExternalServices
{
	public class UserBehaviorService : IUserBehaviorService
	{
		private readonly IUserBehaviorGateway _userBehaviorGateway;
		private readonly IOptions<CredentialsManagerSettings> _credentialsManagerSettings;

		public UserBehaviorService(IUserBehaviorGateway credentialsManagerGateway, IOptions<CredentialsManagerSettings> credentialsManagerSettings)
		{
			_userBehaviorGateway = credentialsManagerGateway;
			_credentialsManagerSettings = credentialsManagerSettings;
		}

		public bool IsHighProfileUser(string userName)
		{
			//TODO Add query string paramenter to URL to pass userName and some other constant parameter like Domain=Construction
			string response = _userBehaviorGateway.IsPlatinumUser(_credentialsManagerSettings.Value.Url);
			return Boolean.Parse(response);
		}
	}
}
