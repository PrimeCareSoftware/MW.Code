# 06 - Cenários de Testes do CRM

> **Módulo:** CRM (Customer Relationship Management)  
> **Tempo estimado:** 20 minutos

## 🎯 Objetivo

Validar funcionalidades do CRM:
- ✅ Gestão de leads
- ✅ Funil de vendas
- ✅ Campanhas de marketing
- ✅ Automação de follow-up

## 📝 Casos de Teste

### CT-CRM-001: Criar Novo Lead
**Passos:** CRM > Leads > Novo Lead > Preencha dados
**Esperado:** Lead criado, status "Novo"

### CT-CRM-002: Mover Lead no Funil
**Passos:** Arraste lead de "Novo" para "Contactado"
**Esperado:** Status atualizado, log criado

### CT-CRM-003: Converter Lead em Paciente
**Passos:** Ações > Converter para Paciente
**Esperado:** Paciente criado no sistema

### CT-CRM-004: Criar Campanha de Marketing
**Passos:** CRM > Campanhas > Nova Campanha > Configure
**Esperado:** Campanha criada, segmentação ativa

### CT-CRM-005: Enviar Email em Massa
**Passos:** Campanha > Enviar Emails > Selecione lista
**Esperado:** Emails enviados, taxa de abertura rastreada

### CT-CRM-006: Agendar Follow-up Automático
**Passos:** Lead > Agendar Follow-up > Data futura
**Esperado:** Task criada, notificação agendada

### CT-CRM-007: Ver Relatório de Conversão
**Passos:** CRM > Relatórios > Funil de Conversão
**Esperado:** Gráfico com taxas de conversão por etapa

### CT-CRM-008: Registrar Interação com Lead
**Passos:** Lead > Nova Interação > Registre ligação
**Esperado:** Histórico atualizado, timeline preservado

## ✅ Critérios de Aceite
- [ ] Leads podem ser criados
- [ ] Funil de vendas funciona
- [ ] Campanhas podem ser criadas
- [ ] Automação funciona
- [ ] Relatórios corretos

## 📚 Documentação
- [CRM User Guide](../../CRM_USER_GUIDE.md)
- [CRM Implementation Guide](../../CRM_IMPLEMENTATION_GUIDE.md)

## ⏭️ Próximos Passos
➡️ [07-Testes-Analytics.md](07-Testes-Analytics.md)
