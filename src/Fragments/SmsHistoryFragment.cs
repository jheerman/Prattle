using System;
using System.Linq;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Java.IO;

using Prattle.Android.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Prattle
{
	public class SmsHistoryFragment: ListFragment
	{
		SmsMessageRepository _messageRepo;
		SmsGroupRepository _smsRepo;
		List<MessageListItem> _sortedItems;
		ProgressDialog _progressDialog;
		
		private ActionMode _actionMode;
		bool _systemUIVisible = true;
		
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			_messageRepo = new SmsMessageRepository();
			_smsRepo = new SmsGroupRepository();
			
			_sortedItems = GetGroupedMessages (20);
			ListAdapter = new MessageListAdapter(Activity, _sortedItems);
		}
		
		public override void OnStart ()
		{
			base.OnStart ();
		}
		
		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			
			ListView.ItemClick += (sender, e) => {
				//already in action mode?
				if (_actionMode != null)
					return;
				
				//toggle system ui visibility
				if (_systemUIVisible)
				{
					Activity.Window.ClearFlags (WindowManagerFlags.Fullscreen);
					ListView.SystemUiVisibility = StatusBarVisibility.Visible;
					Activity.ActionBar.Show ();
				}
				else
				{
					Activity.Window.SetFlags (0, WindowManagerFlags.Fullscreen);
					ListView.SystemUiVisibility = StatusBarVisibility.Hidden;
					Activity.ActionBar.Hide ();
				}
				_systemUIVisible = !_systemUIVisible;
			};
			
			ListView.ItemLongClick += delegate(object sender, AdapterView.ItemLongClickEventArgs e) {
				if (_actionMode != null)
					return;
				
				var callback = new MessageAction(Activity.GetString(Resource.String.message_action_title),
				                                 Activity.GetString(Resource.String.message_action_subtitle));
				
				callback.DeleteActionHandler += delegate {
					DeleteMessage (_sortedItems[e.Position]);
					_actionMode.Finish ();
					_actionMode = null;
				};
				
				callback.ViewActionHandler += delegate {
					ViewMessage(_sortedItems[e.Position]);
					_actionMode.Finish ();
					_actionMode = null;
				};
				
				callback.CopyActionHandler += delegate {
					CopyMessage (_sortedItems[e.Position]);
					_actionMode.Finish ();
					_actionMode = null;
				};
				
				callback.DestroyActionHandler += delegate {
					_actionMode = null;
				};
				
				_actionMode = Activity.StartActionMode (callback);
			};
		}
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			return inflater.Inflate (Resource.Layout.SmsHistory, container, false);
		}
		
		private void CopyMessage(MessageListItem selectedMessage)
		{
			var clipboard = (ClipboardManager)Activity.GetSystemService (Context.ClipboardService);
			clipboard.PrimaryClip = ClipData.NewPlainText ("message", selectedMessage.Text);
			Toast.MakeText (Activity, "Message text copied to clipboard", ToastLength.Short).Show ();
		}
		
		private void ViewMessage(MessageListItem selectedMessage)
		{
//			_progressDialog = new ProgressDialog(Activity);
//			_progressDialog.SetTitle ("Getting Message Details");
//			_progressDialog.SetMessage ("Please Wait.  Retrieving Message Details...");
//			_progressDialog.Show ();
			
			Task.Factory
				.StartNew(() => {
					var items = _messageRepo.GetAllForEvent (selectedMessage.SmsGroup.Id, selectedMessage.DateSent, selectedMessage.Text);
					List<string> recipients = new List<string>();
					items.ForEach (message => {
						recipients.Add(message.ContactName);
					});
					return recipients;
				})
				.ContinueWith(task =>
					Activity.RunOnUiThread(() => {
						DisplayDetail(task.Result, selectedMessage);
//						_progressDialog.Dismiss ();
					}));
		}
		
		private void DisplayDetail(List<string> recipients, MessageListItem selectedMessage)
		{
			var contactNames = string.Join (", ", recipients.ToArray());
			
			new AlertDialog.Builder(Activity)
				.SetTitle ("Message Details")
				.SetMessage (string.Format ("Sent To: {0} at {1} on {2}",
		                            contactNames, selectedMessage.DateSent.ToShortTimeString(), selectedMessage.DateSent.ToShortDateString()))
				.Show ();
		}
		
		private List<MessageListItem> GetGroupedMessages (int rowCount)
		{
			//TODO: Join messages and groups in the message query and eliminate the need to select all groups
			var messages = _messageRepo.GetAll ();
			var smsGroups = _smsRepo.GetAll ();
			
			//join messages to groups to get the sms group name
			var items = from message in messages
						join smsGroup in smsGroups
						on message.SmsGroupId equals smsGroup.Id
						select new 
						{
							SmsGroup = smsGroup,
							Text = message.Text,
							DateSent = message.SentDate
						};
			
			//group messages
			var summary = from item in items
							group item by new { item.SmsGroup, item.DateSent, item.Text } into g
							select new MessageListItem
							{
								SmsGroup = g.Key.SmsGroup,
								DateSent = g.Key.DateSent,
								Text = g.Key.Text,
								RecipientCount = g.Count()
							};
			
			//sort the summaries and display the top 20
			return summary.OrderByDescending (message => message.DateSent).Take(rowCount).ToList();
		}
		
		private void DeleteMessage(MessageListItem selectedMessage)
		{
			_progressDialog = new ProgressDialog(Activity);
			_progressDialog.SetTitle ("Delete Message");
			_progressDialog.SetMessage (string.Format ("Deleting Message with {0} recipients.  Please wait...", selectedMessage.RecipientCount));
			_progressDialog.Show ();
			
			Task.Factory
				.StartNew(() => {
					var messages = _messageRepo.GetAllForEvent (selectedMessage.SmsGroup.Id, selectedMessage.DateSent, selectedMessage.Text);
					messages.ForEach (message => _messageRepo.Delete (message));
				})
				.ContinueWith(task =>
					Activity.RunOnUiThread(() => {
						_sortedItems = GetGroupedMessages (20);
						ListAdapter = new MessageListAdapter(Activity, _sortedItems);
						((BaseAdapter)ListAdapter).NotifyDataSetChanged ();
						_progressDialog.Dismiss ();
				}));
		}
		
		private class MessageAction: Java.Lang.Object, ActionMode.ICallback, IMessageActionNotification
		{
			string _title;
			string _subTitle;
			
			public event EventHandler<EventArgs> DeleteActionHandler;
			public event EventHandler<EventArgs> ViewActionHandler;
			public event EventHandler<EventArgs> CopyActionHandler;
			public event EventHandler<EventArgs> DestroyActionHandler;
			
			public MessageAction (string title, string subTitle)
			{
				_title = title;
				_subTitle = subTitle;
			}
			
			bool ActionMode.ICallback.OnActionItemClicked (ActionMode mode, IMenuItem item)
			{
				switch (item.ItemId)
				{
					case Resource.Id.deleteMessage:
						if (DeleteActionHandler != null)
							DeleteActionHandler (null, null);
						return true;
					case Resource.Id.viewMessage:
						if (ViewActionHandler != null)
							ViewActionHandler(null, null);
						return true;
					case Resource.Id.copyMessage:
						if (CopyActionHandler != null)
							CopyActionHandler (null, null);
						return true;
					default:
						return false;
				}
			}
	
			bool ActionMode.ICallback.OnCreateActionMode (ActionMode mode, IMenu menu)
			{
				mode.Title = _title;
				mode.Subtitle = _subTitle;
				mode.MenuInflater.Inflate (Resource.Menu.message_action_bar, menu);
				return true;
			}
	
			void ActionMode.ICallback.OnDestroyActionMode (ActionMode mode)
			{
				if (DestroyActionHandler != null)
					DestroyActionHandler(null, null);
			}
	
			bool ActionMode.ICallback.OnPrepareActionMode (ActionMode mode, IMenu menu)
			{
				return false;
			}
		}
	}
}