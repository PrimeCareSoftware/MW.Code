import { useState } from "react";
import { Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { FormInput } from "@/components/ui/form-input";
import { Stethoscope, Mail, ArrowLeft, CheckCircle } from "lucide-react";

export default function ForgotPassword() {
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [email, setEmail] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    
    // Simulate password reset request
    setTimeout(() => {
      setIsLoading(false);
      setIsSubmitted(true);
    }, 1000);
  };

  return (
    <div className="min-h-screen bg-background flex items-center justify-center p-6">
      <div className="w-full max-w-md space-y-8 animate-fade-in">
        {/* Logo */}
        <div className="flex items-center justify-center gap-3 mb-8">
          <div className="h-12 w-12 rounded-xl bg-primary flex items-center justify-center">
            <Stethoscope className="h-7 w-7 text-primary-foreground" />
          </div>
        </div>

        {!isSubmitted ? (
          <>
            <div className="text-center">
              <h2 className="text-2xl font-semibold text-foreground">
                Esqueceu sua senha?
              </h2>
              <p className="text-muted-foreground mt-2 max-w-sm mx-auto">
                Não se preocupe! Digite seu e-mail e enviaremos instruções para redefinir sua senha.
              </p>
            </div>

            <form onSubmit={handleSubmit} className="space-y-5">
              <div className="relative">
                <FormInput
                  label="E-mail"
                  type="email"
                  placeholder="seu@email.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className="pl-10"
                />
                <Mail className="absolute left-3 top-[38px] h-4 w-4 text-muted-foreground" />
              </div>

              <Button 
                type="submit" 
                className="w-full h-11" 
                disabled={isLoading}
              >
                {isLoading ? "Enviando..." : "Enviar instruções"}
              </Button>
            </form>
          </>
        ) : (
          <div className="text-center space-y-6">
            <div className="h-16 w-16 rounded-full bg-success/10 flex items-center justify-center mx-auto">
              <CheckCircle className="h-8 w-8 text-success" />
            </div>
            <div>
              <h2 className="text-2xl font-semibold text-foreground">
                E-mail enviado!
              </h2>
              <p className="text-muted-foreground mt-2 max-w-sm mx-auto">
                Enviamos as instruções de recuperação para{" "}
                <span className="font-medium text-foreground">{email}</span>.
                Verifique sua caixa de entrada.
              </p>
            </div>
            <div className="pt-4 space-y-3">
              <p className="text-sm text-muted-foreground">
                Não recebeu o e-mail?{" "}
                <button 
                  onClick={() => setIsSubmitted(false)}
                  className="text-primary hover:text-primary/80 font-medium transition-apple"
                >
                  Tentar novamente
                </button>
              </p>
            </div>
          </div>
        )}

        <div className="pt-4">
          <Link 
            to="/login" 
            className="flex items-center justify-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-apple"
          >
            <ArrowLeft className="h-4 w-4" />
            Voltar para o login
          </Link>
        </div>

        {/* Footer */}
        <p className="text-center text-xs text-muted-foreground pt-8">
          © 2025 MedicWarehouse. Todos os direitos reservados.
        </p>
      </div>
    </div>
  );
}
