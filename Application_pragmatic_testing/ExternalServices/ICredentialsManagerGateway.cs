namespace Application_pragmatic_testing.ExternalServices
{
	public interface ICredentialsManagerGateway
	{
		string IsUserValid(string url);
	}
}