FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "FoodOptimizer.API/FoodOptimizer.API.csproj"
RUN dotnet build "FoodOptimizer.API/FoodOptimizer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodOptimizer.API/FoodOptimizer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodOptimizer.API.dll"]