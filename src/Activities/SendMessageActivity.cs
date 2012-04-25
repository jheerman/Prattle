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
using Android.Views.InputMethods;

namespace Prattle
{
	[Activity (Label = "Send Message", Theme="@style/Theme.ActionLight")]
	public class SendMessageActivity : Activity
	{
		SMSGroupRepository _smsGroupRepo;
		ContactRepository _contactRepo;
		
		SMSGroup _smsGroup;
		List<Contact> _recipients;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.SendMessage);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			
			var groupId = Intent.GetIntExtra("groupId", 0);
			_smsGroupRepo = new SMSGroupRepository();
			_smsGroup = _smsGroupRepo.Get (groupId);
			
			_contactRepo = new ContactRepository(this);
			_recipients = _contactRepo.GetMembersForSMSGroup (groupId);
			
			FindViewById <TextView>(Resource.Id.recipientGroup).Text = _smsGroup.Name;
			FindViewById <TextView>(Resource.Id.recipients).Text = string.Join (", ", _recipients.Select (c => c.Name));
			
			var message = FindViewById<EditText>(Resource.Id.message);
			var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
			imm.ShowSoftInput (message, (int)ShowSoftInputFlags.Explicit);
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
					var intent = new Intent(this, typeof(MainActivity));
					intent.PutExtra ("defaultTab", 1);
					intent.AddFlags (ActivityFlags.ClearTop);
					StartActivity (intent);
					break;
				case Resource.Id.menuCancelMessage:
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
	}
}