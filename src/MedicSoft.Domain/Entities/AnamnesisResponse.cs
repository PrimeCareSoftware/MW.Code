using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.ValueObjects;
using System.Text.Json;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a patient's response to an anamnesis template
    /// </summary>
    public class AnamnesisResponse : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        public Guid TemplateId { get; private set; }
        public DateTime ResponseDate { get; private set; }
        public string AnswersJson { get; private set; } = string.Empty;  // JSON serialized answers
        public bool IsComplete { get; private set; }
        
        // Navigation properties
        public virtual Appointment? Appointment { get; private set; }
        public virtual Patient? Patient { get; private set; }
        public virtual User? Doctor { get; private set; }
        public virtual AnamnesisTemplate? Template { get; private set; }
        
        private List<QuestionAnswer>? _answers;
        
        /// <summary>
        /// Gets the answers to the anamnesis questions
        /// </summary>
        public List<QuestionAnswer> Answers
        {
            get
            {
                if (_answers == null && !string.IsNullOrEmpty(AnswersJson))
                {
                    _answers = JsonSerializer.Deserialize<List<QuestionAnswer>>(AnswersJson) ?? new List<QuestionAnswer>();
                }
                return _answers ?? new List<QuestionAnswer>();
            }
            private set
            {
                _answers = value;
                AnswersJson = JsonSerializer.Serialize(value);
            }
        }

        private AnamnesisResponse() 
        { 
            // EF Core constructor
        }

        public AnamnesisResponse(
            Guid appointmentId,
            Guid patientId,
            Guid doctorId,
            Guid templateId,
            string tenantId) : base(tenantId)
        {
            if (appointmentId == Guid.Empty)
                throw new ArgumentException("O ID do atendimento não pode estar vazio", nameof(appointmentId));
            
            if (patientId == Guid.Empty)
                throw new ArgumentException("O ID do paciente não pode estar vazio", nameof(patientId));
            
            if (doctorId == Guid.Empty)
                throw new ArgumentException("O ID do médico não pode estar vazio", nameof(doctorId));
            
            if (templateId == Guid.Empty)
                throw new ArgumentException("O ID do template não pode estar vazio", nameof(templateId));

            AppointmentId = appointmentId;
            PatientId = patientId;
            DoctorId = doctorId;
            TemplateId = templateId;
            ResponseDate = DateTime.UtcNow;
            IsComplete = false;
            Answers = new List<QuestionAnswer>();
        }

        public void SaveAnswers(List<QuestionAnswer> answers, bool isComplete)
        {
            if (answers == null)
                throw new ArgumentNullException(nameof(answers));

            Answers = answers;
            IsComplete = isComplete;
            UpdateTimestamp();
        }

        public void MarkAsComplete()
        {
            IsComplete = true;
            UpdateTimestamp();
        }
    }
}
