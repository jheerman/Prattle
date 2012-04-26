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
			var message = _messages[position];
			
			var view = convertView ??
				_context.LayoutInflater.Inflate(Resource.Layout.message_item, null);
			
			view.FindViewById<TextView>(Resource.Id.groupName).Text = message.SmsGroup.Name;
			view.FindViewById<TextView>(Resource.Id.recipientCount).Text = message.RecipientCount.ToString ();
			view.FindViewById<TextView>(Resource.Id.date).Text = message.DateSent.ToString ("MMM dd");
			view.FindViewById<TextView>(Resource.Id.message).Text = message.Text;
			return view;
		}
	}
}