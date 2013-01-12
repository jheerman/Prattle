using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Prattle.Android.Core;

namespace Prattle.Activities
{
	[Activity (Label = "Prattle Message", Theme="@style/Theme.ActionLight", WindowSoftInputMode=SoftInput.AdjustResize, NoHistory=true)]
	public class SendMessageActivity : SmsServiceActivity
	{
		SmsGroupRepository _smsGroupRepo;
		ContactRepository _contactRepo;
		ProgressDialog _progressDialog;
		
		List<Contact> _recipients;
		SmsGroup _smsGroup;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			BindService ();
			
			SetContentView(Resource.Layout.SendMessage);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			
			var groupId = Intent.GetIntExtra("groupId", 0);
			_smsGroupRepo = new SmsGroupRepository();
			_smsGroup = _smsGroupRepo.Get (groupId);
			
			_contactRepo = new ContactRepository(this);
			_recipients = _contactRepo.GetMembersForSmsGroup (groupId);
			
			FindViewById <TextView>(Resource.Id.recipientGroup).Text = _smsGroup.Name;
			FindViewById <TextView>(Resource.Id.recipients).Text = string.Join (", ", _recipients.Select (c => c.Name));
			FindViewById <ImageButton>(Resource.Id.cmdSend).Click += (sender, e) => 
				{
					_progressDialog = new ProgressDialog(this);
					_progressDialog.SetTitle ("Sending Messages");
					_progressDialog.SetMessage ("Please wait.  Sending message to recipients in group...");
				
					Task.Factory
						.StartNew(() =>
							QueueMessage ())
						.ContinueWith(task =>
							RunOnUiThread(() => {
								if (task.Result)
								{
									new AlertDialog.Builder(this)
										.SetTitle ("Messages Queued")
										.SetMessage (string.Format ("Your message was queued to be sent to each recipient of the '{0}' group", 
										                            _smsGroup.Name))
										.SetPositiveButton ("Ok", (o, args) => {
													var homeIntent = new Intent();
													homeIntent.PutExtra ("defaultTab", 0);
													homeIntent.AddFlags (ActivityFlags.ClearTop);
													homeIntent.SetClass (this, typeof(MainActivity));
													StartActivity(homeIntent);
												})
										.Show ();
								}
								else
								{
									new AlertDialog.Builder(this)
										.SetTitle ("Message Error")
										.SetMessage (string.Format ("Doh!  Your message could not be sent to the '{0}' group.",
										                            _smsGroup.Name))
										.Show ();
								}
								_progressDialog.Dismiss ();
							}));
				};
		}
		
		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			UnbindService ();

			_recipients = null;
			_smsGroup = null;
			_smsGroupRepo = null;
			_contactRepo = null;
		}
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.message_edit_bar, menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
				case global::Android.Resource.Id.Home:
					new AlertDialog.Builder(this)
						.SetTitle ("Cancel")
						.SetMessage ("Are you sure you want to cancel your message?")
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
				case Resource.Id.menuCancelMessage:
					new AlertDialog.Builder(this)
						.SetTitle ("Cancel")
						.SetMessage ("Are you sure you want to cancel your message?")
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
		
		private bool QueueMessage()
		{
			try
			{
				var messageText = FindViewById<EditText>(Resource.Id.message).Text;
	
				//Check to see if any recipients were removed from list
				var strContacts = FindViewById<EditText>(Resource.Id.recipients).Text;
				var contacts = strContacts.Split (',');
				var messageRecipients = contacts.Select(t => _recipients.FirstOrDefault(r => r.Name == t.Trim()))
                    .Where(recipient => recipient != null).ToList();

			    BoundService.SendMessage (messageText, _smsGroup, messageRecipients);

			    return true;
			}
			catch
			{
				return false;
			}
		}
	}
}