# Guia de Propriedade de Múltiplas Clínicas (Multi-Clinic Ownership)

## Visão Geral

Este documento descreve a nova funcionalidade que permite que um owner (proprietário) seja dono de múltiplas clínicas, cada uma com sua própria licença/assinatura independente.

## 1. Conceito e Motivação

### 1.1 Cenário de Negócio

Muitos profissionais de saúde e empreendedores do setor expandem seus negócios abrindo múltiplas clínicas:
- **Franquias**: Proprietário abre várias unidades da mesma marca
- **Especialidades**: Clínicas em diferentes especialidades médicas
- **Localizações**: Clínicas em diferentes bairros ou cidades
- **Parcerias**: Sócios em múltiplos empreendimentos

### 1.2 Modelo de Licenciamento

**Regra de Negócio Principal:**
> Cada clínica requer uma licença/assinatura separada e independente

**Justificativa:**
- Cada clínica é uma entidade legal separada (CNPJ diferente)
- Diferentes volumes de atendimento e necessidades
- Flexibilidade para escolher planos diferentes
- Controle financeiro independente por unidade

### 1.3 Portal do Owner

O proprietário terá acesso a um **portal centralizado** onde pode:
- Visualizar todas as suas clínicas
- Gerenciar assinaturas de cada clínica
- Trocar entre clínicas rapidamente
- Ver métricas consolidadas (opcional)
- Administrar usuários de cada clínica

## 2. Arquitetura

### 2.1 Relacionamento Atual vs Novo

**Antes (1:1):**
```
Owner (1) -----> (1) Clinic
```
- Um owner vinculado diretamente a uma clínica via `ClinicId`
- Limitação: Owner não pode gerenciar múltiplas clínicas

**Depois (N:N):**
```
Owner (N) <-----> (N) Clinic
           ^
           |
    OwnerClinicLink
```
- Relacionamento muitos-para-muitos através de `OwnerClinicLink`
- Um owner pode ter N clínicas
- Uma clínica pode ter N owners (co-proprietários)
- Cada link tem propriedades próprias

### 2.2 Entidade OwnerClinicLink

```csharp
public class OwnerClinicLink : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public Guid ClinicId { get; private set; }
    public DateTime LinkedDate { get; private set; }
    public bool IsActive { get; private set; }
    
    // Propriedade primária
    public bool IsPrimaryOwner { get; private set; }
    
    // Papel do owner nesta clínica
    public string? Role { get; private set; }  // "Owner", "Co-Owner", "Partner"
    
    // Participação societária (opcional)
    public decimal? OwnershipPercentage { get; private set; }
    
    // Controle de ativação
    public DateTime? InactivatedDate { get; private set; }
    public string? InactivationReason { get; private set; }
    
    // Navigation properties
    public Owner? Owner { get; private set; }
    public Clinic? Clinic { get; private set; }
}
```

### 2.3 Mudanças na Entidade Owner

A entidade `Owner` mantém a propriedade `ClinicId` por compatibilidade, mas:
- `ClinicId` é **nullable** para system owners
- Para clinic owners, serve como referência de "clínica padrão"
- Novas funcionalidades devem usar `OwnerClinicLink`

### 2.4 Licenciamento Independente

```
Owner: João Silva
├── Clínica A (CNPJ: 11.111.111/0001-11)
│   ├── Assinatura: Plano Premium - R$ 320/mês
│   ├── Status: Ativa
│   └── Próximo pagamento: 15/12/2024
│
├── Clínica B (CNPJ: 22.222.222/0001-22)
│   ├── Assinatura: Plano Basic - R$ 190/mês
│   ├── Status: Trial (10 dias restantes)
│   └── Próximo pagamento: 25/11/2024
│
└── Clínica C (CNPJ: 33.333.333/0001-33)
    ├── Assinatura: Plano Standard - R$ 240/mês
    ├── Status: Payment Overdue
    └── Próximo pagamento: Vencido (05/11/2024)
```

Cada clínica mantém sua própria `ClinicSubscription` vinculada a `SubscriptionPlan`.

## 3. Casos de Uso

### 3.1 Owner Registra Nova Clínica

**Fluxo:**
1. Owner já cadastrado faz login no sistema
2. Acessa "Adicionar Nova Clínica" no portal
3. Preenche dados da nova clínica (CNPJ, endereço, etc.)
4. Escolhe plano de assinatura
5. Sistema cria:
   - Nova `Clinic`
   - Nova `ClinicSubscription`
   - Novo `OwnerClinicLink` (com `IsPrimaryOwner = true`)
