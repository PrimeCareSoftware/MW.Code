# Guia de Seeders - MedicWarehouse

## Visão Geral

O sistema MedicWarehouse possui seeders abrangentes para popular o banco de dados com dados de demonstração realísticos. Isso permite testar todas as funcionalidades do sistema sem precisar inserir dados manualmente.

### ✨ Características Principais

- ✅ **Consistência de Dados**: Todos os dados são criados com relacionamentos válidos e datas consistentes
- ✅ **Transações**: Todas as operações são executadas em uma transação - rollback automático em caso de erro
- ✅ **Dados Históricos**: Suporte para criação de agendamentos e registros passados para demonstração completa
- ✅ **Validação de FK**: Ordem correta de inserção respeitando dependências entre entidades
- ✅ **Dados Realísticos**: Informações médicas, pacientes e procedimentos com dados reais para testes

## Endpoints de Seeder

### 1. Informações sobre Dados Demo
```
GET /api/data-seeder/demo-info
```
Retorna informações sobre quais dados serão criados pelos seeders, incluindo quantidade e tipos de entidades.

### 2. Popular Dados Demo
```
POST /api/data-seeder/seed-demo
```
Popula o banco de dados com dados demo completos para teste do sistema.

### 3. Criar Owner do Sistema
```
POST /api/data-seeder/seed-system-owner
```
Cria um owner/administrador do sistema (apenas em ambiente de desenvolvimento).

## Dados Criados pelos Seeders

### 📊 Resumo Quantitativo

| Entidade | Quantidade | Descrição |
|----------|-----------|-----------|
| **Planos de Assinatura** | 5 | Trial, Básico, Standard, Premium, Enterprise |
| **Clínicas** | 1 | Clínica Demo MedicWarehouse |
| **Assinaturas** | 1 | Assinatura ativa no plano Standard |
| **Proprietários (Owners)** | 1 | Proprietário da clínica demo |
| **Usuários** | 3 | Admin, Médico, Recepcionista |
| **Pacientes** | 6 | Incluindo 2 crianças com responsável |
| **Procedimentos** | 8 | Consultas, exames, vacinas, etc. |
| **Agendamentos** | 5 | Passados, hoje e futuros |
| **Procedimentos de Agendamento** | 3 | Vinculados aos agendamentos |
| **Pagamentos** | 2 | Pagamentos processados |
| **Medicamentos** | 8 | Diversos tipos de medicamentos |
| **Prontuários Médicos** | 2 | Consultas finalizadas |
| **Itens de Prescrição** | 3 | Vinculados aos prontuários |
| **Templates de Prescrição** | 4 | Templates reutilizáveis |
| **Templates de Prontuário** | 3 | Clínica geral, cardiologia, pediatria |
| **Notificações** | 5 | SMS, WhatsApp, Email |
| **Rotinas de Notificação** | 5 | Notificações automatizadas |
| **Despesas** | 10 | Várias categorias e status |
| **Solicitações de Exames** | 5 | Laboratoriais, imagem, cardiológicos |

---

## 🔐 Credenciais de Acesso

### Proprietário (Owner)
- **Username:** `owner.demo`
- **Password:** `Owner@123`
- **Email:** owner@clinicademo.com.br
- **Tenant ID:** `demo-clinic-001`

### Administrador do Sistema
- **Username:** `admin`
- **Password:** `Admin@123`
- **Email:** admin@clinicademo.com.br
- **Role:** SystemAdmin
- **Tenant ID:** `demo-clinic-001`

### Médico
- **Username:** `dr.silva`
- **Password:** `Doctor@123`
- **Email:** joao.silva@clinicademo.com.br
- **Role:** Doctor
- **CRM:** CRM-123456
- **Especialidade:** Clínico Geral
- **Tenant ID:** `demo-clinic-001`

### Recepcionista
- **Username:** `recep.maria`
- **Password:** `Recep@123`
- **Email:** maria.santos@clinicademo.com.br
- **Role:** Receptionist
- **Tenant ID:** `demo-clinic-001`

---

## 📋 Detalhes das Entidades Criadas

### 1. Planos de Assinatura

#### Trial Gratuito
- **Preço:** R$ 0,00/mês
- **Período de Teste:** 30 dias
- **Máximo de Usuários:** 3
- **Máximo de Pacientes:** 50
- **Recursos:** Funcionalidades básicas

#### Básico
- **Preço:** R$ 99,90/mês
- **Período de Teste:** 15 dias
- **Máximo de Usuários:** 5
- **Máximo de Pacientes:** 100
- **Recursos:** Relatórios, Notificações SMS

#### Standard (Plano da Clínica Demo)
- **Preço:** R$ 199,90/mês
- **Período de Teste:** 15 dias
- **Máximo de Usuários:** 15
- **Máximo de Pacientes:** 500
- **Recursos:** Todos os recursos incluindo WhatsApp e TISS

