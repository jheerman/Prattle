using System.Collections.Generic;
using Prattle.Android.Core;
using Android.Widget;
using Android.App;
using Android.Views;

namespace Prattle.Views
{
	public class ContactListAdapter : BaseAdapter
	{
		private readonly Activity _context;
		private readonly List<Contact> _contacts;

		public ContactListAdapter (Activity context, List<Contact> contacts)
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

			var contactView = view.FindViewById<CheckedTextView>(Resource.Id.contactName);
			contactView.Text = contact.Name;
			contactView.Checked = contact.Selected;
			view.FindViewById<TextView>(Resource.Id.mobileNumber).Text = contact.MobilePhone;
			return view;
		}
	}
}