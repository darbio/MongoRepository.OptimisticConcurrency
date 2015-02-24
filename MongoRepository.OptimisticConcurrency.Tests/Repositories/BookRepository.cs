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
