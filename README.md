# Securing Asp.Net Core Web API using Identity Server
This is the code base for my 5 part article series **Securing ASP.Net Core Web API with Identity Server**:
- [Part 1: Setting up and configuring Identity Server](https://jinishbhardwaj.wordpress.com/2020/07/11/securing-asp-net-core-web-apis-with-identity-server/)
- [Part 2: Moving Identity Server configuration to the database using Entity Framework Core](https://jinishbhardwaj.wordpress.com/2020/07/11/securing-asp-net-core-web-api-with-identity-server-part-2/)
- [Part 3: Configuring ASP.Net Identity to work with Identity Server using Entity Framework Core](https://jinishbhardwaj.wordpress.com/2020/07/12/securing-asp-net-core-web-api-with-identity-server-part-3/)
- [Part 4: User authentication and authorization with Identity Server](https://jinishbhardwaj.wordpress.com/2020/07/14/securing-asp-net-core-web-api-with-identity-server-part-4/)
- Part 5: Creating, configuring and securing an ASP.Net Core API (using Swagger UI)

## Technologies used
- C#
- ASP.Net Core 3.1 
- Identity Server 4
- ASP.Net Identity
- Entity Framework Core 3.1
- Swagger
- Visual Studio 2019 16.6.3
- SQL Server Express

## Getting started
- Clone the repository
- Checkout the master branch
- Open the **AspNetCoreIdSrv.sln** file in Visual Studio

### Database migrations
- Update the **appsettings.json** file in **IdentityServer.csproj** project and the connection string to point to your instance of SQL Server
- Migrations are in the **Migrations** folder in respective projects, so all that is required is to:
  `dotnet ef database update --context ApplicationDbContext`

