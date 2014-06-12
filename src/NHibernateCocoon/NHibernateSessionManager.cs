using System.Configuration;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web;
using FluentNHibernate.Cfg;
using NHibernate;
using Configuration = NHibernate.Cfg.Configuration;

namespace NHibernateCocoon
{
	/// <summary>
	/// http://www.codeproject.com/Articles/13390/NHibernate-Best-Practices-with-ASP-NET-1-2nd-Ed
	/// </summary>
	public class NHibernateSessionManager
	{
		private const string TransactionKey = "NHIBERNATE_COCOON_CONTEXT_TRANSACTION";
		private const string SessionKey = "NHIBERNATE_COCOON_CONTEXT_SESSION";
		private ISessionFactory _sessionFactory;

		/// <summary>
		/// This is a thread-safe, lazy singleton.
		/// </summary>
		public static NHibernateSessionManager Instance
		{
			get { return Nested.NHibernateSessionManager; }
		}

		/// <summary>
        /// Holds the configuration information for NHibernateCocoon.
		/// </summary>
		public Config Config { get; private set; }

		/// <summary>
		/// Initializes the NHibernate session factory upon instantiation.
		/// </summary>
		private NHibernateSessionManager()
		{
			InitSessionFactory();
		}

		/// <summary>
		/// Private class that assists with ensuring thread-safe, lazy singleton.
		/// </summary>
		private static class Nested
		{
			static Nested() { }
			internal static readonly NHibernateSessionManager NHibernateSessionManager = new NHibernateSessionManager();
		}

		/// <summary>
		/// Creates the session factory.
		/// </summary>
		private void InitSessionFactory()
		{
			// Get config section
            Config = (Config)ConfigurationManager.GetSection("nhibernateCocoon");
			if (Config == null)
			{
                throw new ConfigurationErrorsException("The nhibernateCocoon config section cannot be null.");
			}

			// Read NHibernate config settings
			var cfg = new Configuration();
			cfg.Configure();

			// Configure and create the session factory, using the fluent mappings from the assembly in config
			_sessionFactory = Fluently.Configure(cfg)
				.Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.Load(Config.FluentMappingAssemblyName)))
				.BuildSessionFactory();
		}

		/// <summary>
		/// Allows you to register an interceptor on a new session. This may not be called if there is already
		/// an open session attached to the HttpContext. If you have an interceptor to be used, modify the
		/// HttpModule to call this before calling BeginTransaction().
		/// </summary>
		public void RegisterInterceptor(IInterceptor interceptor)
		{
			var session = ContextSession;

			if (session != null && session.IsOpen)
			{
				throw new HibernateException("You cannot register an interceptor once a session has already been opened.");
			}

			GetSession(interceptor);
		}

		/// <summary>
		/// Gets the NHibernate session.
		/// </summary>
		/// <returns></returns>
		public ISession GetSession()
		{
			return GetSession(null);
		}

		/// <summary>
		/// Gets an NHibernate session with or without an interceptor. This method is not called directly; instead,
		/// it gets invoked from other public methods.
		/// </summary>
		private ISession GetSession(IInterceptor interceptor)
		{
			var session = ContextSession;

			if (session == null)
			{
				session = interceptor != null ? _sessionFactory.OpenSession(interceptor) : _sessionFactory.OpenSession();
				ContextSession = session;
			}

			if (session == null)
			{
				throw new HibernateException("NHibernate session was null.");
			}

			return session;
		}

		/// <summary>
		/// Flushes anything left in the NHibernate session and closes the connection.
		/// </summary>
		public void CloseSession()
		{
			var session = ContextSession;

			if (session != null && session.IsOpen)
			{
				session.Flush();
				session.Close();
			}

			ContextSession = null;
		}

		/// <summary>
		/// Begins a transaction for the NHibernate session.
		/// </summary>
		public void BeginTransaction()
		{
			var transaction = ContextTransaction;

			if (transaction == null)
			{
				transaction = GetSession().BeginTransaction();
				ContextTransaction = transaction;
			}
		}

		/// <summary>
		/// Commits the transaction for the NHibernate session.
		/// </summary>
		public void CommitTransaction()
		{
			var transaction = ContextTransaction;

			try
			{
				if (HasOpenTransaction())
				{
					transaction.Commit();
					ContextTransaction = null;
				}
			}
			catch (HibernateException)
			{
				RollbackTransaction();
				throw;
			}
		}

		/// <summary>
		/// Rolls back the open transaction.
		/// </summary>
		public void RollbackTransaction()
		{
			var transaction = ContextTransaction;

			try
			{
				if (HasOpenTransaction())
				{
					transaction.Rollback();
				}

				ContextTransaction = null;
			}
			finally
			{
				CloseSession();
			}
		}

		/// <summary>
		/// Check if there is an open transaction.
		/// </summary>
		/// <returns></returns>
		public bool HasOpenTransaction()
		{
			var transaction = ContextTransaction;

			return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
		}

		/// <summary>
		/// If within a web context, this uses HttpContext instead of CallContext (which is what
		/// will be used when running tests - since most tests won't have HttpContext). Discussion
		/// about this can be found at http://forum.springframework.net/showthread.php?t=572.
		/// </summary>
		private ITransaction ContextTransaction
		{
			get
			{
				if (IsInWebContext())
				{
					return (ITransaction)HttpContext.Current.Items[TransactionKey];
				}
				else
				{
					return (ITransaction)CallContext.GetData(TransactionKey);
				}
			}
			set
			{
				if (IsInWebContext())
				{
					HttpContext.Current.Items[TransactionKey] = value;
				}
				else
				{
					CallContext.SetData(TransactionKey, value);
				}
			}
		}

		/// <summary>
		/// If within a web context, this uses HttpContext instead of CallContext (which is what
		/// will be used when running tests - since most tests won't have HttpContext). Discussion
		/// about this can be found at http://forum.springframework.net/showthread.php?t=572.
		/// </summary>
		private ISession ContextSession
		{
			get
			{
				if (IsInWebContext())
				{
					return (ISession)HttpContext.Current.Items[SessionKey];
				}
				else
				{
					return (ISession)CallContext.GetData(SessionKey);
				}
			}
			set
			{
				if (IsInWebContext())
				{
					HttpContext.Current.Items[SessionKey] = value;
				}
				else
				{
					CallContext.SetData(SessionKey, value);
				}
			}
		}

		private bool IsInWebContext()
		{
			return HttpContext.Current != null;
		}
	}
}
