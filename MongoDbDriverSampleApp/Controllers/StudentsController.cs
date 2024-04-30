using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.WireProtocol.Messages;
using MongoDbDriverSampleApp.Documents;
using MongoDbDriverSampleApp.Dtos;
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

      // many to many relations select sorgusu
      // userları teacherları ile birlikte getirdik.
      // $TeacherIds tabloadaki FK alanına bağlandığımız ad
      // teacherIds uygulama içerisinde kullandığımız içerisinde FK gelen değerleri In sorgıusuna soktuğumuz değişken.
      var pipeline = new BsonDocument[]
      {
        new BsonDocument("$lookup", new BsonDocument()
        {
          { "from","teachers" },
          { "let", new BsonDocument("teacherIds","$TeacherIds") },
          { "pipeline", new BsonArray
          {
            new BsonDocument("$match", new BsonDocument()
            {
              {"$expr", new BsonDocument("$in", new BsonArray
                {
                 "$_id","$$teacherIds"
                })
              }
            })
          } 
          },
          {"as","teachers" } // document içerisine teachers olarak getir.
        }),
             // $map ile dizi içerisinde dönüp tek tek itemları güncelleyebiliriz. 
                  // $addFields ile yeni bir alan açtık 
        new BsonDocument("$addFields", new BsonDocument
        {
          { "Teachers", new BsonDocument("$map", new BsonDocument
          {
            { "input","$teachers" }, // dizinin mapten önceki hali
            {"as", "teacher" }, // dizideki her bir item
            {"in", new BsonDocument // dizinin içine girip tek tek değerleri transform edebiliriz
            {
              {"_id", new BsonDocument("$toString","$$teacher._id") },
              {"Name", "$$teacher.Name" }
            }}
          })
          }
        }),
        new BsonDocument("$project", new BsonDocument
        {
          {"_id", new BsonDocument("$toString","$_id") },
          {"Name",1 },
          {"ClassName",1 },
          {"Teachers", "$Teachers"}
         
        })
      };


      var doc = await studentCollection.Aggregate<StudentWithTeacherDto>(pipeline).ToListAsync();

      return Ok(doc);
    }
  }
}
