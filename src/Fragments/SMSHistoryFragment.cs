using System;
using System.Linq;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Prattle.Android.Core;

namespace Prattle
{
	public class SmsHistoryFragment: ListFragment
	{
		Repository<SmsMessage> _messageRepo;
		SmsGroupRepository _smsRepo;
		
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			_messageRepo = new Repository<SmsMessage>();
			_smsRepo = new SmsGroupRepository();
			
			var messages = _messageRepo.GetAll ();
			var smsGroups = _smsRepo.GetAll ();
			
			//join messages to groups to get the linked smsGroup
			var items = from message in messages
						join smsGroup in smsGroups
						on message.SMSGroupId equals smsGroup.Id
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
			
			ListAdapter = new MessageListAdapter(Activity, summary.ToList ());
		}
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			return inflater.Inflate (Resource.Layout.SmsHistory, container, false);
		}
	}
}