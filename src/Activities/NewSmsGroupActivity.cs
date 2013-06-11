using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Prattle.Android.Core;

namespace Prattle.Activities
{
	[Activity (Label = "Group Contacts", Theme="@style/Theme.ActionLight", NoHistory = true)]
	public class NewSmsGroupActivity : SmsGroupListActivity
	{
		ProgressDialog _progressDialog;
		ContactRepository _contactRepo;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		
			var groupName = Intent.GetStringExtra("name");
			Title = groupName;
			GroupName = groupName;
			
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
						if (task.Result != null)
							DisplayContacts(task.Result);
						_progressDialog.Dismiss ();
					}));
		}
	}
}