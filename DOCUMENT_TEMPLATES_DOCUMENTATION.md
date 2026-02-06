# 📄 Sistema de Templates de Impressão - Documentação Técnica

## 📋 Visão Geral

Sistema completo para gerenciamento de templates de impressão de documentos médicos, incluindo prontuários, atestados, receitas médicas, e outros documentos clínicos. O sistema permite criar templates personalizados com variáveis dinâmicas que podem ser substituídas durante a geração do documento.

## 🎯 Funcionalidades Principais

### 1. Gerenciamento de Templates
- ✅ Criar novos templates
- ✅ Editar templates existentes
- ✅ Visualizar templates (modo somente leitura)
- ✅ Excluir templates (exceto templates do sistema)
- ✅ Ativar/Desativar templates
- ✅ Filtrar por especialidade, tipo, status

### 2. Sistema de Variáveis
- ✅ Variáveis pré-definidas comuns
- ✅ Criar variáveis personalizadas
- ✅ Tipos de variáveis: texto, data, número, booleano
- ✅ Marcar variáveis como obrigatórias
- ✅ Definir valores padrão
- ✅ Inserção rápida de variáveis no conteúdo
- ✅ Formato: `{{nomeVariavel}}`

### 3. Tipos de Documentos Suportados
1. **Prontuário Médico** - Registro completo da consulta
2. **Receita** - Prescrição de medicamentos
3. **Atestado Médico** - Declaração de condição de saúde
4. **Pedido de Exames** - Solicitação de exames laboratoriais
5. **Relatório Psicológico** - Avaliação psicológica
6. **Plano Alimentar** - Orientações nutricionais
7. **Orçamento Odontológico** - Estimativa de tratamento dental
8. **Odontograma** - Diagrama dental
9. **Avaliação Fisioterapêutica** - Análise fisioterápica
10. **Plano de Tratamento** - Plano terapêutico
11. **Evolução de Sessão** - Registro de progresso
12. **Relatório de Alta** - Documento de conclusão de tratamento
13. **Encaminhamento** - Referência a outro profissional
14. **Termo de Consentimento** - Autorização do paciente
15. **Modelo Personalizado** - Template customizado

### 4. Especialidades Profissionais
1. Médico
2. Psicólogo
3. Nutricionista
4. Fisioterapeuta
5. Dentista
6. Enfermeiro
7. Terapeuta Ocupacional
8. Fonoaudiólogo

## 🏗️ Arquitetura

### Backend (C# .NET 8)

```
src/
├── MedicSoft.Domain/
│   ├── Entities/
│   │   └── DocumentTemplate.cs              # Entidade principal
│   ├── Enums/
│   │   ├── DocumentTemplateType.cs          # Tipos de documento
│   │   └── ProfessionalSpecialty.cs         # Especialidades
│   └── Interfaces/
│       └── IDocumentTemplateRepository.cs   # Contrato do repositório
├── MedicSoft.Repository/
│   └── Repositories/
│       └── DocumentTemplateRepository.cs    # Implementação do repositório
├── MedicSoft.Application/
│   ├── DTOs/DocumentTemplates/
│   │   └── DocumentTemplateDtos.cs          # DTOs de transferência
│   ├── Commands/DocumentTemplates/
│   │   └── DocumentTemplateCommands.cs      # Comandos CQRS
│   ├── Queries/DocumentTemplates/
│   │   └── DocumentTemplateQueries.cs       # Queries CQRS
│   └── Handlers/
│       ├── Commands/DocumentTemplates/      # Handlers de comandos
│       └── Queries/DocumentTemplates/       # Handlers de queries
└── MedicSoft.Api/
    └── Controllers/
        └── DocumentTemplatesController.cs   # API Controller
```

### Frontend (Angular 20)

```
src/app/
├── models/
│   └── document-template.model.ts           # Interfaces TypeScript
├── services/
│   └── document-template.service.ts         # Serviço HTTP
└── pages/clinic-admin/document-templates/
    ├── document-templates.component.*       # Componente de lista
    └── document-template-editor.component.* # Componente de edição
```

