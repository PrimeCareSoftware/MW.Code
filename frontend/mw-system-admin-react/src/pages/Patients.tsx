import { useState } from "react";
import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { DataTable } from "@/components/ui/data-table";
import { StatusBadge } from "@/components/ui/status-badge";
import { FormInput } from "@/components/ui/form-input";
import { FormSelect } from "@/components/ui/form-select";
import { Button } from "@/components/ui/button";
import { Plus, Search, Filter, Download, Upload } from "lucide-react";

interface Patient {
  id: string;
  name: string;
  email: string;
  phone: string;
  birthDate: string;
  lastVisit: string;
  status: "active" | "inactive" | "pending";
}

const patients: Patient[] = [
  { id: "1", name: "Maria Silva", email: "maria@email.com", phone: "(11) 99999-1234", birthDate: "15/03/1985", lastVisit: "10/01/2025", status: "active" },
  { id: "2", name: "João Santos", email: "joao@email.com", phone: "(11) 99999-5678", birthDate: "22/07/1990", lastVisit: "08/01/2025", status: "active" },
  { id: "3", name: "Ana Costa", email: "ana@email.com", phone: "(11) 99999-9012", birthDate: "10/11/1978", lastVisit: "05/01/2025", status: "pending" },
  { id: "4", name: "Pedro Lima", email: "pedro@email.com", phone: "(11) 99999-3456", birthDate: "03/05/1995", lastVisit: "02/01/2025", status: "active" },
  { id: "5", name: "Lucia Fernandes", email: "lucia@email.com", phone: "(11) 99999-7890", birthDate: "28/09/1982", lastVisit: "28/12/2024", status: "inactive" },
  { id: "6", name: "Carlos Oliveira", email: "carlos@email.com", phone: "(11) 99999-2345", birthDate: "14/02/1988", lastVisit: "20/12/2024", status: "active" },
  { id: "7", name: "Fernanda Souza", email: "fernanda@email.com", phone: "(11) 99999-6789", birthDate: "07/08/1992", lastVisit: "15/12/2024", status: "active" },
  { id: "8", name: "Ricardo Almeida", email: "ricardo@email.com", phone: "(11) 99999-0123", birthDate: "19/12/1975", lastVisit: "10/12/2024", status: "pending" },
];

const statusVariants: Record<string, "success" | "warning" | "destructive"> = {
  active: "success",
  inactive: "destructive",
  pending: "warning",
};

const statusLabels: Record<string, string> = {
  active: "Ativo",
  inactive: "Inativo",
  pending: "Pendente",
};

export default function Patients() {
  const [page, setPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("");

  const filteredPatients = patients.filter((patient) => {
    const matchesSearch = patient.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      patient.email.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = !statusFilter || patient.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  const tableColumns = [
    {
      key: "name",
      header: "Paciente",
      render: (item: Patient) => (
        <div>
          <p className="font-medium">{item.name}</p>
          <p className="text-sm text-muted-foreground">{item.email}</p>
        </div>
      ),
    },
    { key: "phone", header: "Telefone" },
    { key: "birthDate", header: "Data de Nascimento" },
    { key: "lastVisit", header: "Última Visita" },
    {
      key: "status",
      header: "Status",
      render: (item: Patient) => (
        <StatusBadge variant={statusVariants[item.status]} dot>
          {statusLabels[item.status]}
        </StatusBadge>
      ),
    },
  ];

  const tableActions = [
    { label: "Ver prontuário", onClick: (item: Patient) => console.log("Prontuário", item) },
    { label: "Agendar consulta", onClick: (item: Patient) => console.log("Agendar", item) },
    { label: "Editar", onClick: (item: Patient) => console.log("Editar", item) },
    { label: "Desativar", onClick: (item: Patient) => console.log("Desativar", item), variant: "destructive" as const },
  ];

  return (
    <DashboardLayout
      breadcrumbs={[{ label: "Dashboard", href: "/" }, { label: "Pacientes" }]}
      variant="medicwarehouse"
    >
      {/* Page Header */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between mb-8">
        <div>
          <h1 className="text-2xl font-semibold text-foreground">Pacientes</h1>
          <p className="text-muted-foreground mt-1">Gerenciamento de pacientes da clínica</p>
        </div>
        <div className="flex gap-3">
          <Button variant="outline" size="sm" className="gap-2">
            <Download className="h-4 w-4" />
            Exportar
          </Button>
          <Button variant="outline" size="sm" className="gap-2">
            <Upload className="h-4 w-4" />
            Importar
          </Button>
          <Button size="sm" className="gap-2">
            <Plus className="h-4 w-4" />
            Novo Paciente
          </Button>
        </div>
      </div>

      {/* Filters */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end mb-6">
        <div className="flex-1 max-w-sm">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <input
              type="text"
              placeholder="Buscar pacientes..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="flex h-10 w-full rounded-lg border border-input bg-card pl-9 pr-4 text-sm placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20 transition-apple"
            />
          </div>
        </div>
        <div className="w-full sm:w-48">
          <FormSelect
            options={[
              { value: "", label: "Todos os status" },
              { value: "active", label: "Ativos" },
              { value: "inactive", label: "Inativos" },
              { value: "pending", label: "Pendentes" },
            ]}
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
          />
        </div>
        <Button variant="outline" size="icon" className="shrink-0">
          <Filter className="h-4 w-4" />
        </Button>
      </div>

      {/* Results count */}
      <p className="text-sm text-muted-foreground mb-4">
        {filteredPatients.length} pacientes encontrados
      </p>

      {/* Table */}
      <DataTable
        columns={tableColumns}
        data={filteredPatients}
        actions={tableActions}
        onRowClick={(item) => console.log("Clicked", item)}
        pagination={{
          currentPage: page,
          totalPages: Math.ceil(filteredPatients.length / 10),
          onPageChange: setPage,
        }}
      />
    </DashboardLayout>
  );
}
