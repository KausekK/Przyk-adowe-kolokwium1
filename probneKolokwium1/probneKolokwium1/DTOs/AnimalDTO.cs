using probneKolokwium1.Models;

namespace probneKolokwium1.DTOs;

public class AnimalDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public DateTime AdmissionDate { get; set; }
    public Owner Owner { get; set; }
    public List<ProcedureInfo> Procedures { get; set; }
    
    public AnimalDTO() {
        this.Owner = new Owner();
        this.Procedures = new List<ProcedureInfo>();
    }
}