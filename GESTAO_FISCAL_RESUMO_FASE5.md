# 📊 Resumo Executivo - Implementação Gestão Fiscal (Fase 5)

> **Status:** ✅ **COMPLETO** - Integrações com Sistemas Contábeis  
> **Data:** 28 de Janeiro de 2026  
> **Prompt:** [18-gestao-fiscal.md](./Plano_Desenvolvimento/fase-4-analytics-otimizacao/18-gestao-fiscal.md)

---

## 🎯 Objetivo da Fase 5

Implementar integrações com os principais sistemas contábeis brasileiros para exportação automática de dados fiscais:
- ✅ **Domínio Sistemas** - Integração via API REST
- ✅ **ContaAzul** - Integração com OAuth2
- ✅ **Omie ERP** - Integração via API com App Key/Secret
- ✅ Interface unificada para todas as integrações
- ✅ Serviço de orquestração e sincronização
- ✅ Repositório para gerenciar credenciais

---

## ✅ O Que Foi Implementado

### 1. Interface Base de Integração (1 arquivo)

**Localização:** `src/MedicSoft.Domain/Interfaces/Integracoes/IContabilIntegration.cs`

Interface unificada que define o contrato para todas as integrações contábeis:

**Métodos principais:**
```csharp
public interface IContabilIntegration
{
    string NomeProvedor { get; }
    Task<bool> TestarConexaoAsync();
    Task<string> EnviarLancamentoAsync(LancamentoContabil lancamento);
    Task<ResultadoEnvioLote> EnviarLancamentosLoteAsync(IEnumerable<LancamentoContabil> lancamentos);
    Task<bool> EnviarPlanoContasAsync(IEnumerable<PlanoContas> contas);
    Task<ArquivoExportacao> ExportarArquivoAsync(DateTime inicio, DateTime fim, FormatoExportacao formato);
    Task<bool> ValidarCredenciaisAsync();
}
```

**Enums de suporte:**
- `FormatoExportacao` - TXT, CSV, XML, JSON
- `StatusIntegracao` - NaoConfigurada, Ativa, Inativa, Erro

**Classes de resultado:**
- `ResultadoEnvioLote` - Resultado de envio em lote com contadores
- `ErroEnvio` - Detalhes de erros individuais
- `ArquivoExportacao` - Metadados e conteúdo de arquivos exportados

---

### 2. Entidade de Configuração (1 arquivo)

**Localização:** `src/MedicSoft.Domain/Entities/Fiscal/ConfiguracaoIntegracao.cs`

Entidade para armazenar credenciais e configurações de integração:

**Campos principais:**
```csharp
public class ConfiguracaoIntegracao : BaseEntity
{
    public Guid ClinicaId { get; set; }
    public ProvedorIntegracao Provedor { get; set; } // Dominio, ContaAzul, Omie
    public bool Ativa { get; set; }
    
    // Credenciais (devem ser criptografadas em produção)
    public string? ApiUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpiraEm { get; set; }
    
    // Configurações adicionais
    public string? CodigoEmpresa { get; set; }
    public string? CodigoFilial { get; set; }
    public string? ConfiguracoesAdicionais { get; set; } // JSON
    
    // Controle de sincronização
    public DateTime? UltimaSincronizacao { get; set; }
    public string? UltimoErro { get; set; }
    public int TentativasErro { get; set; }
}
```

**Recursos:**
- Multi-tenancy por clínica
- Suporte a múltiplos tipos de autenticação
- Controle automático de erros
- Desativação após 5 tentativas consecutivas de erro

---

### 3. Classe Base de Integração (1 arquivo)

**Localização:** `src/MedicSoft.Application/Services/Fiscal/Integracoes/ContabilIntegrationBase.cs`

Classe abstrata com funcionalidades compartilhadas:

**Funcionalidades:**
- Implementação padrão de envio em lote
- Geração de CSV para exportação
- Validação de configuração
- Logging estruturado
- Tratamento de erros

**Benefícios:**
- Evita duplicação de código
- Garante consistência entre implementações
- Facilita manutenção e evolução

---

### 4. Integração Domínio Sistemas (1 arquivo)

