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
		
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			_messageRepo = new Repository<SmsMessage>();
			var messages = _messageRepo.GetAll ().ToList ();
			if (messages == null || messages.Count == 0) return;
			
			var strMessages = messages.Select (message => 
			                                                   string.Format ("{0} - {1}", 
			               message.ContactAddressBookId, 
			               message.Text.Substring (0, message.Text.Length < 20 ? message.Text.Length : 20))).ToArray ();
			ListAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.list_item, strMessages);
		}
		
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			return inflater.Inflate (Resource.Layout.SmsHistory, container, false);
		}
	}
}