namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Types of document templates for healthcare professionals
    /// </summary>
    public enum DocumentTemplateType
    {
        /// <summary>
        /// Medical record / Patient chart / Prontuário
        /// </summary>
        MedicalRecord = 1,
        
        /// <summary>
        /// Prescription / Receita
        /// </summary>
        Prescription = 2,
        
        /// <summary>
        /// Medical certificate / Atestado
        /// </summary>
        MedicalCertificate = 3,
        
        /// <summary>
        /// Lab test request / Pedido de exames
        /// </summary>
        LabTestRequest = 4,
        
        /// <summary>
        /// Psychological report / Relatório psicológico
        /// </summary>
        PsychologicalReport = 5,
        
        /// <summary>
        /// Nutrition plan / Plano alimentar
        /// </summary>
        NutritionPlan = 6,
        
        /// <summary>
        /// Dental budget / Orçamento odontológico
        /// </summary>
        DentalBudget = 7,
        
        /// <summary>
        /// Odontogram / Odontograma
        /// </summary>
        Odontogram = 8,
        
        /// <summary>
        /// Physical therapy evaluation / Avaliação fisioterapêutica
        /// </summary>
        PhysicalTherapyEvaluation = 9,
        
        /// <summary>
        /// Treatment plan / Plano de tratamento
        /// </summary>
        TreatmentPlan = 10,
        
        /// <summary>
        /// Session evolution / Evolução de sessão
        /// </summary>
        SessionEvolution = 11,
        
        /// <summary>
        /// Discharge report / Relatório de alta
        /// </summary>
        DischargeReport = 12,
        
        /// <summary>
        /// Referral / Encaminhamento
        /// </summary>
        Referral = 13,
        
        /// <summary>
        /// Informed consent / Termo de consentimento
        /// </summary>
        InformedConsent = 14,
        
        /// <summary>
        /// Custom template / Modelo personalizado
        /// </summary>
        Custom = 99
    }
}
