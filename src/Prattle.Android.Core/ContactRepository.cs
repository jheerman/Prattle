using System;
using System.Linq;
using System.Collections.Generic;

using Android.Content;

using Prattle.Domain;

using Xamarin.Contacts;

namespace Prattle.Android.Core
{
	public class ContactRepository : IContactRepository<Contact>
	{
		Context _context;
		public ContactRepository (Context context)
		{ 
			_context = context;
		}
		
		public Contact Get(string id)
		{
			return null;
		}
		
		public List<Contact> GetAll()
		{
			var contacts = new List<Contact>();
			var book = new AddressBook(_context);
			book.PreferContactAggregation = true;
			foreach (var contact in book)
				contacts.Add ( new Contact 
								{
									Id = contact.Id,
									Name = contact.DisplayName
								});
			
			return contacts;
		}
		
		public List<Contact> GetAllMobile()
		{
			var contacts = new List<Contact>();
			var book = new AddressBook(_context);
			book.PreferContactAggregation = true;
			
			foreach (var contact in book.Where (c => c.Phones.Any(p => p.Type == PhoneType.Mobile)))
				contacts.Add ( new Contact 
								{
									Id = contact.Id,
									Name = contact.DisplayName
								});
			
			return contacts;
		}
	}
}