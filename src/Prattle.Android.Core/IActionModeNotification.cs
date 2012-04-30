using System;

namespace Prattle.Android.Core
{
	public interface IActionModeNotification
	{
		event EventHandler <EventArgs> DeleteMessageHandler;
		event EventHandler <EventArgs> ViewMessageHandler;
	}
}