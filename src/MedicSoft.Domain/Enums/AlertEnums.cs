namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Tipos de alertas do sistema - baseado em ferramentas de mercado
    /// </summary>
    public enum AlertCategory
    {
        // Alertas de Agendamento
        AppointmentConflict = 1,          // Conflito de horários
        AppointmentNoShow = 2,            // Paciente não compareceu
        AppointmentConfirmationPending = 3, // Confirmação pendente
        AppointmentOverdue = 4,           // Consulta atrasada
        
        // Alertas Financeiros
        PaymentOverdue = 10,              // Pagamento vencido
        PaymentDueSoon = 11,              // Pagamento a vencer em breve
        InvoicePending = 12,              // Fatura pendente de emissão
        LowCashFlow = 13,                 // Fluxo de caixa baixo
        SubscriptionExpiring = 14,        // Assinatura expirando
        SubscriptionExpired = 15,         // Assinatura expirada
        
        // Alertas de Compliance e Documentação
        DocumentExpiring = 20,            // Documento vencendo (CRM, certidões)
        DocumentExpired = 21,             // Documento vencido
        MissingSignature = 22,            // Falta assinatura digital
        ComplianceViolation = 23,         // Violação de compliance
        AuditRequired = 24,               // Auditoria necessária
        
        // Alertas de Estoque
        LowStock = 30,                    // Estoque baixo
        OutOfStock = 31,                  // Sem estoque
        MaterialExpiring = 32,            // Material vencendo
        MaterialExpired = 33,             // Material vencido
        
        // Alertas Clínicos
        PrescriptionDueRenewal = 40,      // Receita precisa renovação
        ExamResultReady = 41,             // Resultado de exame pronto
        LabResultAbnormal = 42,           // Resultado laboratorial anormal
        VaccineReminder = 43,             // Lembrete de vacina
        FollowUpRequired = 44,            // Retorno necessário
        
        // Alertas Operacionais
        SystemMaintenance = 50,           // Manutenção do sistema
        BackupFailed = 51,                // Falha no backup
        HighServerLoad = 52,              // Alta carga no servidor
        IntegrationError = 53,            // Erro de integração
        SecurityAlert = 54,               // Alerta de segurança
        
        // Alertas de Fila e Atendimento
        WaitingRoomFull = 60,             // Sala de espera cheia
        LongWaitTime = 61,                // Tempo de espera longo
        PatientReady = 62,                // Paciente pronto para atendimento
        
        // Alertas de Marketing e CRM
        CampaignDeadline = 70,            // Prazo de campanha
        LeadUncontacted = 71,             // Lead não contatado
        CustomerBirthdayReminder = 72,    // Aniversário de cliente
        
        // Alertas Regulatórios (TISS, ANS, etc)
        TissSubmissionDue = 80,           // Envio TISS pendente
        TissRejected = 81,                // Lote TISS rejeitado
        AnsReportingDue = 82,             // Relatório ANS pendente
        
        // Alertas Gerais
        TaskOverdue = 90,                 // Tarefa atrasada
        ApprovalPending = 91,             // Aprovação pendente
        FormIncomplete = 92,              // Formulário incompleto
        General = 99                      // Geral
    }
    
    /// <summary>
    /// Prioridade dos alertas
    /// </summary>
    public enum AlertPriority
    {
        Low = 1,        // Baixa prioridade
        Normal = 2,     // Prioridade normal
        High = 3,       // Alta prioridade
        Critical = 4    // Crítico - requer ação imediata
    }
    
    /// <summary>
    /// Status do alerta
    /// </summary>
    public enum AlertStatus
    {
        Active = 1,        // Ativo
        Acknowledged = 2,  // Reconhecido pelo usuário
        Resolved = 3,      // Resolvido
        Dismissed = 4,     // Dispensado
        Expired = 5        // Expirado (por tempo ou evento)
    }
    
    /// <summary>
    /// Ação sugerida para o alerta
    /// </summary>
    public enum AlertAction
    {
        None = 0,
        ViewDetails = 1,      // Ver detalhes
        TakeAction = 2,       // Tomar ação
        Approve = 3,          // Aprovar
        Reject = 4,           // Rejeitar
        Contact = 5,          // Contatar
        Schedule = 6,         // Agendar
        Pay = 7,              // Pagar
        Review = 8,           // Revisar
        Sign = 9,             // Assinar
        Restock = 10          // Reabastecer
    }
    
    /// <summary>
    /// Canal de entrega do alerta
    /// </summary>
    public enum AlertChannel
    {
        InApp = 1,         // Notificação in-app
        Email = 2,         // Email
        SMS = 3,           // SMS
        WhatsApp = 4,      // WhatsApp
        Push = 5,          // Push notification
        Desktop = 6,       // Notificação desktop
        Sound = 7          // Som/beep
    }
    
    /// <summary>
    /// Tipo de destinatário do alerta
    /// </summary>
    public enum AlertRecipientType
    {
        User = 1,          // Usuário específico
        Role = 2,          // Papel/função (ex: todos médicos)
        Clinic = 3,        // Toda a clínica
        SystemAdmin = 4    // Administradores do sistema
    }
}
