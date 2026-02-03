# 📝 Guia de Tradução de Código para Português

## 🎯 Objetivo
Este guia estabelece o padrão para traduzir comentários, documentação e mensagens do código Omni Care Software para português, mantendo os identificadores (classes, métodos, variáveis) em inglês.

## ✅ O Que Traduzir

### 1. Comentários de Código
**Traduzir:**
```csharp
// Calculate first check digit
var sum = 0;
```

**Para:**
```csharp
// Calcula o primeiro dígito verificador
var sum = 0;
```

### 2. Documentação XML (C#)
**Traduzir:**
```csharp
/// <summary>
/// Value Object representing a CPF
/// Validates CPF format and check digits
/// </summary>
public record Cpf { }
```

**Para:**
```csharp
/// <summary>
/// Objeto de Valor representando um CPF
/// Valida o formato do CPF e os dígitos verificadores
/// </summary>
public record Cpf { }
```

### 3. Mensagens de Exceção
**Traduzir:**
```csharp
throw new ArgumentException("Name cannot be empty", nameof(name));
throw new InvalidOperationException("Only scheduled appointments can be confirmed");
```

**Para:**
```csharp
throw new ArgumentException("O nome não pode estar vazio", nameof(name));
throw new InvalidOperationException("Apenas agendamentos marcados podem ser confirmados");
```

### 4. Comentários TypeScript/JavaScript
**Traduzir:**
```typescript
// Timer already running
if (this.timerSubscription) {
  return;
}
```

**Para:**
```typescript
// Cronômetro já está em execução
if (this.timerSubscription) {
  return;
}
```

### 5. Console.log e Mensagens de Debug
**Traduzir:**
```typescript
console.error('Error loading appointment:', error);
this.errorMessage.set('Failed to load');
```

**Para:**
```typescript
console.error('Erro ao carregar agendamento:', error);
this.errorMessage.set('Falha ao carregar');
```

## ❌ O Que NÃO Traduzir

### 1. Nomes de Classes
**Manter em Inglês:**
```csharp
public class Patient { }  // ✅ Correto
public class Paciente { } // ❌ Incorreto
```

### 2. Nomes de Métodos
**Manter em Inglês:**
```csharp
public void Confirm() { }      // ✅ Correto
public void Confirmar() { }    // ❌ Incorreto
```

### 3. Nomes de Propriedades
**Manter em Inglês:**
```csharp
public string PatientId { get; set; }    // ✅ Correto
public string IdPaciente { get; set; }   // ❌ Incorreto
```

### 4. Nomes de Variáveis
**Manter em Inglês:**
```csharp
var patientId = Guid.NewGuid();   // ✅ Correto
var idPaciente = Guid.NewGuid();  // ❌ Incorreto
```

### 5. Palavras-chave da Linguagem
**Manter em Inglês:**
```csharp
public class, private, if, for, while, etc.  // ✅ Sempre em inglês
```

## 📚 Glossário de Termos Técnicos

### Termos Comuns
| Inglês | Português |
|--------|-----------|
| Value Object | Objeto de Valor |
| Entity | Entidade |
| Repository | Repositório |
| Service | Serviço |
| Controller | Controlador |
| cannot be empty | não pode estar vazio |
| must be positive | deve ser positivo |
| in the past | no passado |
| already exists | já existe |
| not found | não encontrado |
| invalid format | formato inválido |
| check digit | dígito verificador |
| scheduled | agendado / marcado |
| confirmed | confirmado |
| completed | concluído |
| cancelled | cancelado |
| in progress | em andamento |

### Validações e Exceções
| Inglês | Português |
|--------|-----------|
| Name cannot be empty | O nome não pode estar vazio |
| Invalid email format | Formato de e-mail inválido |
| Date must be in the past | A data deve estar no passado |
| Duration must be positive | A duração deve ser positiva |
| Only X can be Y | Apenas X pode ser Y |
| Cannot cancel completed appointments | Não é possível cancelar agendamentos concluídos |
| Patient cannot be their own guardian | O paciente não pode ser seu próprio responsável |

## 🔧 Processo de Tradução

### Passo 1: Identificar Arquivos
```bash
# Listar arquivos C# com comentários
find src -name "*.cs" -exec grep -l "//\|///\|throw new" {} \;

# Listar arquivos TypeScript com comentários
find frontend -name "*.ts" -exec grep -l "//\|console" {} \;
```

### Passo 2: Traduzir o Arquivo
1. Abrir o arquivo
2. Traduzir todos os comentários (`//`, `/* */`, `///`)
3. Traduzir todas as mensagens de exceção
4. **NÃO** alterar nomes de classes, métodos ou variáveis

