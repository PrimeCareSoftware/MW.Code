# 📚 Manual do Usuário - Sistema CRM Avançado

**Versão:** 2.0  
**Data:** 27 de Janeiro de 2026  
**Sistema:** MedicSoft CRM - Customer Relationship Management

---

## 📋 Índice

1. [Introdução](#introdução)
2. [Jornada do Paciente](#jornada-do-paciente)
3. [Automações de Marketing](#automações-de-marketing)
4. [Pesquisas NPS/CSAT](#pesquisas-npscsat)
5. [Ouvidoria](#ouvidoria)
6. [Análise de Sentimento](#análise-de-sentimento)
7. [Predição de Churn](#predição-de-churn)
8. [APIs Disponíveis](#apis-disponíveis)
9. [Background Jobs](#background-jobs)
10. [Melhores Práticas](#melhores-práticas)

---

## 🎯 Introdução

O Sistema CRM Avançado do MedicSoft é uma plataforma completa para gerenciamento do relacionamento com pacientes, oferecendo:

- **Mapeamento da Jornada do Paciente**: Rastreamento de todas as interações
- **Automação de Marketing**: Campanhas personalizadas e automatizadas
- **Pesquisas de Satisfação**: NPS e CSAT automáticos
- **Ouvidoria**: Gestão de reclamações e SLA
- **Análise de Sentimento**: IA para identificar insatisfação
- **Predição de Churn**: Identificação de pacientes em risco

### Status de Implementação

✅ **Backend Completo**: Todos os serviços, APIs e jobs implementados  
✅ **Background Jobs**: 4 jobs Hangfire rodando automaticamente  
✅ **Testes**: 23 testes unitários criados  
🔄 **Frontend**: Pendente de implementação  

Para detalhes técnicos, consulte: [CRM_IMPLEMENTATION_STATUS.md](/CRM_IMPLEMENTATION_STATUS.md)

---

## 🗺️ Jornada do Paciente

### O que é?

A jornada do paciente mapeia todos os estágios pelos quais um paciente passa, desde o primeiro contato até se tornar um promotor da marca.

### Estágios da Jornada

1. **Descoberta**: Primeiro contato com a clínica
2. **Consideração**: Avaliando opções de tratamento
3. **Primeira Consulta**: Primeiro atendimento médico
4. **Tratamento**: Durante procedimentos/tratamentos
5. **Retorno**: Consultas de acompanhamento
6. **Fidelização**: Cliente recorrente
7. **Advocacia**: Promotor ativo da marca

### Como Usar

#### Buscar Jornada do Paciente
```http
GET /api/crm/journey/{patientId}
```

#### Avançar para Próximo Estágio
```http
POST /api/crm/journey/{patientId}/advance
Content-Type: application/json

{
  "newStage": "PrimeiraConsulta",
  "trigger": "Agendamento de consulta realizado"
}
```

#### Registrar Interação (Touchpoint)
```http
POST /api/crm/journey/{patientId}/touchpoint
Content-Type: application/json

{
  "type": "EmailInteraction",
  "channel": "Email",
  "description": "Email de confirmação enviado",
  "direction": "Outbound"
}
```

Para mais detalhes, veja: [CRM_API_DOCUMENTATION.md](/CRM_API_DOCUMENTATION.md)

---

## 🤖 Automações de Marketing

Sistema para criar campanhas automatizadas baseadas em eventos ou estágios da jornada.

### Criar Automação

```http
POST /api/crm/automation
Content-Type: application/json

{
  "name": "Welcome Email - Novos Pacientes",
  "description": "Email de boas-vindas automático",
  "triggerType": "JourneyStageChanged",
  "triggerStage": "PrimeiraConsulta",
  "actions": [
    {
      "type": "SendEmail",
      "emailTemplateId": "...",
      "delayMinutes": 60
    }
  ]
}
```

### Tipos de Ação

- SendEmail, SendSMS, SendWhatsApp
- AddTag, RemoveTag
- ChangeScore

### Variáveis de Personalização

- `{{patientName}}`, `{{clinicName}}`, `{{doctorName}}`
- `{{appointmentDate}}`, `{{appointmentTime}}`

---

## 📊 Pesquisas NPS/CSAT

### Tipos de Pesquisa

**NPS (Net Promoter Score)**
- Escala 0-10
- Fórmula: (% Promotores - % Detratores)
- Enviado 2 dias após consulta

**CSAT (Customer Satisfaction)**
- Escala 1-5 estrelas
- Para avaliar serviços específicos

### Criar e Enviar

```http
POST /api/crm/survey
POST /api/crm/survey/{id}/activate
POST /api/crm/survey/{id}/send/{patientId}
```

### Analytics

```http
GET /api/crm/survey/{id}/analytics
```

Retorna NPS score, distribuição de respostas, e comentários.

---

## 📞 Ouvidoria

Sistema de gestão de reclamações com protocolo e SLA.

### Criar Reclamação

```http
POST /api/crm/complaint
Content-Type: application/json

{
  "patientId": "...",
  "subject": "Demora no atendimento",
  "description": "Detalhes...",
  "category": "Atendimento",
  "priority": "Medium"
}
```

### Protocolo

Formato: `CMP-2026-000123`

Buscar por protocolo:
```http
GET /api/crm/complaint/protocol/{protocolNumber}
```

### Dashboard SLA

```http
GET /api/crm/complaint/dashboard
```

Métricas:
- Total de reclamações
- SLA médio de resposta
- SLA médio de resolução
- Distribuição por categoria

---

## 🧠 Análise de Sentimento

Análise automática de comentários para identificar insatisfação.

### Como Funciona

1. **Análise Automática**: Jobs em background analisam comentários
2. **Classificação**: Positivo, Neutro ou Negativo
3. **Alertas**: Sentimentos negativos geram alertas
4. **Tópicos**: Extração de tópicos (Atendimento, Médico, etc.)

### Fontes Analisadas

- Comentários de pesquisas
- Descrições de reclamações
- Interações da ouvidoria

### Background Job

**SentimentAnalysisJob** roda a cada hora analisando:
- Comentários não processados
- Gerando alertas para negativos
- Calculando tendências

---

## 📉 Predição de Churn

Identificação de pacientes em risco de abandono.

### Níveis de Risco

- **Low**: Paciente engajado
- **Medium**: Requer atenção
- **High**: Ação necessária
- **Critical**: Intervenção urgente

### Fatores Analisados

1. Dias desde última visita
2. Taxa de no-show
3. NPS score
4. Número de reclamações
5. Histórico de pagamento
6. Engajamento

### Usar API

```http
GET /api/crm/churn/predict/{patientId}
GET /api/crm/churn/high-risk
```

### Background Job

**ChurnPredictionJob** roda semanalmente:
- Predição em lote para todos pacientes
- Notificação de alto risco
- Análise de efetividade de retenção

---

## 🔌 APIs Disponíveis

### Resumo de Endpoints

| Módulo | Endpoints |
|--------|-----------|
| Patient Journey | 6 endpoints |
| Marketing Automation | 10 endpoints |
| Survey | 12 endpoints |
| Complaint | 13 endpoints |

**Total**: 41 endpoints REST

Documentação completa: [CRM_API_DOCUMENTATION.md](/CRM_API_DOCUMENTATION.md)

---

## ⚙️ Background Jobs

### Jobs Ativos (Hangfire)

| Job | Frequência | Descrição |
|-----|------------|-----------|
| AutomationExecutorJob | A cada hora | Executa automações ativas |
| SurveyTriggerJob | Diário às 10:00 UTC | Dispara pesquisas |
| ChurnPredictionJob | Semanal Domingos 03:00 | Predição de churn |
| SentimentAnalysisJob | A cada hora | Análise de sentimento |

### Dashboard

Acesse: `/hangfire`

Funcionalidades:
- Ver jobs em execução
- Histórico de execuções
- Disparar jobs manualmente
- Monitorar falhas

---

## 💡 Melhores Práticas

### Jornada do Paciente

✅ Registre todas interações  
✅ Atualize estágios em tempo real  
✅ Use triggers consistentes  

### Automações

✅ Teste antes de ativar  
✅ Use variáveis para personalização  
✅ Monitore métricas regularmente  

### Pesquisas

✅ Envie no momento certo (2 dias pós-consulta para NPS)  
✅ Mantenha pesquisas curtas (3-5 questões)  
✅ Aja rapidamente em feedback negativo  

### Ouvidoria

✅ Meta de primeira resposta: 24h  
✅ Documente todas interações  
✅ Confirme resolução com paciente  

### Churn

✅ Ação preventiva em risco médio  
✅ Personalize abordagem por fator de risco  
✅ Meça efetividade das ações  

---

## 📞 Suporte e Recursos

**Documentação Adicional:**
- [CRM_IMPLEMENTATION_STATUS.md](/CRM_IMPLEMENTATION_STATUS.md) - Status técnico
- [CRM_API_DOCUMENTATION.md](/CRM_API_DOCUMENTATION.md) - Referência de API
- [CRM_IMPLEMENTATION_GUIDE.md](/CRM_IMPLEMENTATION_GUIDE.md) - Guia de implementação

**Recursos do Sistema:**
- Swagger UI: `/swagger`
- Hangfire Dashboard: `/hangfire`
- Health Check: `/health`

---

**Versão 2.0 - Atualizado em 27/01/2026**  
**© 2026 MedicSoft - Todos os direitos reservados**
