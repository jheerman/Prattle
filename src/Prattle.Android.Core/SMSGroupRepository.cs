using System;
using System.Linq;

using Prattle.Domain;

using SQLite;

namespace Prattle.Android.Core
{
	public class SMSGroupRepository: Repository<SMSGroup>, ISMSGroupRepository<SMSGroup>
	{
		public SMSGroupRepository ()
		{ }
		
		public SMSGroup GetByName (string name)
		{
			return (from smsGroup in cn.Table<SMSGroup>()
					where smsGroup.Name == name
					select smsGroup).FirstOrDefault();
		}
	}
}