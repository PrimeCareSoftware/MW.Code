# Quick Reference: Database Migrations

## TL;DR - Para Desenvolvedores

### Executar TODAS as migrations de uma vez

**Linux/macOS:**
```bash
./run-all-migrations.sh
```

**Windows:**
```powershell
.\run-all-migrations.ps1
```

### Com connection string customizada

**Linux/macOS:**
```bash
./run-all-migrations.sh "Host=localhost;Database=medicsoft;Username=postgres;Password=suasenha"
```

**Windows:**
```powershell
.\run-all-migrations.ps1 -ConnectionString "Host=localhost;Database=medicsoft;Username=postgres;Password=suasenha"
```

## O que o script faz?

1. ✅ Aplica migrations da **aplicação principal** (7 migrations)
2. ✅ Aplica migrations do **Patient Portal** (1 migration)
3. ✅ Aplica migrations de **6 microserviços** (1 migration cada)
4. ✅ Aplica migrations do **Telemedicine** (1 migration)

**Total: 16 migrations em 9 DbContexts diferentes**

## Quando usar?

- ✅ **Primeira vez** que você clonar o repositório
- ✅ Depois de **pull** de mudanças que incluem novas migrations
- ✅ Quando você **criar um novo microserviço** com banco de dados
- ✅ Durante **desenvolvimento** para manter banco sincronizado

## FAQ

**P: E se eu já apliquei algumas migrations?**  
R: Sem problema! O EF Core é inteligente e pula migrations já aplicadas.

**P: Posso rodar o script várias vezes?**  
R: Sim! É completamente seguro e idempotente.

**P: E se uma migration falhar?**  
R: O script continua com as próximas. Você verá um erro no log mas não para tudo.

**P: Como criar uma nova migration?**  
R: Veja o [MIGRATIONS_GUIDE.md](../MIGRATIONS_GUIDE.md) completo para detalhes.

## Documentação Completa

📖 **[MIGRATIONS_GUIDE.md](../MIGRATIONS_GUIDE.md)** - Guia completo e detalhado
