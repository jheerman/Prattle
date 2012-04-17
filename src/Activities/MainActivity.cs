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
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);
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