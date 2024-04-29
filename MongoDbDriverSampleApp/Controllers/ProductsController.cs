using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbDriverSampleApp.Documents;
using MongoDbDriverSampleApp.Dtos;
using MongoDbDriverSampleApp.Settiings;

namespace MongoDbDriverSampleApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductsController : ControllerBase
  {
    private readonly IMongoCollection<Product> productCollection;
    private readonly IMongoCollection<Category> categoryCollection;
    public ProductsController(IMongoDbSettings mongoDbSettings)
    {
      var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
      var db = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);
      this.productCollection = db.GetCollection<Product>("products");
      this.categoryCollection = db.GetCollection<Category>("categories");
      //var session = mongoClient.StartSession();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var products = await productCollection.Find(x => true).Project(x=> new ProductDto
      {
        Id = x.Id.ToString(),
        Name = x.Name,
        Price = x.Price,
        Stock = x.Stock,
        CategoryId = x.CategoryId.ToString()
      }).ToListAsync();

      products.ForEach((item =>
      {

        var cId = new MongoDB.Bson.ObjectId(item.CategoryId);

        item.Category = categoryCollection.Find(y => y.Id == cId).Project(p => new CategoryDto
        {
          Name = p.Name,
          CategoryId = p.Id.ToString()
        }).FirstOrDefault();
      }));

      return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
      #region Test
      //var category = new Category
      //{
      //  Name = "Kategori-1"
      //};

      //var product = new Product
      //{
      //  Name = "P1",
      //  Stock = 10,
      //  Price = 30,
      //  CategoryId = category.Id
      //}
      //this.categoryCollection.InsertOne(category);
      #endregion

      var product = new Product
      {
        Name = dto.Name,
        Price = dto.UnitPrice,
        Stock = dto.Stock,
        CategoryId = new MongoDB.Bson.ObjectId(dto.CategoryId) // arayüzden koleksiyona gönderirken Idler Objectıd formatında olmalıdır
      };


     await  this.productCollection.InsertOneAsync(product);




      return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(string Id, [FromBody] UpdateProductDto dto)
    {
      var updateProduct = new Product
      {
        Id = new ObjectId(Id),
        Name = dto.Name,
        Price = dto.Price
      };


      await productCollection.FindOneAndReplaceAsync(x => x.Id == updateProduct.Id, updateProduct);

      return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string Id)
    {
      var _id = new ObjectId(Id);

      // find and delete yapar
      var product = await productCollection.DeleteOneAsync(x => x.Id == _id);

      return Ok();
    }
  }
}
