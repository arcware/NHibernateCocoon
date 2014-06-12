using FluentNHibernate.Mapping;
using NHibernateCocoon.Tests.Entities;

namespace NHibernateCocoon.Tests.Maps
{
	public class EmployeeMap : ClassMap<Employee>
	{
		public EmployeeMap()
		{
			Table("Employee");
			Id(x => x.EmployeeId);
			Map(x => x.LastName);
			Map(x => x.FirstName);
			Map(x => x.Title);
			Map(x => x.ReportsTo);
			Map(x => x.BirthDate);
			Map(x => x.HireDate);
			Map(x => x.Address);
			Map(x => x.City);
			Map(x => x.State);
			Map(x => x.Country);
			Map(x => x.PostalCode);
			Map(x => x.Phone);
			Map(x => x.Fax);
			Map(x => x.Email);
		}
	}
}
