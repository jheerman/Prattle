using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Prattle.Android.Core;

namespace Prattle
{
	[Activity (Label = "SMSGroupActivity")]
	public class SMSGroupActivity : ListActivity
	{
		ProgressDialog _progressDialog;
		ContactRepository _contactRepo;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			_contactRepo = new ContactRepository(this);

			// Create your application here
			var name = this.Intent.GetStringExtra("name");
			Title = name;
			
			_progressDialog = new ProgressDialog(this);
			_progressDialog.SetMessage("Loading Contacts.  Please wait...");
			_progressDialog.Show();
			
			Task.Factory
				.StartNew(() =>
					_contactRepo.GetAllMobile())
				.ContinueWith(task =>
					RunOnUiThread(() => DisplayContacts(task.Result)));
		}
		
		private void DisplayContacts(List<Contact> contacts)
		{
			ListAdapter = new ArrayAdapter<string> (this, Resource.Layout.sms_item, contacts.Select (c => c.Name).ToArray());
			ListView.TextFilterEnabled = true;
			_progressDialog.Dismiss ();
		}
	}
}