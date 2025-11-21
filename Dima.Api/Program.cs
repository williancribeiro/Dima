using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dima.Api.Data;
using Dima.Core.Models;

var builder = WebApplication.CreateBuilder(args);

var cnnStr = builder
    .Configuration
    .GetConnectionString("DefaultConnection") ?? string.Empty;

builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        options.UseSqlServer(cnnStr);
    });


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(n => n.FullName);
});

builder.Services.AddTransient<Handler>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost(
    "/v1/categories",
    (Request request, Handler handler) 
    => handler.Handle(request))
    .WithName("Categories: Create")
    .WithSummary("Criar uma nova categoria")
    .Produces<Response>();

app.Run();

// Request
public class Request
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

// Response
public class Response
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

//Handler
public class Handler(AppDbContext Context)
{
    public Response Handle(Request request)
    {
        var Category = new Category
        {
            Title = request.Title,
            Description = request.Description
        };

        Context.Categories.Add(Category);
        Context.SaveChanges();

        return new Response
        {
            Id = Category.Id,
            Title = Category.Title
        };
    }
} 