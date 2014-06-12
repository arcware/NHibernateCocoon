using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class PlaylistMap : ClassMap<Playlist>
	{
		public PlaylistMap()
		{
			Table("Playlist");
			Id(x => x.PlaylistId);
			Map(x => x.Name);
		}
	}
}
