using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbDriverSampleApp.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace MongoDbDriverSampleApp.Documents
{
  // Bütün Collectionlar Repository Pattern Kullanmak için Document Base classtan kalıtım almalıdır.
  // Embeded olanlar documan olmadığı için gerek yok.
  public class Product:Document
  {
   
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
