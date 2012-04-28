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

namespace Prattle
{
	public class SmsHistoryFragment: ListFragment
	{
		Repository<SmsMessage> _messageRepo;
		SmsGroupRepository _smsRepo;
		
		ActionMode _actionMode;
		bool _systemUIVisible = true;
		
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			_messageRepo = new Repository<SmsMessage>();
			_smsRepo = new SmsGroupRepository();
			
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
			var sortedItems = summary.OrderByDescending (message => message.DateSent).Take(20);
			ListAdapter = new MessageListAdapter(Activity, sortedItems.ToList());
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
			
			ListView.ItemLongClick += (sender, e) => {
				if (_actionMode != null)
					return;
				
				_actionMode = Activity.StartActionMode (new MessageAction(Activity));
			};
		}
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			return inflater.Inflate (Resource.Layout.SmsHistory, container, false);
		}
		
		private class MessageAction: Java.Lang.Object, ActionMode.ICallback
		{
			Activity _activity;
			
			public MessageAction (Activity activity)
			{
				_activity = activity;
			}
			
			bool ActionMode.ICallback.OnActionItemClicked (ActionMode mode, IMenuItem item)
			{
				switch (item.ItemId)
				{
					case Resource.Id.deleteMessage:
						mode.Finish ();
						break;
					case Resource.Id.viewMessage:
						mode.Finish ();
						break;
					default:
						break;
				}
				
				return true;
			}

			bool ActionMode.ICallback.OnCreateActionMode (ActionMode mode, IMenu menu)
			{
				mode.Title = _activity.GetString(Resource.String.message_action_title);
				_activity.MenuInflater.Inflate(Resource.Menu.message_action_bar, menu);
				return true;
			}

			void ActionMode.ICallback.OnDestroyActionMode (ActionMode mode)
			{ }

			bool ActionMode.ICallback.OnPrepareActionMode (ActionMode mode, IMenu menu)
			{
				return false;
			}
		}
	}
}