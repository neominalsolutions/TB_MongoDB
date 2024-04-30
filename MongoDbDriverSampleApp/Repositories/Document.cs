using MongoDB.Bson;

namespace MongoDbDriverSampleApp.Repositories
{
  public abstract class Document
  {
    public ObjectId Id { get; set; }

    // Tüm koleksiyonlarda Document değerinin Idsinin üretildiği anı yakaladık.
    public DateTime CreatedAt => Id.CreationTime;
  }
}
