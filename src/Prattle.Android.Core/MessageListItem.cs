using System;

namespace Prattle.Android.Core
{
	public class MessageListItem
	{
		public SmsGroup SmsGroup { get; set; }
		public string Text { get; set; }
		public int RecipientCount { get; set; }
		public DateTime DateSent { get; set; }
	}
}