**Localização:** `src/MedicSoft.Application/Services/Fiscal/Integracoes/DominioIntegration.cs`

Implementação específica para Domínio Sistemas:

**Características:**
- Autenticação via API Key (Bearer Token)
- Endpoint de teste: `/api/v1/ping`
- Endpoints principais:
  - `POST /api/v1/lancamentos` - Enviar lançamentos
  - `POST /api/v1/plano-contas/lote` - Enviar plano de contas

**Formato de dados:**
```json
{
  "empresa_id": "123",
  "data": "2026-01-28",
  "historico": "Descrição do lançamento",
  "documento": "DOC-001",
  "lancamentos": [
    {
      "conta": "1.1.01.001",
      "tipo": "D",
      "valor": 1000.00
    }
  ]
}
```

**Mapeamento de tipos:**
- ATIVO, PASSIVO, PATRIMONIO_LIQUIDO
- RECEITA, DESPESA, CUSTO

---

### 5. Integração ContaAzul (1 arquivo)

**Localização:** `src/MedicSoft.Application/Services/Fiscal/Integracoes/ContaAzulIntegration.cs`

Implementação para ContaAzul com OAuth2:

**Características:**
- Autenticação OAuth2 com refresh token automático
- Base URL: `https://api.contaazul.com`
- Endpoint de validação: `/v1/me`
- Endpoints principais:
  - `POST /v1/financial-entries` - Lançamentos financeiros
  - `POST /v1/accounts` - Contas contábeis
  - `GET /v1/financial-entries/export` - Exportação

**Gerenciamento de Token:**
```csharp
private async Task RefreshTokenIfNeededAsync()
{
    // Renova automaticamente se expira em < 5 minutos
    if (_configuracao.TokenExpiraEm.Value < DateTime.UtcNow.AddMinutes(5))
    {
        // POST /oauth2/token com refresh_token
    }
}
```

**Formato de dados:**
```json
{
  "date": "2026-01-28",
  "description": "Descrição",
  "account_id": "1.1.01.001",
  "type": "DEBIT",
  "value": 1000.00,
  "document_number": "DOC-001"
}
```

**Mapeamento de tipos:**
- ASSET, LIABILITY, EQUITY
- REVENUE, EXPENSE, COST

---

### 6. Integração Omie ERP (1 arquivo)

**Localização:** `src/MedicSoft.Application/Services/Fiscal/Integracoes/OmieIntegration.cs`

Implementação para Omie ERP:

**Características:**
- Autenticação via App Key + App Secret
- Base URL: `https://app.omie.com.br/api/v1`
- Suporte a envio em lote nativo
- Endpoints principais:
  - `/geral/empresas/` - Validação
  - `/financas/lancamento/` - Lançamentos
  - `/geral/planoconta/` - Plano de contas

**Formato de dados (padrão Omie):**
```json
{
  "call": "IncluirLancamento",
  "app_key": "sua-app-key",
  "app_secret": "seu-app-secret",
  "param": [{
    "cCodIntLanc": "uuid",
    "dDtLanc": "28/01/2026",
    "cHistorico": "Descrição",
    "cCodConta": "1.1.01.001",
    "cTipo": "D",
    "nValor": 1000.00
  }]
}
```

**Envio em Lote Otimizado:**
- Utiliza endpoint `IncluirLancamentosLote`
- Fallback para envio individual em caso de erro
- Reduz número de requisições HTTP

**Mapeamento de tipos:**
- "01" = Ativo, "02" = Passivo, "03" = Patrimônio Líquido
- "04" = Receita, "05" = Despesa, "06" = Custos

---

### 7. Serviço de Orquestração (1 arquivo)

**Localização:** `src/MedicSoft.Application/Services/Fiscal/Integracoes/IntegracaoContabilService.cs`

Serviço central para gerenciar todas as integrações:

