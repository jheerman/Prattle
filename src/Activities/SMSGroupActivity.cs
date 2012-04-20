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
	[Activity (Label = "SMSGroupActivity")]
	public class SMSGroupActivity : Activity
	{
		Repository<SMSGroup> _smsGroupRepo = new Repository<SMSGroup>();
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView (Resource.Layout.SMSGroup);
			
			var container = FindViewById<LinearLayout>(Resource.Id.smsContainer);
			
			var smsGroups = _smsGroupRepo.GetAll ();
			if (smsGroups == null || smsGroups.ToList ().Count == 0)
				container.AddView (new TextView (this) { Text = "No SMS Groups Found" });
		}
	}
}

