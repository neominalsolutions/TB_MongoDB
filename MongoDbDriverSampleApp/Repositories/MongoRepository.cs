using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbDriverSampleApp.Settiings;

namespace MongoDbDriverSampleApp.Repositories
{
  public abstract class MongoRepository<TDocument> : IMongoRepository<TDocument>
    where TDocument : Document
  {

    private IMongoCollection<TDocument> collection;
    private IMongoUnitOfWork mongoUnitOfWork;

    public MongoRepository(IMongoUnitOfWork mongoUnitOfWork, IMongoDbSettings mongoDbSettings)
    {
      var client = new MongoClient(mongoDbSettings.ConnectionString);
      var db = client.GetDatabase(mongoDbSettings.DatabaseName);
      var collectionName = typeof(TDocument).Name.ToLowerInvariant().Trim();

      //if (typeof(TDocument).Name.ToLowerInvariant().Trim().EndsWith('y'))
      //{

      //  typeof(TDocument).Name.ToLowerInvariant().Trim().LastIndexOf('y');

      //  collectionName = $"{collectionName.Replace('y',' ')}ies";
      //}
      collection = db.GetCollection<TDocument>($"{collectionName}s");
      this.mongoUnitOfWork = mongoUnitOfWork;
    }
    public async Task CreateAsync(TDocument doc)
    {
      // kodu tetiklemeden koleksiyonun içine attık
       await this.mongoUnitOfWork.AddOperation(() => collection.InsertOneAsync(doc));
    }

    public async Task DeleteAsync(string Id)
    {
      var _Id = ObjectId.Parse(Id);
      await this.mongoUnitOfWork.AddOperation(() => collection.DeleteOneAsync(x=> x.Id == _Id));
    }

    public async Task UpdateAsync(TDocument doc)
    {
      await this.mongoUnitOfWork.AddOperation(() => collection.ReplaceOneAsync(x => x.Id == doc.Id,doc));
    }
  }
}
