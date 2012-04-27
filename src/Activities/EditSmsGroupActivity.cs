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
	[Activity (Label = "Edit SMS Group", Theme="@style/Theme.ActionLight", NoHistory=true)]
	public class EditSmsGroupActivity : SmsGroupListActivity
	{
		private ContactRepository _contactRepo;
		private ProgressDialog _progressDialog;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			var groupId = Intent.GetIntExtra ("groupId", -1);
			base._groupId = groupId;
			
			_progressDialog = new ProgressDialog(this);
			_progressDialog.SetMessage("Loading Contacts.  Please wait...");
			_progressDialog.Show();
			
			Task.Factory
				.StartNew(() =>
					GetContacts(groupId))
				.ContinueWith(task =>
					RunOnUiThread(() => {
						DisplayContacts(task.Result);
						_progressDialog.Dismiss ();
					}));
		}
		
		private List<Contact> GetContacts(int groupId)
		{
			_contactRepo = new ContactRepository(this);
			var selectedContacts = _contactRepo.GetMembersForSMSGroup(groupId);
			var contacts = _contactRepo.GetAllMobile ();
			
			foreach (var selectedContact in selectedContacts)
			{
				var contact = contacts.First(c => c.AddressBookId == selectedContact.AddressBookId);
				contact.Selected = true;
				contact.Id = selectedContact.Id;
			}
			
			return contacts;
		}
	}
}

