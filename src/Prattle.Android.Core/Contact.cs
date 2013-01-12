using Prattle.Domain;

namespace Prattle.Android.Core
{
	public class Contact : PrattleBase, IContact
	{
	    public string Name { get; set; }
		public string MobilePhone { get; set; }
		public string AddressBookId { get; set; }
		public bool Selected { get; set; }
		public int SmsGroupId { get; set; }
		
		public override string ToString ()
		{
			return Name;
		}
	}
}