# 🚀 Deploy da Documentação MW.Docs no GitHub Pages

> ⚠️ **IMPORTANTE**: Se o deploy está falando com erro 404, GitHub Pages precisa ser habilitado primeiro. [Ver instruções detalhadas →](../../GITHUB_PAGES_SETUP_REQUIRED.md)

Este documento explica como configurar e usar o deploy automático do projeto de documentação MW.Docs no GitHub Pages.

## 📋 Visão Geral

O workflow de deploy foi criado para publicar automaticamente a documentação do projeto MedicWarehouse sempre que houver alterações na branch `main`. A documentação ficará disponível publicamente em:

**URL**: https://medicwarehouse.github.io/MW.Code/

## ⚙️ Configuração Necessária

### 1. Habilitar GitHub Pages no Repositório

Para que o deploy funcione, você precisa habilitar GitHub Pages nas configurações do repositório:

1. Acesse: **Settings** → **Pages** (no menu lateral)
2. Em **Source**, selecione: **GitHub Actions**
3. Salve as configurações

![GitHub Pages Configuration](https://docs.github.com/assets/cb-47267/mw-1440/images/help/pages/configure-publishing-source.webp)

### 2. Verificar Permissões

O workflow já está configurado com as permissões necessárias:
- `contents: read` - Para ler o código do repositório
- `pages: write` - Para escrever no GitHub Pages
- `id-token: write` - Para autenticação

Nenhuma configuração adicional é necessária.

## 🔧 Como Funciona

### Arquivo de Workflow

O workflow está localizado em: `.github/workflows/deploy-docs.yml`

### Triggers (Gatilhos)

O deploy é executado automaticamente quando:

1. **Push para main** com alterações em:
   - `frontend/mw-docs/**` (qualquer arquivo do projeto de documentação)
   - `.github/workflows/deploy-docs.yml` (o próprio arquivo de workflow)

2. **Execução manual** via GitHub Actions (botão "Run workflow")

### Processo de Deploy

O workflow executa dois jobs:

#### Job 1: Build
1. Faz checkout do código
2. Configura Node.js 20.x com cache do npm
3. Instala dependências com `npm ci`
4. Build da aplicação Angular com `npm run build -- --base-href=/MW.Code/`
5. Faz upload dos arquivos buildados como artefato

#### Job 2: Deploy
1. Aguarda conclusão do job de build
2. Faz deploy dos arquivos para GitHub Pages
3. Retorna a URL da documentação publicada

## 📦 Estrutura de Build

O build do Angular gera os seguintes arquivos em `dist/mw-docs/browser/`:

```
browser/
├── assets/              # Arquivos de documentação Markdown
├── favicon.ico
├── index.html
├── main-[hash].js       # JavaScript principal (~349KB)
├── polyfills-[hash].js  # Polyfills (~34KB)
├── styles-[hash].css    # Estilos (~219 bytes)
└── .nojekyll           # Desabilita processamento Jekyll
```

### Base HREF

O build é configurado com `--base-href=/MW.Code/` para garantir que os links funcionem corretamente quando a aplicação estiver hospedada em `https://medicwarehouse.github.io/MW.Code/`.

### Arquivo .nojekyll

O arquivo `.nojekyll` é incluído automaticamente no build (localizado em `frontend/mw-docs/public/.nojekyll`) para evitar que o GitHub Pages processe os arquivos com Jekyll, o que poderia causar problemas com arquivos que começam com underscore.

## 🚀 Como Usar

### Deploy Automático

Simplesmente faça commit e push das suas alterações para a branch `main`:

```bash
cd frontend/mw-docs

# Faça suas alterações na documentação
# Por exemplo, adicione um novo arquivo .md em src/assets/docs/

git add .
git commit -m "docs: adiciona nova documentação"
git push origin main
```

O deploy será executado automaticamente.

### Deploy Manual

Para fazer deploy manual:

1. Acesse: **Actions** → **Deploy MW.Docs to GitHub Pages**
2. Clique em **Run workflow**
3. Selecione a branch `main`
4. Clique em **Run workflow**

### Verificar Status do Deploy

1. Acesse a aba **Actions** no GitHub
2. Encontre o workflow **Deploy MW.Docs to GitHub Pages**
3. Clique na execução mais recente para ver os logs

### Acessar a Documentação

Após o deploy ser concluído (geralmente leva 2-3 minutos), acesse:

**https://medicwarehouse.github.io/MW.Code/**

## 🔍 Troubleshooting

### Erro: "Pages deployment failed" ou "HttpError: Not Found (404)"

**Causa**: GitHub Pages **NÃO está habilitado** no repositório.

**Solução**: 
1. **PRIMEIRO**: Habilite GitHub Pages em **Settings** → **Pages**
2. Em **Source**, selecione: **GitHub Actions** (não "Deploy from a branch")
3. Aguarde alguns segundos para o GitHub processar
4. Execute o workflow novamente em **Actions**

📖 **Ver guia completo**: [GITHUB_PAGES_SETUP_REQUIRED.md](../../GITHUB_PAGES_SETUP_REQUIRED.md)

🔗 **Link direto**: https://github.com/MedicWarehouse/MW.Code/settings/pages

### Erro: "Permission denied"

**Causa**: Falta de permissões no workflow.

**Solução**: As permissões já estão configuradas no arquivo de workflow. Verifique se o repositório permite workflows terem permissão de escrita.

### A página não carrega corretamente

**Causa**: Base href incorreto ou falta do arquivo .nojekyll.

**Solução**: 
- Verifique se o build inclui `--base-href=/MW.Code/`
- Verifique se existe o arquivo `.nojekyll` em `frontend/mw-docs/public/`

### Alterações não aparecem na documentação

**Causa**: Cache do navegador ou deploy ainda em andamento.

**Solução**:
1. Verifique o status do workflow em **Actions**
2. Aguarde alguns minutos após a conclusão
3. Limpe o cache do navegador (Ctrl+Shift+R ou Cmd+Shift+R)

## 📊 Métricas do Build

- **Tempo de build**: ~7-10 segundos
- **Tempo total de deploy**: ~2-3 minutos
- **Tamanho do bundle**:
  - Raw: ~384 KB
  - Comprimido: ~103 KB
- **Node.js**: 20.x
- **Angular**: 20.3.5

## 🔄 Atualizações Futuras

### Adicionar Novo Documento

1. Adicione o arquivo `.md` em `frontend/mw-docs/src/assets/docs/`
2. Atualize `frontend/mw-docs/src/app/services/documentation.service.ts`
3. Commit e push para `main`
4. O deploy será executado automaticamente

### Modificar Configurações de Build

Edite as configurações em `frontend/mw-docs/angular.json` na seção `production`.

### Alterar Trigger do Workflow

Edite `.github/workflows/deploy-docs.yml` para adicionar ou remover gatilhos:

```yaml
on:
  push:
    branches: [ main, develop ]  # Adicionar mais branches
    paths:
      - 'frontend/mw-docs/**'
  pull_request:  # Adicionar PR trigger
    branches: [ main ]
  workflow_dispatch:
```

## 📝 Checklist de Deploy

Antes de fazer push para produção:

- [ ] Build local funciona: `npm run build -- --base-href=/MW.Code/`
- [ ] Sem erros no console
- [ ] Documentação renderiza corretamente
- [ ] Links internos funcionam
- [ ] Imagens e assets carregam
- [ ] Diagramas Mermaid renderizam
- [ ] Busca funciona corretamente

## 🤝 Contribuindo

Para contribuir com melhorias no processo de deploy:

1. Crie uma branch de feature
2. Faça suas alterações
3. Teste localmente
4. Abra um Pull Request

## 📞 Suporte

Para problemas relacionados ao deploy:

- **Issues**: https://github.com/MedicWarehouse/MW.Code/issues
- **Documentação GitHub Pages**: https://docs.github.com/pages
- **Documentação GitHub Actions**: https://docs.github.com/actions

---

**MedicWarehouse** © 2025 - Sistema de Gestão Médica
