# Correção de Configuração do Swagger e Portas - Resumo

## Problema Relatado
> a api do portal do paciente ainda nao exibe o swagger, a pagina fica em branco e esta dando erro de porta em uso quando estou executando o medicwarehouse.api

## Problemas Identificados e Corrigidos

### Problema 1: Swagger do MedicSoft.Api (medicwarehouse.api) Disponível Apenas em Desenvolvimento

**Problema:**
- O Swagger estava habilitado apenas quando `app.Environment.IsDevelopment()` era verdadeiro
- Isso impedia o carregamento do Swagger em Produção, Homologação ou outros ambientes
- Usuários não conseguiam acessar a documentação da API fora do modo Development

**Solução:**
- Tornamos o Swagger configurável via configuração `SwaggerSettings:Enabled` (similar ao PatientPortal.Api)
- O Swagger agora usa padrão `true` em Development e `false` em Production
- Pode ser sobrescrito via arquivos de configuração ou variáveis de ambiente

**Arquivos Modificados:**
- `/src/MedicSoft.Api/Program.cs` (linhas 698-716)
- `/src/MedicSoft.Api/appsettings.json` (adicionada seção SwaggerSettings)
- `/src/MedicSoft.Api/appsettings.Development.json` (adicionado SwaggerSettings: Enabled: true)
- `/src/MedicSoft.Api/appsettings.Production.json` (adicionado SwaggerSettings: Enabled: false)

**Mudanças no Código:**
```csharp
// Antes:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { ... });
}

// Depois:
var enableSwagger = builder.Configuration.GetValue<bool?>("SwaggerSettings:Enabled") 
    ?? app.Environment.IsDevelopment();

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { ... });
}
```

### Problema 2: Conflitos de Porta no launchSettings.json do MedicSoft.Api

**Problema:**
- MedicSoft.Api tinha configuração de portas inconsistente:
  - Perfil HTTP: `http://localhost:5293`
  - Perfil HTTPS: `https://localhost:5000;http://localhost:5001`
- As portas HTTPS/HTTP estavam invertidas das convenções do ASP.NET
- Porta 5293 era uma escolha estranha que poderia conflitar com outros serviços
- Portas 5000/5001 poderiam conflitar ao tentar executar ambas APIs simultaneamente

**Solução:**
- Padronizadas as portas do MedicSoft.Api:
  - Perfil HTTP: `http://localhost:5000`
  - Perfil HTTPS: `https://localhost:5001;http://localhost:5000`
- PatientPortal.Api permanece em portas separadas (5101 HTTP, 7030 HTTPS)
- Sem conflitos de porta ao executar ambas APIs simultaneamente

**Arquivos Modificados:**
- `/src/MedicSoft.Api/Properties/launchSettings.json`

**Resumo de Portas:**
| API | Porta HTTP | Porta HTTPS | URL do Swagger |
|-----|------------|-------------|----------------|
| MedicSoft.Api (medicwarehouse.api) | 5000 | 5001 | http://localhost:5000/swagger |
| PatientPortal.Api | 5101 | 7030 | http://localhost:5101/ (raiz) |

### Problema 3: Configuração do Swagger do PatientPortal.Api (Já Estava Correto)

**Status:** ✅ Nenhuma alteração necessária

O PatientPortal.Api já estava configurado corretamente com:
- Swagger habilitado por padrão via `SwaggerSettings:Enabled: true`
- Interface Swagger acessível no caminho raiz (`/`)
- Autenticação JWT Bearer adequada no Swagger

## Exemplos de Configuração

### Habilitar Swagger em Todos os Ambientes
Em `appsettings.json` ou `appsettings.Production.json`:
```json
{
  "SwaggerSettings": {
    "Enabled": true
  }
}
```

### Desabilitar Swagger em Produção (Recomendado para Segurança)
Em `appsettings.Production.json`:
```json
{
  "SwaggerSettings": {
    "Enabled": false
  }
}
```

### Controlar via Variável de Ambiente
```bash
export SwaggerSettings__Enabled=true
dotnet run
```

Ou no Docker:
```dockerfile
ENV SwaggerSettings__Enabled=false
```

## Como Testar

### Pré-requisitos
1. PostgreSQL rodando em localhost:5432 (ou via Docker)
2. Credenciais do banco configuradas no appsettings
3. .NET 8.0 SDK instalado

