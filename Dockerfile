# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY RestaurantsBack.Product.sln ./
COPY src/Services/Product/Product.API/Product.API.csproj src/Services/Product/Product.API/
COPY src/Services/Product/Product.Application/Product.Application.csproj src/Services/Product/Product.Application/
COPY src/Services/Product/Product.Domain/Product.Domain.csproj src/Services/Product/Product.Domain/
COPY src/Services/Product/Product.Infrastructure/Product.Infrastructure.csproj src/Services/Product/Product.Infrastructure/
COPY src/Services/Product/Product.Persistence/Product.Persistence.csproj src/Services/Product/Product.Persistence/
RUN dotnet restore
COPY src/ src/
RUN dotnet publish src/Services/Product/Product.API/Product.API.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:${PORT:-10000}
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000
ENTRYPOINT ["dotnet", "Product.API.dll"]
