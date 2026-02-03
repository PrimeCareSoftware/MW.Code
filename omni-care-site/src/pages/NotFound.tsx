import { Link, useLocation } from "react-router-dom";
import { useEffect } from "react";
import { Home, ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";

const NotFound = () => {
  const location = useLocation();

  useEffect(() => {
    console.error("404 Error: User attempted to access non-existent route:", location.pathname);
  }, [location.pathname]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="text-center px-4">
        <div className="w-24 h-24 rounded-2xl hero-gradient flex items-center justify-center mx-auto mb-8">
          <span className="text-5xl text-primary-foreground font-display font-bold">404</span>
        </div>
        <h1 className="font-display text-3xl sm:text-4xl font-bold text-foreground mb-4">
          Página não encontrada
        </h1>
        <p className="text-muted-foreground mb-8 max-w-md mx-auto">
          A página que você está procurando não existe ou foi movida para outro endereço.
        </p>
        <div className="flex flex-col sm:flex-row gap-4 justify-center">
          <Button asChild className="gap-2 hero-gradient text-primary-foreground hover:opacity-90">
            <Link to="/">
              <Home className="w-4 h-4" />
              Voltar ao Início
            </Link>
          </Button>
          <Button asChild variant="outline" className="gap-2">
            <Link to="/contato">
              <ArrowLeft className="w-4 h-4" />
              Fale Conosco
            </Link>
          </Button>
        </div>
      </div>
    </div>
  );
};

export default NotFound;
