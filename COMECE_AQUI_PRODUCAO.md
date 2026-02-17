# 🚀 COMECE AQUI - Deploy em Produção

> **🎯 Você está prestes a publicar seu sistema em produção hoje!**  
> Este guia irá te direcionar para a documentação correta.

## ❓ O Que Você Precisa Fazer?

Você quer fazer o **deploy completo em produção** no Hostinger KVM 2, **SEM o Portal do Paciente**.

## 📖 Qual Guia Seguir?

### 🔥 Guia Principal: [PRODUCAO_HOSTINGER_GUIDE.md](PRODUCAO_HOSTINGER_GUIDE.md)

Este é o guia **mais importante**. Siga ele do início ao fim.

**O que tem neste guia?**
- ✅ Todos os passos do zero até produção
- ✅ Comandos prontos para copiar e colar
- ✅ Configuração do Hostinger KVM 2
- ✅ Deploy de todas as aplicações (exceto portal)
- ✅ Configuração de segurança
- ✅ SSL/HTTPS
- ✅ Backups
- ✅ Checklist final

**Tempo**: 5-8 horas na primeira vez

---

## 🌐 Dúvida Sobre Subdomínios?

Se você não encontra a opção de criar subdomínios no painel da Hostinger:

### 👉 [HOSTINGER_SUBDOMAINS_GUIDE.md](HOSTINGER_SUBDOMAINS_GUIDE.md)

**Por que este guia existe?**

No VPS (KVM 2), você **NÃO** cria subdomínios como na hospedagem compartilhada. Você precisa:
1. Criar **Registros DNS tipo A** no painel
2. Configurar o **Nginx** no servidor

Este guia mostra **exatamente onde** e **como** fazer isso!

**Tempo**: 10 minutos para ler, 30 minutos para configurar

---

## 🔒 Como Garantir Máxima Segurança?

Seu sistema vai lidar com **dados sensíveis** (médicos). Segurança é CRÍTICA!

### 👉 [SECURITY_PRODUCTION_GUIDE.md](SECURITY_PRODUCTION_GUIDE.md)

**O que tem neste guia?**
- ✅ Conformidade LGPD (obrigatório!)
- ✅ Conformidade CFM (obrigatório!)
- ✅ Criptografia de dados
- ✅ Backups seguros
- ✅ Firewall e Fail2Ban
- ✅ Plano de resposta a incidentes

**Quando usar?**
- Leia **DURANTE** o deploy (seção 10 do guia principal)
- Implemente **ANTES** de ir para produção
- Revise **TRIMESTRALMENTE**

**Tempo**: 2-3 horas para implementar tudo

---

## 🐳 Quer Usar Docker?

Se preferir usar Docker/Podman ao invés de instalar tudo manualmente:

### 👉 [docker-compose.production-no-portal.yml](docker-compose.production-no-portal.yml)

**O que é isso?**

Um arquivo Docker Compose **otimizado** para produção, **SEM o Portal do Paciente**.

**Como usar?**

```bash
# 1. Copie .env.production.example para .env.production
cp .env.production.example .env.production

# 2. Edite .env.production e configure suas senhas e domínios
nano .env.production

# 3. Inicie todos os serviços
docker compose -f docker-compose.production-no-portal.yml up -d

# 4. Veja os logs
docker compose -f docker-compose.production-no-portal.yml logs -f
```

**Vantagem**: Mais fácil e rápido  
**Desvantagem**: Requer conhecimento básico de Docker

---

## ⚙️ Template de Variáveis de Ambiente

### 👉 [.env.production.example](.env.production.example)

**O que é isso?**

Um **template** com todas as variáveis que você precisa configurar para produção.

**Como usar?**

```bash
# 1. Copiar o exemplo
cp .env.production.example .env.production

# 2. Editar com suas configurações
nano .env.production

# 3. IMPORTANTE: Proteger o arquivo
chmod 600 .env.production
```

**⚠️ NUNCA commite o arquivo `.env.production` no Git!**

---

## 📋 Resumo Executivo

Quer uma **visão geral** de tudo antes de começar?

### 👉 [DEPLOYMENT_SUMMARY.md](DEPLOYMENT_SUMMARY.md)

**O que tem?**
- Resumo de todos os guias criados
- Custos estimados
- Tempo necessário
- O que está incluído/excluído
- Como usar cada documento

**Tempo**: 5 minutos

---

## 🎯 Passo a Passo Simplificado

### Método 1: Manual (Recomendado para Iniciantes)

1. **Leia**: [DEPLOYMENT_SUMMARY.md](DEPLOYMENT_SUMMARY.md) (5 min)
2. **Siga**: [PRODUCAO_HOSTINGER_GUIDE.md](PRODUCAO_HOSTINGER_GUIDE.md) (5-8h)
3. **Configure Subdomínios**: [HOSTINGER_SUBDOMAINS_GUIDE.md](HOSTINGER_SUBDOMAINS_GUIDE.md) (30 min)
4. **Implemente Segurança**: [SECURITY_PRODUCTION_GUIDE.md](SECURITY_PRODUCTION_GUIDE.md) (2-3h)
5. **✅ Teste tudo e vá para produção!**

### Método 2: Com Docker (Mais Rápido)

