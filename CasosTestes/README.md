# 📋 Casos de Testes - PrimeCare Software

> **Última Atualização:** Fevereiro 2026  
> **Objetivo:** Documentação completa para configuração e execução de testes do sistema

Este diretório contém toda a documentação necessária para:
1. **Configurar o sistema** do zero até estar totalmente funcional
2. **Executar cenários de testes** para garantir a qualidade do software

## 📁 Estrutura de Pastas

```
CasosTestes/
├── README.md (este arquivo)
├── Configuracao/
│   ├── 01-Configuracao-Ambiente.md
│   ├── 02-Configuracao-Backend.md
│   ├── 03-Configuracao-Frontend.md
│   ├── 04-Configuracao-Banco-Dados.md
│   ├── 05-Configuracao-Docker-Podman.md
│   ├── 06-Configuracao-Clinica.md ⭐ NOVO
│   ├── 07-Configuracao-Financeiro.md ⭐ NOVO
│   └── 08-Configuracao-Fiscal.md ⭐ NOVO
└── CenariosTestesQA/
    ├── 01-Testes-Autenticacao.md
    ├── 02-Testes-Agendamento.md
    ├── 03-Testes-Prontuario.md
    ├── 04-Testes-LGPD.md
    ├── 05-Testes-Portal-Paciente.md
    ├── 06-Testes-CRM.md
    ├── 07-Testes-Analytics.md
    ├── 08-Testes-Acessibilidade.md
    └── 09-Cenario-Completo-Setup-Clinica.md ⭐ NOVO
```

## 🚀 Início Rápido

### Para Configurar o Sistema pela Primeira Vez

#### Configuração Técnica do Sistema

Siga os documentos de configuração na ordem:

1. **[Configuração do Ambiente](Configuracao/01-Configuracao-Ambiente.md)** - Instale todas as ferramentas necessárias
2. **[Configuração do Backend](Configuracao/02-Configuracao-Backend.md)** - Configure a API .NET 8
3. **[Configuração do Frontend](Configuracao/03-Configuracao-Frontend.md)** - Configure a aplicação Angular 20
4. **[Configuração do Banco de Dados](Configuracao/04-Configuracao-Banco-Dados.md)** - Configure o PostgreSQL
5. **[Configuração Docker/Podman](Configuracao/05-Configuracao-Docker-Podman.md)** - Configure containers (opcional)

**Tempo estimado:** 30-45 minutos

#### Configuração da Clínica e Operação

Após o sistema estar rodando, configure a clínica:

6. **[Configuração da Clínica](Configuracao/06-Configuracao-Clinica.md)** ⭐ - Registre e configure sua clínica do zero
   - Registro e primeiro acesso
   - Configuração de negócio
   - Personalização visual
   - Gerenciamento de usuários

7. **[Configuração do Módulo Financeiro](Configuracao/07-Configuracao-Financeiro.md)** ⭐ - Configure o sistema financeiro completo
   - Formas de pagamento
   - Categorias de despesas
   - Contas bancárias
   - Fornecedores
   - Permissões e regras

8. **[Configuração Fiscal e Notas Fiscais](Configuracao/08-Configuracao-Fiscal.md)** ⭐ - Configure impostos e emissão de notas
   - Regime tributário
   - Impostos (ISS, PIS, COFINS, IR, CSLL)
   - Notas fiscais (Invoice interno + NF-e/NFS-e)
   - Plano de contas contábil
   - Apuração mensal

**Tempo estimado:** 80-120 minutos

### Cenário Completo de Teste

Para testar todo o fluxo configurado:

🎯 **[Cenário Completo: Setup da Clínica do Zero à Primeira Consulta](CenariosTestesQA/09-Cenario-Completo-Setup-Clinica.md)** ⭐
- Registro → Configuração → Primeira Consulta → Fechamento Financeiro → Apuração Fiscal
- Inclui todos os passos práticos com dados de exemplo
- Troubleshooting de problemas comuns

**Tempo estimado:** 60-90 minutos

### Para Executar Testes de Qualidade (QA)

Os cenários de testes estão organizados por módulo do sistema:

- **[Testes de Autenticação](CenariosTestesQA/01-Testes-Autenticacao.md)** - Login, 2FA, recuperação de senha
- **[Testes de Agendamento](CenariosTestesQA/02-Testes-Agendamento.md)** - Criação e gestão de consultas
- **[Testes de Prontuário](CenariosTestesQA/03-Testes-Prontuario.md)** - SOAP, prescrições, documentos
- **[Testes LGPD](CenariosTestesQA/04-Testes-LGPD.md)** - Conformidade e privacidade
- **[Testes Portal do Paciente](CenariosTestesQA/05-Testes-Portal-Paciente.md)** - Área do paciente
- **[Testes CRM](CenariosTestesQA/06-Testes-CRM.md)** - Gestão de relacionamento
- **[Testes Analytics](CenariosTestesQA/07-Testes-Analytics.md)** - Dashboards e relatórios
- **[Testes de Acessibilidade](CenariosTestesQA/08-Testes-Acessibilidade.md)** - WCAG 2.1 AA
- **[Cenário Completo de Setup](CenariosTestesQA/09-Cenario-Completo-Setup-Clinica.md)** ⭐ - Teste end-to-end completo

