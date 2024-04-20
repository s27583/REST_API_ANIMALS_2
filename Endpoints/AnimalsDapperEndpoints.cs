using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;

namespace REST_API_ANIMALS_2
{
    public static class AnimalsDapperEndpoints
    {
        public static void RegisterAnimalsDapperEndpoints(this WebApplication app)
        {
            var Animals = app.MapGroup("animals-dapper");

            Animals.MapGet("/", GetAnimals);
            Animals.MapGet("{id:int}", GetAnimal);
            Animals.MapPost("/", CreateAnimal);
            Animals.MapDelete("{id:int}", RemoveAnimal);
            Animals.MapPut("{id:int}", UpdateAnimal);
        }

        private static IResult UpdateAnimal(IConfiguration configuration, IValidator<UpdateAnimalRequest> validator, int id, UpdateAnimalRequest request)
        {

            var validation = validator.Validate(request);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem((IDictionary<string, string[]>)validation);
            }

            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var affectedRows = sqlConnection.Execute(
                    "UPDATE Animals SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal",
                    new
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Category = request.Category,
                        Area = request.Area,
                        IdAnimal = id
                    }
                );

                if (affectedRows == 0) return Results.NotFound();
            }

            return Results.NoContent();
        }

        private static IResult RemoveAnimal(IConfiguration configuration, int id)
        {
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var affectedRows = sqlConnection.Execute(
                    "DELETE FROM Animals WHERE ID = @Id",
                    new { Id = id }
                );
                return affectedRows == 0 ? Results.NotFound() : Results.NoContent();
            }
        }

        private static IResult CreateAnimal(IConfiguration configuration, IValidator<AddAnimalRequest> validator, AddAnimalRequest request)
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem((IDictionary<string, string[]>)validation);
            }

            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var id = sqlConnection.ExecuteScalar<int>(
                    "INSERT INTO Animals (Name, Description, Category, Area) values (@Name, @Description, @Category, @Area); SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Category = request.Category,
                        Area = request.Area
                    }
                );

                return Results.Created($"/animals-dapper/{id}", new AddAnimalResponse(id, request));
            }
        }

        private static IResult GetAnimals(IConfiguration configuration)
        {
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var Animals = sqlConnection.Query<GetAnimalsResponse>("SELECT * FROM Animals");
                return Results.Ok(Animals);
            }
        }

        private static IResult GetAnimal(IConfiguration configuration, int id)
        {
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var Animal = sqlConnection.QuerySingleOrDefault<GetAnimalsDetailsResponse>(
                    "SELECT * FROM Animals WHERE IdAnimal = @Id",
                    new { Id = id }
                );

                if (Animal is null) return Results.NotFound();
                return Results.Ok(Animal);
            }
        }
    }
}