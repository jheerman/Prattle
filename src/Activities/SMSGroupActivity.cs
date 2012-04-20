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

namespace Prattle
{
	[Activity (Label = "SMSGroupActivity")]
	public class SMSGroupActivity : ListActivity
	{
		Repository<SMSGroup> _smsGroupRepo;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			_smsGroupRepo = new Repository<SMSGroup>();
			var smsGroups = _smsGroupRepo.GetAll ();
			
			var strGroups = smsGroups.Select (s => s.Name + " (" + s.MemberCount + " Members)").ToArray ();
			ListAdapter = new ArrayAdapter<string> (this, Resource.Layout.list_item, strGroups);
			ListView.TextFilterEnabled = true;
		}
	}
}