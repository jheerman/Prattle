using System;
using System.Collections.Generic;
using Prattle.Domain;

namespace Prattle.Android.Core
{
	public class SmsGroup : PrattleBase, ISmsGroup
	{
		public SmsGroup ()
		{ }
		public int MemberCount { get; set; }
		public string Name { get; set; }
		
		public override string ToString ()
		{
			return Name;
		}
	}
}