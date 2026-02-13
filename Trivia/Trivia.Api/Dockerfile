# Build stage
FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0 AS build
WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Trivia.Api.dll"]
