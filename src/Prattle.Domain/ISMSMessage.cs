using System;

namespace Prattle.Domain
{
	public interface ISmsMessage
	{
		int Id { get; set; }
		string Text { get; set; }
		int SMSGroupId { get; set; }
		string ContactAddressBookId { get;set; }
		DateTime SentDate { get; set; }
	}
}