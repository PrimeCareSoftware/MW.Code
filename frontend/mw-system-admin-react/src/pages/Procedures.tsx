import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { DataTable } from "@/components/ui/data-table";
import { StatusBadge } from "@/components/ui/status-badge";
import { Button } from "@/components/ui/button";
import { Plus, Stethoscope } from "lucide-react";
import { useState } from "react";

const proceduresData = [
  {
    id: 1,
    name: "Limpeza Dental",
    category: "Odontologia",
    duration: "45 min",
    price: "R$ 150,00",
    status: "active",
  },
  {
    id: 2,
    name: "Radiografia Panorâmica",
    category: "Diagnóstico",
    duration: "15 min",
    price: "R$ 80,00",
    status: "active",
  },
  {
    id: 3,
    name: "Extração Simples",
    category: "Cirurgia",
    duration: "30 min",
    price: "R$ 200,00",
    status: "active",
  },
  {
    id: 4,
    name: "Clareamento Dental",
    category: "Estética",
    duration: "60 min",
    price: "R$ 500,00",
    status: "inactive",
  },
  {
    id: 5,
    name: "Canal",
    category: "Endodontia",
    duration: "90 min",
    price: "R$ 800,00",
    status: "active",
  },
];

const columns = [
  {
    key: "name",
    header: "Procedimento",
    render: (item: typeof proceduresData[0]) => (
      <div className="flex items-center gap-3">
        <div className="h-9 w-9 rounded-full bg-primary/10 flex items-center justify-center">
          <Stethoscope className="h-4 w-4 text-primary" />
        </div>
        <span className="font-medium text-foreground">{item.name}</span>
      </div>
    ),
  },
  {
    key: "category",
    header: "Categoria",
  },
  {
    key: "duration",
    header: "Duração",
  },
  {
    key: "price",
    header: "Valor",
    render: (item: typeof proceduresData[0]) => (
      <span className="font-medium text-foreground">{item.price}</span>
    ),
  },
  {
    key: "status",
    header: "Status",
    render: (item: typeof proceduresData[0]) => (
      <StatusBadge
        status={item.status as "active" | "inactive"}
        labels={{
          active: "Ativo",
          inactive: "Inativo",
        }}
      />
    ),
  },
];

export default function Procedures() {
  const [currentPage, setCurrentPage] = useState(1);

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Procedimentos" },
      ]}
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Procedimentos</h1>
            <p className="text-muted-foreground mt-1">
              Gerencie os procedimentos e serviços oferecidos
            </p>
          </div>
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Novo Procedimento
          </Button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Total de Procedimentos</p>
            <p className="text-2xl font-semibold text-foreground mt-1">24</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Ativos</p>
            <p className="text-2xl font-semibold text-success mt-1">20</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Inativos</p>
            <p className="text-2xl font-semibold text-muted-foreground mt-1">4</p>
          </div>
        </div>

        <DataTable
          columns={columns}
          data={proceduresData}
          actions={[
            { label: "Editar", onClick: () => {} },
            { label: "Desativar", onClick: () => {}, variant: "destructive" },
          ]}
          pagination={{
            currentPage,
            totalPages: 2,
            onPageChange: setCurrentPage,
          }}
        />
      </div>
    </DashboardLayout>
  );
}
