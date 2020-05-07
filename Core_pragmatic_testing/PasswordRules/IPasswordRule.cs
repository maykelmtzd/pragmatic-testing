using Core_pragmatic_testing.Entities;

namespace Core_pragmatic_testing.PasswordRules
{
	public interface IPasswordRule
	{
		bool Comply(Password password);
	}
}