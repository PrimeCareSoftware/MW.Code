# 🎯 Como Fazer Deploy no Hostinger - Comece Aqui

> **Para**: Usuários iniciantes que querem usar Hostinger ao invés de Railway  
> **Tempo**: 30 minutos a 2 horas (depende do seu nível)  
> **Custo**: R$ 20-40/mês

## 📖 Qual Guia Devo Usar?

```
┌─────────────────────────────────────────────────────┐
│  Você já usou terminal/linha de comando antes?      │
└────────────────┬────────────────────────────────────┘
                 │
        ┌────────┴────────┐
        │                 │
      SIM                NÃO
        │                 │
        ▼                 ▼
   Experiente        Iniciante Total
        │                 │
        ▼                 ▼
┌──────────────┐   ┌──────────────┐
│ Início       │   │ Guia         │
│ Rápido       │   │ Completo     │
│ 30 min       │   │ 2 horas      │
└──────────────┘   └──────────────┘
```

---

## 🚀 Opção 1: Sou Iniciante Total

### Use o Guia Completo

**Arquivo**: [DEPLOY_HOSTINGER_GUIA_COMPLETO.md](DEPLOY_HOSTINGER_GUIA_COMPLETO.md)

**Características**:
- ✅ 500+ linhas com explicações detalhadas
- ✅ Conceitos técnicos explicados (VPS, SSH, API, etc)
- ✅ Cada comando é explicado
- ✅ 11 seções passo a passo
- ✅ Screenshots e exemplos visuais
- ✅ Troubleshooting extenso

**Tempo**: 2-3 horas

**Como usar**:
1. Abra o arquivo no GitHub ou em seu editor
2. Leia a seção "Entendendo o que é VPS"
3. Siga cada seção em ordem
4. Copie e cole os comandos um por um
5. Verifique cada resultado antes de continuar
6. Use o checklist final para confirmar

**Comece aqui**: 👉 [DEPLOY_HOSTINGER_GUIA_COMPLETO.md](DEPLOY_HOSTINGER_GUIA_COMPLETO.md)

---

## ⚡ Opção 2: Tenho Experiência Básica

### Use o Guia de Início Rápido

**Arquivo**: [DEPLOY_HOSTINGER_INICIO_RAPIDO.md](DEPLOY_HOSTINGER_INICIO_RAPIDO.md)

**Características**:
- ✅ Deploy em 30 minutos
- ✅ 6 etapas simples
- ✅ Comandos consolidados
- ✅ Checklist de verificação
- ✅ Links para guia completo se precisar

**Tempo**: 30-60 minutos

**Como usar**:
1. Tenha seu VPS Hostinger já contratado
2. Copie blocos inteiros de comandos
3. Execute sequencialmente
4. Verifique os checkboxes
5. Se tiver dúvida, consulte o guia completo

**Comece aqui**: 👉 [DEPLOY_HOSTINGER_INICIO_RAPIDO.md](DEPLOY_HOSTINGER_INICIO_RAPIDO.md)

---

## 📚 Documentos de Apoio

### [RESUMO_GUIAS_HOSTINGER.md](RESUMO_GUIAS_HOSTINGER.md)

Resume o que foi criado e como usar os guias.

**Quando usar**:
- Quer entender a estrutura dos guias
- Quer ver custos estimados
- Precisa de recursos adicionais
- Quer saber próximos passos

---

## 🗺️ Passo a Passo Resumido

Independente do guia escolhido, você vai:

```
1️⃣ Contratar VPS Hostinger
    ↓
2️⃣ Acessar via SSH
    ↓
3️⃣ Instalar componentes (.NET, PostgreSQL, etc)
    ↓
4️⃣ Configurar banco de dados
    ↓
5️⃣ Fazer deploy do backend
    ↓
6️⃣ Fazer deploy do frontend
    ↓
7️⃣ Configurar domínio e SSL (opcional)
    ↓
8️⃣ Testar tudo!
    ↓
9️⃣ Sistema no ar! 🎉
```

---

## 💰 Quanto Vai Custar?

| Item | Custo |
|------|-------|
| VPS Hostinger KVM 1 (4GB) | R$ 19,99 - R$ 39,99/mês |
| Domínio .com.br | R$ 40/ano (~R$ 3,33/mês) |
| SSL Let's Encrypt | Grátis |
| **Total** | **R$ 23 - R$ 43/mês** |

**Capacidade**: 10-30 clínicas pequenas

---

## 🎓 O que Preciso Saber Antes?

### Iniciante Total

**Não precisa saber nada!** O guia completo explica tudo.

Mas ajuda se você souber:
- Como usar o terminal/linha de comando básico
- O que é um servidor
- Noções básicas de internet (domínio, IP, etc)

**Se não souber**: Ainda assim consegue! Leia com calma e siga os passos.

### Experiente

Precisa saber:
- SSH básico
- Comandos Linux básicos (cd, ls, nano)
- Como copiar/colar no terminal

---

## 🛠️ O que Vou Precisar?

### Antes de Começar

