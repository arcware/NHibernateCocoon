using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class InvoiceMap : ClassMap<Invoice>
	{
		public InvoiceMap()
		{
			Table("Invoice");
			Id(x => x.InvoiceId);
			Map(x => x.CustomerId);
			Map(x => x.InvoiceDate);
			Map(x => x.BillingAddress);
			Map(x => x.BillingCity);
			Map(x => x.BillingState);
			Map(x => x.BillingCountry);
			Map(x => x.BillingPostalCode);
			Map(x => x.Total);
		}
	}
}
