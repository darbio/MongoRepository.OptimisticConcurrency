namespace MongoRepository.OptimisticConcurrency
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Driver;

    /// <summary>
    /// Deals with concurrent entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public class ConcurrentMongoRepository<T, TKey> : MongoRepository<T, TKey> where T : IConcurrentEntity<TKey>
    {

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        public ConcurrentMongoRepository()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        public ConcurrentMongoRepository(MongoUrl url)
            : base(url) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public ConcurrentMongoRepository(MongoUrl url, string collectionName)
            : base(url, collectionName) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        public ConcurrentMongoRepository(string connectionString)
            : base(connectionString) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public ConcurrentMongoRepository(string connectionString, string collectionName)
            : base(connectionString, collectionName) { }

        public override T Add(T entity)
        {
            // Validation
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            // Set the access id
            entity.AccessId = this.GenerateAccessId();

            return base.Add(entity);
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public override void Add(IEnumerable<T> entities)
        {
            // Validation
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            // Convert entities to array
            var entitiesArray = entities.ToArray();
            for (var i = 0; i < entitiesArray.Count(); i++)
            {
                // Set the access id
                entitiesArray[i].AccessId = this.GenerateAccessId();
            }

            base.Add(entitiesArray);
        }

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public override T Update(T entity)
        {
            // Validation
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            // Compare the entity
            if (!this.CheckAccessIds(entity))
            {
                throw new MongoConcurrencyException(string.Format("Entity was not updated as it was modified by another writer since being retreived from the db: id = {0}", entity.Id));
            }

            // Set the access id
            entity.AccessId = this.GenerateAccessId();

            // Do the update
            return base.Update(entity);
        }

        /// <summary>
        /// Updates the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public override void Update(IEnumerable<T> entities)
        {
            // Validation
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            // Do the check on each entity
            var invalidEntities = new List<TKey>();
            var entityArray = entities.ToArray();
            for (var i = 0; i < entityArray.Length; i++)
            {
                // Compare the ids
                if (!this.CheckAccessIds(entityArray[i]))
                {
                    invalidEntities.Add(entityArray[i].Id);
                    continue;
                }

                // Set the access id
                entityArray[i].AccessId = this.GenerateAccessId();
            }

            // Check
            if (invalidEntities.Count > 0)
            {
                throw new MongoConcurrencyException(string.Format("Entity collection was not updated as one or more entities were modified by another writer since being retreived from the db: id(s) = {0}", string.Join(", ", invalidEntities)));
            }

            // Do the update
            base.Update(entityArray);
        }

        /// <summary>
        /// Generates a unique access id.
        /// </summary>
        /// <returns>Unique access id</returns>
        private string GenerateAccessId()
        {
            return string.Format("{0}", ObjectId.GenerateNewId());
        }

        /// <summary>
        /// Checks an entity to ensure that the access id is equal to that in the database.
        /// </summary>
        /// <param name="entity">The entity to check</param>
        /// <returns>True if they are the same, false if they are different</returns>
        private bool CheckAccessIds(T entity)
        {
            // Get the entity from the collection to compare
            var retrievedEntity = this.GetById(entity.Id);

            // Entity could not be retrieved
            if (retrievedEntity == null)
            {
                return false;
            }

            // Compare the ids
            if (string.Compare(entity.AccessId, retrievedEntity.AccessId) != 0)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <remarks>Entities are assumed to use strings for Id's.</remarks>
    public class ConcurrentMongoRepository<T> : ConcurrentMongoRepository<T, string>, IRepository<T>
        where T : IConcurrentEntity<string>
    {
        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        public ConcurrentMongoRepository()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        public ConcurrentMongoRepository(MongoUrl url)
            : base(url) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public ConcurrentMongoRepository(MongoUrl url, string collectionName)
            : base(url, collectionName) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        public ConcurrentMongoRepository(string connectionString)
            : base(connectionString) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public ConcurrentMongoRepository(string connectionString, string collectionName)
            : base(connectionString, collectionName) { }
    }
}
