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
using System.Threading.Tasks;

namespace Prattle
{
	[Activity (Label = "SMSGroupActivity")]
	public class SMSGroupActivity : ListActivity
	{
		Repository<SMSGroup> _smsGroupRepo;
		ContactRepository _contactRepo;
		ProgressDialog _progressDialog;
		
		List<SMSGroup> _smsGroups;
		int _position = -1;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			_smsGroupRepo = new Repository<SMSGroup>();
			_smsGroups = _smsGroupRepo.GetAll ().ToList ();
			
			var strGroups = _smsGroups.Select (s => s.Name + " (" + s.MemberCount + " Members)").ToArray ();
			ListAdapter = new ArrayAdapter<string> (this, Resource.Layout.list_item, strGroups);
			ListView.TextFilterEnabled = true;
			ListView.ItemClick += delegate(object sender, ItemEventArgs e) {
				var editSMSIntent = new Intent();
				editSMSIntent.PutExtra ("groupId", _smsGroups[e.Position].Id);
				editSMSIntent.SetClass (this, typeof(EditSMSGroupActivity));
				StartActivity (editSMSIntent);
			};
			
			RegisterForContextMenu (ListView);
		}
		
		public override void OnCreateContextMenu (IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			base.OnCreateContextMenu (menu, v, menuInfo);
			
			//capture the selected item's position in the list
			_position = ((AdapterView.AdapterContextMenuInfo) menuInfo).Position;
			
			MenuInflater.Inflate (Resource.Menu.sms_context_menu, menu);
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
					editSMSIntent.SetClass (this, typeof(EditSMSGroupActivity));
					StartActivity (editSMSIntent);
					break;
				case Resource.Id.sendSMS:
					break;
				case Resource.Id.deleteSMS:
					new AlertDialog.Builder(this)
						.SetTitle ("Delete SMS Group")
						.SetMessage (string.Format ("Are you sure you want to delete the following group: {0}",
						                            _smsGroups[_position]))
						.SetPositiveButton ("Ok", (o, e) => {
							_progressDialog = new ProgressDialog(this);
							_progressDialog.SetTitle ("Delete SMS Group");
							_progressDialog.SetMessage ("Please wait.  Deleting SMS Group...");
							_progressDialog.Show ();
							
							Task.Factory
								.StartNew(() =>
									DeleteGroup(_smsGroups[_position]))
								.ContinueWith(task =>
									RunOnUiThread(() => {
										UpdateSMSList();
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
		
		private void DeleteGroup (SMSGroup smsGroup)
		{
			//Delete all group memebers
			_contactRepo = new ContactRepository(this);
			var contacts = _contactRepo.GetMembersForSMSGroup(smsGroup.Id);
			contacts.ForEach (c => _contactRepo.Delete (c));
			
			_smsGroupRepo.Delete (smsGroup);
		}
		
		private void UpdateSMSList()
		{
			_smsGroups.RemoveAt (_position);
			var strGroups = _smsGroups.Select (s => s.Name + " (" + s.MemberCount + " Members)").ToArray ();
			ListAdapter = new ArrayAdapter<string> (this, Resource.Layout.list_item, strGroups);
			((BaseAdapter)ListAdapter).NotifyDataSetChanged ();
		}
	}
}