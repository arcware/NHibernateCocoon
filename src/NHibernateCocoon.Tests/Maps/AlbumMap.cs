using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class AlbumMap : ClassMap<Album>
	{
		public AlbumMap()
		{
			Table("Album");
			Id(x => x.AlbumId);
			Map(x => x.Title);
			Map(x => x.ArtistId);
		}
	}
}
