using System.Collections.Generic;
using WebMonitorApi.Models;

namespace WebMonitorApi.Repository
{
  /// <summary>
  /// Repository interface for storing data.
  /// </summary>
  /// <typeparam name="T">Entity.</typeparam>
  public interface IRepository<T> where T: class, IEntity
  {
    /// <summary>
    /// Get all entities.
    /// </summary>
    /// <returns></returns>
    IEnumerable<T> GetAll();

    /// <summary>
    /// Get entity by id.
    /// </summary>
    /// <param name="id">Unique identificator.</param>
    /// <returns>Entity.</returns>
    T Get(int id);

    /// <summary>
    /// Save entity.
    /// </summary>
    /// <param name="entity">Entity.</param>
    void Save(T entity);

    /// <summary>
    /// Delete entity.
    /// </summary>
    /// <param name="id">Unique identificator.</param>
    void Delete(int id);
  }
}
