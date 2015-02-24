MongoRepository.OptimisticConcurrency
===

[![Build status](https://ci.appveyor.com/api/projects/status/uhcew10mawi2vslt?svg=true)](https://ci.appveyor.com/project/darbio/mongorepository-optimisticconcurrency)

Basic optimistic concurrency implemented on top of the [MongoRepository](https://github.com/RobThree/MongoRepository/wiki/Documentation) project.

The repository will add an `_accessId` property to your entity in Mongo DB, and will throw a `MongoConcurrencyException` if optimistic concurrency is violated.

Usage
===

Inherit from `ConcurrentEntity` on your entity class. For example:

```
namespace MongoRepository.OptimisticConcurrency.Tests.Entities
{
    [CollectionName("Books")] // This must be set at the moment, or end up with a ConcurrentEntity collection name in Mongo
    public class Book : ConcurrentEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public string AuthorName { get; set; }
    }
}
```

Inherit from `ConcurrentMongoRepository` on your repository class. For example:

```
namespace MongoRepository.OptimisticConcurrency.Tests.Repositories
{
    using MongoDB.Driver;

    using MongoRepository.OptimisticConcurrency.Tests.Entities;

    public class BookRepository : ConcurrentMongoRepository<Book>
    {
        public BookRepository(MongoUrl url)
            : base()
        {
            
        }
    }
}
```

Use your repo and entity as before:

```
var bookRepository = new BookRepository(new MongoUrl(ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString));
var book = this.bookRepository.Add(Books.JungleBook);
```

Constraints
===

* Collection name must be explicitly specified in the Entity (see tests).
* Does not do concurrency check on delete.