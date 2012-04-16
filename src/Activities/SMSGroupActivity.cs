using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SQLite;
using Xamarin.Contacts;
using System.Threading.Tasks;

namespace Prattle
{
	[Activity (Label = "SMSGroupActivity")]
	public class SMSGroupActivity : ListActivity
	{
		ProgressDialog _progressDialog;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			var name = this.Intent.GetStringExtra("name");
			Title = name;
			
			_progressDialog = new ProgressDialog(this);
			_progressDialog.SetMessage("Loading Contacts.  Please wait...");
			_progressDialog.Show();
			
			Task.Factory
					.StartNew(() =>
						GetContacts())
					.ContinueWith(task =>
						RunOnUiThread(() => DisplayContacts(task.Result)));
		}
		
		private List<string> GetContacts()
		{
			List<string> contacts = new List<string>();
			var book = new AddressBook(this);
			book.PreferContactAggregation = true;
			
			foreach (var contact in book.Where (c => c.Phones.Any(p => p.Type == PhoneType.Mobile)))
				contacts.Add (contact.DisplayName);
			
			return contacts;
		}
		
		private void DisplayContacts(List<string> contacts)
		{
			ListAdapter = new ArrayAdapter<string> (this, Resource.Layout.list_item, contacts.ToArray());
			ListView.TextFilterEnabled = true;
			_progressDialog.Dismiss ();
		}
	}
}
