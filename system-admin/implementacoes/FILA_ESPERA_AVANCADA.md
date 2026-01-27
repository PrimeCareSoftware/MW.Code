# Sistema de Fila de Espera Avançado - Documentação Técnica

**Status:** Backend Implementado (Janeiro 2026)  
**Próximos Passos:** Frontend (Totem e Painel de TV)

## 📋 Visão Geral

Sistema completo de gestão de filas de espera com totem de autoatendimento, painel de TV em tempo real, sistema de priorização automática e notificações SMS.

## 🏗️ Arquitetura

### Backend (.NET 8)

#### Entidades de Domínio

**FilaEspera** (`MedicSoft.Domain.Entities.FilaEspera`)
- Representa uma fila de espera configurável
- Tipos: Geral, Por Especialidade, Por Médico, Triagem
- Configurações de tempo médio de atendimento
- Controle de uso de prioridade e agendamento

**SenhaFila** (`MedicSoft.Domain.Entities.SenhaFila`)
- Representa uma senha gerada na fila
- Dados do paciente (nome, CPF, telefone)
- Número da senha com prefixo por prioridade
- Rastreamento de horários (entrada, chamada, atendimento, saída)
- Status: Aguardando, Chamando, Em Atendimento, Atendido, Não Compareceu, Cancelado
- Métricas de tempo de espera e atendimento

#### Enumerações

**TipoFila**
- `Geral`: Fila única para todos os atendimentos
- `PorEspecialidade`: Fila separada por especialidade médica
- `PorMedico`: Fila individual por médico
- `Triagem`: Fila de triagem inicial

**PrioridadeAtendimento**
- `Normal`: Atendimento padrão (Prefixo: N)
- `Idoso`: +60 anos (Prefixo: I)
- `Gestante`: Gestantes (Prefixo: G)
- `Deficiente`: Pessoas com deficiência (Prefixo: D)
- `Crianca`: < 2 anos (Prefixo: C)
- `Urgencia`: Casos urgentes (Prefixo: U)

**StatusSenha**
- `Aguardando`: Senha gerada, aguardando chamada
- `Chamando`: Senha sendo chamada
- `EmAtendimento`: Paciente em atendimento
- `Atendido`: Atendimento concluído
- `NaoCompareceu`: Paciente não compareceu após 3 chamadas
- `Cancelado`: Senha cancelada

#### Repositories

**IFilaEsperaRepository** / **FilaEsperaRepository**
- Gerenciamento de filas
- Consulta de filas ativas por clínica
- CRUD completo

**ISenhaFilaRepository** / **SenhaFilaRepository**
- Gerenciamento de senhas
- Consulta de senhas por fila
- Obtenção de próxima senha (com priorização)
- Cálculo de posição na fila
- Consulta por número de senha

#### Services

**IFilaService** / **FilaService**
- Criação e gestão de filas
- Geração automática de senhas
- Determinação de prioridade por idade
- Chamada de próxima senha
- Controle de ciclo de atendimento
- Cálculo de tempo estimado de espera
- Consulta de senhas

Métodos principais:
- `GerarSenhaAsync()`: Gera nova senha com priorização automática
- `ChamarProximaSenhaAsync()`: Chama próxima senha da fila
- `IniciarAtendimentoAsync()`: Inicia atendimento
- `FinalizarAtendimentoAsync()`: Finaliza atendimento
- `CalcularTempoEsperaAsync()`: Calcula tempo estimado
- `ObterPosicaoNaFilaAsync()`: Retorna posição na fila

#### SignalR Hub

**FilaHub** (`MedicSoft.Api.Hubs.FilaHub`)
- Comunicação em tempo real com clientes
- Grupos por fila (`fila_{filaId}`)
- Eventos:
  - `NovaSenha`: Nova senha gerada
  - `ChamarSenha`: Senha sendo chamada
  - `SenhaEmAtendimento`: Atendimento iniciado
  - `SenhaFinalizada`: Atendimento finalizado
  - `AtualizacaoFila`: Atualização geral da fila

Endpoint: `wss://api.domain.com/hubs/fila`

