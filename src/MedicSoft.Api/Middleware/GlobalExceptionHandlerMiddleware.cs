using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Api.Middleware
{
    /// <summary>
    /// Middleware para tratamento global de exceções e sanitização de erros
    /// Garante que erros técnicos não sejam expostos ao usuário final
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case UnauthorizedAccessException _:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = "Você não tem permissão para realizar esta ação.";
                    errorResponse.ErrorCode = "UNAUTHORIZED";
                    break;

                case InvalidOperationException _:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = SanitizeErrorMessage(exception.Message);
                    errorResponse.ErrorCode = "INVALID_OPERATION";
                    break;

                case ArgumentNullException _:
                case ArgumentException _:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = "Os dados fornecidos são inválidos. Por favor, verifique e tente novamente.";
                    errorResponse.ErrorCode = "INVALID_ARGUMENT";
                    break;

                case KeyNotFoundException _:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = "O recurso solicitado não foi encontrado.";
                    errorResponse.ErrorCode = "NOT_FOUND";
                    break;

                case TimeoutException _:
                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    errorResponse.Message = "A operação demorou muito tempo. Por favor, tente novamente.";
                    errorResponse.ErrorCode = "TIMEOUT";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "Ocorreu um erro ao processar sua solicitação. Por favor, tente novamente mais tarde.";
                    errorResponse.ErrorCode = "INTERNAL_ERROR";
                    
                    // Log detalhes completos do erro apenas no servidor
                    _logger.LogError(exception, "Erro interno não categorizado: {ExceptionType}", exception.GetType().Name);
                    break;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var result = JsonSerializer.Serialize(errorResponse, options);
            await context.Response.WriteAsync(result);
        }

        /// <summary>
        /// Sanitiza mensagens de erro para remover detalhes técnicos sensíveis
        /// </summary>
        private string SanitizeErrorMessage(string message)
        {
            // Mapeamento de mensagens comuns em inglês para português
            var commonMessages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Patient not found", "Paciente não encontrado." },
                { "User not found", "Usuário não encontrado." },
                { "Clinic not found", "Clínica não encontrada." },
                { "Appointment not found", "Consulta não encontrada." },
                { "Record not found", "Registro não encontrado." },
                { "Invalid credentials", "Credenciais inválidas." },
                { "Duplicate entry", "Este registro já existe no sistema." },
                { "Required field", "Campo obrigatório não preenchido." },
                { "Invalid format", "Formato inválido." },
                { "Access denied", "Acesso negado." },
                { "Session expired", "Sua sessão expirou. Por favor, faça login novamente." },
            };

            // Verifica se a mensagem contém alguma das mensagens comuns
            foreach (var kvp in commonMessages)
            {
                if (message.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Value;
                }
            }

            // Se a mensagem já está em português e não contém informações técnicas, retorna ela
            if (IsUserFriendlyMessage(message))
            {
                return message;
            }

            // Caso contrário, retorna mensagem genérica
            return "Não foi possível completar a operação. Por favor, verifique os dados e tente novamente.";
        }

        /// <summary>
        /// Verifica se a mensagem é amigável ao usuário (sem detalhes técnicos)
        /// </summary>
        private bool IsUserFriendlyMessage(string message)
        {
            // Lista de palavras/padrões que indicam mensagens técnicas que não devem ser expostas
            var technicalIndicators = new[]
            {
                "exception", "stack", "trace", "null reference", "object reference",
                "sql", "database", "connection", "timeout", "inner exception",
                ".cs:", "line ", "at ", "assembly", "system.", "microsoft.",
                "thread", "memory", "heap", "gc", "collection"
            };

            return !technicalIndicators.Any(indicator => 
                message.Contains(indicator, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Modelo padronizado de resposta de erro
    /// </summary>
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Extensão para facilitar registro do middleware
    /// </summary>
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
