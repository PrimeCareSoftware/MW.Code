# 💳 Guia do Sistema de Pagamento

## 📋 Visão Geral

Este guia documenta o sistema de pagamento do PrimeCare Software, incluindo métodos de pagamento disponíveis, fluxos de cobrança e gestão de assinaturas.

**Status**: ✅ MVP Fase 1 - Funcional
**Gateway**: Integração com provedor de pagamento brasileiro
**Métodos**: PIX e Boleto Bancário

## 🎯 Métodos de Pagamento Disponíveis

### 1. PIX 📱

#### Características:
- ✅ **Confirmação Instantânea**: Pagamento confirmado em tempo real
- ✅ **Disponível 24/7**: Funciona inclusive em finais de semana e feriados
- ✅ **QR Code**: Escanear ou copiar código
- ✅ **Validade**: 30 minutos após geração
- ✅ **Sem Taxas**: Sem custo adicional para o cliente

#### Quando Usar:
- Ativação imediata da assinatura
- Pagamentos urgentes
- Renovações no último dia

### 2. Boleto Bancário 🏦

#### Características:
- ✅ **Aceito em Todo Brasil**: Pague em qualquer banco, lotérica ou app bancário
- ✅ **Prazo de Vencimento**: 3 dias corridos
- ✅ **Confirmação**: 1-2 dias úteis após pagamento
- ✅ **Segunda Via**: Disponível no portal
- ✅ **Sem Taxas**: Sem custo adicional para o cliente

#### Quando Usar:
- Pagamento programado
- Preferência por boleto
- Não tem urgência na ativação

## 🔄 Fluxos de Pagamento

### Fluxo PIX

#### 1. Seleção e Confirmação

```
Cliente seleciona plano
    ↓
Confirma dados e método de pagamento (PIX)
    ↓
Sistema gera QR Code PIX
    ↓
Código copia-e-cola também disponível
```

#### 2. Pagamento

```
Cliente abre app do banco
    ↓
Escaneia QR Code ou cola código
    ↓
Confirma pagamento no app
    ↓
Banco processa (instantâneo)
```

#### 3. Confirmação

```
Gateway recebe confirmação
    ↓
Webhook notifica o sistema
    ↓
Assinatura é ativada (automático)
    ↓
Cliente recebe email de confirmação
    ↓
Acesso liberado imediatamente
```

#### Tempo Total: 
- ⚡ **Pagamento**: Instantâneo
- ⚡ **Ativação**: Menos de 1 minuto

### Fluxo Boleto

#### 1. Geração

```
Cliente seleciona plano
    ↓
Confirma dados e método de pagamento (Boleto)
    ↓
Sistema gera boleto bancário
    ↓
Boleto enviado por email
    ↓
Link para segunda via no portal
```

#### 2. Pagamento

```
Cliente recebe email com boleto
    ↓
Abre o PDF do boleto
    ↓
Paga em banco/lotérica/app (até vencimento)
    ↓
Banco processa compensação
```

#### 3. Confirmação

```
Compensação bancária (1-2 dias úteis)
    ↓
Gateway recebe confirmação
    ↓
Webhook notifica o sistema
    ↓
Assinatura é ativada (automático)
    ↓
Cliente recebe email de confirmação
    ↓
Acesso liberado
```

#### Tempo Total:
- 📅 **Pagamento**: Até vencimento (3 dias)
- 📅 **Confirmação**: 1-2 dias úteis após pagamento
- 📅 **Ativação**: Imediata após confirmação

## 💰 Gestão de Assinaturas

### Ciclo de Cobrança Mensal

#### Primeira Assinatura

```
Dia 1: Cliente assina o plano
    ↓
Pagamento: PIX ou Boleto
    ↓
Confirmação: Imediata (PIX) ou 1-2 dias (Boleto)
    ↓
Ativação: Após confirmação
    ↓
Período: 30 dias de acesso
```

#### Renovação Mensal

```
Dia 25: Sistema gera cobrança do próximo mês
    ↓
Dia 26: Cliente recebe email com boleto/PIX
    ↓
Notificação: "Sua fatura está disponível"
    ↓
Dia 30: Lembrete (se não pago)
    ↓
Dia 31 (vencimento): Cobrança vence
    ↓
Após pagamento: Renovação confirmada
```

### Notificações de Cobrança

#### Calendário de Lembretes:

**7 dias antes**:
- 📧 Email: "Sua próxima fatura em 7 dias"
- 💬 Conteúdo: Valor, data de vencimento, link para pagamento

**3 dias antes**:
- 📧 Email: "Fatura vence em 3 dias"
- 💬 Conteúdo: Lembrete amigável, link direto para boleto/PIX

**Dia do vencimento**:
- 📧 Email: "Sua fatura vence hoje"
- 💬 Conteúdo: Último lembrete, evitar interrupção

**3 dias após vencimento** (se não pago):
- 📧 Email: "Fatura em atraso - Acesso será suspenso"
- 💬 Conteúdo: Aviso de suspensão em 48h