6. Owner pode começar a usar a nova clínica imediatamente

**Regras:**
- CNPJ deve ser único
- Nova clínica começa com trial de 15 dias (se disponível no plano)
- Owner é automaticamente o primary owner

### 3.2 Owner Adiciona Co-Proprietário

**Fluxo:**
1. Primary owner acessa gestão da clínica
2. Convida outro owner por email ou username
3. Sistema valida se o usuário existe
4. Cria novo `OwnerClinicLink`:
   - `IsPrimaryOwner = false`
   - `Role = "Co-Owner"`
   - `OwnershipPercentage` (opcional)
5. Co-proprietário recebe notificação e aceita convite
6. Co-proprietário tem acesso à clínica

**Regras:**
- Apenas primary owner pode adicionar co-owners
- Co-owner tem acesso completo, mas não pode remover primary owner
- Percentuais de participação são opcionais e informativos

### 3.3 Owner Troca Entre Clínicas

**Fluxo:**
1. Owner faz login no sistema
2. Vê lista de todas as suas clínicas no menu
3. Seleciona clínica que deseja gerenciar
4. Sistema atualiza:
   - `TenantId` do contexto
   - `ClinicId` do contexto
   - Menu e permissões específicas da clínica
5. Owner passa a operar naquela clínica

**UX Recomendada:**
- Dropdown no header com lista de clínicas
- Ícone indicando status da assinatura de cada clínica
- Busca rápida por nome da clínica
- Última clínica acessada é salva como padrão

### 3.4 Transferência de Propriedade

**Fluxo:**
1. Primary owner acessa gestão da clínica
2. Seleciona "Transferir Propriedade"
3. Escolhe um co-owner existente
4. Confirma transferência com autenticação (senha ou 2FA)
5. Sistema atualiza:
   - Old primary owner: `IsPrimaryOwner = false`
   - New primary owner: `IsPrimaryOwner = true`
6. Ambos recebem notificação

**Regras:**
- Apenas primary owner pode transferir
- Transferência é irreversível (exceto se reverter novamente)
- Notificações por email para ambos

### 3.5 Owner Remove Vínculo com Clínica

**Fluxo:**
1. Owner ou primary owner acessa gestão
2. Seleciona "Sair da Clínica" ou "Remover Co-Owner"
3. Confirma ação
4. Sistema:
   - Desativa `OwnerClinicLink` (`IsActive = false`)
   - Registra data e razão
   - Mantém histórico para auditoria
5. Owner perde acesso à clínica

**Regras:**
- Primary owner só pode sair se designar novo primary owner
- Última clínica não pode ser removida (deve cancelar conta)
- Histórico permanece no banco de dados

## 4. Portal do Clinic Owner

### 4.1 Dashboard Multi-Clínica

**Tela Principal:**
```
╔═══════════════════════════════════════════════════╗
║  Omni Care Software - Minhas Clínicas                 ║
║  👤 João Silva                        [Sair]      ║
╠═══════════════════════════════════════════════════╣
║                                                    ║
║  📊 Visão Geral                                   ║
║  ┌─────────────────────────────────────────────┐ ║
║  │ 🏥 Clínica A - Centro              ✅ Ativa │ ║
║  │    CNPJ: 11.111.111/0001-11                 │ ║
║  │    Plano: Premium                           │ ║
║  │    Próximo pagamento: 15/12/2024            │ ║
║  │    [Acessar]  [Gerenciar]                   │ ║
║  └─────────────────────────────────────────────┘ ║
║                                                    ║
║  ┌─────────────────────────────────────────────┐ ║
║  │ 🏥 Clínica B - Norte               🆓 Trial │ ║
║  │    CNPJ: 22.222.222/0001-22                 │ ║
║  │    Plano: Basic (10 dias restantes)         │ ║
║  │    [Acessar]  [Gerenciar]  [Ativar Plano]  │ ║
║  └─────────────────────────────────────────────┘ ║
║                                                    ║
║  ┌─────────────────────────────────────────────┐ ║
║  │ 🏥 Clínica C - Sul                 ⚠️ Atraso│ ║
║  │    CNPJ: 33.333.333/0001-33                 │ ║
║  │    Plano: Standard                          │ ║
║  │    Pagamento vencido: 05/11/2024            │ ║
║  │    [Regularizar]  [Gerenciar]              │ ║
║  └─────────────────────────────────────────────┘ ║
║                                                    ║
║  [➕ Adicionar Nova Clínica]                      ║
║                                                    ║
║  💰 Resumo Financeiro (Todas as Clínicas)        ║
║  ├─ Total Mensal: R$ 750,00                      ║
║  ├─ Próximos Vencimentos: 2 clínicas            ║
║  └─ Em Atraso: 1 clínica                         ║
╚═══════════════════════════════════════════════════╝
```

