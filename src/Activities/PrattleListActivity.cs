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
	[Activity (Label = "PrattleListActivity")]
	public class PrattleListActivity : ListActivity
	{
		private ProgressDialog _progressDialog;
		private List<Contact> _contacts;
		
		private SMSGroupRepository _smsRepo;
		private ContactRepository _contactRepo;
		
		public int _groupId;
		public string _groupName;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
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
					intent.PutExtra ("defaultTab", 1);
					intent.AddFlags (ActivityFlags.ClearTop);
					StartActivity (intent);
					break;
				case Resource.Id.menuSaveGroup:
					_progressDialog = new ProgressDialog(this);
					_progressDialog.SetMessage("Saving SMS Group.  Please wait...");
					_progressDialog.Show();
					Task.Factory
						.StartNew(() =>
							SaveGroup())
						.ContinueWith(task =>
							RunOnUiThread(() => 
									{
										_progressDialog.Dismiss ();
										CompleteSave();
									}));
					break;
				case Resource.Id.menuCancelEdit:
					new AlertDialog.Builder(this)
						.SetTitle ("Cancel")
						.SetMessage ("Are you sure you want to cancel your changes for this SMS Group?")
						.SetPositiveButton ("Yes", (o, e) => {
								var homeIntent = new Intent();
								homeIntent.PutExtra ("defaultTab", 1);
								homeIntent.AddFlags (ActivityFlags.ClearTop);
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
		
		protected void DisplayContacts(List<Contact> contacts)
		{
			//save local copy of the contacts for updating
			_contacts = contacts;
			
			ListAdapter = new ContactListAdapter(this, contacts);
			ListView.TextFilterEnabled = true;
			ListView.ChoiceMode = ChoiceMode.Multiple;
			ListView.ItemClick += delegate(object sender, ItemEventArgs e) {
				contacts[e.Position].Selected = ListView.IsItemChecked (e.Position);
			};
		}
		
		private void SaveGroup()
		{
			SMSGroup smsGroup;
			
			//get the selected contacts
			var selectedContacts = _contacts.Where (c => c.Selected);
			
			_smsRepo = new SMSGroupRepository();
			_contactRepo = new ContactRepository(this);
			
			if (string.IsNullOrEmpty (_groupName))
			{
				smsGroup = _smsRepo.Get (_groupId);
				smsGroup.MemberCount = selectedContacts.Count();
				smsGroup.ModifiedDate = DateTime.Now;
				_smsRepo.Save (smsGroup);
				
				_contactRepo.GetMembersForSMSGroup (_groupId).ForEach (c => {
					c.Selected = false;
					c.ModifiedDate = DateTime.Now;
					_contactRepo.Save (c);
				});
			}
			else
			{
				smsGroup = new SMSGroup();
				smsGroup.Name = _groupName;
				smsGroup.CreatedDate = DateTime.Now;
				smsGroup.UUID = Guid.NewGuid ().ToString ();
				smsGroup.MemberCount = selectedContacts.Count ();
				_smsRepo.Save (smsGroup);
			}
			
			foreach (var contact in selectedContacts)
			{
				if (contact.Id == 0)
				{
					contact.Selected = true;
					contact.CreatedDate = DateTime.Now;
					contact.UUID = Guid.NewGuid ().ToString ();
					contact.SMSGroupId = smsGroup.Id;
					_contactRepo.Save(contact);
				}
				else
				{
					contact.Selected = true;
					contact.SMSGroupId = smsGroup.Id;
					contact.ModifiedDate = DateTime.Now;
					_contactRepo.Save (contact);
				}
			}
		}
		
		private void CompleteSave()
		{
			var homeIntent = new Intent();
			homeIntent.PutExtra ("defaultTab", 1);
			homeIntent.AddFlags (ActivityFlags.ClearTop);
			homeIntent.SetClass (this, typeof(MainActivity));
			StartActivity(homeIntent);
		}
	}
}

