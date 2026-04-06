using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Extensions;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Console.Logics;

/// <summary>
/// Реализация интерфейса <see cref="IRepository"/>
/// </summary>
public class JournalRepository : IClientRepository<JournalRowDto>
{
    // Шаблон SQL запроса
    private const string _sql = @"
       select top {0}
            -- Уникальный номер транзакции
            transnumber,
            -- Уникальный номер чека
            receiptn,
            -- Тип транзакции
            transtype,
            -- Период
            dater,
            -- Код номенклатуры
            case when transtype = 101
                then id 
                else 0 
            end as productid,
            -- Код сотрудника
            case when transtype in (387, 386, 211, 216)
                then id
                else 0
            end as emploeeid,
            -- Код категории
            case when transtype = 101
                then categoryid
                else 0
            end as    categoryid ,
            -- Количество
            quantity,
            -- Цена
            price,
            -- Сумма скидки
            discountamount
        from journal
        where transtype in (387, 386, 211, 216, 101, 102)
        and transnumber >= {1}";

    public JournalRepository()
    {
    }

    /// <summary>
    /// Получить выборку данных из журнала транзакций.
    /// </summary>
    /// <param name="connection"> Соединение. </param>
    /// <param name="options"> Опции. </param>
    /// <returns></returns>
    public async Task<IEnumerable<JournalRowDto>> GetRows(DbConnection connection, LoadingSettingsModel options)
    {
         // Проверки
        ArgumentNullException.ThrowIfNull(connection);
        var sql = string.Format(_sql, options.BatchSize, options.StartPosition);    

        try
        {
            if(connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();

            // Выполняем выборку данных
            var command = new SqlCommand (sql, (SqlConnection) connection);
            var dataset = new DataSet();
            var adapter = new SqlDataAdapter(command);
            adapter.Fill(dataset);

            // Выполняем маппинг
            var result = from s in dataset.Tables[0].Rows.Cast<DataRow>()
                         select s.MapRow<JournalRowDto>();

            return result;
        }
        catch(Exception ex)
        {
            throw new InvalidDataException($"Невозможно выполнить SQL запрос {sql}\n{ex.Message}{ex.InnerException?.Message}");
        }
        finally
        {
            await connection.CloseAsync();
        }   
    }
}