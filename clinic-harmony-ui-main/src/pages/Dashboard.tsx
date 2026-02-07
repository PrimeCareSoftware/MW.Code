import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { MetricCard } from "@/components/ui/metric-card";
import { DataTable } from "@/components/ui/data-table";
import { StatusBadge } from "@/components/ui/status-badge";
import { AreaChart } from "@/components/charts/AreaChart";
import { BarChart } from "@/components/charts/BarChart";
import { DonutChart } from "@/components/charts/DonutChart";
import { Users, Calendar, Stethoscope, TrendingUp, Clock } from "lucide-react";
import { useState } from "react";

// Sample data
const revenueData = [
  { name: "Jan", value: 4000, value2: 2400 },
  { name: "Fev", value: 3000, value2: 1398 },
  { name: "Mar", value: 2000, value2: 9800 },
  { name: "Abr", value: 2780, value2: 3908 },
  { name: "Mai", value: 1890, value2: 4800 },
  { name: "Jun", value: 2390, value2: 3800 },
];

const appointmentsByDayData = [
  { name: "Seg", value: 24 },
  { name: "Ter", value: 32 },
  { name: "Qua", value: 28 },
  { name: "Qui", value: 35 },
  { name: "Sex", value: 30 },
  { name: "Sáb", value: 12 },
];

const procedureDistribution = [
  { name: "Consultas", value: 45, color: "hsl(211, 84%, 55%)" },
  { name: "Exames", value: 25, color: "hsl(174, 62%, 47%)" },
  { name: "Procedimentos", value: 20, color: "hsl(38, 92%, 50%)" },
  { name: "Retornos", value: 10, color: "hsl(142, 71%, 45%)" },
];

interface Appointment {
  id: string;
  patient: string;
  time: string;
  type: string;
  status: "scheduled" | "in-progress" | "completed" | "cancelled";
  doctor: string;
}

const upcomingAppointments: Appointment[] = [
  { id: "1", patient: "Maria Silva", time: "09:00", type: "Consulta", status: "scheduled", doctor: "Dr. Carlos" },
  { id: "2", patient: "João Santos", time: "09:30", type: "Retorno", status: "in-progress", doctor: "Dra. Ana" },
  { id: "3", patient: "Ana Costa", time: "10:00", type: "Exame", status: "scheduled", doctor: "Dr. Ricardo" },
  { id: "4", patient: "Pedro Lima", time: "10:30", type: "Consulta", status: "scheduled", doctor: "Dra. Paula" },
  { id: "5", patient: "Lucia Fernandes", time: "11:00", type: "Procedimento", status: "completed", doctor: "Dr. Carlos" },
];

const statusVariants: Record<string, "success" | "warning" | "info" | "destructive"> = {
  scheduled: "info",
  "in-progress": "warning",
  completed: "success",
  cancelled: "destructive",
};

const statusLabels: Record<string, string> = {
  scheduled: "Agendado",
  "in-progress": "Em andamento",
  completed: "Concluído",
  cancelled: "Cancelado",
};

export default function Dashboard() {
  const [page, setPage] = useState(1);

  const tableColumns = [
    {
      key: "time",
      header: "Horário",
      render: (item: Appointment) => (
        <div className="flex items-center gap-2">
          <Clock className="h-4 w-4 text-muted-foreground" />
          <span className="font-medium">{item.time}</span>
        </div>
      ),
    },
    { key: "patient", header: "Paciente" },
    { key: "type", header: "Tipo" },
    { key: "doctor", header: "Médico" },
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

  const tableActions = [
    { label: "Ver detalhes", onClick: (item: Appointment) => console.log("Ver", item) },
    { label: "Editar", onClick: (item: Appointment) => console.log("Editar", item) },
    { label: "Cancelar", onClick: (item: Appointment) => console.log("Cancelar", item), variant: "destructive" as const },
  ];

  return (
    <DashboardLayout
      breadcrumbs={[{ label: "Dashboard" }]}
      variant="medicwarehouse"
    >
      {/* Page Header */}
      <div className="mb-8">
        <h1 className="text-2xl font-semibold text-foreground">Dashboard</h1>
        <p className="text-muted-foreground mt-1">Visão geral da clínica</p>
      </div>

      {/* Metrics Grid */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4 mb-8">
        <MetricCard
          title="Pacientes Ativos"
          value="1,284"
          subtitle="Total cadastrado"
          trend={{ value: 12, label: "vs. mês anterior", direction: "up" }}
          icon={Users}
          variant="primary"
        />
        <MetricCard
          title="Consultas Hoje"
          value="32"
          subtitle="8 concluídas"
          trend={{ value: 8, label: "vs. ontem", direction: "up" }}
          icon={Calendar}
          variant="accent"
        />
        <MetricCard
          title="Procedimentos"
          value="156"
          subtitle="Este mês"
          trend={{ value: 5, label: "vs. mês anterior", direction: "down" }}
          icon={Stethoscope}
          variant="success"
        />
        <MetricCard
          title="Receita Mensal"
          value="R$ 45.2k"
          subtitle="Meta: R$ 50k"
          trend={{ value: 15, label: "vs. mês anterior", direction: "up" }}
          icon={TrendingUp}
          variant="warning"
        />
      </div>

      {/* Charts Row */}
      <div className="grid gap-6 lg:grid-cols-3 mb-8">
        {/* Revenue Chart */}
        <div className="lg:col-span-2 rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="mb-6">
            <h3 className="text-lg font-semibold">Receita x Despesas</h3>
            <p className="text-sm text-muted-foreground">Últimos 6 meses</p>
          </div>
          <AreaChart data={revenueData} height={280} />
        </div>

        {/* Procedure Distribution */}
        <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="mb-6">
            <h3 className="text-lg font-semibold">Distribuição</h3>
            <p className="text-sm text-muted-foreground">Por tipo de atendimento</p>
          </div>
          <DonutChart data={procedureDistribution} height={280} />
        </div>
      </div>

      {/* Appointments by Day */}
      <div className="grid gap-6 lg:grid-cols-3 mb-8">
        <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="mb-6">
            <h3 className="text-lg font-semibold">Consultas por Dia</h3>
            <p className="text-sm text-muted-foreground">Esta semana</p>
          </div>
          <BarChart data={appointmentsByDayData} height={220} />
        </div>

        {/* Quick Stats */}
        <div className="lg:col-span-2 rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="mb-6">
            <h3 className="text-lg font-semibold">Próximas Consultas</h3>
            <p className="text-sm text-muted-foreground">Hoje</p>
          </div>
          <DataTable
            columns={tableColumns}
            data={upcomingAppointments}
            actions={tableActions}
            pagination={{
              currentPage: page,
              totalPages: 3,
              onPageChange: setPage,
            }}
          />
        </div>
      </div>
    </DashboardLayout>
  );
}
