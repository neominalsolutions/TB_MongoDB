using MongoDbDriverSampleApp.Documents;
using MongoDbDriverSampleApp.Settiings;

namespace MongoDbDriverSampleApp.Repositories.Products
{
  public class MongoProductRepo : MongoRepository<Product>, IMongoProductRepo
  {
    public MongoProductRepo(IMongoUnitOfWork mongoUnitOfWork, IMongoDbSettings mongoDbSettings) : base(mongoUnitOfWork, mongoDbSettings)
    {
    }
  }
}
