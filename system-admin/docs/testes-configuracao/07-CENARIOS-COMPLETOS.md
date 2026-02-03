# 🧪 Cenários de Testes Completos - Omni Care Software

## 📌 Visão Geral

Este documento consolida TODOS os cenários de testes possíveis do Omni Care Software, organizados por módulo, fluxo de trabalho e complexidade. Use este guia como checklist completo para garantir que todas as funcionalidades foram testadas.

## 📖 Índice

1. [Testes de Cadastros Básicos](#testes-de-cadastros-básicos)
2. [Testes de Fluxo Operacional](#testes-de-fluxo-operacional)
3. [Testes de Integrações](#testes-de-integrações)
4. [Testes de Segurança](#testes-de-segurança)
5. [Testes de Performance](#testes-de-performance)
6. [Testes de Validação](#testes-de-validação)
7. [Testes de Relatórios](#testes-de-relatórios)
8. [Testes de Edge Cases](#testes-de-edge-cases)

---

## ✅ Testes de Cadastros Básicos

### 1. Cadastro de Clínica
- [ ] Cadastro completo com todos os campos
- [ ] Cadastro mínimo (apenas obrigatórios)
- [ ] Upload de logo da clínica
- [ ] Configuração de CNES
- [ ] Cadastro de múltiplas especialidades
- [ ] Validação de CNPJ
- [ ] Cadastro de endereço via CEP

### 2. Cadastro de Usuários
- [ ] Owner (proprietário da clínica)
- [ ] Medic (médico)
- [ ] Secretary (secretária)
- [ ] Nurse (enfermeiro)
- [ ] Receptionist (recepcionista)
- [ ] Validação de CPF
- [ ] Validação de email único
- [ ] Upload de foto de perfil
- [ ] Configuração de assinatura digital

### 3. Cadastro de Pacientes
- [ ] Paciente completo
- [ ] Paciente menor de idade (com responsável)
- [ ] Paciente com convênio
- [ ] Paciente particular
- [ ] Paciente estrangeiro (sem CPF)
- [ ] Validação de CPF duplicado
- [ ] Busca de CEP automática
- [ ] Upload de documentos
- [ ] Histórico médico completo
- [ ] Alergias e condições pré-existentes

### 4. Cadastro de Convênios
- [ ] Operadora com integração TISS
- [ ] Operadora sem integração
- [ ] Tabela de valores própria
- [ ] Configuração de prazos de pagamento
- [ ] Procedimentos cobertos/não cobertos
- [ ] Configuração de autorização prévia

### 5. Cadastro de Procedimentos/Serviços
- [ ] Procedimento com código TUSS
- [ ] Procedimento sem código TUSS
- [ ] Diferentes valores por convênio
- [ ] Procedimento que requer materiais
- [ ] Procedimento que requer autorização
- [ ] Duração e preparos necessários

### 6. Cadastro de Fornecedores
- [ ] Fornecedor pessoa jurídica (CNPJ)
- [ ] Fornecedor pessoa física (CPF)
- [ ] Dados bancários para pagamento
- [ ] PIX cadastrado
- [ ] Categorização de fornecedores

---

## ✅ Testes de Fluxo Operacional

### Fluxo 1: Primeira Consulta de Novo Paciente

**Cenário Completo:**
1. [ ] Cadastrar novo paciente
2. [ ] Agendar primeira consulta
3. [ ] Paciente chega - Check-in
4. [ ] Triagem (enfermagem)
5. [ ] Médico inicia atendimento
6. [ ] Anamnese completa
7. [ ] Exame físico
8. [ ] Solicitar exames laboratoriais
9. [ ] Definir diagnóstico (CID-10)
10. [ ] Prescrever medicamentos
11. [ ] Finalizar consulta
12. [ ] Processar pagamento
13. [ ] Agendar retorno

### Fluxo 2: Consulta de Retorno

**Cenário Completo:**
1. [ ] Paciente já cadastrado
2. [ ] Agendamento de retorno
3. [ ] Check-in
4. [ ] Médico acessa histórico anterior
5. [ ] Revisa exames realizados
6. [ ] Atualiza diagnóstico
7. [ ] Ajusta prescrição
8. [ ] Finaliza consulta
9. [ ] Processa pagamento com desconto

### Fluxo 3: Atendimento de Urgência

**Cenário Completo:**
1. [ ] Paciente chega sem agendamento
2. [ ] Cadastro rápido (dados básicos)
3. [ ] Encaixe na agenda
4. [ ] Triagem prioritária
5. [ ] Atendimento imediato
6. [ ] Emitir guia de urgência (se convênio)
7. [ ] Documentação mínima
8. [ ] Orientações e medicações
9. [ ] Alta ou encaminhamento

### Fluxo 4: Teleconsulta Completa

**Cenário Completo:**
1. [ ] Agendar teleconsulta
2. [ ] Enviar link ao paciente
3. [ ] Paciente aceita termo de consentimento
4. [ ] Paciente entra na sala de espera
5. [ ] Médico inicia videochamada
6. [ ] Compartilhar documentos/exames
7. [ ] Chat durante consulta
8. [ ] Gravar sessão (com consentimento)
9. [ ] Prescrição digital com certificado
10. [ ] Finalizar e salvar gravação
11. [ ] Processar pagamento online

### Fluxo 5: Faturamento TISS Completo

**Cenário Completo:**
1. [ ] Consultas com convênio realizadas
2. [ ] Verificar guias autorizadas
3. [ ] Criar lote de faturamento
4. [ ] Validar lote (XML)
5. [ ] Enviar para operadora
6. [ ] Consultar status
7. [ ] Receber retorno
8. [ ] Processar glosas
9. [ ] Recursar glosas indevidas
10. [ ] Registrar pagamento
11. [ ] Baixar contas a receber

### Fluxo 6: Ciclo Financeiro Mensal

**Cenário Completo:**
1. [ ] Lançar contas a pagar do mês
2. [ ] Registrar recebimentos
3. [ ] Processar pagamentos de fornecedores
4. [ ] Gerar relatório DRE
5. [ ] Fechar caixa diário
6. [ ] Conciliar contas bancárias
7. [ ] Gerar relatório de inadimplência
8. [ ] Análise de lucratividade

---

## ✅ Testes de Integrações

### Integração 1: TISS/TUSS
- [ ] Importar tabela TUSS
- [ ] Gerar guia de consulta
- [ ] Gerar guia SP/SADT
- [ ] Solicitar autorização
- [ ] Enviar lote via WebService
- [ ] Receber retorno
- [ ] Processar glosas

### Integração 2: ViaCEP
- [ ] Buscar endereço por CEP
- [ ] Preencher campos automaticamente
- [ ] Tratamento de CEP não encontrado
- [ ] CEP inválido

### Integração 3: Email/SMS
- [ ] Enviar confirmação de agendamento
- [ ] Lembrete 1 dia antes
- [ ] Lembrete 1 hora antes
- [ ] Link de teleconsulta
- [ ] Receita digital por email
- [ ] Resultados de exames

### Integração 4: Daily.co (Telemedicina)
- [ ] Criar sala de vídeo
- [ ] Gerar tokens de acesso
- [ ] Iniciar chamada
- [ ] Gravar sessão
- [ ] Obter URL de gravação
- [ ] Deletar sala após uso

### Integração 5: Certificado Digital
- [ ] Upload de certificado A3/A1
- [ ] Validar certificado
- [ ] Assinar prescrição digital
- [ ] Verificar validade
- [ ] Renovar certificado expirado

### Integração 6: Backup e Armazenamento
- [ ] Backup automático diário
- [ ] Backup manual sob demanda
- [ ] Restaurar backup
- [ ] Upload de arquivos (S3/Azure)
- [ ] Download de arquivos

---

## ✅ Testes de Segurança

### Autenticação e Autorização
- [ ] Login com email/senha
- [ ] Login por subdomínio
- [ ] Login com 2FA (se habilitado)
- [ ] Recuperação de senha
- [ ] Expiração de sessão (timeout)
- [ ] Logout forçado após inatividade
- [ ] Múltiplas sessões simultâneas
- [ ] Tentativas de login com credenciais inválidas (rate limiting)

### Controle de Acesso (RBAC)
- [ ] Owner acessa tudo
- [ ] Medic acessa apenas seus pacientes
- [ ] Secretary não acessa dados financeiros sensíveis
- [ ] Nurse não edita prontuários
- [ ] SystemAdmin não vê dados de pacientes
- [ ] Tentativa de acesso não autorizado (403)

### Isolamento Multi-tenant
- [ ] Clínica A não vê dados da Clínica B
- [ ] Query com TenantId correto
- [ ] Tentativa de SQL Injection
- [ ] Validação de TenantId em todos os endpoints

### Criptografia
- [ ] Senhas hasheadas (bcrypt)
- [ ] Dados sensíveis criptografados
- [ ] HTTPS obrigatório
- [ ] Tokens JWT válidos
- [ ] Certificados SSL válidos

### LGPD
- [ ] Termo de consentimento
- [ ] Direito ao esquecimento
- [ ] Exportar dados do paciente
- [ ] Log de acessos ao prontuário
- [ ] Anonimização de dados em relatórios

---

## ✅ Testes de Performance

### Carga de Dados
- [ ] 100 pacientes cadastrados
- [ ] 1.000 pacientes cadastrados
- [ ] 10.000 pacientes cadastrados
- [ ] Busca em lista grande
- [ ] Paginação eficiente
- [ ] Filtros em listas grandes

### Concorrência
- [ ] 10 usuários simultâneos
- [ ] 50 usuários simultâneos
- [ ] 100 usuários simultâneos
- [ ] Múltiplas agendas sendo acessadas
- [ ] Conflitos de agendamento simultâneo

### Tempo de Resposta
- [ ] Login < 1s
- [ ] Carregar lista de pacientes < 2s
- [ ] Abrir prontuário < 1s
- [ ] Salvar consulta < 500ms
- [ ] Gerar relatório < 5s
- [ ] Exportar para Excel < 3s

### Otimização
- [ ] Lazy loading de imagens
- [ ] Cache de consultas frequentes
- [ ] Compressão de respostas (gzip)
- [ ] Minificação de assets
- [ ] CDN para arquivos estáticos

---

## ✅ Testes de Validação

### Validações de Campo
- [ ] Email válido
- [ ] CPF válido
- [ ] CNPJ válido
- [ ] CEP válido
- [ ] Telefone no formato correto
- [ ] Data no formato DD/MM/YYYY
- [ ] Hora no formato HH:MM
- [ ] Valores monetários positivos
- [ ] Campos obrigatórios preenchidos

### Validações de Negócio
- [ ] Idade mínima para procedimento
- [ ] Carteirinha de convênio válida
- [ ] Horário de agendamento disponível
- [ ] Médico com CRM ativo
- [ ] Paciente não pode ter duas consultas simultâneas
- [ ] Valor não pode ser negativo
- [ ] Data de nascimento não pode ser futura
- [ ] Duração da consulta > 0

### Validações de Integridade
- [ ] Não pode deletar médico com consultas agendadas
- [ ] Não pode deletar paciente com histórico
- [ ] Não pode deletar convênio em uso
- [ ] Não pode alterar consulta já finalizada
- [ ] Não pode estornar pagamento sem justificativa

---

## ✅ Testes de Relatórios

### Relatórios Financeiros
- [ ] DRE (Demonstração de Resultado)
- [ ] Fluxo de caixa diário/mensal
- [ ] Contas a receber
- [ ] Contas a pagar
- [ ] Inadimplência
- [ ] Faturamento por convênio
- [ ] Faturamento por médico
- [ ] Custos por categoria

### Relatórios Operacionais
- [ ] Agendamentos do dia/semana/mês
- [ ] Taxa de ocupação da agenda
- [ ] Taxa de absenteísmo
- [ ] Tempo médio de consulta
- [ ] Procedimentos mais realizados
- [ ] Pacientes ativos/inativos
- [ ] Novos pacientes por período

### Relatórios Médicos
- [ ] Consultas por médico
- [ ] Diagnósticos mais comuns (CID-10)
- [ ] Medicamentos mais prescritos
- [ ] Exames mais solicitados
- [ ] Pacientes por faixa etária
- [ ] Distribuição por sexo

### Relatórios de Telemedicina
- [ ] Total de teleconsultas
- [ ] Duração média
- [ ] Taxa de conclusão
- [ ] Problemas de conexão
- [ ] Satisfação do paciente

---

## ✅ Testes de Edge Cases

### Casos Extremos
- [ ] Paciente com nome muito longo (>100 caracteres)
- [ ] Consulta com duração de 1 minuto
- [ ] Consulta com duração de 8 horas
- [ ] Procedimento com valor R$ 0,01
- [ ] Procedimento com valor R$ 1.000.000,00
- [ ] Paciente com 150 anos
- [ ] Paciente recém-nascido (0 anos)
- [ ] Agendamento para 10 anos no futuro
- [ ] 100 medicamentos em uma prescrição

### Cenários de Erro
- [ ] Banco de dados offline
- [ ] API não responde
- [ ] Timeout em requisição
- [ ] WebService TISS indisponível
- [ ] Daily.co offline (telemedicina)
- [ ] Email não envia
- [ ] SMS não envia
- [ ] Certificado digital expirado
- [ ] Disco cheio (upload)
- [ ] Memória insuficiente

### Recuperação de Erros
- [ ] Retry automático de requisições
- [ ] Fallback para modo offline
- [ ] Mensagens de erro amigáveis
- [ ] Log de erros para debug
- [ ] Notificação de administrador em erros críticos
- [ ] Restauração de sessão após falha
- [ ] Auto-save de formulários

### Compatibilidade
- [ ] Chrome (desktop)
- [ ] Firefox (desktop)
- [ ] Safari (desktop)
- [ ] Edge (desktop)
- [ ] Safari Mobile (iOS)
- [ ] Chrome Mobile (Android)
- [ ] Tablet (iPad/Android)
- [ ] Resoluções 1920x1080, 1366x768, 1024x768
- [ ] Dark mode
- [ ] Zoom 150%, 200%

---

## 🎯 Matriz de Prioridade de Testes

### Prioridade ALTA (Crítico)
- Login e autenticação
- Cadastro de paciente
- Agendamento
- Atendimento/consulta
- Prescrição médica
- Fechamento financeiro
- Pagamento

### Prioridade MÉDIA (Importante)
- Relatórios principais
- Integração TISS
- Telemedicina
- Gestão de convênios
- Fluxo de caixa

### Prioridade BAIXA (Desejável)
- Relatórios avançados
- Estatísticas
- Configurações avançadas
- Customizações visuais

---

## ✅ Checklist de Aceitação Final

### Antes de Aprovar uma Release

- [ ] Todos os testes de Prioridade ALTA passaram
- [ ] 90%+ dos testes de Prioridade MÉDIA passaram
- [ ] Sem bugs críticos conhecidos
- [ ] Performance aceitável (< 3s carregamento)
- [ ] Segurança validada (sem vulnerabilidades conhecidas)
- [ ] Documentação atualizada
- [ ] Backup testado e funcionando
- [ ] Rollback plan definido
- [ ] Suporte treinado
- [ ] Usuários-chave testaram

### Critérios de Sucesso

- ✅ **Funcionalidade:** Todas as features funcionam conforme especificado
- ✅ **Usabilidade:** Interface intuitiva, feedback claro
- ✅ **Performance:** Tempos de resposta adequados
- ✅ **Segurança:** Dados protegidos, acesso controlado
- ✅ **Confiabilidade:** Sistema estável, sem crashes
- ✅ **Manutenibilidade:** Código limpo, documentado

---

## 📊 Métricas de Qualidade

### Cobertura de Testes
- **Meta:** 80%+ de cobertura de código
- **Atual:** Verificar via ferramentas de CI/CD

### Taxa de Bugs
- **Meta:** < 5 bugs/1000 linhas de código
- **Críticos:** 0
- **Altos:** < 3
- **Médios:** < 10
- **Baixos:** Aceitável

### Tempo de Resolução
- **Crítico:** < 4 horas
- **Alto:** < 24 horas
- **Médio:** < 1 semana
- **Baixo:** < 1 mês

---

## 📝 Registro de Testes

Use este template para documentar execução de testes:

```markdown
### Teste: [Nome do Teste]
- **Data:** DD/MM/YYYY
- **Executor:** Nome do testador
- **Ambiente:** Dev/Staging/Prod
- **Status:** ✅ Passou / ❌ Falhou / ⚠️ Parcial
- **Resultado:** Descrição do resultado
- **Bugs Encontrados:** Lista de bugs (se houver)
- **Observações:** Notas adicionais
```

---

## 📚 Documentação Relacionada

- [01 - Cadastro de Paciente](01-CADASTRO-PACIENTE.md)
- [02 - Atendimento e Consulta](02-ATENDIMENTO-CONSULTA.md)
- [03 - Módulo Financeiro](03-MODULO-FINANCEIRO.md)
- [04 - TISS Padrão](04-TISS-PADRAO.md)
- [05 - TUSS Tabela](05-TUSS-TABELA.md)
- [06 - Telemedicina](06-TELEMEDICINA.md)
- [Checklist Completo de Testes](../CHECKLIST_TESTES_COMPLETO.md)
- [Guia de Testes Passo a Passo](../GUIA_TESTES_PASSO_A_PASSO.md)

---

## 🔗 Ferramentas de Teste Recomendadas

### Testes Manuais
- Navegador com DevTools
- Postman (testes de API)
- Screenshot/gravação de tela

### Testes Automatizados
- xUnit (backend .NET)
- Jest (frontend Angular)
- Cypress (E2E)
- k6 (performance/carga)

### Monitoramento
- Application Insights
- Sentry (error tracking)
- LogRocket (session replay)
