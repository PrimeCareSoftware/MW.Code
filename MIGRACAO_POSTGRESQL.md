# 🔄 Guia de Migração: SQL Server → PostgreSQL

## 📋 Visão Geral

Este guia mostra como migrar o MedicWarehouse de SQL Server para PostgreSQL, economizando significativamente em custos de infraestrutura.

## 💰 Por que Migrar?

### Comparativo de Custos em Produção

| Item | SQL Server | PostgreSQL | Economia |
|------|-----------|-----------|----------|
| **Licença** | $1,000-5,000/ano | $0 (open source) | 100% |
| **Cloud DB (Azure/AWS)** | $50-200/mês | $5-20/mês (Railway) | 70-90% |
| **Suporte PaaS** | Limitado | Amplo (Railway, Render, Neon) | - |
| **Total Anual** | $1,600-7,400 | $60-240 | **92-96%** |

### Outros Benefícios

- ✅ Melhor performance em queries complexas
- ✅ JSON nativo (melhor que SQL Server)
- ✅ Full-text search mais robusto
- ✅ Extensões poderosas (PostGIS, pg_trgm)
- ✅ Comunidade maior e mais ativa
- ✅ Compatível com todos os PaaS modernos

## 🚀 Processo de Migração

### Etapa 1: Adicionar Suporte PostgreSQL ao Projeto

#### 1.1 Instalar Pacote NuGet

```bash
cd src/MedicSoft.Repository
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
```

#### 1.2 Atualizar ApplicationDbContext

Edite: `src/MedicSoft.Repository/Data/ApplicationDbContext.cs`

**Adicione ou atualize o método OnConfiguring:**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL; // Adicionar

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        // Auto-detectar provedor baseado na connection string
        if (IsPostgreSQL(connectionString))
        {
            ConfigurePostgreSQL(optionsBuilder, connectionString);
        }
        else
        {
            ConfigureSqlServer(optionsBuilder, connectionString);
        }

        // Configurações para desenvolvimento
        if (_env.IsDevelopment())
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
    }
}

private bool IsPostgreSQL(string connectionString)
{
    return connectionString?.Contains("Host=", StringComparison.OrdinalIgnoreCase) == true ||
           connectionString?.Contains("postgres", StringComparison.OrdinalIgnoreCase) == true;
}

private void ConfigurePostgreSQL(DbContextOptionsBuilder optionsBuilder, string connectionString)
{
    optionsBuilder.UseNpgsql(connectionString, options =>
    {
        options.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
        
        options.MigrationsHistoryTable("__EFMigrationsHistory", "public");
        options.CommandTimeout(60);
    });
}

private void ConfigureSqlServer(DbContextOptionsBuilder optionsBuilder, string connectionString)
{
    optionsBuilder.UseSqlServer(connectionString, options =>
    {
        options.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10));
        
        options.CommandTimeout(60);
    });
}
```

#### 1.3 Atualizar Configurações de Entidades

Algumas configurações precisam ser ajustadas para PostgreSQL:

**Exemplo - Decimal Precision:**

```csharp
// Antes (SQL Server)
builder.Property(p => p.Price)
    .HasColumnType("decimal(18,2)");

// Depois (compatível com ambos)
builder.Property(p => p.Price)
    .HasPrecision(18, 2); // Funciona em ambos
```

**Exemplo - String Length:**

```csharp
// Continua igual em ambos
builder.Property(p => p.Name)
    .HasMaxLength(200)
    .IsRequired();
```

### Etapa 2: Criar Migrations para PostgreSQL

#### 2.1 Criar Pasta de Migrations Separada

```bash
# Criar diretório
mkdir -p src/MedicSoft.Repository/Migrations/PostgreSQL

# Gerar migration inicial para PostgreSQL
dotnet ef migrations add InitialPostgreSQL \
  --context ApplicationDbContext \
  --project src/MedicSoft.Repository \
  --startup-project src/MedicSoft.Api \
  --output-dir Migrations/PostgreSQL
```

#### 2.2 Revisar Migration Gerada

Abra o arquivo gerado e verifique:

1. **Tipos de dados** foram mapeados corretamente
2. **Índices** estão presentes
3. **Foreign Keys** estão corretas
4. **Default values** funcionam em PostgreSQL

**Ajustes comuns necessários:**

```csharp
// SQL Server usa NEWSEQUENTIALID()
// PostgreSQL usa gen_random_uuid()