#### Premium
- **Preço:** R$ 399,90/mês
- **Período de Teste:** 15 dias
- **Máximo de Usuários:** 50
- **Máximo de Pacientes:** 2.000
- **Recursos:** Todos os recursos premium

#### Enterprise
- **Preço:** R$ 999,90/mês
- **Período de Teste:** 30 dias
- **Máximo de Usuários:** 200
- **Máximo de Pacientes:** 10.000
- **Recursos:** Suporte dedicado e recursos enterprise

### 2. Pacientes

Os seeders criam 6 pacientes incluindo:
- **Carlos Alberto Santos** - Hipertensão arterial controlada
- **Ana Maria Oliveira** - Diabetes tipo 2
- **Pedro Henrique Costa** - Paciente sem condições especiais
- **Juliana Martins Silva** - Responsável pelas crianças
- **Lucas Martins Silva** (criança) - Asma leve
- **Sofia Martins Silva** (criança) - Alergia à lactose

### 3. Procedimentos Médicos

8 procedimentos variados:
- Consulta Médica Geral (R$ 150,00)
- Consulta Cardiológica (R$ 250,00)
- Exame de Sangue Completo (R$ 80,00)
- Eletrocardiograma (R$ 120,00)
- Vacina Influenza (R$ 50,00)
- Fisioterapia Sessão (R$ 100,00)
- Sutura Pequeno Porte (R$ 200,00)
- Retorno Consulta (R$ 80,00)

### 4. Agendamentos

5 agendamentos em diferentes estados:
- **2 Passados:** Consultas finalizadas (7 e 5 dias atrás)
- **1 Hoje:** Consulta confirmada
- **2 Futuros:** Consultas agendadas (3 dias à frente)

### 5. Medicamentos

8 medicamentos de diferentes categorias:
- Amoxicilina (Antibiótico)
- Dipirona Sódica (Analgésico)
- Ibuprofeno (Anti-inflamatório)
- Losartana Potássica (Anti-hipertensivo)
- Omeprazol (Antiácido)
- Loratadina (Anti-histamínico)
- Metformina (Antidiabético)
- Vitamina D3 (Vitamina)

### 6. Notificações

5 notificações em diferentes estados e canais:
- SMS, WhatsApp e Email
- Estados: Enviada, Entregue, Lida
- Tipos: Lembrete de consulta, Confirmação, Lembrete de pagamento

### 7. Rotinas de Notificação

5 rotinas automatizadas:
1. **Lembrete 24h antes** - WhatsApp
2. **Lembrete 2h antes** - SMS
3. **Confirmação de agendamento** - Email
4. **Aniversário do paciente** - WhatsApp
5. **Pesquisa de satisfação** - Email (24h após consulta)

### 8. Despesas

10 despesas com diferentes categorias e status:
- **Pagas:** Aluguel, energia, internet, limpeza, material médico, marketing
- **Pendentes:** Software de gestão, material médico, contador
- **Vencidas:** Manutenção de ar condicionado
- **Canceladas:** Curso de atualização médica

Categorias incluem:
- Aluguel (R$ 3.500,00)
- Utilidades (R$ 450,00 + R$ 199,90)
- Materiais (R$ 350,00 + R$ 890,00)
- Software (R$ 199,90)
- Marketing (R$ 500,00)
- Serviços Profissionais (R$ 650,00)
- Manutenção (R$ 280,00)
- Treinamento (R$ 1.200,00 - cancelado)

### 9. Solicitações de Exames

5 solicitações em diferentes estados e tipos:
- **Laboratoriais:** Hemograma, glicemia, HbA1c
- **Cardiológicos:** ECG, Ecocardiograma
- **Imagem:** Raio-X de tórax
- **Ultrassom:** Abdômen total

Estados:
- Completados (2)
- Agendados (1)
- Pendentes (2)

---

## 🚀 Como Usar

### Passo 1: Verificar Informações
```bash
curl -X GET http://localhost:5000/api/data-seeder/demo-info
```

### Passo 2: Popular Banco de Dados
```bash
curl -X POST http://localhost:5000/api/data-seeder/seed-demo
```

### Passo 3: Login no Sistema
Use qualquer uma das credenciais acima para acessar o sistema:

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123",
    "tenantId": "demo-clinic-001"
  }'
