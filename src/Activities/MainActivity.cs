using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Views.InputMethods;

namespace Prattle
{
	[Activity (Theme="@style/Theme.ActionLight")]
	public class MainActivity : BaseActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);
			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			
			//Add SMS History tab
			var tab = ActionBar.NewTab ();
			tab.SetText ("Messages");
			tab.SetIcon (Resource.Drawable.ic_tab_sms_history);
			
			// must set event handler before adding tab
			tab.TabSelected += delegate(object sender, ActionBar.TabEventArgs e) {
				e.FragmentTransaction.Add (Resource.Id.fragmentContainer, new SmsHistoryFragment ());
			};
			
			ActionBar.AddTab (tab);
			
			//Add SMS Group tab
			tab = ActionBar.NewTab ();
			tab.SetText ("Groups");
			tab.SetIcon (Resource.Drawable.ic_tab_sms_group);
			
			// must set event handler before adding tab
			tab.TabSelected += delegate(object sender, ActionBar.TabEventArgs e) {
				e.FragmentTransaction.Add (Resource.Id.fragmentContainer, new SmsGroupFragment ());
			};
			
			ActionBar.AddTab (tab);
			
			var defaultTab = Intent.GetIntExtra ("defaultTab", 0);
			ActionBar.SetSelectedNavigationItem(defaultTab);
		}
		
		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			UnbindService ();
			StopService (new Intent(this, typeof(PrattleSmsService)));
		}
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main_action_bar, menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
				case Resource.Id.sendSMS:
					break;
				case Resource.Id.menuCreateGroup:
					var groupName = new EditText(this);
					new AlertDialog.Builder(this)
						.SetTitle ("New SMS Group")
						.SetMessage ("Please enter a name for the SMS group:")
						.SetView (groupName)
						.SetPositiveButton ("Ok", (o, e) => {
								var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
								imm.HideSoftInputFromWindow (groupName.WindowToken, HideSoftInputFlags.None);
								var intent = new Intent();
								intent.SetClass(this, typeof(NewSmsGroupActivity));
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