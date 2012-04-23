using System;

namespace Prattle.Domain
{
	public interface ISMSGroupRepository <T> where T: ISMSGroup
	{
		T GetByName(string name);
	}
}