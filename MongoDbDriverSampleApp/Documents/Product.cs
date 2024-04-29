using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace MongoDbDriverSampleApp.Documents
{
  public class Product
  {
    public ObjectId Id { get; set; } // PK ve FK alanlar ObjectId olmalıdır. Index için önemli
    public string Name { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    [BsonElement("unitPrice")] // veritabanında bsonElement olarak tut
    public decimal Price { get; set; }

    [BsonRepresentation(BsonType.Int32)]
    public int Stock { get; set; }

    public ObjectId CategoryId { get; set; } // Fk alan


    public Product()
    {
      Id = ObjectId.GenerateNewId();
    }

  }
}