#### API REST

**FilaEsperaController** (`/api/FilaEspera`)

Endpoints principais:
- `POST /api/FilaEspera` - Criar nova fila
- `GET /api/FilaEspera/{filaId}` - Obter fila específica
- `GET /api/FilaEspera/{filaId}/summary` - Resumo completo da fila
- `POST /api/FilaEspera/{filaId}/senha` - Gerar nova senha (sem autenticação)
- `GET /api/FilaEspera/{filaId}/senha/{numeroSenha}` - Consultar senha (sem autenticação)
- `POST /api/FilaEspera/{filaId}/chamar` - Chamar próxima senha
- `PUT /api/FilaEspera/senha/{senhaId}/iniciar` - Iniciar atendimento
- `PUT /api/FilaEspera/senha/{senhaId}/finalizar` - Finalizar atendimento
- `DELETE /api/FilaEspera/senha/{senhaId}` - Cancelar senha

## 🎯 Fluxo de Uso

### 1. Geração de Senha (Totem ou Recepção)

```
POST /api/FilaEspera/{filaId}/senha?tenantId=xxx
{
  "nomePaciente": "João Silva",
  "cpf": "12345678900",
  "telefone": "11999999999",
  "dataNascimento": "1950-01-01",
  "isGestante": false,
  "isDeficiente": false,
  "especialidadeId": "guid",
  "agendamentoId": "guid"
}

Response:
{
  "id": "guid",
  "numeroSenha": "I001",
  "prioridade": "Idoso",
  "motivoPrioridade": "Idoso (+60 anos)",
  "posicaoNaFila": 3,
  "tempoEstimadoEspera": 45,
  "status": "Aguardando"
}
```

### 2. Atualização em Tempo Real (SignalR)

Clientes conectam ao hub:
```javascript
const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://api.domain.com/hubs/fila")
  .build();

await connection.start();
await connection.invoke("JoinFila", filaId);

connection.on("ChamarSenha", (data) => {
  // Atualizar painel de TV
  console.log(`Senha ${data.numeroSenha} - ${data.nomePaciente}`);
  console.log(`Consultório ${data.numeroConsultorio}`);
});
```

### 3. Chamada de Senha (Recepção/Médico)

```
POST /api/FilaEspera/{filaId}/chamar
{
  "medicoId": "guid",
  "numeroConsultorio": "1"
}

Response: 
{
  "id": "guid",
  "numeroSenha": "I001",
  "nomePaciente": "João Silva",
  "numeroConsultorio": "1",
  "status": "Chamando"
}
```

### 4. Ciclo de Atendimento

**Iniciar Atendimento:**
```
PUT /api/FilaEspera/senha/{senhaId}/iniciar
```

**Finalizar Atendimento:**
```
PUT /api/FilaEspera/senha/{senhaId}/finalizar
```

## 📊 Sistema de Priorização

O sistema determina automaticamente a prioridade baseado em:

1. **Idade** (via data de nascimento)
   - ≥ 60 anos → Idoso (Prioridade Alta)
   - < 2 anos → Criança (Prioridade Alta)
   
2. **Condição Especial**
   - Gestante → Prioridade Alta
   - Deficiente → Prioridade Alta
   - Urgência → Prioridade Máxima

3. **Ordem de Chamada**
   - Urgência > Deficiente > Gestante > Idoso > Criança > Normal
   - Dentro da mesma prioridade: ordem de chegada (FIFO)

## ⏱️ Cálculo de Tempo de Espera

Fórmula:
```
Tempo Estimado = (Senhas à Frente × Tempo Médio) × Fator Prioridade

Onde:
- Senhas à Frente: Quantidade de senhas com prioridade igual ou maior
- Tempo Médio: Configurado na fila (padrão: 15 minutos)
- Fator Prioridade: 1.0 para prioritários, 1.3 para normais
```

## 🔔 Notificações

### SMS (Planejado)
- Senha gerada com posição e tempo estimado
- Alerta quando estiver próximo (3 senhas antes)
- Chamada da senha

