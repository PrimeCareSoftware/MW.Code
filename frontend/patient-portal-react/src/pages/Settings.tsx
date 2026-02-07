import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { FormInput } from "@/components/ui/form-input";
import { FormSelect } from "@/components/ui/form-select";
import { FormTextarea } from "@/components/ui/form-textarea";
import { ToggleSwitch } from "@/components/ui/toggle-switch";
import { Button } from "@/components/ui/button";
import { useState } from "react";
import { User, Bell, Shield, Palette, Globe, Building2 } from "lucide-react";

export default function Settings() {
  const [notifications, setNotifications] = useState({
    email: true,
    sms: false,
    push: true,
  });

  const [activeTab, setActiveTab] = useState("profile");

  const tabs = [
    { id: "profile", label: "Perfil", icon: User },
    { id: "clinic", label: "Clínica", icon: Building2 },
    { id: "notifications", label: "Notificações", icon: Bell },
    { id: "security", label: "Segurança", icon: Shield },
    { id: "appearance", label: "Aparência", icon: Palette },
    { id: "language", label: "Idioma", icon: Globe },
  ];

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Configurações" },
      ]}
    >
      <div className="space-y-6">
        <div>
          <h1 className="text-2xl font-semibold text-foreground">Configurações</h1>
          <p className="text-muted-foreground mt-1">
            Gerencie suas preferências e configurações do sistema
          </p>
        </div>

        <div className="flex flex-col lg:flex-row gap-6">
          {/* Sidebar */}
          <div className="lg:w-64 shrink-0">
            <nav className="space-y-1">
              {tabs.map((tab) => (
                <button
                  key={tab.id}
                  onClick={() => setActiveTab(tab.id)}
                  className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg text-sm font-medium transition-apple ${
                    activeTab === tab.id
                      ? "bg-primary text-primary-foreground"
                      : "text-muted-foreground hover:bg-secondary"
                  }`}
                >
                  <tab.icon className="h-5 w-5" />
                  {tab.label}
                </button>
              ))}
            </nav>
          </div>

          {/* Content */}
          <div className="flex-1">
            <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
              {activeTab === "profile" && (
                <div className="space-y-6">
                  <h2 className="text-lg font-semibold text-foreground">Informações do Perfil</h2>
                  <div className="flex items-center gap-6">
                    <div className="h-20 w-20 rounded-full bg-primary/10 flex items-center justify-center">
                      <User className="h-10 w-10 text-primary" />
                    </div>
                    <div>
                      <Button variant="outline" size="sm">Alterar Foto</Button>
                      <p className="text-xs text-muted-foreground mt-2">JPG, PNG ou GIF. Máximo 2MB.</p>
                    </div>
                  </div>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <FormInput label="Nome Completo" defaultValue="Dr. João Santos" />
                    <FormInput label="Email" type="email" defaultValue="joao.santos@clinica.com" />
                    <FormInput label="Telefone" defaultValue="(11) 99999-9999" />
                    <FormInput label="CRM" defaultValue="123456-SP" />
                  </div>
                  <FormTextarea label="Biografia" placeholder="Conte um pouco sobre você..." rows={4} />
                  <div className="flex justify-end gap-3">
                    <Button variant="outline">Cancelar</Button>
                    <Button>Salvar Alterações</Button>
                  </div>
                </div>
              )}

              {activeTab === "clinic" && (
                <div className="space-y-6">
                  <h2 className="text-lg font-semibold text-foreground">Dados da Clínica</h2>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <FormInput label="Nome da Clínica" defaultValue="Clínica Odontológica Sorriso" />
                    <FormInput label="CNPJ" defaultValue="12.345.678/0001-90" />
                    <FormInput label="Telefone" defaultValue="(11) 3333-4444" />
                    <FormInput label="Email" defaultValue="contato@clinicasorriso.com" />
                  </div>
                  <FormTextarea label="Endereço" defaultValue="Av. Paulista, 1000 - Bela Vista, São Paulo - SP" />
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <FormInput label="CEP" defaultValue="01310-100" />
                    <FormInput label="Cidade" defaultValue="São Paulo" />
                    <FormSelect
                      label="Estado"
                      options={[
                        { value: "SP", label: "São Paulo" },
                        { value: "RJ", label: "Rio de Janeiro" },
                        { value: "MG", label: "Minas Gerais" },
                      ]}
                      defaultValue="SP"
                    />
                  </div>
                  <div className="flex justify-end gap-3">
                    <Button variant="outline">Cancelar</Button>
                    <Button>Salvar Alterações</Button>
                  </div>
                </div>
              )}

              {activeTab === "notifications" && (
                <div className="space-y-6">
                  <h2 className="text-lg font-semibold text-foreground">Preferências de Notificação</h2>
                  <div className="space-y-4">
                    <ToggleSwitch
                      label="Notificações por Email"
                      description="Receba atualizações e lembretes por email"
                      checked={notifications.email}
                      onChange={(checked) => setNotifications({ ...notifications, email: checked })}
                    />
                    <ToggleSwitch
                      label="Notificações por SMS"
                      description="Receba lembretes de consultas por SMS"
                      checked={notifications.sms}
                      onChange={(checked) => setNotifications({ ...notifications, sms: checked })}
                    />
                    <ToggleSwitch
                      label="Notificações Push"
                      description="Receba notificações em tempo real no navegador"
                      checked={notifications.push}
                      onChange={(checked) => setNotifications({ ...notifications, push: checked })}
                    />
                  </div>
                  <div className="flex justify-end gap-3">
                    <Button>Salvar Preferências</Button>
                  </div>
                </div>
              )}

              {activeTab === "security" && (
                <div className="space-y-6">
                  <h2 className="text-lg font-semibold text-foreground">Segurança</h2>
                  <div className="space-y-4">
                    <FormInput label="Senha Atual" type="password" />
                    <FormInput label="Nova Senha" type="password" />
                    <FormInput label="Confirmar Nova Senha" type="password" />
                  </div>
                  <div className="pt-4 border-t border-border">
                    <h3 className="font-medium text-foreground mb-4">Autenticação de Dois Fatores</h3>
                    <ToggleSwitch
                      label="Ativar 2FA"
                      description="Adicione uma camada extra de segurança à sua conta"
                      checked={false}
                      onChange={() => {}}
                    />
                  </div>
                  <div className="flex justify-end gap-3">
                    <Button>Atualizar Senha</Button>
                  </div>
                </div>
              )}

              {activeTab === "appearance" && (
                <div className="space-y-6">
                  <h2 className="text-lg font-semibold text-foreground">Aparência</h2>
                  <FormSelect
                    label="Tema"
                    options={[
                      { value: "light", label: "Claro" },
                      { value: "dark", label: "Escuro" },
                      { value: "system", label: "Seguir Sistema" },
                    ]}
                    defaultValue="light"
                  />
                  <FormSelect
                    label="Densidade da Interface"
                    options={[
                      { value: "compact", label: "Compacta" },
                      { value: "normal", label: "Normal" },
                      { value: "comfortable", label: "Confortável" },
                    ]}
                    defaultValue="normal"
                  />
                  <div className="flex justify-end gap-3">
                    <Button>Salvar Preferências</Button>
                  </div>
                </div>
              )}

              {activeTab === "language" && (
                <div className="space-y-6">
                  <h2 className="text-lg font-semibold text-foreground">Idioma e Região</h2>
                  <FormSelect
                    label="Idioma"
                    options={[
                      { value: "pt-BR", label: "Português (Brasil)" },
                      { value: "en-US", label: "English (US)" },
                      { value: "es", label: "Español" },
                    ]}
                    defaultValue="pt-BR"
                  />
                  <FormSelect
                    label="Formato de Data"
                    options={[
                      { value: "DD/MM/YYYY", label: "DD/MM/AAAA" },
                      { value: "MM/DD/YYYY", label: "MM/DD/AAAA" },
                      { value: "YYYY-MM-DD", label: "AAAA-MM-DD" },
                    ]}
                    defaultValue="DD/MM/YYYY"
                  />
                  <FormSelect
                    label="Fuso Horário"
                    options={[
                      { value: "America/Sao_Paulo", label: "São Paulo (GMT-3)" },
                      { value: "America/New_York", label: "New York (GMT-5)" },
                      { value: "Europe/London", label: "London (GMT+0)" },
                    ]}
                    defaultValue="America/Sao_Paulo"
                  />
                  <div className="flex justify-end gap-3">
                    <Button>Salvar Preferências</Button>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
