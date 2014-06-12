using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class ArtistMap : ClassMap<Artist>
	{
		public ArtistMap()
		{
			Table("Artist");
			Id(x => x.ArtistId);
			Map(x => x.Name);
		}
	}
}
