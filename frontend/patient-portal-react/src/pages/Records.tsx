import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { DataTable } from "@/components/ui/data-table";
import { Button } from "@/components/ui/button";
import { Search, FileText, User, Calendar } from "lucide-react";
import { useState } from "react";

const recordsData = [
  {
    id: 1,
    patient: "Maria Silva",
    lastVisit: "05/02/2026",
    totalVisits: 12,
    doctor: "Dr. João Santos",
    notes: "Acompanhamento ortodôntico",
  },
  {
    id: 2,
    patient: "Pedro Costa",
    lastVisit: "03/02/2026",
    totalVisits: 5,
    doctor: "Dra. Ana Lima",
    notes: "Tratamento de canal finalizado",
  },
  {
    id: 3,
    patient: "Lucia Ferreira",
    lastVisit: "01/02/2026",
    totalVisits: 8,
    doctor: "Dr. Carlos Mendes",
    notes: "Limpeza semestral",
  },
  {
    id: 4,
    patient: "Roberto Alves",
    lastVisit: "28/01/2026",
    totalVisits: 3,
    doctor: "Dr. João Santos",
    notes: "Avaliação para implante",
  },
  {
    id: 5,
    patient: "Fernanda Souza",
    lastVisit: "25/01/2026",
    totalVisits: 15,
    doctor: "Dra. Ana Lima",
    notes: "Manutenção de prótese",
  },
];

const columns = [
  {
    key: "patient",
    header: "Paciente",
    render: (item: typeof recordsData[0]) => (
      <div className="flex items-center gap-3">
        <div className="h-9 w-9 rounded-full bg-primary/10 flex items-center justify-center">
          <User className="h-4 w-4 text-primary" />
        </div>
        <span className="font-medium text-foreground">{item.patient}</span>
      </div>
    ),
  },
  {
    key: "lastVisit",
    header: "Última Visita",
    render: (item: typeof recordsData[0]) => (
      <div className="flex items-center gap-2 text-muted-foreground">
        <Calendar className="h-4 w-4" />
        <span>{item.lastVisit}</span>
      </div>
    ),
  },
  {
    key: "totalVisits",
    header: "Total de Visitas",
    render: (item: typeof recordsData[0]) => (
      <span className="font-medium text-foreground">{item.totalVisits}</span>
    ),
  },
  {
    key: "doctor",
    header: "Médico Responsável",
  },
  {
    key: "notes",
    header: "Observações",
    render: (item: typeof recordsData[0]) => (
      <span className="text-muted-foreground text-sm">{item.notes}</span>
    ),
  },
];

export default function Records() {
  const [currentPage, setCurrentPage] = useState(1);

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Prontuários" },
      ]}
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Prontuários</h1>
            <p className="text-muted-foreground mt-1">
              Acesse e gerencie os prontuários dos pacientes
            </p>
          </div>
          <div className="flex items-center gap-3">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input
                type="text"
                placeholder="Buscar prontuário..."
                className="h-10 w-64 rounded-lg border border-input bg-card pl-10 pr-4 text-sm focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
              />
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                <FileText className="h-5 w-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total de Prontuários</p>
                <p className="text-2xl font-semibold text-foreground">1.247</p>
              </div>
            </div>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-success/10 flex items-center justify-center">
                <Calendar className="h-5 w-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Atualizados Este Mês</p>
                <p className="text-2xl font-semibold text-foreground">89</p>
              </div>
            </div>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-warning/10 flex items-center justify-center">
                <User className="h-5 w-5 text-warning" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Novos Pacientes</p>
                <p className="text-2xl font-semibold text-foreground">23</p>
              </div>
            </div>
          </div>
        </div>

        <DataTable
          columns={columns}
          data={recordsData}
          actions={[
            { label: "Ver Prontuário", onClick: () => {} },
            { label: "Adicionar Nota", onClick: () => {} },
            { label: "Histórico Completo", onClick: () => {} },
          ]}
          pagination={{
            currentPage,
            totalPages: 5,
            onPageChange: setCurrentPage,
          }}
        />
      </div>
    </DashboardLayout>
  );
}
