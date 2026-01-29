# 📚 PROMPT: Documentação - Sistema de Configuração de Módulos

> **Fase:** 5 de 5  
> **Duração Estimada:** 1 semana  
> **Desenvolvedores:** 1  
> **Prioridade:** 🔥 MÉDIA  
> **Dependências:** 01, 02, 03, 04-PROMPT (concluídos)

---

## 📋 Contexto

Esta fase final cobre a criação de **documentação completa** para o sistema de configuração de módulos.

**Tipos de Documentação:**
1. **Documentação Técnica** (API, Arquitetura)
2. **Guia do Usuário** (System Admin)
3. **Guia do Usuário** (Clínica)
4. **Documentação de Desenvolvimento**
5. **Material de Treinamento** (vídeos, tutoriais)

---

## 🎯 Objetivos da Tarefa

### Objetivos Principais

1. Documentar API REST completa
2. Criar guias de usuário ilustrados
3. Documentar arquitetura e decisões técnicas
4. Criar material de treinamento
5. Preparar release notes

---

## 📝 Tarefas Detalhadas

### 1. Documentação Técnica da API (2 dias)

#### 1.1. OpenAPI/Swagger

**Atualizar:** `/src/MedicSoft.Api/Program.cs` - Configuração do Swagger

```csharp
// Adicionar documentação detalhada ao Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PrimeCare - Module Configuration API",
        Version = "v1",
        Description = "API para gerenciamento de módulos do sistema",
        Contact = new OpenApiContact
        {
            Name = "PrimeCare Software",
            Email = "dev@primecare.com.br"
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Configurar autenticação
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
});
```

**Adicionar comentários XML aos controllers:**

```csharp
/// <summary>
/// Gerenciamento de configuração de módulos por clínica
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ModuleConfigController : BaseController
{
    /// <summary>
    /// Obtém todos os módulos disponíveis para a clínica
    /// </summary>
    /// <returns>Lista de módulos com status de habilitação</returns>
    /// <response code="200">Módulos retornados com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Assinatura da clínica não encontrada</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ModuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModules()
    {
        // ...
    }
    
    // Documentar todos os outros endpoints...
}
```

#### 1.2. Documento de Arquitetura

**Criar:** `/Plano_Desenvolvimento/PlanoModulos/ARQUITETURA_MODULOS.md`

```markdown
# 🏗️ Arquitetura do Sistema de Módulos

## Visão Geral

O sistema de módulos permite habilitar/desabilitar funcionalidades do PrimeCare 
de forma flexível, vinculado aos planos de assinatura.

## Componentes

### 1. Domain Layer
- `ModuleConfiguration`: Entidade que armazena configuração de módulos por clínica
- `ModuleConfigurationHistory`: Histórico de mudanças
- `SystemModules`: Definição estática dos módulos disponíveis
- `SubscriptionPlan`: Planos vinculados a módulos

### 2. Application Layer
- `ModuleConfigurationService`: Lógica de negócio
- `ModuleAnalyticsService`: Métricas e analytics

### 3. API Layer
- `ModuleConfigController`: Endpoints para clínicas
- `SystemAdminModuleController`: Endpoints para system admin

### 4. Frontend
- **System Admin**: Dashboard e configuração global
- **Clínica**: Interface de gestão de módulos

## Fluxo de Dados

```
┌─────────────┐      ┌──────────────┐      ┌─────────────┐
│  Frontend   │─────▶│     API      │─────▶│  Database   │
│  (Angular)  │◀─────│  (ASP.NET)   │◀─────│ (PostgreSQL)│
└─────────────┘      └──────────────┘      └─────────────┘
       │                     │
       │                     ▼
       │             ┌──────────────┐
       │             │   Services   │
       │             │  (Business   │
       └────────────▶│   Logic)     │
                     └──────────────┘
```

## Decisões de Design

### Por que módulos são vinculados a planos?
Permite monetização diferenciada e controle fino de funcionalidades.

### Por que usar JSON para configuração?
Flexibilidade para adicionar novos parâmetros sem alterar schema.

### Por que manter histórico?
Auditoria e rastreabilidade de mudanças críticas.

## Segurança

- Autenticação JWT obrigatória
- Validação de permissões em controller e service
- Auditoria de todas as mudanças
- Validação de plano antes de habilitar módulo

## Performance

- Cache de configurações de módulos (15 min)
- Lazy loading de componentes frontend
- Paginação em listas grandes
- Índices no banco de dados

## Escalabilidade

- Design permite adicionar novos módulos sem quebrar código existente
- Configuração JSON extensível
- API versionada para mudanças futuras
```

