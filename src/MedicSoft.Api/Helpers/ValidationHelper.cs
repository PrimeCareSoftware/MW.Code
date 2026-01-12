using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MedicSoft.Api.Helpers
{
    /// <summary>
    /// Helper para validação e formatação de erros em português
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Converte erros de ModelState para mensagens amigáveis em português
        /// </summary>
        public static ValidationErrorResponse GetValidationErrors(ModelStateDictionary modelState)
        {
            var errors = new Dictionary<string, List<string>>();

            foreach (var key in modelState.Keys)
            {
                var state = modelState[key];
                if (state != null && state.Errors.Count > 0)
                {
                    var errorMessages = state.Errors
                        .Select(e => TranslateValidationMessage(e.ErrorMessage))
                        .Where(msg => !string.IsNullOrWhiteSpace(msg))
                        .ToList();

                    if (errorMessages.Any())
                    {
                        errors[TranslateFieldName(key)] = errorMessages;
                    }
                }
            }

            return new ValidationErrorResponse
            {
                Message = "Existem erros de validação nos dados fornecidos.",
                ErrorCode = "VALIDATION_ERROR",
                Errors = errors
            };
        }

        /// <summary>
        /// Traduz mensagens de validação comuns do inglês para português
        /// </summary>
        private static string TranslateValidationMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "Campo inválido.";

            var translations = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
            {
                { "is required", "é obrigatório." },
                { "field is required", "Este campo é obrigatório." },
                { "The field", "O campo" },
                { "is not a valid", "não é válido." },
                { "must be a number", "deve ser um número." },
                { "must be a valid email", "deve ser um e-mail válido." },
                { "must be between", "deve estar entre" },
                { "maximum length", "comprimento máximo" },
                { "minimum length", "comprimento mínimo" },
                { "invalid format", "formato inválido." },
                { "cannot be empty", "não pode estar vazio." },
                { "too long", "muito longo." },
                { "too short", "muito curto." },
                { "out of range", "fora do intervalo permitido." },
            };

            var translatedMessage = message;
            foreach (var kvp in translations)
            {
                translatedMessage = translatedMessage.Replace(kvp.Key, kvp.Value, System.StringComparison.OrdinalIgnoreCase);
            }

            // Se a mensagem já estava em português ou não mudou muito, retorna ela
            return translatedMessage;
        }

        /// <summary>
        /// Traduz nomes de campos comuns do inglês para português
        /// </summary>
        private static string TranslateFieldName(string fieldName)
        {
            var translations = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
            {
                { "Name", "Nome" },
                { "Email", "E-mail" },
                { "Phone", "Telefone" },
                { "Password", "Senha" },
                { "Username", "Nome de usuário" },
                { "Document", "CPF/CNPJ" },
                { "CPF", "CPF" },
                { "CNPJ", "CNPJ" },
                { "Address", "Endereço" },
                { "City", "Cidade" },
                { "State", "Estado" },
                { "ZipCode", "CEP" },
                { "BirthDate", "Data de nascimento" },
                { "Date", "Data" },
                { "Description", "Descrição" },
                { "Value", "Valor" },
                { "Price", "Preço" },
                { "Amount", "Quantidade" },
                { "Total", "Total" },
                { "Status", "Status" },
                { "Type", "Tipo" },
            };

            return translations.TryGetValue(fieldName, out var translated) ? translated : fieldName;
        }
    }

    /// <summary>
    /// Modelo de resposta para erros de validação
    /// </summary>
    public class ValidationErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public Dictionary<string, List<string>> Errors { get; set; } = new();
        public System.DateTime Timestamp { get; set; } = System.DateTime.UtcNow;
    }
}
