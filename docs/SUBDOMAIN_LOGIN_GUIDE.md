# Guia de Configuração de Subdomínio para Login

## Visão Geral

O sistema agora suporta acesso baseado em subdomínio, permitindo que os usuários acessem suas clínicas sem precisar digitar manualmente o Tenant ID durante o login.

## Formatos Suportados

O sistema suporta dois formatos de acesso:

### 1. Acesso por Subdomínio
```
https://clinic1.mwsistema.com.br
https://clinicaexemplo.mwsistema.com.br
```

### 2. Acesso por Caminho (Path-based)
```
https://mwsistema.com.br/clinic1
https://mwsistema.com.br/clinicaexemplo
```

## Configuração da Clínica

### Passo 1: Definir o Subdomínio

Para configurar o subdomínio de uma clínica, use o método `SetSubdomain()` da entidade `Clinic`:

```csharp
var clinic = await clinicRepository.GetByIdAsync(clinicId, tenantId);
clinic.SetSubdomain("clinic1");
await clinicRepository.UpdateAsync(clinic);
```

### Regras para Subdomínios

Os subdomínios devem seguir estas regras:

- **Caracteres permitidos**: Apenas letras minúsculas (a-z), números (0-9) e hífens (-)
- **Comprimento**: Entre 3 e 63 caracteres
- **Formato**: Deve começar e terminar com letra ou número (não pode começar ou terminar com hífen)
- **Unicidade**: Cada subdomínio deve ser único no sistema

**Exemplos válidos:**
- `clinic1`
- `clinica-exemplo`
- `hospital-sao-paulo`
- `centro-medico-123`

**Exemplos inválidos:**
- `ab` (muito curto)
- `-clinic` (começa com hífen)
- `clinic-` (termina com hífen)
- `Clinic1` (contém letras maiúsculas)
- `clinic@test` (contém caracteres especiais)

## Como Funciona

### Backend

1. **Middleware de Resolução de Tenant** (`TenantResolutionMiddleware`):
   - Intercepta todas as requisições HTTP
   - Extrai o subdomínio do hostname ou do path
   - Consulta o banco de dados para encontrar a clínica correspondente
   - Armazena o `tenantId` no contexto da requisição

2. **AuthController**:
   - O campo `tenantId` agora é opcional no request de login
   - Se não fornecido, usa o valor do contexto (resolvido pelo middleware)
   - Mantém compatibilidade retroativa com login explícito por tenantId

3. **TenantController**:
   - Endpoint `/api/tenant/resolve/{subdomain}`: Resolve informações da clínica por subdomínio
   - Endpoint `/api/tenant/current`: Retorna informações do tenant atual do contexto

### Frontend

1. **TenantResolverService**:
   - Extrai automaticamente o subdomínio ou path da URL atual
   - Fornece métodos para resolver informações da clínica

2. **Login Component**:
   - Detecta automaticamente o tenant da URL
   - Esconde o campo "Tenant ID" quando um subdomínio é detectado
   - Exibe o nome da clínica quando disponível
   - Permite login manual com tenantId se nenhum subdomínio for detectado

## Exemplos de Uso

### Exemplo 1: Login via Subdomínio

**URL**: `https://clinic1.mwsistema.com.br/login`

1. O usuário acessa a URL
2. O sistema detecta o subdomínio `clinic1`
3. O campo "Tenant ID" é automaticamente preenchido e ocultado
4. O usuário insere apenas username e senha
5. Login é realizado com o tenantId correspondente ao subdomínio

### Exemplo 2: Login via Path

**URL**: `https://mwsistema.com.br/clinic1/login`

1. O usuário acessa a URL com path `/clinic1`
2. O sistema detecta `clinic1` no path
3. Mesmo comportamento do exemplo 1

### Exemplo 3: Login Tradicional (Backward Compatible)

**URL**: `https://mwsistema.com.br/login`

