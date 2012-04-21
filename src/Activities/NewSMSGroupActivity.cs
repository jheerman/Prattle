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
	[Activity (Label = "Group Contacts", NoHistory = true)]
	public class NewSMSGroupActivity : ListActivity
	{
		ProgressDialog _progressDialog;
		ContactRepository _contactRepo;
		Repository<SMSGroup> _smsRepo;
		
		List<Contact> _contacts;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			_contactRepo = new ContactRepository(this);
			_smsRepo = new Repository<SMSGroup>();

			Title = Intent.GetStringExtra("name");
			
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
				case Resource.Id.menuSaveGroup:
					_progressDialog.SetMessage("Saving SMS Group.  Please wait...");
					_progressDialog.Show();
					Task.Factory
						.StartNew(() =>
							SaveGroup())
						.ContinueWith(task =>
							RunOnUiThread(() => CompleteSave()));
					break;
				case Resource.Id.menuCancelEdit:
					new AlertDialog.Builder(this)
						.SetTitle ("Cancel")
						.SetMessage ("Are you sure you want to cancel your changes for this SMS Group?")
						.SetPositiveButton ("Yes", (o, e) => {
								var homeIntent = new Intent();
								homeIntent.SetClass (this, typeof(MainActivity));
								StartActivity(homeIntent);
							})
						.SetNegativeButton ("No", (o, e) => { })
						.Show ();
					break;
				default:
					break;
			}
			return true;
		}
		
		private void DisplayContacts(List<Contact> contacts)
		{
			//save local copy of the contacts for updating
			_contacts = contacts;
			
			ListAdapter = new ContactListAdapter(this, contacts);
			ListView.TextFilterEnabled = true;
			ListView.ChoiceMode = ChoiceMode.Multiple;
			ListView.ItemClick += delegate(object sender, ItemEventArgs e) {
				contacts[e.Position].Selected = ListView.IsItemChecked (e.Position);
			};
			_progressDialog.Dismiss ();
		}
		
		private void SaveGroup()
		{
			var smsGroup = new SMSGroup();
			var selectedContacts = _contacts.Where (c => c.Selected);
			
			smsGroup.Name = Intent.GetStringExtra ("name");
			smsGroup.CreatedDate = DateTime.Now;
			smsGroup.UUID = Guid.NewGuid ().ToString ();
			smsGroup.MemberCount = selectedContacts.Count ();
			smsGroup.Id = _smsRepo.Save (smsGroup);
			
			foreach (var contact in selectedContacts)
			{
				contact.CreatedDate = DateTime.Now;
				contact.UUID = Guid.NewGuid ().ToString ();
				contact.SMSGroupId = smsGroup.Id;
				_contactRepo.Save(contact);
			}
		}
		
		private void CompleteSave()
		{
			_progressDialog.Dismiss ();
			var homeIntent = new Intent();
			homeIntent.PutExtra ("defaultTab", 1);
			homeIntent.SetClass (this, typeof(MainActivity));
			StartActivity(homeIntent);
		}
	}
}