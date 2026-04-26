FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EnglishSeedEngine.sln ./
COPY src/EnglishSeedEngine.Api/EnglishSeedEngine.Api.csproj src/EnglishSeedEngine.Api/
COPY src/EnglishSeedEngine.Application/EnglishSeedEngine.Application.csproj src/EnglishSeedEngine.Application/
COPY src/EnglishSeedEngine.Domain/EnglishSeedEngine.Domain.csproj src/EnglishSeedEngine.Domain/
COPY src/EnglishSeedEngine.Infrastructure/EnglishSeedEngine.Infrastructure.csproj src/EnglishSeedEngine.Infrastructure/

RUN dotnet restore src/EnglishSeedEngine.Api/EnglishSeedEngine.Api.csproj

COPY . .
RUN dotnet publish src/EnglishSeedEngine.Api/EnglishSeedEngine.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EnglishSeedEngine.Api.dll"]
