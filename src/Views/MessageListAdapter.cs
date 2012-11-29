using System;
using System.Collections.Generic;

using Prattle.Android.Core;

using Android.Widget;
using Android.App;
using Android.Views;

namespace Prattle
{
	public class MessageListAdapter : BaseAdapter
	{
		private Activity _context;
		private List<MessageListItem> _messages;

		public MessageListAdapter (Activity context, List<MessageListItem> messages) : base ()
		{
			_context = context;
			_messages = messages;
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return position;
		}

		public MessageListItem GetMessage (int position)
		{
			return _messages[position];
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override int Count 
		{
			get { return _messages.Count; }
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var charLimit = 50;
			
			switch (_context.WindowManager.DefaultDisplay.Orientation)
			{
				case (int)Orientation.Horizontal:
					charLimit = (_context.WindowManager.DefaultDisplay.Width / 8);
					break;
				case (int)Orientation.Vertical:
					charLimit = (_context.WindowManager.DefaultDisplay.Width / 9);
					break;
			}
 			
			var message = _messages[position];
			
			var view = convertView ??
				_context.LayoutInflater.Inflate(Resource.Layout.message_item, null);
			
			view.FindViewById<TextView>(Resource.Id.groupName).Text = message.SmsGroup.Name;
			view.FindViewById<TextView>(Resource.Id.recipientCount).Text = string.Format ("{0} recipients", message.RecipientCount);
			view.FindViewById <TextView>(Resource.Id.dayOfMonth).Text = message.DateSent.Day.ToString ();
			view.FindViewById<TextView>(Resource.Id.month).Text = message.DateSent.ToString ("MMM");
			
			view.FindViewById<TextView>(Resource.Id.message).Text = message.Text.Length > charLimit ? 
				String.Concat (message.Text.Substring (0, charLimit), "...") :
				message.Text;
			return view;
		}
	}
}