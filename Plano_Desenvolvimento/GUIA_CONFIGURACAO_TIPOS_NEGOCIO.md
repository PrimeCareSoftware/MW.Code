# ⚙️ Guia de Configuração de Tipos de Negócio
## Manual Completo para Configurar o Sistema por Especialidade

> **Data:** 26 de Janeiro de 2026  
> **Versão:** 1.0  
> **Público:** Administradores, Desenvolvedores, Implementadores  
> **Objetivo:** Guiar a configuração do sistema para diferentes modelos de negócio em saúde

---

## 📋 Sumário Executivo

Este guia detalha como configurar o PrimeCare Software para diferentes tipos de profissionais e clínicas de saúde. O sistema é altamente flexível e pode ser adaptado para:

- 🧠 Psicólogos (solo ou clínica)
- 🥗 Nutricionistas (solo ou clínica)
- 🦷 Dentistas (solo ou clínica)
- 💪 Fisioterapeutas (solo ou clínica)
- 🏥 Médicos (todas especialidades)
- 🐾 Veterinários
- 👥 Outros profissionais de saúde

---

## 🎯 Perfis de Negócio Pré-Configurados

### Visão Geral dos Perfis

| Perfil | Descrição | Profissionais Típicos | Configuração Padrão |
|--------|-----------|----------------------|---------------------|
| **Solo Online** | Profissional autônomo, 100% online, sem consultório | Psicólogos, coaches | CPF, 0 salas, teleatendimento |
| **Solo Híbrido** | Profissional autônomo, consultório compartilhado | Nutricionistas, fisioterapeutas | CPF/CNPJ, 0-1 sala, híbrido |
| **Clínica Pequena** | 2-5 profissionais, consultório próprio | Clínicas especializadas | CNPJ, 1-3 salas, presencial + online |
| **Clínica Média** | 6-20 profissionais, estrutura estabelecida | Clínicas multiespecialidade | CNPJ, 4-10 salas, completo |
| **Clínica Grande** | 20+ profissionais, operação corporativa | Hospitais, redes de clínicas | CNPJ, 10+ salas, enterprise |

---

## 🔧 Configuração Passo a Passo

### 1. Configuração para Psicólogo Autônomo (Solo Online)

#### Cenário Típico
- **Nome:** Julia Silva
- **Registro:** CRP 06/123456
- **Documento:** CPF (não tem CNPJ)
- **Local:** Atende de casa (100% online)
- **Atendimentos:** 15-20 sessões/semana
- **Duração:** 50 minutos por sessão
- **Preço:** R$ 150/sessão

#### Passo 1: Cadastro Inicial

```json
{
  "businessType": "SoloPractitioner",
  "specialty": "Psychology",
  "professionalInfo": {
    "fullName": "Julia Silva",
    "professionalId": "CRP 06/123456",
    "email": "julia.silva@email.com",
    "phone": "+5511999999999"
  },
  "documentInfo": {
    "type": "CPF",
    "number": "123.456.789-00"
  },
  "workLocation": {
    "hasPhysicalOffice": false,
    "numberOfRooms": 0,
    "address": null
  }
}
```

#### Passo 2: Ativar Features

```json
{
  "features": {
    // Recursos Clínicos
    "electronicPrescription": false,      // Psicólogos não prescrevem
    "labIntegration": false,              // Não aplicável
    "vaccineControl": false,              // Não aplicável
    "inventoryManagement": false,         // Não tem estoque
    
    // Recursos Administrativos
    "multiRoom": false,                   // Não tem sala física
    "receptionQueue": false,              // Não tem recepção
    "financialModule": true,              // Precisa controlar receitas
    "healthInsurance": false,             // Geralmente não atende convênios
    
    // Recursos de Atendimento
    "telemedicine": true,                 // ESSENCIAL
    "homeVisit": false,                   // Não faz atendimento domiciliar
    "groupSessions": true,                // Pode fazer terapia em grupo
    
    // Recursos de Marketing
    "publicProfile": true,                // Quer aparecer em busca
    "onlineBooking": true,                // Pacientes agendam online
    "patientReviews": true,               // Aceita avaliações
    
    // Recursos Avançados
    "biReports": false,                   // Não precisa BI complexo
    "apiAccess": false,                   // Não precisa API
    "whiteLabel": false                   // Não precisa marca própria
  }
}
```

#### Passo 3: Configurar Terminologia

