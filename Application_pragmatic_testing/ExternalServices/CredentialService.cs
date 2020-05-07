using System;
using System.Collections.Generic;
using System.Text;
using Application_pragmatic_testing.Configurations;
using Microsoft.Extensions.Options;

namespace Application_pragmatic_testing.ExternalServices
{
	public class CredentialService : ICredentialService
	{
		private readonly ICredentialsManagerGateway _credentialsManagerGateway;
		private readonly IOptions<CredentialsManagerSettings> _credentialsManagerSettings;

		public CredentialService(ICredentialsManagerGateway credentialsManagerGateway, IOptions<CredentialsManagerSettings> credentialsManagerSettings)
		{
			_credentialsManagerGateway = credentialsManagerGateway;
			_credentialsManagerSettings = credentialsManagerSettings;
		}

		public bool IsPlatinumUser(string userName)
		{
			//TODO Add query string paramenter to URL to pass userName and some other constant parameter like Domain=Construction
			string response = _credentialsManagerGateway.IsPlatinumUser(_credentialsManagerSettings.Value.Url);
			return Boolean.Parse(response);
		}
	}
}
