
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using TodoApi;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// הוסף שירותים
builder.Services.AddTransient<MySqlConnection>(sp => 
    new MySqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
    
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
}));
var app = builder.Build();
app.UseCors("MyPolicy");
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
//}
app.MapGet("/",()=>"helllllo");
app.MapGet("/items", async (MySqlConnection db) =>
{
    System.Console.WriteLine(db.ConnectionString+"____________________________");
    var results = await db.QueryAsync<Item>("SELECT * FROM bb0ptnvqqvjhjdloytri.Items");
    return Results.Ok(results);
});

app.MapGet("/items/{id}", async (MySqlConnection db,int id) =>
{
    var result = await db.QueryFirstOrDefaultAsync<Item>("SELECT * FROM bb0ptnvqqvjhjdloytri.Items WHERE id = @id",new {id});
    return result!= null? Results.Ok(result) : Results.NotFound();
});

app.MapPost("/items", async (MySqlConnection db, string name) =>
{
    System.Console.WriteLine("mmm...\n");
    var id = await db.ExecuteScalarAsync<int>("INSERT INTO bb0ptnvqqvjhjdloytri.Items (name) VALUES (@name)", new {name});
    return Results.Created($"/items/{id}", name);
});

app.MapPut("/items/{id}", async (MySqlConnection db,int id, bool IsComplete) =>
{

    var rowsAffected = await db.ExecuteAsync("UPDATE bb0ptnvqqvjhjdloytri.Items SET IsComplete=@IsComplete WHERE Id=@Id", new{id,IsComplete});
    return rowsAffected > 0? Results.NoContent() : Results.NotFound();
   
});

app.MapDelete("/items/{id}", async (MySqlConnection db,int id) =>
{
    var rowsAffected = await db.ExecuteAsync("DELETE FROM bb0ptnvqqvjhjdloytri.Items WHERE Id=@Id", new { id });
    return rowsAffected > 0? Results.NoContent() : Results.NotFound();
});
app.Run();



