# 🍽️ RestaurantsBack - Dish Image Upload Integration

## 📋 მიმოხილვა

ეს სოლუშენი ამატებს სურათის ატვირთვის ფუნქციონალს `AddDish` მეთოდში Clean Architecture პრინციპების დაცვით. სურათები ინახება **Cloudinary**-ში და დაბრუნებული `imageUrl` ჩაიწერება ბაზაში.

---

## 🏗️ არქიტექტურა

```
Product Service
├── Product.API (Presentation Layer)
│   ├── Controllers/DishController.cs
│   └── Program.cs
│
├── Product.Application (Application Layer)
│   ├── Features/
│   │   └── Dishes/
│   │       └── Commands/AddDish/
│   │           ├── AddDishCommand.cs
│   │           ├── AddDishCommandHandler.cs
│   │           └── AddDishCommandValidator.cs
│   └── Common/Interfaces/
│       └── ICloudinaryService.cs
│
└── Product.Infrastructure (Infrastructure Layer)
    ├── Services/CloudinaryService.cs
    ├── Settings/CloudinarySettings.cs
    └── DependencyInjection.cs
```

---

## 📦 საჭირო NuGet პაკეტები

### Product.Infrastructure:
```bash
cd src/Services/Product/Product.Infrastructure
dotnet add package CloudinaryDotNet --version 2.0.0
```

### Product.Application (თუ არ არის):
```bash
cd src/Services/Product/Product.Application
dotnet add package FluentValidation --version 11.9.0
dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.9.0
dotnet add package MediatR --version 12.2.0
```

---

## ⚙️ კონფიგურაცია

### 1. appsettings.json

```json
{
  "CloudinarySettings": {
    "CloudName": "თქვენი-cloud-name",
    "ApiKey": "თქვენი-api-key",
    "ApiSecret": "თქვენი-api-secret"
  }
}
```

**Cloudinary Account-ის შექმნა:**
1. გადადით: https://cloudinary.com/
2. დარეგისტრირდით უფასო Account-ზე
3. Dashboard-დან აიღეთ: Cloud Name, API Key, API Secret

---

### 2. Program.cs

```csharp
using Product.Application;
using Product.Infrastructure;
using Product.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Application Layer (MediatR, FluentValidation)
builder.Services.AddApplication();

// Infrastructure Layer (Cloudinary Service) ✅ ეს დაამატეთ!
builder.Services.AddInfrastructure(builder.Configuration);

// Persistence Layer (DbContext, Repositories)
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
```

---

## 📝 ფაილების სტრუქტურა

### 1. **DishController.cs** (Product.API/Controllers)

```csharp
[ApiController]
[Route("api/[controller]")]
public class DishController : ControllerBase
{
    private readonly IMediator _mediator;

    public DishController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// დაამატე ახალი კერძი სურათით
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")] // ✅ აუცილებელია!
    public async Task<IActionResult> AddDish(
        [FromForm] AddDishCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetDishById), 
            new { id = result.Value.Id }, 
            result.Value
        );
    }
}
```

---

### 2. **AddDishCommand.cs** (Product.Application)

```csharp
using MediatR;
using Microsoft.AspNetCore.Http;
using Product.Application.Common.Models;
using Product.Application.DTOs;

namespace Product.Application.Features.Dishes.Commands.AddDish;

public record AddDishCommand : IRequest<Result<DishResponseDto>>
{
    public string NameKa { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string DescriptionKa { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public decimal? Price { get; init; }
    public Guid DishCategoryId { get; init; }
    public int? PreparationTimeMinutes { get; init; }
    public int? Calories { get; init; }
    public int? SpicyLevel { get; init; }
    public string Ingredients { get; init; } = string.Empty;
    public string IngredientsEn { get; init; } = string.Empty;
    public string Volume { get; init; } = string.Empty;
    public string AlcoholContent { get; init; } = string.Empty;
    public bool IsVeganDish { get; init; }
    public string Comment { get; init; } = string.Empty;
    
    public IFormFile? ImageFile { get; init; } // ✅ სურათის ფაილი
    public string? VideoUrl { get; init; }
}
```

---

### 3. **AddDishCommandHandler.cs** (Product.Application)

**მთავარი ლოგიკა:**
1. ამოწმებს DishCategory-ს არსებობას
2. ვალიდაცია სურათის ფორმატის/ზომის
3. ატვირთავს სურათს Cloudinary-ში
4. იღებს `imageUrl`-ს
5. ქმნის Dish entity-ს
6. ინახავს ბაზაში

სრული კოდი იხილეთ `AddDishCommandHandler.cs` ფაილში.

---

### 4. **ICloudinaryService.cs** (Product.Application/Common/Interfaces)

```csharp
using Microsoft.AspNetCore.Http;

namespace Product.Application.Common.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(
        IFormFile file, 
        string folder, 
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteImageAsync(
        string publicId, 
        CancellationToken cancellationToken = default
    );
}
```

