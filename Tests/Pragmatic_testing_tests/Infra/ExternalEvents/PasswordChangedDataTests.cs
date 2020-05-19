using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Entities;
using Infra_pragmatic_testing.ExternalEvents;
using Xunit;

namespace Pragmatic_testing_tests.Infra.ExternalEvents
{
	public class PasswordChangedDataTests
	{
		/// <summary>
		/// Use functional style to test mapping from (string userName, Password newPassword) into an ExternalEvent
		/// WIP: Asserts need to be added
		/// </summary>
		/// TODO finish this test
		[Fact]
		public void Should_return_proper_output_when_mapping()
		{
			var externalEvent = PasswordChangedData.CreateExternalEvent("someName", new Password("somePassword"));
			//Assert on the right values
		}
	}
}
