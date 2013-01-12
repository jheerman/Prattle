using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Prattle.Android.Core;
using SmsMessage = Prattle.Android.Core.SmsMessage;

namespace Prattle.Receivers
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
				case (int)global::Android.Telephony.SmsResultError.NoService:
				case (int)global::Android.Telephony.SmsResultError.RadioOff:
				case (int)global::Android.Telephony.SmsResultError.NullPdu:
				case (int)global::Android.Telephony.SmsResultError.GenericFailure:
				default:
					Toast.MakeText (context, string.Format("Doh! Message could not be sent to {0}.", message.ContactName), 
				                ToastLength.Short).Show ();
					break;
			}
			_messageRepo = null;
		}
	}
}