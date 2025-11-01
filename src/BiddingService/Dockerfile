FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# copy all .csproj files and restore as distinct layers. Use of the same COPY command for every dockerfile
# in the project to take advantage of docker caching
COPY AutoBidXchange.sln AutoBidXchange.sln
COPY src/BiddingService/BiddingService.csproj src/BiddingService/BiddingService.csproj
COPY src/Contracts/Contracts.csproj src/Contracts/Contracts.csproj

# Restore package steps
RUN dotnet restore ./src/BiddingService/BiddingService.csproj

# Copy the app folders folder
COPY . .
RUN dotnet publish ./src/BiddingService/BiddingService.csproj -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BiddingService.dll"]