**5 dias após vencimento** (se não pago):
- 🚫 Sistema: Acesso suspenso
- 📧 Email: "Acesso suspenso - Regularize pagamento"

### Suspensão e Reativação

#### Suspensão por Inadimplência

**Quando ocorre**: 5 dias após vencimento sem pagamento

**O que acontece**:
- ❌ Login bloqueado para usuários
- 📧 Email para administrador
- 💾 Dados mantidos seguros (30 dias)
- ⚠️ Aviso: "Regularize para reativar"

**O que NÃO acontece**:
- ✅ Dados não são deletados
- ✅ Configurações mantidas
- ✅ Histórico preservado

#### Reativação

**Como reativar**:

1. Cliente acessa portal (área de pagamento)
2. Visualiza fatura em atraso
3. Efetua pagamento (PIX ou novo boleto)
4. Sistema confirma pagamento
5. Acesso é reativado automaticamente

**Tempo de reativação**:
- **PIX**: Imediato (após confirmação)
- **Boleto**: 1-2 dias úteis após pagamento

### Cancelamento de Assinatura

#### Como Cancelar

**Pelo Portal**:
1. Login no sistema
2. Menu **"Minha Assinatura"**
3. Botão **"Cancelar Assinatura"**
4. Confirmar cancelamento
5. (Opcional) Informar motivo

**Por Email**:
- Enviar para: assinaturas@primecaresoftware.com
- Assunto: "Solicitação de Cancelamento"
- Informar: Nome da clínica, CNPJ, email cadastrado

#### Efeitos do Cancelamento

**Acesso**:
- ✅ Mantido até o final do período pago
- Exemplo: Cancela dia 10, período vai até dia 30
- Acesso bloqueado a partir do dia 31

**Dados**:
- 💾 Backup mantido por 30 dias
- 📊 Após 30 dias: Dados são anonimizados
- 📧 Email de confirmação com instruções

**Cobrança**:
- ❌ Não há cobrança do próximo mês
- ❌ Sem multa de cancelamento
- ❌ Sem taxa administrativa

#### Período de Graça

**30 dias após cancelamento**:
- Cliente pode reativar sem perda de dados
- Basta efetuar novo pagamento
- Dados e configurações restaurados

**Após 30 dias**:
- Dados são anonimizados
- Nova assinatura = Novo cadastro
- Configurações precisam ser refeitas

### Upgrade de Plano

#### Como Fazer Upgrade

1. No portal, vá em **"Minha Assinatura"**
2. Clique em **"Alterar Plano"**
3. Selecione o novo plano (superior)
4. Revise a diferença de valor
5. Confirme o upgrade

#### Cobrança Proporcional

**Exemplo prático**:

```
Plano Atual: Starter (R$ 49/mês)
Novo Plano: Professional (R$ 89/mês)
Data do Upgrade: Dia 15 do mês
Período Restante: 15 dias

Cálculo:
- Valor Proporcional: (R$ 89 - R$ 49) × (15/30)
- Diferença: R$ 40 × 0.5 = R$ 20
- Cobrança Imediata: R$ 20

Próximo Mês:
- Cobrança Integral: R$ 89/mês
```

#### Efeito

- ✅ **Imediato**: Novos limites disponíveis
- ✅ **Sem Interrupção**: Zero downtime
- ✅ **Próximo Ciclo**: Valor integral do novo plano

### Downgrade de Plano

#### Como Fazer Downgrade

1. No portal, vá em **"Minha Assinatura"**
2. Clique em **"Alterar Plano"**
3. Selecione o novo plano (inferior)
4. **Atenção**: Verifique os novos limites
5. Confirme o downgrade

#### Validação de Limites

**Sistema verifica**:

```
Novo Plano: Starter (1 usuário, 50 pacientes)
Uso Atual: 2 usuários, 35 pacientes

Verificação:
- ❌ Usuários: Excede limite (precisa desativar 1)
- ✅ Pacientes: Dentro do limite

Ação Necessária:
- Desativar 1 usuário antes de confirmar downgrade
```

#### Efeito

- 📅 **Próximo Ciclo**: Mudança ocorre no próximo mês
- ✅ **Mês Atual**: Continua com plano atual
- 💰 **Próxima Cobrança**: Valor do novo plano
- ⚠️ **Limites**: Ajustar antes da mudança

## 🔍 Consultar Faturas

### Portal do Cliente

#### Faturas Abertas

1. Login no sistema
2. Menu **"Minha Assinatura"** > **"Faturas"**
3. Aba **"Abertas"**
4. Visualize faturas pendentes
5. Clique para pagar (PIX ou 2ª via boleto)

#### Histórico de Faturas

1. Menu **"Minha Assinatura"** > **"Faturas"**
2. Aba **"Histórico"**
3. Visualize todas as faturas pagas
4. Download de comprovantes
5. Notas fiscais (quando disponíveis)

