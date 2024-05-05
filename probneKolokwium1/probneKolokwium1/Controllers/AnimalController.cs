using Microsoft.AspNetCore.Mvc;
using probneKolokwium1.DTOs;
using probneKolokwium1.Services;

namespace probneKolokwium1.Controllers;

[ApiController]
[Route("/api/animals")]
public class AnimalController : ControllerBase
{
    
    private readonly AnimalsService _animalsService;
    
    public AnimalController(AnimalsService animalsService)
    {
        _animalsService = animalsService;
    }

    [HttpGet("{idAnimal}")]
    public IActionResult GetAnimal(int idAnimal)
    {
        var animal = _animalsService.GetAnimalInfo(idAnimal);
        if (animal == null)
        {
            return NotFound($"Animal with id:{idAnimal} does not exist");
        }

        return Ok(animal);
    }

    [HttpPost]
    public IActionResult AddAnimal([FromBody] AddAnimalDTO animal)
    {
        if (!_animalsService.CheckIfOwnerExists(animal.OwnerId))
        {
            return BadRequest("Owner does not exist.");
        }

        try
        {
            int animalId = _animalsService.InsertAnimal(animal.Name, animal.Type, animal.AdmissionDate, animal.OwnerId, animal.Procedures);
            if (animalId > 0)
            {
                return Ok(new { Message = "Animal added successfully", AnimalId = animalId });
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while adding the animal", Details = ex.Message });
        }

        return BadRequest("Failed to add animal.");
    }

}