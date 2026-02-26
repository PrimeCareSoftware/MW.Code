# Sprint 7 — Documentação, Testes de QA e Checklist Final

Este documento consolida os itens de validação da Sprint 7 com foco em:
- documentação funcional e técnica;
- fluxos críticos de negócio (cadastro, edição, busca, atendimento e teleconsulta);
- permissões e perfis;
- integração Twilio;
- campos dinâmicos de registro profissional;
- checklist de release/publicação/lançamento.

## 1) Escopo de QA (fluxos obrigatórios)

### 1.1 Cadastro e Edição
- [ ] Cadastro de usuário profissional com `Pode efetuar atendimento` marcado exige registro profissional (CRM/CRP/CRN/CRO/COREN etc.)
- [ ] Edição de usuário mantém consistência entre perfil, registro profissional e especialidade
- [ ] Cadastro de usuário administrativo (recepção, secretaria, financeiro, admin) não exige registro profissional

### 1.2 Busca de pacientes
- [ ] Busca por nome parcial retorna resultados relevantes e ordenados
- [ ] Busca por CPF/documento encontra paciente exato
- [ ] Busca mantém boa performance com base de dados maior (amostragem QA)

### 1.3 Atendimento clínico
- [ ] Profissionais habilitados para atendimento visualizam telas corretas
- [ ] Profissionais sem permissão de atendimento não acessam fluxo clínico
- [ ] Campos dinâmicos por especialidade aparecem corretamente no formulário

### 1.4 Teleconsulta
- [ ] Profissional com permissão consegue iniciar teleconsulta
- [ ] Paciente consegue ingressar na sessão pelo fluxo previsto
- [ ] Eventos de início/fim de sessão são registrados para auditoria

### 1.5 Permissões e menus
- [ ] Menus renderizados conforme perfil/permissão efetiva
- [ ] Acesso direto por URL (deep link) é bloqueado quando sem permissão
- [ ] Perfis administrativos não recebem menu clínico indevido

### 1.6 Integração Twilio
- [ ] Credenciais (Account SID/Auth Token/From) carregadas por ambiente
- [ ] Envio de mensagens de teste com retorno de status
- [ ] Tratamento de erro para credencial inválida/limite excedido

---

## 2) Casos de teste recomendados (resumo)

### CT-01 — Cadastro profissional com registro obrigatório
**Pré-condição:** perfil profissional selecionado + `Pode efetuar atendimento = true`.

**Passos:**
1. Tentar salvar sem registro profissional.
2. Informar registro profissional e salvar novamente.

**Resultado esperado:**
- Passo 1 bloqueia com validação.
- Passo 2 salva com sucesso.

### CT-02 — Edição de profissional
**Pré-condição:** usuário profissional existente.

**Passos:**
1. Editar especialidade.
2. Alterar flag de atendimento.
3. Salvar.

**Resultado esperado:**
- Dados persistidos e refletidos na listagem.

### CT-03 — Busca eficiente de pacientes
**Passos:**
1. Buscar por prefixo de nome.
2. Buscar por CPF completo.
3. Repetir com volume de dados maior.

**Resultado esperado:**
- Retorno coerente sem degradação perceptível em cenário padrão.

### CT-04 — Menu por permissão
**Passos:**
1. Login com perfil administrativo.
2. Login com perfil clínico.

**Resultado esperado:**
- Itens de menu exibidos conforme permissões.

### CT-05 — Fluxo teleconsulta
**Passos:**
1. Iniciar sessão de teleconsulta no atendimento.
2. Validar entrada do paciente.

**Resultado esperado:**
- Sessão ativa e finalização com registro de status.

### CT-06 — Twilio
**Passos:**
1. Enviar mensagem de teste com credenciais válidas.
2. Repetir com credenciais inválidas.

**Resultado esperado:**
- Sucesso no cenário válido; mensagem de erro amigável no inválido.

---

## 3) Ajuste de backend alinhado ao frontend (Sprint 7)

Para compatibilizar com os perfis exibidos no frontend de gestão de usuários:
- mapeamentos de perfis foram expandidos para `Dentista`, `Enfermeiro(a)`, `Fisioterapeuta` e `Veterinário`;
- lista de perfis reconhecidos e lista de perfis permitidos para criação foram atualizadas;
- isso evita rejeição indevida de perfis já disponíveis na UI.

---

## 4) Checklist final de release/publicação/lançamento

### 4.1 Release técnico
- [ ] Build frontend sem erros
- [ ] Build backend sem erros
- [ ] Migrações de banco revisadas e validadas
- [ ] Variáveis de ambiente (produção) validadas
- [ ] Integrações externas (Twilio/e-mail/etc.) testadas

### 4.2 Publicação
- [ ] Tag de release criada
- [ ] Notas de release publicadas
- [ ] Deploy em staging validado
- [ ] Smoke test pós-deploy executado

### 4.3 Lançamento
- [ ] Janela de lançamento comunicada
- [ ] Plano de rollback pronto e validado
- [ ] Monitoramento (logs/métricas/alertas) ativo
- [ ] Time de suporte alinhado com roteiro de contingência

### 4.4 Pós-lançamento (D+1)
- [ ] Revisão de erros críticos
- [ ] Revisão de integrações externas
- [ ] Coleta de feedback dos primeiros usuários
- [ ] Priorização de hotfixes (se necessário)
