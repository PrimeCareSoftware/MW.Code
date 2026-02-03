# 📮 Como Importar a Coleção da API no Postman

Este arquivo contém todas as APIs do Omni Care Software exportadas para o Postman.

## 📥 Importando no Postman

### Opção 1: Importar pelo Arquivo

1. Abra o **Postman**
2. Clique no botão **"Import"** no canto superior esquerdo
3. Selecione o arquivo **`Omni Care Software-Postman-Collection.json`**
4. Clique em **"Import"**

### Opção 2: Arrastar e Soltar

1. Abra o **Postman**
2. Arraste o arquivo **`Omni Care Software-Postman-Collection.json`** para a janela do Postman
3. A coleção será importada automaticamente

## 🔐 Configurando a Autenticação

A API usa autenticação JWT (Bearer Token). Siga estes passos:

### 1. Gerar Dados de Teste (Opcional)

Se você está começando com um banco de dados vazio:

1. Abra a pasta **"Data Seeder"** na coleção
2. Execute o request **"Seed Demo Data"** (POST /api/data-seeder/seed-demo)
3. Isso criará:
   - Clínica demo com TenantId: `demo-clinic-001`
   - 3 usuários (Admin, Médico, Recepcionista)
   - 6 pacientes
   - 8 procedimentos
   - 5 agendamentos
   - Dados de pagamento

### 2. Fazer Login

1. Abra a pasta **"Auth"** na coleção
2. Execute o request **"Login"** (POST /api/auth/login)
3. As credenciais padrão já estão preenchidas:
   ```json
   {
     "username": "admin",
     "password": "admin123",
     "tenantId": "demo-clinic-001"
   }
   ```
4. Copie o valor do campo **"token"** da resposta

### 3. Configurar o Token

1. Clique na coleção **"Omni Care Software API"** (raiz)
2. Vá para a aba **"Variables"**
3. Cole o token copiado no campo **"Current value"** da variável **"bearer_token"**
4. Clique em **"Save"**

Pronto! Agora todos os requests da coleção usarão automaticamente esse token.

## 🌐 Configurando Ambientes

A coleção vem com variáveis pré-configuradas:

| Variável | Valor Padrão | Descrição |
|----------|-------------|-----------|
| `base_url` | `http://localhost:5000` | URL base da API |
| `bearer_token` | (vazio) | Token JWT obtido após login |
| `tenant_id` | `demo-clinic-001` | ID da clínica/tenant |

### Mudando a URL da API

Se sua API está rodando em outra porta ou servidor:

1. Clique na coleção **"Omni Care Software API"**
2. Vá para a aba **"Variables"**
3. Altere o valor de **"base_url"** (ex: `https://api.medicwarehouse.com`)
4. Clique em **"Save"**

### Mudando o Tenant

Se você tem múltiplas clínicas:

1. Vá para **"Variables"**
2. Altere o valor de **"tenant_id"** para o ID da clínica desejada
3. Clique em **"Save"**

## 📚 Estrutura da Coleção

A coleção está organizada em pastas por funcionalidade:

```
Omni Care Software API/
├── Auth                    # Autenticação
├── Patients               # Gerenciamento de Pacientes
├── Appointments           # Agendamentos
├── Medical Records        # Prontuários Médicos
├── Procedures             # Procedimentos e Serviços
├── Expenses               # Despesas (Contas a Pagar)
├── Reports                # Relatórios e Dashboards
└── Data Seeder            # Geração de Dados de Teste
```

## 🧪 Testando os Endpoints

### Fluxo Básico de Teste

1. **Gerar Dados de Teste** (se necessário)
   - Execute: `Data Seeder > Seed Demo Data`

2. **Autenticar**
   - Execute: `Auth > Login`
   - Configure o token nas variáveis

3. **Listar Pacientes**
   - Execute: `Patients > List Patients`

4. **Criar Agendamento**
   - Execute: `Appointments > Create Appointment`
   - Preencha os IDs necessários (patientId, doctorId)

5. **Visualizar Relatórios**
   - Execute qualquer endpoint da pasta `Reports`

### Substituindo IDs nos Requests

Muitos endpoints precisam de IDs (como patientId, appointmentId, etc.). Para substituir:

1. Abra o request desejado
2. Vá para a aba **"Params"** (para parâmetros de URL) ou **"Body"** (para corpo da requisição)
3. Substitua os valores vazios pelos IDs reais obtidos de outros endpoints
4. Execute o request

## 📖 Documentação Adicional

- **Swagger UI**: http://localhost:5000/swagger (quando a API estiver rodando)
- **README do Projeto**: [README.md](../README.md)
- **Guia de Execução**: [GUIA_EXECUCAO.md](GUIA_EXECUCAO.md)
- **Repositório GitHub**: https://github.com/Omni Care Software/MW.Code

## 💡 Dicas

### Usando Variáveis nos Requests

Você pode criar variáveis personalizadas para reutilizar IDs:

1. Vá para **"Variables"** na coleção
2. Adicione nova variável (ex: `patient_id`)
3. Use nos requests como `{{patient_id}}`

### Salvando Respostas Automaticamente

Você pode usar **Tests** no Postman para salvar automaticamente valores da resposta em variáveis:

```javascript
// Na aba "Tests" do request de Login
pm.test("Save token", function () {
    var jsonData = pm.response.json();
    pm.collectionVariables.set("bearer_token", jsonData.token);
});
```

### Criando Ambientes Múltiplos

Para trabalhar com múltiplos ambientes (Dev, Staging, Production):

1. Clique no ícone de engrenagem (⚙️) no canto superior direito
2. Clique em **"Add"** para criar novo ambiente
3. Adicione as variáveis (base_url, tenant_id, etc.) com valores específicos
4. Selecione o ambiente desejado no dropdown superior

## ❓ Problemas Comuns

### Erro 401 (Unauthorized)

- Verifique se o token JWT está configurado corretamente
- O token pode ter expirado (validade padrão: 60 minutos)
- Execute o Login novamente para obter um novo token

### Erro de Conexão (Connection Refused)

- Verifique se a API está rodando: `cd src/MedicSoft.Api && dotnet run`
- Confirme a URL em `base_url` (deve ser `http://localhost:5293` ou `http://localhost:5000`)

### IDs Inválidos

- Use IDs reais obtidos de outros endpoints
- Execute `Data Seeder > Seed Demo Data` para gerar dados de teste

## 🎉 Pronto!

Agora você pode explorar e testar todos os endpoints da API Omni Care Software usando o Postman!

Se tiver dúvidas ou problemas, consulte a documentação completa no repositório ou abra uma issue no GitHub.
