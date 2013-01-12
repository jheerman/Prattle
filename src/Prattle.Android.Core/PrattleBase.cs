using System;
using SQLite;

namespace Prattle.Android.Core
{
	public class PrattleBase
	{
	    [PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string UUID { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }
	}
}