---

### 2. Guia do Usuário - System Admin (2 dias)

**Criar:** `/Plano_Desenvolvimento/PlanoModulos/GUIA_USUARIO_SYSTEM_ADMIN.md`

```markdown
# 👨‍💼 Guia do Usuário - System Admin

## Bem-vindo ao Sistema de Módulos

Este guia ensina como gerenciar módulos do PrimeCare como administrador do sistema.

## 📊 Dashboard de Módulos

### Acessar o Dashboard

1. Faça login como **System Admin**
2. No menu lateral, clique em **"Módulos"**
3. Você verá o dashboard com métricas de uso

![Dashboard de Módulos](./screenshots/modules-dashboard.png)

### Entendendo as Métricas

**KPIs Principais:**
- **Total de Módulos**: Quantidade de módulos disponíveis no sistema
- **Taxa Média de Adoção**: Percentual médio de clínicas usando cada módulo
- **Mais Usado**: Módulo com maior taxa de adoção
- **Menos Usado**: Módulo com menor taxa de adoção

**Tabela de Uso:**
- Mostra todos os módulos com:
  - Nome e categoria
  - Número de clínicas usando
  - Taxa de adoção (%)
  - Ações disponíveis

### Categorias de Módulos

🌟 **Core**: Módulos essenciais (não podem ser desabilitados)
🔧 **Advanced**: Funcionalidades avançadas
💎 **Premium**: Recursos premium
📊 **Analytics**: Relatórios e análises

---

## 📋 Configurar Módulos por Plano

### Acesso

1. No menu, clique em **"Módulos por Plano"**
2. Selecione um plano no dropdown

### Habilitar/Desabilitar Módulos

1. **Selecione o plano** que deseja configurar
2. Marque/desmarque os checkboxes dos módulos
3. Clique em **"Salvar Configurações"**

![Configuração de Planos](./screenshots/plan-modules.png)

⚠️ **Importante:**
- Módulos **CORE** não podem ser desabilitados
- Módulos com dependências devem ter seus pré-requisitos habilitados

### Tipos de Planos

| Plano | Módulos Típicos |
|-------|----------------|
| **Basic** | Core + básicos |
| **Standard** | Basic + Reports + TISS |
| **Premium** | Standard + WhatsApp + SMS |
| **Enterprise** | Todos os módulos |

---

## 🔍 Detalhes do Módulo

### Visualizar Detalhes

1. No dashboard, clique no ícone 👁️ de um módulo
2. Você verá:
   - Informações completas
   - Lista de clínicas usando
   - Gráficos de adoção
   - Histórico de mudanças

### Ações Globais

**Habilitar Globalmente:**
- Habilita o módulo para todas as clínicas com plano adequado

**Desabilitar Globalmente:**
- Desabilita o módulo para todas as clínicas
- ⚠️ Use com cautela!

---

## 📈 Relatórios e Analytics

### Adoção por Categoria
Veja quais categorias são mais utilizadas.

### Uso por Plano
Compare o uso de módulos entre diferentes planos.

### Tendências
Identifique módulos em crescimento ou declínio.

---

## 💡 Melhores Práticas

✅ **Revise a adoção mensalmente**
✅ **Promova módulos sub-utilizados**
✅ **Configure planos progressivos**
✅ **Monitore feedback das clínicas**
❌ **Evite desabilitar módulos em uso**
❌ **Não remova módulos core dos planos**

---

## 🆘 Problemas Comuns

### "Módulo não pode ser habilitado"
- Verifique se está disponível no plano da clínica
- Confirme que dependências estão satisfeitas

### "Taxa de adoção baixa"
- Analise se o módulo está bem posicionado
- Considere treinamento ou comunicação

### "Clínicas reclamando de limite"
- Avalie fazer upgrade de plano
- Ou ajuste limites específicos

---

## 📞 Suporte

Dúvidas? Entre em contato:
- Email: suporte@primecare.com.br
- Tel: (11) 1234-5678
- Chat: [Sistema de Tickets]
```

