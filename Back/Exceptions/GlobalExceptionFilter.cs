using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net;

namespace SolicitudServidores.Utilities
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var statusCode = exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound, // 404
                UnauthorizedAccessException => HttpStatusCode.Unauthorized, // 401
                ArgumentException => HttpStatusCode.BadRequest, // 400
                _ => HttpStatusCode.InternalServerError // 500
            };

            var errorResponse = new
            {
                StatusCode = (int)statusCode,
                Message = exception.Message,
                Details = exception.InnerException?.Message
            };

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = (int)statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