### Informações da Fatura

Cada fatura mostra:

- 📅 **Período**: Mês de referência
- 💰 **Valor**: Valor total
- 📆 **Vencimento**: Data limite
- 🏷️ **Status**: Aberta, Paga, Vencida, Cancelada
- 🔗 **Ações**: Pagar, Baixar, Enviar por email

## 🔐 Segurança

### Proteção de Dados

#### No Pagamento:
- 🔒 **PCI-DSS Compliant**: Gateway certificado
- 🛡️ **TLS 1.3**: Criptografia em trânsito
- 🔑 **Tokenização**: Não armazenamos dados de cartão
- 📊 **Logs**: Todas as transações são auditadas

#### Dados Bancários:
- ✅ **Não Armazenamos**: Apenas IDs de transação
- ✅ **Gateway Seguro**: Provedor certificado
- ✅ **Conformidade**: PCI-DSS, LGPD

### Privacidade

- 📧 **Emails**: Enviados apenas para administrador
- 🔒 **Faturas**: Acesso apenas por login autenticado
- 🚫 **Sem Compartilhamento**: Dados nunca compartilhados
- 📊 **LGPD**: Total conformidade

## ❓ Perguntas Frequentes

### Geral

**P: Quais métodos de pagamento aceitam?**
R: PIX e Boleto Bancário. Cartão de crédito virá na Fase 2.

**P: Preciso pagar taxa de adesão?**
R: Não, apenas o valor mensal do plano escolhido.

**P: Tem período de trial/teste grátis?**
R: Sim, 14 dias grátis em todos os planos MVP.

### PIX

**P: Quanto tempo para confirmar o PIX?**
R: Confirmação instantânea, ativação em menos de 1 minuto.

**P: O QR Code expira?**
R: Sim, após 30 minutos. Gere um novo se expirar.

**P: Posso pagar PIX de outra pessoa?**
R: Sim, mas recomendamos pagar com CNPJ da clínica.

### Boleto

**P: Quanto tempo para compensar o boleto?**
R: 1-2 dias úteis após pagamento.

**P: Posso pagar após o vencimento?**
R: Sim, mas pode haver juros conforme banco. Gere novo boleto.

**P: Não recebi o boleto por email**
R: Acesse o portal e baixe a 2ª via em "Minhas Faturas".

### Assinatura

**P: Como cancelo minha assinatura?**
R: No portal, em "Minha Assinatura" > "Cancelar". Sem taxas.

**P: O que acontece após cancelar?**
R: Acesso mantido até fim do período pago. Dados por 30 dias.

**P: Posso reativar depois de cancelar?**
R: Sim, em até 30 dias sem perda de dados.

### Cobrança

**P: Quando recebo a próxima fatura?**
R: 5 dias antes do vencimento (dia 26 se vence dia 31).

**P: Posso mudar a data de vencimento?**
R: Sim, entre em contato com suporte.

**P: Tem desconto para pagamento anual?**
R: Ainda não, mas planejado para Fase 2.

## 📞 Suporte Financeiro

### Problemas com Pagamento

**Email**: financeiro@primecaresoftware.com
**Telefone**: (11) 99999-9999 (ramal 2)
**Horário**: Seg-Sex, 9h-18h

### Dúvidas sobre Fatura

**Email**: assinaturas@primecaresoftware.com
**Portal**: [https://app.primecaresoftware.com/suporte](https://app.primecaresoftware.com/suporte)

### Problemas Técnicos

**Email**: suporte@primecaresoftware.com
**Telefone**: (11) 99999-9999
**Chat**: Disponível no portal

## 📋 Documentação Fiscal

### Nota Fiscal

**Emissão**:
- Automática após confirmação do pagamento
- Enviada por email em até 5 dias úteis
- Disponível no portal em "Minhas Faturas"

**Informações**:
- Descrição: Assinatura PrimeCare Software
- CNAE: Serviços de tecnologia
- ISS: Conforme município

### Comprovante de Pagamento

**PIX**:
- Disponível imediatamente após pagamento
- No app do banco
- No portal em "Minhas Faturas"

**Boleto**:
- Comprovante do banco/lotérica
- No portal após compensação
- Solicitação de 2ª via: suporte

## 🔄 Integração Contábil

### Exportação de Dados

**Relatórios Disponíveis**:
- 📊 Histórico de faturas (CSV, Excel)
- 📄 Comprovantes em lote (PDF)
- 📋 Notas fiscais em lote (XML)

**Como Exportar**:
1. Menu **"Relatórios"** > **"Financeiro"**
2. Selecione período
3. Escolha formato
4. Clique em **"Exportar"**

### API (Fase 4)

Em desenvolvimento:
- 🔌 API para integração contábil
- 📊 Webhooks de cobrança
- 💼 Integração com ERPs

---

**Última atualização**: Janeiro 2026
**Versão do documento**: 1.0.0
