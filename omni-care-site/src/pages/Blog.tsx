import { Calendar, Clock, ArrowRight, Tag } from "lucide-react";
import { Link } from "react-router-dom";
import { Layout } from "@/components/layout/Layout";

const posts = [
  {
    id: 1,
    title: "Como a telemedicina está transformando o atendimento pós-pandemia",
    excerpt: "Descubra como clínicas de todo o Brasil estão adotando a telemedicina para oferecer atendimento mais acessível e conveniente aos pacientes.",
    category: "Telemedicina",
    date: "2024-01-15",
    readTime: "5 min",
    image: "/placeholder.svg",
  },
  {
    id: 2,
    title: "5 dicas para reduzir faltas de pacientes na sua clínica",
    excerpt: "Estratégias comprovadas para diminuir o número de no-shows e otimizar a agenda da sua clínica médica.",
    category: "Gestão",
    date: "2024-01-10",
    readTime: "4 min",
    image: "/placeholder.svg",
  },
  {
    id: 3,
    title: "LGPD na saúde: o que sua clínica precisa saber",
    excerpt: "Guia completo sobre como adequar sua clínica às normas da Lei Geral de Proteção de Dados e proteger informações dos pacientes.",
    category: "Compliance",
    date: "2024-01-05",
    readTime: "8 min",
    image: "/placeholder.svg",
  },
  {
    id: 4,
    title: "Prontuário eletrônico: benefícios além da organização",
    excerpt: "Conheça as vantagens de adotar um prontuário eletrônico e como ele pode melhorar a qualidade do atendimento.",
    category: "Tecnologia",
    date: "2024-01-01",
    readTime: "6 min",
    image: "/placeholder.svg",
  },
  {
    id: 5,
    title: "Marketing digital para clínicas: por onde começar",
    excerpt: "Estratégias de marketing digital específicas para profissionais da saúde atraírem mais pacientes.",
    category: "Marketing",
    date: "2023-12-28",
    readTime: "7 min",
    image: "/placeholder.svg",
  },
  {
    id: 6,
    title: "Inteligência artificial na medicina: tendências para 2024",
    excerpt: "Como a IA está revolucionando diagnósticos, tratamentos e a gestão de clínicas médicas.",
    category: "Inovação",
    date: "2023-12-20",
    readTime: "5 min",
    image: "/placeholder.svg",
  },
];

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString("pt-BR", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });
};

const Blog = () => {
  return (
    <Layout>
      <section className="section-padding">
        <div className="container-custom">
          {/* Header */}
          <div className="text-center max-w-3xl mx-auto mb-12">
            <span className="inline-block px-4 py-1.5 rounded-full bg-primary/10 text-primary text-sm font-medium mb-4">
              Blog
            </span>
            <h1 className="font-display text-4xl sm:text-5xl font-bold text-foreground mb-4">
              Insights para{" "}
              <span className="text-gradient">profissionais da saúde</span>
            </h1>
            <p className="text-lg text-muted-foreground">
              Dicas, tendências e novidades sobre gestão de clínicas, tecnologia em saúde e muito mais.
            </p>
          </div>

          {/* Featured Post */}
          <div className="card-elevated overflow-hidden mb-12 animate-fade-in-up">
            <div className="grid lg:grid-cols-2">
              <div className="aspect-video lg:aspect-auto bg-gradient-to-br from-primary/20 to-accent/10 flex items-center justify-center">
                <div className="w-24 h-24 rounded-2xl hero-gradient flex items-center justify-center">
                  <span className="text-4xl text-primary-foreground font-display font-bold">OC</span>
                </div>
              </div>
              <div className="p-6 lg:p-10 flex flex-col justify-center">
                <div className="flex items-center gap-3 mb-4">
                  <span className="px-3 py-1 rounded-full bg-primary/10 text-primary text-sm font-medium">
                    {posts[0].category}
                  </span>
                  <span className="text-muted-foreground text-sm flex items-center gap-1">
                    <Clock className="w-4 h-4" />
                    {posts[0].readTime}
                  </span>
                </div>
                <h2 className="font-display text-2xl lg:text-3xl font-bold text-foreground mb-4">
                  {posts[0].title}
                </h2>
                <p className="text-muted-foreground mb-6">
                  {posts[0].excerpt}
                </p>
                <div className="flex items-center justify-between">
                  <span className="text-sm text-muted-foreground flex items-center gap-2">
                    <Calendar className="w-4 h-4" />
                    {formatDate(posts[0].date)}
                  </span>
                  <Link
                    to={`/blog/${posts[0].id}`}
                    className="text-primary font-medium flex items-center gap-1 hover:gap-2 transition-all"
                  >
                    Leia mais
                    <ArrowRight className="w-4 h-4" />
                  </Link>
                </div>
              </div>
            </div>
          </div>

          {/* Posts Grid */}
          <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-6">
            {posts.slice(1).map((post, index) => (
              <article
                key={post.id}
                className="card-elevated overflow-hidden group animate-fade-in-up"
                style={{ animationDelay: `${index * 0.05}s` }}
              >
                {/* Image */}
                <div className="aspect-video bg-gradient-to-br from-primary/10 to-accent/5 flex items-center justify-center">
                  <div className="w-12 h-12 rounded-xl bg-primary/20 flex items-center justify-center">
                    <Tag className="w-6 h-6 text-primary" />
                  </div>
                </div>

                {/* Content */}
                <div className="p-5">
                  <div className="flex items-center gap-3 mb-3">
                    <span className="px-2 py-1 rounded-md bg-primary/10 text-primary text-xs font-medium">
                      {post.category}
                    </span>
                    <span className="text-muted-foreground text-xs flex items-center gap-1">
                      <Clock className="w-3 h-3" />
                      {post.readTime}
                    </span>
                  </div>

                  <h3 className="font-display font-semibold text-foreground mb-2 line-clamp-2 group-hover:text-primary transition-colors">
                    {post.title}
                  </h3>

                  <p className="text-muted-foreground text-sm mb-4 line-clamp-2">
                    {post.excerpt}
                  </p>

                  <div className="flex items-center justify-between">
                    <span className="text-xs text-muted-foreground">
                      {formatDate(post.date)}
                    </span>
                    <Link
                      to={`/blog/${post.id}`}
                      className="text-primary text-sm font-medium flex items-center gap-1 hover:gap-2 transition-all"
                    >
                      Leia mais
                      <ArrowRight className="w-4 h-4" />
                    </Link>
                  </div>
                </div>
              </article>
            ))}
          </div>
        </div>
      </section>
    </Layout>
  );
};

export default Blog;
