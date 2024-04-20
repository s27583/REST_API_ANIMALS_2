using System.Data.SqlClient;
using FluentValidation;

namespace REST_API_ANIMALS_2
{

    public static class AnimalsEndpoints
    {
        public static void RegisterAnimalsEndpoints(this WebApplication app)
        {
            var Animals = app.MapGroup("animals");

            Animals.MapGet("/", GetAnimals);
            Animals.MapGet("{id:int}", GetAnimal);
            Animals.MapPost("/", AddAnimal);
            Animals.MapDelete("{id:int}", RemoveAnimal);
            Animals.MapPut("{id:int}", ReplaceAnimal);
        }

        private static IResult ReplaceAnimal(IConfiguration configuration, IValidator<UpdateAnimalRequest> validator, int id, UpdateAnimalRequest request)
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem((IDictionary<string, string[]>)validation);
            }

            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand(
                    "UPDATE Animals SET Name = @1, Description = @2, Category = @3, Area = @4 WHERE IdAnimal = @5",
                    sqlConnection
                );
                sqlCommand.Parameters.AddWithValue("@1", request.Name);
                sqlCommand.Parameters.AddWithValue("@2", request.Description);
                sqlCommand.Parameters.AddWithValue("@3", request.Category);
                sqlCommand.Parameters.AddWithValue("@4", request.Area);
                sqlCommand.Parameters.AddWithValue("@5", id);
                sqlCommand.Connection.Open();

                var affectedRows = sqlCommand.ExecuteNonQuery();
                if (affectedRows == 0)
                {
                    return Results.NotFound();
                }
                else
                {
                    return Results.NoContent();
                }
            }
        }

        private static IResult RemoveAnimal(IConfiguration configuration, int id)
        {
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var command = new SqlCommand("DELETE FROM Animals WHERE IdAnimal = @1", sqlConnection);
                command.Parameters.AddWithValue("@1", id);
                command.Connection.Open();

                var affectedRows = command.ExecuteNonQuery();

                if (affectedRows == 0)
                {
                    return Results.NotFound();
                }
                else
                {
                    return Results.NoContent();
                }
            }
        }

        private static IResult AddAnimal(IConfiguration configuration, IValidator<AddAnimalRequest> validator, AddAnimalRequest request)
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem((IDictionary<string, string[]>)validation);
            }

            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand(
                    "INSERT INTO Animals (Name, Description, Category, Area) values (@1, @2, @3, @4); SELECT CAST(SCOPE_IDENTITY() as int)",
                    sqlConnection
                    );
                sqlCommand.Parameters.AddWithValue("@1", request.Name);
                sqlCommand.Parameters.AddWithValue("@2", request.Description);
                sqlCommand.Parameters.AddWithValue("@3", request.Category);
                sqlCommand.Parameters.AddWithValue("@4", request.Area);
                sqlCommand.Connection.Open();

                var id = sqlCommand.ExecuteScalar();

                return Results.Created($"animals/{id}", new AddAnimalResponse((int)id, request));
            }
        }

        private static IResult GetAnimals(IConfiguration configuration)
        {
            var response = new List<GetAnimalsResponse>();
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand("SELECT * FROM Animals", sqlConnection);
                sqlCommand.Connection.Open();
                var reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    response.Add(new GetAnimalsResponse(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4)
                        )
                    );
                }
            }
            return Results.Ok(response);
        }

        private static IResult GetAnimal(IConfiguration configuration, int id)
        {
            using var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"));
            var sqlCommand = new SqlCommand("SELECT * FROM Animals WHERE ID = @1", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@1", id);
            sqlCommand.Connection.Open();

            var reader = sqlCommand.ExecuteReader();
            if (!reader.Read()) return Results.NotFound();

            return Results.Ok(new GetAnimalsDetailsResponse(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4)
                )
            );
        }
    }
}