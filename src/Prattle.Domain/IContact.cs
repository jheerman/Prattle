using System;

namespace Prattle.Domain
{
	public interface IContact
	{
		int Id { get; set; }
		string Name { get; set; }
		string MobilePhone { get; set; }
		string AddressBookId { get; set; }
		bool Selected { get; set; }
	}
}