import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { DataTable } from "@/components/ui/data-table";
import { StatusBadge } from "@/components/ui/status-badge";
import { Button } from "@/components/ui/button";
import { Plus, Calendar, Clock, User } from "lucide-react";
import { useState } from "react";

const appointmentsData = [
  {
    id: 1,
    patient: "Maria Silva",
    doctor: "Dr. João Santos",
    date: "07/02/2026",
    time: "09:00",
    type: "Consulta",
    status: "confirmed",
  },
  {
    id: 2,
    patient: "Pedro Costa",
    doctor: "Dra. Ana Lima",
    date: "07/02/2026",
    time: "10:30",
    type: "Retorno",
    status: "pending",
  },
  {
    id: 3,
    patient: "Lucia Ferreira",
    doctor: "Dr. Carlos Mendes",
    date: "07/02/2026",
    time: "11:00",
    type: "Exame",
    status: "confirmed",
  },
  {
    id: 4,
    patient: "Roberto Alves",
    doctor: "Dr. João Santos",
    date: "07/02/2026",
    time: "14:00",
    type: "Consulta",
    status: "cancelled",
  },
  {
    id: 5,
    patient: "Fernanda Souza",
    doctor: "Dra. Ana Lima",
    date: "08/02/2026",
    time: "08:30",
    type: "Primeira Consulta",
    status: "pending",
  },
];

const columns = [
  {
    key: "patient",
    header: "Paciente",
    render: (item: typeof appointmentsData[0]) => (
      <div className="flex items-center gap-3">
        <div className="h-9 w-9 rounded-full bg-primary/10 flex items-center justify-center">
          <User className="h-4 w-4 text-primary" />
        </div>
        <span className="font-medium text-foreground">{item.patient}</span>
      </div>
    ),
  },
  {
    key: "doctor",
    header: "Médico",
  },
  {
    key: "date",
    header: "Data",
    render: (item: typeof appointmentsData[0]) => (
      <div className="flex items-center gap-2 text-muted-foreground">
        <Calendar className="h-4 w-4" />
        <span>{item.date}</span>
      </div>
    ),
  },
  {
    key: "time",
    header: "Horário",
    render: (item: typeof appointmentsData[0]) => (
      <div className="flex items-center gap-2 text-muted-foreground">
        <Clock className="h-4 w-4" />
        <span>{item.time}</span>
      </div>
    ),
  },
  {
    key: "type",
    header: "Tipo",
  },
  {
    key: "status",
    header: "Status",
    render: (item: typeof appointmentsData[0]) => (
      <StatusBadge
        status={item.status as "confirmed" | "pending" | "cancelled"}
        labels={{
          confirmed: "Confirmado",
          pending: "Pendente",
          cancelled: "Cancelado",
        }}
      />
    ),
  },
];

export default function Appointments() {
  const [currentPage, setCurrentPage] = useState(1);

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Consultas" },
      ]}
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Consultas</h1>
            <p className="text-muted-foreground mt-1">
              Gerencie os agendamentos e consultas da clínica
            </p>
          </div>
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Nova Consulta
          </Button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Hoje</p>
            <p className="text-2xl font-semibold text-foreground mt-1">12</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Esta Semana</p>
            <p className="text-2xl font-semibold text-foreground mt-1">47</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Pendentes</p>
            <p className="text-2xl font-semibold text-warning mt-1">8</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Canceladas</p>
            <p className="text-2xl font-semibold text-destructive mt-1">3</p>
          </div>
        </div>

        <DataTable
          columns={columns}
          data={appointmentsData}
          actions={[
            { label: "Ver Detalhes", onClick: () => {} },
            { label: "Editar", onClick: () => {} },
            { label: "Cancelar", onClick: () => {}, variant: "destructive" },
          ]}
          pagination={{
            currentPage,
            totalPages: 3,
            onPageChange: setCurrentPage,
          }}
        />
      </div>
    </DashboardLayout>
  );
}
