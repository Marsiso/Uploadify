<h1>Uploadify</h1>

<h2>Setup</h2>

<h3>Database</h3>
<p>Docker</p>
<code>
    docker run --name uploadify -e POSTGRES_DB=Uploadify -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=pgAdmin* -p 5433:5432 -d postgres:16.1
</code>

<h2>Maintenance</h2>

<h3>Migrations</h3>
<p>Add</p>
<code>
    dotnet ef migrations add AddTablesAndSchemas --project .\src\Uploadify.Server.Data\ --startup-project .\src\Uploadify.Server.IdentityServer\ --output-dir .\Infrastructure\EF\Migrations\
</code>

<p>Remove</p>
<code>
dotnet ef migrations remove --project .\src\Uploadify.Server.Data\ --startup-project .\src\Uploadify.Server.IdentityServer\ --output-dir .\Infrastructure\EF\Migrations
</code>
