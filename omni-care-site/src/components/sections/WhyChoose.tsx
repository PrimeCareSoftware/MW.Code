import { 
  CheckCircle2, 
  Smartphone, 
  HeartHandshake, 
  TrendingUp, 
  MessageCircle, 
  Cloud,
  FileCheck,
  Calendar,
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
    <section className="section-padding bg-secondary/30">
      <div className="container-custom">
        {/* Header */}
        <div className="text-center max-w-3xl mx-auto mb-16">
          <span className="inline-block px-4 py-1.5 rounded-full bg-primary/10 text-primary text-sm font-medium mb-4">
            Por que escolher
          </span>
          <h2 className="font-display text-3xl sm:text-4xl lg:text-5xl font-bold text-foreground mb-4">
            Por que escolher o{" "}
            <span className="text-gradient">Omni Care</span>
          </h2>
          <p className="text-lg text-muted-foreground">
            Enquanto outros sistemas são complicados e engessados, o Omni Care foi criado 
            para simplificar a rotina de quem cuida de pessoas (e animais).
          </p>
        </div>

        {/* Features Grid */}
        <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-6 mb-16">
          {whyChooseFeatures.map((feature, index) => (
            <div
              key={feature.title}
              className="flex items-center gap-4 p-4 card-elevated hover:border-primary/30 transition-all duration-300 animate-fade-in-up"
              style={{ animationDelay: `${index * 0.1}s` }}
            >
              <div className="w-12 h-12 rounded-lg hero-gradient flex items-center justify-center shrink-0">
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
              className="text-center p-8 card-elevated animate-fade-in-up"
              style={{ animationDelay: `${(index + whyChooseFeatures.length) * 0.1}s` }}
            >
              <div className="inline-flex w-16 h-16 rounded-xl hero-gradient items-center justify-center mb-5">
                <advantage.icon className="w-8 h-8 text-primary-foreground" />
              </div>
              
              <h3 className="font-display font-semibold text-xl text-foreground mb-3">
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
