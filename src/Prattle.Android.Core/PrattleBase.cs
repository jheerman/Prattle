using System;
using SQLite;

namespace Prattle.Android.Core
{
	public class PrattleBase
	{
		public PrattleBase ()
		{ }
		
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Name { get; set; }
		public string UUID { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }
	}
}