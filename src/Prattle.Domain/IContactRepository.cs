using System;
using System.Collections.Generic;

namespace Prattle.Domain
{
	public interface IContactRepository<T> where T: IContact
	{
		T GetByAddressBookId (string addressId);
		List<T> GetAll();
		List<T> GetAllMobile();
		List<T> GetMembersForSMSGroup(int groupId);
	}
}