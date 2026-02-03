import { ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/button";

export const CTA = () => {
  return (
    <section className="section-padding">
      <div className="container-custom">
        <div className="relative overflow-hidden rounded-3xl hero-gradient p-8 md:p-12 lg:p-16">
          {/* Background Decoration */}
          <div className="absolute top-0 right-0 w-96 h-96 bg-white/10 rounded-full blur-3xl -translate-y-1/2 translate-x-1/2" />
          <div className="absolute bottom-0 left-0 w-64 h-64 bg-white/5 rounded-full blur-2xl translate-y-1/2 -translate-x-1/2" />
          
          <div className="relative z-10 text-center max-w-3xl mx-auto">
            <h2 className="font-display text-3xl sm:text-4xl lg:text-5xl font-bold text-primary-foreground mb-4">
              Pronto para transformar sua clínica?
            </h2>
            <p className="text-primary-foreground/80 text-lg mb-8">
              Comece gratuitamente e descubra como o Omni Care pode revolucionar 
              a gestão da sua clínica em apenas alguns minutos.
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Button size="lg" variant="secondary" className="gap-2 text-base px-8">
                Começar Agora Gratuitamente
                <ArrowRight className="w-5 h-5" />
              </Button>
              <Button size="lg" variant="outline" className="gap-2 text-base px-8 bg-transparent border-2 border-primary-foreground/30 text-primary-foreground hover:bg-primary-foreground/10">
                Agendar Demonstração
              </Button>
            </div>
            <p className="text-primary-foreground/60 text-sm mt-6">
              Sem cartão de crédito. Configuração em 5 minutos.
            </p>
          </div>
        </div>
      </div>
    </section>
  );
};