- [ ] Conta no Hostinger (criar em: https://hostinger.com.br)
- [ ] Cartão de crédito para pagamento do VPS
- [ ] Computador com acesso à internet
- [ ] Terminal/PowerShell/CMD
- [ ] 30 minutos - 2 horas de tempo disponível

### Opcional (mas recomendado)

- [ ] Domínio próprio (.com, .com.br, etc)
- [ ] Email profissional
- [ ] Notepad/editor de texto para anotar senhas

---

## 📝 Checklist Rápido

Use este checklist para acompanhar seu progresso:

```
□ VPS Hostinger contratado
□ Consegui acessar via SSH
□ Firewall configurado
□ .NET 8 instalado
□ PostgreSQL instalado
□ Node.js instalado
□ Nginx instalado
□ Banco de dados criado
□ Backend compilado
□ Backend rodando (systemd)
□ Frontend buildado
□ Nginx configurado
□ Site abrindo no navegador
□ API respondendo (testar /swagger)
□ Domínio configurado (opcional)
□ SSL instalado (opcional)
□ Backups configurados
```

---

## 🆘 Preciso de Ajuda!

### Durante o Deploy

1. **Leia a seção de Troubleshooting** do guia que está usando
2. **Veja os logs** com os comandos fornecidos
3. **Consulte o guia completo** se estiver no rápido
4. **Verifique se não pulou nenhum passo**

### Problemas Comuns

| Problema | Onde Encontrar Solução |
|----------|------------------------|
| Não consigo conectar via SSH | Guia Completo - Seção 3 |
| API não inicia | Guia Completo - Seção 11 (Troubleshooting) |
| Site não abre | Guia Completo - Seção 11 (Troubleshooting) |
| Banco não conecta | Guia Completo - Seção 5 + Seção 11 |
| SSL não funciona | Guia Completo - Seção 8 |

### Ainda com Dúvidas?

Abra uma issue no GitHub com:
- Qual guia está seguindo
- Em qual seção/passo parou
- Erro exato que está recebendo
- Print dos logs (se possível)

---

## 🎯 Próximos Passos Após Deploy

Depois do sistema no ar:

1. **Testar funcionalidades**
   - Login
   - Cadastro de pacientes
   - Agendamentos
   - Etc.

2. **Configurar dados iniciais**
   - Seeders
   - Primeiro usuário admin
   - Configurações da clínica

3. **Documentar sua configuração**
   - Anotar senhas (em local seguro!)
   - Anotar IPs e domínios
   - Processo de atualização

4. **Configurar monitoramento**
   - Uptime Robot (gratuito)
   - Alertas de email
   - Backups automáticos

5. **Treinar usuários**
   - Como usar o sistema
   - Processos da clínica
   - Suporte

---

## 🔗 Documentação Relacionada

Outros guias úteis:

- **[INFRA_DOCS_INDEX.md](INFRA_DOCS_INDEX.md)** - Índice completo de infraestrutura
- **[DEPLOY_RAILWAY_GUIDE.md](DEPLOY_RAILWAY_GUIDE.md)** - Alternativa mais simples
- **[CALCULADORA_CUSTOS.md](CALCULADORA_CUSTOS.md)** - Calcular custos
- **[AUTHENTICATION_GUIDE.md](AUTHENTICATION_GUIDE.md)** - Como usar autenticação
- **[SEED_API_GUIDE.md](SEED_API_GUIDE.md)** - Dados iniciais de teste

---

## 💡 Dicas Importantes

### Antes de Começar

- ✅ **Escolha um momento tranquilo** - Vai precisar de foco
- ✅ **Leia o guia completo antes** - Pelo menos uma passada rápida
- ✅ **Anote senhas geradas** - Você vai precisar delas
- ✅ **Faça um passo de cada vez** - Não pule etapas

### Durante o Deploy

- ✅ **Teste cada etapa** - Antes de passar para próxima
- ✅ **Veja os logs** - Se algo não funcionar
- ✅ **Não use ctrl+C** - A menos que tenha certeza
- ✅ **Copie os comandos com atenção** - Caracteres especiais importam

### Após o Deploy

- ✅ **Faça backup imediatamente** - Antes de mexer em qualquer coisa
- ✅ **Teste todas as funcionalidades** - Garanta que está funcionando
- ✅ **Monitore os recursos** - CPU, memória, disco
- ✅ **Documente mudanças** - Tudo que você fizer depois

---

## 🎉 Conclusão

Você tem agora **documentação completa** para fazer deploy no Hostinger!

**Escolha seu guia**:
- 🐢 **Iniciante**: [DEPLOY_HOSTINGER_GUIA_COMPLETO.md](DEPLOY_HOSTINGER_GUIA_COMPLETO.md)
- 🐇 **Experiente**: [DEPLOY_HOSTINGER_INICIO_RAPIDO.md](DEPLOY_HOSTINGER_INICIO_RAPIDO.md)

**Boa sorte!** 🚀

Se conseguir fazer o deploy, compartilhe sua experiência! Isso ajuda outros iniciantes.

---

## 📞 Suporte

- **Issues**: [GitHub Issues](https://github.com/PrimeCareSoftware/MW.Code/issues)
- **Documentação**: Este repositório
- **Hostinger**: Suporte via ticket/chat

---

**Criado em**: Janeiro 2025  
**Para**: Usuários que preferem Hostinger  
**Mantenedor**: PrimeCare Software Team
