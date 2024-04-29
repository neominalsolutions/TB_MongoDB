﻿namespace MongoDbDriverSampleApp.Dtos
{
  public class ProductDto
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string CategoryId { get; set; }


    public CategoryDto? Category { get; set; }

  }
}
