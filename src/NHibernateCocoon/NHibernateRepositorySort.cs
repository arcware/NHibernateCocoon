namespace NHibernateCocoon
{
	/// <summary>
	/// A sort order, based on a property and its sort direction.
	/// </summary>
	public class NHibernateRepositorySort
	{
		/// <summary>
		/// The property which to sort on.
		/// </summary>
		public string Property { get; set; }

		/// <summary>
		/// The sort direction for the property.
		/// </summary>
		public NHibernateRepositorySortDirection SortDirection { get; set; }

		/// <summary>
		/// Creates a new NHibernateRepositorySort class.
		/// </summary>
		/// <param name="property">The property which to sort on.</param>
		/// <param name="sortDirection">The sort direction.</param>
		public NHibernateRepositorySort(string property, NHibernateRepositorySortDirection sortDirection)
        {
            Property = property;
			SortDirection = sortDirection;
        }
	}
}