---

### 3. Guia do Usuário - Clínica (2 dias)

**Criar:** `/Plano_Desenvolvimento/PlanoModulos/GUIA_USUARIO_CLINICA.md`

```markdown
# 🏥 Guia do Usuário - Clínica

## Bem-vindo à Configuração de Módulos

Aprenda a gerenciar os módulos disponíveis para sua clínica.

## 🎯 O que são Módulos?

Módulos são **funcionalidades** do sistema que você pode habilitar ou desabilitar 
conforme necessidade da sua clínica.

**Benefícios:**
- ✨ Personalize o sistema
- 🎯 Foco nas funcionalidades que você usa
- 💰 Otimize custos (planos específicos)

---

## 📱 Acessar Módulos

1. Faça login na área administrativa
2. No menu, clique em **"Configurações"** ou **"Módulos"**
3. Você verá todos os módulos disponíveis

![Tela de Módulos](./screenshots/clinic-modules.png)

---

## ⚙️ Habilitar/Desabilitar Módulos

### Habilitar um Módulo

1. Localize o módulo que deseja habilitar
2. Clique no **toggle** (chave) do módulo
3. Aguarde a confirmação
4. Pronto! O módulo está ativo ✅

### Desabilitar um Módulo

1. Localize o módulo habilitado
2. Clique no **toggle** para desligar
3. Confirme a ação
4. O módulo será desabilitado 🚫

⚠️ **Atenção:**
- Módulos essenciais não podem ser desabilitados
- Alguns módulos dependem de outros

---

## 🎨 Categorias de Módulos

### 🌟 Essenciais (Core)
Módulos básicos que não podem ser desabilitados:
- Gestão de Pacientes
- Agendamento
- Prontuários
- Prescrições

### 🔧 Avançados
Funcionalidades extras:
- Gestão Financeira
- Fila de Espera
- Gestão de Estoque

### 💎 Premium
Recursos premium (necessário plano adequado):
- Relatórios Avançados
- Integração WhatsApp
- Notificações SMS
- Exportação TISS

### 📊 Analytics
Análises e relatórios:
- Dashboards
- Relatórios customizados

---

## 🔧 Configurações Avançadas

Alguns módulos permitem configurações detalhadas.

### Acessar Configurações

1. Clique em **"Configurar"** no módulo desejado
2. Uma janela abrirá com opções
3. Ajuste conforme necessário
4. Clique em **"Salvar"**

![Configurações Avançadas](./screenshots/module-config-dialog.png)

**Exemplo de Configurações:**
```json
{
  "enviarNotificacoes": true,
  "intervaloMinutos": 30,
  "templateMensagem": "Lembrete de consulta..."
}
```

---

## 🚀 Fazer Upgrade de Plano

Viu um módulo com **"UPGRADE NECESSÁRIO"**?

Significa que esse módulo está disponível em planos superiores.

### Como fazer upgrade:

1. Clique em **"Fazer Upgrade"** no módulo
2. Você será direcionado para a página de planos
3. Compare os planos disponíveis
4. Escolha o plano ideal
5. Contate o suporte para ativar

**Planos Disponíveis:**
- **Basic**: R$ 99/mês - Funcionalidades básicas
- **Standard**: R$ 199/mês - + Relatórios + TISS
- **Premium**: R$ 299/mês - + WhatsApp + SMS
- **Enterprise**: Sob consulta - Todos os recursos

---

## ⚠️ Restrições e Dependências

### Módulos que dependem de outros

Alguns módulos precisam de outros habilitados:

- **Fila de Espera** → requer **Agendamento**
- **Notificações SMS** → requer **Gestão de Pacientes**
- **Relatórios** → requer **Prontuários**

Se tentar habilitar um módulo sem a dependência, verá uma mensagem de erro.

### Limites do Plano

Cada plano tem limites:
- Número de usuários
- Número de pacientes
- Módulos disponíveis

Veja seu plano atual na aba **"Assinatura"**.

---

## 💡 Dicas e Melhores Práticas

✅ **Habilite apenas o que você usa**
   - Mantém a interface limpa
   - Facilita o treinamento da equipe

✅ **Teste novos módulos gradualmente**
   - Habilite um de cada vez
   - Treine a equipe antes de usar

✅ **Revise módulos periodicamente**
   - Desabilite o que não usa mais
   - Explore novos módulos disponíveis

✅ **Mantenha backups das configurações**
   - Anote configurações importantes
   - Facilita restauração se necessário

---

## 🆘 Problemas Comuns

### "Não consigo habilitar um módulo"

**Possíveis causas:**
1. Módulo não disponível no seu plano → Fazer upgrade
2. Falta dependência → Habilitar módulos requeridos
3. Limite de módulos atingido → Revisar plano

### "Módulo habilitado não aparece no menu"

**Soluções:**
1. Faça logout e login novamente
2. Limpe o cache do navegador
3. Aguarde alguns minutos (sincronização)

### "Configurações não salvam"

**Verifique:**
1. Formato JSON está correto
2. Tem permissão de administrador
3. Conexão com internet está ok

---

## 📞 Precisa de Ajuda?

**Suporte Técnico:**
- 📧 Email: suporte@primecare.com.br
- 📱 WhatsApp: (11) 98765-4321
- 💬 Chat: [Abrir Ticket]
- 📚 Base de Conhecimento: [Central de Ajuda]

**Horário de Atendimento:**
- Segunda a Sexta: 8h às 18h
- Sábado: 8h às 12h
- Emergências: 24/7

---

## 📺 Vídeo Tutoriais

🎥 [Como Habilitar Módulos](https://youtube.com/...)
🎥 [Configurações Avançadas](https://youtube.com/...)
🎥 [Upgrade de Plano](https://youtube.com/...)

---

*Última atualização: 29 de Janeiro de 2026*
```

