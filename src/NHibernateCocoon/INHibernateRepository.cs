using System.Collections.Generic;
using NHibernate;

namespace NHibernateCocoon
{
	/// <summary>
	/// Provides NHibernate data access methods for entity of type T.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	public interface INHibernateRepository<T> where T : class
	{
		/// <summary>
		/// Retrieves an entity based on its ID.
		/// </summary>
		/// <param name="id">The ID of the entity.</param>
		/// <returns>The entity.</returns>
		T Get(object id);

		/// <summary>
		/// Retrieves an entity based on the name and value of a property.
		/// </summary>
		/// <param name="property">The name of the property; should be a member of type T.</param>
		/// <param name="value">The value of the property.</param>
		/// <returns>The entity.</returns>
		T GetByProperty(string property, object value);

		/// <summary>
		/// Retrieves the value of a property from an entity.
		/// </summary>
		/// <typeparam name="TProperty">The return type of the property value.</typeparam>
		/// <param name="property">The name of the property to retrieve.</param>
		/// <param name="idName">The name of the ID property of the entity.</param>
		/// <param name="idValue">The value of the ID property of the entity.</param>
		/// <returns>The value of the property.</returns>
		TProperty GetProperty<TProperty>(string property, string idName, object idValue);

		/// <summary>
		/// Retrieves the values of many properties of an entity.
		/// </summary>
		/// <param name="properties">The names of the properties to retrieve.</param>
		/// <param name="idName">The name of the ID property of the entity.</param>
		/// <param name="idValue">The value of the ID property of the entity.</param>
		/// <returns>The list of values for the properties.</returns>
		object[] GetProperties(string[] properties, string idName, object idValue);

		/// <summary>
		/// Retrieves the first entity to match the criteria.
		/// </summary>
		/// <param name="criteria">The entity criteria.</param>
		/// <returns>The first entity to match the criteria.</returns>
		T FindFirst(ICriteria criteria);

		/// <summary>
		/// Retrieves the first entity to match the criteria.
		/// </summary>
		/// <param name="criteria">The entity criteria.</param>
		/// <param name="sorts">The list of property sorts.</param>
		/// <returns>The first entity to match the criteria.</returns>
		T FindFirst(ICriteria criteria, NHibernateRepositorySortList sorts);

		/// <summary>
		/// Retrieves entities based on certain criteria.
		/// </summary>
		/// <param name="criteria">The entity criteria.</param>
		/// <param name="totalItems">The total number of items.</param>
		/// <returns>The list of entities that matched the criteria.</returns>
		IList<T> Find(ICriteria criteria, out int totalItems);

		/// <summary>
		/// Retrieves entities based on certain criteria.
		/// </summary>
		/// <param name="criteria">The entity criteria.</param>
		/// <param name="sorts">The list of property sorts.</param>
		/// <param name="totalItems">The total number of items.</param>
		/// <returns>The list of entities that matched the criteria.</returns>
		IList<T> Find(ICriteria criteria, NHibernateRepositorySortList sorts, out int totalItems);

		/// <summary>
		/// Retrieves entities based on certain criteria.
		/// </summary>
		/// <param name="criteria">The entity criteria.</param>
		/// <param name="sorts">The list of property sorts.</param>
		/// <param name="pageIndex">The page index.</param>
		/// <param name="pageSize">The page size.</param>
		/// <param name="totalItems">The total number of items.</param>
		/// <returns>The list of entities that matched the criteria.</returns>
		IList<T> Find(ICriteria criteria, NHibernateRepositorySortList sorts, int? pageIndex, int? pageSize, out int totalItems);

		/// <summary>
		/// Retrieves all entities.
		/// </summary>
		/// <param name="totalItems">The total number of items.</param>
		/// <returns>A list of all entities.</returns>
		IList<T> FindAll(out int totalItems);

		/// <summary>
		/// Retrieves all entities.
		/// </summary>
		/// <param name="sorts">The list of property sorts.</param>
		/// <param name="totalItems">The total number of items.</param>
		/// <returns>A list of all entities.</returns>
		IList<T> FindAll(NHibernateRepositorySortList sorts, out int totalItems);

		/// <summary>
		/// Retrieves all entities.
		/// </summary>
		/// <param name="sorts">The list of property sorts.</param>
		/// <param name="pageIndex">The page index.</param>
		/// <param name="pageSize">The page size.</param>
		/// <param name="totalItems">The total number of items.</param>
		/// <returns>A list of all entities.</returns>
		IList<T> FindAll(NHibernateRepositorySortList sorts, int? pageIndex, int? pageSize, out int totalItems);

		/// <summary>
		/// Retrieves entities based on the name and value of a property.
		/// </summary>
		/// <param name="property">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		/// <param name="totalItems">The total number of items.</param>
		/// <returns>A list of entities that have a property with the name and value.</returns>
		IList<T> FindByProperty(string property, object value, out int totalItems);

		/// <summary>
		/// Retrieves entities based on the name and value of a property.
		/// </summary>
		/// <param name="property">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		/// <param name="sorts">The list of property sorts.</param>
		/// /// <param name="totalItems">The total number of items.</param>
		/// <returns>A list of entities that have a property with the name and value.</returns>
		IList<T> FindByProperty(string property, object value, NHibernateRepositorySortList sorts, out int totalItems);

		/// <summary>
		/// Retrieves entities based on the name and value of a property.
		/// </summary>
		/// <param name="property">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		/// <param name="sorts">The list of property sorts.</param>
		/// <param name="pageIndex">The page index.</param>
		/// <param name="pageSize">The page size.</param>
		/// <param name="totalItems">The total number of items.</param>
		/// <returns>A list of entities that have a property with the name and value.</returns>
		IList<T> FindByProperty(string property, object value, NHibernateRepositorySortList sorts, int? pageIndex, int? pageSize, out int totalItems);

		/// <summary>
		/// Saves the entity.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		/// <remarks>The entity is not actually saved until the transaction is committed.</remarks>
		void Save(T entity);

		/// <summary>
		/// Saves or updates the entity.
		/// </summary>
		/// <param name="entity">The entity to save or update.</param>
		/// <remarks>The entity is not actually saved or updated until the transaction is committed.</remarks>
		void SaveOrUpdate(T entity);

		/// <summary>
		/// Saves the entity then evicts it from the cache.
		/// </summary>
		/// <param name="entity">The entity to save and evict.</param>
		/// <remarks>The entity is not actually saved and evicted until the transaction is committed.</remarks>
		void SaveAndEvict(T entity);

		/// <summary>
		/// Evicts the entity from cache.
		/// </summary>
		/// <param name="entity">The entity to evict.</param>
		/// <remarks>The entity is not actually evicted until the transaction is committed.</remarks>
		void Evict(T entity);

		/// <summary>
		/// Updates the entity.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <remarks>The entity is not actually updated until the transaction is committed.</remarks>
		void Update(T entity);

		/// <summary>
		/// Merges the entity.
		/// </summary>
		/// <param name="entity">The entity to merge.</param>
		/// <remarks>The entity is not actually merged until the transaction is committed.</remarks>
		void Merge(T entity);

		/// <summary>
		/// Deletes an entity.
		/// </summary>
		/// <param name="id">The ID of the entity to delete.</param>
		void Delete(object id);

		/// <summary>
		/// Deletes an entity.
		/// </summary>
		/// <param name="entity">The entity to delete.</param>
		/// <remarks>The entity is not actually deleted until the transaction is committed.</remarks>
		void Delete(T entity);

		/// <summary>
		/// Deletes all entities that match the criteria.
		/// </summary>
		/// <param name="criteria">The entity criteria.</param>
		void Delete(ICriteria criteria);

		/// <summary>
		/// Retrieves the total number of entities that match the criteria.
		/// </summary>
		/// <param name="criteria">The entity criteria.</param>
		/// <returns>The total number of entities that match the criteria.</returns>
		int Count(ICriteria criteria);

		/// <summary>
		/// Checks if at least one entity exists that matches the criteria.
		/// </summary>
		/// <param name="criteria">The entity criteria.</param>
		/// <returns>True if at least one entity matches the criteria; otherwise, false.</returns>
		bool Exists(ICriteria criteria);
	}
}
