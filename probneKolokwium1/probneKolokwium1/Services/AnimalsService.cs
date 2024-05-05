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


    public AnimalDTO GetAnimalInfo(int id)
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

                command.Parameters.AddWithValue("@Id", id);
                
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
                            animal.Procedures?.Add(new ProcedureInfo()
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
    
    
    public bool CheckIfOwnerExists(int id)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "select Owner.ID from Owner where Owner.ID = @Id";
                command.Parameters.AddWithValue("@Id", id);
                var result = command.ExecuteScalar();
                return result != null && (int)result > 0;
            }
        }
    }
    
    public bool CheckIfProcedureExists(int procedureId)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand("SELECT COUNT(1) FROM [Procedure] WHERE ID = @procedureId", connection))
            {
                command.Parameters.AddWithValue("@procedureId", procedureId);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
    }

    public int InsertAnimal(string name, string type, DateTime admissionDate, int ownerId, List<ProcedureInfoToAddWithAnimal>? procedures)
    {
        int animalId = -1;

        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                SqlCommand command = connection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "INSERT INTO Animal (Name, Type, AdmissionDate, Owner_ID) VALUES (@name, @type, @admissionDate, @ownerId); SELECT SCOPE_IDENTITY();";
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@type", type);
                    command.Parameters.AddWithValue("@admissionDate", admissionDate);
                    command.Parameters.AddWithValue("@ownerId", ownerId);

                    animalId = Convert.ToInt32(command.ExecuteScalar());

                    if (procedures != null)
                    {
                        foreach (var procedure in procedures)
                        {
                            if (!CheckIfProcedureExists(procedure.procedureId))
                            {
                                throw new ArgumentException($"Procedure with ID {procedure.procedureId} does not exist.");
                            }

                            command.Parameters.Clear();

                            command.CommandText = "INSERT INTO Procedure_Animal (Procedure_ID, Animal_ID, Date) VALUES (@procedureId, @animalId, @date);";
                            command.Parameters.AddWithValue("@procedureId", procedure.procedureId);
                            command.Parameters.AddWithValue("@animalId", animalId);
                            command.Parameters.AddWithValue("@date", procedure.date);

                            command.ExecuteNonQuery();
                        }
                    }
                    
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw; 
                }
            }
        }

        return animalId;
    }


    
}