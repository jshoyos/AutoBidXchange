# Auctions Management Microservice
This service is in charge of the auctions management (CRUD operations) use this service to create, read, update, or delete auctions.
An auction is like a listing, it will contain the information of the car, as well as the information of the bid (end date, reserve price, etc...)

<!-- GETTING STARTED -->
## Getting Started

### Prerequisites

* ![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)
* ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/jshoyos/AutoBidXchange.git
   ```
2. Open a terminal and Navigate to the AuctionService folder
3. Start the docker containers for postgreSQL
   ```sh
   docker compose up -d
   ```
5. Start the auction service
   ```sh
   dotnet watch
   ```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS AND IMAGES -->
[aspnet-url]:https://dotnet.microsoft.com/en-us/apps/aspnet
[ef-url]:https://learn.microsoft.com/en-us/aspnet/entity-framework
[cs-log]:https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white
[cs]:https://learn.microsoft.com/en-us/dotnet/csharp/
