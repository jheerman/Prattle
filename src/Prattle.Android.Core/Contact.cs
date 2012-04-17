using System;

using Prattle.Domain;

namespace Prattle.Android.Core
{
	public class Contact : PrattleBase, IContact
	{
		public Contact ()
		{ }
		public new string Id { get; set; }
		public string Phone { get; set; }
	}
}