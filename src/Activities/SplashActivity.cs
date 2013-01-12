using Android.App;
using Android.Content;
using Android.OS;

namespace Prattle.Activities
{
	[Activity (MainLauncher=true, Theme="@style/Theme.Splash", NoHistory = true)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			// start the main activity
		    using (var intent = new Intent())
		    {
		        intent.AddFlags (ActivityFlags.SingleTop);
		        intent.SetClass (this, typeof(MainActivity));
		        StartActivity(intent);
		    }
		}
	}
}