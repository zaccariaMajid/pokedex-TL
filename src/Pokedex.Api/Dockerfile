FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Pokedex.sln ./
COPY src/Pokedex.Api/Pokedex.Api.csproj src/Pokedex.Api/
COPY src/Pokedex.Application/Pokedex.Application.csproj src/Pokedex.Application/
COPY src/Pokedex.Domain/Pokedex.Domain.csproj src/Pokedex.Domain/
COPY src/Pokedex.Infrastructure/Pokedex.Infrastructure.csproj src/Pokedex.Infrastructure/
COPY tests/Pokedex.Tests/Pokedex.Tests.csproj tests/Pokedex.Tests/

RUN dotnet restore Pokedex.sln

COPY src/ src/

RUN dotnet publish src/Pokedex.Api/Pokedex.Api.csproj -c Release -o /app/publish --no-restore

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Pokedex.Api.dll"]