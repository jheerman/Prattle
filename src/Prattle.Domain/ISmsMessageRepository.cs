using System;
using System.Collections.Generic;

namespace Prattle.Domain
{
	public interface ISmsMessageRepository <T> where T : ISmsMessage
	{
		List<T> GetAllForGroup(int groupId);
		List<T> GetAllForEvent(int groupId, DateTime date, string text);
	}
}