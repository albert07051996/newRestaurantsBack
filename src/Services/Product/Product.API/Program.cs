using Microsoft.EntityFrameworkCore;
using Product.Application;
using Product.Infrastructure;
using Product.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Application Layer (MediatR, FluentValidation)
builder.Services.AddApplication();

// Add Infrastructure Layer (Cloudinary Service)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Persistence Layer (DbContext, Repositories)
builder.Services.AddPersistence(builder.Configuration);

//builder.Services.AddDbContext<ProductDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Product API", Version = "v1" });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