---

### 4. Release Notes (1 dia)

**Criar:** `/Plano_Desenvolvimento/PlanoModulos/RELEASE_NOTES.md`

```markdown
# 🚀 Release Notes - Sistema de Módulos v1.0

## Data de Lançamento: [DATA]

---

## ✨ Novidades

### Para System Admin

#### 📊 Dashboard de Módulos
- Novo dashboard com métricas de uso
- Visualização de taxa de adoção por módulo
- Gráficos interativos de analytics

#### 📋 Configuração de Planos
- Interface para vincular módulos a planos
- Gestão visual de features por plano
- Validações automáticas de dependências

#### 🔍 Detalhes e Analytics
- Página de detalhes de cada módulo
- Lista de clínicas usando cada módulo
- Histórico completo de mudanças

### Para Clínicas

#### ⚙️ Gestão de Módulos
- Interface visual para habilitar/desabilitar módulos
- Toggle simples e intuitivo
- Feedback visual claro de status

#### 🔧 Configurações Avançadas
- Dialog de configurações por módulo
- Suporte a JSON para ajustes finos
- Histórico de configurações

#### 📱 Interface Responsiva
- Funciona em desktop, tablet e mobile
- Design moderno e intuitivo
- Acessibilidade WCAG 2.1

---

## 🔧 Melhorias Técnicas

### Backend
- Nova entidade `ModuleConfiguration`
- Nova entidade `ModuleConfigurationHistory`
- Serviço `ModuleConfigurationService`
- Serviço `ModuleAnalyticsService`
- 15 novos endpoints REST

### Frontend
- 6 novos componentes Angular standalone
- 2 novos services
- Integração com Angular Material
- Testes E2E com Cypress

### Segurança
- Validação de permissões em todos endpoints
- Auditoria de todas as mudanças
- Logs detalhados de ações

---

## 📚 Documentação

- ✅ Guia do Usuário - System Admin
- ✅ Guia do Usuário - Clínica
- ✅ Documentação da API (Swagger)
- ✅ Arquitetura do Sistema
- ✅ Guia de Desenvolvimento

---

## 🐛 Correções

Nenhuma (primeira versão)

---

## ⚠️ Breaking Changes

Nenhum

---

## 🔄 Migração

Não é necessária migração de dados.
Sistema é retrocompatível.

---

## 📦 Instalação

### Backend
```bash
cd src/MedicSoft.Repository
dotnet ef database update
```

### Frontend System Admin
```bash
cd frontend/mw-system-admin
npm install
ng build
```

### Frontend Clínica
```bash
cd frontend/medicwarehouse-app
npm install
ng build
```

---

## 🎯 Próximos Passos

- [ ] Coletar feedback dos usuários
- [ ] Ajustar baseado em uso real
- [ ] Adicionar mais módulos ao sistema
- [ ] Implementar analytics avançados

---

## 👥 Créditos

Desenvolvido por PrimeCare Software Development Team

---

*Para mais informações, consulte a documentação completa em `/Plano_Desenvolvimento/PlanoModulos/`*
```

