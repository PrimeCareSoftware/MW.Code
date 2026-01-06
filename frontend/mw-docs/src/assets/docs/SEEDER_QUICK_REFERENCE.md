# Guia Rápido - Seeders MedicWarehouse

## 🎯 Objetivo
Popular o banco de dados com dados demo realísticos para teste completo do sistema.

## 🚀 Uso Rápido

### 1️⃣ Ver o que será criado
```bash
GET /api/data-seeder/demo-info
```

### 2️⃣ Criar todos os dados
```bash
POST /api/data-seeder/seed-demo
```

### 3️⃣ Fazer login
Use qualquer usuário abaixo:

| Usuário | Senha | Role |
|---------|-------|------|
| owner.demo | Owner@123 | Owner |
| admin | Admin@123 | SystemAdmin |
| dr.silva | Doctor@123 | Doctor |
| recep.maria | Recep@123 | Receptionist |

**Tenant ID:** `demo-clinic-001`

## 📊 O que é criado

- ✅ 5 Planos de assinatura
- ✅ 1 Clínica demo completa
- ✅ 1 Assinatura ativa
- ✅ 1 Proprietário (owner)
- ✅ 3 Usuários (admin, médico, recepcionista)
- ✅ 6 Pacientes (incluindo crianças)
- ✅ 8 Procedimentos médicos
- ✅ 5 Agendamentos (passados, hoje, futuros)
- ✅ 2 Pagamentos processados
- ✅ 8 Medicamentos
- ✅ 2 Prontuários médicos
- ✅ 3 Prescrições
- ✅ 4 Templates de prescrição
- ✅ 3 Templates de prontuário
- ✅ 5 Notificações
- ✅ 5 Rotinas de notificação
- ✅ 10 Despesas
- ✅ 5 Solicitações de exames

## 🔑 Exemplo de Login via API

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123",
    "tenantId": "demo-clinic-001"
  }'
```

## ⚠️ Importante

- ⚠️ Só funciona uma vez por tenant (demo-clinic-001)
- ⚠️ Se já existem dados, retorna erro
- ⚠️ System owner só funciona em desenvolvimento
- ✅ Todos os dados têm relacionamentos realistas

## 📖 Documentação Completa

Veja [SEEDER_GUIDE.md](./SEEDER_GUIDE.md) para detalhes completos sobre:
- Todos os dados criados
- Credenciais completas
- Casos de teste cobertos
- Troubleshooting
