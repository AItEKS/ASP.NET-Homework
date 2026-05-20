using System;
using Microsoft.AspNetCore.Http;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Api.Logics;

/// <summary>
/// Middleware для обработки исключений
/// </summary>
/// <param name="next"></param>
public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next  = next;

    /// <summary>
    /// Обязательный етод для обработки
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        // Создаем модель
        var model = new ErrorDto()
        {
            ErrorText = $"{ex.Message}{ex.InnerException?.Message}",
            StackTrace = ex.StackTrace ?? string.Empty
        };

        // Готовим контекст
        context.Response.Clear();
        context.Response.StatusCode =  StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        // Загружаем модель в контекст
        await context.Response.WriteAsJsonAsync(  model  );
    }
}
