using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PersonalAccount.Web.Models;

namespace PersonalAccount.Web.Logics;

/// <summary>
/// Фильтр для обработки ошибок.
/// </summary>
public class CustomExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
       // Модель с данными
       var model = new ErrorViewModel()
       {
           Title = context?.ActionDescriptor?.DisplayName ?? string.Empty,
           ErrorText = $"{context?.Exception.Message}{context?.Exception.InnerException?.Message}",
           StackTrace = context?.Exception.StackTrace ?? string.Empty
       };

        // Подмена контекста
       context!.Result = new ViewResult()
       {
           ViewName = "Error",
           ViewData = new ViewDataDictionary(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary())
           {
               Model = model
           }        
       };

       context.ExceptionHandled = true;
    }
}
