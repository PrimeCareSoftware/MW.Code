using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicSoft.Repository.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class AddRequiredFieldsToConsultationFormConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireChiefComplaint",
                table: "ConsultationFormProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireClinicalExamination",
                table: "ConsultationFormProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireCurrentMedications",
                table: "ConsultationFormProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireDiagnosticHypothesis",
                table: "ConsultationFormProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireFamilyHistory",
                table: "ConsultationFormProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireHistoryOfPresentIllness",
                table: "ConsultationFormProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireInformedConsent",
                table: "ConsultationFormProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireLifestyleHabits",
                table: "ConsultationFormProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequirePastMedicalHistory",
                table: "ConsultationFormProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireTherapeuticPlan",
                table: "ConsultationFormProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireChiefComplaint",
                table: "ConsultationFormConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireClinicalExamination",
                table: "ConsultationFormConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireCurrentMedications",
                table: "ConsultationFormConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireDiagnosticHypothesis",
                table: "ConsultationFormConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireFamilyHistory",
                table: "ConsultationFormConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireHistoryOfPresentIllness",
                table: "ConsultationFormConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireInformedConsent",
                table: "ConsultationFormConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireLifestyleHabits",
                table: "ConsultationFormConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequirePastMedicalHistory",
                table: "ConsultationFormConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireTherapeuticPlan",
                table: "ConsultationFormConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "FilasEspera",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    TempoMedioAtendimento = table.Column<int>(type: "integer", nullable: false),
                    UsaPrioridade = table.Column<bool>(type: "boolean", nullable: false),
                    UsaAgendamento = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilasEspera", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilasEspera_Clinics_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SenhasFila",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FilaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: true),
                    NomePaciente = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CpfPaciente = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    TelefonePaciente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NumeroSenha = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DataHoraEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataHoraChamada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataHoraAtendimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataHoraSaida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Prioridade = table.Column<int>(type: "integer", nullable: false),
                    MotivoPrioridade = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TentativasChamada = table.Column<int>(type: "integer", nullable: false),
                    MedicoId = table.Column<Guid>(type: "uuid", nullable: true),
                    EspecialidadeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConsultorioId = table.Column<Guid>(type: "uuid", nullable: true),
                    NumeroConsultorio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AgendamentoId = table.Column<Guid>(type: "uuid", nullable: true),
                    TempoEsperaMinutos = table.Column<int>(type: "integer", nullable: false),
                    TempoAtendimentoMinutos = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SenhasFila", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SenhasFila_Appointments_AgendamentoId",
                        column: x => x.AgendamentoId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SenhasFila_FilasEspera_FilaId",
                        column: x => x.FilaId,
                        principalTable: "FilasEspera",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SenhasFila_Patients_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SenhasFila_Users_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilasEspera_ClinicaId",
                table: "FilasEspera",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_FilasEspera_ClinicaId_Ativa",
                table: "FilasEspera",
                columns: new[] { "ClinicaId", "Ativa" });

            migrationBuilder.CreateIndex(
                name: "IX_FilasEspera_TenantId",
                table: "FilasEspera",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SenhasFila_AgendamentoId",
                table: "SenhasFila",
                column: "AgendamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_SenhasFila_FilaId",
                table: "SenhasFila",
                column: "FilaId");

            migrationBuilder.CreateIndex(
                name: "IX_SenhasFila_FilaId_DataHoraEntrada",
                table: "SenhasFila",
                columns: new[] { "FilaId", "DataHoraEntrada" });

            migrationBuilder.CreateIndex(
                name: "IX_SenhasFila_FilaId_Status",
                table: "SenhasFila",
                columns: new[] { "FilaId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SenhasFila_MedicoId",
                table: "SenhasFila",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_SenhasFila_NumeroSenha_FilaId",
                table: "SenhasFila",
                columns: new[] { "NumeroSenha", "FilaId" });

            migrationBuilder.CreateIndex(
                name: "IX_SenhasFila_PacienteId",
                table: "SenhasFila",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_SenhasFila_Status",
                table: "SenhasFila",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SenhasFila_TenantId",
                table: "SenhasFila",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SenhasFila");

            migrationBuilder.DropTable(
                name: "FilasEspera");

            migrationBuilder.DropColumn(
                name: "RequireChiefComplaint",
                table: "ConsultationFormProfiles");

            migrationBuilder.DropColumn(
                name: "RequireClinicalExamination",
                table: "ConsultationFormProfiles");

            migrationBuilder.DropColumn(
                name: "RequireCurrentMedications",
                table: "ConsultationFormProfiles");

            migrationBuilder.DropColumn(
                name: "RequireDiagnosticHypothesis",
                table: "ConsultationFormProfiles");

            migrationBuilder.DropColumn(
                name: "RequireFamilyHistory",
                table: "ConsultationFormProfiles");

            migrationBuilder.DropColumn(
                name: "RequireHistoryOfPresentIllness",
                table: "ConsultationFormProfiles");

            migrationBuilder.DropColumn(
                name: "RequireInformedConsent",
                table: "ConsultationFormProfiles");

            migrationBuilder.DropColumn(
                name: "RequireLifestyleHabits",
                table: "ConsultationFormProfiles");

            migrationBuilder.DropColumn(
                name: "RequirePastMedicalHistory",
                table: "ConsultationFormProfiles");

            migrationBuilder.DropColumn(
                name: "RequireTherapeuticPlan",
                table: "ConsultationFormProfiles");

            migrationBuilder.DropColumn(
                name: "RequireChiefComplaint",
                table: "ConsultationFormConfigurations");

            migrationBuilder.DropColumn(
                name: "RequireClinicalExamination",
                table: "ConsultationFormConfigurations");

            migrationBuilder.DropColumn(
                name: "RequireCurrentMedications",
                table: "ConsultationFormConfigurations");

            migrationBuilder.DropColumn(
                name: "RequireDiagnosticHypothesis",
                table: "ConsultationFormConfigurations");

            migrationBuilder.DropColumn(
                name: "RequireFamilyHistory",
                table: "ConsultationFormConfigurations");

            migrationBuilder.DropColumn(
                name: "RequireHistoryOfPresentIllness",
                table: "ConsultationFormConfigurations");

            migrationBuilder.DropColumn(
                name: "RequireInformedConsent",
                table: "ConsultationFormConfigurations");

            migrationBuilder.DropColumn(
                name: "RequireLifestyleHabits",
                table: "ConsultationFormConfigurations");

            migrationBuilder.DropColumn(
                name: "RequirePastMedicalHistory",
                table: "ConsultationFormConfigurations");

            migrationBuilder.DropColumn(
                name: "RequireTherapeuticPlan",
                table: "ConsultationFormConfigurations");
        }
    }
}
