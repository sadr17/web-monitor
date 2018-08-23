namespace WebMonitorApi.Models
{
  /// <summary>
  /// Base interface for all entities.
  /// </summary>
  public interface IEntity
  {
    /// <summary>
    /// Unique identificator.
    /// </summary>
    int Id { get; set; }
  }
}
