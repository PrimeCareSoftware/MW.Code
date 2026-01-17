# Local Development Quick Registration

## ⚠️ ATENÇÃO: SOMENTE PARA DESENVOLVIMENTO LOCAL

Esta funcionalidade permite criar rapidamente registros completos (Clínica + Owner + System Admin) para agilizar testes em ambiente de desenvolvimento local.

**IMPORTANTE:** Esta tela NÃO estará disponível em produção e só funciona quando a aplicação está rodando em modo Development.

## Pré-requisitos

Antes de usar esta funcionalidade, você precisa:

1. **Banco de dados rodando:**
   ```bash
   docker compose up -d postgres
   ```

2. **Dados iniciais (planos de assinatura):**
   ```bash
   # Após iniciar a API, execute:
   curl -X POST http://localhost:5000/api/data-seeder/seed-demo
   ```
   
   Ou, se quiser apenas os planos:
   ```bash
   # Use o endpoint de registro normal uma vez para criar os planos automaticamente
   ```

3. **API rodando em modo Development:**
   ```bash
   cd src/MedicSoft.Api
   dotnet run --environment Development
   ```

## Como Usar

### Opção 1: Interface Web (Recomendado)

1. Certifique-se de que a API está rodando em modo Development
2. Acesse: `http://localhost:5000/local-dev-registration.html`
3. Preencha os campos desejados (ou deixe em branco para usar valores padrão)
4. Clique em "Criar Cadastro Completo"
5. Use as credenciais retornadas para fazer login e testar

> **Dica:** Para testar rapidamente, deixe todos os campos em branco e clique em "Criar Cadastro Completo". O sistema usará valores padrão.

### Opção 2: API Direta

**Endpoint:** `POST /api/local-dev/quick-register`

**Exemplo de requisição com valores customizados:**
```json
{
  "clinicName": "Clínica São Paulo",
  "ownerUsername": "owner",
  "ownerPassword": "Owner@123",
  "ownerEmail": "owner@teste.local",
  "ownerName": "Dr. João Silva",
  "adminUsername": "admin",
  "adminPassword": "Admin@123",
  "adminEmail": "admin@teste.local",
  "adminName": "Administrador"
}
```

**Exemplo de requisição com valores padrão (body vazio):**
```json
{}
```

**Resposta de sucesso:**
```json
{
  "message": "Quick registration completed successfully!",
  "clinic": {
    "id": "guid",
    "name": "Clínica Teste Local",
    "tenantId": "clinica-teste-local"
  },
  "owner": {
    "id": "guid",
    "username": "owner",
    "password": "Owner@123",
    "email": "owner@teste.local",
    "loginEndpoint": "/api/auth/owner-login"
  },
  "systemAdmin": {
    "id": "guid",
    "username": "admin",
    "password": "Admin@123",
    "email": "admin@teste.local",
    "loginEndpoint": "/api/auth/login"
  },
  "note": "Use the credentials above to login and test the system"
}
```

## O que é criado?

Quando você usa esta funcionalidade, o sistema cria:

1. **Clínica completa** com todos os dados necessários (endereço, CNPJ gerado automaticamente, etc.)
2. **Owner (Proprietário)** com perfil de proprietário da clínica
3. **System Admin** com perfil de administrador do sistema e acesso total
4. **Assinatura** usando plano Trial (ou primeiro plano ativo disponível)
5. **Perfil de Acesso** SystemAdmin com todas as permissões

## Valores Padrão

Se você não fornecer valores na requisição, os seguintes valores padrão serão usados:

- **Clínica:** "Clínica Teste Local"
- **Owner Username:** "owner"
- **Owner Password:** "Owner@123"
- **Owner Email:** "owner@teste.local"
- **Owner Name:** "Proprietário Teste"
- **Admin Username:** "admin"
- **Admin Password:** "Admin@123"
- **Admin Email:** "admin@teste.local"
- **Admin Name:** "Administrador Teste"

## Segurança