### SignalR (Implementado)
- Atualização em tempo real para painéis de TV
- Notificação para totems
- Sincronização entre dispositivos

## 🗄️ Banco de Dados

### Tabelas

**FilasEspera**
```sql
- Id (uuid)
- ClinicaId (uuid, FK)
- Nome (varchar 200)
- Tipo (int)
- Ativa (bool)
- TempoMedioAtendimento (int)
- UsaPrioridade (bool)
- UsaAgendamento (bool)
- TenantId (varchar 100)
- CreatedAt (timestamp)
- UpdatedAt (timestamp)
```

**SenhasFila**
```sql
- Id (uuid)
- FilaId (uuid, FK)
- PacienteId (uuid, FK nullable)
- NomePaciente (varchar 200)
- CpfPaciente (varchar 14)
- TelefonePaciente (varchar 20)
- NumeroSenha (varchar 20)
- DataHoraEntrada (timestamp)
- DataHoraChamada (timestamp nullable)
- DataHoraAtendimento (timestamp nullable)
- DataHoraSaida (timestamp nullable)
- Prioridade (int)
- MotivoPrioridade (varchar 200)
- Status (int)
- TentativasChamada (int)
- MedicoId (uuid, FK nullable)
- EspecialidadeId (uuid, FK nullable)
- ConsultorioId (uuid, FK nullable)
- NumeroConsultorio (varchar 50)
- AgendamentoId (uuid, FK nullable)
- TempoEsperaMinutos (int)
- TempoAtendimentoMinutos (int)
- TenantId (varchar 100)
- CreatedAt (timestamp)
- UpdatedAt (timestamp)
```

### Índices
- `IX_FilasEspera_ClinicaId`
- `IX_FilasEspera_ClinicaId_Ativa`
- `IX_SenhasFila_FilaId`
- `IX_SenhasFila_FilaId_Status`
- `IX_SenhasFila_FilaId_DataHoraEntrada`
- `IX_SenhasFila_NumeroSenha_FilaId`

## 🎨 Frontend (Planejado)

### Totem de Autoatendimento (Angular)
- Tela inicial com opções
- Fluxo de geração de senha
- Validação de CPF
- Check-in para agendamentos
- Impressão de comprovante

### Painel de TV (Angular)
- Layout full-screen
- Chamada atual em destaque
- Últimas 5 chamadas
- Fila de espera
- Text-to-Speech
- Animações e sons

## 📈 Analytics (Planejado)

**FilaMetricsDto**
- Total de atendimentos
- Tempo médio de espera
- Tempo médio de atendimento
- Taxa de não comparecimento
- Horário de pico
- Atendimentos por prioridade

## 🔐 Segurança

- Endpoints de consulta pública requerem tenantId
- Endpoints administrativos requerem autenticação JWT
- Isolamento de dados por tenant
- SignalR com autenticação opcional

## 🚀 Roadmap

### Fase 1 - Backend (✅ Concluído)
- [x] Entidades de domínio
- [x] Repositories
- [x] Services
- [x] SignalR Hub
- [x] API REST
- [x] Sistema de priorização
- [x] Cálculo de tempo de espera

### Fase 2 - Frontend (📋 Em Desenvolvimento)
- [ ] Totem de autoatendimento
- [ ] Painel de TV
- [ ] Integração SignalR
- [ ] Text-to-Speech
- [ ] Animações

### Fase 3 - Notificações e Analytics (📋 Planejado)
- [ ] Serviço de notificações SMS
- [ ] Analytics de fila
- [ ] Relatórios gerenciais
- [ ] Dashboard de métricas

## 📝 Referências

- Prompt de implementação: [14-fila-espera-avancada.md](../../Plano_Desenvolvimento/fase-4-analytics-otimizacao/14-fila-espera-avancada.md)
- Código fonte backend: `src/MedicSoft.Domain/Entities/`, `src/MedicSoft.Application/Services/`
- API Controller: `src/MedicSoft.Api/Controllers/FilaEsperaController.cs`
- SignalR Hub: `src/MedicSoft.Api/Hubs/FilaHub.cs`