### 4.2 Gestão Individual de Clínica

Ao acessar uma clínica específica:
```
╔═══════════════════════════════════════════════════╗
║  🏥 Clínica A - Centro            [Trocar Clínica]║
╠═══════════════════════════════════════════════════╣
║  [Dashboard] [Pacientes] [Agenda] [Usuários] ...  ║
║                                                    ║
║  ⚙️ Configurações da Clínica                      ║
║                                                    ║
║  📝 Informações Básicas                           ║
║  └─ [Editar Dados]                                ║
║                                                    ║
║  💳 Assinatura                                     ║
║  ├─ Plano Atual: Premium                          ║
║  ├─ Valor: R$ 320/mês                            ║
║  ├─ Status: ✅ Ativa                             ║
║  ├─ Próximo pagamento: 15/12/2024                ║
║  └─ [Trocar Plano] [Congelar] [Cancelar]        ║
║                                                    ║
║  👥 Proprietários                                 ║
║  ├─ João Silva (Você) - Proprietário Principal   ║
║  ├─ Maria Santos - Co-Proprietária (30%)         ║
║  └─ [Adicionar Co-Proprietário]                  ║
║                                                    ║
║  👤 Usuários (3/5 utilizados)                     ║
║  └─ [Gerenciar Usuários]                          ║
╚═══════════════════════════════════════════════════╝
```

### 4.3 Seletor de Clínica

Componente sempre visível no header:
```
┌────────────────────────────────┐
│ 🏥 Clínica A - Centro      ▼  │
└────────────────────────────────┘
        │
        ▼ (ao clicar)
┌────────────────────────────────┐
│ Suas Clínicas:                 │
├────────────────────────────────┤
│ ✓ 🏥 Clínica A - Centro   ✅  │
│   🏥 Clínica B - Norte    🆓  │
│   🏥 Clínica C - Sul      ⚠️  │
├────────────────────────────────┤
│ ➕ Adicionar Nova Clínica      │
│ ⚙️ Gerenciar Todas             │
└────────────────────────────────┘
```

## 5. APIs e Endpoints

### 5.1 Gestão de Vínculos Owner-Clínica

```http
GET /api/owner-clinic-links
    - Lista todas as clínicas de um owner
    - Filtros: ownerId, isActive
    - Retorna: Lista de OwnerClinicLink com dados da clínica

GET /api/owner-clinic-links/{ownerId}/clinics
    - Retorna todas as clínicas de um owner específico
    - Inclui status da assinatura

GET /api/owner-clinic-links/{clinicId}/owners
    - Lista todos os owners de uma clínica
    - Identifica primary owner

POST /api/owner-clinic-links
    - Cria vínculo entre owner e clínica existente
    - Body: { ownerId, clinicId, isPrimaryOwner, role, ownershipPercentage }

PUT /api/owner-clinic-links/{id}
    - Atualiza vínculo (role, percentage)

DELETE /api/owner-clinic-links/{id}
    - Desativa vínculo (soft delete)

POST /api/owner-clinic-links/{id}/transfer-primary
    - Transfere propriedade principal
    - Body: { newPrimaryOwnerId }

POST /api/owner-clinic-links/{clinicId}/invite-owner
    - Convida outro owner para co-propriedade
    - Body: { email, role, ownershipPercentage }
```

### 5.2 Registro de Nova Clínica por Owner Existente

```http
POST /api/clinics/add-for-owner
    - Cria nova clínica e vincula a owner existente
    - Body: {
        ownerId,
        clinicData: { name, cnpj, phone, email, address, ... },
        planId,
        useTrial
      }
    - Retorna: Clinic criada + OwnerClinicLink
```

### 5.3 Dashboard Multi-Clínica

