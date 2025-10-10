using System;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Domain.Services
{
    /// <summary>
    /// Service for validating Brazilian documents (CPF, CNPJ, CRM)
    /// </summary>
    public static class DocumentValidator
    {
        /// <summary>
        /// Validates if a string is a valid CPF
        /// </summary>
        public static bool IsValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            try
            {
                var cpfObj = new Cpf(cpf);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Validates if a string is a valid CNPJ
        /// </summary>
        public static bool IsValidCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            try
            {
                var cnpjObj = new Cnpj(cnpj);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Validates if a string is a valid CRM
        /// </summary>
        public static bool IsValidCrm(string crm)
        {
            if (string.IsNullOrWhiteSpace(crm))
                return false;

            try
            {
                var crmObj = Crm.Parse(crm);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Validates and creates a CPF object from string
        /// </summary>
        public static Cpf ValidateCpf(string cpf)
        {
            return new Cpf(cpf);
        }

        /// <summary>
        /// Validates and creates a CNPJ object from string
        /// </summary>
        public static Cnpj ValidateCnpj(string cnpj)
        {
            return new Cnpj(cnpj);
        }

        /// <summary>
        /// Validates and creates a CRM object from string
        /// </summary>
        public static Crm ValidateCrm(string crm)
        {
            return Crm.Parse(crm);
        }
    }
}
