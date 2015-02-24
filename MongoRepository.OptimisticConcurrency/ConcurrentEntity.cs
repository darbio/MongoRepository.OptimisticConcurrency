namespace MongoRepository.OptimisticConcurrency
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System.Runtime.Serialization;

    /// <summary>
    /// Abstract Entity for all the BusinessEntities.
    /// </summary>
    [DataContract]
    [Serializable]
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class ConcurrentEntity : Entity, IConcurrentEntity<string>
    {
        /// <summary>
        /// Gets or sets the access id for this object.
        /// </summary>
        /// <value>The access id for this object.</value>
        [DataMember]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AccessId { get; set; }
    }
}
