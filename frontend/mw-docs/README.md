# 📚 MedicWarehouse Docs - Central de Documentação

Aplicação Angular standalone criada para centralizar e facilitar a consulta de toda a documentação do projeto MedicWarehouse.

## 🎯 Objetivo

Este projeto foi criado para resolver o problema de ter vários arquivos de documentação soltos no repositório. Agora toda a documentação está organizada e acessível através de uma interface web moderna e intuitiva.

## ✨ Características

- **📱 Interface Moderna**: Design responsivo e amigável
- **🔍 Busca Inteligente**: Encontre documentos rapidamente
- **📊 Categorização**: Documentos organizados por categorias
- **📝 Renderização Markdown**: Suporte completo a Markdown com syntax highlighting
- **📐 Diagramas Mermaid**: Visualização de fluxos e diagramas
- **🎨 Design System Consistente**: Visual padronizado e profissional
- **⚡ Performance**: Carregamento rápido e otimizado

## 📦 Documentos Incluídos

Este projeto integra **29 documentos** organizados em **12 categorias**:

### Categorias

1. **📱 Interface e Experiência do Usuário**
   - SCREENS_DOCUMENTATION.md
   - VISUAL_FLOW_SUMMARY.md

2. **📋 Regras de Negócio e Requisitos**
   - BUSINESS_RULES.md

3. **🔧 Implementação Técnica**
   - TECHNICAL_IMPLEMENTATION.md
   - IMPLEMENTATION.md

4. **🚀 Guias de Uso**
   - README.md
   - GUIA_EXECUCAO.md
   - API_QUICK_GUIDE.md

5. **🔄 CI/CD e Qualidade**
   - CI_CD_DOCUMENTATION.md
   - TEST_SUMMARY.md
   - SECURITY_VALIDATIONS.md
   - SONARCLOUD_SETUP.md

6. **📝 Resumos de Implementação**
   - IMPLEMENTATION_SUMMARY.md
   - IMPLEMENTATION_NEW_FEATURES.md
   - IMPLEMENTATION_SUMMARY_BUSINESS_RULES.md
   - MIGRATION_IMPLEMENTATION_SUMMARY.md

7. **🔐 Segurança**
   - SECURITY_GUIDE.md
   - SECURITY_IMPLEMENTATION_SUMMARY.md

8. **💰 Sistema de Pagamentos**
   - IMPLEMENTATION_PAYMENT_SYSTEM.md
   - PAYMENT_FLOW.md

9. **🔔 Notificações**
   - NOTIFICATION_ROUTINES_DOCUMENTATION.md
   - IMPLEMENTATION_NOTIFICATION_ROUTINES.md
   - NOTIFICATION_ROUTINES_EXAMPLE.md

10. **👨‍👩‍👧 Recursos Especiais**
    - IMPLEMENTATION_GUARDIAN_CHILD.md

11. **🌐 MW.Site - Marketing**
    - MW_SITE_DOCUMENTATION.md
    - MW_SITE_IMPLEMENTATION_SUMMARY.md

12. **📚 Índice e Referências**
    - INDEX.md

## 🚀 Como Executar

### Pré-requisitos

- Node.js 18+ 
- npm 9+

### Instalação e Execução

```bash
# Navegar para o diretório do projeto
cd frontend/mw-docs

# Instalar dependências
npm install

# Executar em modo desenvolvimento
npm start
# ou
ng serve

# A aplicação estará disponível em:
# http://localhost:4200
```

### Build para Produção

```bash
# Gerar build de produção
npm run build
# ou
ng build --configuration production

# Os arquivos estarão em: dist/mw-docs/browser/
```

## 🏗️ Arquitetura

### Tecnologias Utilizadas

- **Angular 20.3.5**: Framework principal
- **TypeScript**: Linguagem de programação
- **SCSS**: Pré-processador CSS
- **ngx-markdown**: Renderização de Markdown
- **Mermaid**: Renderização de diagramas
- **Marked**: Parser de Markdown

### Estrutura do Projeto

