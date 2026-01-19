# API Pública de Consulta de Clínicas e Agendamento

## Visão Geral

Esta documentação descreve a API pública para busca de clínicas e agendamento de consultas diretamente pelo site, sem necessidade de autenticação prévia. A funcionalidade foi implementada seguindo as melhores práticas de segurança e conformidade com a LGPD.

## Segurança e Conformidade LGPD

### Dados Expostos (Apenas Informações Públicas)

A API pública **NÃO EXPÕE** dados sensíveis. Apenas informações essenciais para contato e agendamento são retornadas:

✅ **Dados Públicos Retornados:**
- Nome da clínica
- Nome fantasia
- Telefone de contato
- E-mail de contato
- Endereço completo
- Horário de funcionamento
- Duração padrão das consultas
- Status de aceitação de novos pacientes

❌ **Dados Protegidos (NÃO Expostos):**
- CNPJ completo da clínica
- Dados financeiros
- Informações de proprietários
- Dados de pacientes
- Prontuários médicos
- Informações de faturamento

### Sanitização de Dados

Todos os dados de entrada são validados e sanitizados antes do processamento:

1. **CPF do Paciente:** Validado com algoritmo de verificação
2. **Email:** Validação de formato e domínio
3. **Telefone:** Normalização e validação de formato brasileiro
4. **Data de Nascimento:** Validação de idade mínima e formato
5. **Horário de Agendamento:** Validação de disponibilidade e horário comercial

## Endpoints da API

### 1. Buscar Clínicas

```http
GET /api/public/clinics/search
```

Retorna uma lista paginada de clínicas ativas.

**Parâmetros de Query:**
- `name` (opcional): Nome ou nome fantasia da clínica
- `city` (opcional): Cidade
- `state` (opcional): Estado (sigla, ex: SP)
- `pageNumber` (opcional): Número da página (padrão: 1)
- `pageSize` (opcional): Tamanho da página (padrão: 10, máx: 100)

**Exemplo de Requisição:**
```bash
curl -X GET "https://api.mwsistema.com.br/api/public/clinics/search?city=São Paulo&state=SP&pageSize=10"
```

**Resposta de Sucesso (200 OK):**
```json
{
  "clinics": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Clínica Saúde Total",
      "tradeName": "Saúde Total",
      "phone": "(11) 98765-4321",
      "email": "contato@saudetotal.com.br",
      "address": "Rua das Flores, 123, Centro, São Paulo - SP, 01000-000",
      "city": "São Paulo",
      "state": "SP",
      "openingTime": "08:00:00",
      "closingTime": "18:00:00",
      "appointmentDurationMinutes": 30,
      "isAcceptingNewPatients": true
    }
  ],
  "totalCount": 25,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

### 2. Detalhes de uma Clínica

```http
GET /api/public/clinics/{clinicId}
```

Retorna detalhes públicos de uma clínica específica.

**Parâmetros de Rota:**
- `clinicId`: GUID da clínica

**Exemplo de Requisição:**
```bash
curl -X GET "https://api.mwsistema.com.br/api/public/clinics/3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

