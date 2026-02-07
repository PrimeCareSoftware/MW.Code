import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { DataTable } from "@/components/ui/data-table";
import { StatusBadge } from "@/components/ui/status-badge";
import { Button } from "@/components/ui/button";
import { Plus, Building2, MapPin, Users, Phone } from "lucide-react";
import { useState } from "react";

const clinicsData = [
  {
    id: 1,
    name: "Clínica Central",
    address: "Av. Paulista, 1000",
    city: "São Paulo",
    phone: "(11) 3333-4444",
    patients: 1250,
    staff: 15,
    status: "active",
  },
  {
    id: 2,
    name: "Clínica Norte",
    address: "Rua das Flores, 200",
    city: "São Paulo",
    phone: "(11) 4444-5555",
    patients: 820,
    staff: 10,
    status: "active",
  },
  {
    id: 3,
    name: "Clínica Sul",
    address: "Av. Santo Amaro, 500",
    city: "São Paulo",
    phone: "(11) 5555-6666",
    patients: 650,
    staff: 8,
    status: "maintenance",
  },
  {
    id: 4,
    name: "Clínica Oeste",
    address: "Rua Oscar Freire, 300",
    city: "São Paulo",
    phone: "(11) 6666-7777",
    patients: 480,
    staff: 6,
    status: "inactive",
  },
];

const columns = [
  {
    key: "name",
    header: "Clínica",
    render: (item: typeof clinicsData[0]) => (
      <div className="flex items-center gap-3">
        <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
          <Building2 className="h-5 w-5 text-primary" />
        </div>
        <div>
          <p className="font-medium text-foreground">{item.name}</p>
          <p className="text-sm text-muted-foreground">{item.city}</p>
        </div>
      </div>
    ),
  },
  {
    key: "address",
    header: "Endereço",
    render: (item: typeof clinicsData[0]) => (
      <div className="flex items-center gap-2 text-muted-foreground">
        <MapPin className="h-4 w-4" />
        <span>{item.address}</span>
      </div>
    ),
  },
  {
    key: "phone",
    header: "Telefone",
    render: (item: typeof clinicsData[0]) => (
      <div className="flex items-center gap-2 text-muted-foreground">
        <Phone className="h-4 w-4" />
        <span>{item.phone}</span>
      </div>
    ),
  },
  {
    key: "patients",
    header: "Pacientes",
    render: (item: typeof clinicsData[0]) => (
      <span className="font-medium text-foreground">{item.patients.toLocaleString()}</span>
    ),
  },
  {
    key: "staff",
    header: "Funcionários",
    render: (item: typeof clinicsData[0]) => (
      <div className="flex items-center gap-2">
        <Users className="h-4 w-4 text-muted-foreground" />
        <span>{item.staff}</span>
      </div>
    ),
  },
  {
    key: "status",
    header: "Status",
    render: (item: typeof clinicsData[0]) => (
      <StatusBadge
        status={item.status as "active" | "inactive" | "maintenance"}
        labels={{
          active: "Ativa",
          inactive: "Inativa",
          maintenance: "Manutenção",
        }}
      />
    ),
  },
];

export default function Clinics() {
  const [currentPage, setCurrentPage] = useState(1);

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Clínicas" },
      ]}
      variant="admin"
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Clínicas</h1>
            <p className="text-muted-foreground mt-1">
              Gerencie as clínicas cadastradas no sistema
            </p>
          </div>
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Nova Clínica
          </Button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Total de Clínicas</p>
            <p className="text-2xl font-semibold text-foreground mt-1">12</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Ativas</p>
            <p className="text-2xl font-semibold text-success mt-1">10</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Em Manutenção</p>
            <p className="text-2xl font-semibold text-warning mt-1">1</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Inativas</p>
            <p className="text-2xl font-semibold text-muted-foreground mt-1">1</p>
          </div>
        </div>

        <DataTable
          columns={columns}
          data={clinicsData}
          actions={[
            { label: "Ver Detalhes", onClick: () => {} },
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
