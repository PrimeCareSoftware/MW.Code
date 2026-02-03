# 📚 Omni Care Software Docs - Central de Documentação

Aplicação Angular standalone criada para centralizar e facilitar a consulta de toda a documentação do projeto Omni Care Software, incluindo **todos os 323 documentos do system-admin**.

## 🎯 Objetivo

Este projeto foi criado para resolver o problema de ter vários arquivos de documentação soltos no repositório. Agora toda a documentação está organizada e acessível através de uma interface web moderna e intuitiva, **incluindo toda a documentação do system-admin em páginas HTML navegáveis**.

## ✨ Características

- **📱 Interface Moderna**: Design responsivo e amigável
- **🔍 Busca Inteligente**: Encontre documentos rapidamente entre 370+ documentos
- **📊 Categorização**: Documentos organizados por categorias (20+ categorias)
- **📝 Renderização Markdown**: Suporte completo a Markdown com syntax highlighting
- **📐 Diagramas Mermaid**: Visualização de fluxos e diagramas
- **🎨 Design System Consistente**: Visual padronizado e profissional
- **⚡ Performance**: Carregamento rápido e otimizado
- **🔄 Geração Automática**: System-admin docs gerados automaticamente

## 📦 Documentos Incluídos

Este projeto inclui:

- **323 documentos** do `/system-admin` organizados em 10 categorias (gerados automaticamente)
- **50+ documentos** customizados e tutoriais (configurados manualmente)
- **Total: 370+ documentos** acessíveis via interface web

**Categorias do System-Admin:**
- 🔧 Backend (7 docs)
- ⚕️ CFM Compliance (15 docs)
- 📚 Documentação Geral (70+ docs)
- 🎨 Frontend (12 docs)
- 📖 Guias (50+ docs)
- 🔧 Implementações (40+ docs)
- 🏗️ Infraestrutura (15+ docs)
- 📋 Regras de Negócio (20+ docs)
- 🔒 Segurança (6 docs)
- E mais...

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

### Adicionando Documentos no system-admin

Os documentos do `system-admin` são **automaticamente incluídos** na interface web através de um script de geração:

1. **Adicione o arquivo .md** na pasta `/system-admin` do repositório principal
2. **Execute o script de geração**:
   ```bash
   cd frontend/mw-docs
   npm run generate-docs
   ```
3. **Rebuild** a aplicação:
   ```bash
   npm run build
   ```

O script `generate-docs` escaneia todos os arquivos markdown em `/system-admin` e atualiza automaticamente `src/app/services/generated-docs.ts`.

### Adicionando Documentos Customizados

Para adicionar documentos com configuração manual (fora do system-admin):

1. **Adicione o arquivo .md** na pasta `/docs` do repositório principal
2. **Atualize o serviço** `documentation.service.ts`:
   - Adicione o documento na categoria apropriada em `originalDocs`
   - Configure: id, title, category, path, description

```typescript
{
  id: 'novo-doc',
  title: 'NOVO_DOC.md',
  category: 'Categoria',
  path: 'docs/NOVO_DOC.md',
  description: 'Descrição do documento',
  idealFor: 'Público alvo'
}
```

3. **Rebuild** a aplicação

### Estrutura de Documentação

- **Documentos do `/system-admin`**: Gerados automaticamente via `npm run generate-docs` (323 documentos)
- **Documentos customizados**: Configurados manualmente em `documentation.service.ts`
- **Assets**: Os arquivos markdown são copiados durante o build via configuração em `angular.json`


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

Este projeto faz parte do Omni Care Software e segue a mesma licença do projeto principal.

## 📞 Suporte

Para dúvidas ou problemas, consulte a documentação principal ou entre em contato com a equipe de desenvolvimento.

---

**Omni Care Software** © 2025 - Sistema de Gestão Médica
