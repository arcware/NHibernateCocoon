using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class CustomerMap : ClassMap<Customer>
	{
		public CustomerMap()
		{
			Table("Customer");
			Id(x => x.CustomerId);
			Map(x => x.FirstName);
			Map(x => x.LastName);
			Map(x => x.Company);
			Map(x => x.Address);
			Map(x => x.City);
			Map(x => x.State);
			Map(x => x.Country);
			Map(x => x.PostalCode);
			Map(x => x.Phone);
			Map(x => x.Fax);
			Map(x => x.Email);
			Map(x => x.SupportRepId);
		}
	}
}
