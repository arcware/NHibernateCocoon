using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class PlaylistTrackMap : ClassMap<PlaylistTrack>
	{
		public PlaylistTrackMap()
		{
			Table("PlaylistTrack");
			Id(x => x.PlaylistId);
			Map(x => x.TrackId);
		}
	}
}
