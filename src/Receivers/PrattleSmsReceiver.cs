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

namespace Prattle
{
	[BroadcastReceiver]
	public class PrattleSmsReceiver : BroadcastReceiver
	{
		Repository<SmsMessage> _messageRepo;
		
		public override void OnReceive (Context context, Intent intent)
		{
			var message = new SmsMessage {
				Text = intent.GetStringExtra ("messageText"),
				SmsGroupId = intent.GetIntExtra ("smsGroupId", -1),
				ContactAddressBookId = intent.GetStringExtra ("addressBookId"),
				ContactName = intent.GetStringExtra ("contactName"),
				SentDate = DateTime.Parse (intent.GetStringExtra ("dateSent"))
			};
			
			switch ((int)ResultCode)
			{
				case (int)Result.Ok:
					_messageRepo = new Repository<SmsMessage>();
					_messageRepo.Save (message);
					Toast.MakeText (context, string.Format ("Message sent to {0}", message.ContactName), ToastLength.Short).Show ();
					break;
				case (int)T.SmsResultError.NoService:
				case (int)T.SmsResultError.RadioOff:
				case (int)T.SmsResultError.NullPdu:
				case (int)T.SmsResultError.GenericFailure:
				default:
					Toast.MakeText (context, string.Format("Doh! Message could not be sent to {0}.", message.ContactName), 
				                ToastLength.Short).Show ();
					break;
			}
			_messageRepo = null;
		}
	}
}