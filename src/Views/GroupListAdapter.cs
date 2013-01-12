using System.Collections.Generic;
using System.Globalization;
using Prattle.Android.Core;
using Android.Widget;
using Android.App;
using Android.Views;

namespace Prattle.Views
{
	public class GroupListAdapter : BaseAdapter
	{
		private readonly Activity _context;
		private readonly List<SmsGroup> _groups;

		public GroupListAdapter (Activity context, List<SmsGroup> groups)
		{
			_context = context;
			_groups = groups;
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return position;
		}

		public SmsGroup GetGroup (int position)
		{
			return _groups[position];
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override int Count 
		{
			get { return _groups.Count; }
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var group = _groups[position];
			
			var view = convertView ??
				_context.LayoutInflater.Inflate(Resource.Layout.group_item, null);
			
			view.FindViewById<TextView>(Resource.Id.groupName).Text = group.Name;
			view.FindViewById<TextView>(Resource.Id.recipientCount).Text = group.MemberCount.ToString (CultureInfo.InvariantCulture);
			return view;
		}
	}
}