Esta funcionalidade possui as seguintes proteções:

1. **Só funciona em Development:** O endpoint retorna 403 Forbidden em produção
2. **Configuração adicional:** Requer `Development:EnableDevEndpoints = true` no appsettings.json OU ambiente Development
3. **Interface Web:** O arquivo HTML só é servido em modo Development (via `app.UseStaticFiles()` dentro do bloco `if (app.Environment.IsDevelopment())`)

## Como Fazer Login

### Login como Owner:
```
POST /api/auth/owner-login
{
  "username": "owner",
  "password": "Owner@123",
  "tenantId": "<tenantId retornado>"
}
```

### Login como System Admin:
```
POST /api/auth/login
{
  "username": "admin",
  "password": "Admin@123"
}
```

## Outras Funcionalidades de Desenvolvimento

- `GET /api/local-dev/info` - Informações sobre os endpoints disponíveis
- `POST /api/data-seeder/seed-demo` - Criar dados de demonstração completos
- `POST /api/data-seeder/seed-system-owner` - Criar apenas system owner
- `DELETE /api/data-seeder/clear-database` - Limpar todos os dados do banco

## Troubleshooting

### Erro 403 Forbidden
- Verifique se está rodando em modo Development:
  ```bash
  dotnet run --environment Development
  ```
- Ou adicione `"Development:EnableDevEndpoints": true` no appsettings.Development.json

### Página HTML não carrega
- Verifique se está em modo Development
- A aplicação deve estar configurada para servir arquivos estáticos (já configurado no Program.cs linha ~288)
- Acesse: `http://localhost:5000/local-dev-registration.html` (porta pode variar)

### Erro "No subscription plans available"
- **Opção 1 (Recomendado):** Execute o seed completo:
  ```bash
  curl -X POST http://localhost:5000/api/data-seeder/seed-demo
  ```
- **Opção 2:** Faça um registro normal primeiro usando o endpoint `/api/registration` para criar os planos automaticamente
- **Opção 3:** Crie planos de assinatura manualmente via SQL ou API

### Erro de conexão com banco de dados
- Verifique se o PostgreSQL está rodando:
  ```bash
  docker compose up -d postgres
  docker ps | grep primecare-postgres
  ```
- Verifique a connection string no `appsettings.Development.json`
- Teste a conexão:
  ```bash
  docker exec -it primecare-postgres psql -U postgres -d primecare -c "\dt"
  ```

## Testando a Implementação

### 1. Teste o endpoint info:
```bash
curl http://localhost:5000/api/local-dev/info
```

Resposta esperada:
```json
{
  "title": "Local Development Quick Registration",
  "description": "...",
  "endpoints": { ... }
}
```

### 2. Teste a criação rápida (com valores padrão):
```bash
curl -X POST http://localhost:5000/api/local-dev/quick-register \
  -H "Content-Type: application/json" \
  -d '{}'
```

### 3. Teste a criação rápida (com valores personalizados):
```bash
curl -X POST http://localhost:5000/api/local-dev/quick-register \
  -H "Content-Type: application/json" \
  -d '{
    "clinicName": "Clínica Teste API",
    "ownerUsername": "testowner",
    "ownerPassword": "Test@123",
    "ownerEmail": "testowner@teste.local",
    "adminUsername": "testadmin",
    "adminPassword": "TestAdmin@123",
    "adminEmail": "testadmin@teste.local"
  }'
```

### 4. Teste o login com as credenciais criadas:
```bash
# Owner login
curl -X POST http://localhost:5000/api/auth/owner-login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "owner",
    "password": "Owner@123",
    "tenantId": "clinica-teste-local"
  }'

# System Admin login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123"
  }'
```

## Arquivos Relacionados

- `/src/MedicSoft.Api/Controllers/LocalDevController.cs` - Controller com a lógica
- `/src/MedicSoft.Api/wwwroot/local-dev-registration.html` - Interface web
- `/src/MedicSoft.Api/Program.cs` - Configuração de static files (linha ~288)