```http
GET /api/owners/{ownerId}/dashboard
    - Retorna resumo de todas as clínicas do owner
    - Inclui:
      * Lista de clínicas com status
      * Resumo financeiro consolidado
      * Alertas e notificações
      * Métricas gerais

GET /api/owners/{ownerId}/switch-clinic/{clinicId}
    - Prepara contexto para trocar de clínica
    - Retorna token atualizado com novo TenantId/ClinicId
```

## 6. Segurança e Permissões

### 6.1 Verificação de Acesso

Antes de qualquer operação em uma clínica, o sistema deve verificar:

```csharp
public async Task<bool> HasAccessToClinic(Guid ownerId, Guid clinicId)
{
    return await _ownerClinicLinkRepository
        .HasAccessToClinicAsync(ownerId, clinicId);
}
```

### 6.2 Middleware de Validação

```csharp
// Middleware que valida acesso do owner à clínica
public class OwnerClinicAccessMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var ownerId = context.User.GetOwnerId();
        var clinicId = context.Request.GetClinicId();
        
        if (!await _service.HasAccessToClinic(ownerId, clinicId))
        {
            context.Response.StatusCode = 403;
            return;
        }
        
        await _next(context);
    }
}
```

### 6.3 Permissões Especiais

**Primary Owner:**
- Adicionar/remover co-owners
- Transferir propriedade
- Cancelar assinatura
- Encerrar clínica

**Co-Owner:**
- Acesso completo à gestão da clínica
- Não pode remover primary owner
- Não pode cancelar assinatura (apenas primary)

## 7. Fluxo de Autenticação

### 7.1 Login Multi-Clínica

```
1. Owner faz login: POST /api/auth/owner-login
   ↓
2. Sistema retorna token JWT com:
   - OwnerId
   - Lista de ClinicIds (todas as clínicas do owner)
   - Default ClinicId (última acessada ou primary)
   - TenantId (pode ser multi-tenant)
   ↓
3. Frontend armazena token
   ↓
4. Frontend carrega lista de clínicas
   ↓
5. Owner seleciona clínica
   ↓
6. Frontend atualiza contexto local
   ↓
7. Requests incluem ClinicId no header ou query param
```

### 7.2 JWT Claims Atualizados

```json
{
  "sub": "owner-guid",
  "username": "joao.silva",
  "role": "ClinicOwner",
  "owner_id": "owner-guid",
  "clinic_ids": ["clinic-a-guid", "clinic-b-guid", "clinic-c-guid"],
  "default_clinic_id": "clinic-a-guid",
  "tenant_id": "system",
  "exp": 1234567890
}
```

## 8. Casos Especiais

### 8.1 Clínica com Assinatura Vencida

Se uma clínica tem assinatura vencida:
- Owner ainda pode acessar dashboard da clínica
- Funcionalidades operacionais são bloqueadas
- Owner pode regularizar pagamento
- Outras clínicas do mesmo owner não são afetadas

### 8.2 Owner Remove Todas as Clínicas

Se owner tentar remover vínculo com todas as clínicas:
- Sistema impede remoção da última clínica
- Owner deve cancelar conta completamente
- Ou transferir propriedade antes de sair

### 8.3 Fusão de Clínicas

Caso especial onde duas clínicas precisam ser mescladas:
- Requer suporte administrativo (system owner)
- Dados são consolidados
- Assinatura é ajustada
- Histórico é preservado

## 9. Migração de Dados Existentes

Para owners já existentes com uma única clínica:

```sql
-- Script de migração
INSERT INTO OwnerClinicLinks (
    Id, OwnerId, ClinicId, LinkedDate, IsActive, 
    IsPrimaryOwner, TenantId, CreatedAt
)
SELECT 
    gen_random_uuid(),
    o.Id,
    o.ClinicId,
    o.CreatedAt,
    o.IsActive,
    true, -- IsPrimaryOwner
    o.TenantId,
    NOW()
FROM Owners o
WHERE o.ClinicId IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 FROM OwnerClinicLinks ocl 
      WHERE ocl.OwnerId = o.Id AND ocl.ClinicId = o.ClinicId
  );
```

## 10. Testes

### 10.1 Testes Unitários

