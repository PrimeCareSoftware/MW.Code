# Guia da Fila de Espera (Waiting Queue)

## Visão Geral

A funcionalidade de Fila de Espera do Omni Care Software permite gerenciar o fluxo de atendimento de pacientes de forma eficiente, com priorização por triagem, chamada de pacientes e controle de tempo de espera.

## Recursos Principais

### 1. Gestão de Fila em Tempo Real

- **Atualização Automática**: A fila é atualizada automaticamente a cada 30 segundos
- **Contadores em Tempo Real**: 
  - Total de pacientes aguardando
  - Total de pacientes chamados
  - Total de pacientes em atendimento
  - Tempo médio de espera

### 2. Adição de Pacientes Avulsos

Uma das principais novidades é a capacidade de adicionar pacientes à fila sem necessidade de agendamento prévio:

#### Como Adicionar um Paciente Avulso:

1. Acesse **Fila de Espera** no menu principal
2. Na seção "Adicionar Paciente Avulso à Fila", digite:
   - Nome do paciente
   - CPF
   - Telefone
3. Clique em **🔍 Buscar** para encontrar o paciente
4. Na lista de resultados, clique em **➕ Adicionar à Fila**
5. O paciente será adicionado com prioridade Normal e a observação "Paciente avulso adicionado à fila"

#### Funcionalidades da Busca:

- **Busca por múltiplos critérios**: Nome, CPF ou telefone
- **Resultados em tempo real**: Mostra todos os pacientes correspondentes
- **Informações completas**: Exibe nome, CPF e telefone de cada resultado
- **Adição rápida**: Botão direto para adicionar à fila

### 3. Início de Atendimento Direto da Consulta de Pacientes

Agora é possível iniciar o atendimento de um paciente diretamente da tela de consulta:

#### Como Iniciar Atendimento:

1. Acesse **Pacientes** no menu principal
2. Na lista de pacientes, localize o paciente desejado
3. Na coluna de ações, clique no botão **✓ Iniciar Atendimento** (botão verde)
4. Você será redirecionado para a tela de criação de agendamento com o paciente pré-selecionado

#### Fluxo de Atendimento:

```
Consulta de Pacientes → Iniciar Atendimento → Criar Agendamento → Adicionar à Fila → Atendimento
```

### 4. Gestão de Prioridades (Triagem)

O sistema suporta cinco níveis de prioridade:

- **Emergência** (Emergency): Casos críticos que requerem atendimento imediato
- **Urgente** (Urgent): Casos urgentes com prioridade alta
- **Alta** (High): Casos importantes
- **Normal** (Normal): Casos regulares
- **Baixa** (Low): Casos de menor urgência

#### Como Editar a Triagem:

1. Na tabela da fila, clique no botão **📋** (Editar Triagem)
2. Selecione a nova prioridade
3. Adicione observações se necessário
4. Clique em **Salvar**

### 5. Fluxo de Atendimento

O sistema gerencia o fluxo completo do atendimento:

1. **Aguardando** (Waiting): Paciente na fila
2. **Chamado** (Called): Paciente foi chamado
3. **Em Atendimento** (InProgress): Atendimento em andamento
4. **Concluído** (Completed): Atendimento finalizado
5. **Cancelado** (Cancelled): Entrada cancelada

#### Ações Disponíveis por Status:

| Status | Ações Disponíveis |
|--------|-------------------|
| Aguardando | Editar Triagem, Chamar Paciente, Cancelar |
| Chamado | Iniciar Atendimento, Cancelar |
| Em Atendimento | Finalizar Atendimento, Cancelar |
| Concluído | - |
| Cancelado | - |

### 6. Notificações Sonoras

Quando um paciente é chamado, o sistema emite uma notificação sonora para alertar a equipe.

## Interface da Tela

### Seção 1: Adicionar Paciente Avulso

```
┌─────────────────────────────────────────────┐
│ Adicionar Paciente Avulso à Fila          │
├─────────────────────────────────────────────┤
│ [Buscar paciente...] [🔍 Buscar]          │
│                                             │
│ Resultados da Busca:                        │
│ ┌─────────────────────────────────────────┐ │
│ │ João Silva                              │ │
│ │ CPF: 123.456.789-00                     │ │
│ │ Tel: (11) 98765-4321  [➕ Adicionar]   │ │
│ └─────────────────────────────────────────┘ │
└─────────────────────────────────────────────┘
```

### Seção 2: Resumo da Fila

