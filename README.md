# Conway's Game of Life - .Net 7.0

## About

This sample Api project was built using .NET 7.0 implementing the [Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life).

## How to use it

### Setup Database

The project uses a SQLite database, before running the solution for the first time it is necessary to initialize the database schema.


To accomplish that you have to execute the following command at the "Package Manager Console" (considering you are using Visual Studio):
```bash
Update-Database
```

### Swagger

The project uses authentication, so before calling the API through Swagger, you have to log in.
You can do that through the "Authorize" option at swagger.
The swagger was preconfigured with a valid credential.


![Swagger Authorize](/ReadmeAssets/swaggerAuthorize.jpg)

![Swagger Authorize Step 2](/ReadmeAssets/swaggerAuthorize2.jpg)