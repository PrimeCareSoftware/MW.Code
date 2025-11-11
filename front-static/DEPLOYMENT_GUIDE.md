# Guia de Deploy do Frontend Estático no GitHub Pages

Este guia explica como fazer o deploy do frontend estático no GitHub Pages.

## 📋 Pré-requisitos

1. **GitHub Pages habilitado** no repositório
2. **Permissões corretas** configuradas no GitHub Actions
3. **Branch main** com os arquivos do `front-static/`

## 🚀 Passo a Passo para Deploy

### 1. Habilitar GitHub Pages

1. Acesse as configurações do repositório:
   ```
   https://github.com/MedicWarehouse/MW.Code/settings/pages
   ```

2. Em **"Build and deployment"**, configure:
   - **Source**: Selecione `GitHub Actions`
   - ⚠️ **NÃO** selecione "Deploy from a branch"

3. As configurações são salvas automaticamente

### 2. Verificar Permissões do GitHub Actions

1. Acesse:
   ```
   https://github.com/MedicWarehouse/MW.Code/settings/actions
   ```

2. Em **"Workflow permissions"**, certifique-se de que está configurado:
   - ✅ **Read and write permissions**
   
3. Em **"Fork pull request workflows"** (se aplicável):
   - ✅ Marque "Allow GitHub Actions to create and approve pull requests"

### 3. Fazer Deploy

O deploy acontece automaticamente quando:

#### Opção A: Push para Main (Automático)
```bash
# Qualquer alteração na pasta front-static/ na branch main
git add front-static/
git commit -m "Update static frontend"
git push origin main
```

#### Opção B: Trigger Manual
1. Acesse: `https://github.com/MedicWarehouse/MW.Code/actions`
2. Selecione o workflow **"Deploy Static Frontend to GitHub Pages"**
3. Clique em **"Run workflow"**
4. Selecione a branch **main**
5. Clique em **"Run workflow"**

### 4. Acompanhar o Deploy

1. Vá para a aba **Actions**:
   ```
   https://github.com/MedicWarehouse/MW.Code/actions
   ```

2. Encontre o workflow mais recente:
   - **Nome**: "Deploy Static Frontend to GitHub Pages"
   - **Status**: 🟡 Em andamento ou ✅ Concluído

3. Clique no workflow para ver detalhes e logs

### 5. Acessar o Site

Após o deploy bem-sucedido (geralmente 2-3 minutos):

**URL Principal:**
```
https://medicwarehouse.github.io/MW.Code/front-static/
```

**URLs das Aplicações:**
- Landing Page: `https://medicwarehouse.github.io/MW.Code/front-static/`
- Main App: `https://medicwarehouse.github.io/MW.Code/front-static/medicwarehouse-app/`
- Site: `https://medicwarehouse.github.io/MW.Code/front-static/mw-site/`
- Admin: `https://medicwarehouse.github.io/MW.Code/front-static/mw-system-admin/`
- Docs: `https://medicwarehouse.github.io/MW.Code/front-static/mw-docs/`
- Documentação: `https://medicwarehouse.github.io/MW.Code/front-static/documentacao/MedicWarehouse-Documentacao-Completa.html`

## 🔄 Atualizando o Frontend

### Para fazer alterações:

1. **Modifique o código fonte** em `frontend/{app-name}/`

2. **Reconstrua o frontend estático:**
   ```bash
   bash build-static.sh
   ```

3. **Commit e push das mudanças:**
   ```bash
   git add front-static/
   git commit -m "Update static frontend - [descrição]"
   git push origin main
   ```

4. **Aguarde o deploy automático** (2-3 minutos)

### Para mudanças nos dados mock:

1. **Edite os arquivos mock** em `frontend/{app-name}/src/app/mocks/`

2. **Reconstrua:**
   ```bash
   bash build-static.sh
   ```

3. **Commit e push:**
   ```bash
   git add front-static/
   git commit -m "Update mock data"
   git push origin main
   ```

## 🔍 Troubleshooting

### Deploy Falha com Erro 404

**Causa:** GitHub Pages não está habilitado ou configurado incorretamente.

