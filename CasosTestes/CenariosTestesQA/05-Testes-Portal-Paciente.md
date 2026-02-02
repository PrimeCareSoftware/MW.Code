# 05 - Cenários de Testes do Portal do Paciente

> **Módulo:** Portal do Paciente (Self-Service)  
> **Tempo estimado:** 25 minutos

## 🎯 Objetivo

Validar funcionalidades do portal do paciente:
- ✅ Cadastro e login
- ✅ Visualizar histórico médico
- ✅ Agendar consultas online
- ✅ Ver resultados de exames
- ✅ Comunicação com clínica

## 📝 Casos de Teste

### CT-PORTAL-001: Cadastro de Novo Paciente
**Passos:** Acesse portal > Cadastre-se > Preencha dados > Confirme email
**Esperado:** Conta criada, email de confirmação enviado

### CT-PORTAL-002: Login no Portal
**Passos:** Login com credenciais criadas
**Esperado:** Acesso ao dashboard do paciente

### CT-PORTAL-003: Visualizar Histórico Médico
**Passos:** Acesse "Meu Histórico"
**Esperado:** Lista de consultas, prontuários (autorizados), prescrições

### CT-PORTAL-004: Agendar Consulta Online
**Passos:** "Agendar Consulta" > Escolha médico > Escolha horário disponível
**Esperado:** Agendamento criado, confirmação enviada

### CT-PORTAL-005: Ver Resultados de Exames
**Passos:** Acesse "Meus Exames"
**Esperado:** Lista de exames, possível baixar PDFs

### CT-PORTAL-006: Enviar Mensagem à Clínica
**Passos:** "Contato" > Envie mensagem
**Esperado:** Mensagem enviada, ticket criado

### CT-PORTAL-007: Atualizar Dados Cadastrais
**Passos:** "Meu Perfil" > Edite dados > Salve
**Esperado:** Dados atualizados, log criado

### CT-PORTAL-008: Cancelar Consulta Agendada
**Passos:** "Minhas Consultas" > Cancele consulta
**Esperado:** Cancelamento confirmado, notificação enviada

## ✅ Critérios de Aceite
- [ ] Cadastro funciona
- [ ] Login funciona
- [ ] Histórico visível
- [ ] Agendamento online funciona
- [ ] Exames acessíveis
- [ ] Comunicação funciona

## 📚 Documentação
- [Patient Portal Guide](../../PATIENT_PORTAL_GUIDE.md)

## ⏭️ Próximos Passos
➡️ [06-Testes-CRM.md](06-Testes-CRM.md)
