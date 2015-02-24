using Xunit;

namespace MongoRepository.OptimisticConcurrency.Tests
{
    using System.Configuration;

    using MongoDB.Driver;

    using MongoRepository.OptimisticConcurrency.Tests.Entities;
    using MongoRepository.OptimisticConcurrency.Tests.Repositories;

    public class ConcurrentMongoRepositoryTest
    {
        private BookRepository bookRepository = new BookRepository(new MongoUrl(ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString));

        [Fact]
        public void CheckAdd()
        {
            // Insert the book
            var book = this.bookRepository.Add(Books.JungleBook);

            // Check the access id is set
            Assert.NotNull(book.AccessId);

            // Tidy up
            this.bookRepository.Delete(book);
        }

        /// <summary>
        /// This tests that a book can be updated.
        /// </summary>
        [Fact]
        public void CheckUpdate_Passes()
        {
            // Insert the book
            var book = this.bookRepository.Add(Books.JungleBook);

            // Retrieve and edit
            var book2 = this.bookRepository.GetById(book.Id);
            book2.AuthorName = "KIPLING, Rudyard";

            // Try update
            // This will throw an exception on failure
            var book3 = this.bookRepository.Update(book2);

            // Make sure we persisted our data
            Assert.Equal(book3.AuthorName, book2.AuthorName);

            // Tidy up
            this.bookRepository.Delete(book);
        }

        /// <summary>
        /// This tests the failure when teh same book is edited at the same time.
        /// </summary>
        [Fact]
        public void CheckUpdate_Fails()
        {
            // Insert the book
            var book = this.bookRepository.Add(Books.JungleBook);

            // Retrieve the books
            // Retrieve and edit
            var book2 = this.bookRepository.GetById(book.Id);
            var book3 = this.bookRepository.GetById(book.Id);

            // Edit book 2 and save
            book2.AuthorName = "KIPLING, Rudyard";
            this.bookRepository.Update(book2);

            // Edit book 3 and save
            book3.AuthorName = "Rudy Kipz";

            // Boom exception shoudl be thrown
            var ex = Assert.Throws<MongoConcurrencyException>(() => this.bookRepository.Update(book3));

            // Tidy up
            this.bookRepository.Delete(book);
        }

        private static class Books
        {
            public static Book JungleBook
            {
                get
                {
                    return new Book()
                    {
                        Name = "The Jungle Book",
                        AuthorName = "Rudyard Kipling",
                        Description =
                            "The Jungle Book (1894) is a collection of stories by English author Rudyard Kipling. The stories were first published in magazines in 1893–94. The original publications contain illustrations, some by Rudyard's father, John Lockwood Kipling."
                    };
                }
            }
        }
    }
}
