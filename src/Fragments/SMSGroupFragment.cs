using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using G = Android.Graphics;

using Prattle.Android.Core;

namespace Prattle
{
	public class SMSGroupFragment: ListFragment
	{
		Repository<SMSGroup> _smsGroupRepo;
		ContactRepository _contactRepo;
		ProgressDialog _progressDialog;
		
		List<SMSGroup> _smsGroups;
		int _position = -1;
		
		public override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			_smsGroupRepo = new Repository<SMSGroup>();
			_smsGroups = _smsGroupRepo.GetAll ().ToList ();
			
			var strGroups = _smsGroups.Select (s => s.Name + " (" + s.MemberCount + " Members)").ToArray ();
			ListAdapter = new ArrayAdapter<string> (Activity, Resource.Layout.list_item, strGroups);
			//ListView.SetSelector(Resource.Drawable.group_list_selector);
		}
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			return inflater.Inflate (Resource.Layout.SMSGroup, container, false);
		}
		
		public override void OnActivityCreated (Bundle bundle)
		{
			base.OnActivityCreated (bundle);
			RegisterForContextMenu (ListView);
		}
		
		public override void OnListItemClick (ListView listView, View view, int position, long id)
		{
			var editSMSIntent = new Intent();
			editSMSIntent.PutExtra ("groupId", _smsGroups[position].Id);
			editSMSIntent.SetClass (Activity, typeof(EditSMSGroupActivity));
			StartActivity (editSMSIntent);
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
			switch (item.ItemId)
			{
				case Resource.Id.editSMS:
					var editSMSIntent = new Intent();
					editSMSIntent.PutExtra ("groupId", _smsGroups[_position].Id);
					editSMSIntent.SetClass (Activity, typeof(EditSMSGroupActivity));
					StartActivity (editSMSIntent);
					break;
				case Resource.Id.sendSMS:
					var sendMessage = new Intent();
					sendMessage.PutExtra ("groupId", _smsGroups[_position].Id);
					sendMessage.SetClass (Activity, typeof(SendMessageActivity));
					StartActivity (sendMessage);
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
									//Delete all group memebers then delete sms group
									_contactRepo = new ContactRepository(Activity);
									var contacts = _contactRepo.GetMembersForSMSGroup(smsGroup.Id);
									contacts.ForEach (c => _contactRepo.Delete (c));
									_smsGroupRepo.Delete (smsGroup);
								})
								.ContinueWith(task =>
									Activity.RunOnUiThread(() => {
										_smsGroups.RemoveAt (_position);
										var strGroups = _smsGroups
											.Select (s => s.Name + " (" + s.MemberCount + " Members)").ToArray ();
										ListAdapter = new ArrayAdapter<string> (Activity, Resource.Layout.list_item, strGroups);
										((BaseAdapter)ListAdapter).NotifyDataSetChanged ();
										_progressDialog.Dismiss ();
									}));
						})
						.SetNegativeButton ("Cancel", (o, e) => { })
						.Show ();
					break;
				default:
					break;
			}
			
			return base.OnContextItemSelected (item);
		}
	}
}