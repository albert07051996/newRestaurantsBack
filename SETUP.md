# 🚀 Quick Setup Guide

## 1. Prerequisites
- .NET 9.0 SDK
- PostgreSQL 16+
- Visual Studio 2022 / JetBrains Rider / VS Code
- Cloudinary Account (free tier available at https://cloudinary.com)

## 2. Installation Steps

### Step 1: Clone/Extract Project
```bash
cd RestaurantsBack-Product
```

### Step 2: Restore NuGet Packages
```bash
dotnet restore
```

### Step 3: Configure Database
1. Open `src/Services/Product/Product.API/appsettings.json`
2. Update ConnectionStrings:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=RestaurantsProduct;Username=postgres;Password=YOUR_PASSWORD"
}
```

### Step 4: Configure Cloudinary
1. Sign up at https://cloudinary.com (free)
2. Get your credentials from Dashboard
3. Update `appsettings.json`:
```json
"CloudinarySettings": {
  "CloudName": "your-cloud-name",
  "ApiKey": "your-api-key",
  "ApiSecret": "your-api-secret"
}
```

### Step 5: Create Database & Run Migrations
```bash
cd src/Services/Product/Product.API

# Create migration
dotnet ef migrations add InitialCreate --project ../Product.Persistence

# Apply migration
dotnet ef database update --project ../Product.Persistence
```

### Step 6: Run the Application
```bash
dotnet run
```

API will be available at: https://localhost:7001
Swagger UI: https://localhost:7001/swagger

## 3. Testing with Postman

### Add Dish with Image
1. Method: `POST`
2. URL: `https://localhost:7001/api/Dish`
3. Body: `form-data`
   - NameKa: "ხაჭაპური"
   - NameEn: "Khachapuri"
   - Price: 15.50
   - DishCategoryId: "{your-category-guid}"
   - ImageFile: [Select File]
   - IsVeganDish: false

## 4. Frontend Setup (React)
```bash
cd frontend
npm install
npm start
```

## 5. Common Issues

### Error: "Invalid Signature" from Cloudinary
- Check CloudinarySettings in appsettings.json
- Verify API Secret is correct

### Error: Database connection failed
- Verify PostgreSQL is running
- Check connection string credentials

### Error: Migration already exists
```bash
dotnet ef migrations remove --project ../Product.Persistence
```

## 6. Project Structure
```
RestaurantsBack-Product/
├── src/
│   └── Services/
│       └── Product/
│           ├── Product.API         → Controllers, Program.cs
│           ├── Product.Application → CQRS, Commands, Queries
│           ├── Product.Domain      → Entities
│           ├── Product.Infrastructure → Cloudinary Service
│           └── Product.Persistence → EF Core, DbContext
├── frontend/                       → React TypeScript
├── README.md
└── INSTRUCTIONS.md
```

## 7. Next Steps
- Read full documentation in README.md
- Check INSTRUCTIONS.md for detailed usage
- Add seed data for DishCategories
- Implement Update/Delete endpoints

## 🎉 Done!
Your application is ready to use with image upload functionality!
