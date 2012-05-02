using System;

namespace Prattle.Domain
{
	public interface ISmsGroupRepository <T> where T: ISmsGroup
	{
		T GetByName(string name);
	}
}