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
using V = Android.Views;
using T = Android.Telephony;

using Prattle.Android.Core;
using Android.Views.InputMethods;

namespace Prattle
{
	[Activity (Label = "Prattle Message", Theme="@style/Theme.ActionLight", WindowSoftInputMode=V.SoftInput.AdjustResize, NoHistory=true)]
	public class SendMessageActivity : Activity
	{
		SmsGroupRepository _smsGroupRepo;
		ContactRepository _contactRepo;
		Repository<SmsMessage> _messageRepo;
		ProgressDialog _progressDialog;
		
		SmsGroup _smsGroup;
		List<Contact> _recipients;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.SendMessage);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			
			var groupId = Intent.GetIntExtra("groupId", 0);
			_smsGroupRepo = new SmsGroupRepository();
			_smsGroup = _smsGroupRepo.Get (groupId);
			
			_contactRepo = new ContactRepository(this);
			_recipients = _contactRepo.GetMembersForSMSGroup (groupId);
			
			FindViewById <TextView>(Resource.Id.recipientGroup).Text = _smsGroup.Name;
			FindViewById <TextView>(Resource.Id.recipients).Text = string.Join (", ", _recipients.Select (c => c.Name));
			FindViewById <Button>(Resource.Id.cmdSend).Click += (sender, e) => 
				{
					_progressDialog = new ProgressDialog(this);
					_progressDialog.SetTitle ("Sending Messages");
					_progressDialog.SetMessage ("Please wait.  Sending message to recipients in group...");
				
					Task.Factory
						.StartNew(() =>
							SendMessage ())
						.ContinueWith(task =>
							RunOnUiThread(() => {
								if (task.Result)
								{
									new AlertDialog.Builder(this)
										.SetTitle ("Message Sent Successfully")
										.SetMessage (string.Format ("Your message was sent to each recipient of the '{0}' group", 
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
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.message_edit_bar, menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
				case R.Id.Home:
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
				default:
					break;
			}
			return true;
		}
		
		private bool SendMessage ()
		{
			try
			{
				var messageText = FindViewById<EditText>(Resource.Id.message).Text;
				
				//Check to see if any recipients were removed from list
				var strContacts = FindViewById<EditText>(Resource.Id.recipients).Text;
				var contacts = strContacts.Split (',');
				var newRecipients = new List<Contact>();
				
				for (var i=0; i<contacts.Length; i++)
				{
					var recipient = _recipients.FirstOrDefault (r => r.Name == contacts[i].Trim ());
					if (recipient == null) continue;
					newRecipients.Add (recipient);
				}
				
				var dateSent = DateTime.Now;
				newRecipients.ForEach (recipient => {
					var message = new SmsMessage{
						Text = messageText,
						SmsGroupId = _smsGroup.Id,
						ContactAddressBookId = recipient.AddressBookId,
						ContactName = recipient.Name,
						SentDate = dateSent
					};
					_messageRepo = new Repository<SmsMessage>();
					_messageRepo.Save (message);
					
					//T.SmsManager.Default.SendTextMessage (recipient.MobilePhone, null, message.Text, null, null);
				});
				return true;
			}
			catch 
			{
				return false;
			}
		}
	}
}