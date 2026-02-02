using System.Collections.Generic;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Represents terminology mapping for a specific professional specialty
    /// </summary>
    public class TerminologyMap
    {
        public string Appointment { get; }
        public string Professional { get; }
        public string Registration { get; }
        public string Client { get; }
        public string MainDocument { get; }
        public string ExitDocument { get; }
        
        private TerminologyMap(
            string appointment,
            string professional,
            string registration,
            string client,
            string mainDocument,
            string exitDocument)
        {
            Appointment = appointment;
            Professional = professional;
            Registration = registration;
            Client = client;
            MainDocument = mainDocument;
            ExitDocument = exitDocument;
        }
        
        /// <summary>
        /// Gets the appropriate terminology map for a given professional specialty
        /// </summary>
        public static TerminologyMap For(ProfessionalSpecialty specialty)
        {
            return specialty switch
            {
                ProfessionalSpecialty.Psicologo => new TerminologyMap(
                    appointment: "Sessão",
                    professional: "Psicólogo",
                    registration: "CRP",
                    client: "Paciente",
                    mainDocument: "Prontuário",
                    exitDocument: "Relatório Psicológico"
                ),
                
                ProfessionalSpecialty.Nutricionista => new TerminologyMap(
                    appointment: "Consulta",
                    professional: "Nutricionista",
                    registration: "CRN",
                    client: "Paciente",
                    mainDocument: "Avaliação Nutricional",
                    exitDocument: "Plano Alimentar"
                ),
                
                ProfessionalSpecialty.Dentista => new TerminologyMap(
                    appointment: "Consulta",
                    professional: "Dentista",
                    registration: "CRO",
                    client: "Paciente",
                    mainDocument: "Odontograma",
                    exitDocument: "Orçamento de Tratamento"
                ),
                
                ProfessionalSpecialty.Fisioterapeuta => new TerminologyMap(
                    appointment: "Sessão",
                    professional: "Fisioterapeuta",
                    registration: "CREFITO",
                    client: "Paciente",
                    mainDocument: "Avaliação Fisioterapêutica",
                    exitDocument: "Plano de Tratamento"
                ),
                
                ProfessionalSpecialty.Medico => new TerminologyMap(
                    appointment: "Consulta",
                    professional: "Médico",
                    registration: "CRM",
                    client: "Paciente",
                    mainDocument: "Prontuário Médico",
                    exitDocument: "Receita Médica"
                ),
                
                ProfessionalSpecialty.Enfermeiro => new TerminologyMap(
                    appointment: "Atendimento",
                    professional: "Enfermeiro",
                    registration: "COREN",
                    client: "Paciente",
                    mainDocument: "Prontuário de Enfermagem",
                    exitDocument: "Relatório de Enfermagem"
                ),
                
                ProfessionalSpecialty.TerapeutaOcupacional => new TerminologyMap(
                    appointment: "Sessão",
                    professional: "Terapeuta Ocupacional",
                    registration: "COFFITO",
                    client: "Paciente",
                    mainDocument: "Avaliação Terapêutica",
                    exitDocument: "Plano Terapêutico"
                ),
                
                ProfessionalSpecialty.Fonoaudiologo => new TerminologyMap(
                    appointment: "Sessão",
                    professional: "Fonoaudiólogo",
                    registration: "CRFa",
                    client: "Paciente",
                    mainDocument: "Avaliação Fonoaudiológica",
                    exitDocument: "Plano Terapêutico"
                ),
                
                _ => new TerminologyMap(
                    appointment: "Atendimento",
                    professional: "Profissional",
                    registration: "Registro Profissional",
                    client: "Cliente",
                    mainDocument: "Prontuário",
                    exitDocument: "Documento de Saída"
                )
            };
        }
        
        /// <summary>
        /// Gets all terminology as a dictionary for easy serialization
        /// </summary>
        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { "appointment", Appointment },
                { "professional", Professional },
                { "registration", Registration },
                { "client", Client },
                { "mainDocument", MainDocument },
                { "exitDocument", ExitDocument }
            };
        }
    }
}