```csharp
[Fact]
public void OwnerClinicLink_ShouldCreate_WhenValidData()
{
    // Arrange
    var ownerId = Guid.NewGuid();
    var clinicId = Guid.NewGuid();
    var tenantId = "test-tenant";

    // Act
    var link = new OwnerClinicLink(ownerId, clinicId, tenantId);

    // Assert
    Assert.Equal(ownerId, link.OwnerId);
    Assert.Equal(clinicId, link.ClinicId);
    Assert.True(link.IsActive);
    Assert.True(link.IsPrimaryOwner);
}

[Fact]
public void OwnerClinicLink_ShouldThrow_WhenOwnershipPercentageInvalid()
{
    // Arrange
    var ownerId = Guid.NewGuid();
    var clinicId = Guid.NewGuid();
    var tenantId = "test-tenant";

    // Act & Assert
    Assert.Throws<ArgumentException>(() => 
        new OwnerClinicLink(ownerId, clinicId, tenantId, 
            ownershipPercentage: 150)); // > 100
}
```

### 10.2 Testes de Integração

```csharp
[Fact]
public async Task Owner_ShouldAccessMultipleClinics()
{
    // Arrange
    var owner = await CreateOwner();
    var clinic1 = await CreateClinic();
    var clinic2 = await CreateClinic();
    
    await LinkOwnerToClinic(owner.Id, clinic1.Id);
    await LinkOwnerToClinic(owner.Id, clinic2.Id);

    // Act
    var clinics = await _repository.GetClinicsByOwnerIdAsync(owner.Id);

    // Assert
    Assert.Equal(2, clinics.Count());
    Assert.Contains(clinics, c => c.ClinicId == clinic1.Id);
    Assert.Contains(clinics, c => c.ClinicId == clinic2.Id);
}
```

## 11. Métricas e Analytics

### 11.1 Métricas para System Owner

- Quantos owners têm múltiplas clínicas
- Média de clínicas por owner
- Total de receita por owner multi-clínica
- Taxa de conversão de trial para pago em clínicas secundárias

### 11.2 Métricas para Clinic Owner

- Receita total de todas as clínicas
- Total de pacientes atendidos (agregado)
- Total de consultas realizadas (agregado)
- Comparativo entre clínicas

## 12. Roadmap de Implementação

### Fase 1: Backend (2 semanas)
- [ ] Criar entidade OwnerClinicLink
- [ ] Criar repository e service
- [ ] Implementar endpoints API
- [ ] Testes unitários
- [ ] Migration de dados existentes
- [ ] Atualizar autenticação/JWT

### Fase 2: Frontend - Portal (2 semanas)
- [ ] Dashboard multi-clínica
- [ ] Seletor de clínicas no header
- [ ] Tela de adicionar nova clínica
- [ ] Gestão de co-proprietários
- [ ] Alertas de assinatura por clínica

### Fase 3: Integração e Testes (1 semana)
- [ ] Testes de integração
- [ ] Testes E2E
- [ ] Validação de segurança
- [ ] Testes de performance
- [ ] Documentação de usuário

### Fase 4: Deploy e Monitoramento (1 semana)
- [ ] Deploy em staging
- [ ] Testes beta com usuários reais
- [ ] Ajustes de UX
- [ ] Deploy em produção
- [ ] Monitoramento de uso

**Total estimado:** 6 semanas

## 13. Considerações Finais

### 13.1 Benefícios

✅ **Para Owners:**
- Gerenciamento centralizado de múltiplas unidades
- Economia de tempo na gestão
- Visibilidade consolidada do negócio
- Flexibilidade para expandir

✅ **Para o Negócio (Omni Care Software):**
- Aumento de receita por cliente
- Maior retenção de clientes
- Diferencial competitivo
- Modelo escalável

### 13.2 Riscos e Mitigações

⚠️ **Complexidade adicional:**
- **Mitigação:** UX intuitiva, documentação clara

⚠️ **Performance com muitas clínicas:**
- **Mitigação:** Paginação, caching, queries otimizadas

⚠️ **Segurança de acesso:**
- **Mitigação:** Validação rigorosa, middleware, auditoria

### 13.3 Próximos Passos

1. Revisar documento com stakeholders
2. Aprovar roadmap de implementação
3. Iniciar Fase 1 (Backend)
4. Preparar mockups de UI/UX
5. Planejar comunicação com clientes existentes

---

**Última Atualização**: 2024-11-19  
**Versão**: 1.0  
**Autor**: Omni Care Software Team
