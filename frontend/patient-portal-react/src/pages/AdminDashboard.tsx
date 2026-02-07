import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { MetricCard } from "@/components/ui/metric-card";
import { DataTable } from "@/components/ui/data-table";
import { StatusBadge } from "@/components/ui/status-badge";
import { AreaChart } from "@/components/charts/AreaChart";
import { Building2, Users, Shield, Activity, Server } from "lucide-react";

interface Clinic {
  id: string;
  name: string;
  location: string;
  users: number;
  status: "active" | "inactive" | "maintenance";
  lastSync: string;
}

const clinics: Clinic[] = [
  { id: "1", name: "Clínica São Paulo", location: "São Paulo, SP", users: 45, status: "active", lastSync: "Há 5 min" },
  { id: "2", name: "Clínica Rio", location: "Rio de Janeiro, RJ", users: 32, status: "active", lastSync: "Há 10 min" },
  { id: "3", name: "Clínica Minas", location: "Belo Horizonte, MG", users: 28, status: "maintenance", lastSync: "Há 2h" },
  { id: "4", name: "Clínica Sul", location: "Porto Alegre, RS", users: 18, status: "active", lastSync: "Há 15 min" },
  { id: "5", name: "Clínica Centro", location: "Brasília, DF", users: 22, status: "inactive", lastSync: "Há 1 dia" },
];

const systemUsageData = [
  { name: "00h", value: 120 },
  { name: "04h", value: 80 },
  { name: "08h", value: 450 },
  { name: "12h", value: 380 },
  { name: "16h", value: 520 },
  { name: "20h", value: 280 },
];

const statusVariants: Record<string, "success" | "warning" | "destructive"> = {
  active: "success",
  inactive: "destructive",
  maintenance: "warning",
};

const statusLabels: Record<string, string> = {
  active: "Ativo",
  inactive: "Inativo",
  maintenance: "Manutenção",
};

export default function AdminDashboard() {
  const tableColumns = [
    {
      key: "name",
      header: "Clínica",
      render: (item: Clinic) => (
        <div>
          <p className="font-medium">{item.name}</p>
          <p className="text-sm text-muted-foreground">{item.location}</p>
        </div>
      ),
    },
    { key: "users", header: "Usuários" },
    { key: "lastSync", header: "Última Sinc." },
    {
      key: "status",
      header: "Status",
      render: (item: Clinic) => (
        <StatusBadge variant={statusVariants[item.status]} dot>
          {statusLabels[item.status]}
        </StatusBadge>
      ),
    },
  ];

  const tableActions = [
    { label: "Ver detalhes", onClick: (item: Clinic) => console.log("Detalhes", item) },
    { label: "Configurar", onClick: (item: Clinic) => console.log("Configurar", item) },
    { label: "Desativar", onClick: (item: Clinic) => console.log("Desativar", item), variant: "destructive" as const },
  ];

  return (
    <DashboardLayout
      breadcrumbs={[{ label: "Administração" }]}
      variant="admin"
    >
      {/* Page Header */}
      <div className="mb-8">
        <h1 className="text-2xl font-semibold text-foreground">Painel Administrativo</h1>
        <p className="text-muted-foreground mt-1">Visão geral do sistema</p>
      </div>

      {/* Metrics Grid */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4 mb-8">
        <MetricCard
          title="Clínicas Ativas"
          value="24"
          subtitle="Em 5 estados"
          trend={{ value: 8, label: "vs. mês anterior", direction: "up" }}
          icon={Building2}
          variant="primary"
        />
        <MetricCard
          title="Usuários Totais"
          value="1,458"
          subtitle="342 online"
          trend={{ value: 15, label: "vs. mês anterior", direction: "up" }}
          icon={Users}
          variant="accent"
        />
        <MetricCard
          title="Permissões"
          value="12"
          subtitle="Grupos de acesso"
          icon={Shield}
          variant="success"
        />
        <MetricCard
          title="Uptime"
          value="99.9%"
          subtitle="Últimos 30 dias"
          icon={Server}
          variant="warning"
        />
      </div>

      {/* Charts and Tables */}
      <div className="grid gap-6 lg:grid-cols-2 mb-8">
        {/* System Usage Chart */}
        <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="mb-6">
            <h3 className="text-lg font-semibold">Uso do Sistema</h3>
            <p className="text-sm text-muted-foreground">Requisições por hora (hoje)</p>
          </div>
          <AreaChart data={systemUsageData} height={280} />
        </div>

        {/* Activity Monitor */}
        <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="mb-6">
            <h3 className="text-lg font-semibold">Monitoramento</h3>
            <p className="text-sm text-muted-foreground">Status dos serviços</p>
          </div>
          <div className="space-y-4">
            {[
              { name: "API Principal", status: "online", latency: "45ms" },
              { name: "Banco de Dados", status: "online", latency: "12ms" },
              { name: "Servidor de E-mail", status: "online", latency: "120ms" },
              { name: "Backup", status: "syncing", latency: "—" },
              { name: "CDN", status: "online", latency: "8ms" },
            ].map((service) => (
              <div key={service.name} className="flex items-center justify-between py-3 border-b border-border last:border-0">
                <div className="flex items-center gap-3">
                  <Activity className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium">{service.name}</span>
                </div>
                <div className="flex items-center gap-4">
                  <span className="text-sm text-muted-foreground">{service.latency}</span>
                  <StatusBadge
                    variant={service.status === "online" ? "success" : "warning"}
                    size="sm"
                    dot
                  >
                    {service.status === "online" ? "Online" : "Sincronizando"}
                  </StatusBadge>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Clinics Table */}
      <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
        <div className="mb-6">
          <h3 className="text-lg font-semibold">Clínicas Cadastradas</h3>
          <p className="text-sm text-muted-foreground">Gerenciamento de unidades</p>
        </div>
        <DataTable columns={tableColumns} data={clinics} actions={tableActions} />
      </div>
    </DashboardLayout>
  );
}