```json
{
  "terminology": {
    "appointment": "Sessão",
    "professional": "Psicóloga",
    "registration": "CRP",
    "client": "Paciente",
    "mainDocument": "Prontuário Psicológico",
    "exitDocument": "Relatório"
  }
}
```

#### Passo 4: Configurar Agenda

```json
{
  "schedule": {
    "workingDays": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"],
    "workingHours": {
      "start": "09:00",
      "end": "18:00"
    },
    "appointmentDuration": 50,            // 50 minutos (padrão psicologia)
    "breakBetweenAppointments": 10,       // 10 min para anotações
    "allowEmergencySlots": false,         // Não há emergências em terapia
    "maxDailyAppointments": 8,            // Limite para evitar burnout
    "advanceBookingDays": 30,             // Agendar com até 30 dias de antecedência
    "cancellationPolicy": "24hours"       // Cancelamento até 24h antes
  }
}
```

#### Passo 5: Configurar Sala Virtual

```json
{
  "virtualRoom": {
    "slug": "julia.silva.psicologa",
    "subdomain": "juliasilva",            // juliasilva.primecare.com.br
    "branding": {
      "logoUrl": "https://storage.../logo.png",
      "primaryColor": "#6B46C1",          // Roxo (calmo, terapêutico)
      "secondaryColor": "#9F7AEA",
      "welcomeMessage": "Olá! Seja bem-vindo(a) ao meu consultório virtual. Aguarde um momento que já vou atendê-lo(a). 🌸"
    },
    "waitingRoom": {
      "enabled": true,
      "backgroundMusic": true,
      "musicUrl": "https://storage.../relaxing-music.mp3",
      "maxWaitingTime": 15                // Avisar se passar de 15 min
    },
    "videoSettings": {
      "allowRecording": true,             // Com consentimento
      "allowScreenShare": true,
      "allowChat": true,
      "backgroundBlur": true              // Privacidade
    }
  }
}
```

#### Passo 6: Configurar Templates de Documentos

```json
{
  "documentTemplates": {
    "prontuario": {
      "template": "psychology/prontuario_sessao.html",
      "fields": [
        "dataHora",
        "motivoConsulta",
        "queixaPrincipal",
        "historico",
        "observacoesSessao",
        "intervencoes",
        "tarefasCasa",
        "proximaSessao"
      ]
    },
    "relatorio": {
      "template": "psychology/relatorio_psicologico.html",
      "fields": [
        "identificacaoPaciente",
        "motivoAvaliacao",
        "procedimentosUtilizados",
        "analiseResultados",
        "conclusao",
        "recomendacoes"
      ]
    }
  }
}
```

#### Passo 7: Configurar Financeiro

```json
{
  "financial": {
    "defaultPrice": 150.00,               // R$ 150 por sessão
    "acceptedPaymentMethods": [
      "PIX",
      "CreditCard",
      "DebitCard",
      "Cash"
    ],
    "enableInstallments": false,          // Não parcela sessões
    "issueReceipt": true,                 // Emite recibo (RPS)
    "autoGenerateInvoice": false,         // Não emite NF (é CPF)
    "taxationRegime": "SimplesNacional"   // Se tiver CNPJ
  }
}
```

#### Resultado Final

```
✅ Perfil criado: Julia Silva - Psicóloga
✅ Sala virtual: juliasilva.primecare.com.br
✅ Agenda configurada: 8 sessões/dia, 50 min cada
✅ Teleatendimento ativo
✅ Prontuário psicológico
✅ Recibos automáticos
✅ Trial de 30 dias: R$ 69/mês após trial
```

---

### 2. Configuração para Nutricionista Híbrida

#### Cenário Típico
- **Nome:** Pedro Santos
- **Registro:** CRN 3/45678
- **Documento:** CNPJ (MEI)
- **Local:** Consultório compartilhado 2x/semana + online 3x/semana
- **Atendimentos:** 25 consultas/semana (15 online, 10 presenciais)
- **Duração:** 40 minutos por consulta
- **Preço:** R$ 200/consulta inicial, R$ 120/retorno

#### Passo 1: Cadastro Inicial

