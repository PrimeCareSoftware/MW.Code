# 📖 Exemplos de Uso da API - CFM 1.821

> **Objetivo:** Fornecer exemplos práticos de como utilizar a API REST para criar prontuários conformes com a CFM 1.821/2007.

> **Data:** Janeiro 2026  
> **Versão:** 1.0  
> **Autenticação:** JWT Bearer Token

---

## 📋 Índice

1. [Autenticação](#autenticação)
2. [Criar Prontuário Médico](#criar-prontuário-médico)
3. [Adicionar Exame Clínico](#adicionar-exame-clínico)
4. [Adicionar Hipótese Diagnóstica](#adicionar-hipótese-diagnóstica)
5. [Adicionar Plano Terapêutico](#adicionar-plano-terapêutico)
6. [Registrar Consentimento Informado](#registrar-consentimento-informado)
7. [Consultar Prontuário Completo](#consultar-prontuário-completo)
8. [Atualizar Campos do Prontuário](#atualizar-campos-do-prontuário)
9. [Fluxo Completo de Atendimento](#fluxo-completo-de-atendimento)

---

## 🔐 Autenticação

Todas as requisições requerem autenticação JWT. Primeiro, obtenha um token:

### Login
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "dr.silva@clinica.com.br",
  "password": "SenhaSegura123!",
  "tenantId": "00000000-0000-0000-0000-000000000001"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "user-guid",
    "name": "Dr. João Silva",
    "email": "dr.silva@clinica.com.br",
    "role": "Doctor"
  }
}
```

### Usar o Token
Em todas as requisições subsequentes, inclua o header:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
X-Tenant-Id: 00000000-0000-0000-0000-000000000001
```

---

## 📝 Criar Prontuário Médico

Criar um novo prontuário com campos obrigatórios da CFM 1.821.

```bash
POST /api/medicalrecords
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "appointmentId": "appointment-guid",
  "patientId": "patient-guid",
  "consultationStartTime": "2026-01-04T10:30:00Z",
  "chiefComplaint": "Paciente relata dor no peito há 2 horas",
  "historyOfPresentIllness": "Paciente relata que há aproximadamente 2 horas começou a sentir dor torácica, tipo peso, localizada em região retroesternal, sem irradiação para membros superiores. Refere intensidade moderada (6/10 na escala de dor). Refere que a dor piora com esforço físico mínimo e melhora parcialmente em repouso. Nega dispneia, náuseas, vômitos ou sudorese fria associados.",
  "pastMedicalHistory": "Hipertensão Arterial Sistêmica há 10 anos em uso regular de Losartana 50mg/dia. Diabetes Mellitus tipo 2 há 5 anos controlado com Metformina 850mg 2x/dia. Cirurgia de apendicectomia em 2010 sem complicações.",
  "familyHistory": "Pai com infarto agudo do miocárdio aos 55 anos. Mãe com Diabetes Mellitus tipo 2. Irmão com asma.",
  "lifestyleHabits": "Ex-tabagista (parou há 2 anos, fumou 20 cigarros/dia por 15 anos). Etilista social (2 doses de bebida/semana). Sedentário. Dieta rica em gorduras saturadas e sal.",
  "currentMedications": "Losartana 50mg - 1 comprimido via oral 1x/dia pela manhã (Hipertensão). Metformina 850mg - 1 comprimido via oral 2x/dia (Diabetes). AAS 100mg - 1 comprimido via oral 1x/dia pela manhã (Prevenção cardiovascular)."
}
```

**Resposta (201 Created):**
```json
{
  "id": "medical-record-guid",
  "appointmentId": "appointment-guid",
  "patientId": "patient-guid",
  "consultationStartTime": "2026-01-04T10:30:00Z",
  "chiefComplaint": "Paciente relata dor no peito há 2 horas",
  "historyOfPresentIllness": "Paciente relata que há aproximadamente 2 horas...",
  "pastMedicalHistory": "Hipertensão Arterial Sistêmica há 10 anos...",
  "familyHistory": "Pai com infarto agudo do miocárdio aos 55 anos...",
  "lifestyleHabits": "Ex-tabagista (parou há 2 anos...",
  "currentMedications": "Losartana 50mg - 1 comprimido...",
  "isClosed": false,
  "createdAt": "2026-01-04T10:30:00Z",
  "updatedAt": "2026-01-04T10:30:00Z"
}
```

---

## 🩺 Adicionar Exame Clínico

Registrar sinais vitais e exame físico sistemático.

```bash
POST /api/clinicalexaminations
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "medicalRecordId": "medical-record-guid",
  "bloodPressureSystolic": 140,
  "bloodPressureDiastolic": 90,
  "heartRate": 85,
  "respiratoryRate": 16,
  "temperature": 36.5,
  "oxygenSaturation": 98,
  "systematicExamination": "Paciente em regular estado geral, consciente, orientado no tempo e espaço, levemente ansioso. Corado, hidratado, anictérico, acianótico. Aparelho cardiovascular: Bulhas rítmicas, normofonéticas em 2 tempos, sem sopros. Pulsos periféricos palpáveis e simétricos. Aparelho respiratório: Murmúrio vesicular fisiológico preservado bilateralmente, sem ruídos adventícios. Abdômen: Plano, flácido, indolor à palpação superficial e profunda, sem visceromegalias palpáveis. Ruídos hidroaéreos presentes. Membros inferiores: Sem edema, pulsos pedioso e tibial posterior palpáveis bilateralmente.",
  "generalState": "Regular estado geral, ansioso, hemodinamicamente estável"
}
```

**Resposta (201 Created):**
```json
{
  "id": "examination-guid",
  "medicalRecordId": "medical-record-guid",
  "bloodPressureSystolic": 140,
  "bloodPressureDiastolic": 90,
  "heartRate": 85,
  "respiratoryRate": 16,
  "temperature": 36.5,
  "oxygenSaturation": 98,
  "systematicExamination": "Paciente em regular estado geral...",
  "generalState": "Regular estado geral, ansioso, hemodinamicamente estável",
  "createdAt": "2026-01-04T10:35:00Z"
}
```

---

## 🔍 Adicionar Hipótese Diagnóstica

Registrar diagnóstico com código CID-10 válido.

### Diagnóstico Principal

```bash
POST /api/diagnostichypotheses
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "medicalRecordId": "medical-record-guid",
  "description": "Angina Instável",
  "icd10Code": "I20.0",
  "type": 1,
  "diagnosedAt": "2026-01-04T10:40:00Z"
}
```

**Valores para `type`:**
- `1` = Principal
- `2` = Secondary (Secundário)

**Resposta (201 Created):**
```json
{
  "id": "diagnosis-guid",
  "medicalRecordId": "medical-record-guid",
  "description": "Angina Instável",
  "icd10Code": "I20.0",
  "type": 1,
  "diagnosedAt": "2026-01-04T10:40:00Z",
  "createdAt": "2026-01-04T10:40:00Z"
}
```

### Diagnóstico Secundário

```bash
POST /api/diagnostichypotheses
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "medicalRecordId": "medical-record-guid",
  "description": "Hipertensão Essencial (Primária)",
  "icd10Code": "I10",
  "type": 2,
  "diagnosedAt": "2026-01-04T10:40:00Z"
}
```

---

## 💊 Adicionar Plano Terapêutico

Registrar tratamento completo proposto.

```bash
POST /api/therapeuticplans
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "medicalRecordId": "medical-record-guid",
  "treatment": "Internação hospitalar para investigação diagnóstica de síndrome coronariana aguda. Repouso absoluto no leito. Monitorização cardíaca contínua. Dieta hipossódica e hipogordurosa. Controle rigoroso de sinais vitais de 4/4 horas. Oxigenoterapia se necessário (manter SatO2 > 94%).",
  "medicationPrescription": "1. AAS 200mg - 1 comprimido via oral imediatamente e manter 100mg 1x/dia\n2. Clopidogrel 300mg - 1 dose de ataque via oral, manter 75mg 1x/dia\n3. Atorvastatina 80mg - 1 comprimido via oral 1x/dia à noite\n4. Enoxaparina 60mg - 1 aplicação subcutânea de 12/12 horas\n5. Captopril 25mg - 1 comprimido via oral de 8/8 horas (ajustar conforme PA)\n6. Isossorbida 5mg - 1 comprimido sublingual SOS (dor torácica)",
  "examRequests": "LABORATÓRIO:\n- Hemograma completo\n- Troponina I seriada (0h, 3h, 6h)\n- CK-MB seriada\n- Glicemia de jejum\n- Creatinina e ureia\n- Eletrólitos (Na, K)\n- Lipidograma completo\n- TSH e T4 livre\n\nIMAGEM:\n- ECG de 12 derivações (seriados a cada 6h)\n- Raio-X de tórax PA e perfil\n- Ecocardiograma transtorácico (urgência)\n- Cintilografia miocárdica de perfusão ou Cineangiocoronariografia (definir conforme evolução)",
  "referrals": "Encaminhamento urgente para cardiologista para avaliação e conduta. Avaliar necessidade de cateterismo cardíaco de urgência conforme evolução clínica e marcadores de necrose miocárdica.",
  "patientGuidance": "- Manter repouso absoluto até liberação médica\n- Avisar imediatamente em caso de: piora da dor torácica, falta de ar, palpitações, sudorese fria\n- Não suspender medicações prescritas\n- Dieta pobre em sal e gorduras\n- Evitar esforços físicos e atividades estressantes\n- Não fumar (absolutamente contraindicado)\n- Acompanhamento ambulatorial rigoroso após alta",
  "returnDate": "2026-01-11T10:00:00Z"
}
```

**Resposta (201 Created):**
```json
{
  "id": "plan-guid",
  "medicalRecordId": "medical-record-guid",
  "treatment": "Internação hospitalar para investigação...",
  "medicationPrescription": "1. AAS 200mg...",
  "examRequests": "LABORATÓRIO:\n- Hemograma completo...",
  "referrals": "Encaminhamento urgente para cardiologista...",
  "patientGuidance": "- Manter repouso absoluto...",
  "returnDate": "2026-01-11T10:00:00Z",
  "createdAt": "2026-01-04T10:45:00Z"
}
```

---

## ✍️ Registrar Consentimento Informado

Registrar termo de consentimento para procedimentos.

### 1. Criar Consentimento

```bash
POST /api/informedconsents
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "medicalRecordId": "medical-record-guid",
  "patientId": "patient-guid",
  "consentText": "CONSENTIMENTO INFORMADO PARA CATETERISMO CARDÍACO\n\nEu, José da Silva, CPF 123.456.789-00, declaro que fui informado(a) pelo Dr. João Silva, CRM 12345/SP, sobre:\n\n1. DIAGNÓSTICO: Angina Instável (CID I20.0), suspeita de Síndrome Coronariana Aguda.\n\n2. PROCEDIMENTO PROPOSTO: Cateterismo cardíaco (cineangiocoronariografia) com possibilidade de angioplastia coronariana com implante de stent se houver lesão obstrutiva significativa.\n\n3. RISCOS: Sangramento no local de punção, hematoma, pseudoaneurisma, fístula arteriovenosa, infecção, reações alérgicas ao contraste iodado, nefropatia induzida por contraste, arritmias cardíacas, infarto agudo do miocárdio, acidente vascular cerebral, dissecção ou perfuração coronariana, tamponamento cardíaco, necessidade de cirurgia de emergência, óbito (risco muito baixo, < 0.1%).\n\n4. BENEFÍCIOS: Diagnóstico preciso da anatomia coronariana e extensão da doença arterial coronariana. Possibilidade de tratamento definitivo imediato (angioplastia) se houver lesão passível de intervenção percutânea. Alívio dos sintomas e redução do risco de infarto.\n\n5. ALTERNATIVAS: Tratamento clínico otimizado com medicamentos (menos definitivo). Cirurgia de revascularização miocárdica (mais invasiva). Testes não invasivos (menos precisos).\n\nDeclaro que:\n- Recebi explicações claras sobre o procedimento, riscos, benefícios e alternativas\n- Tive a oportunidade de fazer perguntas e todas foram respondidas satisfatoriamente\n- Compreendi que o procedimento será realizado por equipe médica qualificada\n- Fui informado que posso retirar este consentimento a qualquer momento antes do procedimento\n- Autorizo a realização do procedimento proposto e procedimentos adicionais que se façam necessários\n\nData: 04/01/2026"
}
```

**Resposta (201 Created):**
```json
{
  "id": "consent-guid",
  "medicalRecordId": "medical-record-guid",
  "patientId": "patient-guid",
  "consentText": "CONSENTIMENTO INFORMADO PARA CATETERISMO CARDÍACO...",
  "isAccepted": false,
  "acceptedAt": null,
  "ipAddress": null,
  "digitalSignature": null,
  "createdAt": "2026-01-04T10:50:00Z"
}
```

### 2. Aceitar Consentimento

```bash
POST /api/informedconsents/{consent-guid}/accept
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "ipAddress": "192.168.1.100",
  "digitalSignature": "BASE64_ENCODED_SIGNATURE_IMAGE_OR_HASH"
}
```

**Resposta (200 OK):**
```json
{
  "id": "consent-guid",
  "medicalRecordId": "medical-record-guid",
  "patientId": "patient-guid",
  "consentText": "CONSENTIMENTO INFORMADO PARA CATETERISMO CARDÍACO...",
  "isAccepted": true,
  "acceptedAt": "2026-01-04T10:55:00Z",
  "ipAddress": "192.168.1.100",
  "digitalSignature": "BASE64_ENCODED_SIGNATURE_IMAGE_OR_HASH",
  "createdAt": "2026-01-04T10:50:00Z",
  "updatedAt": "2026-01-04T10:55:00Z"
}
```

---

## 📖 Consultar Prontuário Completo

Buscar prontuário com todas as entidades relacionadas.

```bash
GET /api/medicalrecords/{medical-record-guid}
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
```

**Resposta (200 OK):**
```json
{
  "id": "medical-record-guid",
  "appointmentId": "appointment-guid",
  "patientId": "patient-guid",
  "consultationStartTime": "2026-01-04T10:30:00Z",
  "consultationEndTime": "2026-01-04T11:30:00Z",
  "chiefComplaint": "Paciente relata dor no peito há 2 horas",
  "historyOfPresentIllness": "Paciente relata que há aproximadamente 2 horas...",
  "pastMedicalHistory": "Hipertensão Arterial Sistêmica há 10 anos...",
  "familyHistory": "Pai com infarto agudo do miocárdio aos 55 anos...",
  "lifestyleHabits": "Ex-tabagista (parou há 2 anos...",
  "currentMedications": "Losartana 50mg - 1 comprimido...",
  "isClosed": false,
  "closedAt": null,
  "closedByUserId": null,
  "clinicalExaminations": [
    {
      "id": "examination-guid",
      "bloodPressureSystolic": 140,
      "bloodPressureDiastolic": 90,
      "heartRate": 85,
      "respiratoryRate": 16,
      "temperature": 36.5,
      "oxygenSaturation": 98,
      "systematicExamination": "Paciente em regular estado geral...",
      "generalState": "Regular estado geral, ansioso",
      "createdAt": "2026-01-04T10:35:00Z"
    }
  ],
  "diagnosticHypotheses": [
    {
      "id": "diagnosis-guid-1",
      "description": "Angina Instável",
      "icd10Code": "I20.0",
      "type": 1,
      "diagnosedAt": "2026-01-04T10:40:00Z",
      "createdAt": "2026-01-04T10:40:00Z"
    },
    {
      "id": "diagnosis-guid-2",
      "description": "Hipertensão Essencial (Primária)",
      "icd10Code": "I10",
      "type": 2,
      "diagnosedAt": "2026-01-04T10:40:00Z",
      "createdAt": "2026-01-04T10:41:00Z"
    }
  ],
  "therapeuticPlans": [
    {
      "id": "plan-guid",
      "treatment": "Internação hospitalar para investigação...",
      "medicationPrescription": "1. AAS 200mg...",
      "examRequests": "LABORATÓRIO:\n- Hemograma completo...",
      "referrals": "Encaminhamento urgente para cardiologista...",
      "patientGuidance": "- Manter repouso absoluto...",
      "returnDate": "2026-01-11T10:00:00Z",
      "createdAt": "2026-01-04T10:45:00Z"
    }
  ],
  "informedConsents": [
    {
      "id": "consent-guid",
      "consentText": "CONSENTIMENTO INFORMADO PARA CATETERISMO CARDÍACO...",
      "isAccepted": true,
      "acceptedAt": "2026-01-04T10:55:00Z",
      "ipAddress": "192.168.1.100",
      "digitalSignature": "BASE64_ENCODED_SIGNATURE_IMAGE_OR_HASH",
      "createdAt": "2026-01-04T10:50:00Z"
    }
  ],
  "createdAt": "2026-01-04T10:30:00Z",
  "updatedAt": "2026-01-04T10:55:00Z"
}
```

---

## ✏️ Atualizar Campos do Prontuário

Atualizar informações da anamnese antes de finalizar.

```bash
PUT /api/medicalrecords/{medical-record-guid}
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "chiefComplaint": "Paciente relata dor torácica intensa há 3 horas",
  "historyOfPresentIllness": "ATUALIZAÇÃO: Paciente relata que a dor torácica iniciou há 3 horas (e não 2 como inicialmente relatado)...",
  "pastMedicalHistory": "HAS há 10 anos. DM2 há 5 anos. Apendicectomia em 2010.",
  "consultationEndTime": "2026-01-04T11:30:00Z"
}
```

**Resposta (200 OK):**
```json
{
  "id": "medical-record-guid",
  "chiefComplaint": "Paciente relata dor torácica intensa há 3 horas",
  "historyOfPresentIllness": "ATUALIZAÇÃO: Paciente relata...",
  "consultationEndTime": "2026-01-04T11:30:00Z",
  "updatedAt": "2026-01-04T11:00:00Z"
}
```

**Nota:** Prontuários finalizados (`isClosed = true`) não podem ser editados.

---

## 🔄 Fluxo Completo de Atendimento

Sequência recomendada de chamadas API para um atendimento completo:

### 1. Login e Autenticação
```bash
POST /api/auth/login
```

### 2. Criar Prontuário
```bash
POST /api/medicalrecords
```
- Registrar queixa principal
- Registrar história da doença atual
- Registrar histórico médico, familiar, hábitos, medicações

### 3. Adicionar Exame Clínico
```bash
POST /api/clinicalexaminations
```
- Registrar sinais vitais
- Registrar exame físico sistemático

### 4. Adicionar Diagnósticos
```bash
POST /api/diagnostichypotheses (múltiplas vezes se necessário)
```
- Pelo menos 1 diagnóstico principal
- Diagnósticos secundários conforme necessário

### 5. Adicionar Plano Terapêutico
```bash
POST /api/therapeuticplans
```
- Tratamento/conduta
- Prescrição
- Exames
- Encaminhamentos
- Orientações
- Data de retorno

### 6. Consentimento (se aplicável)
```bash
POST /api/informedconsents
POST /api/informedconsents/{id}/accept
```

### 7. Revisar e Atualizar (se necessário)
```bash
PUT /api/medicalrecords/{id}
PUT /api/clinicalexaminations/{id}
PUT /api/diagnostichypotheses/{id}
PUT /api/therapeuticplans/{id}
```

### 8. Finalizar Prontuário
```bash
PUT /api/medicalrecords/{id}
{
  "isClosed": true,
  "closedAt": "2026-01-04T11:30:00Z",
  "closedByUserId": "user-guid"
}
```

---

## 📚 Códigos CID-10 Comuns

### Cardiologia
- `I10` - Hipertensão essencial (primária)
- `I20.0` - Angina instável
- `I20.8` - Outras formas de angina pectoris
- `I21.9` - Infarto agudo do miocárdio não especificado
- `I25.10` - Doença aterosclerótica do coração
- `I48.0` - Fibrilação atrial paroxística
- `I48.91` - Fibrilação atrial não especificada
- `I50.0` - Insuficiência cardíaca congestiva

### Endocrinologia
- `E11.9` - Diabetes mellitus tipo 2 sem complicações
- `E11.65` - Diabetes mellitus tipo 2 com hiperglicemia
- `E03.9` - Hipotireoidismo não especificado
- `E78.0` - Hipercolesterolemia pura
- `E78.5` - Hiperlipidemia não especificada

### Pneumologia
- `J18.9` - Pneumonia não especificada
- `J20.9` - Bronquite aguda não especificada
- `J45.0` - Asma predominantemente alérgica
- `J45.9` - Asma não especificada
- `J44.0` - DPOC com infecção respiratória aguda

### Gastroenterologia
- `K21.9` - Doença do refluxo gastroesofágico sem esofagite
- `K29.7` - Gastrite não especificada
- `K76.0` - Fígado gorduroso não alcoólico

### Ortopedia
- `M54.5` - Dor lombar baixa
- `M25.561` - Dor em articulação do joelho direito
- `M25.562` - Dor em articulação do joelho esquerdo

---

## 🔍 Validações Importantes

### Formato CID-10
- ✅ Válido: `A00`, `J20.9`, `Z99.01`
- ❌ Inválido: `A`, `A0`, `123`, `A00.1.2`

### Campos Obrigatórios
- `chiefComplaint`: mínimo 10 caracteres
- `historyOfPresentIllness`: mínimo 50 caracteres
- `systematicExamination`: mínimo 20 caracteres
- `treatment`: mínimo 20 caracteres
- `icd10Code`: formato válido
- `description` (diagnóstico): obrigatório

### Ranges de Sinais Vitais
- PA Sistólica: 50-300 mmHg
- PA Diastólica: 30-200 mmHg
- FC: 30-220 bpm
- FR: 8-60 irpm
- Temperatura: 32-45°C
- SatO2: 0-100%

---

## 🆘 Erros Comuns

### 400 Bad Request - Validação falhou
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "ChiefComplaint": [
      "Chief Complaint must be at least 10 characters long."
    ],
    "ICD10Code": [
      "ICD-10 code must match the format: Letter + 2 digits [+ dot + 1-2 digits]. Examples: A00, J20.9, Z99.01"
    ]
  }
}
```

### 401 Unauthorized - Token inválido ou expirado
```json
{
  "message": "Unauthorized"
}
```

### 404 Not Found - Recurso não encontrado
```json
{
  "message": "Medical record not found"
}
```

### 409 Conflict - Prontuário já finalizado
```json
{
  "message": "Cannot modify a closed medical record"
}
```

---

## 📖 Documentação Adicional

- [Swagger UI](http://localhost:5000/swagger) - Documentação interativa da API
- [Especificação CFM 1.821](ESPECIFICACAO_CFM_1821.md) - Requisitos completos
- [Guia do Médico](GUIA_MEDICO_CFM_1821.md) - Guia para profissionais de saúde
- [Implementação Backend](PHASE_3_BACKEND_COMPLETE.md) - Detalhes técnicos do backend
- [Implementação Frontend](PHASE_4_FRONTEND_COMPLETE.md) - Detalhes técnicos do frontend

---

**Documento Elaborado Por:** Equipe MedicWarehouse  
**Data de Atualização:** Janeiro 2026  
**Versão:** 1.0  
**Status:** Oficial