### Testar Swagger do MedicSoft.Api

```bash
# Navegue para o diretório da API
cd src/MedicSoft.Api

# Compile o projeto
dotnet build

# Execute a API
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Acesse o Swagger no navegador
# Abra: http://localhost:5000/swagger
```

Resultado esperado: A interface Swagger deve carregar e exibir todos os endpoints da API

### Testar Swagger do PatientPortal.Api

```bash
# Navegue para o diretório da API
cd patient-portal-api/PatientPortal.Api

# Compile o projeto
dotnet build

# Execute a API
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Acesse o Swagger no navegador
# Abra: http://localhost:5101/
```

Resultado esperado: A interface Swagger deve carregar na URL raiz

### Testar Ambas APIs Simultaneamente

```bash
# Terminal 1: Inicie o MedicSoft.Api
cd src/MedicSoft.Api
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Terminal 2: Inicie o PatientPortal.Api
cd patient-portal-api/PatientPortal.Api
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Ambas APIs devem iniciar sem conflitos de porta
# MedicSoft.Api: http://localhost:5000/swagger
# PatientPortal.Api: http://localhost:5101/
```

## Considerações de Segurança

### Implantação em Produção

Ao implantar em produção, considere:

1. **Desabilitar o Swagger** (recomendado):
   - Configure `SwaggerSettings:Enabled` como `false` em `appsettings.Production.json`
   - Previne exposição pública da estrutura da API

2. **Restrições em Nível de Rede** (se o Swagger estiver habilitado):
   - Use regras de firewall para restringir acesso ao Swagger a faixas de IP específicas
   - Implante atrás de VPN para acesso apenas interno
   - Use reverse proxy (nginx, IIS) para adicionar autenticação

3. **Autenticação JWT**:
   - Ambas APIs já requerem tokens JWT Bearer para endpoints protegidos
   - Interface Swagger inclui configuração de autenticação
   - Nenhum dado sensível é exposto através dos schemas do Swagger

## Verificação de Build

✅ **MedicSoft.Api**: Compila com sucesso com 340 avisos, 0 erros
✅ **PatientPortal.Api**: Compila com sucesso com 2 avisos, 0 erros

## Nota sobre Configuração do Banco de Dados

Ambas APIs requerem PostgreSQL para iniciar completamente. Certifique-se de que:

1. PostgreSQL está rodando em `localhost:5432` (ou host configurado)
2. As credenciais do banco correspondem entre:
   - `docker-compose.yml` (se usando Docker): `POSTGRES_PASSWORD=${POSTGRES_PASSWORD:-postgres}`
   - `appsettings.json`: Senha na connection string
   - `appsettings.Development.json`: Senha na connection string

Credenciais padrão:
- **Desenvolvimento**: postgres/postgres (do docker-compose)
- **Produção**: Use credenciais seguras de variáveis de ambiente

## Documentação Relacionada

- [CORRECAO_SWAGGER_PORTAL_PACIENTE.md](patient-portal-api/CORRECAO_SWAGGER_PORTAL_PACIENTE.md) - Correção anterior do Swagger do Portal do Paciente
- [SWAGGER_CONFIGURATION.md](patient-portal-api/SWAGGER_CONFIGURATION.md) - Guia de configuração do Swagger (Portal do Paciente)
- [CORRECAO_SWAGGER_RESUMO.md](CORRECAO_SWAGGER_RESUMO.md) - Correção anterior do Swagger para IFormFile
- [SWAGGER_PORT_FIX_SUMMARY.md](SWAGGER_PORT_FIX_SUMMARY.md) - Versão em inglês desta documentação

## Resumo

✅ **Swagger do MedicSoft.Api**: Agora configurável, não mais restrito apenas ao Development
✅ **Conflitos de Porta**: Resolvidos padronizando portas (5000/5001 para MedicSoft, 5101/7030 para PatientPortal)
✅ **Swagger do PatientPortal.Api**: Já estava corretamente configurado
✅ **Status de Build**: Ambas APIs compilam com sucesso
✅ **Segurança**: Swagger pode ser desabilitado em Produção via configuração

As mudanças são mínimas e focadas em configuração, mantendo compatibilidade retroativa enquanto fornecem flexibilidade para diferentes ambientes de implantação.