**Solução:**
1. Vá para Settings → Pages
2. Certifique-se de que "Source" está como "GitHub Actions"
3. Aguarde alguns minutos e tente novamente

### Deploy Falha com Erro de Permissões

**Causa:** Workflow não tem permissões para deploy.

**Solução:**
1. Vá para Settings → Actions → General
2. Em "Workflow permissions", selecione "Read and write permissions"
3. Salve e execute o workflow novamente

### Site Não Carrega Corretamente

**Causa:** Base href incorreto ou caminhos quebrados.

**Solução:**
1. Verifique que o `baseHref` está correto em `angular.json`:
   ```json
   "baseHref": "/MW.Code/front-static/{app-name}/"
   ```
2. Reconstrua com `bash build-static.sh`
3. Faça commit e push

### Arquivos Não Atualizam

**Causa:** Cache do GitHub Pages.

**Solução:**
1. Limpe o cache do navegador (Ctrl+Shift+R ou Cmd+Shift+R)
2. Aguarde alguns minutos para propagação do CDN
3. Tente em modo anônimo/privado

## 📊 Monitoramento

### Ver Logs do Deploy

1. Acesse Actions → Selecione o workflow
2. Clique em um job para ver logs detalhados
3. Procure por erros ou avisos

### Verificar Status do GitHub Pages

1. Vá para Settings → Pages
2. Você deve ver:
   ```
   Your site is live at https://medicwarehouse.github.io/MW.Code/front-static/
   ```

## 🔐 Segurança

### O que está sendo deployado:

- ✅ Apenas arquivos estáticos (HTML, CSS, JS)
- ✅ Sem código do backend
- ✅ Sem secrets ou credenciais
- ✅ Sem acesso a banco de dados
- ✅ Apenas dados mockados

### O que NÃO está sendo deployado:

- ❌ Código fonte do backend (.NET)
- ❌ Código TypeScript (apenas JS compilado)
- ❌ Variáveis de ambiente sensíveis
- ❌ node_modules
- ❌ Arquivos de configuração local

## 📝 Workflow Configurado

O arquivo `.github/workflows/deploy-front-static.yml` está configurado para:

1. **Trigger automático** em push para main que afeta `front-static/**`
2. **Deploy manual** via workflow_dispatch
3. **Concurrency control** para evitar deploys simultâneos
4. **Permissões apropriadas** para Pages e ID token
5. **Upload apenas da pasta front-static**

## 💡 Dicas

### Teste Local Antes do Deploy

```bash
# Instale um servidor estático simples
npm install -g http-server

# Sirva a pasta front-static
cd front-static
http-server -p 8080

# Acesse http://localhost:8080
```

### Simule o Base Href Local

```bash
# Crie uma estrutura de pastas simulando GitHub Pages
mkdir -p /tmp/MW.Code/front-static
cp -r front-static/* /tmp/MW.Code/front-static/
cd /tmp
http-server -p 8080

# Acesse http://localhost:8080/MW.Code/front-static/
```

### Build Rápido de Uma App

```bash
cd frontend/medicwarehouse-app
npm run build -- --configuration=static
cp -r dist/medicwarehouse-app/browser ../../front-static/medicwarehouse-app
```

## 📞 Suporte

Se encontrar problemas:

1. Verifique os logs do workflow no GitHub Actions
2. Consulte o README em `front-static/README.md`
3. Abra uma issue no repositório
4. Consulte a documentação do GitHub Pages: https://docs.github.com/pages

## ✅ Checklist Final

Antes de considerar o deploy completo, verifique:

- [ ] GitHub Pages está habilitado
- [ ] Source está configurado como "GitHub Actions"
- [ ] Workflow executou com sucesso
- [ ] Site está acessível na URL do GitHub Pages
- [ ] Todas as aplicações carregam corretamente
- [ ] Navegação entre páginas funciona
- [ ] Mock data está funcionando
- [ ] Sem erros no console do navegador
- [ ] README principal foi atualizado

## 🎉 Pronto!

Seu frontend estático está deployado e acessível publicamente via GitHub Pages!
