using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Prattle.Android.Core;
using Prattle.Views;

namespace Prattle.Activities
{
	[Activity (Label = "SMSGroupListActivity")]
	public class SmsGroupListActivity : ListActivity
	{
		private ProgressDialog _progressDialog;
		private List<Contact> _contacts;
		
		private SmsGroupRepository _smsRepo;
		private ContactRepository _contactRepo;
		
		public int GroupId;
		public string GroupName;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			ActionBar.SetDisplayHomeAsUpEnabled (true);
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();

			_contacts = null;
			_contactRepo = null;
			_smsRepo = null;
		}
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.group_edit_bar, menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
				case global::Android.Resource.Id.Home:
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
						.StartNew(SaveGroup)
						.ContinueWith(task =>
							RunOnUiThread(() => 
									{
										_progressDialog.Dismiss ();
										var homeIntent = new Intent();
										homeIntent.PutExtra ("defaultTab", 1);
										homeIntent.AddFlags (ActivityFlags.ClearTop);
										homeIntent.SetClass (this, typeof(MainActivity));
										StartActivity(homeIntent);
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
			}
			return true;
		}
		
		protected void DisplayContacts(List<Contact> contacts)
		{
			//save local copy of the contacts for updating
			_contacts = contacts;
			
			ListAdapter = new ContactListAdapter(this, contacts);
			ListView.TextFilterEnabled = true;
			ListView.ItemsCanFocus = true;
			ListView.ChoiceMode = ChoiceMode.Multiple;
			ListView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs  e) {
				var itemChecked = ListView.IsItemChecked (e.Position);
				contacts[e.Position].Selected = itemChecked;
				ListView.SetItemChecked (e.Position, itemChecked);
			};
		}
		
		private void SaveGroup()
		{
			SmsGroup smsGroup;
			
			//get the selected contacts
			var selectedContacts = _contacts.Where (c => c.Selected);
			
			_smsRepo = new SmsGroupRepository();
			_contactRepo = new ContactRepository(this);

		    var enumerable = selectedContacts as IList<Contact> ?? selectedContacts.ToList();
		    if (string.IsNullOrEmpty (GroupName))  //if updating an existing group
			{
				smsGroup = _smsRepo.Get (GroupId);
				smsGroup.MemberCount = enumerable.Count();
				smsGroup.ModifiedDate = DateTime.Now;
				_smsRepo.Save (smsGroup);
				
				//reset previously selected contacts
				_contactRepo.GetMembersForSmsGroup (GroupId).ForEach (c => {
					c.Selected = false;
					c.ModifiedDate = DateTime.Now;
					_contactRepo.Save (c);
				});
			}
			else  //if new group
			{
				smsGroup = new SmsGroup
				    {
				        Name = GroupName,
				        CreatedDate = DateTime.Now,
				        UUID = Guid.NewGuid().ToString(),
				        MemberCount = enumerable.Count()
				    };
			    _smsRepo.Save (smsGroup);
			}
			
			foreach (var contact in enumerable)
			{
				if (contact.Id == 0)  //if new contact
				{
					contact.Selected = true;
					contact.CreatedDate = DateTime.Now;
					contact.UUID = Guid.NewGuid ().ToString ();
					contact.SmsGroupId = smsGroup.Id;
					_contactRepo.Save(contact);
				}
				else  //if update existing
				{
					contact.Selected = true;
					contact.SmsGroupId = smsGroup.Id;
					contact.ModifiedDate = DateTime.Now;
					_contactRepo.Save (contact);
				}
			}
		}
	}
}