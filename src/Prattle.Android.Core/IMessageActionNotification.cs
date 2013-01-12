using System;

namespace Prattle.Android.Core
{
	public interface IMessageActionNotification
	{
		event EventHandler <EventArgs> DeleteActionHandler;
		event EventHandler <EventArgs> ViewActionHandler;
		event EventHandler <EventArgs> DestroyActionHandler;
	}
}