FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# copy all .csproj files and restore as distinct layers. Use of the same COPY command for every dockerfile
# in the project to take advantage of docker caching
COPY AutoBidXchange.sln ./
COPY src/NotificationService/NotificationService.csproj src/NotificationService/NotificationService.csproj
COPY src/Contracts/Contracts.csproj src/Contracts/Contracts.csproj

# Restore package steps
RUN dotnet restore ./src/NotificationService/NotificationService.csproj

# Copy the app folders folder
COPY . .
RUN dotnet publish ./src/NotificationService/NotificationService.csproj -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]