using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Application_pragmatic_testing.ExternalServices
{
	// This is a passthrou/wrapper over the external library(code not controlled by the team)
	// that is actually making the call out of process(network call).
	public class CredentialsManagerGateway : ICredentialsManagerGateway
	{
		private HttpClient _httpClient;
		public CredentialsManagerGateway(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		public string IsPlatinumUser(string url)
		{
			return _httpClient.GetStringAsync(url).Result;
		}
	}
}