// Antes (SQL Server)
Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWSEQUENTIALID()")

// Depois (PostgreSQL) - remova o defaultValueSql, use aplicação
Id = table.Column<Guid>(nullable: false)
```

### Etapa 3: Testar Localmente com PostgreSQL

#### 3.1 Atualizar appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=medicwarehouse_dev;Username=postgres;Password=postgres"
  }
}
```

#### 3.2 Iniciar PostgreSQL Local

**Opção 1: Docker**
```bash
docker run -d \
  --name medicwarehouse-postgres \
  -e POSTGRES_DB=medicwarehouse_dev \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:16-alpine
```

**Opção 2: Docker Compose**
```bash
# Use o docker-compose.production.yml com ajustes
docker compose -f docker-compose.production.yml up postgres -d
```

#### 3.3 Aplicar Migrations

```bash
# Aplicar migrations PostgreSQL
dotnet ef database update \
  --context ApplicationDbContext \
  --project src/MedicSoft.Repository \
  --startup-project src/MedicSoft.Api

# Verificar
dotnet ef migrations list \
  --context ApplicationDbContext \
  --project src/MedicSoft.Repository \
  --startup-project src/MedicSoft.Api
```

#### 3.4 Executar Testes

```bash
# Executar todos os testes
dotnet test

# Verificar que nada quebrou
dotnet run --project src/MedicSoft.Api
```

### Etapa 4: Migrar Dados (Se Aplicável)

Se você já tem dados em SQL Server que precisa migrar:

#### 4.1 Export do SQL Server

```bash
# Via sqlcmd (Windows)
sqlcmd -S localhost -d MedicWarehouse -E -Q "SELECT * FROM Patients" -o patients.csv -s"," -w 700

# Via SQL Server Management Studio
# Export Wizard → CSV
```

#### 4.2 Import para PostgreSQL

```bash
# Conectar ao PostgreSQL
docker exec -it medicwarehouse-postgres psql -U postgres -d medicwarehouse_dev

# Import CSV
\copy patients(id, name, email, ...) FROM '/path/to/patients.csv' DELIMITER ',' CSV HEADER;
```

#### 4.3 Script de Migração Automatizado (Avançado)

Se tiver muitos dados, considere uma ferramenta:

- **pgLoader**: Migração direta SQL Server → PostgreSQL
- **AWS DMS**: Database Migration Service
- **Airbyte**: Open source ETL

**Exemplo com pgLoader:**

```bash
# Instalar pgLoader
brew install pgloader  # Mac
apt-get install pgloader  # Linux

# Criar arquivo de configuração
cat > migrate.load << EOF
LOAD DATABASE
  FROM mssql://sa:password@localhost/MedicWarehouse
  INTO postgresql://postgres:postgres@localhost/medicwarehouse_dev
  
  WITH include drop, create tables, create indexes, reset sequences
  
  SET work_mem to '256MB',
      maintenance_work_mem to '512 MB';
EOF

# Executar migração
pgloader migrate.load
```

### Etapa 5: Ajustes no Código (Se Necessário)

#### 5.1 Case Sensitivity

PostgreSQL é case-sensitive por padrão para identificadores entre aspas.

```csharp
// SQL Server (case-insensitive)
SELECT * FROM Patients WHERE Name = 'JOHN'  // Encontra "John", "JOHN", "john"

// PostgreSQL (case-insensitive para colunas sem aspas)
SELECT * FROM patients WHERE name = 'john'  // Só encontra "john"

// Solução: Use ILIKE para busca case-insensitive
SELECT * FROM patients WHERE name ILIKE 'john'  // Encontra todos
```

**No Entity Framework:**

```csharp
// Funciona em ambos (EF Core faz o mapeamento correto)
var patients = await _context.Patients
    .Where(p => p.Name.ToLower() == searchTerm.ToLower())
    .ToListAsync();

// Ou use EF.Functions (recomendado)
var patients = await _context.Patients
    .Where(p => EF.Functions.Like(p.Name, $"%{searchTerm}%"))
    .ToListAsync();
```

#### 5.2 Sequências e Identity

PostgreSQL usa SERIAL/BIGSERIAL ao invés de IDENTITY:

```csharp
// Configuração no OnModelCreating já é compatível
builder.Property(p => p.Id)
    .ValueGeneratedOnAdd(); // Funciona em ambos

// PostgreSQL criará automaticamente SERIAL
```

