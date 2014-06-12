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
	/// Nearly the same implementation as NHibernateSessionManager, but with IStatelessSession instead of ISession.
	/// </summary>
	public class NHibernateStatelessSessionManager
	{
		private const string TransactionKey = "NHIBERNATE_COCOON_CONTEXT_TRANSACTION_STATELESS";
		private const string SessionKey = "NHIBERNATE_COCOON_CONTEXT_SESSION_STATELESS";
		private ISessionFactory _sessionFactory;

		/// <summary>
		/// This is a thread-safe, lazy singleton.
		/// </summary>
		public static NHibernateStatelessSessionManager Instance
		{
			get { return Nested.NHibernateStatelessSessionManager; }
		}

		/// <summary>
        /// Holds the configuration information for Cocoon.
		/// </summary>
		public Config Config { get; private set; }

		/// <summary>
		/// Initializes the NHibernate session factory upon instantiation.
		/// </summary>
		private NHibernateStatelessSessionManager()
		{
			InitSessionFactory();
		}

		/// <summary>
		/// Private class that assists with ensuring thread-safe, lazy singleton.
		/// </summary>
		private static class Nested
		{
			static Nested() { }
			internal static readonly NHibernateStatelessSessionManager NHibernateStatelessSessionManager = new NHibernateStatelessSessionManager();
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
		/// Gets the stateless NHibernate session.
		/// </summary>
		/// <returns></returns>
		public IStatelessSession GetStatelessSession()
		{
			var session = ContextSession;

			if (session == null)
			{
				session = _sessionFactory.OpenStatelessSession();
				ContextSession = session;
			}

			if (session == null)
			{
				throw new HibernateException("The stateless NHibernate session was null.");
			}

			return session;
		}

		/// <summary>
		/// Closes the stateless NHibernate session and the connection.
		/// </summary>
		public void CloseStatelessSession()
		{
			var session = ContextSession;

			if (session != null && session.IsOpen)
			{
				session.Close();
			}

			ContextSession = null;
		}

		/// <summary>
		/// Begins a transaction for the stateless NHibernate session.
		/// </summary>
		public void BeginTransaction()
		{
			var transaction = ContextTransaction;

			if (transaction == null)
			{
				transaction = GetStatelessSession().BeginTransaction();
				ContextTransaction = transaction;
			}
		}

		/// <summary>
		/// Commits the transaction for the stateless NHibernate session.
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
				CloseStatelessSession();
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
		private IStatelessSession ContextSession
		{
			get
			{
				if (IsInWebContext())
				{
					return (IStatelessSession)HttpContext.Current.Items[SessionKey];
				}
				else
				{
					return (IStatelessSession)CallContext.GetData(SessionKey);
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
