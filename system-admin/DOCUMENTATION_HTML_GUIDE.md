# 📚 Guia de Documentação HTML Navegável

## 🎯 Visão Geral

A documentação do **system-admin** foi convertida para páginas HTML navegáveis através da aplicação **mw-docs**, uma aplicação Angular moderna que fornece uma interface web para acessar toda a documentação.

## ✨ Características

- **367 documentos** disponíveis (323 do system-admin + 44 customizados)
- **26 categorias** organizadas
- **Busca em tempo real** entre todos os documentos
- **Interface moderna e responsiva**
- **Renderização completa de Markdown** com syntax highlighting
- **Suporte a diagramas Mermaid**
- **Sem necessidade de acessar o GitHub** - tudo disponível em HTML

## 🌐 Como Acessar

### Opção 1: GitHub Pages (Produção)

Após o deploy, a documentação estará disponível em:
```
https://medicwarehouse.github.io/MW.Code/
```

### Opção 2: Localmente (Desenvolvimento)

Para executar localmente:

```bash
cd frontend/mw-docs
npm install
npm start
```

A aplicação estará disponível em: `http://localhost:4203/`

## 📁 Estrutura

### Documentos do System-Admin (323 documentos)

Os documentos do system-admin são **automaticamente incluídos** através de um script de geração:

#### Categorias:
- **🔧 Backend** (7 documentos) - API, controllers, serviços e arquitetura backend
- **⚕️ CFM Compliance** (15 documentos) - Resoluções do CFM
- **📚 Documentação Geral** (70+ documentos) - Documentação técnica e guias
- **🎨 Frontend** (12 documentos) - Componentes e páginas frontend
- **📖 Guias** (50+ documentos) - Guias de uso para diferentes perfis
- **🔧 Implementações** (40+ documentos) - Resumos de implementações
- **🏗️ Infraestrutura** (15+ documentos) - Deploy, Docker, CI/CD
- **📋 Regras de Negócio** (20+ documentos) - Especificações e políticas
- **🔒 Segurança** (6 documentos) - Análises de segurança e LGPD

### Documentos Customizados (44 documentos)

Documentos configurados manualmente em `documentation.service.ts` incluindo:
- Interface e Experiência do Usuário
- Regras de Negócio e Requisitos
- Implementação Técnica
- Guias de Uso
- CI/CD e Qualidade
- Segurança
- Sistema de Pagamentos
- WhatsApp AI Agent
- E mais...

## 🔄 Como Adicionar Novos Documentos

### Adicionando ao System-Admin

1. **Adicione o arquivo .md** na pasta `/system-admin` (em qualquer subdiretório)

2. **Regenere a estrutura** de documentação:
   ```bash
   cd frontend/mw-docs
   npm run generate-docs
   ```

3. **Rebuild a aplicação**:
   ```bash
   npm run build
   ```

4. **Commit as mudanças**:
   ```bash
   git add .
   git commit -m "docs: add new system-admin documentation"
   git push
   ```

O deploy automático para GitHub Pages será acionado!

### Adicionando Documentos Customizados

Para documentos que precisam de configuração especial:

1. **Adicione o arquivo .md** na pasta `/docs`

2. **Edite** `frontend/mw-docs/src/app/services/documentation.service.ts`

3. **Adicione à lista `originalDocs`**:
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

4. **Rebuild e commit**

## 🚀 Deploy

### Deploy Automático (GitHub Pages)

O deploy é automático quando:
- Há alterações na pasta `frontend/mw-docs/`
- Push para a branch `main`

O workflow `.github/workflows/deploy-docs.yml` cuida de:
1. Build da aplicação Angular
2. Deploy para GitHub Pages
3. Publicação em `https://medicwarehouse.github.io/MW.Code/`

### Habilitando GitHub Pages (Primeira Vez)

Se o deploy falhar com erro 404:

1. Acesse: `https://github.com/Omni CareSoftware/MW.Code/settings/pages`
2. Em "Source", selecione: **GitHub Actions**
3. Salve (automático)
4. Re-execute o workflow manualmente

Para mais detalhes, veja: `system-admin/docs/GITHUB_PAGES_SETUP_REQUIRED.md`

## 🔍 Funcionalidades

### Busca

A busca em tempo real permite encontrar documentos por:
- **Título** do arquivo
- **Descrição** do documento
- **Categoria**

### Navegação

