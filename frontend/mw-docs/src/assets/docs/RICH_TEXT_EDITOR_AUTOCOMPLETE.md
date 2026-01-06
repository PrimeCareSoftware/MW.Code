# 📝 Editor de Texto Rico com Autocomplete de Medicações e Exames

## Resumo

Este documento descreve a implementação do componente **RichTextEditor** que fornece funcionalidades de formatação de texto e autocomplete inteligente para medicações e exames médicos no formulário de atendimento.

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Funcionalidades](#funcionalidades)
3. [Arquitetura](#arquitetura)
4. [Backend - API](#backend---api)
5. [Frontend - Componentes](#frontend---componentes)
6. [Dados de Demonstração](#dados-de-demonstração)
7. [Guia de Uso](#guia-de-uso)
8. [Configurações](#configurações)
9. [API Reference](#api-reference)

---

## Visão Geral

O sistema de Editor de Texto Rico foi desenvolvido para melhorar a experiência do médico durante o preenchimento do prontuário, permitindo:

- **Formatação de texto**: Negrito, itálico, sublinhado, listas e títulos
- **Autocomplete de medicações**: Sugestões inteligentes ao digitar `@@`
- **Autocomplete de exames**: Sugestões inteligentes ao digitar `##`
- **Dados pré-cadastrados**: Base completa de 130+ medicações e 150+ exames em português brasileiro

### Atalhos de Autocomplete

| Gatilho | Funcionalidade | Exemplo |
|---------|----------------|---------|
| `@@` | Busca medicações | `@@dipi...` sugere Dipirona |
| `##` | Busca exames | `##hemo...` sugere Hemograma |

---

## Funcionalidades

### 1. Formatação de Texto

O editor suporta as seguintes formatações (estilo Markdown):

| Formato | Sintaxe | Atalho |
|---------|---------|--------|
| **Negrito** | `**texto**` | Ctrl+B |
| *Itálico* | `_texto_` | Ctrl+I |
| Sublinhado | `__texto__` | - |
| Lista com marcadores | `- item` | - |
| Lista numerada | `1. item` | - |
| Título | `## texto` | - |

### 2. Autocomplete de Medicações

Ao digitar `@@` seguido do nome da medicação, o sistema sugere medicações cadastradas:

```
Exemplo: @@dipi
Sugestões:
- Dipirona Sódica 500mg - Comprimido
- Dipirona Sódica 1g - Comprimido
- Dipirona Gotas 500mg/ml - Solução Oral
```

**Informações exibidas:**
- Nome comercial
- Nome genérico
- Dosagem
- Forma farmacêutica
- Via de administração

### 3. Autocomplete de Exames

Ao digitar `##` seguido do nome do exame, o sistema sugere exames do catálogo:

```
Exemplo: ##hemo
Sugestões:
- Hemograma Completo
- Hemoglobina Glicada (HbA1c)
- Hemocultura
```

**Informações exibidas:**
- Nome do exame
- Tipo de exame
- Categoria
- Instruções de preparo

### 4. Navegação por Teclado

| Tecla | Ação |
|-------|------|
| ↑ / ↓ | Navegar entre sugestões |
| Enter | Selecionar sugestão |
| Tab | Selecionar sugestão |
| Esc | Fechar autocomplete |

---

## Arquitetura

### Estrutura de Arquivos

```
Backend (.NET 8)
├── src/MedicSoft.Api/Controllers/
│   ├── MedicationsController.cs      # API de medicações
│   └── ExamCatalogController.cs      # API de catálogo de exames
├── src/MedicSoft.Application/DTOs/
│   ├── MedicationDto.cs              # DTOs de medicação
│   └── ExamCatalogDto.cs             # DTOs de exames
├── src/MedicSoft.Domain/Entities/
│   ├── Medication.cs                 # Entidade já existente
│   └── ExamCatalog.cs                # Nova entidade de catálogo
├── src/MedicSoft.Domain/Interfaces/
│   └── IExamCatalogRepository.cs     # Interface do repositório
└── src/MedicSoft.Repository/
    ├── Configurations/
    │   └── ExamCatalogConfiguration.cs
    └── Repositories/
        └── ExamCatalogRepository.cs

Frontend (Angular 20)
├── frontend/medicwarehouse-app/src/app/
│   ├── models/
│   │   ├── medication.model.ts       # Modelo de medicação
│   │   └── exam-catalog.model.ts     # Modelo de exame
│   ├── services/
│   │   ├── medication.ts             # Serviço de medicações
│   │   └── exam-catalog.ts           # Serviço de exames
│   ├── shared/rich-text-editor/
│   │   ├── rich-text-editor.ts       # Componente principal
│   │   ├── rich-text-editor.html     # Template
│   │   └── rich-text-editor.scss     # Estilos
│   └── pages/attendance/
│       ├── attendance.ts             # Integração no atendimento
│       └── attendance.html           # Uso do componente
```

### Fluxo de Dados

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  RichTextEditor │────▶│ MedicationService│────▶│ MedicationsAPI  │
│   Component     │     │ ExamCatalogSvc   │     │ ExamCatalogAPI  │
└─────────────────┘     └──────────────────┘     └─────────────────┘
         │                      │                        │
         │                      ▼                        ▼
         │              ┌──────────────────┐     ┌─────────────────┐
         │              │   HTTP Client    │     │   PostgreSQL    │
         │              │   (Angular)      │     │   Database      │
         │              └──────────────────┘     └─────────────────┘
         │
         ▼
┌─────────────────┐
│ Attendance Form │
│   (Prontuário)  │
└─────────────────┘
```

---

## Backend - API

### MedicationsController

Controller responsável pela gestão e busca de medicações.

#### Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/medications` | Lista todas as medicações |
| GET | `/api/medications/search?term={termo}` | Busca medicações para autocomplete |
| GET | `/api/medications/{id}` | Obtém medicação por ID |
| GET | `/api/medications/category/{category}` | Lista por categoria |
| POST | `/api/medications` | Cria nova medicação |
| PUT | `/api/medications/{id}` | Atualiza medicação |
| DELETE | `/api/medications/{id}` | Desativa medicação |

#### Exemplo de Busca

```http
GET /api/medications/search?term=dipi
Authorization: Bearer {token}
X-Tenant-Id: demo-clinic-001
```

**Resposta:**
```json
[
  {
    "id": "guid",
    "name": "Dipirona Sódica",
    "genericName": "Dipyrone",
    "dosage": "500mg",
    "pharmaceuticalForm": "Comprimido",
    "administrationRoute": "Oral",
    "displayText": "Dipirona Sódica 500mg - Comprimido"
  }
]
```

### ExamCatalogController

Controller responsável pela gestão do catálogo de exames.

#### Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/exam-catalog` | Lista todos os exames |
| GET | `/api/exam-catalog/search?term={termo}` | Busca exames para autocomplete |
| GET | `/api/exam-catalog/{id}` | Obtém exame por ID |
| GET | `/api/exam-catalog/type/{examType}` | Lista por tipo |
| GET | `/api/exam-catalog/category/{category}` | Lista por categoria |
| POST | `/api/exam-catalog` | Cria novo exame |
| PUT | `/api/exam-catalog/{id}` | Atualiza exame |
| DELETE | `/api/exam-catalog/{id}` | Desativa exame |

#### Exemplo de Busca

```http
GET /api/exam-catalog/search?term=hemo
Authorization: Bearer {token}
X-Tenant-Id: demo-clinic-001
```

**Resposta:**
```json
[
  {
    "id": "guid",
    "name": "Hemograma Completo",
    "examType": "Laboratory",
    "category": "Hematologia",
    "preparation": "Jejum de 4 horas",
    "displayText": "Hemograma Completo"
  }
]
```

---

## Frontend - Componentes

### RichTextEditor

Componente Angular standalone que implementa o editor de texto rico com autocomplete.

#### Inputs

| Input | Tipo | Padrão | Descrição |
|-------|------|--------|-----------|
| `placeholder` | string | `''` | Texto placeholder |
| `rows` | number | `4` | Número de linhas do textarea |
| `enableMedicationAutocomplete` | boolean | `true` | Habilita autocomplete de medicações |
| `enableExamAutocomplete` | boolean | `true` | Habilita autocomplete de exames |
| `medicationTrigger` | string | `'@@'` | Gatilho para medicações |
| `examTrigger` | string | `'##'` | Gatilho para exames |
| `label` | string | `''` | Rótulo do campo |
| `id` | string | `''` | ID do elemento |
| `minSearchLength` | number | `2` | Mínimo de caracteres para busca |
| `searchDebounceMs` | number | `300` | Debounce em ms |

#### Outputs

| Output | Tipo | Descrição |
|--------|------|-----------|
| `medicationSelected` | `EventEmitter<MedicationAutocomplete>` | Emitido ao selecionar medicação |
| `examSelected` | `EventEmitter<ExamAutocomplete>` | Emitido ao selecionar exame |

#### Exemplo de Uso

```html
<!-- Diagnóstico - Apenas formatação -->
<app-rich-text-editor
  formControlName="diagnosis"
  label="Diagnóstico"
  placeholder="Descreva o diagnóstico do paciente..."
  [rows]="4"
  [enableMedicationAutocomplete]="false"
  [enableExamAutocomplete]="false"
></app-rich-text-editor>

<!-- Prescrição - Com autocomplete de medicações -->
<app-rich-text-editor
  formControlName="prescription"
  label="Prescrição Médica"
  placeholder="Use @@ para buscar medicações"
  [rows]="8"
  [enableMedicationAutocomplete]="true"
  [enableExamAutocomplete]="false"
  (medicationSelected)="onMedicationSelected($event)"
></app-rich-text-editor>

<!-- Observações - Com autocomplete de medicações e exames -->
<app-rich-text-editor
  formControlName="notes"
  label="Observações Clínicas"
  placeholder="Use @@ para medicações e ## para exames"
  [rows]="4"
  [enableMedicationAutocomplete]="true"
  [enableExamAutocomplete]="true"
  (medicationSelected)="onMedicationSelected($event)"
  (examSelected)="onExamSelected($event)"
></app-rich-text-editor>
```

#### Integração com Reactive Forms

O componente implementa `ControlValueAccessor`, permitindo uso direto com `formControlName`:

```typescript
@Component({
  // ...
})
export class Attendance {
  medicalRecordForm = this.fb.group({
    diagnosis: [''],
    prescription: [''],
    notes: ['']
  });

  onMedicationSelected(medication: MedicationAutocomplete): void {
    console.log('Medicação selecionada:', medication);
    // Lógica adicional se necessário
  }

  onExamSelected(exam: ExamAutocomplete): void {
    console.log('Exame selecionado:', exam);
    // Lógica adicional se necessário
  }
}
```

---

## Dados de Demonstração

O sistema inclui dados de demonstração extensivos para teste e uso inicial.

### Medicações (130+ itens)

Categorias disponíveis:

| Categoria | Quantidade | Exemplos |
|-----------|------------|----------|
| Analgésicos | 10+ | Dipirona, Paracetamol, Tramadol |
| Antibióticos | 15+ | Amoxicilina, Azitromicina, Ciprofloxacino |
| Anti-inflamatórios | 10+ | Ibuprofeno, Nimesulida, Diclofenaco |
| Anti-hipertensivos | 12+ | Losartana, Enalapril, Anlodipino |
| Antidiabéticos | 10+ | Metformina, Glibenclamida, Insulina |
| Ansiolíticos | 6+ | Clonazepam, Alprazolam, Diazepam |
| Antidepressivos | 8+ | Sertralina, Escitalopram, Fluoxetina |
| Corticosteroides | 5+ | Prednisona, Dexametasona |
| Broncodilatadores | 5+ | Salbutamol, Formoterol |
| Vitaminas/Suplementos | 8+ | Vitamina D3, B12, Ômega 3 |
| Outros | 15+ | Omeprazol, Domperidona, etc. |

### Exames (150+ itens)

Categorias disponíveis:

| Tipo | Quantidade | Exemplos |
|------|------------|----------|
| Laboratoriais | 90+ | Hemograma, Glicemia, Colesterol, TSH |
| Imagem | 25+ | Raio-X, Tomografia, Ressonância |
| Ultrassonografia | 20+ | Abdome, Tireoide, Mama, Obstétrico |
| Cardíacos | 10+ | ECG, Ecocardiograma, Holter, MAPA |
| Endoscopia | 10+ | Endoscopia, Colonoscopia |
| Biópsia | 12+ | Pele, Mama, Tireoide, Próstata |
| Outros | 30+ | Espirometria, Audiometria, etc. |

### Carregando Dados de Demo

```bash
# Via API
POST /api/data-seeder/seed-demo

# Isso criará automaticamente:
# - 130+ medicações
# - 150+ exames no catálogo
# - Todos os dados associados à clínica demo
```

---

## Guia de Uso

### Fluxo de Atendimento com Editor Rico

1. **Acessar Atendimento**
   - Navegue para `/appointments/{id}/attendance`
   - O formulário de prontuário será exibido com os editores ricos

2. **Preencher Diagnóstico**
   - Use o campo "Diagnóstico"
   - Formate texto usando a barra de ferramentas
   - Sem autocomplete neste campo

3. **Prescrever Medicações**
   - No campo "Prescrição Médica"
   - Digite `@@` seguido do nome da medicação
   - Selecione da lista de sugestões
   - O nome completo será inserido automaticamente

4. **Adicionar Observações com Exames**
   - No campo "Observações Clínicas"
   - Use `@@` para medicações
   - Use `##` para exames
   - Combine ambos conforme necessário

### Exemplo de Prescrição

```
## Prescrição Médica

1. Dipirona Sódica 500mg - Comprimido
   - Tomar 1 comprimido de 6/6 horas se dor ou febre

2. Amoxicilina 500mg - Cápsula
   - Tomar 1 cápsula de 8/8 horas por 7 dias

- Repouso por 3 dias
- Retornar se não houver melhora
```

---

## Configurações

### Configuração do Backend

As medicações e exames são filtrados por `TenantId`, garantindo isolamento multi-tenant.

```csharp
// Program.cs - Registro dos repositórios
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<IExamCatalogRepository, ExamCatalogRepository>();
```

### Configuração do Frontend

```typescript
// environment.ts
export const environment = {
  apiUrl: 'http://localhost:5000/api',
  // ...
};

// O serviço automaticamente usa a URL configurada
```

### Parâmetros de Performance

```typescript
// Configurações do componente
minSearchLength: number = 2;     // Mínimo de caracteres para busca
searchDebounceMs: number = 300;  // Debounce para evitar muitas requisições

// Backend - Limite de resultados
.Take(20)  // Retorna no máximo 20 resultados
```

---

## API Reference

### Modelos TypeScript

```typescript
// medication.model.ts
export interface Medication {
  id: string;
  name: string;
  genericName?: string;
  manufacturer?: string;
  activeIngredient?: string;
  dosage: string;
  pharmaceuticalForm: string;
  concentration?: string;
  administrationRoute?: string;
  category: MedicationCategory;
  requiresPrescription: boolean;
  isControlled: boolean;
  anvisaRegistration?: string;
  barcode?: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface MedicationAutocomplete {
  id: string;
  name: string;
  genericName?: string;
  dosage: string;
  pharmaceuticalForm: string;
  administrationRoute?: string;
  displayText: string;
}

export enum MedicationCategory {
  Analgesic = 0,
  Antibiotic = 1,
  AntiInflammatory = 2,
  Antihypertensive = 3,
  Antihistamine = 4,
  Antidiabetic = 5,
  Antidepressant = 6,
  Anxiolytic = 7,
  Antacid = 8,
  Bronchodilator = 9,
  Diuretic = 10,
  Anticoagulant = 11,
  Corticosteroid = 12,
  Vitamin = 13,
  Supplement = 14,
  Vaccine = 15,
  Contraceptive = 16,
  Antifungal = 17,
  Antiviral = 18,
  Antiparasitic = 19,
  Other = 20
}

// exam-catalog.model.ts
export interface ExamCatalog {
  id: string;
  name: string;
  description?: string;
  examType: ExamType;
  category?: string;
  preparation?: string;
  synonyms?: string;
  tussCode?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface ExamAutocomplete {
  id: string;
  name: string;
  examType: ExamType;
  category?: string;
  preparation?: string;
  displayText: string;
}
```

### Modelos C# (Backend)

```csharp
// ExamCatalog.cs
public class ExamCatalog : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public ExamType ExamType { get; private set; }
    public string? Category { get; private set; }
    public string? Preparation { get; private set; }
    public string? Synonyms { get; private set; }
    public string? TussCode { get; private set; }
    public bool IsActive { get; private set; }
}

// DTOs
public class ExamAutocompleteDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ExamType ExamType { get; set; }
    public string? Category { get; set; }
    public string? Preparation { get; set; }
    public string DisplayText => Name;
}

public class MedicationAutocompleteDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? GenericName { get; set; }
    public string Dosage { get; set; }
    public string PharmaceuticalForm { get; set; }
    public string? AdministrationRoute { get; set; }
    public string DisplayText => $"{Name} {Dosage} - {PharmaceuticalForm}";
}
```

---

## Notas Técnicas

### Performance

- **Debounce**: Busca com debounce de 300ms para evitar requisições excessivas
- **Limite de Resultados**: Máximo de 20 resultados por busca
- **Busca Eficiente**: Uso de `EF.Functions.Like()` para busca eficiente no banco

### Segurança

- **Multi-tenant**: Dados filtrados por `TenantId` automaticamente
- **Autenticação**: Todos os endpoints requerem token JWT válido
- **Validação**: Inputs sanitizados e validados

### Acessibilidade

- **Navegação por Teclado**: Suporte completo para navegação via teclado
- **ARIA Labels**: Labels descritivos para leitores de tela
- **Responsividade**: Layout adaptável para diferentes tamanhos de tela

---

## Troubleshooting

### Autocomplete não aparece

1. Verifique se o gatilho está correto (`@@` ou `##`)
2. Confirme que digitou pelo menos 2 caracteres após o gatilho
3. Verifique se a API está acessível
4. Confirme que existem dados cadastrados

### Dados não carregam

1. Execute o seeder de dados demo:
   ```bash
   POST /api/data-seeder/seed-demo
   ```
2. Verifique a conexão com o banco de dados
3. Confirme o `TenantId` correto no header da requisição

### Formatação não funciona

1. Selecione o texto antes de aplicar formatação
2. Use os botões da barra de ferramentas
3. Verifique se não há erros no console

---

## Referências

- [Documentação Angular](https://angular.dev/)
- [Documentação .NET 8](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ANVISA - Medicamentos](https://www.gov.br/anvisa/pt-br)
- [TUSS - Terminologia Unificada](https://www.ans.gov.br/prestadores/tiss-troca-de-informacao-de-saude-suplementar)
