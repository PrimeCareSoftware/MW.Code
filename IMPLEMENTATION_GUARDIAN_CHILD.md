# Implementação: Relacionamento Guardian-Child (Responsável-Criança)

## 📋 Resumo Executivo

Implementação completa da regra de negócio que exige que pacientes menores de 18 anos tenham um responsável cadastrado. O sistema agora suporta vínculos familiares, facilitando o atendimento de crianças e permitindo cenários como uma mãe levando múltiplos filhos para consulta.

## ✅ Status: COMPLETO

- **Data de Conclusão**: 10 de Outubro de 2025
- **Commits**: 4 commits principais
- **Testes**: 558/558 passando (100%)
- **Arquivos Modificados**: 24
- **Linhas de Código**: ~1,500 adicionadas

## 🎯 Objetivos Alcançados

### Regras de Negócio Implementadas

1. ✅ **Validação de Idade Automática**
   - Sistema calcula idade baseado em data de nascimento
   - Determina automaticamente se paciente é criança (<18 anos)
   - Campo de responsável aparece dinamicamente para crianças

2. ✅ **Vínculo Guardian-Child**
   - Auto-relacionamento na tabela Patients
   - GuardianId (FK nullable) aponta para outro Patient
   - Validações impedem relacionamentos inválidos

3. ✅ **Cenário de Múltiplas Crianças**
   - Endpoint para listar filhos de um responsável
   - Facilita agendamento conjunto (ex: mãe + 2 filhos)
   - UI mostra claramente vínculos familiares

4. ✅ **Validações de Segurança**
   - Criança não pode ser responsável de outra criança
   - Paciente não pode ser responsável de si mesmo
   - Apenas adultos (≥18) podem ser responsáveis
   - Criança sem responsável gera validação obrigatória

## 🏗️ Arquitetura da Solução

### Camadas Modificadas

#### 1. Domain Layer
```
src/MedicSoft.Domain/Entities/Patient.cs
├── + GuardianId (Guid?)
├── + Guardian (Patient?)
├── + Children (List<Patient>)
├── + IsChild() : bool
├── + SetGuardian(Guid)
├── + RemoveGuardian()
├── + AddChild(Patient)
├── + RemoveChild(Guid)
└── + GetChildren() : IEnumerable<Patient>

src/MedicSoft.Domain/Interfaces/IPatientRepository.cs
└── + GetChildrenOfGuardianAsync(Guid, string) : Task<IEnumerable<Patient>>
```

#### 2. Infrastructure Layer
```
src/MedicSoft.Repository/Configurations/PatientConfiguration.cs
└── + Self-referencing FK: Guardian → Children

src/MedicSoft.Repository/Repositories/PatientRepository.cs
└── + GetChildrenOfGuardianAsync implementation

src/MedicSoft.Repository/Migrations/
└── + 20251010_AddGuardianChildRelationship.cs
```

#### 3. Application Layer
```
src/MedicSoft.Application/DTOs/PatientDto.cs
├── + IsChild : bool
├── + GuardianId : Guid?
└── + GuardianName : string?

src/MedicSoft.Application/Commands/Patients/
└── + LinkChildToGuardianCommand.cs

src/MedicSoft.Application/Queries/Patients/
└── + GetChildrenOfGuardianQuery.cs

src/MedicSoft.Application/Handlers/
├── + LinkChildToGuardianCommandHandler.cs
└── + GetChildrenOfGuardianQueryHandler.cs

src/MedicSoft.Application/Services/PatientService.cs
├── + LinkChildToGuardianAsync
└── + GetChildrenOfGuardianAsync

src/MedicSoft.Application/Mappings/MappingProfile.cs
├── + IsChild mapping
└── + GuardianName mapping
```

#### 4. API Layer
```
src/MedicSoft.Api/Controllers/PatientsController.cs
├── + POST /{childId}/link-guardian/{guardianId}
└── + GET /{guardianId}/children
```

