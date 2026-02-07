import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { Button } from "@/components/ui/button";
import { Plus, Shield, Check, X, Users } from "lucide-react";
import { useState } from "react";

const roles = [
  {
    id: 1,
    name: "Administrador",
    description: "Acesso total ao sistema",
    users: 3,
    permissions: {
      dashboard: { view: true, edit: true },
      patients: { view: true, edit: true, delete: true },
      appointments: { view: true, edit: true, delete: true },
      reports: { view: true, export: true },
      settings: { view: true, edit: true },
      users: { view: true, edit: true, delete: true },
    },
  },
  {
    id: 2,
    name: "Médico",
    description: "Acesso a pacientes e consultas",
    users: 18,
    permissions: {
      dashboard: { view: true, edit: false },
      patients: { view: true, edit: true, delete: false },
      appointments: { view: true, edit: true, delete: false },
      reports: { view: true, export: false },
      settings: { view: false, edit: false },
      users: { view: false, edit: false, delete: false },
    },
  },
  {
    id: 3,
    name: "Recepcionista",
    description: "Gerenciamento de agendamentos",
    users: 12,
    permissions: {
      dashboard: { view: true, edit: false },
      patients: { view: true, edit: false, delete: false },
      appointments: { view: true, edit: true, delete: false },
      reports: { view: false, export: false },
      settings: { view: false, edit: false },
      users: { view: false, edit: false, delete: false },
    },
  },
  {
    id: 4,
    name: "Técnico",
    description: "Suporte técnico e manutenção",
    users: 5,
    permissions: {
      dashboard: { view: true, edit: false },
      patients: { view: false, edit: false, delete: false },
      appointments: { view: false, edit: false, delete: false },
      reports: { view: true, export: true },
      settings: { view: true, edit: true },
      users: { view: true, edit: false, delete: false },
    },
  },
];

const permissionLabels: Record<string, string> = {
  dashboard: "Dashboard",
  patients: "Pacientes",
  appointments: "Consultas",
  reports: "Relatórios",
  settings: "Configurações",
  users: "Usuários",
};

const actionLabels: Record<string, string> = {
  view: "Visualizar",
  edit: "Editar",
  delete: "Excluir",
  export: "Exportar",
};

export default function Permissions() {
  const [selectedRole, setSelectedRole] = useState(roles[0]);

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Permissões" },
      ]}
      variant="admin"
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Permissões</h1>
            <p className="text-muted-foreground mt-1">
              Gerencie os perfis de acesso e permissões do sistema
            </p>
          </div>
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Novo Perfil
          </Button>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          {/* Roles List */}
          <div className="lg:col-span-1">
            <div className="rounded-xl border border-border bg-card shadow-apple-sm overflow-hidden">
              <div className="p-4 border-b border-border">
                <h3 className="font-semibold text-foreground">Perfis de Acesso</h3>
              </div>
              <div className="divide-y divide-border">
                {roles.map((role) => (
                  <button
                    key={role.id}
                    onClick={() => setSelectedRole(role)}
                    className={`w-full flex items-center gap-3 p-4 text-left transition-apple ${
                      selectedRole.id === role.id
                        ? "bg-primary/10"
                        : "hover:bg-secondary/50"
                    }`}
                  >
                    <div
                      className={`h-10 w-10 rounded-lg flex items-center justify-center ${
                        selectedRole.id === role.id
                          ? "bg-primary text-primary-foreground"
                          : "bg-secondary"
                      }`}
                    >
                      <Shield className="h-5 w-5" />
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="font-medium text-foreground truncate">{role.name}</p>
                      <p className="text-xs text-muted-foreground flex items-center gap-1">
                        <Users className="h-3 w-3" />
                        {role.users} usuários
                      </p>
                    </div>
                  </button>
                ))}
              </div>
            </div>
          </div>

          {/* Permissions Matrix */}
          <div className="lg:col-span-3">
            <div className="rounded-xl border border-border bg-card shadow-apple-sm">
              <div className="p-6 border-b border-border">
                <div className="flex items-center justify-between">
                  <div>
                    <h3 className="text-lg font-semibold text-foreground">
                      {selectedRole.name}
                    </h3>
                    <p className="text-sm text-muted-foreground">
                      {selectedRole.description}
                    </p>
                  </div>
                  <Button variant="outline" size="sm">
                    Editar Perfil
                  </Button>
                </div>
              </div>

              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="border-b border-border bg-secondary/30">
                      <th className="px-6 py-4 text-left text-xs font-semibold uppercase tracking-wider text-muted-foreground">
                        Módulo
                      </th>
                      <th className="px-6 py-4 text-center text-xs font-semibold uppercase tracking-wider text-muted-foreground">
                        Visualizar
                      </th>
                      <th className="px-6 py-4 text-center text-xs font-semibold uppercase tracking-wider text-muted-foreground">
                        Editar
                      </th>
                      <th className="px-6 py-4 text-center text-xs font-semibold uppercase tracking-wider text-muted-foreground">
                        Excluir/Exportar
                      </th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-border">
                    {Object.entries(selectedRole.permissions).map(([module, perms]) => {
                      const hasEdit = "edit" in perms && perms.edit;
                      const hasDeleteOrExport = ("delete" in perms && perms.delete) || ("export" in perms && perms.export);
                      return (
                      <tr key={module} className="hover:bg-secondary/30 transition-apple">
                        <td className="px-6 py-4">
                          <span className="font-medium text-foreground">
                            {permissionLabels[module]}
                          </span>
                        </td>
                        <td className="px-6 py-4 text-center">
                          {perms.view ? (
                            <span className="inline-flex h-6 w-6 items-center justify-center rounded-full bg-success/10">
                              <Check className="h-4 w-4 text-success" />
                            </span>
                          ) : (
                            <span className="inline-flex h-6 w-6 items-center justify-center rounded-full bg-destructive/10">
                              <X className="h-4 w-4 text-destructive" />
                            </span>
                          )}
                        </td>
                        <td className="px-6 py-4 text-center">
                          {hasEdit ? (
                            <span className="inline-flex h-6 w-6 items-center justify-center rounded-full bg-success/10">
                              <Check className="h-4 w-4 text-success" />
                            </span>
                          ) : (
                            <span className="inline-flex h-6 w-6 items-center justify-center rounded-full bg-destructive/10">
                              <X className="h-4 w-4 text-destructive" />
                            </span>
                          )}
                        </td>
                        <td className="px-6 py-4 text-center">
                          {hasDeleteOrExport ? (
                            <span className="inline-flex h-6 w-6 items-center justify-center rounded-full bg-success/10">
                              <Check className="h-4 w-4 text-success" />
                            </span>
                          ) : (
                            <span className="inline-flex h-6 w-6 items-center justify-center rounded-full bg-destructive/10">
                              <X className="h-4 w-4 text-destructive" />
                            </span>
                          )}
                        </td>
                      </tr>
                    )})}
                  </tbody>
                </table>
              </div>

              <div className="p-4 border-t border-border flex justify-end gap-3">
                <Button variant="outline">Cancelar</Button>
                <Button>Salvar Alterações</Button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
