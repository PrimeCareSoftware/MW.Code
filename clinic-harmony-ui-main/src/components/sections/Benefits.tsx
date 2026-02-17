import { Calendar, FileText, Users, DollarSign, Shield, Smartphone } from "lucide-react";

const benefits = [
  {
    icon: Calendar,
    title: "Agendamentos inteligentes",
    description: "Evite conflitos de horário e reduza faltas com lembretes automáticos.",
  },
  {
    icon: FileText,
    title: "Prontuários digitais organizados",
    description: "Histórico completo do paciente, sempre acessível e seguro.",
  },
  {
    icon: Users,
    title: "Gestão eficiente de pacientes",
    description: "Mais controle, melhor atendimento e fidelização de clientes.",
  },
  {
    icon: DollarSign,
    title: "Controle financeiro simplificado",
    description: "Tenha visão clara de receitas, pagamentos e fluxo de caixa.",
  },
  {
    icon: Shield,
    title: "Segurança e confiabilidade",
    description: "Dados organizados seguindo as melhores práticas da área da saúde.",
  },
  {
    icon: Smartphone,
    title: "Acesso em qualquer dispositivo",
    description: "Funciona no computador, tablet ou celular. Onde você precisar.",
  },
];

export const Benefits = () => {
  return (
    <section className="py-20">
      <div className="container mx-auto px-6">
        {/* Header */}
        <div className="text-center max-w-3xl mx-auto mb-16">
          <span className="inline-block px-4 py-1.5 rounded-full bg-primary/10 text-primary text-sm font-medium mb-4">
            Principais benefícios
          </span>
          <h2 className="text-3xl md:text-4xl lg:text-5xl font-semibold text-foreground mb-4">
            Tudo que você precisa para gerenciar sua clínica{" "}
            <span className="text-primary">de forma profissional e eficiente</span>
          </h2>
        </div>

        {/* Benefits Grid */}
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
          {benefits.map((benefit, index) => (
            <div
              key={benefit.title}
              className="group relative p-6 animate-fade-in"
              style={{ animationDelay: `${index * 0.1}s` }}
            >
              {/* Icon */}
              <div className="w-14 h-14 rounded-xl bg-gradient-to-br from-primary to-primary/80 flex items-center justify-center mb-5 group-hover:scale-110 transition-apple">
                <benefit.icon className="w-7 h-7 text-primary-foreground" />
              </div>
              
              {/* Content */}
              <h3 className="font-semibold text-xl text-foreground mb-3">
                {benefit.title}
              </h3>
              
              <p className="text-muted-foreground leading-relaxed">
                {benefit.description}
              </p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};