#### 5. Frontend Layer
```
frontend/medicwarehouse-app/src/app/models/patient.model.ts
├── + isChild: boolean
├── + guardianId?: string
└── + guardianName?: string

frontend/medicwarehouse-app/src/app/services/patient.ts
├── + linkChildToGuardian(childId, guardianId)
├── + getChildren(guardianId)
└── + search(searchTerm)

frontend/medicwarehouse-app/src/app/pages/patients/
├── patient-form.ts: Lógica de busca e seleção de responsável
├── patient-form.html: Campo dinâmico de responsável
├── patient-form.scss: Estilos para guardian search
├── patient-list.html: Badge de criança e coluna de responsável
└── patient-list.scss: Estilos para badges
```

## 📊 Dados e Schema

### Estrutura do Banco de Dados

```sql
ALTER TABLE Patients
ADD GuardianId uniqueidentifier NULL;

CREATE NONCLUSTERED INDEX IX_Patients_GuardianId
ON Patients (GuardianId);

ALTER TABLE Patients
ADD CONSTRAINT FK_Patients_Patients_GuardianId
FOREIGN KEY (GuardianId) REFERENCES Patients(Id)
ON DELETE NO ACTION;
```

### Relacionamento

```
Patient (Guardian)
  Id: GUID-MAE
  Name: "Maria Silva"
  DateOfBirth: 1985-05-10 (40 anos)
  GuardianId: NULL
  
  ↓ (1:N)
  
Patient (Child 1)
  Id: GUID-FILHO1
  Name: "João Silva"
  DateOfBirth: 2015-03-15 (9 anos)
  GuardianId: GUID-MAE
  
Patient (Child 2)
  Id: GUID-FILHO2
  Name: "Ana Silva"
  DateOfBirth: 2017-08-20 (7 anos)
  GuardianId: GUID-MAE
```

## 🔌 API Endpoints

### 1. Criar Paciente com Responsável
```http
POST /api/patients
Content-Type: application/json

{
  "name": "João Silva",
  "document": "12345678901",
  "dateOfBirth": "2015-03-15",
  "gender": "M",
  "email": "joao@example.com",
  "phoneCountryCode": "+55",
  "phoneNumber": "11999999999",
  "address": { ... },
  "guardianId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}

Response: 201 Created
{
  "id": "...",
  "name": "João Silva",
  "age": 9,
  "isChild": true,
  "guardianId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "guardianName": "Maria Silva",
  ...
}
```

### 2. Vincular Criança a Responsável
```http
POST /api/patients/{childId}/link-guardian/{guardianId}

Response: 200 OK
{
  "success": true
}
```

### 3. Listar Filhos de um Responsável
```http
GET /api/patients/{guardianId}/children

Response: 200 OK
[
  {
    "id": "...",
    "name": "João Silva",
    "age": 9,
    "isChild": true,
    "guardianId": "...",
    "guardianName": "Maria Silva"
  },
  {
    "id": "...",
    "name": "Ana Silva",
    "age": 7,
    "isChild": true,
    "guardianId": "...",
    "guardianName": "Maria Silva"
  }
]
```

## 🖥️ Interface do Usuário

### Tela de Cadastro de Paciente

```
┌─────────────────────────────────────────────┐
│ Novo Paciente                    [Voltar]   │
├─────────────────────────────────────────────┤
│ Dados Pessoais                              │
│ ─────────────────                           │
│ Nome: [João Silva____________]              │
│ Data Nasc: [2015-03-15]  ← Sistema calcula │
│                             idade = 9 anos  │
│                                             │
│ ┌──────────────────────────────────────┐   │
│ │ 🧒 Responsável *                     │   │
│ │ [Maria Silva___] 🔍                  │   │
│ │                                      │   │
│ │ Resultados:                          │   │
│ │ ┌────────────────────────────────┐   │   │
│ │ │ Maria Silva - CPF: 123.456.789-01│   │
│ │ │ 40 anos                          │   │
│ │ └────────────────────────────────┘   │   │
│ │                                      │   │
│ │ ✓ Responsável: Maria Silva           │   │
│ └──────────────────────────────────────┘   │
│                                             │
│ [Cancelar]              [Salvar]            │
└─────────────────────────────────────────────┘
```

