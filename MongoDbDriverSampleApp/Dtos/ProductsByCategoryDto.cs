namespace MongoDbDriverSampleApp.Dtos
{
  public class ProductsByCategoryDto
  {
    public string Id { get; set; }
    public decimal TotalCost { get; set; }
    public decimal AvgPrice { get; set; }
    public int TotalCount { get; set; }

  }
}
