import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { DataTable } from "@/components/ui/data-table";
import { StatusBadge } from "@/components/ui/status-badge";
import { Button } from "@/components/ui/button";
import { Plus, User, Mail, Building2, Calendar } from "lucide-react";
import { useState } from "react";

const usersData = [
  {
    id: 1,
    name: "Dr. João Santos",
    email: "joao.santos@clinica.com",
    role: "Médico",
    clinic: "Clínica Central",
    lastAccess: "Há 5 min",
    status: "online",
  },
  {
    id: 2,
    name: "Dra. Ana Lima",
    email: "ana.lima@clinica.com",
    role: "Médica",
    clinic: "Clínica Norte",
    lastAccess: "Há 2 horas",
    status: "offline",
  },
  {
    id: 3,
    name: "Carlos Mendes",
    email: "carlos.mendes@clinica.com",
    role: "Recepcionista",
    clinic: "Clínica Central",
    lastAccess: "Há 30 min",
    status: "online",
  },
  {
    id: 4,
    name: "Fernanda Souza",
    email: "fernanda.souza@clinica.com",
    role: "Administrador",
    clinic: "Todas",
    lastAccess: "Há 1 dia",
    status: "offline",
  },
  {
    id: 5,
    name: "Roberto Alves",
    email: "roberto.alves@clinica.com",
    role: "Técnico",
    clinic: "Clínica Sul",
    lastAccess: "Há 3 dias",
    status: "inactive",
  },
];

const columns = [
  {
    key: "name",
    header: "Usuário",
    render: (item: typeof usersData[0]) => (
      <div className="flex items-center gap-3">
        <div className="relative">
          <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
            <User className="h-5 w-5 text-primary" />
          </div>
          <span
            className={`absolute bottom-0 right-0 h-3 w-3 rounded-full border-2 border-card ${
              item.status === "online"
                ? "bg-success"
                : item.status === "offline"
                ? "bg-muted-foreground"
                : "bg-destructive"
            }`}
          />
        </div>
        <div>
          <p className="font-medium text-foreground">{item.name}</p>
          <p className="text-sm text-muted-foreground">{item.role}</p>
        </div>
      </div>
    ),
  },
  {
    key: "email",
    header: "Email",
    render: (item: typeof usersData[0]) => (
      <div className="flex items-center gap-2 text-muted-foreground">
        <Mail className="h-4 w-4" />
        <span>{item.email}</span>
      </div>
    ),
  },
  {
    key: "clinic",
    header: "Clínica",
    render: (item: typeof usersData[0]) => (
      <div className="flex items-center gap-2 text-muted-foreground">
        <Building2 className="h-4 w-4" />
        <span>{item.clinic}</span>
      </div>
    ),
  },
  {
    key: "lastAccess",
    header: "Último Acesso",
    render: (item: typeof usersData[0]) => (
      <div className="flex items-center gap-2 text-muted-foreground">
        <Calendar className="h-4 w-4" />
        <span>{item.lastAccess}</span>
      </div>
    ),
  },
  {
    key: "status",
    header: "Status",
    render: (item: typeof usersData[0]) => (
      <StatusBadge
        status={item.status as "online" | "offline" | "inactive"}
        labels={{
          online: "Online",
          offline: "Offline",
          inactive: "Inativo",
        }}
      />
    ),
  },
];

export default function Users() {
  const [currentPage, setCurrentPage] = useState(1);

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Usuários" },
      ]}
      variant="admin"
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Usuários</h1>
            <p className="text-muted-foreground mt-1">
              Gerencie os usuários do sistema
            </p>
          </div>
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Novo Usuário
          </Button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Total de Usuários</p>
            <p className="text-2xl font-semibold text-foreground mt-1">48</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Online Agora</p>
            <p className="text-2xl font-semibold text-success mt-1">12</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Médicos</p>
            <p className="text-2xl font-semibold text-primary mt-1">18</p>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <p className="text-sm text-muted-foreground">Administrativos</p>
            <p className="text-2xl font-semibold text-foreground mt-1">30</p>
          </div>
        </div>

        <DataTable
          columns={columns}
          data={usersData}
          actions={[
            { label: "Ver Perfil", onClick: () => {} },
            { label: "Editar", onClick: () => {} },
            { label: "Redefinir Senha", onClick: () => {} },
            { label: "Desativar", onClick: () => {}, variant: "destructive" },
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