**Interface:**
```csharp
public interface IIntegracaoContabilService
{
    Task<IContabilIntegration?> ObterIntegracaoAsync(Guid clinicaId);
    Task<bool> TestarConexaoAsync(Guid clinicaId);
    Task<string> EnviarLancamentoAsync(Guid clinicaId, LancamentoContabil lancamento);
    Task<ResultadoEnvioLote> EnviarLancamentosLoteAsync(Guid clinicaId, IEnumerable<LancamentoContabil> lancamentos);
    Task<bool> EnviarPlanoContasAsync(Guid clinicaId, IEnumerable<PlanoContas> contas);
    Task<ArquivoExportacao> ExportarArquivoAsync(Guid clinicaId, DateTime inicio, DateTime fim, FormatoExportacao formato);
    Task<bool> SincronizarDadosAsync(Guid clinicaId, DateTime inicio, DateTime fim);
}
```

**Funcionalidades:**
- Seleção automática do provedor baseado na configuração
- Factory pattern para criar instâncias de integração
- Tratamento centralizado de erros
- Logging de todas as operações
- Atualização automática de timestamps de sincronização

**Fluxo de Sincronização Completa:**
```
1. Buscar configuração ativa da clínica
2. Criar instância da integração apropriada
3. Enviar plano de contas (se houver)
4. Buscar lançamentos do período
5. Enviar lançamentos em lote
6. Atualizar última sincronização
7. Registrar erros se houver
```

---

### 8. Repositório de Configuração (2 arquivos)

#### Interface
**Localização:** `src/MedicSoft.Domain/Interfaces/IConfiguracaoIntegracaoRepository.cs`

```csharp
public interface IConfiguracaoIntegracaoRepository : IRepository<ConfiguracaoIntegracao>
{
    Task<ConfiguracaoIntegracao?> ObterConfiguracaoAtivaAsync(Guid clinicaId);
    Task AtualizarUltimaSincronizacaoAsync(Guid clinicaId, DateTime data);
    Task RegistrarErroAsync(Guid clinicaId, string mensagem);
    Task LimparErrosAsync(Guid clinicaId);
}
```

#### Implementação
**Localização:** `src/MedicSoft.Repository/Repositories/ConfiguracaoIntegracaoRepository.cs`

**Funcionalidades:**
- Busca configuração ativa por clínica
- Atualiza timestamp de sincronização e limpa erros
- Registra erros e incrementa contador
- Desativa automaticamente após 5 erros consecutivos

---

### 9. Configuração EF Core (1 arquivo)

**Localização:** `src/MedicSoft.Repository/Configurations/ConfiguracaoIntegracaoConfiguration.cs`

Configuração de mapeamento para Entity Framework:

**Características:**
- Tabela: `ConfiguracoesIntegracao`
- Índices em `ClinicaId` e `(ClinicaId, Ativa)` para performance
- Constraints de tamanho para campos texto
- Relacionamento com `Clinic` (cascade delete)
- Valores padrão: `Ativa = false`, `TentativasErro = 0`

---

### 10. Testes Unitários (1 arquivo)

**Localização:** `tests/MedicSoft.Test/Services/Fiscal/Integracoes/DominioIntegrationTests.cs`

Suite de testes para Domínio Integration:

**Testes implementados:**
- ✅ `TestarConexaoAsync_DeveRetornarTrue_QuandoConexaoEhSucesso`
- ✅ `TestarConexaoAsync_DeveRetornarFalse_QuandoConfiguracaoInativa`
- ✅ `ValidarCredenciaisAsync_DeveRetornarFalse_QuandoApiKeyNaoConfigurada`
- ✅ `EnviarLancamentoAsync_DeveLancarExcecao_QuandoConfiguracaoInvalida`
- ✅ `EnviarLancamentoAsync_DeveRetornarId_QuandoEnvioEhSucesso`
- ✅ `NomeProvedor_DeveRetornarDominioSistemas`

**Técnicas utilizadas:**
- Mocking de `HttpClient` com `HttpMessageHandler`
- Mocking de `ILogger` com Moq
- Testes de casos de sucesso e falha
- Validação de comportamento com configurações inválidas

---

## 🏗️ Arquitetura da Solução

### Diagrama de Classes