```

---

## ⚠️ Observações Importantes

### Tenant ID
Todos os dados demo são criados com o **Tenant ID**: `demo-clinic-001`

### Execução Única
O seeder verifica se já existem dados para o tenant antes de criar novos. Se dados já existirem, retorna erro:
```json
{
  "error": "Demo data already exists for this tenant"
}
```

### Ambiente de Produção
O endpoint de criação de system owner só funciona em:
- Ambiente de desenvolvimento (`IsDevelopment`)
- Ou quando `Development:EnableDevEndpoints` está configurado como `true`

### Relacionamentos
Os dados são criados com relacionamentos realistas:
- Pacientes vinculados à clínica
- Agendamentos com procedimentos
- Prontuários com prescrições
- Despesas com diferentes fornecedores
- Exames vinculados aos agendamentos

---

## 🧪 Casos de Teste Cobertos

Os seeders criam dados que permitem testar:

### Funcionalidades Básicas
- ✅ Gerenciamento de pacientes (adultos e crianças)
- ✅ Agendamento de consultas
- ✅ Registro de prontuários médicos
- ✅ Prescrições médicas
- ✅ Gestão de pagamentos

### Funcionalidades Avançadas
- ✅ Notificações multi-canal (SMS, WhatsApp, Email)
- ✅ Rotinas de notificação automatizadas
- ✅ Controle de despesas
- ✅ Solicitação e acompanhamento de exames
- ✅ Templates de prescrição e prontuário
- ✅ Gestão de assinaturas e planos

### Cenários Específicos
- ✅ Pacientes com condições médicas especiais
- ✅ Crianças com responsáveis
- ✅ Agendamentos em diferentes estados (pendente, confirmado, finalizado)
- ✅ Pagamentos em diferentes métodos (dinheiro, cartão, PIX, transferência)
- ✅ Despesas em diferentes estados (pago, pendente, vencido, cancelado)
- ✅ Exames em diferentes estados (pendente, agendado, concluído)

---

## 📝 Templates Disponíveis

### Templates de Prescrição
1. **Receita Antibiótico Amoxicilina**
2. **Receita Anti-hipertensivo**
3. **Receita Analgésico Simples**
4. **Receita Diabetes**

### Templates de Prontuário
1. **Consulta Clínica Geral**
2. **Consulta Cardiológica**
3. **Consulta Pediátrica**

---

## 🔄 Limpeza de Dados

Para limpar os dados demo e recomeçar:

1. **Opção 1:** Deletar a clínica e todos os dados relacionados através da API
2. **Opção 2:** Recriar o banco de dados
3. **Opção 3:** Usar um novo Tenant ID para testes isolados

---

## 🔒 Garantias de Consistência

### Transações
Todas as operações de seeding são executadas dentro de uma transação de banco de dados:
- ✅ Se todas as operações forem bem-sucedidas, a transação é confirmada (commit)
- ✅ Se qualquer operação falhar, todas as mudanças são revertidas (rollback)
- ✅ Garante que o banco de dados nunca fica em estado inconsistente

### Ordem de Inserção
Os dados são criados na ordem correta respeitando todas as dependências:
1. Planos de Assinatura (sem dependências)
2. Clínica
3. Assinatura da Clínica
4. Owner e Usuários
5. Procedimentos e Pacientes
6. Links Paciente-Clínica
7. Agendamentos
8. Procedimentos de Agendamento, Pagamentos
9. Medicamentos e Prontuários Médicos
10. Prescrições e Templates
11. Notificações e Rotinas
12. Despesas e Solicitações de Exames

### Validações
- ✅ Verifica se dados demo já existem antes de criar
- ✅ Todas as foreign keys são válidas
- ✅ Datas são consistentes entre entidades relacionadas
- ✅ Validações de negócio são respeitadas

---

## 💡 Dicas de Uso

1. **Testar Fluxo Completo:** Use os agendamentos passados para ver prontuários completos
2. **Testar Notificações:** As rotinas de notificação estão configuradas e podem ser testadas
3. **Testar Gestão Financeira:** Use as despesas para testar relatórios financeiros
4. **Testar Multi-usuário:** Faça login com diferentes usuários para testar permissões
5. **Testar Prescrições:** Use os templates para criar novas prescrições rapidamente

---

## 🆘 Troubleshooting

### Erro: "Demo data already exists"
**Solução:** Os dados já foram criados. Use a API para gerenciar ou deletar os dados existentes.

### Erro: "This endpoint is only available in Development"
**Solução:** Configure `Development:EnableDevEndpoints: true` no appsettings ou execute em modo Development.

### Dados não aparecem na consulta
**Solução:** Verifique se está usando o Tenant ID correto: `demo-clinic-001`

### Erro durante o seeding
**Solução:** Graças às transações, nenhum dado parcial é inserido. Verifique os logs para identificar o problema específico. O banco de dados permanece em estado consistente.

---

## 📚 Referências

- [Documentação da API](../README.md)
- [Guia de Autenticação](./AUTHENTICATION_GUIDE.md)
- [Postman Collection](../MedicWarehouse-Postman-Collection.json)
