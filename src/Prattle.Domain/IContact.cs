using System;

namespace Prattle.Domain
{
	public interface IContact
	{
		int Id { get;set; }
		string Name { get; set; }
	}
}