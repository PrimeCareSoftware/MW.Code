import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { Button } from "@/components/ui/button";
import { StatusBadge } from "@/components/ui/status-badge";
import { FileText, Download, Eye, Calendar, Clock } from "lucide-react";

const exams = [
  {
    id: 1,
    name: "Radiografia Panorâmica",
    date: "05/02/2026",
    doctor: "Dr. João Santos",
    status: "ready",
    type: "Imagem",
  },
  {
    id: 2,
    name: "Hemograma Completo",
    date: "03/02/2026",
    doctor: "Dra. Ana Lima",
    status: "ready",
    type: "Laboratorial",
  },
  {
    id: 3,
    name: "Tomografia",
    date: "10/02/2026",
    doctor: "Dr. Carlos Mendes",
    status: "pending",
    type: "Imagem",
  },
  {
    id: 4,
    name: "Glicemia em Jejum",
    date: "01/02/2026",
    doctor: "Dra. Ana Lima",
    status: "ready",
    type: "Laboratorial",
  },
];

const scheduledExams = [
  {
    id: 5,
    name: "Raio-X Periapical",
    date: "15/02/2026",
    time: "08:30",
    location: "Laboratório Central",
    instructions: "Jejum de 8 horas",
  },
];

export default function MyExams() {
  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Início", href: "/" },
        { label: "Meus Exames" },
      ]}
      variant="patient-portal"
    >
      <div className="space-y-6">
        <div>
          <h1 className="text-2xl font-semibold text-foreground">Meus Exames</h1>
          <p className="text-muted-foreground mt-1">
            Visualize seus resultados de exames e agendamentos
          </p>
        </div>

        {/* Scheduled Exams Alert */}
        {scheduledExams.length > 0 && (
          <div className="rounded-xl border border-info/50 bg-info/5 p-4">
            <h3 className="font-medium text-foreground mb-3">Exames Agendados</h3>
            {scheduledExams.map((exam) => (
              <div key={exam.id} className="flex items-center justify-between">
                <div className="flex items-center gap-4">
                  <div className="h-10 w-10 rounded-full bg-info/10 flex items-center justify-center">
                    <Calendar className="h-5 w-5 text-info" />
                  </div>
                  <div>
                    <p className="font-medium text-foreground">{exam.name}</p>
                    <div className="flex items-center gap-4 text-sm text-muted-foreground">
                      <span className="flex items-center gap-1">
                        <Calendar className="h-3.5 w-3.5" />
                        {exam.date}
                      </span>
                      <span className="flex items-center gap-1">
                        <Clock className="h-3.5 w-3.5" />
                        {exam.time}
                      </span>
                    </div>
                  </div>
                </div>
                <div className="text-right">
                  <p className="text-sm text-muted-foreground">{exam.location}</p>
                  <p className="text-xs text-warning">{exam.instructions}</p>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Total de Exames</p>
            <p className="text-2xl font-semibold text-foreground mt-1">12</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Prontos para Download</p>
            <p className="text-2xl font-semibold text-success mt-1">10</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Em Processamento</p>
            <p className="text-2xl font-semibold text-warning mt-1">2</p>
          </div>
        </div>

        {/* Exams List */}
        <div className="rounded-xl border border-border bg-card shadow-apple-sm overflow-hidden">
          <div className="px-6 py-4 border-b border-border">
            <h3 className="font-semibold text-foreground">Resultados de Exames</h3>
          </div>
          <div className="divide-y divide-border">
            {exams.map((exam) => (
              <div key={exam.id} className="flex items-center justify-between p-4 hover:bg-secondary/30 transition-apple">
                <div className="flex items-center gap-4">
                  <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                    <FileText className="h-5 w-5 text-primary" />
                  </div>
                  <div>
                    <p className="font-medium text-foreground">{exam.name}</p>
                    <div className="flex items-center gap-3 text-sm text-muted-foreground">
                      <span>{exam.date}</span>
                      <span>•</span>
                      <span>{exam.doctor}</span>
                      <span>•</span>
                      <span>{exam.type}</span>
                    </div>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <StatusBadge
                    status={exam.status as "ready" | "pending"}
                    labels={{
                      ready: "Pronto",
                      pending: "Processando",
                    }}
                  />
                  {exam.status === "ready" && (
                    <div className="flex items-center gap-2">
                      <Button variant="ghost" size="icon" className="h-8 w-8">
                        <Eye className="h-4 w-4" />
                      </Button>
                      <Button variant="ghost" size="icon" className="h-8 w-8">
                        <Download className="h-4 w-4" />
                      </Button>
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
