using MongoDB.Bson;

namespace MongoDbDriverSampleApp.Documents
{
  public class Category
  {
    public ObjectId Id { get; set; }
    public string Name { get; set; }

    public Category()
    {
      Id = ObjectId.GenerateNewId(); // Program tarafında Object Id üretmek istersek.
    }
  }
}
