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

using Prattle.Android.Core;

namespace Prattle
{
	[BroadcastReceiver]
	public class PrattleSmsReceiver : BroadcastReceiver
	{
		Repository<SmsMessage> _messageRepo;
		public SmsMessage Message { get; set; }
		
		public override void OnReceive (Context context, Intent intent)
		{
			switch (ResultCode)
			{
				case Result.Ok:
					_messageRepo = new Repository<SmsMessage>();
					_messageRepo.Save (Message);
					Toast.MakeText (context, string.Format ("Message sent to {0}", Message.ContactName), ToastLength.Short).Show ();
					break;
				default:
					Toast.MakeText (context, "Doh!  Message was unsuccessful.", ToastLength.Short).Show ();
					break;
			}
			_messageRepo = null;
		}
	}
}