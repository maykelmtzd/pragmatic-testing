using System;
using System.Collections.Generic;
using System.Text;
using Core_pragmatic_testing.Common;

namespace Infra_pragmatic_testing.ExternalEvents
{
	public class ExternalEvent : ValueObject
	{
		public string EventType { get; }
		public object Data { get; }
		public string Subject { get; }
		public string DataVersion { get; }

		public static ExternalEvent Create(string eventType, object data, string subject, string dataVersion)
		{
			return new ExternalEvent(eventType, data, subject, dataVersion);
		}

		private ExternalEvent(string eventType, object data, string subject, string dataVersion)
		{
			EventType = eventType;
			Data = data;
			Subject = subject;
			DataVersion = dataVersion;
		}
		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return EventType;
			yield return Data;
			yield return Subject;
			yield return DataVersion;
		}
	}
}
