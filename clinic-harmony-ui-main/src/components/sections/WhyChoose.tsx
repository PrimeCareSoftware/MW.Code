import { 
  CheckCircle2, 
  Smartphone, 
  HeartHandshake, 
  TrendingUp, 
  MessageCircle, 
  Cloud,
  FileCheck,
  Clock
} from "lucide-react";

const whyChooseFeatures = [
  {
    icon: Smartphone,
    title: "Interface intuitiva e fácil de usar",
  },
  {
    icon: HeartHandshake,
    title: "Suporte humanizado e rápido",
  },
  {
    icon: TrendingUp,
    title: "Atualizações constantes sem custo extra",
  },
  {
    icon: FileCheck,
    title: "Relatórios e insights automatizados",
  },
  {
    icon: MessageCircle,
    title: "Integração com WhatsApp",
  },
  {
    icon: Cloud,
    title: "Backup automático em nuvem",
  },
];

const advantages = [
  {
    icon: CheckCircle2,
    title: "Mais organização",
    description: "Todos os dados centralizados e acessíveis em segundos",
  },
  {
    icon: FileCheck,
    title: "Menos papel e planilhas",
    description: "Automatize processos e elimine trabalho manual repetitivo",
  },
  {
    icon: Clock,
    title: "Mais tempo para atender melhor",
    description: "Foque no que importa: cuidar dos seus pacientes",
  },
];

export const WhyChoose = () => {
  return (
    <section className="py-20 bg-secondary/30">
      <div className="container mx-auto px-6">
        {/* Header */}
        <div className="text-center max-w-3xl mx-auto mb-16">
          <span className="inline-block px-4 py-1.5 rounded-full bg-primary/10 text-primary text-sm font-medium mb-4">
            Por que escolher
          </span>
          <h2 className="text-3xl md:text-4xl lg:text-5xl font-semibold text-foreground mb-4">
            Por que escolher o{" "}
            <span className="text-primary">MedicWarehouse</span>
          </h2>
          <p className="text-lg text-muted-foreground">
            Enquanto outros sistemas são complicados e engessados, o MedicWarehouse foi criado 
            para simplificar a rotina de quem cuida de pessoas.
          </p>
        </div>

        {/* Features Grid */}
        <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-6 mb-16">
          {whyChooseFeatures.map((feature, index) => (
            <div
              key={feature.title}
              className="flex items-center gap-4 p-4 rounded-2xl border border-border bg-card shadow-apple-sm hover:border-primary/30 transition-apple animate-fade-in"
              style={{ animationDelay: `${index * 0.1}s` }}
            >
              <div className="w-12 h-12 rounded-lg bg-gradient-to-br from-primary to-primary/80 flex items-center justify-center shrink-0">
                <feature.icon className="w-6 h-6 text-primary-foreground" />
              </div>
              <p className="font-medium text-foreground">{feature.title}</p>
            </div>
          ))}
        </div>

        {/* Advantages Section */}
        <div className="grid md:grid-cols-3 gap-8 mt-16">
          {advantages.map((advantage, index) => (
            <div
              key={advantage.title}
              className="text-center p-8 rounded-2xl border border-border bg-card shadow-apple-sm animate-fade-in"
              style={{ animationDelay: `${(index + whyChooseFeatures.length) * 0.1}s` }}
            >
              <div className="inline-flex w-16 h-16 rounded-xl bg-gradient-to-br from-primary to-primary/80 items-center justify-center mb-5">
                <advantage.icon className="w-8 h-8 text-primary-foreground" />
              </div>
              
              <h3 className="font-semibold text-xl text-foreground mb-3">
                {advantage.title}
              </h3>
              
              <p className="text-muted-foreground leading-relaxed">
                {advantage.description}
              </p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};
