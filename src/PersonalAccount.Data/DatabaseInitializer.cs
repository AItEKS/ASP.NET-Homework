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

    public void Init()
    {
        string script = GetSqlScript("schema.sql");

        if (string.IsNullOrWhiteSpace(script))
        {
            throw new Exception("SQL скрипт пуст или не найден");
        }

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                using (var command = new NpgsqlCommand(script, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
        }
    }

    public void SeedData()
    {
        string script = GetSqlScript("mock_data.sql");
        
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
