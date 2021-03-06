using System;

using Prattle.Domain;

namespace Prattle.Android.Core
{
	public class SmsMessage : PrattleBase, ISmsMessage
	{
	    public string Text { get; set; }
		public int SmsGroupId { get; set; }
		public string ContactAddressBookId { get; set; }
		public string ContactName { get; set; }
		public DateTime SentDate { get; set; }
	}
}