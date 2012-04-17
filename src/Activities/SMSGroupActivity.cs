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
using R = Android.Resource;

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
			
			ActionBar.SetDisplayHomeAsUpEnabled (true);
		}
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.group_edit_bar, menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
				case R.Id.Home:
					var intent = new Intent(this, typeof(MainActivity));
					intent.AddFlags (ActivityFlags.ClearTop);
					StartActivity (intent);
					break;
				default:
					break;
			}
			return true;
		}
		
		private void DisplayContacts(List<Contact> contacts)
		{
			ListAdapter = new ContactListAdapter(this, contacts);
			ListView.TextFilterEnabled = true;
			_progressDialog.Dismiss ();
		}
	}
}