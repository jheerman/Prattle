namespace Prattle.Domain
{
	public interface ISmsGroupRepository <out T> where T: ISmsGroup
	{
		T GetByName(string name);
	}
}