## 🔌 API Endpoints

### Base URL: `/api/document-templates`

#### 1. Listar Templates
```http
GET /api/document-templates
Query Parameters:
  - specialty: ProfessionalSpecialty (opcional)
  - type: DocumentTemplateType (opcional)
  - isActive: boolean (opcional)
  - isSystem: boolean (opcional)
  - clinicId: Guid (opcional)

Response: 200 OK
[
  {
    "id": "guid",
    "name": "string",
    "description": "string",
    "specialty": 1,
    "type": 1,
    "content": "string",
    "variables": "json string",
    "isActive": true,
    "isSystem": false,
    "clinicId": "guid",
    "tenantId": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
]
```

#### 2. Obter Template por ID
```http
GET /api/document-templates/{id}

Response: 200 OK | 404 Not Found
```

#### 3. Criar Template
```http
POST /api/document-templates
Headers:
  - Authorization: Bearer {token}
  - Content-Type: application/json

Body:
{
  "name": "string",
  "description": "string",
  "specialty": 1,
  "type": 1,
  "content": "string",
  "variables": "[{...}]",
  "clinicId": "guid"
}

Response: 201 Created
```

#### 4. Atualizar Template
```http
PUT /api/document-templates/{id}

Body:
{
  "name": "string",
  "description": "string",
  "content": "string",
  "variables": "[{...}]"
}

Response: 200 OK | 404 Not Found
```

#### 5. Excluir Template
```http
DELETE /api/document-templates/{id}

Response: 204 No Content | 404 Not Found | 400 Bad Request
```

#### 6. Ativar Template
```http
POST /api/document-templates/{id}/activate

Response: 204 No Content | 404 Not Found
```

#### 7. Desativar Template
```http
POST /api/document-templates/{id}/deactivate

Response: 204 No Content | 404 Not Found
```

#### 8-10. Filtros Adicionais
```http
GET /api/document-templates/by-specialty/{specialty}?activeOnly=true
GET /api/document-templates/by-type/{type}
GET /api/document-templates/by-clinic/{clinicId}
```

## 🔐 Segurança e Permissões

### Permissões Necessárias
- **Visualizar**: `form-configuration.view`
- **Gerenciar**: `form-configuration.manage`

### Isolamento Multi-tenant
- Todos os dados são isolados por `TenantId`
- Usuários só podem acessar templates do seu tenant
- Templates do sistema são compartilhados entre todos os tenants

### Proteção de Templates do Sistema
- Templates marcados como `IsSystem = true` não podem ser:
  - Excluídos
  - Desativados (alguns casos)
  - Modificados (dependendo da lógica de negócio)

## 📝 Formato de Variáveis

### Estrutura JSON
```json
[
  {
    "key": "patientName",
    "label": "Nome do Paciente",
    "type": "text",
    "description": "Nome completo do paciente",
    "defaultValue": "",
    "isRequired": true,
    "displayOrder": 1
  }
]
```

### Tipos de Variáveis
- `text`: Texto livre
- `date`: Data (formato: DD/MM/YYYY)
- `number`: Número
- `boolean`: Verdadeiro/Falso

### Uso no Template
```
RECEITA MÉDICA

Paciente: {{patientName}}
CPF: {{patientCpf}}
Data: {{consultationDate}}

Prescrição:
[Conteúdo da prescrição]

___________________________
{{professionalName}}
{{professionalRegistration}}
```

## 🚀 Como Usar

### 1. Acessar o Sistema
```
/clinic-admin/document-templates
```

### 2. Criar um Novo Template

1. Clique em "➕ Novo Template"
2. Preencha os dados básicos:
   - Nome do template
   - Especialidade
   - Tipo de documento
   - Descrição (opcional)
3. Configure as variáveis:
   - Use variáveis pré-definidas
   - Ou crie variáveis personalizadas
4. Escreva o conteúdo do template:
   - Use o formato `{{variavel}}` para inserir variáveis
   - Clique nas variáveis da barra lateral para inserir
5. Salve o template

