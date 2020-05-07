namespace Application_pragmatic_testing.ExternalServices
{
	public interface ICredentialService
	{
		bool IsUserValid(string userName);
	}
}