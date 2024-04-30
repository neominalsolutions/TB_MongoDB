using MongoDbDriverSampleApp.Documents;

namespace MongoDbDriverSampleApp.Repositories.Products
{
  public interface IMongoProductRepo:IMongoRepository<Product>
  {
    // Buarada sadece Product ile ilgili ek repository işlemleri yapılsın diye açtık.
  }
}
