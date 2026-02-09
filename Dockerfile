# Stage 1: Build Frontend
FROM node:20-alpine AS frontend-build
WORKDIR /app/frontend
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci
COPY frontend/ ./
RUN npm run build

# Stage 2: Build Backend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
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

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=backend-build /app/publish ./
COPY --from=frontend-build /app/frontend/dist ./wwwroot
ENV ASPNETCORE_URLS=http://+:${PORT:-10000}
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000
ENTRYPOINT ["dotnet", "Product.API.dll"]
