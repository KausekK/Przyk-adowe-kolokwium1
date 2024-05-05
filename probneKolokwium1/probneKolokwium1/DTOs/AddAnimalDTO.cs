using probneKolokwium1.Models;

namespace probneKolokwium1.DTOs;

public class AddAnimalDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime AdmissionDate { get; set; }
    public int OwnerId { get; set; } 
    public List<ProcedureInfoToAddWithAnimal>? Procedures { get; set; }
}