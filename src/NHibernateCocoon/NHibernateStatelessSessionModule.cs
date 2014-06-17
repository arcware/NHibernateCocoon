using System;
using System.Web;

namespace NHibernateCocoon
{
	/// <summary>
	/// HTTP module that opens and closes a stateless NHibernate session during the request lifecycle.
	/// If transaction scope is set to request, then a transaction will start when the session is opened
	/// and will commit when the session closes.
	/// </summary>
	public class NHibernateStatelessSessionModule : IHttpModule
	{
		/// <summary>
		/// Initializes the module and sets up the request event handlers.
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(OpenStatelessSession);
			context.EndRequest += new EventHandler(CloseStatelessSession);
		}

		/// <summary>
		/// Opens a stateless NHibernate session.
		/// </summary>
		private void OpenStatelessSession(object sender, EventArgs e)
		{
			NHibernateStatelessSessionManager.Instance.GetStatelessSession();

			// If transaction scope is request, start transaction at the beginning of the request.
			if (NHibernateStatelessSessionManager.Instance.Config.TransactionScope == "request")
			{
				NHibernateStatelessSessionManager.Instance.BeginTransaction();
			}
		}

		/// <summary>
		/// Closes the stateless NHibernate session.
		/// </summary>
		private void CloseStatelessSession(object sender, EventArgs e)
		{
			// If transaction scope is request, commit the transaction at the end of the request.
			if (NHibernateStatelessSessionManager.Instance.Config.TransactionScope == "request")
			{
				NHibernateStatelessSessionManager.Instance.CommitTransaction();
			}

			NHibernateStatelessSessionManager.Instance.CloseStatelessSession();
		}

		/// <summary>
		/// Disposes of the module.
		/// </summary>
		public void Dispose()
		{
			// Nothing to see here, but required to implement the interface.
		}
	}
}
