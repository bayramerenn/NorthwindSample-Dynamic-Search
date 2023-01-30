using Core.Security;
using Microsoft.EntityFrameworkCore;
using NorthwindSample.Encrypt;
using NorthwindSample.Models;
using NorthwindSample.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        var enumConverter = new JsonStringEnumConverter();
        options.JsonSerializerOptions.Converters.Add(enumConverter);
    }).ConfigureApiBehaviorOptions(opt => opt.SuppressModelStateInvalidFilter = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
    //options.AddPolicy("AllowSpecificOrigins",
    //    builder =>
    //    {
    //        List<CorsOrigin> origins = new List<CorsOrigin>();
    //        configuration.GetSection("cors:origins").Bind(origins);
    //        foreach (var o in origins)
    //        {
    //            builder.WithOrigins(o.Uri);
    //        }
    //        builder
    //            .AllowAnyMethod()
    //            .AllowAnyHeader()
    //            .AllowCredentials();
    //    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<NorthwindContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));
builder.Services.AddScoped<IEncryption, Encryption>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.Configure<NrtConfig>(builder.Configuration.GetSection(nameof(NrtConfig)));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAnyOrigin");
app.UseAuthorization();

app.MapControllers();

app.Run();