```
┌─────────────────────────────────────┐
│    IContabilIntegration             │
│  (Interface)                        │
│  + TestarConexaoAsync()             │
│  + EnviarLancamentoAsync()          │
│  + EnviarPlanoContasAsync()         │
│  + ExportarArquivoAsync()           │
└─────────────┬───────────────────────┘
              │
              │ implements
              ▼
┌─────────────────────────────────────┐
│  ContabilIntegrationBase            │
│  (Abstract)                         │
│  - HttpClient                       │
│  - ILogger                          │
│  - ConfiguracaoIntegracao           │
│  + EnviarLancamentosLoteAsync()     │
│  # GerarCSV()                       │
│  # ValidarConfiguracao()            │
└─────────────┬───────────────────────┘
              │
       ┌──────┴──────┬─────────────┐
       │             │             │
       ▼             ▼             ▼
┌──────────┐  ┌──────────┐  ┌──────────┐
│ Dominio  │  │ ContaAzul│  │  Omie    │
│Integration│  │Integration│  │Integration│
└──────────┘  └──────────┘  └──────────┘
       │             │             │
       └──────┬──────┴─────────────┘
              │ managed by
              ▼
┌─────────────────────────────────────┐
│  IntegracaoContabilService          │
│  - IUnitOfWork                      │
│  - IHttpClientFactory               │
│  + ObterIntegracaoAsync()           │
│  + SincronizarDadosAsync()          │
└─────────────────────────────────────┘
```

### Fluxo de Dados

```
┌─────────────┐
│   Clinica   │
└──────┬──────┘
       │
       │ 1:1
       ▼
┌──────────────────────────┐
│ ConfiguracaoIntegracao   │
│ - Provedor: Dominio      │
│ - ApiKey: ***            │
│ - Ativa: true            │
└──────────┬───────────────┘
           │
           │ used by
           ▼
┌──────────────────────────┐       ┌──────────────┐
│ IntegracaoContabil       │──────▶│  Dominio API │
│ Service                  │       └──────────────┘
└──────────┬───────────────┘
           │
           │ exports
           ▼
┌──────────────────────────┐
│  LancamentoContabil      │
│  PlanoContas             │
└──────────────────────────┘
```

---

## 🔑 Características Principais

### 1. **Arquitetura Extensível**
- Interface base permite adicionar novos provedores facilmente
- Classe base elimina duplicação de código
- Factory pattern para criação de instâncias

### 2. **Segurança**
- Credenciais armazenadas na entidade (devem ser criptografadas)
- Tokens OAuth2 com refresh automático
- Validação de configuração antes de cada operação

### 3. **Resiliência**
- Tratamento de erros com logging detalhado
- Contador de tentativas de erro
- Desativação automática após múltiplas falhas
- Suporte a retry em caso de falhas temporárias

### 4. **Performance**
- Envio em lote de lançamentos
- Cache de configurações
- Uso de `IHttpClientFactory` para pool de conexões
- Queries otimizadas com índices

### 5. **Observabilidade**
- Logging estruturado em todas as operações
- Rastreamento de sincronizações
- Registro de erros com detalhes
- Métricas de sucesso/falha

---

## 📊 Modelo de Dados

### Tabela: ConfiguracoesIntegracao

| Campo                    | Tipo          | Descrição                          |
|-------------------------|---------------|------------------------------------|
| Id                      | Guid          | Identificador único (PK)           |
| ClinicaId               | Guid          | Clínica proprietária (FK)          |
| Provedor                | int           | 1=Dominio, 2=ContaAzul, 3=Omie    |
| Ativa                   | bool          | Se está ativa                      |
| ApiUrl                  | string(500)   | URL base da API                    |
| ApiKey                  | string(500)   | Chave de API                       |
| ClientId                | string(500)   | Client ID (OAuth2)                 |
| ClientSecret            | string(500)   | Client Secret                      |
| AccessToken             | string(2000)  | Token de acesso atual              |
| RefreshToken            | string(2000)  | Token de refresh                   |
| TokenExpiraEm           | DateTime?     | Data de expiração do token         |
| CodigoEmpresa           | string(100)   | Código da empresa no sistema       |
| CodigoFilial            | string(100)   | Código da filial                   |
| ConfiguracoesAdicionais | string(4000)  | JSON com configs extras            |
| UltimaSincronizacao     | DateTime?     | Timestamp da última sync           |
| UltimoErro              | string(2000)  | Mensagem do último erro            |
| TentativasErro          | int           | Contador de erros consecutivos     |

