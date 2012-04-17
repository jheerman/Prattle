using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using SQLite;
using Prattle.Domain;

namespace Prattle.Android.Core
{
	public class Repository<T> : IRepository<T> where T: PrattleBase, new()
	{
		protected const string dbName = "db_prattle.db3";
		protected SQLiteConnection cn;
		
		public Repository ()
		{
			//if the database table doesn't exist, create one
			var path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			cn = new SQLiteConnection(Path.Combine (path, dbName));
			cn.CreateTable<T>();
		}
		
		public int Save (T item)
		{
			return item.Id == 0
				? cn.Insert (item)
				: cn.Update (item);
		}
		
		public T Get (int id)
		{
			return (from item in cn.Table<T> ()
				where item.Id == id
				select item).FirstOrDefault ();
		}
		
		public IEnumerable<T> GetAll ()
		{
			return (from item in cn.Table<T> () select item);
		}
		
		public void Delete(T item)
		{
			cn.Delete<T>(item);
		}
		
		public void Truncate()
		{
			foreach (var item in cn.Table<T>())
				cn.Delete<T>(item);
		}
	}
}

