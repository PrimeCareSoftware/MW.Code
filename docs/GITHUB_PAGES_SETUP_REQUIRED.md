# ⚠️ AÇÃO NECESSÁRIA: Habilitar GitHub Pages

## 🚨 Status Atual: Deploy Falhou

O deploy da documentação para GitHub Pages está **falhando** com o seguinte erro:

```
Error: Creating Pages deployment failed
Error: HttpError: Not Found
Error: Failed to create deployment (status: 404)
```

## 🎯 Causa do Problema

**GitHub Pages não está habilitado nas configurações do repositório.**

O workflow de deploy está configurado corretamente, mas o GitHub Pages precisa ser habilitado manualmente nas configurações do repositório para que o deploy funcione.

## ✅ Solução: Habilitar GitHub Pages (5 minutos)

### Passo a Passo

1. **Acesse as configurações do repositório:**
   - Vá para: https://github.com/PrimeCare Software/MW.Code/settings/pages
   - Ou navegue: **Repository** → **Settings** → **Pages** (no menu lateral esquerdo)

2. **Configure a fonte (Source):**
   - Em **"Source"** ou **"Build and deployment"**, selecione: **GitHub Actions**
   - ⚠️ **NÃO selecione** "Deploy from a branch" - selecione **"GitHub Actions"**

3. **Salve as configurações:**
   - As configurações são salvas automaticamente ao selecionar "GitHub Actions"
   - Não há necessidade de configurar branch ou diretório

4. **Aguarde alguns segundos:**
   - O GitHub pode levar alguns segundos para processar a mudança

### Imagem de Referência

![GitHub Pages Configuration](https://docs.github.com/assets/cb-47267/mw-1440/images/help/pages/configure-publishing-source.webp)

Na seção **"Build and deployment"**, certifique-se de que está selecionado:
- **Source**: GitHub Actions

## 🚀 Após Habilitar GitHub Pages

1. **Execute o workflow novamente:**
   - Vá para: https://github.com/PrimeCare Software/MW.Code/actions
   - Selecione o workflow **"Deploy MW.Docs to GitHub Pages"**
   - Clique em **"Run workflow"**
   - Selecione a branch **main**
   - Clique em **"Run workflow"**

2. **Aguarde o deploy:**
   - O workflow levará aproximadamente 2-3 minutos para completar
   - Você pode acompanhar o progresso na aba Actions

3. **Acesse a documentação:**
   - Após o deploy bem-sucedido, acesse: https://medicwarehouse.github.io/MW.Code/

## 📋 Checklist de Verificação

Após habilitar GitHub Pages, verifique:

- [ ] GitHub Pages está habilitado em Settings → Pages
- [ ] Source está configurado como "GitHub Actions"
- [ ] Workflow foi executado novamente
- [ ] Deploy foi concluído com sucesso (status: ✅)
- [ ] Documentação está acessível em https://medicwarehouse.github.io/MW.Code/

## 🔍 Verificando se está Configurado Corretamente

Você saberá que GitHub Pages está configurado corretamente quando:

1. **Em Settings → Pages**, você ver:
   ```
   Your site is ready to be published at https://medicwarehouse.github.io/MW.Code/
   ```
   ou
   ```
   Your site is live at https://medicwarehouse.github.io/MW.Code/
   ```

2. **No workflow (Actions)**, o job "Deploy to GitHub Pages" completar com sucesso:
   ```
   ✅ Creating Pages deployment with payload
   ✅ Deploy to GitHub Pages
   ```

## 📞 Suporte Adicional

Se após seguir estes passos o deploy continuar falhar:

1. **Verifique permissões:**
   - Em **Settings** → **Actions** → **General**
   - Em **"Workflow permissions"**, certifique-se de que está em **"Read and write permissions"**

2. **Verifique se o repositório é público:**
   - GitHub Pages gratuito funciona apenas para repositórios públicos
   - Para repositórios privados, é necessário GitHub Pro/Team/Enterprise

3. **Consulte a documentação:**
   - GitHub Pages: https://docs.github.com/pages
   - GitHub Actions: https://docs.github.com/actions
   - Deploy específico: [frontend/mw-docs/DEPLOY.md](frontend/mw-docs/DEPLOY.md)

## 🎉 Próximos Passos

Após habilitar GitHub Pages e realizar o primeiro deploy com sucesso:

1. **Deploys automáticos:**
   - Cada push para `main` que modificar arquivos em `frontend/mw-docs/` fará deploy automático
   - Não é necessário executar o workflow manualmente novamente

2. **Documentação:**
   - Adicione novos arquivos `.md` em `frontend/mw-docs/src/assets/docs/`
   - Faça commit e push para `main`
   - O deploy acontecerá automaticamente

3. **Remova este arquivo:**
   - Após configurar com sucesso, você pode remover este arquivo:
   ```bash
   git rm GITHUB_PAGES_SETUP_REQUIRED.md
   git commit -m "docs: remove setup instructions after GitHub Pages configured"
   git push origin main
   ```

---

**⏰ Esta é uma configuração única que leva apenas 5 minutos!**

**🔗 Link direto para configuração:** https://github.com/PrimeCare Software/MW.Code/settings/pages
