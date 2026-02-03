import { useState } from "react";
import { Mail, Phone, MapPin, Send, Clock, CheckCircle2 } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { Layout } from "@/components/layout/Layout";
import { useToast } from "@/hooks/use-toast";
import { z } from "zod";

const contactSchema = z.object({
  name: z.string().trim().min(1, "Nome é obrigatório").max(100, "Nome deve ter menos de 100 caracteres"),
  email: z.string().trim().email("Email inválido").max(255, "Email deve ter menos de 255 caracteres"),
  phone: z.string().trim().max(20, "Telefone deve ter menos de 20 caracteres").optional(),
  subject: z.string().trim().min(1, "Assunto é obrigatório").max(200, "Assunto deve ter menos de 200 caracteres"),
  message: z.string().trim().min(1, "Mensagem é obrigatória").max(1000, "Mensagem deve ter menos de 1000 caracteres"),
});

type ContactForm = z.infer<typeof contactSchema>;

const contactInfo = [
  {
    icon: Phone,
    title: "Telefone",
    value: "(11) 9999-9999",
    description: "Segunda a Sexta, 8h às 18h",
  },
  {
    icon: Mail,
    title: "Email",
    value: "contato@omnicare.com.br",
    description: "Respondemos em até 24h",
  },
  {
    icon: MapPin,
    title: "Endereço",
    value: "Av. Paulista, 1000",
    description: "São Paulo - SP, 01310-100",
  },
  {
    icon: Clock,
    title: "Horário",
    value: "Seg - Sex: 8h às 18h",
    description: "Sábado: 9h às 13h",
  },
];

