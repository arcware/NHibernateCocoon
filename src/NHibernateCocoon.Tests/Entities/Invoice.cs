using System;

namespace NHibernateCocoon.Tests.Entities
{
	public class Invoice
	{
		public virtual int InvoiceId { get; set; }
		public virtual int CustomerId { get; set; }
		public virtual DateTime InvoiceDate { get; set; }
		public virtual string BillingAddress { get; set; }
		public virtual string BillingCity { get; set; }
		public virtual string BillingState { get; set; }
		public virtual string BillingCountry { get; set; }
		public virtual string BillingPostalCode { get; set; }
		public virtual decimal Total { get; set; }
	}
}
