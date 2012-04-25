using System;
using System.Linq;

using Prattle.Domain;

using SQLite;

namespace Prattle.Android.Core
{
	public class SmsGroupRepository: Repository<SmsGroup>, ISmsGroupRepository<SmsGroup>
	{
		public SmsGroupRepository ()
		{ }
		
		public SmsGroup GetByName (string name)
		{
			return (from smsGroup in cn.Table<SmsGroup>()
					where smsGroup.Name == name
					select smsGroup).FirstOrDefault();
		}
	}
}