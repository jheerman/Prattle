using System.Linq;

using Prattle.Domain;

namespace Prattle.Android.Core
{
	public class SmsGroupRepository: Repository<SmsGroup>, ISmsGroupRepository<SmsGroup>
	{
	    public SmsGroup GetByName (string name)
		{
			return (from smsGroup in Cn.Table<SmsGroup>()
					where smsGroup.Name == name
					select smsGroup).FirstOrDefault();
		}
	}
}