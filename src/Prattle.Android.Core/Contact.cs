using System;

using Prattle.Domain;

namespace Prattle.Android.Core
{
	public class Contact : PrattleBase, IContact
	{
		public Contact ()
		{ }
		public string MobilePhone { get; set; }
		public string AddressBookId { get; set; }
		public bool Selected { get; set; }
	}
}