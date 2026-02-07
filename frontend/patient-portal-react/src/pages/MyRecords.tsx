import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { Button } from "@/components/ui/button";
import { FileText, Download, Calendar, User, Stethoscope, Pill, AlertCircle } from "lucide-react";

const patientInfo = {
  name: "Maria Silva",
  birthDate: "15/03/1985",
  cpf: "123.456.789-00",
  bloodType: "O+",
  allergies: ["Penicilina", "Dipirona"],
  chronicConditions: ["Hipertensão"],
};

const medicalHistory = [
  {
    id: 1,
    date: "05/02/2026",
    type: "Consulta",
    doctor: "Dr. João Santos",
    description: "Consulta de rotina. Paciente apresenta boa saúde geral.",
    prescriptions: ["Vitamina D 2000UI - 1x ao dia"],
  },
  {
    id: 2,
    date: "20/01/2026",
    type: "Procedimento",
    doctor: "Dra. Ana Lima",
    description: "Limpeza dental profissional realizada com sucesso.",
    prescriptions: [],
  },
  {
    id: 3,
    date: "10/01/2026",
    type: "Exame",
    doctor: "Dr. Carlos Mendes",
    description: "Radiografia panorâmica para avaliação ortodôntica.",
    prescriptions: [],
  },
];

export default function MyRecords() {
  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Início", href: "/" },
        { label: "Meu Prontuário" },
      ]}
      variant="patient-portal"
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Meu Prontuário</h1>
            <p className="text-muted-foreground mt-1">
              Visualize seu histórico médico e informações de saúde
            </p>
          </div>
          <Button variant="outline" className="gap-2">
            <Download className="h-4 w-4" />
            Exportar Prontuário
          </Button>
        </div>

        {/* Patient Info Card */}
        <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="flex items-start gap-6">
            <div className="h-20 w-20 rounded-full bg-primary/10 flex items-center justify-center">
              <User className="h-10 w-10 text-primary" />
            </div>
            <div className="flex-1 grid grid-cols-1 md:grid-cols-3 gap-6">
              <div>
                <p className="text-sm text-muted-foreground">Nome Completo</p>
                <p className="font-semibold text-foreground">{patientInfo.name}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Data de Nascimento</p>
                <p className="font-semibold text-foreground">{patientInfo.birthDate}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">CPF</p>
                <p className="font-semibold text-foreground">{patientInfo.cpf}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Tipo Sanguíneo</p>
                <p className="font-semibold text-foreground">{patientInfo.bloodType}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Alergias</p>
                <div className="flex flex-wrap gap-1 mt-1">
                  {patientInfo.allergies.map((allergy) => (
                    <span
                      key={allergy}
                      className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full bg-destructive/10 text-destructive text-xs"
                    >
                      <AlertCircle className="h-3 w-3" />
                      {allergy}
                    </span>
                  ))}
                </div>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Condições Crônicas</p>
                <div className="flex flex-wrap gap-1 mt-1">
                  {patientInfo.chronicConditions.map((condition) => (
                    <span
                      key={condition}
                      className="inline-flex px-2 py-0.5 rounded-full bg-warning/10 text-warning text-xs"
                    >
                      {condition}
                    </span>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Medical History */}
        <div>
          <h2 className="text-lg font-semibold text-foreground mb-4">Histórico Médico</h2>
          <div className="space-y-4">
            {medicalHistory.map((record) => (
              <div
                key={record.id}
                className="rounded-xl border border-border bg-card p-5 shadow-apple-sm"
              >
                <div className="flex items-start justify-between mb-4">
                  <div className="flex items-center gap-3">
                    <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                      {record.type === "Consulta" && <Stethoscope className="h-5 w-5 text-primary" />}
                      {record.type === "Procedimento" && <Stethoscope className="h-5 w-5 text-primary" />}
                      {record.type === "Exame" && <FileText className="h-5 w-5 text-primary" />}
                    </div>
                    <div>
                      <p className="font-semibold text-foreground">{record.type}</p>
                      <p className="text-sm text-muted-foreground">{record.doctor}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Calendar className="h-4 w-4" />
                    <span>{record.date}</span>
                  </div>
                </div>
                <p className="text-foreground mb-4">{record.description}</p>
                {record.prescriptions.length > 0 && (
                  <div className="pt-4 border-t border-border">
                    <p className="text-sm font-medium text-foreground mb-2 flex items-center gap-2">
                      <Pill className="h-4 w-4 text-primary" />
                      Prescrições
                    </p>
                    <ul className="space-y-1">
                      {record.prescriptions.map((prescription, index) => (
                        <li key={index} className="text-sm text-muted-foreground">
                          • {prescription}
                        </li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
