# Uploadify
> A browser that supports the WASM format is required for the application to work properly.

An open source network filesharing solution that uses ASP.NET Core, WASM and relational database technologies, OAuth2 + OpenID Connect protocols or the OpenAPI specification for implementation.
## Setup
### Database
> The database connection settings must be the exact same for each application.

The solution requires a database connection. If required, you can configure the database connection settings, which can be found in the `appsettings.json` file of each application.
#### Configuration file locations
1. `./src/Uploadify.Server.IdentityProvider/appsettings.json`
2. `./src/Uploadify.Server.ResourceServer/appsettings.json`
3. `./src/Uploadify.Client.Api/appsettings.json`
#### Docker
> The parameters must be exact same as the database connection settings.

    docker run --name uploadify -e POSTGRES_DB=Uploadify -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=pgAdmin* -p 5433:5432 -d postgres:16.1
## Maintenance
### Certificates
#### Developer
A certificate will be required for local development, which can be issued using the `cmdlet` below.

    dotnet dev-certs https
### Migrations
#### Add
> The `cmdlet` to add the migration must be run through the console from the root of the solution.

> The `cmdlet` to add a migration requires `dotnet ef-tools` to be installed. You may use the following `cmdlet` to install them, `dotnet tool install --global dotnet-ef`, or update them, `dotnet tool update --global dotnet-ef`.

You can use the `cmdlet` below to add a migration, be sure to adjust the migration name.

    dotnet ef migrations add AddTablesAndSchemas --project .\src\Uploadify.Server.Data\ --startup-project .\src\Uploadify.Server.IdentityServer\ --output-dir .\Infrastructure\EF\Migrations\
