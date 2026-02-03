import { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { Menu, X, ChevronDown, Calendar, User, Building2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

const navLinks = [
  { name: "Home", path: "/" },
  { name: "Planos", path: "/planos" },
  { name: "Clínicas", path: "/clinicas" },
  { name: "Blog", path: "/blog" },
  { name: "Contato", path: "/contato" },
];

export const Navbar = () => {
  const [isOpen, setIsOpen] = useState(false);
  const location = useLocation();

  const isActive = (path: string) => location.pathname === path;

  return (
    <nav className="fixed top-0 left-0 right-0 z-50 bg-background/80 backdrop-blur-lg border-b border-border/50">
      <div className="container-custom">
        <div className="flex items-center justify-between h-16 lg:h-20">
          {/* Logo */}
          <Link to="/" className="flex items-center gap-2">
            <div className="w-10 h-10 rounded-xl hero-gradient flex items-center justify-center">
              <span className="text-primary-foreground font-bold text-lg">O</span>
            </div>
            <span className="font-display font-bold text-xl text-foreground">
              Omni <span className="text-primary">Care</span>
            </span>
          </Link>

          {/* Desktop Navigation */}
          <div className="hidden lg:flex items-center gap-1">
            {navLinks.map((link) => (
              <Link
                key={link.path}
                to={link.path}
                className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
                  isActive(link.path)
                    ? "text-primary bg-primary/10"
                    : "text-muted-foreground hover:text-foreground hover:bg-muted"
                }`}
              >
                {link.name}
              </Link>
            ))}
          </div>

          {/* Desktop Actions */}
          <div className="hidden lg:flex items-center gap-3">
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" className="gap-2">
                  Acessar
                  <ChevronDown className="w-4 h-4" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-56 bg-popover border border-border shadow-lg">
                <DropdownMenuItem asChild>
                  <Link to="/login" className="flex items-center gap-3 cursor-pointer">
                    <Building2 className="w-4 h-4 text-primary" />
                    <div>
                      <p className="font-medium">Login no Sistema</p>
                      <p className="text-xs text-muted-foreground">Para clínicas e profissionais</p>
                    </div>
                  </Link>
                </DropdownMenuItem>
                <DropdownMenuItem asChild>
                  <Link to="/portal-paciente" className="flex items-center gap-3 cursor-pointer">
                    <User className="w-4 h-4 text-primary" />
                    <div>
                      <p className="font-medium">Portal do Paciente</p>
                      <p className="text-xs text-muted-foreground">Acesse seus agendamentos</p>
                    </div>
                  </Link>
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>

            <Button className="gap-2 hero-gradient text-primary-foreground hover:opacity-90 transition-opacity">
              <Calendar className="w-4 h-4" />
              Agendar Consulta
            </Button>
          </div>

          {/* Mobile Menu Button */}
          <button
            onClick={() => setIsOpen(!isOpen)}
            className="lg:hidden p-2 rounded-lg hover:bg-muted transition-colors"
          >
            {isOpen ? (
              <X className="w-6 h-6 text-foreground" />
            ) : (
              <Menu className="w-6 h-6 text-foreground" />
            )}
          </button>
        </div>

        {/* Mobile Menu */}
        {isOpen && (
          <div className="lg:hidden py-4 border-t border-border animate-fade-in">
            <div className="flex flex-col gap-1">
              {navLinks.map((link) => (
                <Link
                  key={link.path}
                  to={link.path}
                  onClick={() => setIsOpen(false)}
                  className={`px-4 py-3 rounded-lg text-sm font-medium transition-colors ${
                    isActive(link.path)
                      ? "text-primary bg-primary/10"
                      : "text-muted-foreground hover:text-foreground hover:bg-muted"
                  }`}
                >
                  {link.name}
                </Link>
              ))}
              <div className="border-t border-border my-2" />
              <Link
                to="/login"
                onClick={() => setIsOpen(false)}
                className="px-4 py-3 rounded-lg text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-muted flex items-center gap-3"
              >
                <Building2 className="w-4 h-4" />
                Login no Sistema
              </Link>
              <Link
                to="/portal-paciente"
                onClick={() => setIsOpen(false)}
                className="px-4 py-3 rounded-lg text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-muted flex items-center gap-3"
              >
                <User className="w-4 h-4" />
                Portal do Paciente
              </Link>
              <div className="px-4 pt-2">
                <Button className="w-full gap-2 hero-gradient text-primary-foreground">
                  <Calendar className="w-4 h-4" />
                  Agendar Consulta
                </Button>
              </div>
            </div>
          </div>
        )}
      </div>
    </nav>
  );
};