1. **Leia**: [DEPLOYMENT_SUMMARY.md](DEPLOYMENT_SUMMARY.md) (5 min)
2. **Configure VPS**: Seções 1-3 do [PRODUCAO_HOSTINGER_GUIDE.md](PRODUCAO_HOSTINGER_GUIDE.md) (1h)
3. **Use Docker**: [docker-compose.production-no-portal.yml](docker-compose.production-no-portal.yml) (30 min)
4. **Configure DNS**: [HOSTINGER_SUBDOMAINS_GUIDE.md](HOSTINGER_SUBDOMAINS_GUIDE.md) (30 min)
5. **Configure SSL**: Seção 9 do [PRODUCAO_HOSTINGER_GUIDE.md](PRODUCAO_HOSTINGER_GUIDE.md) (15 min)
6. **Segurança**: [SECURITY_PRODUCTION_GUIDE.md](SECURITY_PRODUCTION_GUIDE.md) (2h)
7. **✅ Teste tudo e vá para produção!**

---

## 💰 Quanto Vai Custar?

| Item | Custo Mensal |
|------|--------------|
| Hostinger KVM 2 | R$ 39,99 |
| Domínio .com.br | R$ 3,33 |
| SSL (Let's Encrypt) | R$ 0,00 |
| Daily.co (Telemedicina) | R$ 0-500 |
| **Total** | **R$ 43-573** |

**Economia**: 60-80% vs AWS/Azure/Google Cloud

---

## 🚦 O Que Será Implantado?

### ✅ Componentes INCLUÍDOS:
- 🌐 Site Principal (Frontend)
- 🔧 Sistema de Administração (Frontend)
- 🔌 API Principal (Backend)
- 📹 Microserviço de Telemedicina (Backend)
- 🗄️ Banco de Dados PostgreSQL

### ❌ Componentes EXCLUÍDOS:
- ~~Portal do Paciente (Frontend)~~
- ~~API do Portal do Paciente (Backend)~~

**Motivo**: Conforme solicitado, o portal ficará de fora por enquanto.

---

## 🆘 Precisa de Ajuda?

### Durante o Deploy

1. **Problema com subdomínios?**
   → Veja [HOSTINGER_SUBDOMAINS_GUIDE.md](HOSTINGER_SUBDOMAINS_GUIDE.md), seção "Troubleshooting"

2. **Erro ao iniciar API?**
   → Veja [PRODUCAO_HOSTINGER_GUIDE.md](PRODUCAO_HOSTINGER_GUIDE.md), seção 12 "Troubleshooting"

3. **Dúvida sobre segurança?**
   → Veja [SECURITY_PRODUCTION_GUIDE.md](SECURITY_PRODUCTION_GUIDE.md)

4. **Outros problemas?**
   → Abra uma issue no GitHub

---

## ⏱️ Quanto Tempo Vai Levar?

### Primeira Vez (sem experiência)
- **Leitura**: 1 hora
- **Setup do VPS**: 1 hora
- **Deploy**: 2-3 horas
- **Segurança**: 2-3 horas
- **Total**: **6-9 horas**

### Com Alguma Experiência
- **Setup + Deploy**: 2-3 horas
- **Segurança**: 1 hora
- **Total**: **3-4 horas**

### Profissional Experiente
- **Setup + Deploy + Segurança**: 1.5-2 horas

---

## 🎉 Pronto para Começar?

### Escolha seu caminho:

**📖 Quero ler tudo antes**
→ Comece com [DEPLOYMENT_SUMMARY.md](DEPLOYMENT_SUMMARY.md)

**🚀 Quero começar AGORA**
→ Vá direto para [PRODUCAO_HOSTINGER_GUIDE.md](PRODUCAO_HOSTINGER_GUIDE.md)

**🌐 Só preciso configurar subdomínios**
→ Veja [HOSTINGER_SUBDOMAINS_GUIDE.md](HOSTINGER_SUBDOMAINS_GUIDE.md)

**🔒 Só preciso de segurança**
→ Veja [SECURITY_PRODUCTION_GUIDE.md](SECURITY_PRODUCTION_GUIDE.md)

**🐳 Quero usar Docker**
→ Use [docker-compose.production-no-portal.yml](docker-compose.production-no-portal.yml)

---

## ✅ Checklist Rápido

Antes de começar, certifique-se de ter:

- [ ] Conta na Hostinger
- [ ] Plano KVM 2 contratado (ou prestes a contratar)
- [ ] Domínio registrado (ou vai registrar)
- [ ] 6-9 horas disponíveis
- [ ] Chave API do Daily.co para telemedicina
- [ ] Conhecimento básico de terminal/linha de comando

**Falta alguma coisa?**

- **Hostinger**: https://www.hostinger.com.br/vps
- **Domínio**: Registro.br ou Hostinger
- **Daily.co**: https://www.daily.co/

---

## 🔐 Lembrete de Segurança

**⚠️ IMPORTANTE: Seu sistema vai lidar com dados MÉDICOS!**

Isso significa que você **DEVE**:
- ✅ Usar HTTPS (SSL) em TUDO
- ✅ Configurar firewall
- ✅ Usar senhas fortes (20+ caracteres)
- ✅ Fazer backups criptografados
- ✅ Seguir a LGPD (Lei 13.709/2018)
- ✅ Implementar logs de auditoria
- ✅ Ter plano de resposta a incidentes

**Não pule a seção de segurança!**

---

## 📞 Suporte

- **Issues**: [GitHub Issues](https://github.com/PrimeCareSoftware/MW.Code/issues)
- **Documentação**: Este repositório
- **Hostinger**: Suporte via ticket/chat no painel

---

**Última atualização**: Fevereiro 2026  
**Versão**: 1.0  
**Criado para**: Deploy em produção sem Portal do Paciente

🚀 **Boa sorte com seu deploy! Você consegue!** 💪
