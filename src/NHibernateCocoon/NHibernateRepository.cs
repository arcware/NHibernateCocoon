namespace NHibernateCocoon
{
	/// <summary>
	/// Provides many NHibernate data access methods; for use with NHibernate session.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	public class NHibernateRepository<T> : BaseNHibernateRepository<T> where T : class
	{
		/// <summary>
		/// Creates a new instance of NHibernateRepository.
		/// </summary>
		public NHibernateRepository()
		{
			Session = NHibernateSessionManager.Instance.GetSession();
		}
	}
}
