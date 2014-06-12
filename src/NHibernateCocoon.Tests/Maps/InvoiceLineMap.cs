using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class InvoiceLineMap : ClassMap<InvoiceLine>
	{
		public InvoiceLineMap()
		{
			Table("InvoiceLine");
			Id(x => x.InvoiceLineId);
			Map(x => x.InvoiceId);
			Map(x => x.TrackId);
			Map(x => x.UnitPrice);
			Map(x => x.Quantity);
		}
	}
}
