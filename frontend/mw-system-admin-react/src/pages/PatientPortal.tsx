import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { MetricCard } from "@/components/ui/metric-card";
import { DataTable } from "@/components/ui/data-table";
import { StatusBadge } from "@/components/ui/status-badge";
import { Button } from "@/components/ui/button";
import { Calendar, FileText, Clock, Bell, Plus } from "lucide-react";

interface Appointment {
  id: string;
  date: string;
  time: string;
  doctor: string;
  specialty: string;
  status: "scheduled" | "completed" | "cancelled";
}

const appointments: Appointment[] = [
  { id: "1", date: "15/01/2025", time: "09:00", doctor: "Dr. Carlos Mendes", specialty: "Clínico Geral", status: "scheduled" },
  { id: "2", date: "20/01/2025", time: "14:30", doctor: "Dra. Ana Paula", specialty: "Cardiologia", status: "scheduled" },
  { id: "3", date: "08/01/2025", time: "10:00", doctor: "Dr. Ricardo Silva", specialty: "Ortopedia", status: "completed" },
  { id: "4", date: "02/01/2025", time: "16:00", doctor: "Dra. Fernanda Lima", specialty: "Dermatologia", status: "completed" },
];

const statusVariants: Record<string, "success" | "info" | "destructive"> = {
  scheduled: "info",
  completed: "success",
  cancelled: "destructive",
};

const statusLabels: Record<string, string> = {
  scheduled: "Agendado",
  completed: "Realizado",
  cancelled: "Cancelado",
};

export default function PatientPortal() {
  const tableColumns = [
    { key: "date", header: "Data" },
    { key: "time", header: "Horário" },
    { key: "doctor", header: "Médico" },
    { key: "specialty", header: "Especialidade" },
    {
      key: "status",
      header: "Status",
      render: (item: Appointment) => (
        <StatusBadge variant={statusVariants[item.status]} dot>
          {statusLabels[item.status]}
        </StatusBadge>
      ),
    },
  ];

  return (
    <DashboardLayout
      breadcrumbs={[{ label: "Minha Área" }]}
      variant="patient-portal"
    >
      {/* Welcome Section */}
      <div className="mb-8">
        <h1 className="text-2xl font-semibold text-foreground">Olá, Maria!</h1>
        <p className="text-muted-foreground mt-1">Bem-vindo ao seu portal de saúde</p>
      </div>

      {/* Quick Actions */}
      <div className="flex flex-wrap gap-3 mb-8">
        <Button className="gap-2">
          <Plus className="h-4 w-4" />
          Agendar Consulta
        </Button>
        <Button variant="outline" className="gap-2">
          <FileText className="h-4 w-4" />
          Ver Exames
        </Button>
        <Button variant="outline" className="gap-2">
          <Bell className="h-4 w-4" />
          Notificações
        </Button>
      </div>

      {/* Metrics */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4 mb-8">
        <MetricCard
          title="Próxima Consulta"
          value="15 Jan"
          subtitle="Dr. Carlos Mendes"
          icon={Calendar}
          variant="primary"
        />
        <MetricCard
          title="Consultas Realizadas"
          value="12"
          subtitle="Este ano"
          icon={Clock}
          variant="success"
        />
        <MetricCard
          title="Exames Pendentes"
          value="2"
          subtitle="Aguardando resultado"
          icon={FileText}
          variant="warning"
        />
        <MetricCard
          title="Notificações"
          value="3"
          subtitle="Não lidas"
          icon={Bell}
          variant="accent"
        />
      </div>

      {/* Upcoming Appointments */}
      <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
        <div className="flex items-center justify-between mb-6">
          <div>
            <h2 className="text-lg font-semibold">Minhas Consultas</h2>
            <p className="text-sm text-muted-foreground">Histórico e agendamentos</p>
          </div>
          <Button variant="outline" size="sm">
            Ver todas
          </Button>
        </div>
        <DataTable columns={tableColumns} data={appointments} />
      </div>
    </DashboardLayout>
  );
}