#### 5.3 Datas e Timezones

PostgreSQL tem tipos TIMESTAMP WITH TIME ZONE e WITHOUT TIME ZONE:

```csharp
// Recomendado: Sempre use UTC no banco
builder.Property(p => p.CreatedAt)
    .HasColumnType("timestamp with time zone"); // PostgreSQL
    // SQL Server usa datetime2(7)
```

### Etapa 6: Atualizar Connection Strings

#### 6.1 Development (appsettings.Development.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=medicwarehouse_dev;Username=postgres;Password=postgres;Include Error Detail=true"
  }
}
```

#### 6.2 Production (appsettings.Production.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${DATABASE_URL}"
  }
}
```

**Railway/Render fornecem DATABASE_URL automaticamente:**
```
postgresql://user:password@host:5432/database?sslmode=require
```

### Etapa 7: Deploy em Produção

Siga o guia: [DEPLOY_RAILWAY_GUIDE.md](DEPLOY_RAILWAY_GUIDE.md)

## 🔍 Verificações Pós-Migração

### Checklist de Validação

- [ ] **Aplicação inicia** sem erros
- [ ] **Migrations aplicadas** com sucesso
- [ ] **Testes passam** (dotnet test)
- [ ] **Queries básicas** funcionam (GET /api/patients)
- [ ] **Inserts funcionam** (POST /api/patients)
- [ ] **Updates funcionam** (PUT /api/patients/{id})
- [ ] **Deletes funcionam** (DELETE /api/patients/{id})
- [ ] **Relacionamentos** carregam corretamente (Include)
- [ ] **Validações** funcionam como antes
- [ ] **Performance** está adequada (< 500ms queries simples)

### Testes de Performance

```sql
-- PostgreSQL: Analisar query
EXPLAIN ANALYZE 
SELECT * FROM patients WHERE name ILIKE '%silva%';

-- Criar índice se necessário
CREATE INDEX idx_patients_name_gin ON patients USING gin (name gin_trgm_ops);
```

## 🐛 Troubleshooting Comum

### Erro: "column does not exist"

**Causa**: PostgreSQL é case-sensitive com aspas.

**Solução**:
```csharp
// Garantir que nomes de colunas estão consistentes
builder.Property(p => p.Name)
    .HasColumnName("name"); // lowercase
```

### Erro: "relation does not exist"

**Causa**: Migration não foi aplicada.

**Solução**:
```bash
dotnet ef database update
```

### Performance Lenta

**Solução**:
```sql
-- Criar índices necessários
CREATE INDEX idx_patients_cpf ON patients(cpf);
CREATE INDEX idx_appointments_date ON appointments(appointment_date);

-- Analisar estatísticas
ANALYZE patients;
ANALYZE appointments;

-- Vacuum (limpeza)
VACUUM ANALYZE;
```

### Connection Pool Esgotado

**Solução** (em appsettings.json):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;Maximum Pool Size=50;Minimum Pool Size=5;Connection Idle Lifetime=300"
  }
}
```

## 📊 Comparativo Final

### Antes (SQL Server)

```
Infraestrutura:
- SQL Server Express: Limitado a 10GB
- SQL Server Standard: $1,500/ano + $50-200/mês cloud
- Hosting: Limitado a Azure/AWS (caro)
- Backups: Manual ou pago

Total: $1,500-5,000/ano
```

### Depois (PostgreSQL)

```
Infraestrutura:
- PostgreSQL: Grátis e ilimitado
- Railway: $5-20/mês (tudo incluído)
- Hosting: Railway, Render, Neon, DigitalOcean, Hetzner
- Backups: Automáticos no Railway

Total: $60-240/ano (economia de 90-96%)
```

## 🎯 Recomendação

**Para novos projetos**: Use PostgreSQL desde o início

**Para projetos existentes**: 
1. Adicione suporte PostgreSQL (mantém SQL Server)
2. Teste em staging
3. Migre dados
4. Corte custos!

## 📚 Recursos

- [Npgsql Documentation](https://www.npgsql.org/efcore/)
- [PostgreSQL vs SQL Server](https://www.postgresql.org/about/)
- [EF Core Providers](https://docs.microsoft.com/ef/core/providers/)
- [Railway PostgreSQL](https://docs.railway.app/databases/postgresql)

---

**Criado por**: GitHub Copilot  
**Versão**: 1.0  
**Última atualização**: Outubro 2025
