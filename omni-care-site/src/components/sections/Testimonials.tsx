import { Star, Quote } from "lucide-react";

const testimonials = [
  {
    name: "Dra. Mariana Silva",
    role: "Cardiologista",
    clinic: "Clínica Coração Saudável",
    image: "/placeholder.svg",
    content: "O Omni Care revolucionou a forma como gerencio minha clínica. A telemedicina integrada me permite atender pacientes de todo o Brasil.",
    rating: 5,
  },
  {
    name: "Dr. Ricardo Mendes",
    role: "Diretor Clínico",
    clinic: "Centro Médico Vida",
    image: "/placeholder.svg",
    content: "Reduzimos em 40% as faltas de pacientes com os lembretes automáticos. O sistema de agendamento online é simplesmente perfeito.",
    rating: 5,
  },
  {
    name: "Dra. Ana Paula Costa",
    role: "Dermatologista",
    clinic: "Dermato Estética",
    image: "/placeholder.svg",
    content: "A integração com laboratórios e a prescrição digital economizam horas do meu dia. Consigo focar no que realmente importa: meus pacientes.",
    rating: 5,
  },
];

export const Testimonials = () => {
  return (
    <section className="section-padding">
      <div className="container-custom">
        {/* Header */}
        <div className="text-center max-w-3xl mx-auto mb-16">
          <span className="inline-block px-4 py-1.5 rounded-full bg-primary/10 text-primary text-sm font-medium mb-4">
            Depoimentos
          </span>
          <h2 className="font-display text-3xl sm:text-4xl lg:text-5xl font-bold text-foreground mb-4">
            O que nossos clientes{" "}
            <span className="text-gradient">dizem sobre nós</span>
          </h2>
          <p className="text-lg text-muted-foreground">
            Milhares de profissionais da saúde já transformaram suas clínicas com o Omni Care.
          </p>
        </div>

        {/* Testimonials Grid */}
        <div className="grid md:grid-cols-3 gap-6">
          {testimonials.map((testimonial, index) => (
            <div
              key={testimonial.name}
              className="card-elevated p-6 relative animate-fade-in-up"
              style={{ animationDelay: `${index * 0.1}s` }}
            >
              {/* Quote Icon */}
              <div className="absolute top-6 right-6 text-primary/10">
                <Quote className="w-12 h-12" />
              </div>

              {/* Rating */}
              <div className="flex gap-1 mb-4">
                {Array.from({ length: testimonial.rating }).map((_, i) => (
                  <Star key={i} className="w-5 h-5 fill-amber-400 text-amber-400" />
                ))}
              </div>

              {/* Content */}
              <p className="text-foreground/80 leading-relaxed mb-6 relative z-10">
                "{testimonial.content}"
              </p>

              {/* Author */}
              <div className="flex items-center gap-4">
                <div className="w-12 h-12 rounded-full hero-gradient flex items-center justify-center text-primary-foreground font-bold">
                  {testimonial.name.split(" ").map(n => n[0]).join("").slice(0, 2)}
                </div>
                <div>
                  <p className="font-semibold text-foreground">{testimonial.name}</p>
                  <p className="text-sm text-muted-foreground">{testimonial.role}</p>
                  <p className="text-xs text-primary">{testimonial.clinic}</p>
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Trust Badges */}
        <div className="mt-16 pt-12 border-t border-border">
          <p className="text-center text-muted-foreground mb-8">
            Empresas que confiam no Omni Care
          </p>
          <div className="flex flex-wrap justify-center items-center gap-8 lg:gap-16 opacity-50">
            {["Hospital A", "Clínica B", "Centro C", "Lab D", "Plano E"].map((company) => (
              <div key={company} className="text-foreground/40 font-display font-bold text-xl">
                {company}
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
};
