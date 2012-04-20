using System;
using System.Collections.Generic;

using Prattle.Android.Core;

using Android.Widget;
using Android.App;
using Android.Views;

namespace Prattle
{
	public class ContactListAdapter : BaseAdapter
	{
		private Activity _context;
		private List<Contact> _contacts;

		public ContactListAdapter (Activity context, List<Contact> contacts) : base ()
		{
			_context = context;
			_contacts = contacts;
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return position;
		}

		public Contact GetContact (int position)
		{
			return _contacts[position];
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override int Count 
		{
			get { return _contacts.Count; }
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var contact = _contacts[position];
			
			var view = convertView ??
				_context.LayoutInflater.Inflate(Resource.Layout.contact_item, null);
			
			view.FindViewById<CheckedTextView>(Resource.Id.contactName).Text = contact.Name;
			view.FindViewById<CheckedTextView>(Resource.Id.contactName).Checked = contact.Selected;
			view.FindViewById<TextView>(Resource.Id.mobileNumber).Text = contact.MobilePhone;
			return view;
		}
	}
}