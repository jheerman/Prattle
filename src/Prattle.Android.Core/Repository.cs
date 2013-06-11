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
		protected const string DbName = "db_prattle.db3";
		protected SQLiteConnection Cn;
		
		public Repository ()
		{
			//if the database table doesn't exist, create one
			var path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var db = Path.Combine (path, DbName);
			Cn = new SQLiteConnection (db);
			Cn.CreateTable<T>();
		}
		
		public int Save (T item)
		{
			return item.Id == 0
				? Cn.Insert (item)
				: Cn.Update (item);
		}
		
		public T Get (int id)
		{
			return (from item in Cn.Table<T> ()
				where item.Id == id
				select item).FirstOrDefault ();
		}
		
		public IEnumerable<T> GetAll ()
		{
			return (from item in Cn.Table<T> () select item);
		}
		
		public void Delete(T item)
		{
			Cn.Delete(item);
		}
		
		public void Truncate()
		{
			foreach (var item in Cn.Table<T>())
				Cn.Delete(item);
		}
	}
}

