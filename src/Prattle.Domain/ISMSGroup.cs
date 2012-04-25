using System;
using System.Collections.Generic;

namespace Prattle.Domain
{
	public interface ISmsGroup
	{
		int Id { get; set; }
		string Name { get; set; }
		int MemberCount { get; set; }
	}
}