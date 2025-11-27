# Conformidade com LGPD - MedicWarehouse

## Visão Geral

Este documento descreve as implementações realizadas no frontend do MedicWarehouse para garantir conformidade com a Lei Geral de Proteção de Dados (LGPD - Lei nº 13.709/2018).

## Recursos Implementados

### 1. Serviço de Persistência de Formulários (`FormPersistenceService`)

Um serviço dedicado foi criado para gerenciar o armazenamento temporário de dados de formulário no navegador do usuário, com total conformidade à LGPD.

**Localização:** `/frontend/mw-site/src/app/services/form-persistence.ts`

#### Características Principais:

1. **Consentimento Explícito**
   - O usuário deve conceder consentimento explícito antes que qualquer dado seja armazenado
   - Modal de consentimento é exibido quando dados salvos são detectados
   - O usuário pode aceitar ou recusar o armazenamento de dados

2. **Expiração Automática**
   - Dados armazenados expiram automaticamente após 7 dias
   - Dados expirados são removidos automaticamente na inicialização do serviço
   - O consentimento também expira após 7 dias

3. **Minimização de Dados**
   - Apenas dados necessários são armazenados
   - **Senhas NUNCA são armazenadas** no localStorage
   - Dados sensíveis são sanitizados antes do armazenamento

4. **Transparência**
   - Usuários são informados sobre:
     - Quais dados são armazenados
     - Por quanto tempo os dados são mantidos
     - Como os dados podem ser removidos
     - Direitos do titular dos dados

5. **Direito de Exclusão**
   - Usuários podem revogar o consentimento a qualquer momento
   - Revogação resulta na exclusão imediata de todos os dados armazenados
   - Dados são automaticamente removidos após conclusão bem-sucedida do cadastro

#### Métodos Principais:

```typescript
// Verificar se há consentimento
hasConsent(): boolean

// Conceder consentimento
grantConsent(): void

// Revogar consentimento e limpar dados
revokeConsent(): void

// Salvar dados do formulário
saveFormData(data: Partial<RegistrationRequest>): void

// Carregar dados salvos
loadFormData(): Partial<RegistrationRequest> | null

// Limpar dados do formulário
clearFormData(): void

// Verificar se há dados salvos
hasSavedData(): boolean

// Obter data de expiração
getExpirationDate(): Date | null
```

### 2. Fluxo de Cadastro Aprimorado

O componente de registro foi aprimorado para integrar o serviço de persistência de formulários.

**Localização:** `/frontend/mw-site/src/app/pages/register/`

#### Melhorias Implementadas:

1. **Auto-salvamento**
   - Dados são salvos automaticamente a cada 30 segundos
   - Salvamento ocorre ao avançar para a próxima etapa
   - Salvamento ocorre ao alterar o plano selecionado

2. **Recuperação de Dados**
   - Sistema detecta automaticamente se há dados salvos
   - Modal de consentimento é exibido se houver dados sem consentimento
   - Dados são carregados automaticamente com consentimento

3. **Etapa de Revisão de Plano (Novo)**
   - Nova etapa adicionada antes da confirmação final
   - Usuário pode revisar e alterar o plano selecionado
   - Plano previamente selecionado é destacado visualmente
   - Interface intuitiva com seleção por rádio

4. **Limpeza Automática**
   - Dados são removidos automaticamente após cadastro bem-sucedido
   - Previne acúmulo desnecessário de dados

### 3. Documentação Legal

Páginas dedicadas foram criadas para documentar políticas de privacidade e termos de uso.

#### Política de Privacidade

**Localização:** `/frontend/mw-site/src/app/pages/privacy/`

**URL:** `/privacy`

Inclui seções sobre:
- Tipos de dados coletados
- Finalidade do uso dos dados
- Armazenamento no navegador
- Compartilhamento de dados
- Medidas de segurança
- Direitos dos titulares (LGPD)
- Retenção de dados
- Cookies e tecnologias similares
- Contato do Encarregado de Dados (DPO)

#### Termos de Uso

**Localização:** `/frontend/mw-site/src/app/pages/terms/`

**URL:** `/terms`

Inclui seções sobre:
- Aceitação dos termos
- Descrição do serviço
- Cadastro e segurança da conta
- Período de teste gratuito
- Planos e pagamentos
- Uso aceitável
- Dados e privacidade (referência à LGPD)
- Propriedade intelectual
- Garantias e limitações
- Lei aplicável

