namespace NHibernateCocoon
{
	/// <summary>
    /// Holds the configuration information for NHibernateCocoon.
	/// </summary>
	public class Config
	{
		/// <summary>
		/// The name of the assembly that contains the fluent mappings.
		/// </summary>
		public string FluentMappingAssemblyName { get; set; }

		/// <summary>
		/// The transaction scope for NHibernate, either "batch" or "statement".
		/// </summary>
		public string TransactionScope { get; set; }
	}
}
