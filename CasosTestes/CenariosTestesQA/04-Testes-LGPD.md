# 04 - Cenários de Testes de Conformidade LGPD

> **Módulo:** Privacidade e Conformidade LGPD (Lei 13.709/2018)  
> **Tempo estimado:** 30 minutos  
> **Pré-requisitos:** Sistema configurado

## 🎯 Objetivo dos Testes

Validar conformidade com a LGPD:
- ✅ Consentimento do titular
- ✅ Direito de acesso aos dados
- ✅ Direito de retificação
- ✅ Direito de exclusão (anonimização)
- ✅ Portabilidade de dados
- ✅ Logs de auditoria
- ✅ Relatório de impacto

## 📝 Principais Casos de Teste

### CT-LGPD-001: Solicitar Consentimento do Paciente

**Passos:**
1. Cadastre novo paciente
2. Apresentar termo de consentimento
3. Paciente aceita termo

**Resultado Esperado:**
- ✅ Termo de consentimento exibido
- ✅ Aceite registrado com timestamp
- ✅ Log de auditoria criado

**Prioridade:** 🔴 Crítica

---

### CT-LGPD-002: Paciente Solicita Acesso aos Dados

**Passos:**
1. Login no Portal do Paciente
2. Acesse "Meus Dados"
3. Clique em "Solicitar Cópia dos Dados"

**Resultado Esperado:**
- ✅ Solicitação registrada
- ✅ Email enviado ao DPO
- ✅ Prazo de 15 dias informado
- ✅ Dados exportados em formato legível

**Prioridade:** 🔴 Crítica

---

### CT-LGPD-003: Paciente Solicita Correção de Dados

**Passos:**
1. No Portal do Paciente
2. Acesse "Meus Dados"
3. Clique em "Solicitar Correção"
4. Informe campo e novo valor

**Resultado Esperado:**
- ✅ Solicitação registrada
- ✅ Notificação para análise
- ✅ Histórico de alteração mantido

**Prioridade:** 🟡 Média

---

### CT-LGPD-004: Paciente Solicita Exclusão de Dados

**Passos:**
1. No Portal do Paciente
2. Acesse "Meus Dados"
3. Clique em "Solicitar Exclusão"
4. Confirme solicitação

**Resultado Esperado:**
- ✅ Solicitação registrada
- ✅ Dados anonimizados (não deletados)
- ✅ Identificação pessoal removida
- ✅ Dados médicos mantidos (obrigação legal)

**Prioridade:** 🔴 Crítica

---

### CT-LGPD-005: Exportar Dados do Paciente (Portabilidade)

**Passos:**
1. Como admin, acesse paciente
2. Clique em "Exportar Dados LGPD"
3. Selecione formato: JSON

**Resultado Esperado:**
- ✅ Arquivo JSON gerado
- ✅ Contém todos os dados pessoais
- ✅ Formato legível por máquina
- ✅ Log de exportação criado

**Prioridade:** 🟡 Média

---

### CT-LGPD-006: Visualizar Logs de Auditoria

**Passos:**
1. Como admin, acesse "LGPD" > "Auditoria"
2. Visualize logs

**Resultado Esperado:**
- ✅ Lista de todas as ações LGPD
- ✅ Timestamp, usuário, ação
- ✅ Possível filtrar por paciente
- ✅ Possível exportar logs

**Prioridade:** 🔴 Crítica

---

### CT-LGPD-007: Relatório de Impacto (RIPD)

**Passos:**
1. Acesse "LGPD" > "Relatórios"
2. Clique em "Gerar RIPD"
3. Visualize relatório

**Resultado Esperado:**
- ✅ Relatório com dados agregados
- ✅ Tipos de dados coletados
- ✅ Finalidades de tratamento
- ✅ Medidas de segurança

**Prioridade:** 🟡 Média

---

### CT-LGPD-008: Revogar Consentimento

**Passos:**
1. Paciente no portal
2. Acesse "Meus Consentimentos"
3. Revogue consentimento específico

**Resultado Esperado:**
- ✅ Consentimento revogado
- ✅ Uso dos dados interrompido
- ✅ Notificação ao sistema

**Prioridade:** 🔴 Crítica

---

### CT-LGPD-009: Verificar Criptografia de Dados Sensíveis

**Passos:**
1. Acesse banco de dados
2. Verifique campos sensíveis (CPF, RG)

**Resultado Esperado:**
- ✅ Dados criptografados no banco
- ✅ Visíveis apenas via aplicação
- ✅ Algoritmo AES-256 usado

**Prioridade:** 🔴 Crítica (Segurança)

---

### CT-LGPD-010: Notificação de Incidente de Segurança

**Passos:**
1. Como admin, acesse "LGPD" > "Incidentes"
2. Registre novo incidente
3. Salve

**Resultado Esperado:**
- ✅ Incidente registrado
- ✅ ANPD notificada (se necessário)
- ✅ Titulares afetados identificados
- ✅ Prazo de 72h monitorado

**Prioridade:** 🔴 Crítica

---

## ✅ Critérios de Aceite

- [ ] Consentimento pode ser dado e revogado
- [ ] Paciente pode acessar seus dados
- [ ] Paciente pode solicitar correções
- [ ] Dados podem ser anonimizados
- [ ] Portabilidade funciona
- [ ] Logs de auditoria completos
- [ ] RIPD pode ser gerado
- [ ] Dados sensíveis criptografados

## 📚 Documentação Relacionada

- [LGPD Compliance Guide](../../LGPD_COMPLIANCE_GUIDE.md)
- [LGPD Admin Guide](../../LGPD_ADMIN_GUIDE.md)
- [User Guide LGPD](../../USER_GUIDE_LGPD.md)

## ⏭️ Próximos Passos

➡️ Vá para [05-Testes-Portal-Paciente.md](05-Testes-Portal-Paciente.md)