---

### 5. **CloudinaryService.cs** (Product.Infrastructure/Services)

```csharp
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Product.Application.Common.Interfaces;
using Product.Infrastructure.Settings;

namespace Product.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings)
    {
        var settings = cloudinarySettings.Value;
        var account = new Account(
            settings.CloudName,
            settings.ApiKey,
            settings.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(
        IFormFile file, 
        string folder, 
        CancellationToken cancellationToken = default)
    {
        using var stream = file.OpenReadStream();
        
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            Transformation = new Transformation()
                .Width(800)
                .Height(600)
                .Crop("limit")
                .Quality("auto")
                .FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(
            uploadParams, 
            cancellationToken
        );

        if (uploadResult.Error != null)
        {
            throw new Exception(
                $"Cloudinary upload failed: {uploadResult.Error.Message}"
            );
        }

        return uploadResult.SecureUrl.ToString();
    }

    public async Task<bool> DeleteImageAsync(
        string publicId, 
        CancellationToken cancellationToken = default)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }
}
```

---

### 6. **DependencyInjection.cs** (Product.Infrastructure)

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Application.Common.Interfaces;
using Product.Infrastructure.Services;
using Product.Infrastructure.Settings;

namespace Product.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Cloudinary Settings
        services.Configure<CloudinarySettings>(
            configuration.GetSection(CloudinarySettings.SectionName)
        );

        // Cloudinary Service
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        return services;
    }
}
```

---

## 🧪 ტესტირება

### Postman-ით:

1. **Method:** `POST`
2. **URL:** `https://localhost:7001/api/Dish`
3. **Body:** `form-data`

| Key | Value | Type |
|-----|-------|------|
| NameKa | ხაჭაპური | Text |
| NameEn | Khachapuri | Text |
| DescriptionKa | ქართული ტრადიციული კერძი | Text |
| DescriptionEn | Georgian traditional dish | Text |
| Price | 15.50 | Text |
| DishCategoryId | your-category-guid | Text |
| ImageFile | [Select File] | File |
| IsVeganDish | false | Text |

---

### React TypeScript-ით:

იხილეთ `AddDishForm.tsx` ფაილი სრული იმპლემენტაციისთვის.

**მოკლედ:**
```typescript
const formData = new FormData();
formData.append('NameKa', 'ხაჭაპური');
formData.append('NameEn', 'Khachapuri');
formData.append('Price', '15.50');
formData.append('DishCategoryId', categoryId);
formData.append('IsVeganDish', 'false');

// სურათი
if (imageFile) {
  formData.append('ImageFile', imageFile);
}

const response = await fetch('https://your-api.com/api/Dish', {
  method: 'POST',
  body: formData,
});

const result = await response.json();
console.log('Image URL:', result.imageUrl);
```

---

## ✅ მთავარი ფიჩები

✨ **Multipart/Form-Data** - სურათის ატვირთვა ფორმით
✨ **Cloudinary Integration** - ავტომატური ატვირთვა cloud-ში
✨ **Validation** - ფაილის ტიპის და ზომის შემოწმება
✨ **Clean Architecture** - სწორი layer separation
✨ **Error Handling** - დეტალური error messages
✨ **Image Optimization** - ავტომატური resize/compression

---

## 🛡️ ვალიდაციის წესები

- **Supported formats:** .jpg, .jpeg, .png, .gif, .webp
- **Max file size:** 5MB
- **Auto transformation:** 800x600px, quality: auto
- **NameKa/NameEn:** Required, max 200 chars
- **Description:** Max 1000 chars
- **Price:** >= 0
- **SpicyLevel:** 0-5

---

## 🔧 Troubleshooting

### პრობლემა: "Invalid Signature" error
**გადაწყვეტა:** შეამოწმეთ CloudinarySettings appsettings.json-ში

### პრობლემა: "Content-Type" არასწორია
**გადაწყვეტა:** დარწმუნდით რომ Controller-ში არის `[Consumes("multipart/form-data")]`

### პრობლემა: სურათი არ აიტვირთება
**გადაწყვეტა:** შეამოწმეთ:
- ფაილის ზომა (< 5MB)
- ფაილის ფორმატი (.jpg, .png, etc.)
- Cloudinary credentials

---

## 📚 დამატებითი რესურსები

- [Cloudinary .NET SDK Docs](https://cloudinary.com/documentation/dotnet_integration)
- [ASP.NET Core File Upload](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/)

---

## 👨‍💻 შენიშვნები

მოცემული კოდი სრულად შეესაბამება შენი პროექტის არქიტექტურას და მზად არის გამოყენებისთვის. 

**Clean Architecture Layers:**
- ✅ API Layer - Controller მხოლოდ HTTP requests-ს ამუშავებს
- ✅ Application Layer - Business logic და Validation
- ✅ Infrastructure Layer - Cloudinary integration
- ✅ No circular dependencies!

წარმატებებს! 🎉
