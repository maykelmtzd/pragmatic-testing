using System;
using System.Collections.Generic;
using System.Text;

namespace Infra_pragmatic_testing.ExternalEvents
{
	public class ExternalEvent
	{
		public string EventType { get; set; }
		public object Data { get; set; }
		public string Subject { get; set; }
		public string DataVersion { get; set; }
	}
}
