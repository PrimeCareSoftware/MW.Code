# Solução Rápida: Erro "IsPaid column does not exist"

## 🚨 Mensagem de Erro

```
Npgsql.PostgresException (0x80004005): 42703: column "IsPaid" of relation "Appointments" does not exist
```

## 📝 Descrição do Problema

Este erro ocorre quando as migrações do Entity Framework Core não foram aplicadas corretamente ao banco de dados PostgreSQL. Especificamente, as colunas de rastreamento de pagamento (`IsPaid`, `PaidAt`, `PaidByUserId`, `PaymentReceivedBy`, `PaymentAmount`, `PaymentMethod`) estão faltando na tabela `Appointments`.

## ✅ Soluções (em ordem de preferência)

### Solução 1: Reiniciar a Aplicação (Recomendado)

A aplicação aplica automaticamente as migrações pendentes na inicialização.

```bash
# Pare a aplicação (Ctrl+C se estiver rodando no terminal)
# Em seguida, inicie novamente:
cd src/MedicSoft.Api
dotnet run
```

Você deverá ver nos logs:
```
Aplicando migrações do banco de dados...
Migrações do banco de dados aplicadas com sucesso
```

### Solução 2: Executar o Script de Correção SQL (Mais Rápido)

Se a Solução 1 não funcionar, execute o script SQL diretamente:

```bash
# Usando psql
psql -U postgres -d primecare -f scripts/fix-missing-payment-columns.sql

# Ou com credenciais personalizadas
psql -h localhost -U seu_usuario -d primecare -f scripts/fix-missing-payment-columns.sql
```

O script irá:
- ✓ Verificar quais colunas estão faltando
- ✓ Adicionar apenas as colunas que não existem (idempotente)
- ✓ Criar índices e chaves estrangeiras necessárias
- ✓ Mostrar o status final

### Solução 3: Executar o Script de Migração

```bash
# Do diretório raiz do projeto
./run-all-migrations.sh

# Ou com string de conexão personalizada
./run-all-migrations.sh "Host=localhost;Database=primecare;Username=postgres;Password=SuaSenha"
```

### Solução 4: Usar dotnet ef CLI

```bash
cd src/MedicSoft.Api
dotnet ef database update --context MedicSoftDbContext
```

### Solução 5: Recriar o Banco (Apenas Desenvolvimento)

⚠️ **ATENÇÃO**: Isto irá DELETAR todos os dados!

```bash
cd src/MedicSoft.Api
dotnet ef database drop --context MedicSoftDbContext --force
dotnet ef database update --context MedicSoftDbContext
```

## 🔍 Verificação

Após aplicar a correção, verifique se as colunas foram criadas:

```sql
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'public'
  AND LOWER(table_name) = 'appointments'
  AND LOWER(column_name) IN ('ispaid', 'paidat', 'paidbyuserid', 'paymentreceivedby', 'paymentamount', 'paymentmethod')
ORDER BY column_name;
```

Resultado esperado: Todas as 6 colunas devem aparecer.

## 🧪 Testando a Correção

1. Reinicie a aplicação
2. Acesse o endpoint: `POST /api/DataSeeder/seed-demo`
3. O erro não deve mais aparecer

## 🔧 Por Que Este Erro Acontece?

1. O banco de dados foi criado antes das migrações de pagamento serem adicionadas
2. As migrações não foram executadas após atualizar o código
3. Houve um problema de sensibilidade a maiúsculas/minúsculas no PostgreSQL (corrigido neste PR)

## 📚 Informações Técnicas

### Migrações Relacionadas

- `20260121193310_AddPaymentTrackingFields` - Adiciona IsPaid, PaidAt, PaidByUserId, PaymentReceivedBy
- `20260123011851_AddRoomConfigurationAndPaymentDetails` - Adiciona PaymentAmount, PaymentMethod  
- `20260131130000_EnsurePaymentTrackingColumnsExist` - Migração de segurança (corrigida neste PR)

### O Que Foi Corrigido

A migração `20260131130000_EnsurePaymentTrackingColumnsExist.cs` estava usando comparação com case sensitivity ao verificar a existência das colunas. No PostgreSQL, quando tabelas são criadas com identificadores entre aspas (como `"Appointments"`), os nomes são case-sensitive, mas as consultas em `information_schema` requerem comparação case-insensitive.

**Antes:**
```sql
WHERE table_name = 'Appointments' AND column_name = 'IsPaid'
```

**Depois (corrigido):**
```sql
WHERE LOWER(table_name) = 'appointments' AND LOWER(column_name) = 'ispaid'
```

## 🆘 Ainda Com Problemas?

Se o erro persistir após tentar estas soluções:

1. Verifique os logs da aplicação em `logs/`
2. Confirme que o PostgreSQL está rodando:
   ```bash
   podman ps | grep postgres  # ou docker ps | grep postgres
   ```
3. Verifique sua connection string em `appsettings.Development.json`
4. Verifique se você tem permissões suficientes no banco de dados
5. Consulte a documentação completa: [docs/troubleshooting/MISSING_DATABASE_COLUMNS.md](../docs/troubleshooting/MISSING_DATABASE_COLUMNS.md)

## 🛡️ Prevenção

Para evitar este problema no futuro:

1. Sempre execute `./run-all-migrations.sh` após atualizar o código
2. A aplicação aplica migrações automaticamente na inicialização (nenhuma ação manual necessária)
3. Mantenha seu banco de desenvolvimento atualizado
4. Use Docker/Podman Compose para ambiente de desenvolvimento consistente

## 📖 Recursos Adicionais

- **Script SQL de Correção**: [scripts/fix-missing-payment-columns.sql](../scripts/fix-missing-payment-columns.sql)
- **Guia de Migrações**: [MIGRATIONS_GUIDE.md](../MIGRATIONS_GUIDE.md)
- **Resumo Técnico**: [FIX_SUMMARY_ISPAID_COLUMN.md](../FIX_SUMMARY_ISPAID_COLUMN.md)
