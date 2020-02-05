# builder
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine3.10 AS build
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# run
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine3.10
WORKDIR /app
COPY --from=build /app .
CMD ["dotnet", "kestrel.dll"]