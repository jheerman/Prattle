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
	public class BaseActivity : Activity
	{
		private bool _isBound;
		private IServiceConnection _connection;
		public PrattleSmsService boundService;

		public BaseActivity ()
		{
			_connection = new PrattleSmsServiceConnection (this);
		}
	
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		}
		
		protected void BindService ()
		{
			base.BindService (new Intent (this, typeof (PrattleSmsService)),
						_connection, Bind.AutoCreate);
			_isBound = true;
		}

		protected void UnbindService ()
		{
			if (_isBound) {
				base.UnbindService (_connection);
				_isBound = false;
			}
		}
	}
	
	class PrattleSmsServiceConnection : Java.Lang.Object, IServiceConnection
	{
		BaseActivity _self;

		public PrattleSmsServiceConnection (BaseActivity self)
		{
			_self = self;
		}

		public void OnServiceConnected (ComponentName className, IBinder service)
		{
			_self.boundService = ((PrattleSmsService.LocalBinder) service).Service;
			Toast.MakeText (_self, "connected", ToastLength.Short).Show ();
		}

		public void OnServiceDisconnected (ComponentName className)
		{
			_self.boundService = null;
			Toast.MakeText (_self, "disconnected", ToastLength.Short).Show ();
		}
	}
}