# 📋 TUSS (Terminologia Unificada da Saúde Suplementar) - Guia de Configuração e Testes

## 📌 Visão Geral

Este guia fornece instruções completas para configurar e testar a Tabela TUSS (Terminologia Unificada da Saúde Suplementar) no Omni Care Software, incluindo importação da tabela, cadastro de procedimentos, vinculação com convênios e precificação.

## 🔧 Pré-requisitos

- Sistema iniciado (API + Frontend)
- Usuário com perfil Owner logado
- Arquivo TUSS atualizado baixado do site da ANS
- Convênios cadastrados no sistema

## 📖 Índice

1. [O que é TUSS](#o-que-é-tuss)
2. [Estrutura da Tabela TUSS](#estrutura-da-tabela-tuss)
3. [Configuração Inicial](#configuração-inicial)
4. [Cenários de Teste - Importação](#cenários-de-teste---importação)
5. [Cenários de Teste - Procedimentos](#cenários-de-teste---procedimentos)
6. [Cenários de Teste - Precificação](#cenários-de-teste---precificação)
7. [Cenários de Teste - Integração](#cenários-de-teste---integração)
8. [API Testing](#api-testing)
9. [Troubleshooting](#troubleshooting)

---

## 🎯 O que é TUSS

TUSS (Terminologia Unificada da Saúde Suplementar) é a **tabela única** de referência para:

- 🏥 Procedimentos médicos, odontológicos e hospitalares
- 💊 Materiais e medicamentos
- 🔬 Exames diagnósticos
- 🩺 Serviços profissionais
- 🏨 Diárias e taxas

### Finalidade

- Padronizar a nomenclatura e codificação de procedimentos
- Facilitar o faturamento entre prestadores e operadoras
- Garantir transparência nas relações contratuais
- Permitir análise estatística e epidemiológica

### Versão TUSS Implementada

O Omni Care Software suporta **TUSS versão Vigente** (atualização automática via ANS)

---

## 📊 Estrutura da Tabela TUSS

### Hierarquia de Códigos

```
1. PROCEDIMENTOS MÉDICOS (10000000 - 19999999)
   ├── Consultas (10101012, 10101020, etc.)
   ├── Exames clínicos
   ├── Terapias
   └── Pequenas cirurgias

2. PROCEDIMENTOS ODONTOLÓGICOS (30000000 - 39999999)
   ├── Diagnóstico
   ├── Prevenção
   ├── Restaurações
   └── Cirurgias

3. PROCEDIMENTOS HOSPITALARES (20000000 - 29999999)
   ├── Cirurgias
   ├── Transplantes
   └── Procedimentos especiais

4. MATERIAIS E MEDICAMENTOS (40000000 - 49999999)
   ├── Medicamentos
   ├── Materiais descartáveis
   ├── Órteses e próteses
   └── Gases medicinais

5. DIÁRIAS, TAXAS E GASES (50000000 - 59999999)
   ├── Diárias de internação
   ├── Taxas de sala
   └── Honorários
```

### Informações em Cada Código TUSS

- **Código:** 8 dígitos numéricos
- **Descrição:** Nome do procedimento
- **Tipo:** Procedimento, material, diária, taxa
- **Especialidade:** Área médica relacionada
- **Sexo:** Se aplicável (M/F/Ambos)
- **Idade Mínima/Máxima:** Restrições de idade

---

## 🔧 Configuração Inicial

### 1. Baixar Tabela TUSS Atualizada

**Passos:**
1. Acesse o site da ANS: https://www.ans.gov.br
2. Vá em **"Prestadores"** → **"TISS"** → **"Tabela TUSS"**
3. Baixe a versão mais recente:
   - Arquivo: `TUSS_Vigente.xlsx` ou `TUSS_Vigente.xml`
4. Salve em local seguro

**Versão Atual (Jan/2026):** Verificar site ANS

---

### 2. Importar Tabela TUSS no Sistema

**Passos:**
1. Acesse **"Configurações"** → **"TISS/TUSS"** → **"Tabela TUSS"**
2. Clique em **"Importar Tabela"**
3. Selecione o arquivo baixado
4. Configure opções de importação:
   - [x] Sobrescrever códigos existentes
   - [x] Importar apenas procedimentos ativos
   - [x] Validar integridade dos dados
   - [x] Gerar log de importação

5. Clique em **"Iniciar Importação"**
6. Aguarde processamento (pode levar 5-10 minutos)

**Resultado Esperado:**
- ✅ Importação concluída
- ✅ XX.XXX procedimentos importados
- ✅ Log de importação disponível
- ✅ Tabela pronta para uso

---

### 3. Verificar Importação

**Passos:**
1. Acesse **"TISS/TUSS"** → **"Pesquisar TUSS"**
2. Teste buscas:
   - Por código: `10101012`
   - Por descrição: "Consulta médica"
   - Por especialidade: "Cardiologia"

**Resultado Esperado:**
- ✅ Códigos encontrados
- ✅ Descrições corretas
- ✅ Dados completos

---

## 🧪 Cenários de Teste - Importação

### Cenário 1.1: Importação Completa da Tabela

**Objetivo:** Importar toda a tabela TUSS

**Passos:**
1. Baixe arquivo oficial da ANS
2. Importe via interface
3. Aguarde conclusão
4. Verifique log

**Resultado Esperado:**
- ✅ Todos os códigos importados
- ✅ Sem erros de validação
- ✅ Backup da tabela anterior criado

---

### Cenário 1.2: Atualização Parcial

**Objetivo:** Atualizar apenas códigos modificados

**Passos:**
1. Sistema já tem TUSS anterior
2. Importe versão atualizada
3. Sistema compara versões
4. Atualiza apenas o que mudou

**Resultado Esperado:**
- ✅ Códigos novos adicionados
- ✅ Códigos alterados atualizados
- ✅ Códigos obsoletos desativados
- ✅ Histórico mantido

---

### Cenário 1.3: Importação com Erros

**Objetivo:** Tratamento de arquivo inválido

**Passos:**
1. Tente importar arquivo corrompido
2. Sistema valida
3. Identifica erros

**Resultado Esperado:**
- ❌ Importação rejeitada
- ✅ Lista de erros exibida
- ✅ Tabela anterior preservada
- ✅ Orientação para correção

---

## 🧪 Cenários de Teste - Procedimentos

### Cenário 2.1: Buscar Procedimento por Código

**Objetivo:** Localizar procedimento pelo código TUSS

**Passos:**
1. Acesse **"Pesquisar TUSS"**
2. Digite código: `10101012`
3. Pressione Enter

**Resultado Esperado:**
- ✅ Procedimento encontrado:
  - **Código:** 10101012
  - **Descrição:** Consulta médica em consultório
  - **Tipo:** Procedimento
  - **Especialidade:** Clínica Médica

---

### Cenário 2.2: Buscar por Descrição

**Objetivo:** Localizar por nome parcial

**Passos:**
1. Digite: "hemograma"
2. Busque

**Resultado Esperado:**
- ✅ Lista de procedimentos relacionados:
  - 40304310 - Hemograma completo
  - 40304329 - Hemograma com contagem de plaquetas
  - 40304337 - Hemograma com contagem de reticulócitos

---

### Cenário 2.3: Filtrar por Especialidade

**Objetivo:** Listar procedimentos de uma área

**Passos:**
1. Selecione filtro **"Especialidade"**
2. Escolha: **"Cardiologia"**
3. Aplique filtro

**Resultado Esperado:**
- ✅ Lista apenas procedimentos cardiológicos
- ✅ Ordenados por código
- ✅ Com descrições completas

---

### Cenário 2.4: Procedimentos Favoritos

**Objetivo:** Marcar procedimentos mais usados

**Passos:**
1. Busque procedimento: "Consulta médica"
2. Clique em ⭐ **"Adicionar aos Favoritos"**
3. Repita para outros procedimentos comuns
4. Acesse **"Meus Favoritos"**

**Resultado Esperado:**
- ✅ Lista personalizada criada
- ✅ Acesso rápido aos procedimentos
- ✅ Ordenação customizável

---

### Cenário 2.5: Detalhes do Procedimento

**Objetivo:** Ver informações completas

**Passos:**
1. Clique em um procedimento
2. Visualize detalhes:
   - Código TUSS
   - Descrição completa
   - Tipo de procedimento
   - Especialidade(s)
   - Restrições de sexo
   - Faixa etária permitida
   - Valor de referência
   - Data de vigência

**Resultado Esperado:**
- ✅ Todas as informações exibidas
- ✅ Histórico de alterações
- ✅ Versões anteriores disponíveis

---

## 🧪 Cenários de Teste - Precificação

### Cenário 3.1: Definir Valor Padrão

**Objetivo:** Configurar preço base para procedimento

**Passos:**
1. Acesse procedimento: 10101012 (Consulta médica)
2. Clique em **"Definir Valor"**
3. Configure:
   - **Valor Padrão:** R$ 200,00
   - **Baseado em:** CBHPM 2024
   - **Aplicar para:** Todos os convênios (padrão)

4. Salve

**Resultado Esperado:**
- ✅ Valor salvo
- ✅ Usado como referência
- ✅ Pode ser sobrescrito por convênio

---

### Cenário 3.2: Tabela de Valores por Convênio

**Objetivo:** Preços específicos por operadora

**Passos:**
1. Acesse **"Configurações"** → **"Tabelas de Preços"**
2. Crie tabela: **"Unimed São Paulo"**
3. Importe ou preencha valores:
   - Consulta médica: R$ 180,00
   - Hemograma: R$ 18,00
   - ECG: R$ 45,00
   - etc.

4. Vincule à operadora Unimed
5. Salve

**Resultado Esperado:**
- ✅ Tabela criada e vinculada
- ✅ Valores diferentes do padrão
- ✅ Usados automaticamente em guias TISS

---

### Cenário 3.3: Reajuste em Massa

**Objetivo:** Atualizar valores por percentual

**Passos:**
1. Selecione tabela de preços
2. Clique em **"Reajustar Valores"**
3. Configure:
   - **Tipo:** Percentual
   - **Valor:** +5%
   - **Aplicar em:** Todos os procedimentos
   - **Arredondamento:** 2 casas decimais

4. Visualize preview
5. Confirme reajuste

**Resultado Esperado:**
- ✅ Todos os valores aumentados em 5%
- ✅ Histórico de reajuste mantido
- ✅ Data de vigência registrada

---

### Cenário 3.4: Comparar Tabelas

**Objetivo:** Análise de valores entre convênios

**Passos:**
1. Acesse **"Comparar Tabelas"**
2. Selecione:
   - Tabela 1: Unimed
   - Tabela 2: Bradesco Saúde
   - Tabela 3: SulAmérica

3. Escolha procedimentos para comparar
4. Visualize relatório

**Resultado Esperado:**
- ✅ Tabela comparativa exibida
- ✅ Diferenças destacadas
- ✅ Identificação de melhores preços
- ✅ Exportação para Excel

---

### Cenário 3.5: Alertas de Preço

**Objetivo:** Notificar sobre valores desatualizados

**Passos:**
1. Configure em **"Configurações"**:
   - Alertar se valor diferir mais que 20% da referência
   - Verificar mensalmente

2. Sistema analisa tabelas
3. Gera relatório de alertas

**Resultado Esperado:**
- ✅ Lista de procedimentos com valores suspeitos
- ✅ Sugestão de atualização
- ✅ Comparação com mercado

---

## 🧪 Cenários de Teste - Integração

### Cenário 4.1: TUSS em Cadastro de Procedimento

**Objetivo:** Usar TUSS ao criar procedimento

**Passos:**
1. Acesse **"Cadastros"** → **"Procedimentos"**
2. Clique em **"+ Novo Procedimento"**
3. Clique em **"Buscar TUSS"**
4. Pesquise: "Hemograma"
5. Selecione: 40304310 - Hemograma completo
6. Sistema preenche:
   - Código TUSS
   - Nome
   - Descrição
   - Valor padrão

7. Complete outros campos:
   - Duração: 5 minutos
   - Requer preparo: Sim (jejum)
   - Especialidade: Patologia Clínica

8. Salve

**Resultado Esperado:**
- ✅ Procedimento criado com TUSS
- ✅ Código vinculado
- ✅ Usado em guias automaticamente

---

### Cenário 4.2: TUSS em Solicitação de Exame

**Objetivo:** Códigos TUSS em pedidos médicos

**Passos:**
1. Durante consulta, solicite exames
2. Sistema lista procedimentos cadastrados
3. Cada um com código TUSS
4. Médico seleciona exames
5. Gera solicitação

**Resultado Esperado:**
- ✅ Solicitação com códigos TUSS
- ✅ Aceita por laboratórios
- ✅ Facilita autorização de convênio

---

### Cenário 4.3: TUSS em Guia TISS

**Objetivo:** Códigos na guia de faturamento

**Passos:**
1. Finalize consulta
2. Gere guia TISS
3. Procedimentos automaticamente incluem:
   - Código TUSS
   - Descrição
   - Valor da tabela do convênio

**Resultado Esperado:**
- ✅ Guia completa e validada
- ✅ Códigos corretos
- ✅ Valores conforme contrato

---

### Cenário 4.4: Relatório de Procedimentos Mais Realizados

**Objetivo:** Análise estatística

**Passos:**
1. Acesse **"Relatórios"** → **"Procedimentos"**
2. Selecione período: Último mês
3. Visualize ranking:
   - 1º: 10101012 - Consulta médica (150x)
   - 2º: 40304310 - Hemograma (45x)
   - 3º: 40301354 - Glicemia (38x)

4. Filtre por convênio, médico, etc.

**Resultado Esperado:**
- ✅ Estatísticas precisas
- ✅ Baseadas em códigos TUSS
- ✅ Útil para gestão

---

### Cenário 4.5: Validação Automática

**Objetivo:** Sistema valida códigos em tempo real

**Passos:**
1. Tente adicionar procedimento com código inválido
2. Digite: 99999999
3. Sistema valida contra tabela TUSS

**Resultado Esperado:**
- ❌ Erro: "Código TUSS não encontrado"
- ✅ Sugere códigos similares
- ✅ Impede uso de código inválido

---

## 🔌 API Testing

### Endpoint: Buscar Procedimento TUSS

```bash
curl -X GET "http://localhost:5000/api/tuss/procedures?search=hemograma" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

**Resposta Esperada:**
```json
{
  "items": [
    {
      "code": "40304310",
      "description": "Hemograma completo",
      "type": "Laboratorial",
      "specialty": "Patologia Clínica",
      "referenceValue": 20.00
    }
  ],
  "totalCount": 1
}
```

---

### Endpoint: Obter Valor por Convênio

```bash
curl -X GET "http://localhost:5000/api/tuss/procedures/40304310/price?insuranceId={insurance_id}" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

**Resposta Esperada:**
```json
{
  "tussCode": "40304310",
  "insuranceName": "Unimed São Paulo",
  "price": 18.00,
  "currency": "BRL",
  "effectiveDate": "2026-01-01"
}
```

---

### Endpoint: Importar Tabela TUSS

```bash
curl -X POST "http://localhost:5000/api/tuss/import" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -F "file=@TUSS_Vigente.xlsx"
```

---

### Endpoint: Atualizar Preços em Massa

```bash
curl -X POST "http://localhost:5000/api/tuss/prices/bulk-update" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "insuranceId": "insurance-uuid",
    "adjustmentType": "Percentage",
    "adjustmentValue": 5.0,
    "effectiveDate": "2026-02-01"
  }'
```

---

## 🐛 Troubleshooting

### Problema 1: Código TUSS não encontrado

**Causa:** Tabela desatualizada

**Solução:**
1. Baixe versão mais recente da ANS
2. Reimporte a tabela
3. Verifique novamente

---

### Problema 2: Valores incorretos

**Causa:** Tabela de convênio não configurada

**Solução:**
1. Configure tabela específica do convênio
2. Importe valores oficiais
3. Valide os preços

---

### Problema 3: Importação lenta

**Causa:** Arquivo muito grande

**Solução:**
1. Use importação em horário de baixo uso
2. Aguarde conclusão (não interrompa)
3. Verifique log após finalizar

---

### Problema 4: Códigos duplicados

**Causa:** Múltiplas importações

**Solução:**
1. Use opção "Sobrescrever existentes"
2. Limpe tabela antiga antes
3. Importe versão única

---

## ✅ Checklist de Validação Final

- [ ] Baixar tabela TUSS oficial
- [ ] Importar tabela completa
- [ ] Verificar códigos principais
- [ ] Buscar por código
- [ ] Buscar por descrição
- [ ] Filtrar por especialidade
- [ ] Criar favoritos
- [ ] Definir valores padrão
- [ ] Criar tabela por convênio
- [ ] Reajustar valores em massa
- [ ] Comparar tabelas
- [ ] Configurar alertas
- [ ] Integrar com procedimentos
- [ ] Usar em solicitações
- [ ] Validar em guias TISS
- [ ] Gerar relatórios
- [ ] Validação automática
- [ ] Testes de API

---

## 📚 Documentação Relacionada

- [TISS (Guias e Faturamento)](04-TISS-PADRAO.md)
- [Guia do Usuário TUSS](../GUIA_USUARIO_TUSS.md)
- [Implementação TISS/TUSS](../TISS_TUSS_IMPLEMENTATION.md)
- [Avaliação TISS/TUSS](../EVALUATION_SUMMARY_TISS_TUSS.md)

## 🔗 Links Úteis

- [Portal ANS - Tabela TUSS](https://www.ans.gov.br/prestadores/tiss-troca-de-informacao-de-saude-suplementar/tuss-terminologia-unificada-da-saude-suplementar)
- [Download TUSS Vigente](https://www.ans.gov.br/images/stories/prestadores/TUSS_Vigente.zip)
- [Manual de Utilização TUSS](https://www.ans.gov.br/images/stories/prestadores/E-CAT-01.pdf)
