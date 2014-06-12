using NHibernate;

namespace NHibernateCocoon
{
	/// <summary>
	/// Provides many NHibernate data access methods; for use with stateless NHibernate session.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	public class NHibernateStatelessRepository<T> : BaseNHibernateRepository<T> where T : class
	{
		/// <summary>
		/// The stateless NHibernate session.
		/// </summary>
		public new IStatelessSession Session
		{
			get { return StatelessSession; }
		}

		/// <summary>
		/// Creates a new instance of NHibernateStatelessRepository.
		/// </summary>
		public NHibernateStatelessRepository()
		{
			StatelessSession = NHibernateStatelessSessionManager.Instance.GetStatelessSession();
		}
	}
}
