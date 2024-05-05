using Microsoft.AspNetCore.Mvc;
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
            return NotFound();
        }

        return Ok(animal);
    }

}