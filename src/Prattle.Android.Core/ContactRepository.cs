using System;
using System.Linq;
using System.Collections.Generic;

using Android.Content;
using Prattle.Domain;
using Xamarin.Contacts;

namespace Prattle.Android.Core
{
	public class ContactRepository : Repository<Contact>, IContactRepository<Contact>
	{
		AddressBook _book;
		public ContactRepository (Context context)
		{
			_book = new AddressBook(context);
		}
		
		public Contact GetByAddressBookId (string addressBookId)
		{
			_book.PreferContactAggregation = true;
			
			var contact = _book.FirstOrDefault (c => c.Id == addressBookId);
			if (contact == null) return null;
			
			return new Contact {
				AddressBookId = contact.Id,
				Name = contact.DisplayName,
				MobilePhone = contact.Phones.FirstOrDefault(pt => pt.Type == PhoneType.Mobile).Number
			};
		}
		
		public new List<Contact> GetAll()
		{
			var contacts = new List<Contact>();
			_book.PreferContactAggregation = true;
			foreach (var contact in _book)
				contacts.Add ( new Contact 
								{
									AddressBookId = contact.Id,
									Name = contact.DisplayName
								});
			
			return contacts;
		}
		
		public List<Contact> GetAllMobile()
		{
			var contacts = new List<Contact>();
			_book.PreferContactAggregation = true;
			
			foreach (var contact in _book.Where (c => c.Phones.Any(p => p.Type == PhoneType.Mobile)))
				contacts.Add ( new Contact 
								{
									AddressBookId = contact.Id,
									Name = contact.DisplayName,
									MobilePhone = contact.Phones.FirstOrDefault (p => p.Type == PhoneType.Mobile).Number
								});
			
			return contacts;
		}
		
		public List<Contact> GetMembersForSMSGroup (int groupId)
		{
			return (from contact in cn.Table<Contact>()
			        where contact.SMSGroupId == groupId
			        where contact.Selected == true
			        select contact).ToList ();
		}
	}
}