**Resposta de Sucesso (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Clínica Saúde Total",
  "tradeName": "Saúde Total",
  "phone": "(11) 98765-4321",
  "email": "contato@saudetotal.com.br",
  "address": "Rua das Flores, 123, Centro, São Paulo - SP, 01000-000",
  "city": "São Paulo",
  "state": "SP",
  "openingTime": "08:00:00",
  "closingTime": "18:00:00",
  "appointmentDurationMinutes": 30,
  "isAcceptingNewPatients": true
}
```

**Resposta de Erro (404 Not Found):**
```json
{
  "error": "Clínica não encontrada."
}
```

### 3. Horários Disponíveis

```http
GET /api/public/clinics/{clinicId}/available-slots
```

Retorna horários disponíveis para agendamento em uma data específica.

**Parâmetros:**
- `clinicId` (rota): GUID da clínica
- `date` (query): Data desejada (formato: YYYY-MM-DD)
- `durationMinutes` (query, opcional): Duração em minutos (padrão: 30)

**Exemplo de Requisição:**
```bash
curl -X GET "https://api.mwsistema.com.br/api/public/clinics/3fa85f64-5717-4562-b3fc-2c963f66afa6/available-slots?date=2026-01-25&durationMinutes=30"
```

**Resposta de Sucesso (200 OK):**
```json
[
  {
    "date": "2026-01-25T00:00:00",
    "time": "08:00:00",
    "durationMinutes": 30,
    "isAvailable": true
  },
  {
    "date": "2026-01-25T00:00:00",
    "time": "08:30:00",
    "durationMinutes": 30,
    "isAvailable": true
  },
  {
    "date": "2026-01-25T00:00:00",
    "time": "09:00:00",
    "durationMinutes": 30,
    "isAvailable": true
  }
]
```

### 4. Criar Agendamento Público

```http
POST /api/public/clinics/appointments
```

Cria um novo agendamento sem necessidade de autenticação. Se o paciente não existir, será criado automaticamente.

**Body da Requisição:**
```json
{
  "clinicId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "scheduledDate": "2026-01-25",
  "scheduledTime": "08:00:00",
  "durationMinutes": 30,
  "patientName": "João da Silva",
  "patientCpf": "123.456.789-00",
  "patientBirthDate": "1990-05-15",
  "patientPhone": "(11) 98765-4321",
  "patientEmail": "joao.silva@email.com",
  "notes": "Primeira consulta - dor nas costas"
}
```

**Validações Aplicadas:**
- CPF: Formato e dígitos verificadores válidos
- Email: Formato RFC 5322 válido
- Data de Nascimento: Não pode ser futura
- Data do Agendamento: Não pode ser passada
- Horário: Dentro do horário de funcionamento da clínica
- Conflito: Verifica se o horário está disponível

**Resposta de Sucesso (201 Created):**
```json
{
  "appointmentId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "clinicId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "clinicName": "Clínica Saúde Total",
  "scheduledDate": "2026-01-25T00:00:00",
  "scheduledTime": "08:00:00",
  "status": "Agendado",
  "message": "Agendamento realizado com sucesso! Você receberá uma confirmação por e-mail em joao.silva@email.com. Consulta agendada para 25/01/2026 às 08:00."
}
```

**Resposta de Erro (400 Bad Request):**
```json
{
  "error": "Este horário não está mais disponível. Por favor, escolha outro horário."
}
```

## Fluxo de Agendamento Público

### 1. Usuário Busca Clínicas

1. Acessa página de busca: `/site/clinics`
2. Filtra por nome, cidade ou estado (opcional)
3. Visualiza lista de clínicas disponíveis

### 2. Seleciona Clínica e Horário

1. Clica em "Agendar Consulta"
2. Navega para `/site/clinics/{id}/schedule`
3. Seleciona data desejada
4. Sistema exibe horários disponíveis

### 3. Preenche Dados Pessoais

1. Informa nome completo
2. Informa CPF
3. Informa data de nascimento
4. Informa telefone de contato
5. Informa e-mail
6. Adiciona observações (opcional)

### 4. Confirma Agendamento

1. Revisa informações
2. Confirma agendamento
3. Recebe confirmação na tela
4. Recebe e-mail de confirmação (futuro)

### 5. Processamento Backend

**Se Paciente Existir (baseado no CPF):**
1. Busca paciente existente
2. Verifica vínculo com clínica
3. Cria vínculo se não existir
4. Cria agendamento

**Se Paciente Não Existir:**
1. Cria novo paciente com dados mínimos
2. Cria vínculo com clínica
3. Cria agendamento
4. Paciente poderá completar cadastro posteriormente

## Arquitetura da Solução

### Backend (C# / .NET 8)

```
📁 MedicSoft.Application/
├── 📁 DTOs/
│   └── PublicClinicDto.cs (DTOs públicos)
├── 📁 Queries/PublicClinics/
│   └── PublicClinicQueries.cs
├── 📁 Commands/PublicAppointments/
│   └── CreatePublicAppointmentCommand.cs
└── 📁 Handlers/
    ├── 📁 Queries/PublicClinics/
    │   ├── SearchPublicClinicsQueryHandler.cs
    │   └── GetPublicClinicQueryHandlers.cs
    └── 📁 Commands/PublicAppointments/
        └── CreatePublicAppointmentCommandHandler.cs

