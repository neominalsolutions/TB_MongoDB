﻿namespace MongoDbDriverSampleApp.Settiings
{
  public interface IMongoDbSettings
  {
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }

  }

  public class MongoDbSettings : IMongoDbSettings
  {
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }


  }
}
