using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernateCriterion = NHibernate.Criterion;

namespace NHibernateCocoon
{
	/// <summary>
	/// Base class for NHibernateRepository and NHibernateStatelessRepository.
    /// Note about transactions: If NHibernateCocoon is configured to use batch
    /// transaction scope, then the Begin and Commit transaction calls found in
    /// the methods in this class will use the transaction that was created at
    /// the start of the request. Otherwise, they will use their own transaction
    /// created at the statement level.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	public abstract class BaseNHibernateRepository<T> : INHibernateRepository<T> where T : class
	{
		/// <summary>
		/// The NHibernate session.
		/// </summary>
		public ISession Session { get; set; }

		/// <summary>
		/// The stateless NHibernate session.
		/// </summary>
		public IStatelessSession StatelessSession { get; set; }

		private bool IsStatelessSession
		{
			get { return StatelessSession != null; }
		}

		public virtual T Get(object id)
		{
			return IsStatelessSession
				? StatelessSession.Get<T>(id)
				: Session.Get<T>(id);
		}

		public virtual T GetByProperty(string property, object value)
		{
			var hql = new StringBuilder();
			hql.Append(String.Format("FROM {0} t ", typeof(T).FullName));
			hql.Append(String.Format("WHERE t.{0} = ?", property));

			var query = IsStatelessSession
				? StatelessSession.CreateQuery(hql.ToString())
				: Session.CreateQuery(hql.ToString());

			return query
				.SetParameter(0, value)
				.UniqueResult<T>();
		}

		public virtual TProperty GetProperty<TProperty>(string property, string idName, object idValue)
		{
			var hql = new StringBuilder();
			hql.Append(String.Format("SELECT t.{0} ", property));
			hql.Append(String.Format("FROM {0} t ", typeof(T).FullName));
			hql.Append(String.Format("WHERE t.{0} = ?", idName));

			var query = IsStatelessSession
				? StatelessSession.CreateQuery(hql.ToString())
				: Session.CreateQuery(hql.ToString());

			return query
				.SetParameter(0, idValue)
				.UniqueResult<TProperty>();
		}

		public virtual object[] GetProperties(string[] properties, string idName, object idValue)
		{
			var hql = "SELECT ";

			foreach (var p in properties)
			{
				if (!String.IsNullOrEmpty(p))
				{
					hql += String.Format("t.{0}, ", p);
				}
			}

			// Replace the trailing comma and space with just a space
			hql = hql.TrimEnd(", ".ToCharArray());
			hql += " ";

			// Now add the FROM and WHERE clauses
			hql += String.Format("FROM {0} t ", typeof(T).FullName);
			hql += String.Format("WHERE t.{0} = ?", idName);

			var query = IsStatelessSession
				? StatelessSession.CreateQuery(hql)
				: Session.CreateQuery(hql);

			return query
				.SetParameter(0, idValue)
				.List<object>()
				.ToArray();
		}

		public virtual T FindFirst(ICriteria criteria)
		{
			return FindFirst(criteria, null);
		}

		public virtual T FindFirst(ICriteria criteria, NHibernateRepositorySortList sorts)
		{
			AddSortOrders(criteria, sorts);

			var totalItems = 0;
			var list = GetList(criteria, 0, 1, out totalItems);

			return list.Count > 0 ? list[0] : default(T);
		}

		public virtual IList<T> Find(ICriteria criteria, out int totalItems)
		{
			return Find(criteria, null, out totalItems);
		}

		public virtual IList<T> Find(ICriteria criteria, NHibernateRepositorySortList sorts, out int totalItems)
		{
			return Find(criteria, sorts, null, null, out totalItems);
		}

		public virtual IList<T> Find(ICriteria criteria, NHibernateRepositorySortList sorts, int? pageIndex, int? pageSize, out int totalItems)
		{
			AddSortOrders(criteria, sorts);
			return GetList(criteria, pageIndex, pageSize, out totalItems);
		}

		public virtual IList<T> FindAll(out int totalItems)
		{
			return FindAll(null, out totalItems);
		}

		public virtual IList<T> FindAll(NHibernateRepositorySortList sorts, out int totalItems)
		{
			return FindAll(sorts, null, null, out totalItems);
		}

		public virtual IList<T> FindAll(NHibernateRepositorySortList sorts, int? pageIndex, int? pageSize, out int totalItems)
		{
			var criteria = IsStatelessSession
				? StatelessSession.CreateCriteria(typeof(T))
				: Session.CreateCriteria(typeof(T));

			AddSortOrders(criteria, sorts);
			return GetList(criteria, pageIndex, pageSize, out totalItems);
		}

		public virtual IList<T> FindByProperty(string property, object value, out int totalItems)
		{
			return FindByProperty(property, value, null, out totalItems);
		}

		public virtual IList<T> FindByProperty(string property, object value, NHibernateRepositorySortList sorts, out int totalItems)
		{
			return FindByProperty(property, value, sorts, null, null, out totalItems);
		}

		public virtual IList<T> FindByProperty(string property, object value, NHibernateRepositorySortList sorts, int? pageIndex, int? pageSize, out int totalItems)
		{
			var criteria = IsStatelessSession
				? StatelessSession.CreateCriteria(typeof(T))
				: Session.CreateCriteria(typeof(T));

			criteria.Add(NHibernateCriterion.Restrictions.Eq(property, value));

			AddSortOrders(criteria, sorts);
			return GetList(criteria, pageIndex, pageSize, out totalItems);
		}

		public virtual void Save(T entity)
		{
			if (IsStatelessSession)
			{
				if (!StatelessSession.IsOpen) throw new HibernateException("The stateless NHibernate session must be open before an entity can be saved.");
				if (StatelessSession.Transaction == null) throw new HibernateException("Saves must be done within an NHibernate transaction.");

				StatelessSession.BeginTransaction();
				StatelessSession.Insert(entity);
				StatelessSession.Transaction.Commit();
			}
			else
			{
				if (!Session.IsOpen) throw new HibernateException("NHibernate session must be open before an entity can be saved.");
				if (Session.Transaction == null) throw new HibernateException("Saves must be done within an NHibernate transaction.");

				Session.BeginTransaction();
				Session.Save(entity);
				Session.Transaction.Commit();
			}
		}

		public virtual void SaveOrUpdate(T entity)
		{
			if (IsStatelessSession)
			{
				throw new HibernateException("SaveOrUpdate() does not apply to stateless NHibernate sessions; you must call Save() and Update() individually.");
			}
			else
			{
				if (!Session.IsOpen) throw new HibernateException("NHibernate session must be open before an entity can be saved or updated.");
				if (Session.Transaction == null) throw new HibernateException("Saves or updates must be done within an NHibernate transaction.");

				Session.BeginTransaction();
				Session.SaveOrUpdate(entity);
				Session.Transaction.Commit();
			}
		}

		public virtual void SaveAndEvict(T entity)
		{
			if (IsStatelessSession)
			{
				throw new HibernateException("SaveAndEvict() does not apply to stateless NHibernate sessions.");
			}
			else
			{
				if (!Session.IsOpen) throw new HibernateException("NHibernate session must be open before an entity can be saved and evicted.");
				if (Session.Transaction == null) throw new HibernateException("Saves and evicts must be done within an NHibernate transaction.");

				Session.BeginTransaction();
				Session.Save(entity);
				Session.Evict(entity);
				Session.Transaction.Commit();
			}
		}

		public virtual void Evict(T entity)
		{
			if (IsStatelessSession)
			{
				throw new HibernateException("Evict() does not apply to stateless NHibernate sessions.");
			}
			else
			{
				if (!Session.IsOpen) throw new HibernateException("NHibernate session must be open before an entity can be evicted.");
				if (Session.Transaction == null) throw new HibernateException("Evicts must be done within an NHibernate transaction.");

				Session.BeginTransaction();
				Session.Evict(entity);
				Session.Transaction.Commit();
			}
		}

		public virtual void Update(T entity)
		{
			if (IsStatelessSession)
			{
				if (!StatelessSession.IsOpen) throw new HibernateException("The stateless NHibernate session must be open before an entity can be updated.");
				if (StatelessSession.Transaction == null) throw new HibernateException("Updates must be done within an NHibernate transaction.");

				StatelessSession.BeginTransaction();
				StatelessSession.Update(entity);
				StatelessSession.Transaction.Commit();
			}
			else
			{
				if (!Session.IsOpen) throw new HibernateException("NHibernate session must be open before an entity can be updated.");
				if (Session.Transaction == null) throw new HibernateException("Updates must be done within an NHibernate transaction.");

				Session.BeginTransaction();
				Session.Update(entity);
				Session.Transaction.Commit();
			}
		}

		public virtual void Merge(T entity)
		{
			if (IsStatelessSession)
			{
				throw new HibernateException("Merge() does not apply to stateless NHibernate sessions.");
			}
			else
			{
				if (!Session.IsOpen) throw new HibernateException("NHibernate session must be open before an entity can be merged.");
				if (Session.Transaction == null) throw new HibernateException("Merges must be done within an NHibernate transaction.");

				Session.BeginTransaction();
				Session.Merge(entity);
				Session.Transaction.Commit();
			}
		}

		public virtual void Delete(object id)
		{
			var entity = Get(id);
			Delete(entity);
		}

		public virtual void Delete(T entity)
		{
			if (IsStatelessSession)
			{
				if (!StatelessSession.IsOpen) throw new HibernateException("The stateless NHibernate session must be open before an entity can be deleted.");
				if (StatelessSession.Transaction == null) throw new HibernateException("Deletes must be done within an NHibernate transaction.");

				StatelessSession.BeginTransaction();
				StatelessSession.Delete(entity);
				StatelessSession.Transaction.Commit();
			}
			else
			{
				if (!Session.IsOpen) throw new HibernateException("NHibernate session must be open before an entity can be deleted.");
				if (Session.Transaction == null) throw new HibernateException("Deletes must be done within an NHibernate transaction.");

				Session.BeginTransaction();
				Session.Delete(entity);
				Session.Transaction.Commit();
			}
		}

		public virtual void Delete(ICriteria criteria)
		{
			var totalItems = 0;
			var list = Find(criteria, out totalItems);

			foreach (var item in list)
			{
				Delete(item);
			}
		}

		public virtual int Count(ICriteria criteria)
		{
			return criteria.SetProjection(NHibernateCriterion.Projections.RowCount()).UniqueResult<int>();
		}

		public virtual bool Exists(ICriteria criteria)
		{
			return Count(criteria) > 0;
		}

		private void AddSortOrders(ICriteria criteria, NHibernateRepositorySortList sorts)
		{
			if (criteria != null)
			{
				if (sorts != null && sorts.Count > 0)
				{
					foreach (var sort in sorts)
					{
						var order = GetSortOrder(sort);
						if (order != null)
						{
							criteria.AddOrder(order);
						}
					}
				}
			}
		}

		private NHibernateCriterion.Order GetSortOrder(NHibernateRepositorySort sort)
		{
			if (sort == null) return null;

			// Default the order to ascending
			var order = new NHibernateCriterion.Order(sort.Property, true);

			// Check to switch to descending
			if (sort.SortDirection == NHibernateRepositorySortDirection.Desc)
			{
				order = new NHibernateCriterion.Order(sort.Property, false);
			}

			return order;
		}

		private IList<T> GetList(ICriteria criteria, int? pageIndex, int? pageSize, out int totalItems)
		{
			// Clone the criteria so it can be used for getting the total items
			var totalItemsCriteria = CriteriaTransformer.Clone(criteria);
			totalItems = Count(totalItemsCriteria);

			// Only do paging if both index and size are set
			if (pageIndex.HasValue && pageSize.HasValue)
			{
				var pageStart = pageIndex.Value * pageSize.Value;
				return criteria.SetFirstResult(pageStart).SetMaxResults(pageSize.Value).List<T>();
			}

			// Otherwise return everything
			return criteria.List<T>();
		}
	}
}
