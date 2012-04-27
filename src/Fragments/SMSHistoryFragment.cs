using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Prattle
{
	public class SMSHistoryFragment: Fragment
	{
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			
			var view = inflater.Inflate (Resource.Layout.SMSHistory, container, false);
			view.FindViewById<TextView>(Resource.Id.sampleTextView).Text = "No Message History";
			
			return view;
		}
	}
}