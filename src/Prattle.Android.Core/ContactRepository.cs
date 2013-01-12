using System.Linq;
using System.Collections.Generic;

using Android.Content;
using Prattle.Domain;
using Xamarin.Contacts;

namespace Prattle.Android.Core
{
	public class ContactRepository : Repository<Contact>, IContactRepository<Contact>
	{
	    readonly AddressBook _book;
		public ContactRepository (Context context)
		{
			_book = new AddressBook(context);
		}
		
		public Contact GetByAddressBookId (string addressBookId)
		{
			_book.PreferContactAggregation = true;
			
			var contact = _book.FirstOrDefault (c => c.Id == addressBookId);
			if (contact == null) return null;

		    var firstOrDefault = contact.Phones.FirstOrDefault(pt => pt.Type == PhoneType.Mobile);
		    if (firstOrDefault != null)
		        return new Contact {
		            AddressBookId = contact.Id,
		            Name = contact.DisplayName,
		            MobilePhone = firstOrDefault.Number
		        };
		    return null;
		}
		
		public new List<Contact> GetAll()
		{
		    _book.PreferContactAggregation = true;

		    return _book.Select(contact => new Contact
		        {
		            AddressBookId = contact.Id, Name = contact.DisplayName
		        }).ToList();
		}
		
		public List<Contact> GetAllMobile()
		{
		    _book.PreferContactAggregation = true;

		    return _book.Where(c => c.Phones.Any(p => p.Type == PhoneType.Mobile)).Select(contact => new Contact
		        {
		            AddressBookId = contact.Id, Name = contact.DisplayName, MobilePhone = contact.Phones.FirstOrDefault(p => p.Type == PhoneType.Mobile).Number
		        }).ToList();
		}
		
		public List<Contact> GetMembersForSmsGroup (int groupId)
		{
			return (from contact in Cn.Table<Contact>()
			        where contact.SmsGroupId == groupId
			        where contact.Selected
			        select contact).ToList ();
		}
	}
}