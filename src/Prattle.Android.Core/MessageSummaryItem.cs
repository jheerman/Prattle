using System;

namespace Prattle.Android.Core
{
	public class MessageSummaryItem
	{
		public MessageSummaryItem ()
		{ }
		
		public SmsGroup SmsGroup { get; set; }
		public string Message { get; set; }
		public int RecipientCount { get; set; }
		public DateTime DateSent { get; set; }
	}
}