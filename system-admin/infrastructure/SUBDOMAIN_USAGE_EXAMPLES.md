# Exemplos de Uso: Login por Subdomínio

Este documento fornece exemplos práticos de como usar o novo sistema de login por subdomínio.

## Cenários de Uso

### Cenário 1: Clínica Configurando Subdomínio pela Primeira Vez

**Situação:** Uma clínica nova acabou de se registrar e quer configurar seu subdomínio.

**Passos:**

1. O proprietário da clínica faz login pela primeira vez usando o método tradicional:
   ```
   URL: https://mwsistema.com.br/login
   Username: admin@clinica1
   Password: ********
   Tenant ID: tenant-abc-123
   ```

2. Após o login, o proprietário acessa as configurações da clínica e define o subdomínio:
   ```csharp
   // No backend, através da API
   PUT /api/clinics/{clinicId}
   {
     "subdomain": "clinica1"
   }
   ```

3. A partir de agora, os usuários podem acessar diretamente:
   ```
   https://clinica1.mwsistema.com.br
   ```

### Cenário 2: Login Simplificado para Funcionários

**Situação:** Um médico da clínica quer fazer login.

**Antes (tradicional):**
```
URL: https://mwsistema.com.br/login
Username: dr.silva
Password: ********
Tenant ID: tenant-abc-123  ← Tinha que lembrar/anotar isso!
```

**Depois (com subdomínio):**
```
URL: https://clinica1.mwsistema.com.br/login
Username: dr.silva
Password: ********
(Tenant ID é detectado automaticamente!)
```

**Resultado:** Login mais simples, apenas 2 campos!

### Cenário 3: Marketing e Comunicação

**Situação:** A clínica quer divulgar o sistema para seus pacientes e funcionários.

**Email/WhatsApp para funcionários:**
```
Olá Equipe!

Nosso novo sistema está no ar! 🎉

Acesse: https://clinicaexemplo.mwsistema.com.br
Username: seu.email@clinica.com.br
Senha: enviada por SMS

Qualquer dúvida, entre em contato!
```

**Material impresso (cartão/folder):**
```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━
   CLÍNICA EXEMPLO
   Sistema de Gestão
   
   🌐 clinicaexemplo.mwsistema.com.br
   📱 Entre em contato para suas credenciais
━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

### Cenário 4: Múltiplas Filiais

**Situação:** Uma rede de clínicas com várias unidades.

**Configuração:**
```
Matriz: https://matriz.mwsistema.com.br
Filial Centro: https://centro.mwsistema.com.br
Filial Norte: https://norte.mwsistema.com.br
Filial Sul: https://sul.mwsistema.com.br
```

**Benefícios:**
- Cada unidade tem seu próprio acesso
- Branding individualizado por unidade
- Funcionários só precisam lembrar qual unidade trabalham
- Facilita treinamento e suporte

### Cenário 5: Acesso via Path (Alternativa)

**Situação:** Configuração DNS não disponível ou em transição.

**URLs alternativas funcionam:**
```
https://mwsistema.com.br/clinica1/login
https://mwsistema.com.br/clinica1/dashboard
https://mwsistema.com.br/clinica1/patients
```

**Mesmo comportamento:** Tenant detectado automaticamente!

### Cenário 6: Desenvolvimento e Testes

**Situação:** Desenvolvedor quer testar com diferentes clínicas localmente.

**Configuração do arquivo hosts:**
```
# Windows: C:\Windows\System32\drivers\etc\hosts
# Linux/Mac: /etc/hosts

127.0.0.1   clinic1.localhost
127.0.0.1   clinic2.localhost
127.0.0.1   testclinic.localhost
```

**Teste rápido:**
```bash
# Terminal 1 - Backend
cd src/MedicSoft.Api
dotnet run

# Terminal 2 - Frontend
cd frontend/medicwarehouse-app
npm start

# Navegador
# Acesse: http://clinic1.localhost:4200
```

### Cenário 7: Migração Gradual

**Situação:** Clínica já existente quer migrar para subdomínio sem quebrar acesso existente.

**Fase 1 - Configurar subdomínio:**
```csharp
clinic.SetSubdomain("clinicaantiga");
```

**Fase 2 - Comunicar nova URL:**
- Mantenha ambos funcionando:
  - ✅ Antigo: Login manual com Tenant ID
  - ✅ Novo: https://clinicaantiga.mwsistema.com.br

**Fase 3 - Todos migrados:**
- Desabilitar login manual se desejar (opcional)
- Subdomínio vira único método de acesso

### Cenário 8: Resolução de Problemas

**Situação:** Usuário não consegue fazer login.

**Checklist de verificação:**

1. **Verificar se subdomínio está configurado:**
   ```
   GET /api/tenant/resolve/clinica1
   
   Resposta esperada:
   {
     "tenantId": "...",
     "subdomain": "clinica1",
     "clinicName": "Clínica Exemplo",
     "isActive": true
   }
   ```

2. **Se retornar 404:**
   - Subdomínio não configurado
   - Solução: Usar login tradicional com Tenant ID

3. **Se isActive = false:**
   - Clínica desativada
   - Solução: Contatar suporte

4. **Se DNS não resolve:**
   - Problema de configuração DNS
   - Solução temporária: Usar path-based
     ```
     https://mwsistema.com.br/clinica1/login
     ```

### Cenário 9: API Testing com Subdomain

**Situação:** Testando a API com diferentes tenants.

**Usando cURL:**
```bash
# Login com subdomain no header
curl -X POST https://api.mwsistema.com.br/api/auth/login \
  -H "Host: clinica1.mwsistema.com.br" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "senha123"
  }'