## 🎯 Público-Alvo

Esta documentação é destinada para:

- ✅ **Equipe de QA** - Para executar testes manuais e automatizados
- ✅ **Desenvolvedores** - Para configurar ambiente de desenvolvimento
- ✅ **DevOps** - Para configurar ambientes de staging e produção
- ✅ **Novos membros da equipe** - Para onboarding rápido
- ✅ **Proprietários de Clínicas** - Para configurar e operar suas clínicas
- ✅ **Equipe de Suporte** - Para auxiliar clientes na configuração inicial

## 📊 Status do Sistema

O PrimeCare Software está com **95% de completude**:

- ✅ Backend .NET 8 (50+ controllers)
- ✅ Frontend Angular 20 (171+ componentes)
- ✅ PostgreSQL com migrations
- ✅ 792+ testes automatizados
- ✅ PWA multiplataforma
- ✅ Conformidade LGPD
- ✅ Acessibilidade WCAG 2.1 AA

## 🔗 Links Úteis

### Documentação Principal
- [README Principal](../README.md)
- [Mapa de Documentação](../DOCUMENTATION_MAP.md)
- [Guia de Contribuição](../CONTRIBUTING.md)

### Guias Técnicos
- [Guia Multiplataforma](../system-admin/guias/GUIA_MULTIPLATAFORMA.md)
- [Guia de Início Rápido](../system-admin/guias/GUIA_INICIO_RAPIDO_LOCAL.md)
- [Migrations Guide](../MIGRATIONS_GUIDE.md)

### Guias para Proprietários
- [Guia de Primeiro Acesso](../system-admin/guias/OWNER_FIRST_LOGIN_GUIDE.md)
- [Guia de Administração da Clínica](../system-admin/guias/CLINIC_ADMIN_GUIDE.md)
- [Guia de Relatórios Financeiros](../system-admin/guias/GUIA_USUARIO_RELATORIOS_FINANCEIROS.md)
- [Guia NF-e/NFS-e](../system-admin/guias/NFE_NFSE_USER_GUIDE.md)

### Documentação Técnica
- [Módulo Financeiro](../system-admin/docs/MODULO_FINANCEIRO.md)
- [Gestão Fiscal](../GESTAO_FISCAL_IMPLEMENTACAO.md)
- [Testes do Módulo Financeiro](../system-admin/docs/testes-configuracao/03-MODULO-FINANCEIRO.md)

### Testes Existentes
- [Guia de Testes BI Analytics](../TESTING_GUIDE_BI_ANALYTICS.md)
- [Guia de Testes TISS](../tests/TISS_TUSS_TESTING_GUIDE.md)
- [Guia de Testes de Acessibilidade](../ACCESSIBILITY_TESTING_GUIDE.md)

## 💡 Dicas Importantes

### Antes de Começar os Testes

1. ✅ Certifique-se que o sistema está completamente configurado
2. ✅ Verifique que todos os serviços estão rodando (backend, frontend, banco)
3. ✅ Limpe o cache do navegador antes de testar
4. ✅ Use dados de teste (não use dados reais de pacientes)
5. ✅ Documente qualquer bug encontrado com screenshots

### Ambiente de Testes Recomendado

- **Sistema Operacional:** Windows 10+, macOS 10.15+, ou Linux (Ubuntu 20.04+)
- **Navegadores:** Chrome (recomendado), Firefox, Safari, Edge
- **Resolução:** Teste em múltiplas resoluções (desktop, tablet, mobile)
- **Ferramentas:** Postman para testes de API, DevTools do navegador

## 🐛 Reportando Bugs

Ao encontrar um bug durante os testes:

1. Documente o comportamento esperado vs. o comportamento atual
2. Capture screenshots ou grave a tela
3. Anote os passos para reproduzir
4. Verifique o console do navegador para erros
5. Abra uma issue no GitHub com todos os detalhes

## 📞 Suporte

Se tiver dúvidas ou precisar de ajuda:

- 📖 Consulte a documentação em `/docs` e `/system-admin/docs`
- 🐛 Abra uma issue no GitHub
- 💬 Entre em contato com a equipe de desenvolvimento

---

**Versão do Sistema:** 1.0  
**Última Revisão:** Fevereiro 2026  
**Mantido por:** Equipe PrimeCare Software