---

### 5. Vídeos Tutoriais (1-2 dias)

#### 5.1. Scripts para Gravação

**Criar:** `/Plano_Desenvolvimento/PlanoModulos/VIDEO_SCRIPTS.md`

```markdown
# 🎬 Scripts para Vídeos Tutoriais

## Vídeo 1: Introdução ao Sistema de Módulos (3 min)

**Objetivo**: Apresentar o conceito e benefícios

**Roteiro:**
1. Abertura (10s)
   - "Bem-vindo ao PrimeCare!"
   - "Hoje vamos conhecer o Sistema de Módulos"

2. O que são módulos? (30s)
   - Explicar conceito
   - Mostrar exemplos
   - Destacar flexibilidade

3. Benefícios (30s)
   - Personalização
   - Otimização
   - Escalabilidade

4. Onde acessar (30s)
   - Demonstrar acesso
   - Mostrar menu
   - Preview da tela

5. Próximos passos (20s)
   - Convite para próximos vídeos
   - Link para documentação

## Vídeo 2: Habilitar/Desabilitar Módulos (Clínica) (5 min)

[Roteiro detalhado...]

## Vídeo 3: Configuração Avançada (Clínica) (4 min)

[Roteiro detalhado...]

## Vídeo 4: Dashboard e Analytics (System Admin) (6 min)

[Roteiro detalhado...]

## Vídeo 5: Configurar Módulos por Plano (System Admin) (7 min)

[Roteiro detalhado...]
```

---

## ✅ Critérios de Sucesso

### Documentação
- ✅ API documentada no Swagger
- ✅ Guias de usuário completos
- ✅ Arquitetura documentada
- ✅ Release notes criadas

### Material de Treinamento
- ✅ Scripts de vídeo prontos
- ✅ Screenshots capturadas
- ✅ FAQs criadas

### Qualidade
- ✅ Linguagem clara e acessível
- ✅ Exemplos práticos
- ✅ Imagens ilustrativas
- ✅ Fácil navegação

---

## 📊 Checklist de Entrega

### Documentação Técnica
- [ ] Swagger configurado e documentado
- [ ] Documento de arquitetura
- [ ] Diagramas de fluxo
- [ ] Decisões de design documentadas

### Guias de Usuário
- [ ] Guia System Admin completo
- [ ] Guia Clínica completo
- [ ] Screenshots atualizados
- [ ] FAQs incluídas

### Release Notes
- [ ] Novidades listadas
- [ ] Breaking changes documentados
- [ ] Instruções de instalação
- [ ] Créditos e contatos

### Material de Treinamento
- [ ] Scripts de vídeo prontos
- [ ] Lista de screenshots necessários
- [ ] Plano de gravação
- [ ] Checklist de publicação

---

## 🎯 Localização dos Documentos

```
/Plano_Desenvolvimento/PlanoModulos/
├── README.md (índice principal)
├── ARQUITETURA_MODULOS.md
├── GUIA_USUARIO_SYSTEM_ADMIN.md
├── GUIA_USUARIO_CLINICA.md
├── RELEASE_NOTES.md
├── VIDEO_SCRIPTS.md
└── screenshots/
    ├── modules-dashboard.png
    ├── plan-modules.png
    ├── clinic-modules.png
    └── module-config-dialog.png
```

---

## ⏭️ Finalização

Após completar este prompt:
1. Revisar toda a documentação
2. Validar links e referências
3. Publicar na wiki/portal
4. Comunicar equipe e usuários
5. **PROJETO CONCLUÍDO** 🎉

---

> **Status:** 📝 Pronto para desenvolvimento  
> **Última Atualização:** 29 de Janeiro de 2026
