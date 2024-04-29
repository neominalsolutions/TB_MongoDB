using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace MongoDbDriverSampleApp.Dtos
{
  public class CreateProductDto
  {
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("unitsinPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("unitsStock")]
    public int Stock { get; set; }

    [JsonPropertyName("categoryId")]
    public string CategoryId { get; set; }

  }
}
