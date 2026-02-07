import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { Button } from "@/components/ui/button";
import { ToggleSwitch } from "@/components/ui/toggle-switch";
import {
  Server,
  Database,
  HardDrive,
  Cpu,
  Activity,
  RefreshCw,
  Download,
  Upload,
  AlertTriangle,
  CheckCircle,
  Clock,
} from "lucide-react";
import { useState } from "react";

const systemStats = {
  cpu: 45,
  memory: 68,
  storage: 52,
  uptime: "45 dias, 12 horas",
  lastBackup: "Há 2 horas",
  version: "2.5.1",
};

const services = [
  { name: "API Principal", status: "running", latency: "45ms" },
  { name: "Banco de Dados", status: "running", latency: "12ms" },
  { name: "Cache Redis", status: "running", latency: "3ms" },
  { name: "Serviço de Email", status: "running", latency: "120ms" },
  { name: "Backup Automático", status: "running", latency: "-" },
  { name: "Processamento de Filas", status: "warning", latency: "250ms" },
];

const recentLogs = [
  { time: "14:32:15", level: "info", message: "Backup automático concluído com sucesso" },
  { time: "14:30:00", level: "info", message: "Sincronização de dados finalizada" },
  { time: "14:25:42", level: "warning", message: "Alta latência detectada no serviço de filas" },
  { time: "14:20:00", level: "info", message: "Limpeza de cache executada" },
  { time: "14:15:33", level: "info", message: "Novo usuário registrado: carlos.mendes@clinica.com" },
];

export default function System() {
  const [maintenanceMode, setMaintenanceMode] = useState(false);
  const [autoBackup, setAutoBackup] = useState(true);

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Sistema" },
      ]}
      variant="admin"
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Sistema</h1>
            <p className="text-muted-foreground mt-1">
              Monitoramento e configurações do sistema
            </p>
          </div>
          <div className="flex items-center gap-3">
            <Button variant="outline" className="gap-2">
              <Download className="h-4 w-4" />
              Backup Manual
            </Button>
            <Button className="gap-2">
              <RefreshCw className="h-4 w-4" />
              Reiniciar Serviços
            </Button>
          </div>
        </div>

        {/* System Overview */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3 mb-3">
              <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                <Cpu className="h-5 w-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">CPU</p>
                <p className="text-xl font-semibold text-foreground">{systemStats.cpu}%</p>
              </div>
            </div>
            <div className="h-2 bg-secondary rounded-full overflow-hidden">
              <div
                className="h-full bg-primary rounded-full transition-all"
                style={{ width: `${systemStats.cpu}%` }}
              />
            </div>
          </div>

          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3 mb-3">
              <div className="h-10 w-10 rounded-full bg-info/10 flex items-center justify-center">
                <Database className="h-5 w-5 text-info" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Memória</p>
                <p className="text-xl font-semibold text-foreground">{systemStats.memory}%</p>
              </div>
            </div>
            <div className="h-2 bg-secondary rounded-full overflow-hidden">
              <div
                className="h-full bg-info rounded-full transition-all"
                style={{ width: `${systemStats.memory}%` }}
              />
            </div>
          </div>

          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3 mb-3">
              <div className="h-10 w-10 rounded-full bg-success/10 flex items-center justify-center">
                <HardDrive className="h-5 w-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Armazenamento</p>
                <p className="text-xl font-semibold text-foreground">{systemStats.storage}%</p>
              </div>
            </div>
            <div className="h-2 bg-secondary rounded-full overflow-hidden">
              <div
                className="h-full bg-success rounded-full transition-all"
                style={{ width: `${systemStats.storage}%` }}
              />
            </div>
          </div>

          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-warning/10 flex items-center justify-center">
                <Clock className="h-5 w-5 text-warning" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Uptime</p>
                <p className="text-lg font-semibold text-foreground">{systemStats.uptime}</p>
              </div>
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Services Status */}
          <div className="rounded-xl border border-border bg-card shadow-apple-sm">
            <div className="p-4 border-b border-border flex items-center justify-between">
              <h3 className="font-semibold text-foreground">Status dos Serviços</h3>
              <Button variant="ghost" size="sm" className="gap-2">
                <RefreshCw className="h-4 w-4" />
                Atualizar
              </Button>
            </div>
            <div className="divide-y divide-border">
              {services.map((service) => (
                <div key={service.name} className="flex items-center justify-between p-4">
                  <div className="flex items-center gap-3">
                    {service.status === "running" ? (
                      <CheckCircle className="h-5 w-5 text-success" />
                    ) : (
                      <AlertTriangle className="h-5 w-5 text-warning" />
                    )}
                    <span className="font-medium text-foreground">{service.name}</span>
                  </div>
                  <div className="flex items-center gap-4">
                    <span className="text-sm text-muted-foreground">{service.latency}</span>
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-medium ${
                        service.status === "running"
                          ? "bg-success/10 text-success"
                          : "bg-warning/10 text-warning"
                      }`}
                    >
                      {service.status === "running" ? "Online" : "Atenção"}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* System Settings */}
          <div className="rounded-xl border border-border bg-card shadow-apple-sm">
            <div className="p-4 border-b border-border">
              <h3 className="font-semibold text-foreground">Configurações do Sistema</h3>
            </div>
            <div className="p-4 space-y-4">
              <ToggleSwitch
                label="Modo de Manutenção"
                description="Desativa o acesso ao sistema para usuários comuns"
                checked={maintenanceMode}
                onChange={setMaintenanceMode}
              />
              <ToggleSwitch
                label="Backup Automático"
                description="Realiza backup diário às 03:00"
                checked={autoBackup}
                onChange={setAutoBackup}
              />
              <div className="pt-4 border-t border-border">
                <div className="flex items-center justify-between mb-2">
                  <span className="text-sm font-medium text-foreground">Versão do Sistema</span>
                  <span className="text-sm text-muted-foreground">v{systemStats.version}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-sm font-medium text-foreground">Último Backup</span>
                  <span className="text-sm text-muted-foreground">{systemStats.lastBackup}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Recent Logs */}
        <div className="rounded-xl border border-border bg-card shadow-apple-sm">
          <div className="p-4 border-b border-border flex items-center justify-between">
            <h3 className="font-semibold text-foreground">Logs Recentes</h3>
            <Button variant="ghost" size="sm">
              Ver Todos
            </Button>
          </div>
          <div className="divide-y divide-border">
            {recentLogs.map((log, index) => (
              <div key={index} className="flex items-start gap-4 p-4">
                <span className="text-xs text-muted-foreground font-mono whitespace-nowrap">
                  {log.time}
                </span>
                <span
                  className={`px-2 py-0.5 rounded text-xs font-medium ${
                    log.level === "info"
                      ? "bg-info/10 text-info"
                      : log.level === "warning"
                      ? "bg-warning/10 text-warning"
                      : "bg-destructive/10 text-destructive"
                  }`}
                >
                  {log.level.toUpperCase()}
                </span>
                <span className="text-sm text-foreground">{log.message}</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
