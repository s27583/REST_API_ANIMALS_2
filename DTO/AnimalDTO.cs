using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST_API_ANIMALS_2
{

    public record GetAnimalsResponse(int IdAnimal, string Name, string Description, string Category, string Area);
    public record GetAnimalsDetailsResponse(int IdAnimal, string Name, string Description, string Category, string Area);

    public record AddAnimalRequest(
    [Required][MaxLength(50)] string Name,
    [Required][MaxLength(50)] string Description,
    [Required][MaxLength(50)] string Category,
    [Required][MaxLength(50)] string Area
    );
    public record UpdateAnimalRequest(
    [Required][MaxLength(50)] string Name,
    [Required][MaxLength(50)] string Description,
    [Required][MaxLength(50)] string Category,
    [Required][MaxLength(50)] string Area
    );

    public record AddAnimalResponse(int IdAnimal, string Name, string Description, string Category, string Area)
    {
        public AddAnimalResponse(int IdAnimal, AddAnimalRequest request) : this(IdAnimal, request.Name, request.Description, request.Category, request.Area) { }
    };
    public class AnimalDTO
    {
        [Required]
        public int IdAnimal { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Area { get; set; }
    }
}