```json
{
  "businessType": "SoloPractitioner",
  "specialty": "Nutrition",
  "professionalInfo": {
    "fullName": "Pedro Santos",
    "professionalId": "CRN 3/45678",
    "email": "pedro.santos@email.com",
    "phone": "+5511988888888"
  },
  "documentInfo": {
    "type": "CNPJ",
    "number": "12.345.678/0001-90"        // MEI
  },
  "workLocation": {
    "hasPhysicalOffice": true,
    "numberOfRooms": 1,                   // Sala compartilhada
    "address": "Rua das Flores, 123 - São Paulo/SP",
    "workingDaysAtOffice": ["Tuesday", "Thursday"] // Apenas 2 dias
  }
}
```

#### Passo 2: Ativar Features

```json
{
  "features": {
    // Recursos Clínicos
    "electronicPrescription": true,       // Planos alimentares = prescrições
    "labIntegration": true,               // Pedidos de exames (hemograma, etc.)
    "vaccineControl": false,
    "inventoryManagement": false,
    
    // Recursos Administrativos
    "multiRoom": false,                   // Só 1 sala
    "receptionQueue": false,              // Não tem recepção
    "financialModule": true,
    "healthInsurance": false,             // Poucos convênios cobrem nutrição
    
    // Recursos de Atendimento
    "telemedicine": true,                 // 60% das consultas
    "homeVisit": false,
    "groupSessions": true,                // Workshops de nutrição
    
    // Recursos de Marketing
    "publicProfile": true,
    "onlineBooking": true,
    "patientReviews": true,
    
    // Recursos Avançados
    "biReports": false,
    "apiAccess": false,
    "whiteLabel": false
  }
}
```

#### Passo 3: Configurar Agenda Híbrida

```json
{
  "schedule": {
    "workingDays": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"],
    "defaultLocation": "online",          // Padrão é online
    "locationByDay": {
      "Monday": "online",
      "Tuesday": "inPerson",              // Consultório
      "Wednesday": "online",
      "Thursday": "inPerson",             // Consultório
      "Friday": "online"
    },
    "workingHours": {
      "start": "08:00",
      "end": "19:00"
    },
    "appointmentDuration": 40,            // 40 min (nutrição)
    "breakBetweenAppointments": 20,       // 20 min para análise/registros
    "allowEmergencySlots": false,
    "maxDailyAppointments": 10
  }
}
```

#### Passo 4: Configurar Precificação Diferenciada

```json
{
  "pricing": {
    "consultationTypes": [
      {
        "type": "FirstConsultation",
        "name": "Consulta Inicial (Avaliação Nutricional Completa)",
        "duration": 60,
        "price": 200.00,
        "description": "Anamnese completa + plano alimentar inicial"
      },
      {
        "type": "FollowUp",
        "name": "Retorno (Acompanhamento)",
        "duration": 40,
        "price": 120.00,
        "description": "Avaliação de evolução + ajustes no plano"
      },
      {
        "type": "OnlineOnly",
        "name": "Consulta Online Express",
        "duration": 30,
        "price": 80.00,
        "description": "Apenas online, para dúvidas rápidas"
      }
    ],
    "enablePackages": true,               // Pacotes de consultas
    "packages": [
      {
        "name": "Pacote 3 Meses",
        "consultations": 6,               // 6 retornos
        "price": 600.00,                  // Economia de R$ 120
        "validityDays": 90
      },
      {
        "name": "Pacote 6 Meses",
        "consultations": 12,
        "price": 1100.00,                 // Economia de R$ 340
        "validityDays": 180
      }
    ]
  }
}
```

#### Passo 5: Configurar Templates Nutricionais

```json
{
  "documentTemplates": {
    "anamneseNutricional": {
      "template": "nutrition/anamnese_nutricional.html",
      "fields": [
        "dadosPessoais",
        "historicoSaude",
        "historicoFamiliar",
        "objetivos",
        "restricoesAlimentares",
        "rotinaDiaria",
        "atividadeFisica",
        "avaliacaoAntropometrica",
        "examesBioquimicos"
      ]
    },
    "planoAlimentar": {
      "template": "nutrition/plano_alimentar.html",
      "features": [
        "calculadoraCalorias",
        "distribuicaoMacronutrientes",
        "sugestoesRefeicoes",
        "listaCompras",
        "receitasAdaptadas",
        "suplementacao"
      ]
    },
    "evolucao": {
      "template": "nutrition/evolucao_nutricional.html",
      "fields": [
        "dataConsulta",
        "pesoAtual",
        "medidasCorporais",
        "percentualGordura",
        "aderenciaPlano",
        "dificuldadesRelatadas",
        "ajustesRealizados",
        "metasProximaConsulta"
      ]
    }
  }
}
```

