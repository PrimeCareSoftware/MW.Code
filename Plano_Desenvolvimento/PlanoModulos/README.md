# 🎯 Plano de Desenvolvimento - Sistema de Configuração de Módulos

> **Data de Criação:** 29 de Janeiro de 2026  
> **Versão:** 1.0  
> **Objetivo:** Criar tela de configuração para habilitar/desabilitar módulos do sistema

---

## 📋 Sumário Executivo

### Contexto Atual

O sistema PrimeCare já possui uma **base sólida** para gerenciamento de módulos:

**Estrutura Existente:**
- ✅ Entidade `ModuleConfiguration` (banco de dados)
- ✅ Controller `ModuleConfigController` (API REST)
- ✅ Classe `SystemModules` com 13 módulos definidos
- ✅ Integração com planos de assinatura
- ✅ Vinculação módulo ↔ clínica

**Módulos Atualmente Disponíveis:**
1. `PatientManagement` - Gestão de Pacientes
2. `AppointmentScheduling` - Agendamento de Consultas
3. `MedicalRecords` - Prontuários Médicos
4. `Prescriptions` - Prescrições
5. `FinancialManagement` - Gestão Financeira
6. `Reports` - Relatórios
7. `WhatsAppIntegration` - Integração WhatsApp
8. `SMSNotifications` - Notificações SMS
9. `TissExport` - Exportação TISS
10. `InventoryManagement` - Gestão de Estoque
11. `UserManagement` - Gestão de Usuários
12. `WaitingQueue` - Fila de Espera
13. `DoctorFieldsConfig` - Configuração de Campos do Médico

