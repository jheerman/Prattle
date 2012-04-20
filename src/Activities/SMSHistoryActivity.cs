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

namespace Prattle
{
	[Activity (Label = "SMSHistory")]
	public class SMSHistoryActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView (Resource.Layout.SMSHistory);
			var container = FindViewById<LinearLayout>(Resource.Id.smsContainer);
			container.AddView (new TextView(this) { Text = "No Message History" });
		}
	}
}

