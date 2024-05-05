using System.Data;
using System.Data.SqlClient;
using probneKolokwium1.DTOs;
using probneKolokwium1.Models;

namespace probneKolokwium1.Services;

public class AnimalsService
{
    private readonly IConfiguration _configuration;
    
    public AnimalsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public AnimalDTO  GetAnimalInfo(int Id)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText =
                    "SELECT a.ID, a.Name, a.Type, a.AdmissionDate, o.ID,o.FirstName, o.LastName, p.Name AS ProcedureName, " +
                    "p.Description, pa.Date AS ProcedureDate " +
                    "FROM Animal a " +
                    "JOIN Owner o ON a.Owner_ID = o.ID " +
                    "LEFT JOIN Procedure_Animal pa ON a.ID = pa.Animal_ID " +
                    "LEFT JOIN [Procedure] p ON pa.Procedure_ID = p.ID " +
                    "WHERE a.ID = @Id";

                command.Parameters.AddWithValue("@Id", Id);
                
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    var animal = new AnimalDTO();
                    while (reader.Read())
                    {
                        animal.Id = reader.GetInt32(0);
                        animal.Name = reader.GetString(1);
                        animal.Type = reader.GetString(2);
                        animal.AdmissionDate = reader.GetDateTime(3);
                        animal.Owner = new Owner()
                        {
                            Id = reader.GetInt32(4),
                            FirstName = reader.GetString(5),
                            LastName = reader.GetString(6)
                        };
                        if (!reader.IsDBNull(7))
                        {
                            animal.Procedures.Add(new ProcedureInfo()
                            {
                                Name = reader.GetString(7),
                                Description = reader.GetString(8),
                                Date = reader.GetDateTime(9)
                            });
                        }
                    }

                    return animal;
                }

                return null;
            }
        }
    }
}