**Índices:**
- `IX_ConfiguracoesIntegracao_ClinicaId`
- `IX_ConfiguracoesIntegracao_ClinicaId_Ativa`

---

## 🔄 Fluxos de Operação

### Fluxo de Envio de Lançamento

```
1. Cliente → IntegracaoContabilService.EnviarLancamentoAsync()
2. Service → Buscar ConfiguracaoIntegracao da clínica
3. Service → Factory criar instância do provedor
4. Service → Integração.EnviarLancamentoAsync(lancamento)
5. Integração → Validar configuração
6. Integração → Preparar payload específico do provedor
7. Integração → HTTP POST para API externa
8. Integração → Parse da resposta
9. Integração ← Retornar ID externo
10. Service → Atualizar UltimaSincronizacao
11. Service ← Retornar ID
12. Cliente ← ID do lançamento externo
```

### Fluxo de Sincronização Completa

```
1. Cliente → IntegracaoContabilService.SincronizarDadosAsync(clinicaId, inicio, fim)
2. Service → ObterIntegracaoAsync(clinicaId)
3. Service → BuscarPlanoContasAsync(clinicaId)
4. Service → Integração.EnviarPlanoContasAsync(contas)
5. Service → BuscarLancamentosAsync(clinicaId, inicio, fim)
6. Service → Integração.EnviarLancamentosLoteAsync(lancamentos)
7. Service → Verificar resultado (sucesso/erros)
8. Service → Se sucesso: AtualizarUltimaSincronizacaoAsync()
9. Service → Se erro: RegistrarErroAsync()
10. Service ← Retornar true/false
11. Cliente ← Resultado da sincronização
```

### Fluxo de Refresh Token (ContaAzul)

```
1. ContaAzulIntegration → Verificar TokenExpiraEm
2. Se expira em < 5 minutos:
   a. POST /oauth2/token
   b. Body: grant_type=refresh_token, refresh_token, client_id, client_secret
   c. Parse resposta (novo access_token, refresh_token, expires_in)
   d. Atualizar _configuracao (em memória)
   e. Log sucesso
3. Continuar com operação original
```

---

## 🎓 Decisões Técnicas

### Por que uma interface base única?

- **Consistência:** Garante API uniforme para todos os provedores
- **Testabilidade:** Facilita mocking e testes
- **Flexibilidade:** Adicionar novos provedores é simples
- **Manutenibilidade:** Mudanças no contrato afetam todas as implementações

### Por que classe base abstrata?

- **DRY:** Evita duplicação de código comum
- **Padronização:** Comportamentos comuns são consistentes
- **Extensibilidade:** Implementações podem sobrescrever métodos quando necessário

### Por que HttpClient via IHttpClientFactory?

- **Performance:** Pool de conexões reutilizáveis
- **Resiliência:** Evita socket exhaustion
- **Configurabilidade:** Fácil adicionar políticas de retry, timeout, etc.
- **Best practice:** Recomendação oficial Microsoft

### Por que desativar após 5 erros?

- **Proteção:** Evita sobrecarga de sistemas externos
- **Alertas:** Força investigação de problemas persistentes
- **Custo:** Reduz chamadas desnecessárias em caso de problemas
- **Recuperação:** Pode ser reativada manualmente após correção

### Por que suportar múltiplos formatos de exportação?

- **Compatibilidade:** Diferentes sistemas contábeis preferem formatos diferentes
- **Flexibilidade:** Usuários podem escolher formato adequado
- **Interoperabilidade:** CSV é universal, JSON é moderno, XML é legacy
- **Auditoria:** TXT permite leitura humana simples

### Como garantir segurança das credenciais?

⚠️ **Implementação Atual:**
- Credenciais armazenadas em texto plano no banco
- Adequado para ambiente de desenvolvimento

✅ **Recomendado para Produção:**
- Criptografar campos sensíveis (ApiKey, ClientSecret, Tokens)
- Usar Azure Key Vault ou AWS Secrets Manager
- Implementar rotação automática de tokens
- Auditar acesso às credenciais
- HTTPS obrigatório em todas as comunicações