**Tecnologias:**
- Backend: ASP.NET Core (C#)
- Frontend: Angular 20 (standalone components)
- System Admin: Angular 20 separado (`mw-system-admin`)
- Clínica Frontend: Angular 20 (`medicwarehouse-app`)

---

## 🎯 Objetivo da Funcionalidade

### Necessidade de Negócio

Criar uma **tela de configuração centralizada** onde seja possível:

1. **Configuração Global (System Admin)**
   - Habilitar/desabilitar módulos para todas as clínicas
   - Definir quais módulos estão disponíveis por plano de assinatura
   - Visualizar uso e status dos módulos no sistema

2. **Configuração por Clínica (Cadastro da Clínica)**
   - Habilitar/desabilitar módulos específicos para uma clínica
   - Respeitar restrições do plano contratado
   - Permitir ajustes finos de configuração por módulo

3. **Vinculação com Planos do Site**
   - Associar cada plano de assinatura com módulos específicos
   - Controlar features premium vs. básicas
   - Facilitar upgrade/downgrade de planos

### Benefícios Esperados

- 🎯 **Gestão Simplificada:** Interface visual para controle de módulos
- 💰 **Monetização:** Diferenciar planos por funcionalidades
- 🔧 **Flexibilidade:** Ajustes personalizados por clínica
- 📊 **Visibilidade:** Métricas de uso de cada módulo
- 🚀 **Escalabilidade:** Fácil adição de novos módulos

---

## 📁 Estrutura dos Prompts

Esta pasta contém **prompts detalhados** para implementação da funcionalidade:

```
PlanoModulos/
├── README.md (este arquivo)
├── 01-PROMPT-BACKEND.md (Backend/API)
├── 02-PROMPT-FRONTEND-SYSTEM-ADMIN.md (Frontend System Admin)
├── 03-PROMPT-FRONTEND-CLINIC.md (Frontend Clínica)
├── 04-PROMPT-TESTES.md (Testes Automatizados)
└── 05-PROMPT-DOCUMENTACAO.md (Documentação Técnica e Usuário)
```

---

## 🚀 Fases de Implementação

### **Fase 1: Backend e API (2-3 semanas)**
**Arquivo:** `01-PROMPT-BACKEND.md`

**Tarefas:**
- [ ] Expandir endpoints existentes da API
- [ ] Criar endpoints para configuração global
- [ ] Adicionar vinculação módulos ↔ planos
- [ ] Implementar serviços de validação
- [ ] Criar DTOs e ViewModels
- [ ] Adicionar logs de auditoria

**Entregas:**
- API REST completa para CRUD de configurações
- Endpoints para system-admin e clínica
- Validações de negócio implementadas

---

### **Fase 2: Frontend System Admin (2-3 semanas)**
**Arquivo:** `02-PROMPT-FRONTEND-SYSTEM-ADMIN.md`

**Status:** ✅ **CONCLUÍDA** (29 de Janeiro de 2026)

**Tarefas:**
- [x] Criar página de configuração global de módulos
- [x] Implementar interface de vinculação módulos ↔ planos
- [x] Criar dashboard de métricas de uso
- [x] Adicionar filtros e buscas
- [x] Implementar ações em lote

**Entregas:**
- ✅ Tela de gestão global de módulos (`modules-dashboard`)
- ✅ Interface de configuração de planos (`plan-modules`)
- ✅ Dashboard com métricas de uso e KPIs
- ✅ Página de detalhes de módulo (`module-details`)
- ✅ Integração completa com API backend
- ✅ Navegação e rotas configuradas

---

### **Fase 3: Frontend Clínica (2-3 semanas)**
**Arquivo:** `03-PROMPT-FRONTEND-CLINIC.md`

**Status:** ✅ **CONCLUÍDA** (29 de Janeiro de 2026)

**Tarefas:**
- [x] Criar aba "Módulos" no cadastro da clínica
- [x] Implementar toggle habilitar/desabilitar
- [x] Adicionar validação de plano
- [x] Criar interface de configurações avançadas
- [x] Implementar feedback visual de status

**Entregas:**
- ✅ Modelo de dados `module-config.model.ts`
- ✅ Serviço de integração `module-config.service.ts`
- ✅ Componente principal `clinic-modules.component.ts`
- ✅ Template HTML responsivo
- ✅ Estilos SCSS com suporte mobile
- ✅ Dialog de configuração avançada
- ✅ Integração com rotas e navegação
- ✅ Menu item adicionado à navegação

---

### **Fase 4: Testes Automatizados (1-2 semanas)**
**Arquivo:** `04-PROMPT-TESTES.md`

**Tarefas:**
- [ ] Testes unitários do backend
- [ ] Testes de integração da API
- [ ] Testes E2E do frontend
- [ ] Testes de permissões e segurança
- [ ] Testes de validação de planos

**Entregas:**
- Cobertura de testes > 80%
- Suite de testes automatizados
- Testes de regressão

---

### **Fase 5: Documentação (1 semana)**
**Arquivo:** `05-PROMPT-DOCUMENTACAO.md`

**Tarefas:**
- [ ] Documentação técnica da API
- [ ] Guia do usuário (system-admin)
- [ ] Guia do usuário (clínica)
- [ ] Documentação de arquitetura
- [ ] Vídeos tutoriais

**Entregas:**
- Documentação completa
- Guias de usuário
- Documentação de API

---

## 📊 Estimativas

### Esforço Total

| Fase | Duração | Desenvolvedores | Custo Estimado |
|------|---------|----------------|----------------|
| 1. Backend e API | 2-3 semanas | 1-2 devs | R$ 20.000 - R$ 30.000 |
| 2. Frontend System Admin | 2-3 semanas | 1-2 devs | R$ 20.000 - R$ 30.000 |
| 3. Frontend Clínica | 2-3 semanas | 1-2 devs | R$ 20.000 - R$ 30.000 |
| 4. Testes | 1-2 semanas | 1 dev | R$ 10.000 - R$ 15.000 |
| 5. Documentação | 1 semana | 1 dev | R$ 5.000 - R$ 8.000 |
| **TOTAL** | **8-12 semanas** | **1-2 devs** | **R$ 75.000 - R$ 113.000** |

### Cronograma Sugerido

**Execução Sequencial (1 dev):** 8-12 semanas  
**Execução Paralela (2 devs):** 5-7 semanas

```
Semana 1-3:   Backend e API
Semana 4-6:   Frontend System Admin  
Semana 7-9:   Frontend Clínica
Semana 10-11: Testes
Semana 12:    Documentação
```

**Execução Paralela:**
```
Semana 1-3:   Backend (Dev 1) + Frontend System Admin (Dev 2)
Semana 4-5:   Frontend Clínica (Dev 1) + Testes Backend (Dev 2)
Semana 6-7:   Testes Frontend (Dev 1) + Documentação (Dev 2)
```

---

## 🎯 Critérios de Sucesso

### Funcional
- ✅ System-admin consegue habilitar/desabilitar módulos globalmente
- ✅ System-admin consegue vincular módulos aos planos
- ✅ Clínica consegue configurar módulos respeitando o plano
- ✅ Sistema valida permissões antes de permitir mudanças
- ✅ Mudanças são auditadas e logadas

### Técnico
- ✅ API RESTful seguindo padrões do projeto
- ✅ Frontend responsivo e acessível (WCAG 2.1)
- ✅ Cobertura de testes > 80%
- ✅ Performance: carregamento < 2s
- ✅ Documentação completa

### Negócio
- ✅ Interface intuitiva e fácil de usar
- ✅ Redução de 80% no tempo de configuração
- ✅ Diferenciação clara entre planos
- ✅ Facilita upsell/cross-sell

---

## ⚠️ Considerações Importantes

### Segurança
- 🔐 Apenas usuários `SystemAdmin` podem configurar globalmente
- 🔐 Clínicas só podem configurar seus próprios módulos
- 🔐 Validar permissões em frontend e backend
- 🔐 Auditar todas as mudanças de configuração

### Compatibilidade
- ✅ Manter compatibilidade com API existente
- ✅ Não quebrar funcionalidades atuais
- ✅ Migração automática de dados existentes

### Desempenho
- ⚡ Cache de configurações de módulos
- ⚡ Lazy loading de componentes
- ⚡ Paginação para listas grandes

### UX/UI
- 🎨 Seguir design system existente (Angular Material)
- 🎨 Feedback visual claro (loading, success, error)
- 🎨 Responsivo (desktop, tablet, mobile)
- 🎨 Acessibilidade (WCAG 2.1)

---

## 📚 Dependências Técnicas

### Backend
```
- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL
- Azure Key Vault (segredos)
```

### Frontend
```
- Angular 20 (standalone components)
- Angular Material
- RxJS
- TypeScript 5.0+
```

### Infraestrutura
```
- Docker
- Azure App Service
- Azure PostgreSQL
```

---

## 🔗 Documentos Relacionados

### Código Existente
- `/src/MedicSoft.Domain/Entities/ModuleConfiguration.cs`
- `/src/MedicSoft.Domain/Entities/SubscriptionPlan.cs`
- `/src/MedicSoft.Api/Controllers/ModuleConfigController.cs`
- `/frontend/mw-system-admin/` (System Admin Frontend)
- `/frontend/medicwarehouse-app/` (Clínica Frontend)

### Documentação do Sistema
- [SYSTEM_ADMIN_USER_GUIDE.md](../../SYSTEM_ADMIN_USER_GUIDE.md)
- [PLANO_MELHORIAS_SYSTEM_ADMIN_2026.md](../PLANO_MELHORIAS_SYSTEM_ADMIN_2026.md)
- [API_DOCUMENTATION.md](../../docs/API_DOCUMENTATION.md)

### Planos de Desenvolvimento
- [fase-system-admin-melhorias/](../fase-system-admin-melhorias/)
- [PLANO_DESENVOLVIMENTO.md](../../docs/PLANO_DESENVOLVIMENTO.md)

---

## 🎯 Próximos Passos

### Para Iniciar o Desenvolvimento

1. **Revisar os Prompts**
   - Ler todos os arquivos `0X-PROMPT-*.md`
   - Entender escopo de cada fase
   - Identificar dependências entre fases

2. **Preparar Ambiente**
   - Configurar ambiente de desenvolvimento
   - Clonar repositório
   - Instalar dependências

3. **Executar Fase 1 (Backend)**
   - Seguir `01-PROMPT-BACKEND.md`
   - Implementar endpoints da API
   - Testar com Postman/Swagger

4. **Continuar com Fases Seguintes**
   - Executar fases em ordem
   - Validar critérios de sucesso
   - Documentar desvios/mudanças

---

## 📞 Contato

**PrimeCare Software - Equipe de Desenvolvimento**
- GitHub: [PrimeCareSoftware/MW.Code](https://github.com/PrimeCareSoftware/MW.Code)
- Documentação: `/docs`

---

> **Última Atualização:** 29 de Janeiro de 2026  
> **Versão:** 1.0  
> **Status:** 📝 Planejamento Completo