- Clique em qualquer card de documento para visualizá-lo
- Use o botão "← Voltar" para retornar à lista
- Todos os links internos do Markdown funcionam

### Renderização

- **Markdown completo** com formatação
- **Syntax highlighting** para blocos de código
- **Diagramas Mermaid** renderizados automaticamente
- **Tabelas** formatadas
- **Imagens** (se disponíveis)

## 📊 Estatísticas

- **Total de Documentos**: 367
- **Documentos System-Admin**: 323
- **Documentos Customizados**: 44
- **Categorias**: 26
- **Tamanho do Bundle**: ~500KB (comprimido: ~127KB)

## 🛠️ Manutenção

### Regenerando Documentos

Para atualizar a lista de documentos após adicionar/remover arquivos:

```bash
cd frontend/mw-docs
npm run generate-docs
```

Isso escaneia o diretório `/system-admin` e atualiza automaticamente:
- `src/app/services/generated-docs.ts`

### Build de Produção

```bash
cd frontend/mw-docs
npm run build
```

Os arquivos estarão em: `dist/mw-docs/browser/`

### Testando Localmente

```bash
cd frontend/mw-docs
npm start
# Acesse: http://localhost:4203/
```

## 📝 Estrutura Técnica

### Arquivos Principais

- `generate-docs-structure.js` - Script que gera a estrutura de docs
- `src/app/services/generated-docs.ts` - Estrutura gerada automaticamente
- `src/app/services/documentation.service.ts` - Serviço principal
- `src/app/components/home/` - Página inicial com listagem
- `src/app/components/doc-viewer/` - Visualizador de documentos
- `angular.json` - Configuração de assets

### Como Funciona

1. **Geração**: Script Node.js escaneia `/system-admin` e extrai metadados
2. **Build**: Angular copia todos os arquivos .md para `dist/assets/`
3. **Runtime**: Aplicação carrega documentos via HTTP e renderiza com ngx-markdown
4. **Deploy**: GitHub Actions faz build e publica no GitHub Pages

## 🎨 Customização

### Modificando o Tema

Edite os arquivos SCSS:
- `src/styles.scss` - Estilos globais
- `src/app/components/home/home.component.scss` - Página inicial
- `src/app/components/doc-viewer/doc-viewer.component.scss` - Visualizador

### Adicionando Categorias

No script `generate-docs-structure.js`, edite o `CATEGORY_MAP`:

```javascript
const CATEGORY_MAP = {
  'nova-pasta': { name: '🆕 Nova Categoria', icon: '🆕' },
  // ...
};
```

## 🔐 Segurança

- Todos os documentos são **estáticos** (arquivos .md)
- **Sem backend** necessário
- **Sem autenticação** (documentação pública)
- **Sanitização** de HTML via Angular DomSanitizer

## 📚 Referências

- [README do mw-docs](../frontend/mw-docs/README.md)
- [Guia de Deploy](../frontend/mw-docs/DEPLOY.md)
- [GitHub Pages Setup](docs/GITHUB_PAGES_SETUP_REQUIRED.md)

## 🎉 Benefícios

### Antes
- ❌ Documentos espalhados em 323 arquivos .md
- ❌ Necessário navegar pelo GitHub ou clonar repositório
- ❌ Difícil encontrar documentação específica
- ❌ Sem busca unificada
- ❌ Visualização básica do Markdown no GitHub

### Depois
- ✅ Interface web moderna e navegável
- ✅ Acesso direto via URL (GitHub Pages)
- ✅ Busca em tempo real entre 367 documentos
- ✅ Organização por categorias
- ✅ Renderização completa com diagramas
- ✅ Design responsivo (mobile-friendly)
- ✅ Geração automática de novos documentos

## 💡 Dicas

1. **Adicione sempre títulos H1** no topo dos documentos .md para melhor extração
2. **Use descrições claras** no primeiro parágrafo
3. **Organize em subpastas** do system-admin para melhor categorização
4. **Execute `npm run generate-docs`** sempre após adicionar documentos
5. **Teste localmente** com `npm start` antes de fazer push

## 📞 Suporte

Para dúvidas ou problemas:
- Consulte a [documentação do Angular](https://angular.dev)
- Veja o [README do mw-docs](../frontend/mw-docs/README.md)
- Entre em contato com a equipe de desenvolvimento

---

**Omni Care Software** © 2025 - Documentação HTML Navegável
