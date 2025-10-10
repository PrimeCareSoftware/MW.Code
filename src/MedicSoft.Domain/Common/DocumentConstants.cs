namespace MedicSoft.Domain.Common
{
    /// <summary>
    /// Constants for Brazilian document validation
    /// </summary>
    public static class DocumentConstants
    {
        /// <summary>
        /// Standard length of a CPF (Cadastro de Pessoas Físicas) document
        /// </summary>
        public const int CpfLength = 11;

        /// <summary>
        /// Standard length of a CNPJ (Cadastro Nacional da Pessoa Jurídica) document
        /// </summary>
        public const int CnpjLength = 14;
    }
}
