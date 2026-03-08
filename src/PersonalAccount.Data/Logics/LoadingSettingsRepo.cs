using System.Text.Json;
using Npgsql;
using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Data.Logics;

public class LoadingSettingsRepo : ILoadingSettingsRepo
{
    private readonly string _connectionString;
    
    public LoadingSettingsRepo(string connectionString)
    {
        _connectionString = connectionString;    
    }
    
    public async Task<bool> Save(Organization organization, CancellationToken token)
    {
        if (organization.Settings == null) 
        {
            return false;
        }

        string sql = @"
            update ""organization""
            set ""import_settings"" = @settings::jsonb
            where ""id"" = @id";
    
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);

        await using var command = new NpgsqlCommand(sql, connection);
        
        string jsonSettings = JsonSerializer.Serialize(organization.Settings);

        command.Parameters.AddWithValue("settings", jsonSettings);
        command.Parameters.AddWithValue("id", organization.Id);

        var result = await command.ExecuteNonQueryAsync(token);

        return result > 0; 
    }

    public async Task<bool> Load(Organization organization, CancellationToken token)
    {
        string sql = @"
            select ""import_settings""
            from ""organization""
            where ""id"" = @id";

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", organization.Id);

        var result = await command.ExecuteScalarAsync(token);

        if (result == null || result == DBNull.Value)
        {
            return false;
        }

        string jsonString = result.ToString();
        
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return false;
        }

        var settings = JsonSerializer.Deserialize<ImportSettings>(jsonString);

        organization.Settings = settings;

        return true;
    }
}
