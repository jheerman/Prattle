using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Prattle.Activities;
using Prattle.Android.Core;
using Prattle.Views;

namespace Prattle.Fragments
{
	public class SmsGroupFragment: ListFragment
	{
		Repository<SmsGroup> _smsGroupRepo;
		ContactRepository _contactRepo;
		ProgressDialog _progressDialog;
		
		List<SmsGroup> _smsGroups;
		int _position = -1;
		
		public override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			_smsGroupRepo = new Repository<SmsGroup>();
			_smsGroups = _smsGroupRepo.GetAll ().ToList ();
			
			ListAdapter = new GroupListAdapter(Activity, _smsGroups);
		}
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			return inflater.Inflate (Resource.Layout.SmsGroup, container, false);
		}
		
		public override void OnActivityCreated (Bundle bundle)
		{
			base.OnActivityCreated (bundle);
			RegisterForContextMenu (ListView);
		}
		
		public override void OnListItemClick (ListView listView, View view, int position, long id)
		{
			var editSmsIntent = new Intent();
			editSmsIntent.PutExtra ("groupId", _smsGroups[position].Id);
			editSmsIntent.SetClass (Activity, typeof(EditSmsGroupActivity));
			StartActivity (editSmsIntent);
		}
		
		public override void OnCreateContextMenu (IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			base.OnCreateContextMenu (menu, v, menuInfo);
			
			//capture the selected item's position in the list
			_position = ((AdapterView.AdapterContextMenuInfo) menuInfo).Position;
			
			Activity.MenuInflater.Inflate (Resource.Menu.sms_context_menu, menu);
			menu.SetHeaderTitle ("SMS Group Options");
			menu.SetHeaderIcon (Resource.Drawable.ic_menu_sms_context_header);
		}
		
		public override bool OnContextItemSelected (IMenuItem item)
		{
			if (_position < 0) return false;
			
			switch (item.ItemId)
			{
				case Resource.Id.editSMS:
			        using (var editSmsIntent = new Intent())
			        {
			            editSmsIntent.PutExtra ("groupId", _smsGroups[_position].Id);
			            editSmsIntent.SetClass (Activity, typeof(EditSmsGroupActivity));
			            StartActivity (editSmsIntent);
			        }
			        break;
				case Resource.Id.sendSMS:
			        using (var sendMessage = new Intent())
			        {
			            sendMessage.PutExtra ("groupId", _smsGroups[_position].Id);
			            sendMessage.SetClass (Activity, typeof(SendMessageActivity));
			            StartActivity (sendMessage);
			        }
			        break;
				case Resource.Id.deleteSMS:
					new AlertDialog.Builder(Activity)
						.SetTitle ("Delete SMS Group")
						.SetMessage (string.Format ("Are you sure you want to delete the following group: {0}",
						                            _smsGroups[_position]))
						.SetPositiveButton ("Ok", (o, e) => {
							_progressDialog = new ProgressDialog(Activity);
							_progressDialog.SetTitle ("Delete SMS Group");
							_progressDialog.SetMessage ("Please wait.  Deleting SMS Group...");
							_progressDialog.Show ();
							
							Task.Factory
								.StartNew(() => {
									var smsGroup = _smsGroups[_position];
									
									//Delete all messages, group memebers, and then delete sms group
									var messageRepo = new SmsMessageRepository();
									var messages = messageRepo.GetAllForGroup (smsGroup.Id);
									messages.ForEach (messageRepo.Delete);
									
									_contactRepo = new ContactRepository(Activity);
									var contacts = _contactRepo.GetMembersForSmsGroup(smsGroup.Id);
									contacts.ForEach (c => _contactRepo.Delete (c));
									
									_smsGroupRepo.Delete (smsGroup);
								})
								.ContinueWith(task =>
									Activity.RunOnUiThread(() => {
										_smsGroups.RemoveAt (_position);
										ListAdapter = new GroupListAdapter(Activity, _smsGroups);
										((BaseAdapter)ListAdapter).NotifyDataSetChanged ();
										_progressDialog.Dismiss ();
									}));
						})
						.SetNegativeButton ("Cancel", (o, e) => { })
						.Show ();
					break;
			}
			
			return base.OnContextItemSelected (item);
		}
	}
}