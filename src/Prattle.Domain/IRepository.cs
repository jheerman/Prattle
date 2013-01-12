using System.Collections.Generic;

namespace Prattle.Domain
{
	public interface IRepository <T> where T : class
	{
		int Save (T item);
		T Get(int id);
		IEnumerable<T> GetAll();
		void Delete (T item);
		void Truncate();
	}
}