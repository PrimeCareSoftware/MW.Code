import { useParams, Link } from "react-router-dom";
import { Layout } from "@/components/layout/Layout";
import { Calendar, Clock, ArrowLeft } from "lucide-react";
import { useState, useEffect } from "react";

interface Post {
  id: string;
  title: string;
  content: string;
  excerpt: string;
  category: string;
  publishedAt?: string;
  readTimeMinutes: number;
  authorName: string;
  featuredImage?: string;
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return 'N/A';
  return new Date(dateString).toLocaleDateString("pt-BR", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });
};

const BlogPost = () => {
  const { id } = useParams<{ id: string }>();
  const [post, setPost] = useState<Post | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;

    fetch(`/api/blog-posts/${id}`)
      .then(response => {
        if (!response.ok) {
          if (response.status === 404) {
            throw new Error('Post não encontrado');
          }
          throw new Error('Erro ao carregar post');
        }
        return response.json();
      })
      .then(data => {
        setPost(data);
        setLoading(false);
      })
      .catch(err => {
        console.error('Error fetching post:', err);
        setError(err.message || 'Erro ao carregar post');
        setLoading(false);
      });
  }, [id]);

  if (loading) {
    return (
      <Layout>
        <section className="section-padding">
          <div className="container-custom text-center">
            <p>Carregando post...</p>
          </div>
        </section>
      </Layout>
    );
  }

  if (error || !post) {
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
                {post.readTimeMinutes} min
              </span>
            </div>
            <h1 className="font-display text-4xl sm:text-5xl font-bold text-foreground mb-4">
              {post.title}
            </h1>
            <div className="flex items-center gap-4 text-muted-foreground">
              <span className="flex items-center gap-2">
                <Calendar className="w-4 h-4" />
                {formatDate(post.publishedAt)}
              </span>
              <span>Por {post.authorName}</span>
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
