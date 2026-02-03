import { useState } from "react";
import { Search, MapPin, Stethoscope, Star, Calendar } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Layout } from "@/components/layout/Layout";

const specialties = [
  "Todas Especialidades",
  "Cardiologia",
  "Dermatologia",
  "Ginecologia",
  "Ortopedia",
  "Pediatria",
  "Psiquiatria",
];

const cities = [
  "Todas Cidades",
  "São Paulo",
  "Rio de Janeiro",
  "Belo Horizonte",
  "Curitiba",
  "Porto Alegre",
  "Brasília",
];

const clinics = [
  {
    name: "Centro Médico Vida",
    specialty: "Cardiologia",
    city: "São Paulo",
    address: "Av. Paulista, 1000 - Bela Vista",
    rating: 4.9,
    reviews: 234,
    image: "/placeholder.svg",
    professionals: 12,
  },
  {
    name: "Clínica Coração Saudável",
    specialty: "Cardiologia",
    city: "São Paulo",
    address: "Rua Augusta, 500 - Consolação",
    rating: 4.8,
    reviews: 189,
    image: "/placeholder.svg",
    professionals: 8,
  },
  {
    name: "Dermato Estética",
    specialty: "Dermatologia",
    city: "Rio de Janeiro",
    address: "Av. Atlântica, 200 - Copacabana",
    rating: 4.9,
    reviews: 312,
    image: "/placeholder.svg",
    professionals: 6,
  },
  {
    name: "OrtoClínica",
    specialty: "Ortopedia",
    city: "Belo Horizonte",
    address: "Av. Afonso Pena, 1500 - Centro",
    rating: 4.7,
    reviews: 156,
    image: "/placeholder.svg",
    professionals: 10,
  },
  {
    name: "Pediatria Total",
    specialty: "Pediatria",
    city: "Curitiba",
    address: "Rua XV de Novembro, 800 - Centro",
    rating: 4.9,
    reviews: 278,
    image: "/placeholder.svg",
    professionals: 15,
  },
  {
    name: "Mente Saudável",
    specialty: "Psiquiatria",
    city: "Porto Alegre",
    address: "Av. Borges de Medeiros, 300 - Centro",
    rating: 4.8,
    reviews: 198,
    image: "/placeholder.svg",
    professionals: 5,
  },
];

const Clinicas = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCity, setSelectedCity] = useState("Todas Cidades");
  const [selectedSpecialty, setSelectedSpecialty] = useState("Todas Especialidades");

  const filteredClinics = clinics.filter((clinic) => {
    const matchesSearch = clinic.name.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesCity = selectedCity === "Todas Cidades" || clinic.city === selectedCity;
    const matchesSpecialty = selectedSpecialty === "Todas Especialidades" || clinic.specialty === selectedSpecialty;
    return matchesSearch && matchesCity && matchesSpecialty;
  });

  return (
    <Layout>
      <section className="section-padding">
        <div className="container-custom">
          {/* Header */}
          <div className="text-center max-w-3xl mx-auto mb-12">
            <span className="inline-block px-4 py-1.5 rounded-full bg-primary/10 text-primary text-sm font-medium mb-4">
              Rede de Clínicas
            </span>
            <h1 className="font-display text-4xl sm:text-5xl font-bold text-foreground mb-4">
              Encontre a clínica{" "}
              <span className="text-gradient">perfeita para você</span>
            </h1>
            <p className="text-lg text-muted-foreground">
              Mais de 500 clínicas e profissionais credenciados em todo o Brasil.
            </p>
          </div>

          {/* Filters */}
          <div className="card-elevated p-4 md:p-6 mb-8">
            <div className="flex flex-col md:flex-row gap-4">
              <div className="flex-1 relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                <Input
                  placeholder="Buscar clínica por nome..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
              <Select value={selectedCity} onValueChange={setSelectedCity}>
                <SelectTrigger className="w-full md:w-48 bg-background">
                  <MapPin className="w-4 h-4 mr-2 text-muted-foreground" />
                  <SelectValue />
                </SelectTrigger>
                <SelectContent className="bg-popover border border-border shadow-lg z-50">
                  {cities.map((city) => (
                    <SelectItem key={city} value={city}>
                      {city}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <Select value={selectedSpecialty} onValueChange={setSelectedSpecialty}>
                <SelectTrigger className="w-full md:w-48 bg-background">
                  <Stethoscope className="w-4 h-4 mr-2 text-muted-foreground" />
                  <SelectValue />
                </SelectTrigger>
                <SelectContent className="bg-popover border border-border shadow-lg z-50">
                  {specialties.map((specialty) => (
                    <SelectItem key={specialty} value={specialty}>
                      {specialty}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Results Count */}
          <p className="text-muted-foreground mb-6">
            {filteredClinics.length} clínicas encontradas
          </p>

          {/* Clinics Grid */}
          <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-6">
            {filteredClinics.map((clinic, index) => (
              <div
                key={clinic.name}
                className="card-elevated overflow-hidden group animate-fade-in-up"
                style={{ animationDelay: `${index * 0.05}s` }}
              >
                {/* Image */}
                <div className="aspect-video bg-gradient-to-br from-primary/20 to-accent/10 relative overflow-hidden">
                  <div className="absolute inset-0 flex items-center justify-center">
                    <div className="w-16 h-16 rounded-xl hero-gradient flex items-center justify-center">
                      <Stethoscope className="w-8 h-8 text-primary-foreground" />
                    </div>
                  </div>
                  <div className="absolute top-3 right-3 px-2 py-1 rounded-lg bg-background/90 backdrop-blur-sm text-xs font-medium flex items-center gap-1">
                    <Star className="w-3 h-3 fill-amber-400 text-amber-400" />
                    {clinic.rating} ({clinic.reviews})
                  </div>
                </div>

                {/* Content */}
                <div className="p-5">
                  <div className="flex items-start justify-between gap-2 mb-2">
                    <h3 className="font-display font-semibold text-foreground group-hover:text-primary transition-colors">
                      {clinic.name}
                    </h3>
                    <span className="px-2 py-1 rounded-md bg-primary/10 text-primary text-xs font-medium shrink-0">
                      {clinic.specialty}
                    </span>
                  </div>

                  <div className="flex items-center gap-2 text-sm text-muted-foreground mb-4">
                    <MapPin className="w-4 h-4 shrink-0" />
                    <span className="truncate">{clinic.address}</span>
                  </div>

                  <div className="flex items-center justify-between">
                    <span className="text-sm text-muted-foreground">
                      {clinic.professionals} profissionais
                    </span>
                    <Button size="sm" className="gap-1 hero-gradient text-primary-foreground hover:opacity-90">
                      <Calendar className="w-4 h-4" />
                      Agendar
                    </Button>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {filteredClinics.length === 0 && (
            <div className="text-center py-16">
              <p className="text-muted-foreground">
                Nenhuma clínica encontrada com os filtros selecionados.
              </p>
            </div>
          )}
        </div>
      </section>
    </Layout>
  );
};

export default Clinicas;
