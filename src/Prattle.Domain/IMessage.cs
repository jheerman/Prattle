using System;

namespace Prattle.Domain
{
	public class IMessage
	{
		int Id { get; set; }
		string Text { get; set; }
		int SMSGroupId { get; set; }
		DateTime SentDate { get; set; }
	}
}