using Microsoft.Extensions.Options;
using MongoDbDriverSampleApp.Settiings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// uygulama genelinde kullanýlacaðý için singleton tanýmladýk.
builder.Services.AddSingleton<IMongoDbSettings>(serviceprovider => serviceprovider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

//builder.Services.AddSingleton<IMongoDbSettings, MongoDbSettings>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
