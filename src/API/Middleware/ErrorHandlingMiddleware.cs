using Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ErrorHandlingMiddleware(RequestDelegate next, IStringLocalizer<SharedResources> localizer)
    {
        _next = next;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = _localizer["InternalServerError"].Value;

        var response = new
        {
            StatusCode = (int)statusCode,
            Message = message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}