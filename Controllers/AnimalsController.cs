using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace REST_API_ANIMALS_2
{
    [ApiController]
    [Route("controller-animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AnimalsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("all")]
        public IActionResult GetAllAnimals()
        {
            var response = new List<GetAnimalsResponse>();
            using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
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
            return Ok(response);
        }

        
        [HttpGet("sorted/{orderBy}")]
        public IEnumerable<GetAnimalsResponse> GetSortedAnimals(string orderBy)
        {
            using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand($"SELECT * FROM Animals ORDER BY {orderBy}", sqlConnection);
                sqlCommand.Connection.Open();
                var reader = sqlCommand.ExecuteReader();

                var response = new List<GetAnimalsResponse>();
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
                return response;
            }
        }

        [HttpGet("{IdAnimal}")]
        public IActionResult GetAnimal(int IdAnimal)
        {
            using var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
            var sqlCommand = new SqlCommand("SELECT * FROM Animals WHERE ID = @1", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@1", IdAnimal);
            sqlCommand.Connection.Open();

            var reader = sqlCommand.ExecuteReader();
            if (!reader.Read()) return NotFound();

            return Ok(new GetAnimalsDetailsResponse(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4)
                )
            );
        }

        [HttpPost]
        public IActionResult AddAnimal(AddAnimalRequest request)
        {
            using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
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

                return Created($"animals/{id}", new AddAnimalResponse((int)id, request));
            }
        }

        [HttpPut("{idAnimal}")]
        public IActionResult UpdateAnimal(int id, UpdateAnimalRequest request)
        {
            using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
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
                return affectedRows == 0 ? NotFound() : NoContent();
            }
        }

        [HttpDelete("{idAnimal}")]
        public IActionResult DeleteAnimal(int id)
        {
            using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                var command = new SqlCommand("DELETE FROM Animals WHERE IdAnimal = @1", sqlConnection);
                command.Parameters.AddWithValue("@1", id);
                command.Connection.Open();

                var affectedRows = command.ExecuteNonQuery();

                return affectedRows == 0 ? NotFound() : NoContent();
            }
        }
    }
}