```
mw-docs/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   ├── home/                    # Página inicial com listagem
│   │   │   │   ├── home.component.ts
│   │   │   │   ├── home.component.html
│   │   │   │   └── home.component.scss
│   │   │   └── doc-viewer/              # Visualizador de documentos
│   │   │       ├── doc-viewer.component.ts
│   │   │       ├── doc-viewer.component.html
│   │   │       └── doc-viewer.component.scss
│   │   ├── models/
│   │   │   └── doc-item.model.ts        # Interfaces TypeScript
│   │   ├── services/
│   │   │   └── documentation.service.ts # Serviço de documentação
│   │   ├── app.ts                       # Componente raiz
│   │   ├── app.config.ts                # Configuração da aplicação
│   │   └── app.routes.ts                # Rotas da aplicação
│   ├── assets/
│   │   └── docs/                        # Documentos Markdown
│   │       ├── *.md                     # Docs da raiz
│   │       └── docs/                    # Docs da pasta /docs
│   ├── index.html
│   └── styles.scss                      # Estilos globais
├── angular.json
├── package.json
└── README.md
```

## 🎨 Funcionalidades

### Página Inicial

- Lista todas as categorias de documentação
- Mostra estatísticas (número de documentos e categorias)
- Busca em tempo real
- Cards informativos para cada documento

### Visualizador de Documentos

- Renderização completa de Markdown
- Suporte a código com syntax highlighting
- Renderização de diagramas Mermaid
- Navegação fácil (botão voltar)
- Design responsivo

### Busca

- Busca por título do documento
- Busca por descrição
- Busca por categoria
- Resultados filtrados instantaneamente

## 🌐 Deploy

### GitHub Pages (Automático)

A documentação é automaticamente publicada no GitHub Pages sempre que houver alterações na branch `main`.

**URL da Documentação**: https://medicwarehouse.github.io/MW.Code/

**📖 Guia completo de deploy**: Consulte [DEPLOY.md](DEPLOY.md) para instruções detalhadas sobre configuração, troubleshooting e como usar o workflow de deploy.

### Outros Servidores

Os arquivos buildados também podem ser servidos por qualquer servidor web estático:

- **Nginx**
- **Apache**
- **AWS S3 + CloudFront**
- **Firebase Hosting**
- **Vercel**
- **Netlify**

### Exemplo de configuração Nginx

```nginx
server {
    listen 80;
    server_name docs.medicwarehouse.com;
    root /var/www/mw-docs/browser;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

## 📝 Atualizando a Documentação

Para adicionar novos documentos:

1. **Copie o arquivo .md** para `src/assets/docs/`
2. **Atualize o serviço** `documentation.service.ts`:
   - Adicione o documento na categoria apropriada
   - Configure: id, title, category, path, description

```typescript
{
  id: 'novo-doc',
  title: 'NOVO_DOC.md',
  category: 'Categoria',
  path: 'NOVO_DOC.md',
  description: 'Descrição do documento',
  idealFor: 'Público alvo'
}
```

3. **Rebuild** a aplicação

## 🔧 Customização

### Cores e Tema

Edite os arquivos SCSS para alterar o tema:
- `src/styles.scss` - Estilos globais
- `src/app/components/home/home.component.scss` - Página inicial
- `src/app/components/doc-viewer/doc-viewer.component.scss` - Visualizador

### Adicionar Categorias

No arquivo `documentation.service.ts`, adicione novas categorias ao array `documentStructure`:

```typescript
{
  name: '🆕 Nova Categoria',
  icon: '🆕',
  docs: [
    // seus documentos aqui
  ]
}
```

## 📊 Informações Técnicas

- **Bundle Size**: ~1.5MB (desenvolvimento)
- **Build Size**: ~400KB (produção, comprimido)
- **Browsers Suportados**: Chrome, Firefox, Safari, Edge (versões recentes)
- **Mobile-First**: Design totalmente responsivo

## 🤝 Contribuindo

Para contribuir com melhorias:

1. Faça suas alterações
2. Teste localmente com `npm start`
3. Build de produção `npm run build`
4. Commit suas mudanças
5. Abra um Pull Request

## 📄 Licença

Este projeto faz parte do MedicWarehouse e segue a mesma licença do projeto principal.

## 📞 Suporte

Para dúvidas ou problemas, consulte a documentação principal ou entre em contato com a equipe de desenvolvimento.

---

**MedicWarehouse** © 2025 - Sistema de Gestão Médica