### 4. Interface de Usuário

#### Modal de Consentimento

Um modal responsivo e acessível foi criado para solicitar consentimento:

- Design claro e profissional
- Informações transparentes sobre o armazenamento
- Botões de ação bem definidos (Aceitar/Recusar)
- Lista de benefícios e garantias LGPD
- Responsivo para dispositivos móveis

#### Etapa de Seleção de Plano

Interface intuitiva para revisão e alteração de planos:

- Cards visuais com todos os planos disponíveis
- Plano selecionado claramente destacado
- Badge "Mais Indicado" para plano recomendado
- Lista compacta de recursos de cada plano
- Nota informativa sobre flexibilidade de mudança de plano

## Conformidade LGPD

### Princípios Atendidos

1. **Finalidade** ✓
   - Dados coletados apenas para finalidades específicas e informadas

2. **Adequação** ✓
   - Tratamento compatível com as finalidades informadas

3. **Necessidade** ✓
   - Limitação ao mínimo necessário
   - Senhas não são armazenadas localmente

4. **Livre Acesso** ✓
   - Usuário pode acessar seus dados salvos
   - Transparência sobre quais dados são armazenados

5. **Qualidade dos Dados** ✓
   - Dados são mantidos atualizados (auto-salvamento)
   - Usuário pode corrigir dados a qualquer momento

6. **Transparência** ✓
   - Informações claras sobre coleta e uso
   - Política de privacidade completa

7. **Segurança** ✓
   - Medidas técnicas para proteger dados
   - Senhas nunca armazenadas localmente
   - Dados expiram automaticamente

8. **Prevenção** ✓
   - Medidas para prevenir danos
   - Expiração automática de dados

9. **Não Discriminação** ✓
   - Tratamento sem fins discriminatórios

10. **Responsabilização e Prestação de Contas** ✓
    - Demonstração de conformidade
    - Documentação de medidas adotadas

### Direitos dos Titulares Implementados

- ✓ **Confirmação da existência de tratamento:** Usuário pode verificar se há dados salvos
- ✓ **Acesso aos dados:** Usuário pode carregar dados salvos
- ✓ **Correção de dados:** Usuário pode editar formulário a qualquer momento
- ✓ **Eliminação:** Usuário pode revogar consentimento e excluir dados
- ✓ **Revogação do consentimento:** Função específica para revogar

## Uso Técnico

### Para Desenvolvedores

#### Integração do Serviço

```typescript
import { FormPersistenceService } from '../../services/form-persistence';

// No componente
private formPersistence = inject(FormPersistenceService);

// Verificar consentimento
if (this.formPersistence.hasConsent()) {
  // Salvar dados
  this.formPersistence.saveFormData(this.formData);
}

// Carregar dados
const savedData = this.formPersistence.loadFormData();
if (savedData) {
  Object.assign(this.model, savedData);
}
```

#### Auto-salvamento

```typescript
ngOnInit() {
  // Iniciar auto-save a cada 30 segundos
  this.autoSaveInterval = window.setInterval(() => {
    if (this.formPersistence.hasConsent()) {
      this.formPersistence.saveFormData(this.model);
    }
  }, 30000);
}

ngOnDestroy() {
  // Limpar intervalo
  if (this.autoSaveInterval) {
    clearInterval(this.autoSaveInterval);
  }
}
```

## Melhorias Futuras

1. **Criptografia Local**
   - Implementar criptografia de dados no localStorage
   - Usar Web Crypto API para maior segurança

2. **Auditoria**
   - Log de ações relacionadas a dados (com consentimento)
   - Registro de consentimentos concedidos/revogados

3. **Exportação de Dados**
   - Permitir que usuário exporte dados em formato estruturado
   - Atender direito de portabilidade da LGPD

4. **Notificações**
   - Notificar usuário quando dados estão próximos da expiração
   - Opção de renovar retenção de dados

## Referências

- [Lei Geral de Proteção de Dados (LGPD)](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [Guia de Boas Práticas LGPD - ANPD](https://www.gov.br/anpd)
- [Web Storage API](https://developer.mozilla.org/pt-BR/docs/Web/API/Web_Storage_API)

## Contato

Para questões sobre a implementação LGPD:
- **E-mail Técnico:** dev@medicwarehouse.com
- **DPO:** dpo@medicwarehouse.com

---

**Última Atualização:** 02 de novembro de 2025
