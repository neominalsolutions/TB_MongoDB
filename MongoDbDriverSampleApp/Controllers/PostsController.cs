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


      // post document içerisinde user Name can ile başlayan postları getir.

      var filter2 = Builders<Post>.Filter.Regex(x => x.User.Name, "/^can/i");

      var doc2 = await postCollection.Find(filter2).ToListAsync();

      // post documanlarında Tag3 ve Tag1 etkiletine sahip postlar.
      // AnyIn veya gibi çalışır, Tag3 veya Tag1 olması yeterlidir
      // All and gibi çalışır, Tag1 ve Tag3 alt düğümde olmak zorundadır. eşleşenleri getirir.
      var filter3 = Builders<Post>.Filter.All(x => x.Tags, new List<string> { "Tag3", "Tag1" });
      var doc3 = await postCollection.Find(filter3).ToListAsync();

      // Post Commentsleri içerisindeki User Objecnin name alanı ali olan kayıtları getir. alinin yorum yaptığı postları getir.

      var filter4 = Builders<Post>.Filter.ElemMatch(x => x.Comments, comment => comment.CommentUser.Name == "ali");

      var doc4 = await postCollection.Find(filter4).ToListAsync();

      return Ok(doc4);
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
      // embded document insert sample

      var post = new Post();
      post.Title = "Post-3";
      post.Content = "Test-3";
      post.Tags = new List<string> { "Tag14","Tag23", "Tag3"};
      post.User = new User { Name = "mert" };
      post.Comments = new List<Comment>
      {
        new Comment
        {
          Text = "Comment-3",
          CreatedAt = DateTime.Now,
          CommentUser = new User {Name = "Can"}
        }
      };

     await  postCollection.InsertOneAsync(post);
     

      return Ok();
    }
  }
}