```
┌──────────┬──────────┬──────────┬──────────┐
│    ⏳    │    📢    │    🏥    │    ⏱️    │
│    5     │    2     │    3     │  15 min  │
│Aguardando│ Chamados │Em Atend. │T. Médio  │
└──────────┴──────────┴──────────┴──────────┘
```

### Seção 3: Tabela da Fila

| Posição | Paciente | Prioridade | Status | Tempo Espera | Ações |
|---------|----------|------------|--------|--------------|-------|
| 1 | Maria Santos | Alta | Aguardando | 10 min | 📋 📢 ❌ |
| 2 | João Silva | Normal | Chamado | 5 min | ▶️ ❌ |
| - | Pedro Costa | Normal | Em Atend. | 15 min | ✅ ❌ |

## Configuração

### Requisitos

- Clínica configurada no sistema
- Pacientes cadastrados
- Usuário com permissões adequadas

### Configuração da Clínica

Para usar a fila de espera, certifique-se de que:

1. O `clinicId` está configurado no localStorage ou no serviço de autenticação
2. A configuração da fila foi criada para a clínica

## API Endpoints Utilizados

### Fila de Espera

- `GET /api/waiting-queue/summary?clinicId={id}` - Obter resumo da fila
- `POST /api/waiting-queue` - Adicionar paciente à fila
- `PUT /api/waiting-queue/{entryId}/triage` - Atualizar triagem
- `PUT /api/waiting-queue/{entryId}/call` - Chamar paciente
- `PUT /api/waiting-queue/{entryId}/start` - Iniciar atendimento
- `PUT /api/waiting-queue/{entryId}/complete` - Finalizar atendimento
- `DELETE /api/waiting-queue/{entryId}` - Cancelar entrada

### Pacientes

- `GET /api/patients/search?searchTerm={term}` - Buscar pacientes

## Troubleshooting

### Problema: "Clínica não configurada"

**Solução**: Configure o `clinicId` no localStorage:
```javascript
localStorage.setItem('clinicId', 'seu-clinic-id-aqui');
```

### Problema: Busca de pacientes não retorna resultados

**Verificações**:
1. Certifique-se de que há pacientes cadastrados no sistema
2. Verifique se o termo de busca tem pelo menos 2 caracteres
3. Confirme que o endpoint de busca está funcionando corretamente

### Problema: Erro ao adicionar paciente à fila

**Verificações**:
1. Confirme que o `clinicId` está configurado corretamente
2. Verifique se o paciente já está na fila
3. Confirme que o serviço de fila de espera está ativo

## Boas Práticas

1. **Triagem Adequada**: Sempre classifique os pacientes com a prioridade correta
2. **Observações Detalhadas**: Adicione notas relevantes na triagem para ajudar no atendimento
3. **Atualização Regular**: Mantenha a lista atualizada, removendo ou finalizando atendimentos
4. **Comunicação**: Use as notificações sonoras para chamar os pacientes
5. **Monitoramento**: Acompanhe o tempo médio de espera e ajuste o fluxo conforme necessário

## Exemplos de Uso

### Cenário 1: Paciente com Agendamento

1. Paciente chega para consulta agendada
2. Sistema adiciona automaticamente à fila quando o check-in é feito
3. Recepcionista faz a triagem se necessário
4. Médico chama o paciente quando pronto
5. Atendimento é iniciado e depois finalizado

### Cenário 2: Paciente Avulso (Walk-in)

1. Paciente chega sem agendamento
2. Recepcionista busca o paciente na tela de Fila de Espera
3. Adiciona o paciente à fila usando a busca
4. Faz a triagem conforme necessidade
5. Paciente aguarda ser chamado
6. Fluxo normal de atendimento continua

### Cenário 3: Emergência

1. Paciente chega em situação de emergência
2. Recepcionista adiciona à fila (se não estiver)
3. Edita a triagem para "Emergência"
4. Sistema reorganiza a fila automaticamente
5. Paciente é atendido com prioridade

## Recursos Futuros

- [ ] Painel público para exibição da fila em TV
- [ ] Notificações push para pacientes
- [ ] Integração com sistema de senhas
- [ ] Relatórios de tempo de espera
- [ ] Dashboard analítico da fila

## Suporte

Para mais informações ou suporte, consulte:
- [Documentação Principal](../README.md)
- [Guia de API](API_GUIDE.md)
- [Issues do GitHub](https://github.com/Omni Care Software/MW.Code/issues)
