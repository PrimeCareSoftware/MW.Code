import { ArrowRight, Play, CheckCircle2 } from "lucide-react";
import { Button } from "@/components/ui/button";

const benefits = [
  "Gestão completa em uma única plataforma",
  "Telemedicina integrada e segura",
  "Suporte 24/7 para sua equipe",
];

export const Hero = () => {
  return (
    <section className="relative overflow-hidden">
      {/* Background Gradient */}
      <div className="absolute inset-0 hero-gradient opacity-5" />
      <div className="absolute top-0 right-0 w-1/2 h-full hero-gradient opacity-10 blur-3xl" />
      
      <div className="container-custom section-padding relative">
        <div className="grid lg:grid-cols-2 gap-12 lg:gap-16 items-center">
          {/* Content */}
          <div className="text-center lg:text-left">
            <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-primary/10 text-primary text-sm font-medium mb-6 animate-fade-in">
              <span className="w-2 h-2 rounded-full bg-primary animate-pulse" />
              Plataforma líder em gestão de saúde
            </div>
            
            <h1 className="font-display text-4xl sm:text-5xl lg:text-6xl font-bold text-foreground leading-tight mb-6 animate-fade-in-up">
              Transforme sua clínica com{" "}
              <span className="text-gradient">gestão completa</span>{" "}
              e telemedicina integrada
            </h1>
            
            <p className="text-lg text-muted-foreground max-w-xl mx-auto lg:mx-0 mb-8 animate-fade-in-up" style={{ animationDelay: "0.1s" }}>
              Simplifique o gerenciamento de consultas, profissionais e pacientes. 
              Tudo em uma única plataforma moderna e intuitiva.
            </p>

            <div className="flex flex-col sm:flex-row gap-4 justify-center lg:justify-start mb-8 animate-fade-in-up" style={{ animationDelay: "0.2s" }}>
              <Button size="lg" className="hero-gradient text-primary-foreground hover:opacity-90 gap-2 text-base px-8">
                Experimente Gratuitamente
                <ArrowRight className="w-5 h-5" />
              </Button>
              <Button size="lg" variant="outline" className="gap-2 text-base px-8 border-2">
                <Play className="w-5 h-5" />
                Fale com um Especialista
              </Button>
            </div>

            <ul className="flex flex-col sm:flex-row gap-4 justify-center lg:justify-start animate-fade-in-up" style={{ animationDelay: "0.3s" }}>
              {benefits.map((benefit) => (
                <li key={benefit} className="flex items-center gap-2 text-sm text-muted-foreground">
                  <CheckCircle2 className="w-5 h-5 text-success shrink-0" />
                  {benefit}
                </li>
              ))}
            </ul>
          </div>

          {/* Hero Image / Illustration */}
          <div className="relative animate-scale-in" style={{ animationDelay: "0.2s" }}>
            <div className="relative rounded-2xl overflow-hidden glow-effect">
              <div className="aspect-square lg:aspect-[4/3] bg-gradient-to-br from-primary/20 via-accent/10 to-secondary rounded-2xl p-8 flex items-center justify-center">
                {/* Abstract Healthcare Illustration */}
                <div className="relative w-full h-full">
                  {/* Main Card */}
                  <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-64 h-40 card-elevated rounded-2xl p-4 animate-float">
                    <div className="flex items-center gap-3 mb-3">
                      <div className="w-10 h-10 rounded-full hero-gradient" />
                      <div>
                        <div className="h-3 w-24 bg-foreground/20 rounded mb-1" />
                        <div className="h-2 w-16 bg-foreground/10 rounded" />
                      </div>
                    </div>
                    <div className="space-y-2">
                      <div className="h-2 w-full bg-foreground/10 rounded" />
                      <div className="h-2 w-3/4 bg-foreground/10 rounded" />
                    </div>
                  </div>

                  {/* Floating Elements */}
                  <div className="absolute top-4 left-4 w-20 h-20 card-elevated rounded-xl p-3 animate-float" style={{ animationDelay: "0.5s" }}>
                    <div className="w-8 h-8 rounded-lg bg-success/20 flex items-center justify-center mb-2">
                      <CheckCircle2 className="w-4 h-4 text-success" />
                    </div>
                    <div className="h-2 w-12 bg-foreground/10 rounded" />
                  </div>

                  <div className="absolute bottom-8 right-4 w-24 h-16 card-elevated rounded-xl p-3 animate-float" style={{ animationDelay: "1s" }}>
                    <div className="h-2 w-16 bg-primary/30 rounded mb-2" />
                    <div className="h-6 w-full bg-primary/20 rounded" />
                  </div>

                  <div className="absolute top-8 right-8 w-16 h-16 rounded-full hero-gradient opacity-80 animate-pulse-glow" />
                </div>
              </div>
            </div>

            {/* Stats */}
            <div className="absolute -bottom-6 left-1/2 -translate-x-1/2 flex gap-4">
              <div className="card-elevated px-6 py-4 text-center">
                <p className="font-display font-bold text-2xl text-primary">500+</p>
                <p className="text-xs text-muted-foreground">Clínicas</p>
              </div>
              <div className="card-elevated px-6 py-4 text-center">
                <p className="font-display font-bold text-2xl text-primary">50k+</p>
                <p className="text-xs text-muted-foreground">Pacientes</p>
              </div>
              <div className="card-elevated px-6 py-4 text-center">
                <p className="font-display font-bold text-2xl text-primary">99.9%</p>
                <p className="text-xs text-muted-foreground">Uptime</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};
