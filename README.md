# CoffeeShop
Welcome to CoffeeShop - a simple yet robust Web API for processing coffee orders. It features asynchronous background processing, retry logic for failed orders, and a clean architecture using ASP.NET Core and PostgreSQL.

## Setup Instructions                                                                  |
1. Ensure PostgreSQL is running with a schema you wish to use for this API, as well as a user exists with the necessary privileges.
2. Update the `CoffeeOrderAPI/appsettings.json` file with your connection string:
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Username=your_user;Password=your_password;Database=your_database"
   }
3. Run the setup.bat script. This will build the project as well as migrate the database
4. Run the run.bat script to launch the application. 
5. You are ready to start receiving orders
6. If you ever wish to uninstall, run the clean.bat script

## Assumptions made
1. If an order fails, 3 retries will occur and then fails permanently
2. Any order can fail with a default probability of 25%

##  Design Decisions
1. The API is publicly accessible - no authentication or API tokens are required.
2. SwaggerUI is available for testing the endpoints through HTTP on Port 5005 on localhost (http://localhost:5005/swagger/index.html)
3. Data are held raw in the DB.

## Prerequisites
1. .NET 8 SDK(https://dotnet.microsoft.com/download)
2. Entity Framework CLI tool (dotnet tool install --global dotnet-ef)
2. PostgreSQL(https://www.postgresql.org/download/)

## Future improvements
1. Create a Frontend for the application
2. Create unit tests