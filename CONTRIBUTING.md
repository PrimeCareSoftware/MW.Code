# 🤝 Guia de Contribuição - Omni Care Software

> **Bem-vindo!** Obrigado por considerar contribuir para o Omni Care Software.

---

## 📋 Índice

- [Código de Conduta](#código-de-conduta)
- [Como Posso Contribuir?](#como-posso-contribuir)
- [Processo de Desenvolvimento](#processo-de-desenvolvimento)
- [Padrões de Código](#padrões-de-código)
- [Commits e Pull Requests](#commits-e-pull-requests)
- [Testes](#testes)
- [Documentação](#documentação)
- [Primeiros Passos](#primeiros-passos)

---

## 📜 Código de Conduta

Este projeto adere a um Código de Conduta. Ao participar, você concorda em manter um ambiente respeitoso e acolhedor.

### Nossos Compromissos

- 🤝 Ser respeitoso e inclusivo
- 💬 Aceitar críticas construtivas
- 🎯 Focar no que é melhor para a comunidade
- 🌟 Mostrar empatia com outros membros

---

## 🚀 Como Posso Contribuir?

### 🐛 Reportando Bugs

Antes de criar um issue, verifique se já não existe um similar.

**Ao reportar um bug, inclua:**

- Descrição clara e concisa do problema
- Passos para reproduzir
- Comportamento esperado vs. observado
- Screenshots (se aplicável)
- Ambiente (SO, versão do .NET, navegador, etc.)
- Logs de erro

**Template de Bug:**

```markdown
## Descrição
[Descrição clara do problema]

## Passos para Reproduzir
1. Vá para '...'
2. Clique em '...'
3. Veja o erro

## Comportamento Esperado
[O que deveria acontecer]

## Comportamento Observado
[O que aconteceu]

## Ambiente
- SO: [ex: Ubuntu 22.04]
- .NET: [ex: 8.0.0]
- Navegador: [ex: Chrome 120]

## Logs
```
[Cole os logs aqui]
```
```

### ✨ Sugerindo Melhorias

**Ao sugerir uma melhoria, inclua:**

- Descrição clara da funcionalidade
- Por que seria útil
- Exemplos de uso
- Possível implementação (opcional)

### 💻 Contribuindo com Código

1. **Fork** o repositório
2. **Clone** seu fork
3. Crie uma **branch** para sua feature
4. Faça suas **mudanças**
5. **Teste** suas mudanças
6. **Commit** com mensagens claras
7. **Push** para seu fork
8. Abra um **Pull Request**

---

## 🔧 Processo de Desenvolvimento

### 1. Setup do Ambiente

```bash
# Clone seu fork
git clone https://github.com/seu-usuario/MW.Code.git
cd MW.Code

# Adicione o upstream
git remote add upstream https://github.com/Omni Care Software/MW.Code.git

# Configure .env
cp .env.example .env
# Edite .env com suas configurações

# Inicie o banco de dados
podman-compose up postgres -d

# Restaure dependências
dotnet restore

# Execute a API
cd src/MedicSoft.Api
dotnet run
```

**Ver guia completo:** [GUIA_INICIO_RAPIDO_LOCAL.md](docs/GUIA_INICIO_RAPIDO_LOCAL.md)

### 2. Criando uma Branch

```bash
# Atualize sua main
git checkout main
git pull upstream main

# Crie uma branch para sua feature
git checkout -b feature/nome-da-feature

# Ou para bugfix
git checkout -b fix/nome-do-bug
```

**Nomenclatura de Branches:**

- `feature/` - Novas funcionalidades
- `fix/` - Correções de bugs
- `docs/` - Documentação
- `refactor/` - Refatoração
- `test/` - Adição/melhoria de testes
- `chore/` - Tarefas de manutenção

### 3. Fazendo Mudanças

- Faça mudanças pequenas e focadas
- Siga os padrões de código do projeto
- Adicione testes para novas funcionalidades
- Atualize a documentação se necessário
- Execute os testes localmente

### 4. Testando

```bash
# Execute todos os testes
dotnet test

# Execute testes de uma categoria específica
dotnet test --filter "FullyQualifiedName~Patients"

# Execute com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### 5. Documentando

- Adicione comentários XML para APIs públicas
- Atualize README.md se necessário
- Adicione exemplos de uso
- Documente decisões de design

---

## 📏 Padrões de Código

### Backend (.NET)

#### Nomenclatura

```csharp
// Classes: PascalCase
public class PatientService { }

// Métodos: PascalCase
public void CreatePatient() { }

// Propriedades: PascalCase
public string Name { get; set; }

// Parâmetros e variáveis: camelCase
public void Method(string patientName) 
{
    var doctorId = Guid.NewGuid();
}

// Constantes: PascalCase
public const int MaxAttempts = 3;

// Private fields: _camelCase
private readonly IPatientRepository _patientRepository;
```

#### Princípios

- **DDD**: Entidades ricas com comportamento
- **SOLID**: Princípios de design orientado a objetos
- **CQRS**: Separação de comandos e queries
- **Clean Code**: Código limpo e legível

#### Exemplo de Entidade

```csharp
public class Patient : BaseEntity
{
    // Construtor privado para EF
    private Patient() { }

    // Factory method
    public static Patient Create(
        string name, 
        string document, 
        DateTime birthDate,
        string tenantId)
    {
        ValidateName(name);
        ValidateDocument(document);
        ValidateBirthDate(birthDate);

        return new Patient
        {
            Id = Guid.NewGuid(),
            Name = name,
            Document = document,
            BirthDate = birthDate,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    // Validações privadas
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required");
            
        if (name.Length < 3)
            throw new DomainException("Name must have at least 3 characters");
    }

    // Métodos de negócio
    public void UpdateContactInfo(string email, string phone)
    {
        ValidateEmail(email);
        Email = email;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### Frontend (Angular)

#### Nomenclatura

```typescript
// Classes e Interfaces: PascalCase
export class PatientService { }
export interface Patient { }

// Métodos e variáveis: camelCase
private patientService: PatientService;
public getPatients(): void { }

// Constantes: UPPER_SNAKE_CASE
export const API_BASE_URL = 'http://localhost:5000';

// Arquivos: kebab-case
patient-list.component.ts
patient.service.ts
```

#### Estrutura de Componentes

```typescript
@Component({
  selector: 'app-patient-list',
  templateUrl: './patient-list.component.html',
  styleUrls: ['./patient-list.component.scss']
})
export class PatientListComponent implements OnInit, OnDestroy {
  // Properties
  patients: Patient[] = [];
  loading = false;
  error: string | null = null;
  
  // Subscriptions
  private destroy$ = new Subject<void>();

  // Constructor com DI
  constructor(
    private patientService: PatientService,
    private router: Router
  ) { }

  // Lifecycle hooks
  ngOnInit(): void {
    this.loadPatients();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Public methods
  loadPatients(): void {
    this.loading = true;
    this.patientService.getPatients()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (patients) => {
          this.patients = patients;
          this.loading = false;
        },
        error: (error) => {
          this.error = error.message;
          this.loading = false;
        }
      });
  }

  // Private methods
  private formatDate(date: Date): string {
    return date.toLocaleDateString('pt-BR');
  }
}
```

---

## ✍️ Commits e Pull Requests

### Mensagens de Commit

Use o formato [Conventional Commits](https://www.conventionalcommits.org/):

```
<tipo>(<escopo>): <descrição>

[corpo opcional]

[rodapé opcional]
```

**Tipos:**

- `feat:` - Nova funcionalidade
- `fix:` - Correção de bug
- `docs:` - Documentação
- `style:` - Formatação (não afeta código)
- `refactor:` - Refatoração
- `test:` - Testes
- `chore:` - Manutenção

**Exemplos:**

```bash
feat(patients): add search by CPF functionality

fix(auth): correct JWT token expiration time

docs(readme): update installation instructions

refactor(services): extract common validation logic

test(appointments): add tests for cancellation flow
```

### Pull Requests

**Template de PR:**

```markdown
## Descrição
[Descrição clara das mudanças]

## Tipo de Mudança
- [ ] 🐛 Bug fix
- [ ] ✨ Nova feature
- [ ] 🔄 Refatoração
- [ ] 📝 Documentação
- [ ] 🧪 Testes

## Checklist
- [ ] Código segue os padrões do projeto
- [ ] Testes foram adicionados/atualizados
- [ ] Testes passam localmente
- [ ] Documentação foi atualizada
- [ ] Nenhum warning novo foi introduzido
- [ ] Commit messages seguem o padrão

## Como Testar
1. [Passo 1]
2. [Passo 2]
3. [Resultado esperado]

## Screenshots (se aplicável)
[Adicione screenshots]

## Issues Relacionadas
Closes #123
Fixes #456
```

---

## 🧪 Testes

### Princípios

- **AAA Pattern**: Arrange, Act, Assert
- **Isolamento**: Cada teste é independente
- **Nomenclatura Clara**: `Should_ExpectedBehavior_When_Condition`

### Exemplo de Teste

```csharp
public class PatientTests
{
    [Fact]
    public void Should_CreatePatient_When_ValidDataProvided()
    {
        // Arrange
        var name = "João Silva";
        var document = "12345678901";
        var birthDate = new DateTime(1990, 1, 1);
        var tenantId = "clinic-001";

        // Act
        var patient = Patient.Create(name, document, birthDate, tenantId);

        // Assert
        Assert.NotNull(patient);
        Assert.Equal(name, patient.Name);
        Assert.Equal(document, patient.Document);
        Assert.True(patient.IsActive);
    }

    [Fact]
    public void Should_ThrowException_When_NameIsEmpty()
    {
        // Arrange
        var name = "";
        var document = "12345678901";
        var birthDate = new DateTime(1990, 1, 1);
        var tenantId = "clinic-001";

        // Act & Assert
        Assert.Throws<DomainException>(() => 
            Patient.Create(name, document, birthDate, tenantId));
    }
}
```

### Cobertura de Testes

Mantemos **100% de cobertura** nas entidades de domínio.

```bash
# Gerar relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"

# Ver relatório
cd TestResults/{guid}/
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

---

## 📚 Documentação

### Comentários XML

```csharp
/// <summary>
/// Creates a new patient with the provided information.
/// </summary>
/// <param name="name">Full name of the patient</param>
/// <param name="document">CPF document (11 digits)</param>
/// <param name="birthDate">Date of birth</param>
/// <param name="tenantId">Clinic tenant identifier</param>
/// <returns>The created patient instance</returns>
/// <exception cref="DomainException">Thrown when validation fails</exception>
public static Patient Create(
    string name, 
    string document, 
    DateTime birthDate,
    string tenantId)
{
    // Implementation
}
```

### README de Funcionalidade

Ao adicionar uma funcionalidade complexa, crie um README:

```markdown
# Feature: Patient Search

## Visão Geral
Permite buscar pacientes por CPF, nome ou telefone.

## Endpoints
- `GET /api/patients/search?searchTerm={termo}`

## Regras de Negócio
- Busca deve ser case-insensitive
- Busca por CPF deve remover formatação
- Resultados limitados a 50 por página

## Testes
- [x] Busca por CPF exato
- [x] Busca por nome parcial
- [x] Busca por telefone
- [x] Paginação

## Exemplos
```bash
curl -X GET "http://localhost:5000/api/patients/search?searchTerm=João"
```
```

---

## 🎯 Primeiros Passos

### Boas Issues para Começar

Procure por issues marcadas com:

- `good first issue` - Boas para iniciantes
- `help wanted` - Precisamos de ajuda
- `documentation` - Melhorias na doc
- `bug` - Correções de bugs

### Encontrar uma Issue

1. Veja as [issues abertas](https://github.com/Omni Care Software/MW.Code/issues)
2. Escolha uma que você consiga resolver
3. Comente na issue dizendo que vai trabalhar nela
4. Aguarde aprovação do maintainer
5. Fork e comece a trabalhar!

### Pedir Ajuda

Não hesite em pedir ajuda:

- Comente na issue
- Entre no canal de discussões
- Envie um email para contato@omnicaresoftware.com

---

## 🔍 Review Process

### O que Esperamos

- ✅ Código limpo e legível
- ✅ Testes passando
- ✅ Documentação atualizada
- ✅ Segue os padrões do projeto
- ✅ Não quebra funcionalidades existentes

### O que Acontece Após o PR

1. **Automação**: CI/CD roda testes automaticamente
2. **Review**: Maintainer revisa o código
3. **Feedback**: Você pode receber solicitações de mudanças
4. **Aprovação**: Se tudo estiver OK, PR é aprovado
5. **Merge**: PR é merged para main

### Tempo de Review

- Issues simples: 1-2 dias
- Issues médias: 3-5 dias
- Issues complexas: 1-2 semanas

---

## 🏆 Reconhecimento

### Contribuidores

Todos os contribuidores são listados no README e no arquivo AUTHORS.

### Como Ganhar Destaque

- Contribuições consistentes
- Qualidade do código
- Ajuda na comunidade
- Revisão de PRs de outros
- Melhoria da documentação

---

## 📞 Contato

- **GitHub Issues:** [github.com/Omni Care Software/MW.Code/issues](https://github.com/Omni Care Software/MW.Code/issues)
- **Email:** contato@omnicaresoftware.com
- **Documentação:** [Índice Completo](docs/DOCUMENTATION_INDEX.md)

---

## 📖 Recursos Adicionais

- [README Principal](README.md)
- [Resumo Técnico Completo](docs/RESUMO_TECNICO_COMPLETO.md)
- [Guia de APIs](docs/GUIA_COMPLETO_APIs.md)
- [CHANGELOG](CHANGELOG.md)
- [Guia de Início Rápido](docs/GUIA_INICIO_RAPIDO_LOCAL.md)

---

**Obrigado por contribuir! 🎉**

Sua contribuição ajuda a tornar o Omni Care Software melhor para todos.
