import { useParams, Link } from "react-router-dom";
import { Layout } from "@/components/layout/Layout";
import { Calendar, Clock, ArrowLeft, Tag } from "lucide-react";

// Temporary mock data - will be replaced with API call
const posts = [
  {
    id: 1,
    title: "Como a telemedicina está transformando o atendimento pós-pandemia",
    content: `
      <h2>Introdução</h2>
      <p>A telemedicina revolucionou a forma como os profissionais de saúde atendem seus pacientes. Com a pandemia de COVID-19, essa modalidade de atendimento se tornou não apenas uma alternativa, mas uma necessidade.</p>
      
      <h2>Benefícios da Telemedicina</h2>
      <p>Entre os principais benefícios, destacam-se:</p>
      <ul>
        <li>Maior acessibilidade para pacientes em áreas remotas</li>
        <li>Redução de custos operacionais</li>
        <li>Flexibilidade de horários</li>
        <li>Menor tempo de espera</li>
      </ul>
      
      <h2>Desafios e Soluções</h2>
      <p>Apesar dos benefícios, a implementação da telemedicina apresenta desafios como a necessidade de infraestrutura tecnológica adequada e a adaptação de profissionais e pacientes às novas ferramentas.</p>
      
      <h2>O Futuro da Telemedicina</h2>
      <p>A tendência é que a telemedicina continue crescendo e se consolidando como uma modalidade essencial de atendimento médico, complementando o atendimento presencial.</p>
    `,
    excerpt: "Descubra como clínicas de todo o Brasil estão adotando a telemedicina para oferecer atendimento mais acessível e conveniente aos pacientes.",
    category: "Telemedicina",
    date: "2024-01-15",
    readTime: "5 min",
    image: "/placeholder.svg",
  },
  {
    id: 2,
    title: "5 dicas para reduzir faltas de pacientes na sua clínica",
    content: `
      <h2>Por que pacientes faltam às consultas?</h2>
      <p>Entender os motivos das faltas é o primeiro passo para reduzi-las. Os principais motivos incluem esquecimento, falta de transporte, problemas financeiros e falta de percepção da importância da consulta.</p>
      
      <h2>Dica 1: Implemente lembretes automáticos</h2>
      <p>Envie lembretes por SMS, WhatsApp ou e-mail 24 e 48 horas antes da consulta. Estudos mostram que essa prática pode reduzir faltas em até 40%.</p>
      
      <h2>Dica 2: Facilite o reagendamento</h2>
      <p>Permita que os pacientes reagendem suas consultas de forma fácil e rápida através de um portal online ou aplicativo.</p>
      
      <h2>Dica 3: Ofereça agendamento online</h2>
      <p>Quanto mais fácil for para o paciente marcar uma consulta, maior a chance de ele comparecer.</p>
      
      <h2>Dica 4: Mantenha uma lista de espera</h2>
      <p>Quando um paciente cancelar, você pode preencher rapidamente a vaga com outro paciente da lista de espera.</p>
      
      <h2>Dica 5: Eduque seus pacientes</h2>
      <p>Explique a importância do acompanhamento regular e as consequências de faltar às consultas.</p>
    `,
    excerpt: "Estratégias comprovadas para diminuir o número de no-shows e otimizar a agenda da sua clínica médica.",
    category: "Gestão",
    date: "2024-01-10",
    readTime: "4 min",
    image: "/placeholder.svg",
  },
  {
    id: 3,
    title: "LGPD na saúde: o que sua clínica precisa saber",
    content: `
      <h2>O que é a LGPD?</h2>
      <p>A Lei Geral de Proteção de Dados (LGPD) é a legislação brasileira que regulamenta o tratamento de dados pessoais, incluindo dados de saúde.</p>
      
      <h2>Dados sensíveis na área da saúde</h2>
      <p>Dados de saúde são considerados dados sensíveis pela LGPD, exigindo cuidados especiais no seu tratamento.</p>
      
      <h2>Obrigações das clínicas</h2>
      <p>As clínicas devem:</p>
      <ul>
        <li>Obter consentimento explícito dos pacientes</li>
        <li>Implementar medidas de segurança adequadas</li>
        <li>Nomear um encarregado de dados (DPO)</li>
        <li>Manter registro das operações de tratamento de dados</li>
      </ul>
      
      <h2>Consequências do descumprimento</h2>
      <p>O não cumprimento da LGPD pode resultar em multas de até 2% do faturamento da empresa, limitadas a R$ 50 milhões por infração.</p>
    `,
    excerpt: "Guia completo sobre como adequar sua clínica às normas da Lei Geral de Proteção de Dados e proteger informações dos pacientes.",
    category: "Compliance",
    date: "2024-01-05",
    readTime: "8 min",
    image: "/placeholder.svg",
  },
  {
    id: 4,
    title: "Prontuário eletrônico: benefícios além da organização",
    content: `
      <h2>O que é um prontuário eletrônico?</h2>
      <p>O prontuário eletrônico é a versão digital do prontuário médico tradicional, permitindo o registro, armazenamento e acesso digital às informações dos pacientes.</p>
      
      <h2>Vantagens do prontuário eletrônico</h2>
      <ul>
        <li>Acesso rápido e fácil às informações</li>
        <li>Redução de erros médicos</li>
        <li>Economia de espaço físico</li>
        <li>Facilita a continuidade do cuidado</li>
        <li>Melhora a comunicação entre profissionais</li>
      </ul>
      
      <h2>Segurança e conformidade</h2>
      <p>Os sistemas modernos de prontuário eletrônico oferecem recursos avançados de segurança, incluindo criptografia, controle de acesso e auditoria.</p>
    `,
    excerpt: "Conheça as vantagens de adotar um prontuário eletrônico e como ele pode melhorar a qualidade do atendimento.",
    category: "Tecnologia",
    date: "2024-01-01",
    readTime: "6 min",
    image: "/placeholder.svg",
  },
  {
    id: 5,
    title: "Marketing digital para clínicas: por onde começar",
    content: `
      <h2>A importância do marketing digital na saúde</h2>
      <p>O marketing digital permite que clínicas alcancem mais pacientes, construam autoridade e fortaleçam sua marca.</p>
      
      <h2>Estratégias fundamentais</h2>
      <ul>
        <li>Criar um site profissional e otimizado</li>
        <li>Investir em SEO local</li>
        <li>Manter presença nas redes sociais</li>
        <li>Produzir conteúdo educativo</li>
        <li>Utilizar Google Meu Negócio</li>
      </ul>
      
      <h2>Ética e compliance</h2>
      <p>É fundamental seguir as diretrizes éticas do CFM e respeitar as normas de publicidade médica.</p>
    `,
    excerpt: "Estratégias de marketing digital específicas para profissionais da saúde atraírem mais pacientes.",
    category: "Marketing",
    date: "2023-12-28",
    readTime: "7 min",
    image: "/placeholder.svg",
  },
  {
    id: 6,
    title: "Inteligência artificial na medicina: tendências para 2024",
    content: `
      <h2>IA transformando a medicina</h2>
      <p>A inteligência artificial está revolucionando diversos aspectos da medicina, desde diagnósticos até a gestão de clínicas.</p>
      
      <h2>Aplicações práticas</h2>
      <ul>
        <li>Diagnóstico assistido por IA</li>
        <li>Análise de imagens médicas</li>
        <li>Previsão de surtos de doenças</li>
        <li>Personalização de tratamentos</li>
        <li>Otimização de processos administrativos</li>
      </ul>
      
      <h2>Desafios éticos</h2>
      <p>A implementação de IA na medicina levanta questões éticas importantes sobre privacidade, responsabilidade e tomada de decisões.</p>
      
      <h2>O futuro próximo</h2>
      <p>Espera-se que a IA se torne cada vez mais integrada ao dia a dia dos profissionais de saúde, auxiliando em decisões clínicas e melhorando resultados.</p>
    `,
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

const BlogPost = () => {
  const { id } = useParams<{ id: string }>();
  const post = posts.find((p) => p.id === parseInt(id || "0"));

  if (!post) {
    return (
      <Layout>
        <section className="section-padding">
          <div className="container-custom text-center">
            <h1 className="font-display text-4xl font-bold mb-4">Post não encontrado</h1>
            <Link to="/blog" className="text-primary hover:underline flex items-center justify-center gap-2">
              <ArrowLeft className="w-4 h-4" />
              Voltar para o blog
            </Link>
          </div>
        </section>
      </Layout>
    );
  }

  return (
    <Layout>
      <article className="section-padding">
        <div className="container-custom max-w-4xl">
          {/* Back button */}
          <Link
            to="/blog"
            className="inline-flex items-center gap-2 text-muted-foreground hover:text-primary transition-colors mb-8"
          >
            <ArrowLeft className="w-4 h-4" />
            Voltar para o blog
          </Link>

          {/* Header */}
          <header className="mb-8">
            <div className="flex items-center gap-3 mb-4">
              <span className="px-3 py-1 rounded-full bg-primary/10 text-primary text-sm font-medium">
                {post.category}
              </span>
              <span className="text-muted-foreground text-sm flex items-center gap-1">
                <Clock className="w-4 h-4" />
                {post.readTime}
              </span>
            </div>
            <h1 className="font-display text-4xl sm:text-5xl font-bold text-foreground mb-4">
              {post.title}
            </h1>
            <div className="flex items-center gap-4 text-muted-foreground">
              <span className="flex items-center gap-2">
                <Calendar className="w-4 h-4" />
                {formatDate(post.date)}
              </span>
            </div>
          </header>

          {/* Featured Image */}
          <div className="aspect-video bg-gradient-to-br from-primary/20 to-accent/10 rounded-2xl flex items-center justify-center mb-12">
            <div className="w-24 h-24 rounded-2xl hero-gradient flex items-center justify-center">
              <span className="text-4xl text-primary-foreground font-display font-bold">OC</span>
            </div>
          </div>

          {/* Content */}
          <div 
            className="prose prose-lg max-w-none"
            dangerouslySetInnerHTML={{ __html: post.content }}
          />

          {/* Related posts section could go here */}
          <div className="mt-12 pt-8 border-t border-border">
            <Link
              to="/blog"
              className="inline-flex items-center gap-2 text-primary hover:gap-3 transition-all font-medium"
            >
              <ArrowLeft className="w-4 h-4" />
              Ver todos os posts
            </Link>
          </div>
        </div>
      </article>
    </Layout>
  );
};

export default BlogPost;
