# Guia do Usuário - TUSS (Terminologia Unificada da Saúde Suplementar)

## 📋 Índice
1. [O que é TUSS](#o-que-é-tuss)
2. [Para que serve](#para-que-serve)
3. [Como funciona no Omni Care](#como-funciona-no-primecare)
4. [Estrutura dos Códigos TUSS](#estrutura-dos-códigos-tuss)
5. [Categorias de Procedimentos](#categorias-de-procedimentos)
6. [Como Buscar Procedimentos](#como-buscar-procedimentos)
7. [Cadastrando Procedimentos](#cadastrando-procedimentos)
8. [Tabela de Valores](#tabela-de-valores)
9. [Usando TUSS nos Atendimentos](#usando-tuss-nos-atendimentos)
10. [Perguntas Frequentes](#perguntas-frequentes)

---

## O que é TUSS?

**TUSS** significa **Terminologia Unificada da Saúde Suplementar** e é a **tabela padrão** estabelecida pela **ANS (Agência Nacional de Saúde Suplementar)** que define **códigos únicos** para:

- Procedimentos médicos e odontológicos
- Materiais e medicamentos
- Diárias hospitalares
- Taxas e aluguéis de equipamentos

### Por que existe?

Antes do TUSS, cada operadora de plano de saúde e cada clínica usava seus próprios códigos, o que causava:
- ❌ Confusão e erros no faturamento
- ❌ Dificuldade de comparação de preços
- ❌ Demora no processamento
- ❌ Glosas por códigos incompatíveis

O TUSS **padronizou** todos os códigos para que **clínicas e operadoras falem a mesma língua**.

### Relação entre TISS e TUSS

- **TISS** = O **formato** para troca de informações (as guias, os lotes, os XMLs)
- **TUSS** = A **tabela de códigos** usada dentro das guias TISS

**Analogia simples:**
- TISS é o "envelope e o papel"
- TUSS são os "códigos" escritos no papel

---

## Para que serve?

O TUSS serve para:

1. ✅ **Identificar procedimentos** de forma única e padronizada
2. ✅ **Facilitar o faturamento** com operadoras de planos
3. ✅ **Evitar erros** e glosas por código incorreto
4. ✅ **Permitir comparação** de valores entre diferentes serviços
5. ✅ **Garantir cobertura** - operadoras definem o que é coberto usando códigos TUSS

### Benefícios para sua clínica:

- 📋 **Padronização** - todos falam a mesma língua
- 💰 **Menos glosas** - códigos corretos = menos rejeições
- ⏰ **Agilidade** - operadoras processam mais rápido
- 📊 **Relatórios precisos** - sabe exatamente o que foi realizado

---

## Como funciona no Omni Care?

O Omni Care Software tem a **tabela TUSS integrada**, permitindo:

1. 🔍 **Buscar procedimentos** por código ou descrição
2. 📝 **Cadastrar procedimentos personalizados** usando códigos TUSS
3. 💵 **Definir valores** para cada procedimento
4. 📋 **Usar nos atendimentos** automaticamente
5. 📊 **Gerar guias TISS** com códigos corretos

### Fluxo de uso:

```
1. Cadastra procedimentos TUSS no sistema
   ↓
2. Define valores para cada procedimento
   ↓
3. Durante atendimento, seleciona procedimento
   ↓
4. Sistema busca automaticamente o código TUSS
   ↓
5. Código é incluído na guia TISS
   ↓
6. Operadora reconhece e processa corretamente
```

---

## Estrutura dos Códigos TUSS

### Formato do Código

Códigos TUSS têm **8 dígitos** no formato: `XX.XX.XX.XX`

**Estrutura:**
- **2 primeiros dígitos:** Grupo principal
- **2 segundos dígitos:** Subgrupo
- **4 últimos dígitos:** Procedimento específico

### Exemplo:

**Código:** `10.01.01.07-9`

- **10** = Procedimentos clínicos
- **01** = Consultas
- **01** = Consultas em consultório
- **07** = Consulta em consultório (no horário normal)
- **-9** = Dígito verificador

### Grupos Principais (primeiro par de dígitos):

| Código | Grupo |
|--------|-------|
| **10** | Procedimentos clínicos |
| **20** | Cirurgias |
| **30** | Procedimentos diagnósticos (exames) |
| **40** | Procedimentos terapêuticos |
| **50** | Transplantes |
| **60** | Medicamentos |
| **70** | Materiais |
| **80** | Diárias e taxas |
| **90** | Pacotes |

---

## Categorias de Procedimentos

### 1️⃣ Procedimentos Clínicos (Código 10.XX.XX.XX)

**Subcategorias principais:**

- **10.01.XX.XX** - Consultas
  - Consulta médica em consultório
  - Consulta de retorno
  - Consulta odontológica
  - Consulta de urgência

- **10.02.XX.XX** - Visitas hospitalares
  - Visita médica (internação)
  - Visita de acompanhamento

**Exemplos:**
- `10.01.01.07-9` - Consulta médica em consultório
- `10.01.01.01-0` - Consulta odontológica
- `10.02.01.03-0` - Visita hospitalar

### 2️⃣ Cirurgias (Código 20.XX.XX.XX)

Procedimentos cirúrgicos organizados por especialidade e complexidade.

**Exemplos:**
- `20.01.05.01-8` - Biópsia de pele
- `20.08.01.06-4` - Colecistectomia (remoção da vesícula)
- `20.10.03.02-5` - Artroscopia de joelho

### 3️⃣ Exames Diagnósticos (Código 30.XX.XX.XX)

**Subcategorias:**

- **30.01.XX.XX** - Exames laboratoriais
- **30.02.XX.XX** - Exames de imagem
- **30.03.XX.XX** - Endoscopias
- **30.04.XX.XX** - Exames cardiológicos
- **30.05.XX.XX** - Exames neurológicos

**Exemplos:**
- `30.01.01.01-4` - Hemograma completo
- `30.01.01.04-9` - Glicemia de jejum
- `30.02.01.01-8` - Raio-X de tórax
- `30.02.01.16-6` - Ultrassonografia abdominal
- `30.02.01.32-8` - Tomografia computadorizada de crânio
- `30.02.02.01-3` - Ressonância magnética de coluna

### 4️⃣ Procedimentos Terapêuticos (Código 40.XX.XX.XX)

**Subcategorias:**

- **40.01.XX.XX** - Fisioterapia
- **40.02.XX.XX** - Terapia ocupacional
- **40.03.XX.XX** - Fonoaudiologia
- **40.04.XX.XX** - Psicoterapia
- **40.05.XX.XX** - Nutrição
- **40.06.XX.XX** - Quimioterapia
- **40.07.XX.XX** - Radioterapia

**Exemplos:**
- `40.01.01.03-0` - Sessão de fisioterapia motora
- `40.04.01.01-0` - Sessão de psicoterapia individual
- `40.03.01.02-7` - Sessão de fonoaudiologia

### 5️⃣ Medicamentos (Código 60.XX.XX.XX)

Medicamentos padronizados pela ANS.

**Exemplos:**
- `60.01.01.01-1` - Paracetamol 500mg
- `60.02.03.05-8` - Omeprazol 20mg

**Observação:** A maioria dos medicamentos não está na tabela TUSS. Nesses casos, usa-se códigos genéricos ou tabela complementar da operadora.

### 6️⃣ Materiais (Código 70.XX.XX.XX)

Órteses, próteses e materiais especiais (OPME).

**Exemplos:**
- `70.01.01.01-6` - Prótese de quadril
- `70.02.03.04-2` - Stent coronário
- `70.05.01.01-9` - Marca-passo cardíaco

### 7️⃣ Diárias e Taxas (Código 80.XX.XX.XX)

**Subcategorias:**

- **80.01.XX.XX** - Diárias hospitalares
- **80.02.XX.XX** - Taxas de sala
- **80.03.XX.XX** - Gases medicinais

**Exemplos:**
- `80.01.01.01-2` - Diária de enfermaria
- `80.01.01.02-0` - Diária de apartamento
- `80.02.01.01-5` - Taxa de sala cirúrgica

### 8️⃣ Pacotes (Código 90.XX.XX.XX)

Conjuntos de procedimentos oferecidos em pacote.

**Exemplo:**
- `90.01.01.01-9` - Parto normal sem complicações (inclui consultas + exames + parto)

---

## Como Buscar Procedimentos

### No Omni Care Software

**Menu:** Configurações → Procedimentos TUSS → Buscar

### Busca por Código:

1. Digite o **código TUSS** (ex: `30.01.01.01`)
2. Sistema retorna o procedimento correspondente

### Busca por Descrição:

1. Digite **palavras-chave** do procedimento
2. Sistema busca na descrição

**Exemplos de busca:**
- "hemograma" → retorna código `30.01.01.01-4`
- "consulta" → retorna vários códigos de consulta
- "raio-x tórax" → retorna `30.02.01.01-8`
- "fisioterapia" → retorna códigos 40.01.XX.XX

### Filtros Avançados:

- **Por categoria:** Clínicos, Cirurgias, Exames, etc.
- **Por especialidade:** Cardiologia, Ortopedia, etc.
- **Por valor:** Ordenar do menor para o maior
- **Requer autorização:** Apenas procedimentos que exigem autorização prévia

---

## Cadastrando Procedimentos

### Procedimentos Padrão TUSS

**Menu:** Configurações → Procedimentos TUSS → Novo

1. Clique em **"Buscar na Tabela TUSS"**
2. Localize o procedimento desejado
3. Clique em **"Adicionar ao Sistema"**
4. Preencha informações adicionais:
   - **Nome personalizado:** Como você quer que apareça no sistema (opcional)
   - **Valor padrão:** Quanto sua clínica cobra (obrigatório)
   - **Tempo estimado:** Duração média do procedimento
   - **Requer autorização:** Marque se a operadora exige autorização prévia
   - **Ativo:** Marque para que apareça nas buscas
5. Clique em **"Salvar"**

### Procedimento Não Existe na Tabela TUSS?

Se o procedimento não está na tabela oficial:

1. Verifique se há um **código genérico** que se aplica
2. Entre em contato com as **operadoras** para saber qual código usar
3. Use códigos da **tabela AMB** ou **CBHPM** (se a operadora aceitar)
4. Registre como **"Outros procedimentos"** com código genérico

### Múltiplas Tabelas de Valores

Você pode ter **valores diferentes** para:

- **Particular:** Preço para pagamento direto
- **Por operadora:** Cada convênio tem sua tabela de valores
- **Por plano:** Dentro de uma operadora, planos podem ter valores diferentes

**Como configurar:**

1. Cadastre o procedimento com **valor padrão** (particular)
2. Vá em: **Configurações → Convênios → Operadoras → Ver Tabela de Valores**
3. Clique em **"Adicionar/Editar Valor"**
4. Selecione o **procedimento TUSS**
5. Defina o **valor** para aquela operadora/plano
6. Clique em **"Salvar"**

Quando você usar esse procedimento num atendimento de convênio, o sistema **automaticamente** buscará o valor correspondente.

---

## Tabela de Valores

### Tabelas de Referência

No Brasil, existem tabelas de referência de valores:

1. **AMB (Associação Médica Brasileira)**
   - Tabela tradicional de honorários médicos
   - Usado como referência por muitos convênios

2. **CBHPM (Classificação Brasileira Hierarquizada de Procedimentos Médicos)**
   - Atualização da AMB
   - Mais detalhada e moderna
   - Mantida pela AMB + CFM + outras entidades

3. **SIMPRO (Tabelas SIMPRO)**
   - Valores de materiais e medicamentos
   - Referência para OPME

4. **Tabelas próprias das operadoras**
   - Cada operadora pode ter valores próprios
   - Geralmente baseadas em % da AMB/CBHPM
   - Ex: "90% da AMB", "120% da CBHPM"

### Como Definir seus Valores

**Estratégias comuns:**

1. **Valor fixo por procedimento**
   - Você define o preço de cada procedimento
   - Ex: Consulta = R$ 200,00

2. **Percentual da tabela de referência**
   - Ex: 100% da AMB, 150% da CBHPM
   - Sistema calcula automaticamente

3. **Negociação com operadora**
   - Algumas operadoras negociam valores
   - Pode ter contrato com valores fixos

**Dica:** Mantenha uma **planilha atualizada** com:
- Código TUSS
- Descrição
- Valor AMB/CBHPM atualizado
- Seu valor particular
- Valor de cada convênio

### Atualizações da Tabela TUSS

A ANS atualiza a tabela TUSS periodicamente (geralmente a cada 1-2 anos).

**O que muda:**
- Novos procedimentos são adicionados
- Procedimentos obsoletos são removidos
- Descrições são atualizadas
- Códigos podem ser reorganizados

**No Omni Care:**
- Sistema é atualizado automaticamente
- Você é **notificado** de mudanças
- Procedimentos antigos ficam marcados como "obsoletos"
- Você precisa **migrar** para os novos códigos

---

## Usando TUSS nos Atendimentos

### Passo a Passo

**Durante o Atendimento:**

1. **Registre o atendimento** no prontuário eletrônico
2. Na seção **"Procedimentos Realizados"**, clique em **"Adicionar Procedimento"**
3. Sistema abre busca de procedimentos
4. **Busque** por:
   - Nome do procedimento, ou
   - Código TUSS
5. **Selecione** o procedimento desejado
6. Sistema **preenche automaticamente**:
   - Código TUSS
   - Descrição
   - Valor (baseado no convênio do paciente ou particular)
7. Ajuste a **quantidade** se necessário (ex: 2 radiografias)
8. Se necessário, marque **"Requer autorização"**
9. Clique em **"Adicionar"**

### Múltiplos Procedimentos

Você pode adicionar **vários procedimentos** no mesmo atendimento.

**Exemplo - Consulta + Exame:**
1. Adiciona `10.01.01.07-9` - Consulta médica
2. Adiciona `30.01.01.01-4` - Hemograma completo
3. Adiciona `30.02.01.01-8` - Raio-X de tórax

Cada procedimento vira **um item separado** na guia TISS.

### Procedimentos com Autorização

Se o procedimento **requer autorização prévia**:

1. Sistema **alerta** automaticamente
2. Você precisa informar o **número da autorização**
3. Sem autorização, procedimento fica **pendente**
4. Guia TISS só pode ser enviada **após** ter autorização

### Guia TISS é Criada Automaticamente

Quando você finaliza o atendimento com procedimentos de convênio:

1. Sistema **cria automaticamente** uma guia TISS
2. Guia fica em status **"Rascunho"**
3. Todos os códigos TUSS são incluídos
4. Valores são calculados automaticamente
5. Você só precisa **revisar e finalizar**

---

## Perguntas Frequentes

### 1. Todo procedimento tem código TUSS?

**Quase todos**, mas não 100%.

✅ **Têm código TUSS:**
- Consultas médicas
- Exames laboratoriais e imagens
- Cirurgias comuns
- Fisioterapia, psicoterapia
- Procedimentos ambulatoriais

❌ **Podem não ter:**
- Procedimentos muito novos
- Procedimentos experimentais
- Serviços não cobertos por convênios
- Procedimentos estéticos

Se não tem código TUSS, use:
- **Código genérico** da categoria
- **Tabela complementar** da operadora (se houver)
- **Descrição manual** + código "outros"

### 2. Posso criar meus próprios códigos?

**Não no padrão TISS.**

Códigos TUSS são **oficiais da ANS**. Você não pode inventar códigos.

O que você **pode fazer:**
- Usar **códigos genéricos** (ex: "Outros procedimentos clínicos")
- Usar **códigos da AMB/CBHPM** (se operadora aceitar)
- **Descrever manualmente** procedimentos não padronizados
- Cadastrar como **procedimento interno** (para controle) e usar código genérico TUSS no faturamento

### 3. O código TUSS define quanto vou receber?

**Não diretamente.**

- Código TUSS **identifica** o procedimento
- Cada operadora tem sua **tabela de valores**
- Valores podem ser baseados em AMB/CBHPM
- Ou podem ser **negociados** entre clínica e operadora

**Exemplo:**
- Código: `10.01.01.07-9` (Consulta médica)
- Valor AMB 2023: R$ 180,00
- Operadora A paga: 100% AMB = R$ 180,00
- Operadora B paga: 80% AMB = R$ 144,00
- Operadora C paga: valor fixo negociado = R$ 150,00

### 4. Qual a diferença entre TUSS, AMB e CBHPM?

| Tabela | O que é | Mantida por | Uso |
|--------|---------|-------------|-----|
| **TUSS** | Códigos de procedimentos | ANS | Obrigatória para planos de saúde |
| **AMB** | Valores de honorários médicos | AMB | Referência de preços (tradicional) |
| **CBHPM** | Códigos + valores atualizados | AMB + CFM | Substitui a AMB (mais moderna) |

**Resumo:**
- Use códigos **TUSS** nas guias (obrigatório)
- Use valores **CBHPM/AMB** como referência de preço
- Operadoras pagam baseado em % da CBHPM/AMB

### 5. Como saber se um procedimento requer autorização?

**Fontes:**

1. **Contrato com a operadora**
   - Lista procedimentos que exigem autorização
   - Geralmente: cirurgias, exames complexos, terapias

2. **Manual da operadora**
   - Disponível no portal da operadora
   - Lista procedimentos e regras

3. **Experiência**
   - Com o tempo, você aprende quais requerem

4. **No Omni Care**
   - Cadastre essa informação por operadora/plano
   - Sistema alerta automaticamente

**Regra geral (não absoluta):**
- ✅ Requerem: cirurgias, internações, exames de alta complexidade, terapias (múltiplas sessões)
- ❌ Não requerem: consultas, exames simples, urgências

### 6. O que acontece se eu usar o código TUSS errado?

**Consequências:**

1. **Glosa (não pagamento)**
   - Operadora rejeita a guia
   - Você não recebe

2. **Atraso no pagamento**
   - Operadora solicita retificação
   - Precisa reenviar com código correto

3. **Auditoria**
   - Operadora pode auditar
   - Em casos graves, pode resultar em multa

**Como evitar:**
- ✅ Sempre **verifique** o código antes de enviar
- ✅ Use a **busca** do sistema para encontrar códigos
- ✅ **Treine sua equipe** nos códigos mais usados
- ✅ **Revise** todas as guias antes do envio do lote

### 7. Posso cobrar mais que o valor TUSS/CBHPM?

**Depende:**

**Para convênios:**
- ❌ **Não**, você recebe o que o convênio paga (valor da tabela deles)
- Se o convênio paga 80% da CBHPM, você recebe 80%
- Você **não pode cobrar** a diferença do paciente (chamado "cobrança de diferença" ou "taxa de coparticipação não autorizada")

**Para particulares:**
- ✅ **Sim**, você define seu preço livremente
- Pode ser acima ou abaixo da tabela de referência
- Paciente paga o que você cobrar

**Exceção:**
- Se houver **coparticipação** definida no plano, paciente paga essa parte
- Se procedimento **não for coberto**, você pode cobrar à parte (mas informe o paciente ANTES)

### 8. Preciso decorar os códigos TUSS?

**Não é necessário**, mas ajuda conhecer os principais.

**Códigos que valem a pena memorizar:**

- `10.01.01.07-9` - Consulta médica em consultório
- `10.01.01.01-0` - Consulta odontológica
- `30.01.01.01-4` - Hemograma completo
- `30.01.01.04-9` - Glicemia
- `30.02.01.01-8` - Raio-X de tórax
- `40.01.01.03-0` - Sessão de fisioterapia

Para os demais, use a **busca do sistema**.

### 9. A tabela TUSS é gratuita?

**Sim**, a tabela TUSS é **pública e gratuita**.

Você pode:
- Consultar no site da ANS: [www.ans.gov.br/tiss](http://www.ans.gov.br/tiss)
- Baixar planilhas atualizadas
- Usar no seu sistema sem custo

**Mas:**
- Tabelas de **valores** (AMB/CBHPM) podem ser **pagas**
- Consultar é gratuito, mas planilhas oficiais podem ter custo
- Muitos convênios disponibilizam suas tabelas de valores gratuitamente

### 10. Como me atualizo sobre mudanças no TUSS?

**Fontes oficiais:**

1. **Site da ANS**
   - [www.ans.gov.br/tiss](http://www.ans.gov.br/tiss)
   - Notifica todas as atualizações

2. **E-mails das operadoras**
   - Operadoras informam seus credenciados sobre mudanças

3. **Omni Care Software**
   - Sistema é atualizado automaticamente
   - Você recebe notificação de mudanças

4. **Associações de classe**
   - AMB, CFM, CRM, CRO, etc.
   - Enviam comunicados sobre atualizações

**Frequência de atualizações:**
- **Grande atualização:** A cada 1-2 anos
- **Pequenas correções:** A cada 3-6 meses

---

## 📞 Suporte

Dúvidas sobre o uso da tabela TUSS no Omni Care?

- 📧 **E-mail:** suporte@omnicaresoftware.com
- 💬 **Chat:** Disponível no sistema (canto inferior direito)
- 📚 **Base de conhecimento:** [docs.omnicaresoftware.com](https://docs.omnicaresoftware.com)
- 🎥 **Vídeos tutoriais:** Canal do YouTube Omni Care Software

---

## 📚 Documentos Relacionados

- [Guia do Usuário - TISS](./GUIA_USUARIO_TISS.md)
- [Guia de Integração com Operadoras](./HEALTH_INSURANCE_INTEGRATION_GUIDE.md)
- [Status de Implementação TISS](./TISS_PHASE1_IMPLEMENTATION_STATUS.md)

---

## 🔗 Links Úteis

- **Site oficial TISS/TUSS:** [www.ans.gov.br/tiss](http://www.ans.gov.br/tiss)
- **Downloads da tabela TUSS:** [www.ans.gov.br/tiss/padroes](http://www.ans.gov.br/tiss/padroes)
- **Tabela CBHPM:** [cbhpm.org.br](https://cbhpm.org.br)
- **Resolução Normativa ANS sobre TISS:** RN 305/2012

---

**Última atualização:** Janeiro 2026  
**Versão:** 1.0  
**Elaborado por:** Omni Care Software
