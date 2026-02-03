# Vídeo Demonstrativo - README

> **Quick Start Guide**  
> **Para:** Equipe de Produção, Stakeholders  
> **Status:** 🚧 80% Completo - Aguardando Produção do Vídeo

---

## 📋 O Que Foi Feito?

### ✅ Completo (80%)

Todo o planejamento, documentação e infraestrutura técnica estão prontos:

1. **Script Completo** → [VIDEO_DEMONSTRATIVO_SCRIPT.md](./VIDEO_DEMONSTRATIVO_SCRIPT.md)
   - Storyboard detalhado com timing preciso
   - Narração palavra por palavra
   - 6 features a serem demonstradas
   - Diretrizes visuais e técnicas

2. **Guia de Produção** → [VIDEO_PRODUCTION_GUIDE.md](./VIDEO_PRODUCTION_GUIDE.md)
   - Setup de ambiente de gravação
   - Ferramentas recomendadas (OBS, DaVinci Resolve)
   - Passo a passo para cada feature
   - Checklist de qualidade

3. **Código Implementado**
   - Player de vídeo condicional na homepage
   - Placeholder elegante enquanto vídeo não existe
   - Pronto para receber URL do vídeo

4. **Status Detalhado** → [PROMPT2_IMPLEMENTATION_STATUS.md](./PROMPT2_IMPLEMENTATION_STATUS.md)
   - Acompanhamento completo do progresso
   - Orçamento e timeline
   - Próximos passos

### ⏳ Pendente (20%)

Aguardando:
- **Produção do vídeo** (gravação + edição + narração)
- **Publicação** (upload + integração)

---

## 🎬 Como Produzir o Vídeo?

### Opção 1: Equipe Interna

**Se você tem:**
- Editor de vídeo experiente
- Software (OBS, Premiere/DaVinci Resolve)
- Microfone de qualidade
- Tempo disponível (~2 semanas)

**Faça:**
1. Leia [VIDEO_DEMONSTRATIVO_SCRIPT.md](./VIDEO_DEMONSTRATIVO_SCRIPT.md)
2. Siga [VIDEO_PRODUCTION_GUIDE.md](./VIDEO_PRODUCTION_GUIDE.md)
3. Grave as telas conforme roteiro
4. Edite seguindo as especificações
5. Publique (YouTube ou Vimeo)
6. Atualize o código (ver "Como Integrar")

**Custo:** R$ 2.000 - 3.000 (música, ferramentas, narrador)

### Opção 2: Freelancer/Agência

**Se você prefere contratar:**

**Perfil Necessário:**
- Editor de vídeo com experiência em screencasts
- Portfolio com vídeos corporativos/SaaS
- Narrador profissional PT-BR (pode ser separado)

**O que entregar para o freelancer:**
- [VIDEO_DEMONSTRATIVO_SCRIPT.md](./VIDEO_DEMONSTRATIVO_SCRIPT.md)
- [VIDEO_PRODUCTION_GUIDE.md](./VIDEO_PRODUCTION_GUIDE.md)
- Acesso ao sistema demo (ou gravar você mesmo e enviar)

**Custo:** R$ 5.000 - 8.000

