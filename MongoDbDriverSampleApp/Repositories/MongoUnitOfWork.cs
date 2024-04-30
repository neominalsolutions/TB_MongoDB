using MongoDB.Driver;
using MongoDbDriverSampleApp.Settiings;

namespace MongoDbDriverSampleApp.Repositories
{
  public class MongoUnitOfWork : IMongoUnitOfWork
  {


    private List<Func<Task>> _operations { get; set; } = new List<Func<Task>>(); // asenkron tüm db işlemleri bu listede tutulacaktır.

    private IMongoClient client;
    public MongoUnitOfWork(IMongoDbSettings mongoDbSettings)
    {
      client = new MongoClient(mongoDbSettings.ConnectionString);
      var db = client.GetDatabase(mongoDbSettings.DatabaseName);
 
      
    }
    public Task AddOperation(Func<Task> operation)
    {
      _operations.Add(operation);

      return Task.CompletedTask;

    }

    public async Task CommitAsync()
    {
     var session =  await client.StartSessionAsync();
      session.StartTransaction();
     

      try
      {

       await Task.WhenAll(_operations.Select(f => f()));

        await session.CommitTransactionAsync();

      }
      catch (Exception)
      {
       await session.AbortTransactionAsync();
      }

    }
  }
}
