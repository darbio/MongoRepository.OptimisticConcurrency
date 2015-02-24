namespace MongoRepository.OptimisticConcurrency.Tests.Entities
{
    [CollectionName("Books")]
    public class Book : ConcurrentEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public string AuthorName { get; set; }
    }
}
