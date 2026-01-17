# Local Development Quick Registration

## ⚠️ ATENÇÃO: SOMENTE PARA DESENVOLVIMENTO LOCAL

Esta funcionalidade permite criar rapidamente registros completos (Clínica + Owner + System Admin) para agilizar testes em ambiente de desenvolvimento local.

**IMPORTANTE:** Esta tela NÃO estará disponível em produção e só funciona quando a aplicação está rodando em modo Development.

## Como Usar

### Opção 1: Interface Web (Recomendado)

1. Certifique-se de que a API está rodando em modo Development
2. Acesse: `http://localhost:5000/local-dev-registration.html`
3. Preencha os campos desejados (ou deixe em branco para usar valores padrão)
4. Clique em "Criar Cadastro Completo"
5. Use as credenciais retornadas para fazer login e testar

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
- Verifique se está rodando em modo Development
- Ou adicione `"Development:EnableDevEndpoints": true` no appsettings.Development.json

### Página HTML não carrega
- Verifique se está em modo Development
- A aplicação deve estar configurada para servir arquivos estáticos

### Erro "No subscription plans available"
- Execute primeiro: `POST /api/data-seeder/seed-demo`
- Ou crie planos de assinatura manualmente

## Arquivos Relacionados

- `/src/MedicSoft.Api/Controllers/LocalDevController.cs` - Controller com a lógica
- `/src/MedicSoft.Api/wwwroot/local-dev-registration.html` - Interface web
- `/src/MedicSoft.Api/Program.cs` - Configuração de static files (linha ~288)
