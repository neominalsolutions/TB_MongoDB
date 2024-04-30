using MongoDB.Bson;

namespace MongoDbDriverSampleApp.Documents
{
  public class Teacher
  {
    public ObjectId  Id { get; set; }

    public string Name { get; set; }

    // Eğitmenin birden fazla öğrencisi var ilişkisi
    public List<ObjectId> StudentIds { get; set; } = new List<ObjectId>();


    public Teacher()
    {
      Id = ObjectId.GenerateNewId();
    }
  }
}