1. O usuário acessa a URL sem subdomínio ou path específico
2. O campo "Tenant ID" permanece visível
3. O usuário deve inserir username, senha E tenant ID
4. Login funciona normalmente

## Configuração DNS

Para usar subdomínios, você precisa configurar DNS wildcard:

### Exemplo de Configuração DNS

```
Type    Name        Value                   TTL
A       @           192.0.2.1              300
A       *           192.0.2.1              300
CNAME   www         mwsistema.com.br.      300
```

Isso permitirá que qualquer subdomínio (clinic1, clinic2, etc.) aponte para o mesmo servidor.

### Configuração no Servidor Web

#### Nginx

```nginx
server {
    listen 80;
    server_name *.mwsistema.com.br mwsistema.com.br;

    location / {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

#### Apache

```apache
<VirtualHost *:80>
    ServerName mwsistema.com.br
    ServerAlias *.mwsistema.com.br

    ProxyPass / http://localhost:5000/
    ProxyPassReverse / http://localhost:5000/
    
    ProxyPreserveHost On
</VirtualHost>
```

## Desenvolvimento Local

Durante o desenvolvimento, você pode testar subdomínios localmente editando o arquivo `hosts`:

### Windows
Arquivo: `C:\Windows\System32\drivers\etc\hosts`

### Linux/Mac
Arquivo: `/etc/hosts`

Adicione:
```
127.0.0.1   clinic1.localhost
127.0.0.1   clinic2.localhost
```

Então acesse: `http://clinic1.localhost:4200`

## API Endpoints

### Resolver Tenant por Subdomínio

```http
GET /api/tenant/resolve/{subdomain}
```

**Resposta de Sucesso:**
```json
{
  "tenantId": "tenant-123",
  "subdomain": "clinic1",
  "clinicName": "Clínica Exemplo",
  "clinicId": "guid-here",
  "isActive": true
}
```

### Obter Tenant Atual

```http
GET /api/tenant/current
```

**Resposta de Sucesso:**
```json
{
  "tenantId": "tenant-123",
  "subdomain": "clinic1"
}
```

## Segurança

- Subdomínios são sempre convertidos para lowercase para evitar problemas de case-sensitivity
- A validação garante que apenas caracteres seguros sejam usados
- Clínicas inativas não são resolvidas pelo middleware
- O middleware não interfere em rotas de API que não requerem tenant

## Troubleshooting

### Problema: "No tenant context found"

**Causa**: O subdomínio não foi configurado ou não existe no banco de dados.

**Solução**: 
1. Verifique se o subdomínio foi configurado na clínica
2. Verifique se a clínica está ativa (`IsActive = true`)
3. Tente acessar via URL direta ou com tenantId explícito

### Problema: Login não funciona com subdomínio

**Causa**: Middleware não está sendo executado ou DNS não está configurado corretamente.

**Solução**:
1. Verifique os logs do servidor para ver se o middleware está detectando o subdomínio
2. Verifique a configuração DNS
3. Em desenvolvimento, confirme que o arquivo hosts está configurado

### Problema: Subdomínio retorna 404

**Causa**: Configuração do servidor web não aceita wildcard domains.

**Solução**:
1. Configure o servidor web (Nginx/Apache) para aceitar wildcard domains
2. Reinicie o servidor web após as mudanças

## Benefícios

1. **Melhor UX**: Usuários não precisam lembrar ou digitar Tenant IDs
2. **Branding**: Cada clínica pode ter sua própria URL personalizada
3. **Simplicidade**: Login com apenas 2 campos (username e senha)
4. **Backward Compatible**: Mantém compatibilidade com fluxo antigo
5. **SEO Friendly**: URLs mais amigáveis para busca

## Próximos Passos

Após implementar o sistema de subdomínios, considere:

1. Adicionar página de landing personalizada por clínica
2. Implementar temas/cores personalizados por clínica
3. Adicionar logo personalizada por clínica
4. Implementar domínios customizados (CNAME) para clínicas premium
