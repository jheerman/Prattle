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
using R = Android.Resource;

using Prattle.Android.Core;
using System.Threading.Tasks;

namespace Prattle
{
	[Activity (Label = "Edit SMS Group")]
	public class EditSMSGroupActivity : ListActivity
	{
		private ContactRepository _contactRepo;
		private ProgressDialog _progressDialog;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			_contactRepo = new ContactRepository(this);
			
			var groupId = Intent.GetIntExtra ("groupId", -1);
			
			_progressDialog = new ProgressDialog(this);
			_progressDialog.SetMessage("Loading Contacts.  Please wait...");
			_progressDialog.Show();
			
			Task.Factory
				.StartNew(() =>
					GetContacts(groupId))
				.ContinueWith(task =>
					RunOnUiThread(() => DisplayContacts(task.Result)));
			
			ActionBar.SetDisplayHomeAsUpEnabled (true);
		}
		
		private List<Contact> GetContacts(int groupId)
		{
			var selectedContacts = _contactRepo.GetMembersForSMSGroup(groupId);
			var contacts = _contactRepo.GetAllMobile ();
			var selected = contacts.Where (contact => selectedContacts.Contains (contact));
			foreach (var contact in selected)
				contact.Selected = true;
			
			return contacts;
		}
		
		private void DisplayContacts(List<Contact> contacts)
		{
			ListAdapter = new ContactListAdapter(this, contacts);
			ListView.TextFilterEnabled = true;
			ListView.ChoiceMode = ChoiceMode.Multiple;
			ListView.ItemClick += delegate(object sender, ItemEventArgs e) {
				contacts[e.Position].Selected = ListView.IsItemChecked (e.Position);
			};
			_progressDialog.Dismiss ();
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
	}
}

