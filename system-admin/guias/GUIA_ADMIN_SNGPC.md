# Guia do Administrador: Sistema SNGPC

**Versão:** 1.0  
**Data:** 29 de Janeiro de 2026  
**Público:** Administradores de Sistema, Responsáveis Técnicos, Gerentes de Clínica

---

## 📚 Índice

1. [Introdução ao SNGPC](#introdução-ao-sngpc)
2. [Configuração Inicial](#configuração-inicial)
3. [Gerenciamento de Relatórios](#gerenciamento-de-relatórios)
4. [Sistema de Alertas](#sistema-de-alertas)
5. [Transmissão para ANVISA](#transmissão-para-anvisa)
6. [Auditoria e Compliance](#auditoria-e-compliance)
7. [Backup e Segurança](#backup-e-segurança)
8. [Troubleshooting](#troubleshooting)

---

## Introdução ao SNGPC

### O que é SNGPC?

**Sistema Nacional de Gerenciamento de Produtos Controlados** - Sistema da ANVISA (Agência Nacional de Vigilância Sanitária) para monitoramento e controle da prescrição e dispensação de medicamentos controlados no Brasil.

### Legislação

- **ANVISA Portaria 344/1998** - Define substâncias controladas (Listas A, B, C)
- **ANVISA RDC 27/2007** - Institui o SNGPC
- **ANVISA RDC 22/2014** - Atualização do SNGPC (versão atual)

### Medicamentos Sujeitos ao SNGPC

**Lista A (Entorpecentes):**
- A1: Entorpecentes (ex: Morfina, Metadona)
- A2: Entorpecentes (ex: Codeína, Cannabis)
- A3: Psicotrópicos (ex: Pentobarbital, Cetamina)

**Lista B (Psicotrópicos):**
- B1: Psicotrópicos (ex: Clonazepam, Diazepam, Alprazolam)
- B2: Psicotrópicos anorexígenos (ex: Anfepramona, Femproporex)

**Lista C1 (Outras Substâncias Controladas):**
- Antidepressivos, anticonvulsivantes e outros controlados

### Obrigações Legais

- ✅ **Escrituração mensal** de todas as movimentações
- ✅ **Transmissão até dia 10** do mês seguinte
- ✅ **Balanço mensal** com estoque inicial, entradas, saídas e estoque final
- ✅ **Registro de receitas** com dados completos do prescritor e paciente
- ✅ **Manutenção de arquivos** por no mínimo 2 anos

### Penalidades por Não Compliance

- ⚠️ **Advertência** - Primeira ocorrência leve
- 💰 **Multa** - R$ 2.000 a R$ 1.500.000 dependendo da gravidade
- 🔒 **Suspensão temporária** - Atividades suspensas
- ❌ **Cancelamento de licença** - Em casos graves

---

## Configuração Inicial

### 1. Configurar Dados da Clínica

**Local:** Configurações → Dados da Clínica → SNGPC

Preencha:
- **CNPJ da Clínica**
- **Razão Social**
- **Nome Fantasia**
- **Endereço Completo**
- **Telefone**
- **Email para notificações**
- **Responsável Técnico** (nome e CRF se aplicável)

### 2. Cadastrar Certificado Digital (Quando disponível)

Para transmissão automática à ANVISA:

1. Obtenha certificado digital **e-CNPJ** ou **e-CPF** ICP-Brasil
2. Vá em **Configurações → Segurança → Certificados Digitais**
3. Faça upload do arquivo `.pfx` ou `.p12`
4. Informe a senha do certificado
5. Teste a conexão

**Tipos de Certificado:**
- **e-CNPJ A1** - Software, válido 1 ano, mais prático
- **e-CNPJ A3** - Token/Smartcard, válido 3 anos, mais seguro

### 3. Configurar Alertas Automáticos

**Local:** SNGPC → Configurações → Alertas

Configure quando deseja receber alertas:
- ✅ **Deadline Approaching** - Quantos dias antes? (Padrão: 5)
- ✅ **Missing Report** - Avisar no dia 1 de cada mês?
- ✅ **Overdue Report** - Avisar após dia 10?
- ✅ **Negative Balance** - Alertar imediatamente?

**Destinatários:**
- Adicione emails para receber notificações
- Configure notificações no sistema
- Integre com Telegram/WhatsApp (se disponível)

### 4. Configurar Backup Automático

**Local:** Configurações → Backup → SNGPC

Recomendações:
- ✅ Backup diário dos dados SNGPC
- ✅ Backup antes de cada transmissão
- ✅ Manter backups por 2 anos (mínimo legal)
- ✅ Testar restauração mensalmente

---

## Gerenciamento de Relatórios

### Ciclo Mensal do SNGPC

```
Dia 1 → Sistema cria relatório do mês anterior automaticamente
Dia 1-9 → Revisar dados, gerar XML, preparar transmissão
Dia 10 → PRAZO FINAL para transmissão
Dia 11+ → Atraso! Risco de penalidade
```

### Dashboard SNGPC

**Local:** SNGPC → Dashboard

**Cards Informativos:**

1. **Prescrições Não Reportadas**
   - Mostra prescrições controladas ainda não incluídas em relatório
   - **Ação:** Verificar se são do mês atual ou esquecidas

2. **Relatórios Vencidos**
   - Mostra relatórios que passaram do dia 10
   - **Ação:** Transmitir imediatamente!

3. **Total de Transmissões**
   - Histórico de transmissões bem-sucedidas
   - **Ação:** Monitoramento de compliance

**Tabela de Relatórios:**

Colunas:
- **Período** - Mês/Ano do relatório
- **Status** - Draft, Generated, Transmitted, Failed
- **Prescrições** - Quantidade incluída
- **Deadline** - Data limite de transmissão
- **Ações** - Gerar XML, Transmitir, Download

### Workflow do Relatório

#### 1. Criação Automática (Dia 1)

O sistema cria automaticamente no dia 1 de cada mês.

**Se não criou automaticamente:**
```
SNGPC → Relatórios → Criar Novo Relatório
Selecionar: Mês e Ano
Clicar: Criar Relatório
```

#### 2. Revisão dos Dados

Antes de gerar o XML, revise:

```
Abrir relatório → Visualizar Prescrições Incluídas
```

Verifique:
- ✅ Todas as prescrições do período estão incluídas?
- ✅ Dados dos prescritores estão corretos?
- ✅ CPF dos pacientes estão válidos?
- ✅ Classificações ANVISA estão corretas?

**Se encontrar erros:**
- Prescrições com dados incorretos **não podem ser corrigidas**
- Você precisa criar uma nova prescrição e incluir no relatório
- Marque a errada como "cancelada" internamente

#### 3. Geração do XML

```
Relatório → Ações → Gerar XML
```

O sistema gera um arquivo XML no formato:
```
SNGPC_AAAA_MM_CNPJ.xml
Exemplo: SNGPC_2026_01_12345678000190.xml
```

**Estrutura do XML:**
- ✅ Cabeçalho com período e totais
- ✅ Dados de cada prescrição
- ✅ Dados de cada medicamento controlado
- ✅ Conforme ANVISA schema v2.1

#### 4. Validação do XML

Antes de transmitir, valide:

1. **Download do XML** - Salve localmente
2. **Abra em navegador** - Verifique se abre sem erros
3. **Validação ANVISA** (opcional):
   - Acesse: https://www.anvisa.gov.br/sngpc
   - Use ferramenta de validação
   - Faça upload do XML
   - Verifique se não há erros

#### 5. Transmissão

**Opção A: Transmissão Manual (Atual)**

1. Download do XML do sistema
2. Acesse portal ANVISA: https://www.anvisa.gov.br/sngpc
3. Login com certificado digital
4. Upload do arquivo XML
5. Aguarde processamento
6. Copie o número de protocolo
7. Volte ao sistema
8. **Relatório → Marcar como Transmitido**
9. Informe o protocolo ANVISA

**Opção B: Transmissão Automática (Futuro)**

Quando a integração automática estiver configurada:

1. **Relatório → Transmitir Automaticamente**
2. Sistema envia diretamente para ANVISA
3. Protocolo é registrado automaticamente

#### 6. Confirmação

Após transmissão:
- ✅ Status muda para "Transmitted"
- ✅ Protocolo é salvo no histórico
- ✅ Alerta de deadline desaparece
- ✅ Backup automático é feito

---

## Sistema de Alertas

### Tipos de Alertas

#### 1. DeadlineApproaching (⚠️ Warning)

**Gatilho:** 5 dias antes do dia 10  
**Descrição:** "Relatório de [Mês/Ano] deve ser transmitido até [Data]"  
**Ação Requerida:** Preparar XML e transmitir nos próximos dias

#### 2. DeadlineOverdue (🔴 Critical)

**Gatilho:** Após dia 10  
**Descrição:** "Relatório de [Mês/Ano] está vencido desde [Data]"  
**Ação Requerida:** Transmitir IMEDIATAMENTE para evitar penalidade

#### 3. MissingReport (⚠️ Warning)

**Gatilho:** Dia 5 e relatório não foi criado  
**Descrição:** "Relatório mensal de [Mês/Ano] não foi criado"  
**Ação Requerida:** Criar o relatório manualmente

#### 4. InvalidBalance (🟡 Error)

**Gatilho:** Balanço calculado não fecha  
**Descrição:** "Balanço de [Medicamento] está inconsistente"  
**Ação Requerida:** Revisar entradas e saídas, corrigir registros

#### 5. NegativeBalance (🔴 Critical)

**Gatilho:** Estoque negativo detectado  
**Descrição:** "Estoque de [Medicamento] está negativo: -X unidades"  
**Ação Requerida:** Corrigir URGENTEMENTE - possível erro de registro

#### 6. TransmissionFailed (🔴 Critical)

**Gatilho:** Erro ao transmitir para ANVISA  
**Descrição:** "Transmissão de [Mês/Ano] falhou: [Erro]"  
**Ação Requerida:** Verificar erro, corrigir e retransmitir

#### 7. UnusualMovement (ℹ️ Info)

**Gatilho:** Padrão incomum detectado  
**Descrição:** "Prescrição de [Medicamento] aumentou X% no último mês"  
**Ação Requerida:** Revisar se é esperado ou investigar

#### 8. ExcessiveDispensing (⚠️ Warning)

**Gatilho:** Quantidade acima do padrão  
**Descrição:** "Prescrição de [Medicamento]: [Quantidade] unidades (acima do normal)"  
**Ação Requerida:** Verificar se é legítimo

### Gerenciamento de Alertas

**Local:** SNGPC → Alertas

**Ações Disponíveis:**

#### Reconhecer (Acknowledge)
```
Alerta → Reconhecer
Adicionar nota: "Verificado, transmissão agendada para amanhã"
```

**Efeito:** Marca como visto, mas mantém ativo

#### Resolver (Resolve)
```
Alerta → Resolver
Descrição da resolução: "Transmitido com sucesso. Protocolo: ANVISA-2026-01-12345"
```

**Efeito:** Marca como resolvido, sai da lista de ativos

#### Filtros
- Por severidade: Critical, Error, Warning, Info
- Por tipo: Deadline, Balance, Transmission, etc.
- Por data: Hoje, Última semana, Último mês
- Por status: Ativo, Reconhecido, Resolvido

### Dashboard de Alertas

**Métricas:**
- 🔴 **Críticos Ativos** - Requerem ação imediata
- ⚠️ **Avisos Ativos** - Requerem atenção
- ✅ **Resolvidos no Mês** - Histórico de resolução

---

## Transmissão para ANVISA

### Pré-requisitos

Antes de transmitir, certifique-se:

- ✅ **Certificado Digital** configurado (e-CNPJ ou e-CPF)
- ✅ **Cadastro no Portal ANVISA** aprovado
- ✅ **Permissões** de acesso ao SNGPC
- ✅ **XML Validado** sem erros

### Portal ANVISA

**URL:** https://www.anvisa.gov.br/sngpc

**Login:**
1. Selecione "Acesso com Certificado Digital"
2. Escolha seu certificado (A1 ou A3)
3. Informe PIN (se A3)
4. Aguarde autenticação

**Navegação:**
```
Menu → SNGPC → Escrituração → Enviar Arquivo
```

### Processo de Envio

#### 1. Upload do XML

1. Clique em "Selecionar Arquivo"
2. Escolha o XML gerado pelo sistema
3. Clique em "Enviar"
4. Aguarde processamento (pode levar 1-5 minutos)

#### 2. Validação ANVISA

O sistema da ANVISA valida:
- ✅ Estrutura do XML (schema v2.1)
- ✅ Dados obrigatórios preenchidos
- ✅ CPF/CNPJ válidos
- ✅ CRM dos prescritores
- ✅ Datas dentro do período
- ✅ Classificações corretas

#### 3. Resultado

**Sucesso:** ✅
```
Arquivo recebido com sucesso!
Protocolo: ANVISA-2026-01-12345
Data/Hora: 08/02/2026 14:30:00
```

**Erro:** ❌
```
Erro na linha 42: CPF inválido
Erro na linha 58: CRM não cadastrado
```

### Tratamento de Erros

**Erro Comum 1: CPF Inválido**

**Solução:**
1. Identifique a prescrição com erro
2. Verifique o CPF do paciente
3. Corrija no cadastro do paciente
4. Crie nova prescrição
5. Regere o XML
6. Retransmita

**Erro Comum 2: CRM Não Cadastrado**

**Solução:**
1. Verifique se CRM está correto
2. Pode precisar cadastrar o médico no portal ANVISA
3. Aguarde aprovação (1-2 dias úteis)
4. Retransmita

**Erro Comum 3: Data Fora do Período**

**Solução:**
1. Verifique datas das prescrições
2. Certifique-se que estão dentro do mês reportado
3. Remova prescrições fora do período
4. Regere XML
5. Retransmita

### Retry e Resiliência

**Falha de Conexão:**
- Sistema tenta automaticamente 3 vezes
- Intervalo de 30 segundos entre tentativas
- Se falhar, alerta é criado

**Timeout:**
- Timeout de 60 segundos
- Se exceder, tente novamente após 5 minutos
- Portal ANVISA pode estar congestionado (pico próximo ao dia 10)

---

## Auditoria e Compliance

### Relatórios de Compliance

**Local:** SNGPC → Relatórios → Compliance

**Relatórios Disponíveis:**

#### 1. Histórico de Transmissões
- Lista todas as transmissões realizadas
- Status: Sucesso, Falha, Pendente
- Protocolos ANVISA
- Data e hora de cada transmissão

#### 2. Prescrições por Período
- Total de prescrições controladas
- Quebra por tipo (A, B, C1)
- Quebra por médico
- Gráfico de evolução mensal

#### 3. Medicamentos Mais Prescritos
- Top 10 medicamentos controlados
- Quantidade por medicamento
- Classificação ANVISA
- Variação vs. mês anterior

#### 4. Compliance Score
- Percentual de transmissões no prazo
- Número de alertas resolvidos
- Tempo médio de resolução
- Indicador de risco

### Auditoria Interna

**Checklist Mensal:**

- [ ] Todos os relatórios foram transmitidos?
- [ ] Protocolos ANVISA foram registrados?
- [ ] Não há alertas críticos ativos?
- [ ] Backup foi realizado?
- [ ] Prescrições canceladas foram documentadas?
- [ ] Não há saldos negativos?

**Documentação:**
- Mantenha log de todas as ações administrativas
- Registre justificativas para atrasos
- Documente erros e correções
- Salve comprovantes de transmissão

### Preparação para Fiscalização

**Documentos a ter prontos:**

1. **Relatórios SNGPC** - Todos os meses dos últimos 2 anos
2. **Protocolos ANVISA** - Comprovantes de transmissão
3. **Prescrições Originais** - Digitais com QR Code
4. **Logs de Sistema** - Auditoria de ações
5. **Certificados** - Comprovação de certificado digital válido

**Dicas:**
- ✅ Mantenha tudo organizado por mês
- ✅ Tenha backup offline atualizado
- ✅ Documente processos internos
- ✅ Treine equipe sobre compliance

---

## Backup e Segurança

### Estratégia de Backup

**Backup Diário:**
- Dados de prescrições
- Relatórios SNGPC
- Alertas e resoluções
- Executado automaticamente às 2h da manhã

**Backup Mensal:**
- Snapshot completo do banco de dados
- Arquivos XML gerados
- Logs de auditoria
- Executado no dia 1 de cada mês

**Backup Antes de Transmissão:**
- Snapshot do relatório que será transmitido
- Cópia do XML gerado
- Estado atual dos alertas

### Retenção

**Mínimo Legal:** 2 anos  
**Recomendado:** 5 anos  
**Sistema:** 20 anos (mesmo período das prescrições)

### Localização dos Backups

**Padrão:** `/backup/sngpc/`

Estrutura:
```
/backup/sngpc/
  ├── 2026/
  │   ├── 01/
  │   │   ├── daily/
  │   │   ├── reports/
  │   │   └── xml/
  │   ├── 02/
  │   └── ...
  ├── 2025/
  └── ...
```

### Segurança

**Acesso Restrito:**
- Apenas administradores podem acessar SNGPC
- Log de todas as ações administrativas
- Autenticação two-factor obrigatória

**Criptografia:**
- Dados em repouso: AES-256
- Dados em trânsito: TLS 1.3
- Backups: Criptografados

**Certificados Digitais:**
- Armazenados em HSM ou local seguro
- Senhas não salvas em texto plano
- Rotação periódica

---

## Troubleshooting

### Problema: Relatório não foi criado automaticamente

**Diagnóstico:**
```bash
# Verificar se job de criação está ativo
tail -f /var/log/sngpc-jobs.log | grep "CreateMonthlyReport"
```

**Solução:**
1. Verifique se serviço de jobs está rodando
2. Verifique configuração de timezone
3. Crie manualmente: SNGPC → Criar Relatório

---

### Problema: XML com erro de validação

**Erro:** "Character reference is invalid"

**Causa:** Caracteres especiais não sanitizados

**Solução:**
1. Sistema sanitiza automaticamente
2. Se persistir, verifique nomes de pacientes/médicos
3. Remova emojis, acentos problemáticos

---

### Problema: Certificado digital não reconhecido

**Diagnóstico:**
1. Certificado expirou?
2. Certificado foi revogado?
3. Driver do token A3 instalado?

**Solução:**
1. Verifique validade: Propriedades do Certificado
2. Se expirado, renove
3. Se A3, instale driver do fabricante

---

### Problema: Transmissão sempre falha

**Erro:** "Timeout connecting to ANVISA"

**Causa:** Firewall bloqueando, ou portal ANVISA fora do ar

**Solução:**
1. Libere porta 443 para *.anvisa.gov.br
2. Tente em horário diferente
3. Verifique status do portal ANVISA
4. Use VPN se necessário

---

### Problema: Saldo negativo detectado

**Causa:** Erro de registro ou prescrição duplicada

**Solução:**
1. Liste todas as prescrições do medicamento
2. Identifique duplicatas ou erros
3. Corrija registros manualmente no banco (com cuidado!)
4. Regere balanço mensal

---

## 📞 Contatos Importantes

**Suporte PrimeCare:**
- Email: suporte@primecaresoftware.com
- Telefone: (11) XXXX-XXXX
- Horário: 8h-18h dias úteis

**ANVISA - SNGPC:**
- Portal: https://www.anvisa.gov.br/sngpc
- Email: sngpc@anvisa.gov.br
- Telefone: 0800 642 9782

**Emergência Compliance:**
- Para problemas críticos de compliance fora do horário
- WhatsApp: (11) 9XXXX-XXXX

---

## 📖 Documentação Adicional

- Manual Completo ANVISA: https://www.anvisa.gov.br/sngpc/manual
- Legislação Atualizada: https://www.anvisa.gov.br/legislacao
- FAQ ANVISA: https://www.anvisa.gov.br/sngpc/faq

---

**Última Atualização:** 29 de Janeiro de 2026  
**Versão:** 1.0  
**Autor:** PrimeCare Software
