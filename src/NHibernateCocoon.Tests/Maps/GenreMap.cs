using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class GenreMap : ClassMap<Genre>
	{
		public GenreMap()
		{
			Table("Genre");
			Id(x => x.GenreId);
			Map(x => x.Name);
		}
	}
}