### 3. Editar um Template Existente

1. Na lista, clique no ícone de editar (✏️)
2. Modifique os campos desejados
3. Salve as alterações

### 4. Gerenciar Status

- **Ativar/Desativar**: Clique no ícone de status (✓/○)
- **Excluir**: Clique no ícone de lixeira (🗑️)
  - Não disponível para templates do sistema

### 5. Filtrar Templates

Use os filtros disponíveis:
- Busca por texto (nome/descrição)
- Especialidade profissional
- Tipo de documento
- Status (ativo/inativo)

## 🧪 Testes

### Teste Manual via API

1. **Criar Template**:
```bash
curl -X POST https://api.example.com/api/document-templates \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Receita Padrão",
    "description": "Receita médica padrão",
    "specialty": 1,
    "type": 2,
    "content": "RECEITA\n\nPaciente: {{patientName}}\nData: {{consultationDate}}",
    "variables": "[{\"key\":\"patientName\",\"label\":\"Nome do Paciente\",\"type\":\"text\",\"isRequired\":true,\"displayOrder\":1}]"
  }'
```

2. **Listar Templates**:
```bash
curl -X GET https://api.example.com/api/document-templates \
  -H "Authorization: Bearer {token}"
```

3. **Atualizar Template**:
```bash
curl -X PUT https://api.example.com/api/document-templates/{id} \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Receita Padrão Atualizada",
    "description": "Descrição atualizada",
    "content": "Novo conteúdo",
    "variables": "[...]"
  }'
```

## 🐛 Troubleshooting

### Problema: Template não aparece na lista
**Solução**: Verifique se o template está ativo e se você tem permissão `form-configuration.view`

### Problema: Não consigo excluir um template
**Solução**: Templates do sistema não podem ser excluídos. Verifique se `isSystem = false` e se você tem permissão `form-configuration.manage`

### Problema: Variáveis não são substituídas
**Solução**: Verifique se o formato está correto: `{{nomeVariavel}}` (sem espaços)

### Problema: Erro 403 Forbidden
**Solução**: Verifique suas permissões de acesso (`form-configuration.view` ou `form-configuration.manage`)

### Problema: Erro 404 Not Found
**Solução**: Verifique se o ID do template está correto e se pertence ao seu tenant

## 📊 Métricas e Monitoramento

### Endpoints para Monitorar
- Taxa de sucesso de criação de templates
- Tempo de resposta das queries
- Uso de memória na serialização de variáveis
- Erros de validação frequentes

### Logs Importantes
- Falhas de autenticação/autorização
- Tentativas de exclusão de templates do sistema
- Erros de deserialização de JSON de variáveis
- Violações de multi-tenancy

## 🔄 Próximas Melhorias

### Versão 1.1 (Sugerida)
- [ ] Editor WYSIWYG (rich text) para conteúdo
- [ ] Pré-visualização em tempo real
- [ ] Versionamento de templates
- [ ] Histórico de alterações
- [ ] Duplicar templates
- [ ] Importar/Exportar templates
- [ ] Templates compartilhados entre clínicas

### Versão 2.0 (Futuro)
- [ ] Geração de PDF dos templates
- [ ] Assinatura digital integrada
- [ ] Templates em múltiplos idiomas
- [ ] Editor de layout avançado
- [ ] Biblioteca de templates comunitários
- [ ] IA para sugestão de conteúdo

## 📚 Referências

- [Documentação da API](./API_DOCUMENTATION.md)
- [Guia de Permissões](./PERMISSIONS_REFERENCE.md)
- [Arquitetura CQRS](./CQRS_PATTERN.md)
- [Guia de Desenvolvimento](./DEVELOPMENT_GUIDE.md)

## 👥 Suporte

Para dúvidas ou problemas, entre em contato com:
- Email: suporte@primecare.com.br
- Slack: #dev-medicwarehouse
- Issues: GitHub Issues

---

**Versão**: 1.0.0  
**Data**: Fevereiro 2026  
**Autor**: Sistema MedicWarehouse  
**Status**: ✅ Produção
