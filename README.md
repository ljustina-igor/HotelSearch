# Hotel Search application

Application for searching cheapest and closest hotels for given coordinates. Also supports adding, updating and deleting of the hotels.

## Getting started

Application uses SQL Server for the database.

For docker SQL Server usage, You can use:

> docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -e "MSSQL_PID=Evaluation" -p 1433:1433  --name sqlpreview --hostname sqlpreview -d mcr.microsoft.com/mssql/server:2022-preview-ubuntu-22.04

Please modify the command depending on the purpose.

To run the application .NET 8 runetime is needed.
> https://dotnet.microsoft.com/en-us/download/dotnet/8.0

## Usage

To run migrations, installation of `command-line interface (CLI) tools for Entity Framework Core` is needed.
Run the following command in command prompt to install it globally:

> `dotnet tool install --global dotnet-ef`

Fill database connection string in appsettings.json file located in HotelSearch.Api project.

After connection string setup is completed, navigate inside HotelSearch.Api folder in command prompt and use `dotnet ef database update` command to create and populate the database:


>  `-- --environment development` can be added to the command depending on environment used