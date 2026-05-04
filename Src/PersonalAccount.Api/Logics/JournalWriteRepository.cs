using System;
using System.Data.Common;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;
using Npgsql;
using NpgsqlTypes;

namespace PersonalAccount.Api.Logics;

public class JournalWriteRepository : IServerRepository<JournalRowDto>
{
    private const string _sql = @"
        COPY journal (
            transnumber,
            transtype,
            receiptn,
            productid,
            product_name,
            categoryid,
            category_name,
            emploeeid,
            emploee_name,
            dater,
            quantity,
            price,
            discountamount,
            company_id,
            branch_id 
        ) 
        FROM STDIN (FORMAT BINARY)";

    public async Task<LoadingSettingsModel?> SaveRows(DbConnection connection, IEnumerable<JournalRowDto> transactions, LoadingSettingsModel options)
    {
        ArgumentNullException.ThrowIfNull(connection);
        
        if (options.Owner == null || options.Owner.Owner.Id == null)
            throw new InvalidDataException("Невозможно сохранить данные: отсутствует информация о филиале или организации-владельце!");

        try
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();

            using var writer = (connection as NpgsqlConnection)!.BeginBinaryImport(_sql);
            
            foreach (var transaction in transactions) 
            {
                writer.StartRow();
             
                // 1. transnumber
                writer.Write(transaction.Code, NpgsqlDbType.Bigint);
                // 2. transtype
                writer.Write(transaction.TypeCode, NpgsqlDbType.Bigint);
                // 3. receiptn
                writer.Write(transaction.ReceiptNumber, NpgsqlDbType.Bigint);

                // 4. productid
                if (transaction.ProductCode.HasValue)
                    writer.Write(transaction.ProductCode.Value, NpgsqlDbType.Bigint); 
                else 
                    writer.WriteNull(); 

                // 5. product_name
                if (!string.IsNullOrEmpty(transaction.ProductName))
                    writer.Write(transaction.ProductName, NpgsqlDbType.Text);
                else    
                    writer.WriteNull(); 

                // 6. categoryid
                if (transaction.CategoryCode.HasValue)
                    writer.Write(transaction.CategoryCode.Value, NpgsqlDbType.Bigint);
                else
                    writer.WriteNull();

                // 7. category_name
                if (!string.IsNullOrEmpty(transaction.CategoryName))
                    writer.Write(transaction.CategoryName, NpgsqlDbType.Text);
                else
                   writer.WriteNull();

                // 8. emploeeid
                if (transaction.EmploeeCode.HasValue)
                    writer.Write(transaction.EmploeeCode.Value, NpgsqlDbType.Bigint);
                else
                    writer.WriteNull();

                // 9. emploee_name
                if (!string.IsNullOrEmpty(transaction.EmploeeName))
                    writer.Write(transaction.EmploeeName, NpgsqlDbType.Text);
                else
                    writer.WriteNull();

                // 10. dater (timestamp with time zone)
                writer.Write(DateTime.SpecifyKind(transaction.Period, DateTimeKind.Utc), NpgsqlDbType.TimestampTz);
                
                // 11-13. Финансовые данные
                writer.Write(transaction.Quantity, NpgsqlDbType.Double);
                writer.Write(transaction.Price, NpgsqlDbType.Double);
                writer.Write(transaction.Discount, NpgsqlDbType.Double);

                // 14. company_id (Берем из Owner нашего филиала)
                writer.Write(options.Owner.Owner.Id, NpgsqlDbType.Uuid);

                // 15. branch_id (Это ID самого филиала - владельца настроек)
                writer.Write(options.Owner.Id, NpgsqlDbType.Uuid);
            }

    		writer.Complete();

            options.StartPosition = transactions.Max(x => x.Code);
            return options;
        }
        catch(Exception ex)
        {
            throw new InvalidDataException($"Ошибка Binary COPY в PostgreSQL: {ex.Message}");
        }
        finally
        {
            await connection.CloseAsync();
        }           
    }
}