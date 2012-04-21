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
	[Activity (Label = "Edit SMS Group")]
	public class EditSMSGroupActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView (Resource.Layout.EditSMSGroup);
			
			var textView = new TextView(this);
			textView.Text = "Edit SMS";
			FindViewById<LinearLayout>(Resource.Id.smsContainer).AddView (textView);
			
			ActionBar.SetDisplayHomeAsUpEnabled (true);
		}
	}
}

