using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbDriverSampleApp.Documents;
using MongoDbDriverSampleApp.Documents.Posts;
using MongoDbDriverSampleApp.Settiings;

namespace MongoDbDriverSampleApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PostsController : ControllerBase
  {
    // Sadece Collectionlar üzerinden veriye erişebilir.
    // Repository pattern implementasyonuda collecksiyonları kullanıcağız.
    // Meded Documentlara sadece Collection üzerinden müdehale edebiliriz.
    private readonly IMongoCollection<Post> postCollection;


    public PostsController(IMongoDbSettings settings)
    {
      var client = new MongoClient(settings.ConnectionString);
      var db = client.GetDatabase(settings.DatabaseName);
      postCollection = db.GetCollection<Post>("posts");
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {

      var filter = Builders<Post>.Filter.Eq("Id", ObjectId.Parse("6630b8fcf642c6cafb472a3f"));

      var doc = await postCollection.Find(filter).ToListAsync();

      return Ok(doc);
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
      var post = new Post();
      post.Title = "Post-1";
      post.Content = "Test";
      post.Tags = new List<string> { "Tag1","Tag2", "Tag3"};
      post.User = new User { Name = "ali" };
      post.Comments = new List<Comment>
      {
        new Comment
        {
          Text = "Comment-1",
          CreatedAt = DateTime.Now,
          CommentUser = new User {Name = "Can"}
        }
      };

     await  postCollection.InsertOneAsync(post);
     

      return Ok();
    }
  }
}