const Contato = () => {
  const { toast } = useToast();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [errors, setErrors] = useState<Partial<Record<keyof ContactForm, string>>>({});
  const [formData, setFormData] = useState<ContactForm>({
    name: "",
    email: "",
    phone: "",
    subject: "",
    message: "",
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (errors[name as keyof ContactForm]) {
      setErrors((prev) => ({ ...prev, [name]: undefined }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrors({});

    const result = contactSchema.safeParse(formData);
    if (!result.success) {
      const fieldErrors: Partial<Record<keyof ContactForm, string>> = {};
      result.error.issues.forEach((issue) => {
        const field = issue.path[0] as keyof ContactForm;
        fieldErrors[field] = issue.message;
      });
      setErrors(fieldErrors);
      return;
    }

    setIsSubmitting(true);

    // Simulate API call
    await new Promise((resolve) => setTimeout(resolve, 1500));

    setIsSubmitting(false);
    setIsSuccess(true);
    toast({
      title: "Mensagem enviada!",
      description: "Entraremos em contato em breve.",
    });
  };

  if (isSuccess) {
    return (
      <Layout>
        <section className="section-padding">
          <div className="container-custom">
            <div className="max-w-lg mx-auto text-center">
              <div className="w-20 h-20 rounded-full bg-success/10 flex items-center justify-center mx-auto mb-6">
                <CheckCircle2 className="w-10 h-10 text-success" />
              </div>
              <h1 className="font-display text-3xl font-bold text-foreground mb-4">
                Mensagem Enviada!
              </h1>
              <p className="text-muted-foreground mb-8">
                Obrigado por entrar em contato. Nossa equipe responderá em até 24 horas.
              </p>
              <Button
                onClick={() => {
                  setIsSuccess(false);
                  setFormData({ name: "", email: "", phone: "", subject: "", message: "" });
                }}
                variant="outline"
              >
                Enviar nova mensagem
              </Button>
            </div>
          </div>
        </section>
      </Layout>
    );
  }

  return (
    <Layout>
      <section className="section-padding">
        <div className="container-custom">
          {/* Header */}
          <div className="text-center max-w-3xl mx-auto mb-12">
            <span className="inline-block px-4 py-1.5 rounded-full bg-primary/10 text-primary text-sm font-medium mb-4">
              Contato
            </span>
            <h1 className="font-display text-4xl sm:text-5xl font-bold text-foreground mb-4">
              Fale com nossa{" "}
              <span className="text-gradient">equipe</span>
            </h1>
            <p className="text-lg text-muted-foreground">
              Estamos prontos para ajudar você a transformar sua clínica. Entre em contato!
            </p>
          </div>

          <div className="grid lg:grid-cols-3 gap-8">
            {/* Contact Info */}
            <div className="lg:col-span-1">
              <div className="grid sm:grid-cols-2 lg:grid-cols-1 gap-4">
                {contactInfo.map((info) => (
                  <div key={info.title} className="card-elevated p-5 animate-fade-in">
                    <div className="flex items-start gap-4">
                      <div className="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center shrink-0">
                        <info.icon className="w-6 h-6 text-primary" />
                      </div>
                      <div>
                        <h3 className="font-semibold text-foreground mb-1">{info.title}</h3>
                        <p className="text-foreground/80">{info.value}</p>
                        <p className="text-sm text-muted-foreground">{info.description}</p>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            {/* Contact Form */}
            <div className="lg:col-span-2">
              <form onSubmit={handleSubmit} className="card-elevated p-6 lg:p-8 animate-fade-in-up">
                <h2 className="font-display font-semibold text-xl text-foreground mb-6">
                  Envie sua mensagem
                </h2>

                <div className="grid sm:grid-cols-2 gap-4 mb-4">
                  <div>
                    <label htmlFor="name" className="block text-sm font-medium text-foreground mb-2">
                      Nome completo *
                    </label>
                    <Input
                      id="name"
                      name="name"
                      value={formData.name}
                      onChange={handleChange}
                      placeholder="Seu nome"
                      className={errors.name ? "border-destructive" : ""}
                    />
                    {errors.name && <p className="text-destructive text-sm mt-1">{errors.name}</p>}
                  </div>
                  <div>
                    <label htmlFor="email" className="block text-sm font-medium text-foreground mb-2">
                      Email *
                    </label>
                    <Input
                      id="email"
                      name="email"
                      type="email"
                      value={formData.email}
                      onChange={handleChange}
                      placeholder="seu@email.com"
                      className={errors.email ? "border-destructive" : ""}
                    />
                    {errors.email && <p className="text-destructive text-sm mt-1">{errors.email}</p>}
                  </div>
                </div>

                <div className="grid sm:grid-cols-2 gap-4 mb-4">
                  <div>
                    <label htmlFor="phone" className="block text-sm font-medium text-foreground mb-2">
                      Telefone
                    </label>
                    <Input
                      id="phone"
                      name="phone"
                      value={formData.phone}
                      onChange={handleChange}
                      placeholder="(11) 99999-9999"
                    />
                  </div>
                  <div>
                    <label htmlFor="subject" className="block text-sm font-medium text-foreground mb-2">
                      Assunto *
                    </label>
                    <Input
                      id="subject"
                      name="subject"
                      value={formData.subject}
                      onChange={handleChange}
                      placeholder="Como podemos ajudar?"
                      className={errors.subject ? "border-destructive" : ""}
                    />
                    {errors.subject && <p className="text-destructive text-sm mt-1">{errors.subject}</p>}
                  </div>
                </div>

                <div className="mb-6">
                  <label htmlFor="message" className="block text-sm font-medium text-foreground mb-2">
                    Mensagem *
                  </label>
                  <Textarea
                    id="message"
                    name="message"
                    value={formData.message}
                    onChange={handleChange}
                    placeholder="Descreva como podemos ajudar sua clínica..."
                    rows={5}
                    className={errors.message ? "border-destructive" : ""}
                  />
                  {errors.message && <p className="text-destructive text-sm mt-1">{errors.message}</p>}
                </div>

                <Button
                  type="submit"
                  size="lg"
                  className="w-full sm:w-auto gap-2 hero-gradient text-primary-foreground hover:opacity-90"
                  disabled={isSubmitting}
                >
                  {isSubmitting ? (
                    "Enviando..."
                  ) : (
                    <>
                      <Send className="w-5 h-5" />
                      Enviar Mensagem
                    </>
                  )}
                </Button>
              </form>
            </div>
          </div>
        </div>
      </section>
    </Layout>
  );
};

export default Contato;
