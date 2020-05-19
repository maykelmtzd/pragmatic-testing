
using System;
using System.Collections.Generic;
using System.Text;
using Infra_pragmatic_testing.ExternalEvents;
using Infra_pragmatic_testing.Services;
using Xunit;

namespace Pragmatic_testing_tests.Infra.Services
{
	/// <summary>
	/// Only test the mapping between our application ExternalEvent type and EventGridEvent type who's coming from
	/// and external library
	/// </summary>
	public class ExternalEventPublisherServTests
	{
		/// <summary>
		/// WIP
		/// </summary>
		/// TODO finish this test
		[Fact]
		public void Should_return_the_right_EventGridEvent_when_mapping()
		{
			var externalEvent = ExternalEvent.Create("", "", "", "");
			var eventGridEvent = ExternalEventPublisherServ.MapToEventGridEvent(externalEvent);

			//Assert on the right values for eventGridEvent
		}
	}
}