**Onde encontrar:**
- Workana (https://www.workana.com)
- 99Freelas (https://www.99freelas.com.br)
- GetNinjas (https://www.getninjas.com.br)
- Upwork (internacional, mais caro)

---

## 🚀 Como Integrar o Vídeo no Site?

### Passo 1: Publicar o Vídeo

**Opção A: YouTube (Recomendado para começar)**

1. Criar/usar conta YouTube da empresa
2. Upload do vídeo
3. Configurar:
   - Título: "Omni Care Software - Vídeo Demonstrativo"
   - Descrição: [copiar de VIDEO_DEMONSTRATIVO_SCRIPT.md]
   - Thumbnail: personalizado (extrair do vídeo)
   - Privacidade: "Público" ou "Não listado"
4. Adicionar legendas PT-BR (arquivo SRT)
5. Obter URL de embed:
   - Clicar em "Compartilhar" → "Incorporar"
   - Copiar URL do iframe
   - Exemplo: `https://www.youtube.com/embed/ABC123XYZ`

**Opção B: Vimeo (Mais profissional)**

1. Criar conta Vimeo Pro ($20/mês)
2. Upload do vídeo
3. Personalizar player (cores, logo)
4. Obter URL de embed:
   - "Share" → "Embed"
   - Copiar URL
   - Exemplo: `https://player.vimeo.com/video/123456789`

### Passo 2: Atualizar o Código

**Arquivo:** `/frontend/medicwarehouse-app/src/app/pages/site/home/home.ts`

**Linha ~23 (aproximadamente):**

**ANTES:**
```typescript
demoVideoUrl: string = ''; // Empty = show placeholder
```

**DEPOIS (YouTube):**
```typescript
demoVideoUrl: string = 'https://www.youtube.com/embed/VIDEO_ID?rel=0&modestbranding=1&cc_load_policy=1&cc_lang_pref=pt';
```

**OU DEPOIS (Vimeo):**
```typescript
demoVideoUrl: string = 'https://player.vimeo.com/video/VIDEO_ID';
```

**Substitua `VIDEO_ID` pelo ID real do seu vídeo!**

### Passo 3: Commit e Deploy

```bash
cd /home/runner/work/MW.Code/MW.Code
git add frontend/medicwarehouse-app/src/app/pages/site/home/home.ts
git commit -m "Add demo video URL to homepage"
git push
```

Depois faça deploy da aplicação.

### Passo 4: Testar

1. Abrir homepage em navegador
2. Verificar se vídeo aparece (não mais o placeholder)
3. Testar play/pause
4. Testar em mobile
5. Verificar legendas

**Pronto! 🎉**

---

## 💡 Perguntas Frequentes

### "Quanto tempo vai levar para produzir?"

**Resposta:** 2-3 semanas se contratar profissionais:
- Gravação: 3 dias
- Edição: 5 dias
- Revisão: 2 dias
- Ajustes: 1-2 dias
- Publicação: 1 dia

### "Posso fazer um vídeo simples primeiro e melhorar depois?"

**Resposta:** Sim! Pode fazer um MVP:
- Gravar tela com OBS (gratuito)
- Narração sua mesmo (se clara)
- Edição básica
- Upload no YouTube

Depois substitui por versão profissional quando tiver orçamento.

### "O vídeo vai funcionar em mobile?"

**Resposta:** Sim! O player é responsivo e funciona perfeitamente em:
- iOS (iPhone/iPad)
- Android
- Desktop (Windows/Mac/Linux)

### "Preciso de legendas?"

**Resposta:** **Sim!** Por duas razões:
1. **Acessibilidade** (WCAG 2.1 AA - requerido por lei)
2. **Conversão** (muitas pessoas assistem sem som)

YouTube gera legendas automáticas, mas revisar manualmente é importante.

### "Quanto custa hospedar o vídeo?"

| Plataforma | Custo | Observações |
|-----------|-------|-------------|
| **YouTube** | Grátis | Tem logo YouTube, mas é aceitável |
| **Vimeo Pro** | $20/mês | Sem branding, mais profissional |
| **AWS S3** | ~$20-50/mês | Só se quiser controle total |

**Recomendação:** YouTube para começar.

---

## 📞 Precisa de Ajuda?

### Documentação Completa

1. **Script do Vídeo:**  
   📄 [VIDEO_DEMONSTRATIVO_SCRIPT.md](./VIDEO_DEMONSTRATIVO_SCRIPT.md)

2. **Guia de Produção:**  
   📄 [VIDEO_PRODUCTION_GUIDE.md](./VIDEO_PRODUCTION_GUIDE.md)

3. **Status Detalhado:**  
   📄 [PROMPT2_IMPLEMENTATION_STATUS.md](./PROMPT2_IMPLEMENTATION_STATUS.md)

4. **Plano Geral:**  
   📄 [PLANO_MELHORIAS_WEBSITE_UXUI.md](./PLANO_MELHORIAS_WEBSITE_UXUI.md)

### Contato Técnico

**Dúvidas sobre código/integração:**  
Falar com Tech Lead ou abrir issue no GitHub

**Dúvidas sobre conteúdo/script:**  
Consultar documentos acima ou Product Manager

---

## ✅ Checklist Rápido

Antes de considerar 100% completo:

- [ ] Vídeo produzido (2-3 minutos, MP4 1080p)
- [ ] Narração profissional em PT-BR
- [ ] Legendas criadas (SRT/VTT)
- [ ] Música de fundo adicionada
- [ ] Upload realizado (YouTube/Vimeo)
- [ ] URL do vídeo obtida
- [ ] Código atualizado (`demoVideoUrl` preenchido)
- [ ] Deploy realizado
- [ ] Testado em desktop
- [ ] Testado em mobile
- [ ] Legendas validadas
- [ ] Analytics configurado

---

## 🎯 Resultado Esperado

Quando tudo estiver pronto, o visitante da homepage verá:

1. **Hero section** chamativa
2. **Seção de vídeo** com player profissional ✨
3. Vídeo de 2-3 minutos mostrando o sistema
4. Legendas em português
5. Call-to-action claro para trial gratuito

**Objetivo:** Aumentar conversão de visitantes → trial em 20%+

---

**Última Atualização:** 28 de Janeiro de 2026  
**Versão:** 1.0  
**Status:** 🚧 80% Completo
