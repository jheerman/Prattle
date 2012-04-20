using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Prattle
{
	[Activity (Label = "Prattle")]
	public class MainActivity : TabActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);
			
			TabHost.TabSpec spec;
			
			var intent = new Intent (this, typeof (SMSHistoryActivity));
			intent.AddFlags (ActivityFlags.NewTask);
			spec = TabHost.NewTabSpec ("history");
			spec.SetIndicator ("History", Resources.GetDrawable (Resource.Drawable.ic_tab_sms_history));
			spec.SetContent (intent);
			TabHost.AddTab (spec);
			
			intent = new Intent (this, typeof (SMSGroupActivity));
			intent.AddFlags (ActivityFlags.NewTask);
			spec = TabHost.NewTabSpec ("groups");
			spec.SetIndicator ("Groups", Resources.GetDrawable (Resource.Drawable.ic_tab_sms_group));
			spec.SetContent (intent);
			TabHost.AddTab (spec);
			
			TabHost.CurrentTab = 0;
		}
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main_action_bar, menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
				case Resource.Id.menuCreateGroup:
					var groupName = new EditText(this);
					new AlertDialog.Builder(this)
						.SetTitle ("New SMS Group")
						.SetMessage ("Please enter a name for the SMS group:")
						.SetView (groupName)
						.SetPositiveButton ("Ok", (o, e) => {
								var intent = new Intent();
								intent.SetClass (this, typeof(SMSGroupContactsActivity));
								intent.PutExtra("name", groupName.Text);
								StartActivity(intent);
							})
						.SetNegativeButton ("Cancel", (o, e) => { })
						.Show ();
					break;
				default:
					break;
			}
			
			return true;
		}
	}
}