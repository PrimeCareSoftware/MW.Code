import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { AreaChart } from "@/components/charts/AreaChart";
import { BarChart } from "@/components/charts/BarChart";
import { DonutChart } from "@/components/charts/DonutChart";
import { Button } from "@/components/ui/button";
import { Download, Calendar, TrendingUp, Users, DollarSign } from "lucide-react";

const revenueData = [
  { name: "Jan", value: 45000 },
  { name: "Fev", value: 52000 },
  { name: "Mar", value: 48000 },
  { name: "Abr", value: 61000 },
  { name: "Mai", value: 55000 },
  { name: "Jun", value: 67000 },
];

const proceduresData = [
  { name: "Limpeza", value: 145 },
  { name: "Consulta", value: 230 },
  { name: "Extração", value: 45 },
  { name: "Canal", value: 32 },
  { name: "Clareamento", value: 28 },
];

const patientTypeData = [
  { name: "Novos", value: 35, color: "hsl(210, 100%, 56%)" },
  { name: "Retorno", value: 45, color: "hsl(142, 76%, 36%)" },
  { name: "Emergência", value: 20, color: "hsl(38, 92%, 50%)" },
];

export default function Reports() {
  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Relatórios" },
      ]}
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Relatórios</h1>
            <p className="text-muted-foreground mt-1">
              Análise de desempenho e métricas da clínica
            </p>
          </div>
          <div className="flex items-center gap-3">
            <Button variant="outline" className="gap-2">
              <Calendar className="h-4 w-4" />
              Últimos 6 meses
            </Button>
            <Button className="gap-2">
              <Download className="h-4 w-4" />
              Exportar
            </Button>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                <DollarSign className="h-5 w-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Receita Total</p>
                <p className="text-xl font-semibold text-foreground">R$ 328.000</p>
              </div>
            </div>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-success/10 flex items-center justify-center">
                <TrendingUp className="h-5 w-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Crescimento</p>
                <p className="text-xl font-semibold text-success">+18.5%</p>
              </div>
            </div>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-info/10 flex items-center justify-center">
                <Users className="h-5 w-5 text-info" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Pacientes Atendidos</p>
                <p className="text-xl font-semibold text-foreground">480</p>
              </div>
            </div>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-warning/10 flex items-center justify-center">
                <Calendar className="h-5 w-5 text-warning" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Consultas</p>
                <p className="text-xl font-semibold text-foreground">892</p>
              </div>
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
            <h3 className="text-lg font-semibold text-foreground mb-4">Receita Mensal</h3>
            <AreaChart data={revenueData} />
          </div>
          <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
            <h3 className="text-lg font-semibold text-foreground mb-4">Procedimentos por Tipo</h3>
            <BarChart data={proceduresData} />
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
            <h3 className="text-lg font-semibold text-foreground mb-4">Tipo de Paciente</h3>
            <DonutChart data={patientTypeData} />
          </div>
          <div className="lg:col-span-2 rounded-xl border border-border bg-card p-6 shadow-apple-sm">
            <h3 className="text-lg font-semibold text-foreground mb-4">Resumo do Período</h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div className="text-center p-4 rounded-lg bg-secondary/30">
                <p className="text-2xl font-bold text-foreground">95%</p>
                <p className="text-sm text-muted-foreground">Taxa de Comparecimento</p>
              </div>
              <div className="text-center p-4 rounded-lg bg-secondary/30">
                <p className="text-2xl font-bold text-foreground">4.8</p>
                <p className="text-sm text-muted-foreground">Avaliação Média</p>
              </div>
              <div className="text-center p-4 rounded-lg bg-secondary/30">
                <p className="text-2xl font-bold text-foreground">32 min</p>
                <p className="text-sm text-muted-foreground">Tempo Médio Consulta</p>
              </div>
              <div className="text-center p-4 rounded-lg bg-secondary/30">
                <p className="text-2xl font-bold text-foreground">R$ 368</p>
                <p className="text-sm text-muted-foreground">Ticket Médio</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
