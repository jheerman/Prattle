using System;
using System.Linq;
using System.Collections.Generic;

using Prattle.Domain;

using SQLite;

namespace Prattle.Android.Core
{
	public class SmsMessageRepository : Repository<SmsMessage>, ISmsMessageRepository<SmsMessage>
	{
		public SmsMessageRepository ()
		{ }
		
		public List<SmsMessage> GetAllForGroup (int groupId)
		{
			return (from message in cn.Table<SmsMessage>()
			        where message.SmsGroupId == groupId
			        select message).ToList();
		}
		
		public List<SmsMessage> GetAllForEvent (int groupId, DateTime date, string text)
		{
			return (from message in cn.Table<SmsMessage>()
			        where message.SmsGroupId == groupId
			        where message.SentDate == date
			        where message.Text == text
			        select message).ToList ();
		}
	}
}