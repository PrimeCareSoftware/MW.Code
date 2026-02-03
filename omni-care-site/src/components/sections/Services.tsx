import { Building2, Calendar, Video, Layers, ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/button";

const services = [
  {
    icon: Building2,
    title: "Gerenciamento de Clínicas",
    description: "Controle completo de múltiplas unidades, profissionais, salas e equipamentos em uma única plataforma.",
    features: ["Multi-unidades", "Controle de estoque", "Relatórios gerenciais"],
  },
  {
    icon: Calendar,
    title: "Agendamento de Consultas",
    description: "Sistema inteligente de agendamentos com lembretes automáticos e confirmação por WhatsApp.",
    features: ["Agenda online 24/7", "Lembretes automáticos", "Lista de espera"],
  },
  {
    icon: Video,
    title: "Telemedicina",
    description: "Consultas por vídeo com segurança, integração de prontuário e prescrição digital.",
    features: ["Videochamadas HD", "Prontuário integrado", "Prescrição digital"],
  },
  {
    icon: Layers,
    title: "Integração com Sistemas",
    description: "Conecte-se com laboratórios, convênios e sistemas de pagamento de forma simplificada.",
    features: ["APIs abertas", "Integração PIX", "Convênios"],
  },
];

export const Services = () => {
  return (
    <section className="section-padding bg-secondary/30">
      <div className="container-custom">
        {/* Header */}
        <div className="text-center max-w-3xl mx-auto mb-16">
          <span className="inline-block px-4 py-1.5 rounded-full bg-primary/10 text-primary text-sm font-medium mb-4">
            Nossos Serviços
          </span>
          <h2 className="font-display text-3xl sm:text-4xl lg:text-5xl font-bold text-foreground mb-4">
            Tudo que sua clínica precisa em{" "}
            <span className="text-gradient">um só lugar</span>
          </h2>
          <p className="text-lg text-muted-foreground">
            Soluções completas para modernizar a gestão da sua clínica e 
            oferecer a melhor experiência aos seus pacientes.
          </p>
        </div>

        {/* Services Grid */}
        <div className="grid sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {services.map((service, index) => (
            <div
              key={service.title}
              className="group card-elevated p-6 hover:border-primary/30 transition-all duration-300 animate-fade-in-up"
              style={{ animationDelay: `${index * 0.1}s` }}
            >
              <div className="w-14 h-14 rounded-xl hero-gradient flex items-center justify-center mb-5 group-hover:scale-110 transition-transform duration-300">
                <service.icon className="w-7 h-7 text-primary-foreground" />
              </div>
              
              <h3 className="font-display font-semibold text-lg text-foreground mb-2">
                {service.title}
              </h3>
              
              <p className="text-muted-foreground text-sm mb-4 leading-relaxed">
                {service.description}
              </p>

              <ul className="space-y-2 mb-4">
                {service.features.map((feature) => (
                  <li key={feature} className="flex items-center gap-2 text-sm text-muted-foreground">
                    <div className="w-1.5 h-1.5 rounded-full bg-primary" />
                    {feature}
                  </li>
                ))}
              </ul>

              <Button variant="ghost" className="p-0 h-auto text-primary hover:text-primary/80 group/btn">
                Saiba mais
                <ArrowRight className="w-4 h-4 ml-1 group-hover/btn:translate-x-1 transition-transform" />
              </Button>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};