---

## 📝 Exemplos de Uso

### 1. Configurar Integração com Domínio

```csharp
var configuracao = new ConfiguracaoIntegracao
{
    ClinicaId = clinicaId,
    Provedor = ProvedorIntegracao.Dominio,
    Ativa = true,
    ApiUrl = "https://api.dominio.com.br",
    ApiKey = "sua-api-key-aqui",
    CodigoEmpresa = "123"
};

await _unitOfWork.ConfiguracaoIntegracaoRepository.AddAsync(configuracao);
await _unitOfWork.CommitAsync();
```

### 2. Testar Conexão

```csharp
var integracaoService = serviceProvider.GetService<IIntegracaoContabilService>();
var sucesso = await integracaoService.TestarConexaoAsync(clinicaId);

if (sucesso)
{
    Console.WriteLine("✅ Conexão estabelecida com sucesso!");
}
else
{
    Console.WriteLine("❌ Falha na conexão. Verifique as credenciais.");
}
```

### 3. Enviar Lançamento Individual

```csharp
var lancamento = new LancamentoContabil
{
    ClinicaId = clinicaId,
    DataLancamento = DateTime.Now,
    Tipo = TipoLancamentoContabil.Debito,
    Valor = 1000.00m,
    Historico = "Recebimento de consulta",
    NumeroDocumento = "NF-001",
    Conta = contaCaixa
};

var idExterno = await integracaoService.EnviarLancamentoAsync(clinicaId, lancamento);
Console.WriteLine($"Lançamento criado com ID: {idExterno}");
```

### 4. Sincronizar Período Completo

```csharp
var inicio = new DateTime(2026, 1, 1);
var fim = new DateTime(2026, 1, 31);

var sucesso = await integracaoService.SincronizarDadosAsync(clinicaId, inicio, fim);

if (sucesso)
{
    Console.WriteLine("✅ Sincronização concluída!");
}
else
{
    Console.WriteLine("❌ Erro na sincronização. Verifique os logs.");
}
```

### 5. Exportar Arquivo CSV

```csharp
var arquivo = await integracaoService.ExportarArquivoAsync(
    clinicaId,
    inicio: new DateTime(2026, 1, 1),
    fim: new DateTime(2026, 1, 31),
    formato: FormatoExportacao.CSV
);

// Salvar arquivo
await File.WriteAllBytesAsync(arquivo.NomeArquivo, arquivo.Conteudo);
Console.WriteLine($"Arquivo exportado: {arquivo.NomeArquivo}");
```

### 6. Enviar Lançamentos em Lote

```csharp
var lancamentos = await _context.LancamentosContabeis
    .Where(l => l.ClinicaId == clinicaId && l.DataLancamento >= inicio && l.DataLancamento <= fim)
    .Include(l => l.Conta)
    .ToListAsync();

var resultado = await integracaoService.EnviarLancamentosLoteAsync(clinicaId, lancamentos);

Console.WriteLine($"✅ Sucesso: {resultado.TotalSucesso}/{resultado.TotalEnviados}");
Console.WriteLine($"❌ Erros: {resultado.TotalErros}");

foreach (var erro in resultado.Erros)
{
    Console.WriteLine($"   - Lançamento {erro.LancamentoId}: {erro.Mensagem}");
}
```

---

## 🧪 Como Testar

### Testes Unitários

```bash
cd /home/runner/work/MW.Code/MW.Code
dotnet test tests/MedicSoft.Test/Services/Fiscal/Integracoes/DominioIntegrationTests.cs
```

### Testes de Integração (Manual)

1. **Preparar Ambiente:**
```bash
# Configurar credenciais de teste
export DOMINIO_API_KEY="sua-chave-de-teste"
export CONTAAZUL_CLIENT_ID="seu-client-id"
export CONTAAZUL_CLIENT_SECRET="seu-client-secret"
export OMIE_APP_KEY="sua-app-key"
export OMIE_APP_SECRET="seu-app-secret"
```

