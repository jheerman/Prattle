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
using T = Android.Telephony;

using Prattle.Android.Core;
using Android.Util;

namespace Prattle
{
	[Service]
	public class PrattleSmsService : Service
	{
		PrattleSmsReceiver _receiver;
		IBinder _binder;
		PendingIntent _sentIntent;
		
		public PrattleSmsService ()
		{
			_binder = new LocalBinder(this);
		}
		
		public override void OnCreate ()
		{
			base.OnCreate ();

			_receiver = new PrattleSmsReceiver();
			RegisterReceiver (_receiver, new IntentFilter("SMS_SENT"));
		}
		
		public override IBinder OnBind (Intent intent)
		{
			return _binder;
		}
		
		public override void OnStart (Intent intent, int startId)
		{
			base.OnStart (intent, startId);
		}
		
		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			return StartCommandResult.NotSticky;
		}
		
		public override void OnDestroy ()
		{
			base.OnDestroy ();
			UnregisterReceiver (_receiver);
		}
		
		public bool SendMessage (string messageText, SmsGroup smsGroup, List<Contact> recipients)
		{
			var dateSent = DateTime.Now;
			recipients.ForEach (recipient => {
				var messageIntent = new Intent("SMS_SENT");
				messageIntent.PutExtra ("messageText", messageText);
				messageIntent.PutExtra ("smsGroupId", smsGroup.Id);
				messageIntent.PutExtra ("addressBookId", recipient.AddressBookId);
				messageIntent.PutExtra ("contactName", recipient.Name);
				messageIntent.PutExtra ("dateSent", dateSent.ToString ("M/d/yyyy hh:mm:ss tt"));
				
				_sentIntent = PendingIntent.GetBroadcast (ApplicationContext, 0, messageIntent, PendingIntentFlags.UpdateCurrent);
				T.SmsManager.Default.SendTextMessage (recipient.MobilePhone, null, messageText, _sentIntent, null);
			});
			return true;
		}
		
		public class LocalBinder : Binder
		{
			PrattleSmsService _service;

			public LocalBinder (PrattleSmsService service)
			{
				_service = service;
			}

			public PrattleSmsService Service
			{
				get { return _service; }
			}
		}
	}
}