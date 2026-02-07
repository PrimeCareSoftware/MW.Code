import { useState } from "react";
import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { FormInput } from "@/components/ui/form-input";
import { FormSelect } from "@/components/ui/form-select";
import { FormTextarea } from "@/components/ui/form-textarea";
import { ToggleSwitch } from "@/components/ui/toggle-switch";
import { Button } from "@/components/ui/button";
import { Save, ArrowLeft, User, MapPin, Phone, FileText } from "lucide-react";
import { useNavigate } from "react-router-dom";

export default function PatientForm() {
  const navigate = useNavigate();
  const [receiveNotifications, setReceiveNotifications] = useState(true);
  const [shareData, setShareData] = useState(false);

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Dashboard", href: "/" },
        { label: "Pacientes", href: "/patients" },
        { label: "Novo Paciente" },
      ]}
      variant="medicwarehouse"
    >
      {/* Page Header */}
      <div className="flex items-center gap-4 mb-8">
        <Button
          variant="ghost"
          size="icon"
          onClick={() => navigate(-1)}
          className="shrink-0"
        >
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <div>
          <h1 className="text-2xl font-semibold text-foreground">Novo Paciente</h1>
          <p className="text-muted-foreground mt-1">Cadastre um novo paciente</p>
        </div>
      </div>

      <form className="max-w-4xl space-y-8">
        {/* Personal Information */}
        <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="flex items-center gap-3 mb-6">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10">
              <User className="h-5 w-5 text-primary" />
            </div>
            <div>
              <h2 className="text-lg font-semibold">Informações Pessoais</h2>
              <p className="text-sm text-muted-foreground">Dados básicos do paciente</p>
            </div>
          </div>

          <div className="grid gap-6 sm:grid-cols-2">
            <FormInput
              label="Nome Completo"
              placeholder="Digite o nome completo"
              required
            />
            <FormInput
              label="CPF"
              placeholder="000.000.000-00"
              required
            />
            <FormInput
              label="Data de Nascimento"
              type="date"
              required
            />
            <FormSelect
              label="Sexo"
              placeholder="Selecione"
              options={[
                { value: "male", label: "Masculino" },
                { value: "female", label: "Feminino" },
                { value: "other", label: "Outro" },
              ]}
            />
            <FormSelect
              label="Estado Civil"
              placeholder="Selecione"
              options={[
                { value: "single", label: "Solteiro(a)" },
                { value: "married", label: "Casado(a)" },
                { value: "divorced", label: "Divorciado(a)" },
                { value: "widowed", label: "Viúvo(a)" },
              ]}
            />
            <FormInput
              label="Profissão"
              placeholder="Digite a profissão"
            />
          </div>
        </div>

        {/* Contact Information */}
        <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="flex items-center gap-3 mb-6">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-accent/10">
              <Phone className="h-5 w-5 text-accent" />
            </div>
            <div>
              <h2 className="text-lg font-semibold">Contato</h2>
              <p className="text-sm text-muted-foreground">Informações de contato</p>
            </div>
          </div>

          <div className="grid gap-6 sm:grid-cols-2">
            <FormInput
              label="E-mail"
              type="email"
              placeholder="email@exemplo.com"
            />
            <FormInput
              label="Telefone"
              placeholder="(00) 00000-0000"
            />
            <FormInput
              label="Telefone Secundário"
              placeholder="(00) 00000-0000"
              hint="Opcional"
            />
            <FormInput
              label="Contato de Emergência"
              placeholder="Nome e telefone"
            />
          </div>
        </div>

        {/* Address */}
        <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="flex items-center gap-3 mb-6">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-success/10">
              <MapPin className="h-5 w-5 text-success" />
            </div>
            <div>
              <h2 className="text-lg font-semibold">Endereço</h2>
              <p className="text-sm text-muted-foreground">Localização do paciente</p>
            </div>
          </div>

          <div className="grid gap-6 sm:grid-cols-3">
            <FormInput
              label="CEP"
              placeholder="00000-000"
            />
            <div className="sm:col-span-2">
              <FormInput
                label="Rua"
                placeholder="Nome da rua"
              />
            </div>
            <FormInput
              label="Número"
              placeholder="000"
            />
            <FormInput
              label="Complemento"
              placeholder="Apto, sala, etc."
            />
            <FormInput
              label="Bairro"
              placeholder="Nome do bairro"
            />
            <FormInput
              label="Cidade"
              placeholder="Nome da cidade"
            />
            <FormSelect
              label="Estado"
              placeholder="Selecione"
              options={[
                { value: "SP", label: "São Paulo" },
                { value: "RJ", label: "Rio de Janeiro" },
                { value: "MG", label: "Minas Gerais" },
                { value: "RS", label: "Rio Grande do Sul" },
              ]}
            />
          </div>
        </div>

        {/* Medical Information */}
        <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <div className="flex items-center gap-3 mb-6">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-warning/10">
              <FileText className="h-5 w-5 text-warning" />
            </div>
            <div>
              <h2 className="text-lg font-semibold">Informações Médicas</h2>
              <p className="text-sm text-muted-foreground">Dados de saúde relevantes</p>
            </div>
          </div>

          <div className="grid gap-6 sm:grid-cols-2">
            <FormSelect
              label="Tipo Sanguíneo"
              placeholder="Selecione"
              options={[
                { value: "A+", label: "A+" },
                { value: "A-", label: "A-" },
                { value: "B+", label: "B+" },
                { value: "B-", label: "B-" },
                { value: "AB+", label: "AB+" },
                { value: "AB-", label: "AB-" },
                { value: "O+", label: "O+" },
                { value: "O-", label: "O-" },
              ]}
            />
            <FormInput
              label="Convênio"
              placeholder="Nome do convênio"
            />
            <FormInput
              label="Número do Convênio"
              placeholder="Número da carteirinha"
            />
            <FormInput
              label="Validade do Convênio"
              type="date"
            />
            <div className="sm:col-span-2">
              <FormTextarea
                label="Alergias"
                placeholder="Liste as alergias conhecidas..."
              />
            </div>
            <div className="sm:col-span-2">
              <FormTextarea
                label="Medicamentos em Uso"
                placeholder="Liste os medicamentos em uso contínuo..."
              />
            </div>
            <div className="sm:col-span-2">
              <FormTextarea
                label="Observações"
                placeholder="Outras informações relevantes..."
              />
            </div>
          </div>
        </div>

        {/* Preferences */}
        <div className="rounded-xl border border-border bg-card p-6 shadow-apple-sm">
          <h2 className="text-lg font-semibold mb-6">Preferências</h2>
          <div className="space-y-6">
            <ToggleSwitch
              checked={receiveNotifications}
              onChange={setReceiveNotifications}
              label="Receber notificações"
              description="Enviar lembretes de consultas por SMS e e-mail"
            />
            <ToggleSwitch
              checked={shareData}
              onChange={setShareData}
              label="Compartilhar dados"
              description="Permitir compartilhamento de prontuário com outros profissionais"
            />
          </div>
        </div>

        {/* Actions */}
        <div className="flex items-center justify-end gap-4 pb-8">
          <Button variant="outline" type="button" onClick={() => navigate(-1)}>
            Cancelar
          </Button>
          <Button type="submit" className="gap-2">
            <Save className="h-4 w-4" />
            Salvar Paciente
          </Button>
        </div>
      </form>
    </DashboardLayout>
  );
}