### Passo 3: Atualizar Testes
Se houver testes que verificam mensagens de erro:
1. Localizar o arquivo de teste correspondente
2. Atualizar os `Assert.Equal()` com as mensagens em português
3. Exemplo:
```csharp
// Antes
Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);

// Depois
Assert.Equal("O nome não pode estar vazio (Parameter 'name')", exception.Message);
```

### Passo 4: Validar
```bash
# Build do projeto
dotnet build Omni Care Software.sln

# Executar testes
dotnet test

# Ou executar testes específicos
dotnet test --filter "FullyQualifiedName~NomeDoTeste"
```

### Passo 5: Commit
```bash
git add .
git commit -m "Traduzir comentários e mensagens de erro em [NomeDoArquivo]"
```

## 📁 Arquivos Prioritários para Tradução

### Domain Layer (Alta Prioridade)
```
src/MedicSoft.Domain/
├── Entities/
│   ├── ✅ Patient.cs (Concluído)
│   ├── ✅ Appointment.cs (Concluído)
│   ├── ⬜ Clinic.cs
│   ├── ⬜ User.cs
│   ├── ⬜ MedicalRecord.cs
│   ├── ⬜ Payment.cs
│   └── ... (23 entities restantes)
└── ValueObjects/
    ├── ✅ Crm.cs (Concluído)
    ├── ✅ Cpf.cs (Concluído)
    ├── ✅ Cnpj.cs (Concluído)
    ├── ✅ Email.cs (Concluído)
    ├── ✅ Phone.cs (Concluído)
    └── ✅ Address.cs (Concluído)
```

### Application Layer (Média Prioridade)
```
src/MedicSoft.Application/
├── Services/
│   ├── ⬜ AuthService.cs
│   ├── ⬜ UserService.cs
│   ├── ⬜ AppointmentService.cs
│   └── ... (~20 services)
└── Queries/
    └── ... (arquivos de query)
```

### API Layer (Média Prioridade)
```
src/MedicSoft.Api/
└── Controllers/
    ├── ⬜ PatientsController.cs
    ├── ⬜ AppointmentsController.cs
    ├── ⬜ AuthController.cs
    └── ... (~10 controllers)
```

### Frontend (Baixa Prioridade)
```
frontend/
├── medicwarehouse-app/
│   └── src/app/
│       ├── pages/
│       │   ├── ✅ attendance/attendance.ts (Concluído)
│       │   └── ... (~50 componentes)
│       └── services/
│           └── ... (~10 services)
└── mw-site/
    └── ... (componentes do site)
```

## ✨ Exemplos Completos

### Exemplo 1: Entity Completa
```csharp
namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Entidade representando uma clínica médica
    /// </summary>
    public class Clinic : BaseEntity
    {
        // Propriedades de navegação
        public Owner Owner { get; private set; }
        
        private Clinic() 
        { 
            // Construtor do EF - avisos de nulabilidade suprimidos
        }
        
        public Clinic(string name, Cnpj cnpj, string tenantId) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome não pode estar vazio", nameof(name));
            
            // Valida o CNPJ
            if (cnpj == null)
                throw new ArgumentNullException(nameof(cnpj));
            
            Name = name.Trim();
            Cnpj = cnpj;
        }
    }
}
```

### Exemplo 2: Service com Comentários
```csharp
public class AppointmentService : IAppointmentService
{
    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, string tenantId)
    {
        // Valida se o paciente existe
        var patient = await _patientRepository.GetByIdAsync(dto.PatientId, tenantId);
        if (patient == null)
            throw new NotFoundException("Paciente não encontrado");
        
        // Cria o agendamento
        var appointment = new Appointment(
            dto.PatientId, 
            dto.ClinicId, 
            dto.ScheduledDate,
            dto.ScheduledTime,
            dto.DurationMinutes,
            dto.Type,
            tenantId
        );
        
        // Salva no banco
        await _repository.AddAsync(appointment);
        
        return appointment.ToDto();
    }
}
```

## 🎓 Dicas Importantes

1. **Consistência**: Use sempre os mesmos termos para as mesmas mensagens
2. **Naturalidade**: Traduza para um português natural, não literal
3. **Contexto**: Considere o contexto da mensagem ao traduzir
4. **Testes**: Sempre execute os testes após traduzir
5. **Commits**: Faça commits pequenos e focados por arquivo ou grupo de arquivos relacionados

## 📞 Suporte

Se tiver dúvidas sobre a tradução de algum termo específico:
1. Consulte o glossário acima
2. Verifique arquivos já traduzidos para referência
3. Mantenha a consistência com traduções existentes

## 🏁 Status Atual

**Progresso Geral:**
- ✅ Value Objects: 6/6 (100%)
- ✅ Entities: 2/25 (8%)
- ⬜ Services: 0/20 (0%)
- ⬜ Controllers: 0/10 (0%)
- ⬜ Frontend: 1/59 (2%)

**Total de Testes:**
- ✅ 168/168 testes passando (100%)
