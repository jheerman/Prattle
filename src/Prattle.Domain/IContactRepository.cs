using System;
using System.Collections.Generic;

namespace Prattle.Domain
{
	public interface IContactRepository<T> where T: IContact
	{
		T Get(string id);
		List<T> GetAll();
		List<T> GetAllMobile();
	}
}