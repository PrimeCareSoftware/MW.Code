import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { FormInput } from "@/components/ui/form-input";
import { Stethoscope, Eye, EyeOff, Mail, Lock, User, Phone } from "lucide-react";

export default function Register() {
  const navigate = useNavigate();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phone: "",
    password: "",
    confirmPassword: "",
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    
    // Simulate registration - replace with actual auth logic
    setTimeout(() => {
      setIsLoading(false);
      navigate("/dashboard");
    }, 1000);
  };

  return (
    <div className="min-h-screen bg-background flex">
      {/* Left side - Branding */}
      <div className="hidden lg:flex lg:w-1/2 bg-gradient-to-br from-accent via-accent/90 to-accent/80 p-12 flex-col justify-between">
        <div className="flex items-center gap-3">
          <div className="h-10 w-10 rounded-xl bg-white/20 backdrop-blur-sm flex items-center justify-center">
            <Stethoscope className="h-6 w-6 text-white" />
          </div>
          <span className="text-xl font-semibold text-white">MedicWarehouse</span>
        </div>
        
        <div className="space-y-6">
          <h1 className="text-4xl font-semibold text-white leading-tight">
            Comece sua jornada com a gente
          </h1>
          <p className="text-white/80 text-lg max-w-md">
            Crie sua conta e tenha acesso a todas as funcionalidades 
            da plataforma de gestão médica mais completa.
          </p>
          <ul className="space-y-3">
            {[
              "Dashboard analítico em tempo real",
              "Gestão completa de pacientes",
              "Agendamento inteligente",
              "Prontuários eletrônicos seguros",
            ].map((feature) => (
              <li key={feature} className="flex items-center gap-3 text-white/90">
                <span className="h-1.5 w-1.5 rounded-full bg-white" />
                {feature}
              </li>
            ))}
          </ul>
        </div>

        <p className="text-white/60 text-sm">
          © 2025 MedicWarehouse. Todos os direitos reservados.
        </p>
      </div>

      {/* Right side - Register Form */}
      <div className="flex-1 flex items-center justify-center p-6 lg:p-12 overflow-y-auto">
        <div className="w-full max-w-md space-y-6 animate-fade-in py-8">
          {/* Mobile Logo */}
          <div className="lg:hidden flex items-center justify-center gap-3 mb-6">
            <div className="h-10 w-10 rounded-xl bg-primary flex items-center justify-center">
              <Stethoscope className="h-6 w-6 text-primary-foreground" />
            </div>
            <span className="text-xl font-semibold">MedicWarehouse</span>
          </div>

          <div className="text-center lg:text-left">
            <h2 className="text-2xl font-semibold text-foreground">
              Crie sua conta
            </h2>
            <p className="text-muted-foreground mt-2">
              Preencha os dados abaixo para começar
            </p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="relative">
              <FormInput
                label="Nome completo"
                type="text"
                placeholder="Seu nome"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                required
                className="pl-10"
              />
              <User className="absolute left-3 top-[38px] h-4 w-4 text-muted-foreground" />
            </div>

            <div className="relative">
              <FormInput
                label="E-mail"
                type="email"
                placeholder="seu@email.com"
                value={formData.email}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                required
                className="pl-10"
              />
              <Mail className="absolute left-3 top-[38px] h-4 w-4 text-muted-foreground" />
            </div>

            <div className="relative">
              <FormInput
                label="Telefone"
                type="tel"
                placeholder="(11) 99999-9999"
                value={formData.phone}
                onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                className="pl-10"
              />
              <Phone className="absolute left-3 top-[38px] h-4 w-4 text-muted-foreground" />
            </div>

            <div className="relative">
              <FormInput
                label="Senha"
                type={showPassword ? "text" : "password"}
                placeholder="••••••••"
                value={formData.password}
                onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                required
                hint="Mínimo de 8 caracteres"
                className="pl-10 pr-10"
              />
              <Lock className="absolute left-3 top-[38px] h-4 w-4 text-muted-foreground" />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-[38px] text-muted-foreground hover:text-foreground transition-apple"
              >
                {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
              </button>
            </div>

            <div className="relative">
              <FormInput
                label="Confirmar senha"
                type={showConfirmPassword ? "text" : "password"}
                placeholder="••••••••"
                value={formData.confirmPassword}
                onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
                required
                className="pl-10 pr-10"
              />
              <Lock className="absolute left-3 top-[38px] h-4 w-4 text-muted-foreground" />
              <button
                type="button"
                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                className="absolute right-3 top-[38px] text-muted-foreground hover:text-foreground transition-apple"
              >
                {showConfirmPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
              </button>
            </div>

            <label className="flex items-start gap-3 cursor-pointer">
              <input 
                type="checkbox" 
                required
                className="h-4 w-4 mt-0.5 rounded border-input text-primary focus:ring-primary/20"
              />
              <span className="text-sm text-muted-foreground">
                Eu concordo com os{" "}
                <a href="#" className="text-primary hover:text-primary/80">Termos de Uso</a>
                {" "}e{" "}
                <a href="#" className="text-primary hover:text-primary/80">Política de Privacidade</a>
              </span>
            </label>

            <Button 
              type="submit" 
              className="w-full h-11" 
              disabled={isLoading}
            >
              {isLoading ? "Criando conta..." : "Criar conta"}
            </Button>
          </form>

          <div className="relative">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-border" />
            </div>
            <div className="relative flex justify-center text-xs uppercase">
              <span className="bg-background px-2 text-muted-foreground">
                Ou cadastre-se com
              </span>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <Button variant="outline" className="h-11">
              <svg className="h-5 w-5 mr-2" viewBox="0 0 24 24">
                <path
                  fill="currentColor"
                  d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                />
                <path
                  fill="currentColor"
                  d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                />
                <path
                  fill="currentColor"
                  d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                />
                <path
                  fill="currentColor"
                  d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                />
              </svg>
              Google
            </Button>
            <Button variant="outline" className="h-11">
              <svg className="h-5 w-5 mr-2" fill="currentColor" viewBox="0 0 24 24">
                <path d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z" />
              </svg>
              X
            </Button>
          </div>

          <p className="text-center text-sm text-muted-foreground">
            Já tem uma conta?{" "}
            <Link 
              to="/login" 
              className="text-primary hover:text-primary/80 font-medium transition-apple"
            >
              Entrar
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
