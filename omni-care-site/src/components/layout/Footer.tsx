import { Link } from "react-router-dom";
import { Facebook, Instagram, Linkedin, Twitter, Mail, Phone, MapPin } from "lucide-react";

const quickLinks = [
  { name: "Planos", path: "/planos" },
  { name: "Clínicas", path: "/clinicas" },
  { name: "Blog", path: "/blog" },
  { name: "Contato", path: "/contato" },
];

const services = [
  { name: "Gerenciamento de Clínicas", path: "/planos" },
  { name: "Agendamento Online", path: "/planos" },
  { name: "Telemedicina", path: "/planos" },
  { name: "Integração de Sistemas", path: "/planos" },
];

const socialLinks = [
  { icon: Facebook, href: "#", label: "Facebook" },
  { icon: Instagram, href: "#", label: "Instagram" },
  { icon: Linkedin, href: "#", label: "LinkedIn" },
  { icon: Twitter, href: "#", label: "Twitter" },
];

export const Footer = () => {
  return (
    <footer className="bg-foreground text-background/90">
      <div className="container-custom section-padding">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8 lg:gap-12">
          {/* Brand Column */}
          <div className="lg:col-span-1">
            <Link to="/" className="flex items-center gap-2 mb-4">
              <div className="w-10 h-10 rounded-xl bg-primary flex items-center justify-center">
                <span className="text-primary-foreground font-bold text-lg">O</span>
              </div>
              <span className="font-display font-bold text-xl text-background">
                Omni <span className="text-primary">Care</span>
              </span>
            </Link>
            <p className="text-background/70 text-sm leading-relaxed mb-6">
              Transformando a gestão de clínicas com tecnologia moderna, 
              telemedicina integrada e soluções completas para profissionais da saúde.
            </p>
            <div className="flex gap-3">
              {socialLinks.map((social) => (
                <a
                  key={social.label}
                  href={social.href}
                  aria-label={social.label}
                  className="w-10 h-10 rounded-lg bg-background/10 hover:bg-primary hover:text-primary-foreground flex items-center justify-center transition-colors"
                >
                  <social.icon className="w-5 h-5" />
                </a>
              ))}
            </div>
          </div>

          {/* Quick Links */}
          <div>
            <h4 className="font-display font-semibold text-background mb-4">Links Rápidos</h4>
            <ul className="space-y-3">
              {quickLinks.map((link) => (
                <li key={link.path}>
                  <Link
                    to={link.path}
                    className="text-background/70 hover:text-primary transition-colors text-sm"
                  >
                    {link.name}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Services */}
          <div>
            <h4 className="font-display font-semibold text-background mb-4">Serviços</h4>
            <ul className="space-y-3">
              {services.map((service) => (
                <li key={service.name}>
                  <Link
                    to={service.path}
                    className="text-background/70 hover:text-primary transition-colors text-sm"
                  >
                    {service.name}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Contact */}
          <div>
            <h4 className="font-display font-semibold text-background mb-4">Contato</h4>
            <ul className="space-y-4">
              <li className="flex items-start gap-3">
                <MapPin className="w-5 h-5 text-primary shrink-0 mt-0.5" />
                <span className="text-background/70 text-sm">
                  Av. Paulista, 1000 - Bela Vista<br />
                  São Paulo - SP, 01310-100
                </span>
              </li>
              <li className="flex items-center gap-3">
                <Phone className="w-5 h-5 text-primary shrink-0" />
                <a href="tel:+551199999999" className="text-background/70 hover:text-primary transition-colors text-sm">
                  (11) 9999-9999
                </a>
              </li>
              <li className="flex items-center gap-3">
                <Mail className="w-5 h-5 text-primary shrink-0" />
                <a href="mailto:contato@omnicare.com.br" className="text-background/70 hover:text-primary transition-colors text-sm">
                  contato@omnicare.com.br
                </a>
              </li>
            </ul>
          </div>
        </div>

        {/* Bottom Bar */}
        <div className="border-t border-background/10 mt-12 pt-8">
          <div className="flex flex-col md:flex-row justify-between items-center gap-4">
            <p className="text-background/50 text-sm text-center md:text-left">
              © {new Date().getFullYear()} Omni Care. Todos os direitos reservados.
            </p>
            <p className="text-background/50 text-sm">
              CNPJ: 00.000.000/0001-00
            </p>
            <div className="flex gap-6">
              <Link to="/privacidade" className="text-background/50 hover:text-primary transition-colors text-sm">
                Política de Privacidade
              </Link>
              <Link to="/termos" className="text-background/50 hover:text-primary transition-colors text-sm">
                Termos de Uso
              </Link>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
};
