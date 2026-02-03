import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

interface BlogPost {
  id: number;
  title: string;
  excerpt: string;
  category: string;
  date: string;
  readTime: string;
  image?: string;
}

@Component({
  selector: 'app-blog',
  standalone: true,
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './blog.component.html',
  styleUrl: './blog.component.scss'
})
export class BlogComponent implements OnInit {
  posts: BlogPost[] = [
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

  featuredPost: BlogPost | null = null;
  remainingPosts: BlogPost[] = [];
  
  ngOnInit(): void {
    this.featuredPost = this.posts[0];
    this.remainingPosts = this.posts.slice(1);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString("pt-BR", {
      day: "numeric",
      month: "long",
      year: "numeric",
    });
  }
}
