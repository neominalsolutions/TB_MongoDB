namespace MongoDbDriverSampleApp.Repositories
{
  public interface IMongoRepository<TDocument>
    where TDocument: Document
  {

    public Task CreateAsync(TDocument doc);
    public Task UpdateAsync(TDocument doc);
    public Task DeleteAsync(string Id);




  }
}