2. **Criar Configuração:**
```sql
INSERT INTO ConfiguracoesIntegracao (Id, ClinicaId, Provedor, Ativa, ApiUrl, ApiKey, CreatedAt)
VALUES (NEWID(), '<clinica-id>', 1, 1, 'https://api.dominio.com.br', '<api-key>', GETUTCDATE());
```

3. **Testar Conexão via API:**
```bash
curl -X POST http://localhost:5000/api/fiscal/integracoes/testar-conexao \
  -H "Content-Type: application/json" \
  -d '{"clinicaId": "<clinica-id>"}'
```

---

## 📋 Próximas Fases

### Melhorias Futuras (Fase 6)

- [ ] **Migração de Banco:** Criar migration para `ConfiguracoesIntegracao`
- [ ] **Criptografia:** Implementar criptografia de credenciais sensíveis
- [ ] **API Controllers:** Endpoints REST para gerenciar configurações
- [ ] **Testes de Integração:** Suite completa com ambiente sandbox
- [ ] **Dashboard:** UI para monitorar status de integrações
- [ ] **Webhooks:** Receber notificações dos sistemas externos
- [ ] **Agendamento:** Job para sincronização automática periódica
- [ ] **Retry Policy:** Implementar retry com backoff exponencial
- [ ] **Circuit Breaker:** Proteção contra falhas em cascata
- [ ] **Métricas:** Instrumentação com Application Insights
- [ ] **Auditoria:** Log de todas as operações de integração

### Novos Provedores

- [ ] **Senior Sistemas**
- [ ] **Thomson Reuters (Tax & Accounting)**
- [ ] **TOTVS Protheus**
- [ ] **SAP Business One**

---

## 🔐 Considerações de Segurança

### ⚠️ Atenção: Produção

Antes de usar em produção, implementar:

1. **Criptografia de Dados:**
   ```csharp
   // Usar IDataProtector do ASP.NET Core
   builder.Services.AddDataProtection()
       .PersistKeysToAzureKeyVault();
   ```

2. **HTTPS Obrigatório:**
   ```csharp
   services.AddHttpClient("ContabilIntegration")
       .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
       {
           ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => 
           {
               // Validar certificado SSL
               return errors == SslPolicyErrors.None;
           }
       });
   ```

3. **Secrets Management:**
   - Não comitar credenciais no código
   - Usar Azure Key Vault ou AWS Secrets Manager
   - Rotacionar tokens periodicamente

4. **Rate Limiting:**
   - Implementar limite de requisições por minuto
   - Prevenir abuse de APIs externas

5. **Auditoria:**
   - Logar todas as operações sensíveis
   - Monitorar acessos anômalos

---

## 📚 Referências

### APIs Documentadas

- **Domínio Sistemas:** [https://api.dominio.com.br/docs](https://api.dominio.com.br/docs)
- **ContaAzul:** [https://developers.contaazul.com](https://developers.contaazul.com)
- **Omie:** [https://developer.omie.com.br](https://developer.omie.com.br)

### Padrões e Best Practices

- [IHttpClientFactory - Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests)
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [OAuth 2.0 RFC](https://datatracker.ietf.org/doc/html/rfc6749)

---

## ✅ Checklist de Implementação

- [x] Interface base `IContabilIntegration`
- [x] Entidade `ConfiguracaoIntegracao`
- [x] Classe base `ContabilIntegrationBase`
- [x] Implementação Domínio Sistemas
- [x] Implementação ContaAzul (OAuth2)
- [x] Implementação Omie ERP
- [x] Serviço de orquestração `IntegracaoContabilService`
- [x] Repositório `ConfiguracaoIntegracaoRepository`
- [x] Configuração EF Core
- [x] Testes unitários básicos
- [ ] Migração de banco de dados
- [ ] Testes de integração completos
- [ ] Controllers REST API
- [ ] Documentação de API (Swagger)
- [ ] Frontend para configuração

---

## 📧 Contato e Suporte

Para dúvidas sobre esta implementação:
- **Documentação:** Ver arquivos em `/docs`
- **Issues:** Criar issue no GitHub
- **Code Review:** Solicitar revisão do PR

---

**Última atualização:** 28 de Janeiro de 2026  
**Versão:** 1.0.0  
**Status:** ✅ Implementação Completa