#### Passo 6: Integrações Especiais

```json
{
  "integrations": {
    "foodDatabase": {
      "enabled": true,
      "source": "TACO",                   // Tabela Brasileira de Composição de Alimentos
      "allowCustomFoods": true
    },
    "mealPlanner": {
      "enabled": true,
      "generateWeeklyMenu": true,
      "considerRestrictions": true,       // Alergias, intolerâncias
      "calorieCalculation": true
    },
    "progressTracking": {
      "enabled": true,
      "trackWeight": true,
      "trackMeasurements": true,          // Cintura, quadril, etc.
      "trackPhotos": true,                // Fotos de progresso
      "generateCharts": true
    }
  }
}
```

---

### 3. Configuração para Clínica Odontológica (5 dentistas)

#### Cenário Típico
- **Nome:** OdontoSorrir Clínica
- **CNPJ:** 12.345.678/0001-00
- **Profissionais:** 5 dentistas
- **Salas:** 5 cadeiras odontológicas
- **Atendimentos:** 120 consultas/semana
- **Convênios:** Sim (TISS)

#### Passo 1: Cadastro da Clínica

```json
{
  "businessType": "SmallClinic",
  "specialty": "Dental",
  "clinicInfo": {
    "name": "OdontoSorrir Clínica Odontológica Ltda",
    "tradeName": "OdontoSorrir",
    "cnpj": "12.345.678/0001-00",
    "phone": "+5511977777777",
    "email": "contato@odontosorrirclinica.com.br",
    "address": "Av. Paulista, 1000 - São Paulo/SP",
    "subdomain": "odontosorrirclinica"
  },
  "structure": {
    "numberOfRooms": 5,
    "numberOfProfessionals": 5,
    "hasReception": true,
    "hasWaitingRoom": true,
    "hasXRayRoom": true
  }
}
```

#### Passo 2: Cadastrar Profissionais

```json
{
  "professionals": [
    {
      "name": "Dra. Ana Paula Silva",
      "cro": "CRO-SP 12345",
      "specialty": "Ortodontia",
      "workingDays": ["Monday", "Wednesday", "Friday"],
      "room": 1
    },
    {
      "name": "Dr. Carlos Eduardo Santos",
      "cro": "CRO-SP 23456",
      "specialty": "Implantodontia",
      "workingDays": ["Tuesday", "Thursday"],
      "room": 2
    },
    {
      "name": "Dra. Beatriz Costa",
      "cro": "CRO-SP 34567",
      "specialty": "Endodontia",
      "workingDays": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"],
      "room": 3
    },
    {
      "name": "Dr. Daniel Oliveira",
      "cro": "CRO-SP 45678",
      "specialty": "Periodontia",
      "workingDays": ["Monday", "Wednesday", "Friday"],
      "room": 4
    },
    {
      "name": "Dra. Eduarda Ferreira",
      "cro": "CRO-SP 56789",
      "specialty": "Dentística",
      "workingDays": ["Tuesday", "Thursday", "Saturday"],
      "room": 5
    }
  ]
}
```

#### Passo 3: Ativar Features de Clínica

```json
{
  "features": {
    // Recursos Clínicos
    "electronicPrescription": true,       // Receitas odontológicas
    "labIntegration": true,               // Laboratórios de prótese
    "vaccineControl": false,
    "inventoryManagement": true,          // Materiais odontológicos
    
    // Recursos Administrativos
    "multiRoom": true,                    // 5 cadeiras
    "receptionQueue": true,               // Fila de espera
    "financialModule": true,              // Controle financeiro completo
    "healthInsurance": true,              // Convênios odontológicos (TISS)
    
    // Recursos de Atendimento
    "telemedicine": false,                // Odontologia é presencial
    "homeVisit": false,
    "groupSessions": false,
    
    // Recursos de Marketing
    "publicProfile": true,
    "onlineBooking": true,
    "patientReviews": true,
    
    // Recursos Avançados
    "biReports": true,                    // Relatórios gerenciais
    "apiAccess": false,
    "whiteLabel": false
  }
}
```

#### Passo 4: Configurar Procedimentos e Preços