📁 MedicSoft.Api/
└── 📁 Controllers/
    └── PublicClinicsController.cs (sem [Authorize])

📁 MedicSoft.Domain/
├── 📁 Interfaces/
│   ├── IRepository.cs (+ GetByIdWithoutTenantAsync)
│   └── IClinicRepository.cs (+ SearchPublicClinicsAsync)
└── 📁 Entities/
    ├── Clinic.cs
    ├── Appointment.cs
    └── Patient.cs

📁 MedicSoft.Repository/
└── 📁 Repositories/
    ├── BaseRepository.cs
    └── ClinicRepository.cs
```

### Frontend (Angular 20)

```
📁 frontend/medicwarehouse-app/
├── 📁 src/app/services/
│   └── public-clinic.service.ts
└── 📁 src/app/pages/site/clinics/
    ├── clinic-search.ts
    ├── clinic-search.html
    └── clinic-search.scss
```

## Testes

### Testes Unitários Implementados

```csharp
// MedicSoft.Test/Handlers/Queries/PublicClinics/
SearchPublicClinicsQueryHandlerTests.cs
- Handle_ShouldReturnPaginatedClinics
- Handle_ShouldFilterByName
- Handle_ShouldExtractCityAndStateFromAddress
- Handle_ShouldReturnEmptyListWhenNoClinicsFound
```

### Testes de Segurança Recomendados

1. **Verificar que dados sensíveis não são expostos:**
   - CNPJ completo não deve ser retornado
   - Dados de outros pacientes não devem vazar
   - Informações financeiras não devem ser acessíveis

2. **Validação de Entrada:**
   - SQL Injection
   - XSS (Cross-Site Scripting)
   - CSRF (Cross-Site Request Forgery)

3. **Rate Limiting:**
   - Implementar limite de requisições por IP
   - Prevenir abuse da API pública

## Próximos Passos

### Funcionalidades Pendentes

- [ ] Componente de seleção de horários disponíveis
- [ ] Formulário completo de agendamento
- [ ] Notificação por e-mail após agendamento
- [ ] Notificação por WhatsApp (integração existente)
- [ ] Rate limiting na API pública
- [ ] Captcha para prevenir bots

### Melhorias Futuras

- [ ] Cache de clínicas para melhor performance
- [ ] Geolocalização para buscar clínicas próximas
- [ ] Avaliações e comentários de pacientes
- [ ] Fotos das clínicas
- [ ] Informações sobre especialidades médicas
- [ ] Integração com Google Maps
- [ ] Sistema de fila de espera

## Contribuindo

Ao contribuir com esta funcionalidade, certifique-se de:

1. **Manter conformidade LGPD:** Nunca expor dados sensíveis
2. **Validar todas as entradas:** Prevenir injeções e ataques
3. **Escrever testes:** Cobrir cenários de sucesso e erro
4. **Documentar mudanças:** Atualizar esta documentação
5. **Seguir padrões:** Manter consistência com o código existente

## Suporte

Para dúvidas ou problemas relacionados a esta funcionalidade:

- **Issues:** https://github.com/PrimeCareSoftware/MW.Code/issues
- **Documentação:** https://github.com/PrimeCareSoftware/MW.Code/tree/main/docs
- **E-mail:** suporte@primecaresoftware.com.br

---

**Última Atualização:** Janeiro 2026  
**Versão da API:** 1.0  
**Autor:** Equipe PrimeCare Software
