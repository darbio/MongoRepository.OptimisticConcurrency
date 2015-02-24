namespace MongoRepository.OptimisticConcurrency
{
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Generic Concurrent Entity interface
    /// </summary>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public interface IConcurrentEntity<TKey> : IEntity<TKey>
    {
        /// <summary>
        /// Gets or sets the _accessId of the entity.
        /// </summary>
        /// <value>AccessId of the entity</value>
        [BsonElement("_accessId")]
        string AccessId { get; set; }
    }

    /// <summary>
    /// "Default" Entity interface.
    /// </summary>
    /// <remarks>Entities are assumed to use strings for ID's</remarks>
    public interface IConcurrentEntity : IEntity<string>
    {
    }
}
