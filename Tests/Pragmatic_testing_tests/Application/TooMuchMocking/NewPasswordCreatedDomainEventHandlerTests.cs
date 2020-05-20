using System;
using System.Collections.Generic;
using System.Text;

namespace Pragmatic_testing_tests.Application.TooMuchMocking
{
	/// <summary>
	/// We won't have unit tests for NewPasswordCreatedDomainEventHandler. If we did we would be mostly testing domain 
	/// implementation details through collaboration verification:
	/// Same idea as Pragmatic_testing_tests.Application.TooMuchMocking.ChangePasswordCommandTests. 
	/// This is exactly what we want to avoid. NewPasswordCreatedDomainEventHandler will be tested through 
	/// integration/component tests. Besides, the piece of mapping logic that this class contains will be tested in
	/// isolation using a functional verification approach through PasswordChangedData.CreateExternalEvent()
	/// </summary>
	public class NewPasswordCreatedDomainEventHandlerTests
	{
	}
}
