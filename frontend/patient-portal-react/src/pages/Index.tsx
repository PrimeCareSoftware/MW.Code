import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { 
  Stethoscope, 
  Users, 
  Shield, 
  ArrowRight,
  Building2,
  UserCircle,
  Settings
} from "lucide-react";

const systems = [
  {
    id: "medicwarehouse",
    title: "MedicWarehouse",
    description: "Sistema completo de gestão de clínicas médicas",
    icon: Stethoscope,
    path: "/dashboard",
    color: "bg-primary",
    features: ["Dashboard analítico", "Gestão de pacientes", "Agendamentos", "Prontuários"],
  },
  {
    id: "patient-portal",
    title: "Portal do Paciente",
    description: "Acesso do paciente às suas informações e agendamentos",
    icon: UserCircle,
    path: "/patient-portal",
    color: "bg-accent",
    features: ["Minhas consultas", "Exames", "Histórico médico", "Agendamento online"],
  },
  {
    id: "admin",
    title: "System Admin",
    description: "Administração central do sistema",
    icon: Shield,
    path: "/admin",
    color: "bg-warning",
    features: ["Gestão de clínicas", "Usuários", "Permissões", "Configurações"],
  },
];

export default function Index() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-background">
      {/* Header */}
      <header className="border-b border-border bg-card/80 backdrop-blur-xl sticky top-0 z-50">
        <div className="container mx-auto flex h-16 items-center justify-between px-6">
          <div className="flex items-center gap-3">
            <div className="h-9 w-9 rounded-lg bg-primary flex items-center justify-center">
              <Stethoscope className="h-5 w-5 text-primary-foreground" />
            </div>
            <span className="text-lg font-semibold">MedicWarehouse</span>
          </div>
          <nav className="hidden md:flex items-center gap-6">
            <a href="#systems" className="text-sm text-muted-foreground hover:text-foreground transition-apple">
              Sistemas
            </a>
            <a href="#features" className="text-sm text-muted-foreground hover:text-foreground transition-apple">
              Recursos
            </a>
            <a href="#contact" className="text-sm text-muted-foreground hover:text-foreground transition-apple">
              Contato
            </a>
          </nav>
          <Button variant="outline" size="sm">
            Entrar
          </Button>
        </div>
      </header>

      {/* Hero Section */}
      <section className="relative overflow-hidden py-20 lg:py-32">
        <div className="absolute inset-0 bg-gradient-to-br from-primary/5 via-transparent to-accent/5" />
        <div className="container mx-auto px-6 relative">
          <div className="max-w-3xl mx-auto text-center animate-fade-in">
            <div className="inline-flex items-center gap-2 rounded-full bg-primary/10 px-4 py-1.5 text-sm text-primary mb-6">
              <span className="h-1.5 w-1.5 rounded-full bg-primary animate-pulse-soft" />
              Plataforma de Gestão Médica
            </div>
            <h1 className="text-4xl md:text-5xl lg:text-6xl font-semibold tracking-tight text-foreground mb-6">
              Gestão de clínicas{" "}
              <span className="text-primary">simplificada</span>
            </h1>
            <p className="text-lg text-muted-foreground mb-8 max-w-2xl mx-auto">
              Uma suíte completa de sistemas para gerenciar sua clínica médica com 
              interface minimalista inspirada nos padrões Apple.
            </p>
            <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
              <Button size="lg" className="gap-2 w-full sm:w-auto" onClick={() => navigate("/dashboard")}>
                Acessar Dashboard
                <ArrowRight className="h-4 w-4" />
              </Button>
              <Button variant="outline" size="lg" className="w-full sm:w-auto">
                Ver Documentação
              </Button>
            </div>
          </div>
        </div>
      </section>

      {/* Systems Section */}
      <section id="systems" className="py-20 bg-secondary/30">
        <div className="container mx-auto px-6">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-semibold mb-4">Sistemas Disponíveis</h2>
            <p className="text-muted-foreground max-w-xl mx-auto">
              Três sistemas integrados para cobrir todas as necessidades da sua clínica
            </p>
          </div>

          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3 max-w-5xl mx-auto">
            {systems.map((system, index) => (
              <div
                key={system.id}
                className="group rounded-2xl border border-border bg-card p-6 shadow-apple-sm transition-apple card-hover animate-fade-in"
                style={{ animationDelay: `${index * 100}ms` }}
              >
                <div className={`h-12 w-12 rounded-xl ${system.color} flex items-center justify-center mb-5`}>
                  <system.icon className="h-6 w-6 text-primary-foreground" />
                </div>
                <h3 className="text-xl font-semibold mb-2">{system.title}</h3>
                <p className="text-muted-foreground text-sm mb-5">{system.description}</p>
                <ul className="space-y-2 mb-6">
                  {system.features.map((feature) => (
                    <li key={feature} className="flex items-center gap-2 text-sm text-muted-foreground">
                      <span className="h-1.5 w-1.5 rounded-full bg-primary" />
                      {feature}
                    </li>
                  ))}
                </ul>
                <Button
                  variant="outline"
                  className="w-full gap-2 group-hover:bg-primary group-hover:text-primary-foreground transition-apple"
                  onClick={() => navigate(system.path)}
                >
                  Acessar
                  <ArrowRight className="h-4 w-4" />
                </Button>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section id="features" className="py-20">
        <div className="container mx-auto px-6">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-semibold mb-4">Design Minimalista</h2>
            <p className="text-muted-foreground max-w-xl mx-auto">
              Interface limpa e elegante inspirada nos padrões de design da Apple
            </p>
          </div>

          <div className="grid gap-8 md:grid-cols-2 lg:grid-cols-4 max-w-5xl mx-auto">
            {[
              { icon: Building2, title: "Responsivo", desc: "Funciona em qualquer dispositivo" },
              { icon: Settings, title: "Modular", desc: "Componentes reutilizáveis" },
              { icon: Users, title: "Multi-usuário", desc: "Controle de acesso granular" },
              { icon: Shield, title: "Seguro", desc: "Proteção de dados sensíveis" },
            ].map((feature, index) => (
              <div
                key={feature.title}
                className="text-center p-6 animate-fade-in"
                style={{ animationDelay: `${index * 100}ms` }}
              >
                <div className="h-14 w-14 rounded-2xl bg-secondary flex items-center justify-center mx-auto mb-4">
                  <feature.icon className="h-6 w-6 text-foreground" />
                </div>
                <h3 className="font-semibold mb-2">{feature.title}</h3>
                <p className="text-sm text-muted-foreground">{feature.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t border-border py-12">
        <div className="container mx-auto px-6">
          <div className="flex flex-col md:flex-row items-center justify-between gap-4">
            <div className="flex items-center gap-3">
              <div className="h-8 w-8 rounded-lg bg-primary flex items-center justify-center">
                <Stethoscope className="h-4 w-4 text-primary-foreground" />
              </div>
              <span className="font-semibold">MedicWarehouse</span>
            </div>
            <p className="text-sm text-muted-foreground">
              © 2025 MedicWarehouse. Todos os direitos reservados.
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
}
