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
	[Activity (Label = "Group Contacts", Theme="@style/Theme.ActionLight", NoHistory = true)]
	public class NewSMSGroupActivity : SMSGroupListActivity
	{
		ProgressDialog _progressDialog;
		ContactRepository _contactRepo;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		
			var groupName = Intent.GetStringExtra("name");
			Title = groupName;
			base._groupName = groupName;
			
			_contactRepo = new ContactRepository(this);
			
			_progressDialog = new ProgressDialog(this);
			_progressDialog.SetMessage("Loading Contacts.  Please wait...");
			_progressDialog.Show();
			
			Task.Factory
				.StartNew(() =>
					_contactRepo.GetAllMobile())
				.ContinueWith(task =>
					RunOnUiThread(() => 
					{
						DisplayContacts(task.Result);
						_progressDialog.Dismiss ();
					}));
		}
	}
}