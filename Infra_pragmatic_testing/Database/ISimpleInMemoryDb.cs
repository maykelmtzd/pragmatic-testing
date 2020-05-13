namespace Infra_pragmatic_testing.Database
{
	public interface ISimpleInMemoryDb
	{
		void AddPasswordHistoryDto(PasswordHistoryDto passwordHistory);
		void DeletePasswordHistoryDto(string userName);
		PasswordHistoryDto GetPasswordHistoryDto(string userName);
		void UpdatePasswordHistoryDto(PasswordHistoryDto passwordHistory);
	}
}