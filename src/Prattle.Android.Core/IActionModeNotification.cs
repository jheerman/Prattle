using System;

namespace Prattle.Android.Core
{
	public interface IActionModeNotification
	{
		event EventHandler <EventArgs> MessageProcessed;
	}
}