# Ou explicitamente com tenantId (backward compatible)
curl -X POST https://api.mwsistema.com.br/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "senha123",
    "tenantId": "tenant-abc-123"
  }'
```

**Usando Postman:**
```
1. Criar environment variables:
   - subdomain: clinica1
   - base_url: {{subdomain}}.mwsistema.com.br
   
2. Configurar request:
   POST {{base_url}}/api/auth/login
   Body: {
     "username": "admin",
     "password": "senha123"
   }
```

### Cenário 10: Integração com Aplicativo Mobile

**Situação:** App mobile precisa detectar clínica e fazer login.

**Fluxo sugerido:**

1. **Tela inicial - Inserir subdomínio:**
   ```
   ┌─────────────────────────┐
   │  Qual sua clínica?      │
   │  ┌───────────────────┐  │
   │  │ clinica1          │  │
   │  └───────────────────┘  │
   │  [Continuar]            │
   └─────────────────────────┘
   ```

2. **App resolve tenant:**
   ```javascript
   const response = await fetch(
     `https://api.mwsistema.com.br/api/tenant/resolve/${subdomain}`
   );
   const { tenantId, clinicName } = await response.json();
   ```

3. **Tela de login personalizada:**
   ```
   ┌─────────────────────────┐
   │  Clínica Exemplo        │
   │  ┌───────────────────┐  │
   │  │ Usuário          │  │
   │  └───────────────────┘  │
   │  ┌───────────────────┐  │
   │  │ Senha            │  │
   │  └───────────────────┘  │
   │  [Entrar]              │
   └─────────────────────────┘
   ```

4. **Login com tenant resolvido:**
   ```javascript
   const loginResponse = await fetch(
     'https://api.mwsistema.com.br/api/auth/login',
     {
       method: 'POST',
       headers: {
         'Content-Type': 'application/json'
       },
       body: JSON.stringify({
         username,
         password,
         tenantId // Obtido do passo 2
       })
     }
   );
   ```

## Melhores Práticas

### Para Clínicas

1. **Escolha um subdomínio memorável:**
   - ✅ Bom: `clinicacoracaosaude`, `drsilvacardio`
   - ❌ Ruim: `cli123`, `temp`, `test`

2. **Comunique claramente:**
   - Inclua URL em todos os materiais
   - Treine funcionários
   - Tenha suporte disponível na transição

3. **Mantenha consistência:**
   - Use o mesmo subdomínio em todos os canais
   - Não mude frequentemente

### Para Desenvolvedores

1. **Sempre teste ambos os métodos:**
   - Login com subdomínio
   - Login com Tenant ID explícito

2. **Valide entrada do usuário:**
   - Normalize subdomínios (lowercase)
   - Valide formato antes de chamar API

3. **Trate erros graciosamente:**
   - Subdomínio não encontrado → sugerir Tenant ID manual
   - Clínica inativa → mensagem clara
   - Erro de rede → retry com exponential backoff

### Para Administradores

1. **Monitore uso:**
   - Quais clínicas usam subdomínio
   - Taxa de sucesso vs erro
   - Padrões de acesso

2. **Valide subdomínios antes de aprovar:**
   - Verificar se não é ofensivo
   - Verificar unicidade
   - Verificar formato válido

3. **Mantenha documentação atualizada:**
   - Lista de subdomínios ativos
   - Procedimentos de suporte
   - Scripts de troubleshooting

## Próximos Passos

Após implementação básica, considere:

1. **Branding personalizado por clínica**
2. **Domínios customizados** (clinica.com.br → CNAME)
3. **Analytics por subdomínio**
4. **Rate limiting por tenant**
5. **Cache de resolução de subdomain**

---

**Dúvidas?** Consulte o [Guia Completo](SUBDOMAIN_LOGIN_GUIDE.md) ou [Documentação de Segurança](SECURITY_SUMMARY_SUBDOMAIN.md).
