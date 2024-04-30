using MongoDB.Bson;

namespace MongoDbDriverSampleApp.Documents
{
  public class Student
  {
    public ObjectId Id { get; set; } // PK
    public string Name { get; set; }

    public string ClassName { get; set; }

    // ara collection açmadan many to many ref ilişkisi sağlamış olduk.
    public List<ObjectId> TeacherIds { get; set; } = new List<ObjectId>(); // FK


    public Student()
    {
      Id = ObjectId.GenerateNewId();
    }
  }
}