### Lista de Pacientes

```
┌─────────────────────────────────────────────────────────┐
│ Pacientes                      [+ Novo Paciente]        │
├─────────────────────────────────────────────────────────┤
│ Nome          │ CPF      │ Idade │ Responsável  │ Ações │
├─────────────────────────────────────────────────────────┤
│ Maria Silva   │ 123...   │ 40    │ -            │ ✏️ 🗑️ │
│ 🧒 João Silva │ 234...   │ 9     │ 👤 Maria     │ ✏️ 🗑️ │
│ 🧒 Ana Silva  │ 345...   │ 7     │ 👤 Maria     │ ✏️ 🗑️ │
└─────────────────────────────────────────────────────────┘
```

## 🧪 Testes Unitários

### Novos Testes Adicionados: 12

1. `IsChild_WhenUnder18_ReturnsTrue` ✅
2. `IsChild_When18OrOver_ReturnsFalse` ✅
3. `SetGuardian_WithValidGuardianId_SetsGuardian` ✅
4. `SetGuardian_WhenNotChild_ThrowsInvalidOperationException` ✅
5. `SetGuardian_WithEmptyGuid_ThrowsArgumentException` ✅
6. `SetGuardian_WithSelfId_ThrowsArgumentException` ✅
7. `RemoveGuardian_RemovesGuardianId` ✅
8. `AddChild_WithValidChild_AddsChild` ✅
9. `AddChild_WhenNotChild_ThrowsArgumentException` ✅
10. `AddChild_WithSelf_ThrowsArgumentException` ✅
11. `RemoveChild_RemovesChildFromCollection` ✅
12. `GetChildren_ReturnsOnlyActiveChildren` ✅

### Cobertura de Testes

```
Total de Testes: 558
Aprovados: 558 (100%)
Reprovados: 0
Tempo de Execução: ~4 segundos
```

## 📚 Documentação Atualizada

1. ✅ **BUSINESS_RULES.md**
   - Seção 1.1.1: Regras de Responsáveis para Crianças
   - Seção 6.1: Fluxo atualizado com validação de responsável
   - Seção 7: Novo fluxo de atendimento de crianças com responsável

2. ✅ **SCREENS_DOCUMENTATION.md**
   - Fluxo 4: Cadastro de Criança com Responsável
   - Fluxo 5: Atendimento de Múltiplas Crianças
   - Mockups atualizados com campo de responsável

3. ✅ **TECHNICAL_IMPLEMENTATION.md**
   - Estrutura de dados atualizada
   - Novos endpoints documentados
   - Scripts de migração incluídos

4. ✅ **README.md**
   - Endpoints da API atualizados
   - Schema do banco atualizado

5. ✅ **TEST_SUMMARY.md**
   - Estatísticas atualizadas (558 testes)
   - Exemplos dos novos testes
   - Cenários de erro expandidos

## 🚀 Deploy e Migração

### Passo 1: Executar Migração do Banco

```sql
-- Executar no banco de dados de produção
ALTER TABLE Patients ADD GuardianId uniqueidentifier NULL;
CREATE NONCLUSTERED INDEX IX_Patients_GuardianId ON Patients(GuardianId);
ALTER TABLE Patients ADD CONSTRAINT FK_Patients_Patients_GuardianId
  FOREIGN KEY (GuardianId) REFERENCES Patients(Id) ON DELETE NO ACTION;
```

### Passo 2: Identificar Crianças Sem Responsável

```sql
-- Listar crianças que precisam de responsável
SELECT Id, Name, Document, DateOfBirth, 
       DATEDIFF(YEAR, DateOfBirth, GETDATE()) as Age
FROM Patients
WHERE DATEDIFF(YEAR, DateOfBirth, GETDATE()) < 18
  AND GuardianId IS NULL
  AND IsActive = 1
ORDER BY Age DESC;
```