```json
{
  "procedures": [
    {
      "category": "Prevenção",
      "items": [
        { "code": "001", "name": "Consulta Odontológica", "price": 80.00, "duration": 30 },
        { "code": "002", "name": "Limpeza (Profilaxia)", "price": 150.00, "duration": 40 },
        { "code": "003", "name": "Aplicação de Flúor", "price": 60.00, "duration": 20 }
      ]
    },
    {
      "category": "Restaurações",
      "items": [
        { "code": "101", "name": "Restauração Resina 1 Face", "price": 180.00, "duration": 40 },
        { "code": "102", "name": "Restauração Resina 2 Faces", "price": 250.00, "duration": 50 },
        { "code": "103", "name": "Restauração Resina 3 Faces", "price": 320.00, "duration": 60 }
      ]
    },
    {
      "category": "Endodontia",
      "items": [
        { "code": "201", "name": "Tratamento de Canal 1 Raiz", "price": 800.00, "duration": 90 },
        { "code": "202", "name": "Tratamento de Canal 2 Raízes", "price": 1200.00, "duration": 120 },
        { "code": "203", "name": "Tratamento de Canal 3 Raízes", "price": 1500.00, "duration": 150 }
      ]
    },
    {
      "category": "Implantes",
      "items": [
        { "code": "301", "name": "Implante Dentário (Unitário)", "price": 2500.00, "duration": 120 },
        { "code": "302", "name": "Prótese sobre Implante", "price": 1800.00, "duration": 60 }
      ]
    }
  ],
  "installments": {
    "enabled": true,
    "maxInstallments": 12,
    "minValuePerInstallment": 100.00,
    "interestRate": 2.5                   // 2.5% ao mês
  }
}
```

#### Passo 5: Configurar TISS (Convênios)

```json
{
  "tiss": {
    "enabled": true,
    "operators": [
      {
        "name": "Bradesco Dental",
        "code": "00001",
        "registrationNumber": "12345",
        "requiresAuthorization": true,
        "authorizationTypes": ["online", "manual"]
      },
      {
        "name": "Amil Dental",
        "code": "00002",
        "registrationNumber": "23456",
        "requiresAuthorization": false
      }
    ],
    "xmlGeneration": {
      "version": "4.02.00",
      "autoSend": true,
      "batchFrequency": "daily"
    }
  }
}
```

#### Passo 6: Configurar Estoque de Materiais

```json
{
  "inventory": {
    "enabled": true,
    "categories": [
      {
        "name": "Anestésicos",
        "items": [
          { "name": "Articaína 4%", "unit": "tubete", "minStock": 50, "alertThreshold": 20 },
          { "name": "Lidocaína 2%", "unit": "tubete", "minStock": 100, "alertThreshold": 30 }
        ]
      },
      {
        "name": "Materiais Restauradores",
        "items": [
          { "name": "Resina A2", "unit": "seringa", "minStock": 10, "alertThreshold": 3 },
          { "name": "Resina A3", "unit": "seringa", "minStock": 10, "alertThreshold": 3 }
        ]
      }
    ],
    "autoOrdering": {
      "enabled": true,
      "suppliers": [
        { "name": "Dental Cremer", "email": "pedidos@dentalcremer.com.br" }
      ]
    }
  }
}
```

#### Passo 7: Configurar Odontograma

```json
{
  "odontogram": {
    "enabled": true,
    "notation": "FDI",                    // Sistema de numeração FDI (internacional)
    "features": {
      "markConditions": true,             // Cáries, restaurações, etc.
      "trackTreatments": true,            // Histórico de procedimentos
      "planTreatments": true,             // Plano de tratamento
      "generateBudget": true,             // Orçamento baseado no odontograma
      "beforeAfterPhotos": true           // Fotos antes/depois
    }
  }
}
```

---

## 📊 Tabela Comparativa de Configurações

| Feature | Psicólogo Solo | Nutricionista Híbrida | Clínica Odonto | Clínica Médica |
|---------|----------------|----------------------|----------------|----------------|
| **Documento** | CPF | CNPJ (MEI) | CNPJ | CNPJ |
| **N° Salas** | 0 | 0-1 | 5 | 10+ |
| **Teleatendimento** | ✅ Obrigatório | ✅ 60% | ❌ Não | ⚠️ Opcional |
| **Prescrição** | ❌ | ✅ Planos | ✅ Receitas | ✅ Receitas |
| **Convênios** | ❌ | ❌ | ✅ TISS | ✅ TISS |
| **Estoque** | ❌ | ❌ | ✅ Materiais | ✅ Medicamentos |
| **Fila Espera** | ❌ | ❌ | ✅ Recepção | ✅ Recepção |
| **Financeiro** | Simples | Simples | Completo | Completo |
| **BI/Relatórios** | ❌ | ❌ | ✅ | ✅ |
| **Preço Sugerido** | R$ 69/mês | R$ 89/mês | R$ 299/mês | R$ 499/mês |

