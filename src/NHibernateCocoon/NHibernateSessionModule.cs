using System;
using System.Web;

namespace NHibernateCocoon
{
	/// <summary>
	/// HTTP module that opens and closes an NHibernate session during the request lifecycle.
	/// If transaction scope is set to request, then a transaction will start when the session
	/// is opened and will commit when the session closes.
	/// </summary>
	public class NHibernateSessionModule : IHttpModule
	{
		/// <summary>
		/// Initializes the module and sets up the request event handlers.
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(OpenSession);
			context.EndRequest += new EventHandler(CloseSession);
		}

		/// <summary>
		/// Opens an NHibernate session.
		/// </summary>
		private void OpenSession(object sender, EventArgs e)
		{
			NHibernateSessionManager.Instance.GetSession();

			// If transaction scope is request, start transaction at the beginning of the request.
			if (NHibernateSessionManager.Instance.Config.TransactionScope == "request")
			{
				NHibernateSessionManager.Instance.BeginTransaction();
			}
		}

		/// <summary>
		/// Closes the NHibernate session.
		/// </summary>
		private void CloseSession(object sender, EventArgs e)
		{
			// If transaction scope is request, commit the transaction at the end of the request.
			if (NHibernateSessionManager.Instance.Config.TransactionScope == "request")
			{
				NHibernateSessionManager.Instance.CommitTransaction();
			}

			NHibernateSessionManager.Instance.CloseSession();
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
