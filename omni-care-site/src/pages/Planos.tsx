import { Check, X, Sparkles } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Layout } from "@/components/layout/Layout";

const plans = [
  {
    name: "Básico",
    description: "Para pequenas clínicas começando a digitalização",
    price: "199",
    period: "/mês",
    highlighted: false,
    features: [
      { name: "Até 3 profissionais", included: true },
      { name: "Agendamento online", included: true },
      { name: "Prontuário eletrônico", included: true },
      { name: "Lembretes por SMS", included: true },
      { name: "Telemedicina", included: false },
      { name: "Integração com convênios", included: false },
      { name: "Relatórios avançados", included: false },
      { name: "Suporte prioritário", included: false },
    ],
  },
  {
    name: "Profissional",
    description: "O mais popular para clínicas em crescimento",
    price: "399",
    period: "/mês",
    highlighted: true,
    features: [
      { name: "Até 10 profissionais", included: true },
      { name: "Agendamento online", included: true },
      { name: "Prontuário eletrônico", included: true },
      { name: "Lembretes por SMS e WhatsApp", included: true },
      { name: "Telemedicina ilimitada", included: true },
      { name: "Integração com convênios", included: true },
      { name: "Relatórios avançados", included: false },
      { name: "Suporte prioritário", included: false },
    ],
  },
  {
    name: "Enterprise",
    description: "Para redes de clínicas e hospitais",
    price: "Sob consulta",
    period: "",
    highlighted: false,
    features: [
      { name: "Profissionais ilimitados", included: true },
      { name: "Agendamento online", included: true },
      { name: "Prontuário eletrônico", included: true },
      { name: "Lembretes por SMS e WhatsApp", included: true },
      { name: "Telemedicina ilimitada", included: true },
      { name: "Integração com convênios", included: true },
      { name: "Relatórios avançados", included: true },
      { name: "Suporte prioritário 24/7", included: true },
    ],
  },
];

const Planos = () => {
  return (
    <Layout>
      <section className="section-padding">
        <div className="container-custom">
          {/* Header */}
          <div className="text-center max-w-3xl mx-auto mb-16">
            <span className="inline-block px-4 py-1.5 rounded-full bg-primary/10 text-primary text-sm font-medium mb-4">
              Planos e Preços
            </span>
            <h1 className="font-display text-4xl sm:text-5xl font-bold text-foreground mb-4">
              Escolha o plano ideal{" "}
              <span className="text-gradient">para sua clínica</span>
            </h1>
            <p className="text-lg text-muted-foreground">
              Planos flexíveis que crescem com você. Cancele quando quiser.
            </p>
          </div>

          {/* Plans Grid */}
          <div className="grid md:grid-cols-3 gap-6 lg:gap-8">
            {plans.map((plan, index) => (
              <div
                key={plan.name}
                className={`relative rounded-2xl p-6 lg:p-8 animate-fade-in-up ${
                  plan.highlighted
                    ? "bg-foreground text-background border-2 border-foreground"
                    : "card-elevated"
                }`}
                style={{ animationDelay: `${index * 0.1}s` }}
              >
                {plan.highlighted && (
                  <div className="absolute -top-4 left-1/2 -translate-x-1/2 px-4 py-1.5 rounded-full hero-gradient text-primary-foreground text-sm font-medium flex items-center gap-1">
                    <Sparkles className="w-4 h-4" />
                    Mais Popular
                  </div>
                )}

                <div className="mb-6">
                  <h3 className={`font-display font-bold text-xl mb-2 ${plan.highlighted ? "text-background" : "text-foreground"}`}>
                    {plan.name}
                  </h3>
                  <p className={`text-sm ${plan.highlighted ? "text-background/70" : "text-muted-foreground"}`}>
                    {plan.description}
                  </p>
                </div>

                <div className="mb-6">
                  <span className={`font-display font-bold text-4xl ${plan.highlighted ? "text-background" : "text-foreground"}`}>
                    {plan.price.includes("consulta") ? "" : "R$"}
                    {plan.price}
                  </span>
                  <span className={plan.highlighted ? "text-background/70" : "text-muted-foreground"}>
                    {plan.period}
                  </span>
                </div>

                <Button
                  className={`w-full mb-6 ${
                    plan.highlighted
                      ? "bg-primary text-primary-foreground hover:bg-primary/90"
                      : "hero-gradient text-primary-foreground hover:opacity-90"
                  }`}
                  size="lg"
                >
                  {plan.price.includes("consulta") ? "Falar com Vendas" : "Assinar Agora"}
                </Button>

                <ul className="space-y-3">
                  {plan.features.map((feature) => (
                    <li key={feature.name} className="flex items-center gap-3">
                      {feature.included ? (
                        <Check className={`w-5 h-5 ${plan.highlighted ? "text-primary" : "text-success"}`} />
                      ) : (
                        <X className={`w-5 h-5 ${plan.highlighted ? "text-background/30" : "text-muted-foreground/50"}`} />
                      )}
                      <span className={`text-sm ${
                        feature.included
                          ? plan.highlighted ? "text-background" : "text-foreground"
                          : plan.highlighted ? "text-background/40" : "text-muted-foreground"
                      }`}>
                        {feature.name}
                      </span>
                    </li>
                  ))}
                </ul>
              </div>
            ))}
          </div>

          {/* FAQ CTA */}
          <div className="text-center mt-16">
            <p className="text-muted-foreground mb-4">
              Tem dúvidas sobre qual plano escolher?
            </p>
            <Button variant="outline" size="lg">
              Fale com um Especialista
            </Button>
          </div>
        </div>
      </section>
    </Layout>
  );
};

export default Planos;
