using System.Reflection;
using Npgsql;

namespace PersonalAccount.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void ExecuteSqlScript(string scriptPath)
    {
        string script = GetSqlScript(scriptPath);
        
        if (string.IsNullOrWhiteSpace(script)) return;

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand(script, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    private string GetSqlScript(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resName = $"PersonalAccount.Data.Scripts.{fileName}";

        using (var stream = assembly.GetManifestResourceStream(resName))
        {
            if (stream == null)
            {
                throw new FileNotFoundException($"Ресурс '{resName}' не найден");
            }

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
