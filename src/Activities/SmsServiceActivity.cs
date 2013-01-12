using Android.App;
using Android.Content;
using Android.OS;

namespace Prattle.Activities
{
	public class SmsServiceActivity : Activity
	{
		private bool _isBound;
		private readonly IServiceConnection _connection;
		public PrattleSmsService BoundService;

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
		    if (!_isBound) return;
		    base.UnbindService (_connection);
		    _isBound = false;
		}
	}
	
	class PrattleSmsServiceConnection : Java.Lang.Object, IServiceConnection
	{
	    readonly SmsServiceActivity _serviceActivity;

		public PrattleSmsServiceConnection (SmsServiceActivity serviceActivity)
		{
			_serviceActivity = serviceActivity;
		}

		public void OnServiceConnected (ComponentName className, IBinder service)
		{
			_serviceActivity.BoundService = ((PrattleSmsService.LocalBinder) service).Service;
		}

		public void OnServiceDisconnected (ComponentName className)
		{
			_serviceActivity.BoundService = null;
		}
	}
}