namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Tipo de ação executada no sistema
    /// </summary>
    public enum AuditAction
    {
        // CRUD Operations
        CREATE,
        READ,
        UPDATE,
        DELETE,
        
        // Authentication
        LOGIN,
        LOGOUT,
        LOGIN_FAILED,
        PASSWORD_CHANGED,
        PASSWORD_RESET_REQUESTED,
        MFA_ENABLED,
        MFA_DISABLED,
        
        // Authorization
        ACCESS_DENIED,
        PERMISSION_CHANGED,
        ROLE_CHANGED,
        
        // Data Export
        EXPORT,
        DOWNLOAD,
        PRINT,
        
        // LGPD Rights
        DATA_ACCESS_REQUEST,
        DATA_DELETION_REQUEST,
        DATA_PORTABILITY_REQUEST,
        DATA_CORRECTION_REQUEST
    }

    /// <summary>
    /// Resultado da operação executada
    /// </summary>
    public enum OperationResult
    {
        SUCCESS,
        FAILED,
        UNAUTHORIZED,
        PARTIAL_SUCCESS
    }

    /// <summary>
    /// Categoria de dados conforme LGPD
    /// </summary>
    public enum DataCategory
    {
        PUBLIC,          // Dados públicos
        PERSONAL,        // Dados pessoais (LGPD)
        SENSITIVE,       // Dados sensíveis (saúde, biométricos)
        CONFIDENTIAL     // Dados confidenciais (segredos comerciais)
    }

    /// <summary>
    /// Finalidade do tratamento de dados conforme LGPD
    /// </summary>
    public enum LgpdPurpose
    {
        HEALTHCARE,             // Prestação de serviços de saúde
        BILLING,                // Faturamento
        LEGAL_OBLIGATION,       // Obrigação legal
        LEGITIMATE_INTEREST,    // Interesse legítimo
        CONSENT                 // Consentimento
    }

    /// <summary>
    /// Severidade do evento de auditoria
    /// </summary>
    public enum AuditSeverity
    {
        INFO,       // Informativo
        WARNING,    // Aviso
        ERROR,      // Erro
        CRITICAL    // Crítico (violação de segurança, tentativa de invasão)
    }
}
