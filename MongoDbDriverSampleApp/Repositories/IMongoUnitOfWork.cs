namespace MongoDbDriverSampleApp.Repositories
{
  public interface IMongoUnitOfWork
  {

    Task AddOperation(Func<Task> operation);
    Task CommitAsync();

  }
}
