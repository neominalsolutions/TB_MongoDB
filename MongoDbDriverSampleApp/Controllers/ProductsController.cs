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

    [HttpGet("queries")]
    public async Task<IActionResult> Queries(int pageIndex)
    {

      var filter = Builders<Product>.Filter; // Product Collection Filter

      // İsmi eşit olan
      var q1 = productCollection.Find(filter.Eq("Name", "Test22")).ToListAsync();
      // like sorgusu örneği
      var q2 = productCollection.Find(filter.Regex("Name", "/test/i")).ToListAsync();
      // Price 10 between 100
      var q3 = productCollection.Find(filter.And(filter.Gte("unitPrice", 10), filter.Lte("unitPrice", 100))).ToListAsync();
      // sortlama işlemi

      var sortByNameAscAndPriceDesc = Builders<Product>.Sort.Ascending("Name").Descending("unitPrice");
      var q4 = productCollection.Find(x => true).Sort(sortByNameAscAndPriceDesc).ToListAsync();

      // sayfalama işlemleri için
      // fiyatı 10 dan büyük olanları sayfalama yap
      var q5 = productCollection.Find(filter.Lt("unitPrice", 10)).Skip((pageIndex -1 * 10)).Limit(10).ToListAsync();

      // grouplama işlemlerinde pipeline kullanıyoruz yada linq ile de yapabiliriz.
      // hangi kategoride kaç adet ürün var linq.

      var q6 = productCollection.AsQueryable().GroupBy(x => x.CategoryId).Select(a => new
      {
         Count = a.Count(),
         CategoryName  = a.Key.ToString()
      }).ToList();

      // kategorilerine göre maliyetlerinide hesaplatmak istersem
      // kategorisine göre ortalama maaliyet
      var groupPipeline = new BsonDocument[]
      {
        new BsonDocument("$project",new BsonDocument()
        {
          {"_id", new BsonDocument("$toString","$_id") },
          {"Name",1 },
          {"unitPrice",1 },
          {"Stock",1 },
          {"CategoryId", new BsonDocument("$toString","$CategoryId") }
        }),
        new BsonDocument("$group", new BsonDocument()
        {
          {"_id","$CategoryId"},
          {"TotalCount", new BsonDocument("$sum",1) },
          {"AvgPrice", new BsonDocument("$avg","$unitPrice") }, // kategori bazlı ortalama birim fiyat
          { "TotalCost", new BsonDocument("$sum", new BsonDocument("$multiply", new BsonArray {"$unitPrice","$Stock"})) }

        })
      };

     var res = await productCollection.Aggregate<ProductsByCategoryDto>(groupPipeline).ToListAsync();



      return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      #region Yöntem1
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

      #endregion

      #region Yöntem2

      var response = (from p in productCollection.AsQueryable()
                   join c in categoryCollection.AsQueryable()
                  on p.CategoryId equals c.Id
                   select new ProductDto
                   {
                     Id = p.Id.ToString(),
                     CategoryId = c.Id.ToString(),
                     Category = new CategoryDto
                     {
                       Name = c.Name
                     },
                     Name = p.Name,
                     Price = p.Price,
                     Stock = p.Stock
                   }).ToList();

      #endregion

      // en hızlı veri çekme yöntemi

      #region Yöntem3


      var pipeline = new BsonDocument[]
      {
        new BsonDocument("$lookup", new BsonDocument()
        {
          new BsonDocument("from","categories"),
          new BsonDocument("localField","CategoryId"),
          new BsonDocument("foreignField","_id"),
          new BsonDocument("as","category")
        }),
        new BsonDocument("$addFields", new BsonDocument() // döndürülecek result için yeni bir alan aç ismine Category de
        {
          new BsonDocument("Category", new BsonDocument("$first","$category"))
        }),
        new BsonDocument("$unwind","$category"), // nesneyi diziden obje formatına çevir
        new BsonDocument("$project", new BsonDocument()
        {
          { "_id", new BsonDocument("$toString","$_id") },
          { "Name",1},
          { "Price",1 },
          { "Category", new BsonDocument()
            {
              { "CategoryId",new BsonDocument("$toString","$category._id") },
              { "Name","$category.Name" }
            }
          }
        })
      };

      var response2 = await  productCollection.Aggregate<ProductDto>(pipeline).ToListAsync();

      #endregion

      return Ok(response2);
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


      //await productCollection.FindOneAndReplaceAsync(x => x.Id == updateProduct.Id, updateProduct);

      var updateDefination = new UpdateDefinitionBuilder<Product>().Set("Name", dto.Name).Set("unitPrice", dto.Price);

      // Upsert = false sadece update eder.
      await productCollection.UpdateOneAsync(x => x.Id == ObjectId.Parse(Id), updateDefination, new UpdateOptions { IsUpsert = true });

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
