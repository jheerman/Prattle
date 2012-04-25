using System;

namespace Prattle.Android.Core
{
	public class Message
	{
		public Message ()
		{ }
		
		public int Id { get; set; }
		public string Text { get; set; }
		public int SMSGroupId { get; set; }
		public DateTime SentDate { get; set; }
	}
}