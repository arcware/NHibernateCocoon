using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class TrackMap : ClassMap<Track>
	{
		public TrackMap()
		{
			Table("Track");
			Id(x => x.TrackId);
			Map(x => x.Name);
			Map(x => x.AlbumId);
			Map(x => x.MediaTypeId);
			Map(x => x.GenreId);
			Map(x => x.Composer);
			Map(x => x.Milliseconds);
			Map(x => x.Bytes);
			Map(x => x.UnitPrice);
		}
	}
}
