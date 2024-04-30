using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Core.WireProtocol.Messages;
using MongoDbDriverSampleApp.Documents;
using MongoDbDriverSampleApp.Settiings;

namespace MongoDbDriverSampleApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class StudentsController : ControllerBase
  {

    private readonly IMongoCollection<Student> studentCollection;
    private readonly IMongoCollection<Teacher> teacherCollection;
    private readonly IClientSessionHandle clientSession;

    public StudentsController(IMongoDbSettings mongoDbSettings)
    {
      var client = new MongoClient(mongoDbSettings.ConnectionString);
      var db = client.GetDatabase(mongoDbSettings.DatabaseName);
      clientSession = client.StartSession();
      studentCollection = db.GetCollection<Student>("students");
      teacherCollection = db.GetCollection<Teacher>("teachers");
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {

      clientSession.StartTransaction();

      try
      {
        var t1 = new Teacher
        {
          Name = "Tansu"
        };

        var t2 = new Teacher
        {
          Name = "Hakan"
        };

        var s = new Student
        {
          Name = "Mehmet",
          ClassName = "A",
          TeacherIds = new List<MongoDB.Bson.ObjectId> { t1.Id, t2.Id }
        };

        t1.StudentIds.Add(s.Id);
        t2.StudentIds.Add(s.Id);

        await teacherCollection.InsertManyAsync(new List<Teacher> { t1, t2 });

        throw new Exception("Mongo Exception");

        await studentCollection.InsertOneAsync(s);

        await clientSession.CommitTransactionAsync();
      }
      catch
      {
        await clientSession.AbortTransactionAsync();
      }
      

      return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      return Ok();
    }
  }
}
