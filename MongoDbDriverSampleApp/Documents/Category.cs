using MongoDB.Bson;
using MongoDbDriverSampleApp.Repositories;

namespace MongoDbDriverSampleApp.Documents
{
  public class Category:Document
  {
    public string Name { get; set; }

    public Category()
    {
      Id = ObjectId.GenerateNewId(); // Program tarafında Object Id üretmek istersek.
    }
  }
}
