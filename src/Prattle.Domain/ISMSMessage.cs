using System;

namespace Prattle.Domain
{
	public interface ISmsMessage
	{
		int Id { get; set; }
		string Text { get; set; }
		int SmsGroupId { get; set; }
		string ContactAddressBookId { get;set; }
		DateTime SentDate { get; set; }
	}
}