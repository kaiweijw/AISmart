using MongoDB.Driver;
using AISmart.Authors;
using Volo.Abp.MongoDB;

namespace AISmart.Books;

public class BookStoreMongoDbContext : AbpMongoDbContext
{
    public IMongoCollection<Book> Books => Collection<Book>();
    public IMongoCollection<Author> Authors => Collection<Author>();

}
