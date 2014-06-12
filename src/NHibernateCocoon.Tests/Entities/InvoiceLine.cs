namespace NHibernateCocoon.Tests.Entities
{
	public class InvoiceLine
	{
		public virtual int InvoiceLineId { get; set; }
		public virtual int InvoiceId { get; set; }
		public virtual int TrackId { get; set; }
		public virtual decimal UnitPrice { get; set; }
		public virtual int Quantity { get; set; }
	}
}
