# საჭირო NuGet პაკეტები

## Product.Infrastructure პროექტისთვის:

```bash
dotnet add package CloudinaryDotNet --version 2.0.0
```

## Product.Application პროექტისთვის (თუ არ არის დაინსტალირებული):

```bash
dotnet add package FluentValidation --version 11.9.0
dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.9.0
dotnet add package MediatR --version 12.2.0
```

## Product.API პროექტისთვის:

```bash
# ეს უკვე უნდა იყოს, მაგრამ თუ არა:
dotnet add package Microsoft.AspNetCore.OpenApi --version 9.0.0
dotnet add package Swashbuckle.AspNetCore --version 7.2.0
```

---

# პროექტის სტრუქტურა

```
src/
├── Services/
│   └── Product/
│       ├── Product.API/
│       │   ├── Controllers/
│       │   │   └── DishController.cs
│       │   ├── Program.cs
│       │   └── appsettings.json
│       │
│       ├── Product.Application/
│       │   ├── Features/
│       │   │   └── Dishes/
│       │   │       └── Commands/
│       │   │           └── AddDish/
│       │   │               ├── AddDishCommand.cs
│       │   │               ├── AddDishCommandHandler.cs
│       │   │               └── AddDishCommandValidator.cs
│       │   ├── Common/
│       │   │   └── Interfaces/
│       │   │       └── ICloudinaryService.cs
│       │   └── DTOs/
│       │       └── DishResponseDto.cs
│       │
│       └── Product.Infrastructure/
│           ├── Services/
│           │   └── CloudinaryService.cs
│           ├── Settings/
│           │   └── CloudinarySettings.cs
│           └── DependencyInjection.cs
```

---

# როგორ გამოვიყენოთ

## 1. appsettings.json-ში დაამატეთ Cloudinary პარამეტრები:

```json
{
  "CloudinarySettings": {
    "CloudName": "თქვენი-cloud-name",
    "ApiKey": "თქვენი-api-key",
    "ApiSecret": "თქვენი-api-secret"
  }
}
```

## 2. Program.cs-ში დარეგისტრირეთ სერვისები:

```csharp
// Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);
```

## 3. Frontend-დან (Postman/React) გაგზავნეთ Request:

### Postman-ში:
- Method: `POST`
- URL: `https://your-api.com/api/Dish`
- Body: `form-data`
  - NameKa: "ხაჭაპური"
  - NameEn: "Khachapuri"
  - DescriptionKa: "ქართული ტრადიციული კერძი"
  - DescriptionEn: "Georgian traditional dish"
  - Price: 15.50
  - DishCategoryId: "guid-here"
  - ImageFile: [select file]
  - IsVeganDish: false
  - და ა.შ.

### React-დან (TypeScript):

```typescript
const addDish = async (dishData: FormData) => {
  const response = await fetch('https://your-api.com/api/Dish', {
    method: 'POST',
    body: dishData, // FormData ობიექტი
  });

  if (!response.ok) {
    throw new Error('Failed to add dish');
  }

  return await response.json();
};

// გამოყენება:
const formData = new FormData();
formData.append('NameKa', 'ხაჭაპური');
formData.append('NameEn', 'Khachapuri');
formData.append('DescriptionKa', 'ქართული ტრადიციული კერძი');
formData.append('DescriptionEn', 'Georgian traditional dish');
formData.append('Price', '15.50');
formData.append('DishCategoryId', 'your-category-guid');
formData.append('IsVeganDish', 'false');

// სურათი
const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
if (fileInput?.files?.[0]) {
  formData.append('ImageFile', fileInput.files[0]);
}

await addDish(formData);
```

---

# მნიშვნელოვანი დეტალები

1. **Multipart/Form-Data**: Controller-ში უნდა იყოს `[Consumes("multipart/form-data")]` ატრიბუტი
2. **IFormFile**: Command-ში გამოვიყენეთ `IFormFile?` სურათის მისაღებად
3. **Validation**: FluentValidation ავტომატურად ამოწმებს ფაილის ფორმატს და ზომას
4. **Cloudinary**: სურათი ავტომატურად აიტვირთება და დაბრუნდება `imageUrl`
5. **Clean Architecture**: ყველა layer სწორად არის გამიჯნული და დამოუკიდებელი

---

# რას აკეთებს Handler:

1. ამოწმებს არსებობს თუ არა DishCategory
2. ვალიდაცია სურათის ფორმატის და ზომის
3. ატვირთავს სურათს Cloudinary-ში
4. იღებს imageUrl-ს Cloudinary-დან
5. ქმნის Dish entity-ს imageUrl-ით
6. ინახავს ბაზაში
7. აბრუნებს DishResponseDto-ს

ასე რომ, სრულყოფილად არის დაცული Clean Architecture პრინციპები!