### Passo 3: Deploy da Aplicação

```bash
# Backend
cd src/MedicSoft.Api
dotnet publish -c Release
# Deploy para servidor/Azure/AWS

# Frontend
cd frontend/medicwarehouse-app
npm run build
# Deploy para servidor web/CDN
```

## 🎓 Cenários de Uso

### Cenário 1: Cadastro de Criança Nova

```
1. Recepcionista clica em "Novo Paciente"
2. Preenche nome e data de nascimento (ex: 2015-03-15)
3. Sistema calcula: idade = 9 anos → É criança
4. Campo "Responsável" aparece como obrigatório
5. Recepcionista busca por "Maria Silva" ou CPF
6. Sistema mostra resultados (apenas adultos)
7. Recepcionista seleciona a mãe
8. Completa demais dados e salva
9. Vínculo criado automaticamente
```

### Cenário 2: Mãe com Dois Filhos para Consulta

```
1. Mãe chega na recepção com João (9 anos) e Ana (7 anos)
2. Recepcionista busca cadastro da mãe (Maria Silva)
3. Clica em "Ver Filhos" ou usa endpoint GET /patients/{maeId}/children
4. Sistema lista: João e Ana
5. Recepcionista agenda:
   - João às 14:00
   - Ana às 14:30
6. Consultas próximas facilitam atendimento conjunto
7. Durante atendimento, médico vê que são irmãos
```

### Cenário 3: Criança Completa 18 Anos

```
1. Sistema continua mostrando GuardianId no banco
2. Propriedade IsChild() retorna false (idade >= 18)
3. Interface não mostra mais badge de criança
4. Histórico de quem foi o responsável é mantido
5. Paciente agora pode ser responsável de outros
```

## ⚠️ Considerações Importantes

### Validações de Negócio

1. **Criança sem CPF**: Se criança não tiver CPF próprio, usar documento do responsável com sufixo
2. **Email da Criança**: Pode usar email do responsável se criança não tiver
3. **Múltiplos Responsáveis**: Atualmente suporta apenas 1 responsável (pode ser extendido)
4. **Troca de Responsável**: Sistema permite atualizar GuardianId se necessário
5. **Responsável Inativo**: Se responsável for desativado, crianças permanecem visíveis

### Performance

1. **Índice no GuardianId**: Criado para queries rápidas
2. **Eager Loading**: Considerar incluir Guardian em queries frequentes
3. **Cache**: Frontend pode cachear lista de filhos para melhor UX

### Segurança

1. **Isolamento por Tenant**: GuardianId só funciona dentro do mesmo tenant
2. **Validação de Idade**: Feita no backend, não confia em frontend
3. **FK Restrict**: Impede deleção acidental de responsável com filhos vinculados

## 📈 Métricas de Sucesso

- ✅ 100% dos testes passando
- ✅ 0 erros de build
- ✅ Frontend compila sem warnings críticos
- ✅ Documentação completa e atualizada
- ✅ Migração testada e documentada
- ✅ Código revisado e seguindo padrões do projeto

## 🤝 Próximos Passos (Opcional)

1. **Múltiplos Responsáveis**: Permitir pai E mãe como responsáveis
2. **Notificações**: Enviar SMS/Email para responsável sobre consultas
3. **Relatórios**: Dashboard com estatísticas de atendimento familiar
4. **Histórico Compartilhado**: Opção de compartilhar histórico entre irmãos (com consentimento)
5. **Integração com Cartão SUS**: Vincular responsável do cartão SUS

---

**Status**: ✅ IMPLEMENTAÇÃO COMPLETA E TESTADA  
**Versão**: 1.0  
**Data**: 10 de Outubro de 2025  
**Desenvolvedor**: GitHub Copilot + igorleessa
