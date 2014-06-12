using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class MediaTypeMap : ClassMap<MediaType>
	{
		public MediaTypeMap()
		{
			Table("MediaType");
			Id(x => x.MediaTypeId);
			Map(x => x.Name);
		}
	}
}
