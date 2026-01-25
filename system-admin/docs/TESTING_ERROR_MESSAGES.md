# Teste de Mensagens de Erro Amigáveis

Este documento fornece instruções para testar as melhorias de tratamento de erros implementadas.

## Como Testar

### Pré-requisitos
- Backend (.NET) rodando
- Frontend (Angular) rodando
- Acesso ao navegador com ferramentas de desenvolvedor

### Cenários de Teste

#### 1. Erro de Validação (400 Bad Request)

**Backend:**
```bash
# Teste via curl ou Postman
POST /api/patients
Content-Type: application/json

{
  "name": "",
  "cpf": "invalid"
}
```

**Esperado:**
- Status: 400
- Mensagem em português: "Existem erros de validação nos dados fornecidos."
- Detalhes específicos dos campos com erro

**Frontend:**
- Ao tentar salvar um formulário com campos vazios
- Toast de erro deve aparecer com mensagem clara em português
- Nenhum detalhe técnico deve ser visível

#### 2. Erro de Autenticação (401 Unauthorized)

**Backend:**
```bash
POST /api/auth/login
Content-Type: application/json

{
  "username": "usuario_invalido",
  "password": "senha_errada"
}
```

**Esperado:**
- Status: 401
- Mensagem: "Usuário ou senha incorretos."
- NÃO deve diferenciar se usuário existe ou não

**Frontend:**
- Tente fazer login com credenciais inválidas
- Deve exibir mensagem genérica
- Deve redirecionar para tela de login após timeout de sessão

#### 3. Erro de Permissão (403 Forbidden)

**Teste:**
- Com um usuário sem permissões adequadas
- Tente acessar uma funcionalidade restrita

**Esperado:**
- Status: 403
- Mensagem: "Você não tem permissão para realizar esta ação."

#### 4. Recurso Não Encontrado (404 Not Found)

**Backend:**
```bash
GET /api/patients/00000000-0000-0000-0000-000000000000
```

**Esperado:**
- Status: 404
- Mensagem: "Paciente não encontrado." (ou recurso apropriado)
- NÃO deve revelar estrutura de banco de dados

#### 5. Erro de Servidor (500 Internal Server Error)

**Teste:**
- Simule uma exceção não tratada no backend
- Por exemplo, causando um null reference

**Esperado:**
- Status: 500
- Mensagem genérica: "Ocorreu um erro ao processar sua solicitação. Por favor, tente novamente mais tarde."
- Stack trace NUNCA deve ser visível ao usuário
- Detalhes completos devem estar nos logs do servidor

#### 6. Erro de Conexão (0 Network Error)

**Teste:**
- Desligue o backend
- Tente fazer uma requisição do frontend

**Esperado:**
- Mensagem: "Não foi possível conectar ao servidor. Verifique sua conexão com a internet."
- Toast de erro vermelho deve aparecer

### Verificações de Segurança

#### ❌ O QUE NÃO DEVE APARECER

1. Stack traces
2. Nomes de assemblies (.dll)
3. Caminhos de arquivos do servidor
4. Queries SQL
5. Strings de conexão
6. Nomes de variáveis de código
7. Mensagens em inglês (exceto em logs do servidor)
8. Detalhes de exceções internas

#### ✅ O QUE DEVE APARECER

1. Mensagens claras em português
2. Descrição do que deu errado
3. Orientação sobre o que fazer
4. Feedback visual (toast colorido)
5. Código de erro genérico (opcional)

### Ferramentas para Teste

#### Chrome DevTools
1. Abra o Console (F12)
2. Vá para a aba Network
3. Faça uma requisição que gera erro
4. Clique na requisição falhada
5. Verifique a resposta em "Response"

#### Verificar Logs do Backend
```bash
# Logs devem conter detalhes completos
tail -f logs/app.log

# Procure por:
# - Stack traces completos
# - Detalhes de exceção
# - Contexto da requisição
```

### Teste Automatizado

Você pode usar este script bash para testar vários endpoints:

```bash
#!/bin/bash

BASE_URL="http://localhost:5000/api"

echo "=== Testando Mensagens de Erro ==="

# Teste 1: Login inválido
echo -e "\n1. Teste de login inválido:"
curl -X POST "$BASE_URL/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"invalid","password":"wrong"}' \
  -w "\nStatus: %{http_code}\n"

# Teste 2: Validação de paciente
echo -e "\n2. Teste de validação (campos vazios):"
curl -X POST "$BASE_URL/patients" \
  -H "Content-Type: application/json" \
  -d '{"name":"","cpf":""}' \
  -w "\nStatus: %{http_code}\n"

# Teste 3: Paciente não encontrado
echo -e "\n3. Teste de recurso não encontrado:"
curl -X GET "$BASE_URL/patients/00000000-0000-0000-0000-000000000000" \
  -w "\nStatus: %{http_code}\n"

echo -e "\n=== Testes Concluídos ==="
```

### Checklist de Validação

- [ ] Todas as mensagens estão em português
- [ ] Nenhum stack trace é visível ao usuário
- [ ] Toasts de erro aparecem automaticamente
- [ ] Mensagens de validação são específicas e claras
- [ ] Erro 401 redireciona para login
- [ ] Mensagens de erro de senha não revelam se usuário existe
- [ ] Logs do servidor contêm informações detalhadas
- [ ] Interface não trava após erros
- [ ] Usuário consegue continuar usando o sistema após erro

### Reportando Problemas

Se encontrar alguma mensagem em inglês ou detalhes técnicos expostos:

1. Capture um screenshot
2. Anote a URL e ação que causou o erro
3. Copie a resposta do Network tab
4. Crie uma issue no GitHub com estes detalhes

## Conclusão

Estes testes garantem que:
- ✅ Usuários recebem feedback claro e útil
- ✅ Informações sensíveis não são expostas
- ✅ Sistema é mais seguro e profissional
- ✅ Experiência do usuário é melhorada

Para mais detalhes técnicos, consulte:
- `docs/ERROR_HANDLING_PT.md` - Documentação completa
- `docs/SECURITY_SUMMARY_ERROR_HANDLING.md` - Análise de segurança
