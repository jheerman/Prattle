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

namespace Prattle
{
	public class SmsServiceActivity : Activity
	{
		private bool _isBound;
		private IServiceConnection _connection;
		public PrattleSmsService boundService;

		public SmsServiceActivity ()
		{
			_connection = new PrattleSmsServiceConnection (this);
		}
	
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		}
		
		protected void BindService ()
		{
			base.BindService (new Intent (ApplicationContext, typeof (PrattleSmsService)),
						_connection, Bind.AutoCreate);
			_isBound = true;
		}

		protected void UnbindService ()
		{
			if (_isBound) 
			{
				base.UnbindService (_connection);
				_isBound = false;
			}
		}
	}
	
	class PrattleSmsServiceConnection : Java.Lang.Object, IServiceConnection
	{
		SmsServiceActivity _serviceActivity;

		public PrattleSmsServiceConnection (SmsServiceActivity serviceActivity)
		{
			_serviceActivity = serviceActivity;
		}

		public void OnServiceConnected (ComponentName className, IBinder service)
		{
			_serviceActivity.boundService = ((PrattleSmsService.LocalBinder) service).Service;
		}

		public void OnServiceDisconnected (ComponentName className)
		{
			_serviceActivity.boundService = null;
		}
	}
}