---

## 🎨 Personalização Visual por Especialidade

### Paletas de Cores Recomendadas

#### Psicologia
```css
--primary-color: #6B46C1;      /* Roxo (calma, introspecção) */
--secondary-color: #9F7AEA;    /* Roxo claro */
--accent-color: #E9D8FD;       /* Lilás suave */
```

#### Nutrição
```css
--primary-color: #38A169;      /* Verde (saúde, vitalidade) */
--secondary-color: #68D391;    /* Verde claro */
--accent-color: #C6F6D5;       /* Verde menta */
```

#### Odontologia
```css
--primary-color: #3182CE;      /* Azul (confiança, limpeza) */
--secondary-color: #63B3ED;    /* Azul claro */
--accent-color: #BEE3F8;       /* Azul céu */
```

#### Fisioterapia
```css
--primary-color: #DD6B20;      /* Laranja (energia, movimento) */
--secondary-color: #ED8936;    /* Laranja claro */
--accent-color: #FEEBC8;       /* Pêssego */
```

#### Medicina Geral
```css
--primary-color: #2D3748;      /* Cinza escuro (profissionalismo) */
--secondary-color: #4A5568;    /* Cinza médio */
--accent-color: #E2E8F0;       /* Cinza claro */
```

---

## 🚀 Wizard de Configuração Rápida

### Fluxo Interativo (UI)

```
Passo 1: Qual o seu perfil?
┌────────────────────────────────────────┐
│ [ ] Profissional Autônomo (solo)      │
│ [ ] Dupla de Profissionais            │
│ [ ] Clínica Pequena (3-5 pessoas)     │
│ [ ] Clínica Média (6-20 pessoas)      │
│ [ ] Clínica Grande (20+ pessoas)      │
└────────────────────────────────────────┘

Passo 2: Qual a sua especialidade?
┌────────────────────────────────────────┐
│ [ ] Psicologia                         │
│ [ ] Nutrição                           │
│ [ ] Odontologia                        │
│ [ ] Fisioterapia                       │
│ [ ] Medicina                           │
│ [ ] Outro                              │
└────────────────────────────────────────┘

Passo 3: Como você atende?
┌────────────────────────────────────────┐
│ [ ] 100% Online (sem consultório)     │
│ [ ] 100% Presencial (consultório)     │
│ [ ] Híbrido (online + presencial)     │
│ [ ] Atendimento domiciliar            │
└────────────────────────────────────────┘

Passo 4: Você tem CNPJ?
┌────────────────────────────────────────┐
│ ( ) Sim, tenho CNPJ                    │
│ ( ) Não, trabalho com CPF              │
│ ( ) Ainda não, mas vou abrir           │
└────────────────────────────────────────┘

Passo 5: Configuração Automática
┌────────────────────────────────────────┐
│ ✅ Perfil configurado                  │
│ ✅ Features ativadas                   │
│ ✅ Agenda pré-configurada              │
│ ✅ Templates selecionados              │
│ ✅ Preços sugeridos                    │
│                                        │
│ [Revisar Configuração] [Começar!]     │
└────────────────────────────────────────┘
```

---

## 📞 Suporte e Recursos

### Central de Ajuda
- 📚 **Base de Conhecimento:** [help.primecare.com.br](https://help.primecare.com.br)
- 🎥 **Vídeos Tutoriais:** [youtube.com/primecare](https://youtube.com/primecare)
- 💬 **Chat:** Disponível 9h-18h (dias úteis)
- 📧 **Email:** suporte@primecare.com.br
- 📞 **WhatsApp:** +55 11 9 9999-9999

### Documentação Relacionada
- [PLANO_ADAPTACAO_MULTI_NEGOCIOS.md](./PLANO_ADAPTACAO_MULTI_NEGOCIOS.md)
- [FEATURE_FLAGS_SPECIFICATION.md](./FEATURE_FLAGS_SPECIFICATION.md)
- [TELEATENDIMENTO_PROFISSIONAIS_AUTONOMOS.md](./TELEATENDIMENTO_PROFISSIONAIS_AUTONOMOS.md)

---

> **Versão:** 1.0  
> **Data:** 26 de Janeiro de 2026  
> **Próxima Revisão:** Trimestral
