import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { DataTable } from "@/components/ui/data-table";
import { StatusBadge } from "@/components/ui/status-badge";
import { Button } from "@/components/ui/button";
import { Plus, Package, AlertTriangle } from "lucide-react";
import { useState } from "react";

const inventoryData = [
  {
    id: 1,
    name: "Luvas Descartáveis (M)",
    category: "EPI",
    quantity: 500,
    minStock: 100,
    unit: "unidades",
    status: "ok",
  },
  {
    id: 2,
    name: "Máscara Cirúrgica",
    category: "EPI",
    quantity: 45,
    minStock: 50,
    unit: "unidades",
    status: "low",
  },
  {
    id: 3,
    name: "Anestésico Local",
    category: "Medicamento",
    quantity: 30,
    minStock: 20,
    unit: "frascos",
    status: "ok",
  },
  {
    id: 4,
    name: "Resina Composta A2",
    category: "Material",
    quantity: 8,
    minStock: 15,
    unit: "seringas",
    status: "critical",
  },
  {
    id: 5,
    name: "Algodão Hidrófilo",
    category: "Consumível",
    quantity: 200,
    minStock: 50,
    unit: "rolos",
    status: "ok",
  },
];

const columns = [
  {
    key: "name",
    header: "Item",
    render: (item: typeof inventoryData[0]) => (
      <div className="flex items-center gap-3">
        <div className="h-9 w-9 rounded-full bg-primary/10 flex items-center justify-center">
          <Package className="h-4 w-4 text-primary" />
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
    key: "quantity",
    header: "Quantidade",
    render: (item: typeof inventoryData[0]) => (
      <div className="flex items-center gap-2">
        <span className="font-medium text-foreground">{item.quantity}</span>
        <span className="text-muted-foreground text-sm">{item.unit}</span>
      </div>
    ),
  },
  {
    key: "minStock",
    header: "Estoque Mínimo",
    render: (item: typeof inventoryData[0]) => (
      <span className="text-muted-foreground">{item.minStock} {item.unit}</span>
    ),
  },
  {
    key: "status",
    header: "Status",
    render: (item: typeof inventoryData[0]) => (
      <StatusBadge
        status={item.status as "ok" | "low" | "critical"}
        labels={{
          ok: "Em Estoque",
          low: "Baixo",
          critical: "Crítico",
        }}
      />
    ),
  },
];

export default function Inventory() {
  const [currentPage, setCurrentPage] = useState(1);

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Inventário" },
      ]}
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Inventário</h1>
            <p className="text-muted-foreground mt-1">
              Controle de estoque e materiais da clínica
            </p>
          </div>
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Adicionar Item
          </Button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Total de Itens</p>
            <p className="text-2xl font-semibold text-foreground mt-1">156</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Em Estoque</p>
            <p className="text-2xl font-semibold text-success mt-1">142</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Estoque Baixo</p>
            <p className="text-2xl font-semibold text-warning mt-1">10</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Crítico</p>
            <p className="text-2xl font-semibold text-destructive mt-1">4</p>
          </div>
        </div>

        <div className="rounded-xl border border-warning/50 bg-warning/5 p-4">
          <div className="flex items-center gap-3">
            <AlertTriangle className="h-5 w-5 text-warning" />
            <div>
              <p className="font-medium text-foreground">Atenção: Itens com estoque baixo</p>
              <p className="text-sm text-muted-foreground">
                4 itens estão abaixo do estoque mínimo e precisam de reposição
              </p>
            </div>
          </div>
        </div>

        <DataTable
          columns={columns}
          data={inventoryData}
          actions={[
            { label: "Editar", onClick: () => {} },
            { label: "Registrar Entrada", onClick: () => {} },
            { label: "Registrar Saída", onClick: () => {} },
          ]}
          pagination={{
            currentPage,
            totalPages: 4,
            onPageChange: setCurrentPage,
          }}
        />
      </div>
    </DashboardLayout>
  );
}
