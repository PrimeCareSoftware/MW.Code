# Guia do Usuário - Sistema de Notas Fiscais Eletrônicas (NF-e/NFS-e)

## Índice
1. [Introdução](#introdução)
2. [Contexto Legal](#contexto-legal)
3. [Pré-requisitos](#pré-requisitos)
4. [Configuração Inicial](#configuração-inicial)
5. [Emissão de Notas Fiscais](#emissão-de-notas-fiscais)
6. [Cálculos Tributários](#cálculos-tributários)
7. [Gerenciamento de Notas](#gerenciamento-de-notas)
8. [Cancelamento e Substituição](#cancelamento-e-substituição)
9. [Solução de Problemas](#solução-de-problemas)
10. [Boas Práticas](#boas-práticas)

---

## Introdução

O Sistema de Notas Fiscais Eletrônicas integrado ao Omni Care permite a emissão e gerenciamento de documentos fiscais obrigatórios para prestação de serviços médicos e venda de produtos no Brasil.

### Tipos de Documentos Suportados

- **NFS-e (Nota Fiscal de Serviços eletrônica)**: Para serviços médicos (consultas, procedimentos, exames)
- **NF-e (Nota Fiscal Eletrônica)**: Para venda de produtos (medicamentos, materiais)
- **NFC-e (Nota Fiscal ao Consumidor eletrônica)**: Para vendas ao consumidor final

---

## Contexto Legal

### Legislação Aplicável

A emissão de notas fiscais eletrônicas é regulamentada por:

- **Lei Complementar 116/2003**: Define os serviços sujeitos ao ISS
- **Emenda Constitucional 87/2015**: Regulamenta o ICMS
- **Ajuste SINIEF 07/2005**: Institui a Nota Fiscal Eletrônica
- **Legislações Municipais**: Cada município possui regras específicas para NFS-e

### Obrigatoriedade

⚠️ **IMPORTANTE**: A emissão de nota fiscal é obrigatória para:
- Prestação de serviços médicos com valor acima de R$ 10,00
- Vendas de produtos sujeitos ao ICMS
- Atendimentos a pessoas jurídicas (sempre)

---

## Pré-requisitos

Antes de utilizar o sistema, você precisa:

### 1. CNPJ Ativo
- CNPJ da clínica ou profissional (se autônomo) regularizado na Receita Federal
- Inscrição Municipal ativa (para NFS-e)
- Inscrição Estadual ativa (para NF-e e NFC-e)

### 2. Certificado Digital

📋 **Certificado necessário**: e-CNPJ tipo A1 ou A3

**Onde obter:**
- Autoridades Certificadoras credenciadas pelo ICP-Brasil
- Exemplos: Serasa, Certisign, Valid

**Validade:**
- Tipo A1: 1 ano (arquivo digital)
- Tipo A3: 1 a 3 anos (cartão ou token)

### 3. Conta em Gateway de Notas Fiscais

Contrate um provedor homologado:
- **Para NFS-e**: Verifique os provedores autorizados pelo seu município
- **Para NF-e/NFC-e**: Integração direta com a SEFAZ

Exemplos de provedores:
- NFe.io
- Focus NFe
- ENotas
- Bling

### 4. Dados Cadastrais Completos

- Endereço completo da empresa
- Dados bancários
- Código de serviço municipal (CNAE)
- Regime tributário (Simples, Lucro Presumido, Lucro Real)

---

## Configuração Inicial

### Passo 1: Acessar Configurações

1. Faça login no Omni Care
2. Acesse **Menu** → **Configurações** → **Notas Fiscais**

![Tela de configurações - placeholder para screenshot]

### Passo 2: Configurar Dados da Empresa

Complete os seguintes campos:

**Dados Básicos:**
- CNPJ
- Razão Social
- Nome Fantasia
- Inscrição Municipal
- Inscrição Estadual

**Endereço:**
- CEP (o sistema preenche automaticamente os demais campos)
- Número
- Complemento (se houver)

**Contato:**
- Telefone
- E-mail para envio de notas

### Passo 3: Configurar Regime Tributário

Selecione seu regime:

1. **Simples Nacional**
   - Alíquota varia de 6% a 33% conforme faturamento
   - Sistema calcula automaticamente o anexo aplicável

2. **Lucro Presumido**
   - Configure as alíquotas de ISS, PIS, COFINS, IRPJ e CSLL

3. **Lucro Real**
   - Configure todas as alíquotas aplicáveis

💡 **Dica**: Consulte seu contador para definir corretamente seu regime tributário.

### Passo 4: Configurar Certificado Digital

**Para certificado A1 (arquivo):**
1. Clique em **"Upload Certificado"**
2. Selecione o arquivo .pfx
3. Digite a senha do certificado
4. Clique em **"Validar"**

**Para certificado A3 (token/cartão):**
1. Conecte o dispositivo ao computador
2. Selecione **"Certificado A3"**
3. O sistema detectará automaticamente
4. Digite o PIN quando solicitado

✅ Certificado válido até: [data exibida]

### Passo 5: Configurar Gateway

1. Selecione seu provedor na lista
2. Insira as credenciais fornecidas pelo provedor:
   - API Key ou Token
   - ID da empresa (se aplicável)
3. Clique em **"Testar Conexão"**
4. Aguarde confirmação: ✅ "Conexão estabelecida com sucesso"

### Passo 6: Configurar Séries de Numeração

**NFS-e:**
- Série: (geralmente 1)
- Número Inicial: (obtido na prefeitura)

**NF-e:**
- Série: (geralmente 1)
- Número Inicial: (obtido na SEFAZ)

**NFC-e:**
- Série: (geralmente 1)
- Número Inicial: (obtido na SEFAZ)

⚠️ **ATENÇÃO**: Não altere a numeração após iniciar a emissão de notas.

---

## Emissão de Notas Fiscais

### Fluxo Automático

O sistema emite notas fiscais automaticamente quando:
- Um atendimento é finalizado (NFS-e para consultas)
- Uma venda é concluída no ponto de venda (NFC-e)
- Um produto é faturado (NF-e)

### Emissão Manual de NFS-e

**Quando usar:** Para serviços prestados fora do sistema ou lançamentos retroativos.

#### Passo a Passo:

1. Acesse **Faturamento** → **Notas Fiscais** → **Emitir NFS-e**

![Formulário de emissão - placeholder para screenshot]

2. **Dados do Tomador:**
   - Tipo: Pessoa Física ou Jurídica
   - CPF/CNPJ
   - Nome/Razão Social
   - Endereço completo
   - E-mail (para envio automático)

3. **Dados do Serviço:**
   - Código de Serviço (lista do município)
   - Descrição detalhada
   - Valor do Serviço
   - Alíquota de ISS
   - Retenção de Impostos (se aplicável)

4. **Informações Adicionais:**
   - Observações para o tomador
   - Dados adicionais para controle interno

5. Clique em **"Calcular Impostos"**
   - O sistema exibirá o detalhamento tributário
   - Valor líquido a receber

6. Revise os dados e clique em **"Emitir NFS-e"**

7. Aguarde o processamento:
   - ⏳ "Enviando para a prefeitura..."
   - ✅ "NFS-e emitida com sucesso!"
   - 📄 Número da nota: XXXXX

8. A nota é enviada automaticamente por e-mail ao tomador

### Emissão de NF-e (Produtos)

**Quando usar:** Para venda de medicamentos, materiais médicos, produtos ortopédicos.

#### Passo a Passo:

1. Acesse **Faturamento** → **Notas Fiscais** → **Emitir NF-e**

2. **Destinatário:**
   - Similar à NFS-e
   - Incluir Inscrição Estadual (se PJ)

3. **Produtos:**
   - Clique em **"Adicionar Produto"**
   - Selecione da lista ou cadastre novo
   - Informe:
     - Quantidade
     - Valor unitário
     - NCM (Nomenclatura Comum do Mercosul)
     - CFOP (Código Fiscal de Operações)
     - CST/CSOSN (tributação ICMS)
     - CST PIS/COFINS

4. **Cálculo de Impostos:**
   - Sistema calcula automaticamente:
     - ICMS
     - PIS
     - COFINS
     - IPI (se aplicável)

5. **Transporte:**
   - Modalidade (sem frete, por conta do destinatário, etc.)
   - Dados da transportadora (se aplicável)

6. **Pagamento:**
   - Forma de pagamento
   - Parcelas (se parcelado)

7. Clique em **"Emitir NF-e"**

8. Aguarde autorização da SEFAZ:
   - ⏳ "Aguardando autorização..."
   - ✅ "NF-e autorizada!"
   - Chave de acesso: XXXX XXXX XXXX XXXX XXXX XXXX...

### Emissão de NFC-e (Consumidor)

A NFC-e é emitida automaticamente pelo módulo de PDV (Ponto de Venda).

**Fluxo no PDV:**

1. Registre os produtos vendidos
2. Informe CPF/CNPJ do cliente (opcional)
3. Selecione forma de pagamento
4. Clique em **"Finalizar Venda"**
5. Sistema emite NFC-e automaticamente
6. QR Code é impresso no cupom

💡 **Dica**: Sempre pergunte ao cliente se deseja CPF/CNPJ na nota.

---

## Cálculos Tributários

### Entendendo os Impostos

#### Para Serviços (NFS-e):

**ISS (Imposto Sobre Serviços):**
- Alíquota: 2% a 5% (varia por município)
- Base de cálculo: valor do serviço
- Quem paga: prestador (recolhe) ou tomador (retém)

**Simples Nacional:**
- Alíquota única contempla todos os tributos
- Anexo III: Serviços médicos
- Faixa varia conforme faturamento anual

**Retenções na Fonte (quando tomador é PJ):**
- IRRF: 1,5%
- PIS: 0,65%
- COFINS: 3%
- CSLL: 1%
- Total retido: 6,15%

#### Para Produtos (NF-e/NFC-e):

**ICMS:**
- Varia por estado (7% a 18%)
- Substituição tributária (ICMS-ST) para medicamentos

**PIS e COFINS:**
- Regime cumulativo: 0,65% e 3%
- Regime não-cumulativo: 1,65% e 7,6%

**IPI (se aplicável):**
- Produtos específicos
- Varia conforme classificação

### Exemplo de Cálculo - NFS-e

```
Valor do Serviço: R$ 200,00
Regime: Simples Nacional (Anexo III - 11,20%)
Município: São Paulo (ISS 5%)

Cálculo:
- Simples Nacional: R$ 22,40 (11,20%)
- ISS devido: R$ 10,00 (5%)
- Valor Líquido: R$ 177,60

Na nota:
- Valor Total: R$ 200,00
- Desconto Simples: R$ 22,40
- Líquido a Receber: R$ 177,60
```

### Exemplo de Cálculo - NF-e (Lucro Presumido)

```
Produto: Medicamento X
Valor: R$ 100,00
Quantidade: 10 unidades
Valor Total: R$ 1.000,00

Impostos:
- ICMS (12%): R$ 120,00
- PIS (0,65%): R$ 6,50
- COFINS (3%): R$ 30,00
- Total Impostos: R$ 156,50

Valor Líquido: R$ 843,50
```

💡 **Dica**: O sistema calcula automaticamente. Os exemplos são para compreensão.

---

## Gerenciamento de Notas

### Listar Notas Fiscais

1. Acesse **Faturamento** → **Notas Fiscais** → **Consultar**

![Lista de notas - placeholder para screenshot]

2. Utilize os filtros:
   - **Período**: Data inicial e final
   - **Tipo**: NFS-e, NF-e, NFC-e
   - **Status**: Todas, Autorizadas, Canceladas, Erro
   - **Tomador/Destinatário**: Nome ou documento
   - **Número**: Número da nota

3. Clique em **"Buscar"**

### Visualizar Detalhes

1. Na lista, clique no número da nota
2. Visualize:
   - Dados completos da nota
   - Impostos calculados
   - Status de envio
   - Protocolo de autorização
   - Histórico de eventos

### Baixar Documentos

**Para cada nota você pode baixar:**

1. **PDF da Nota**
   - Clique no ícone 📄 **"PDF"**
   - Layout padrão do governo

2. **XML**
   - Clique no ícone 📋 **"XML"**
   - Arquivo para contabilidade

3. **DANFE** (NF-e/NFC-e)
   - Documento Auxiliar
   - Para acompanhar mercadoria

💾 **Dica**: Organize os arquivos por mês/ano para facilitar a contabilidade.

### Reenviar por E-mail

1. Na lista ou detalhes da nota
2. Clique em **"Reenviar E-mail"**
3. Confirme ou altere o endereço
4. Clique em **"Enviar"**
5. ✅ "E-mail enviado com sucesso!"

### Imprimir Nota

1. Abra os detalhes da nota
2. Clique em **"Imprimir"**
3. Selecione a impressora
4. Configure:
   - Uma via ou duas vias
   - Com ou sem canhoto de recebimento

---

## Cancelamento e Substituição

### Cancelar Nota Fiscal

⚠️ **IMPORTANTE**: Há prazos legais para cancelamento:
- **NFS-e**: Geralmente até o último dia do mês de emissão (varia por município)
- **NF-e**: Até 24 horas após autorização
- **NFC-e**: Até 30 minutos após autorização

#### Passo a Passo:

1. Acesse a nota que deseja cancelar
2. Verifique o status: ✅ "Autorizada"
3. Clique em **"Cancelar Nota"**
4. Digite o motivo do cancelamento (mínimo 15 caracteres):
   - Exemplo: "Emissão em duplicidade"
   - Exemplo: "Erro no valor dos serviços"
   - Exemplo: "Desistência do cliente"
5. Clique em **"Confirmar Cancelamento"**
6. Aguarde processamento:
   - ⏳ "Solicitando cancelamento..."
   - ✅ "Nota cancelada com sucesso!"

**Após o cancelamento:**
- Status muda para: 🚫 "Cancelada"
- Não é possível reverter
- Nova nota deve ser emitida se necessário

### Carta de Correção (NF-e)

Para corrigir pequenos erros em NF-e **já autorizada**:

**Pode corrigir:**
- Dados do destinatário (exceto CNPJ/CPF)
- Descrições de produtos
- Informações adicionais
- Dados de transporte

**Não pode corrigir:**
- Valores
- Quantidades
- CNPJ/CPF
- Data de emissão
- Tributos

#### Como fazer:

1. Abra a NF-e autorizada
2. Clique em **"Carta de Correção"**
3. Digite a correção necessária
4. Clique em **"Enviar"**
5. ✅ "Carta de Correção autorizada!"

📄 A CC-e fica vinculada à nota original.

### Substituir NFS-e

Alguns municípios permitem substituição em vez de cancelamento:

1. Cancele a nota original (se necessário)
2. Emita nova nota com dados corretos
3. No campo "Observações", referencie a nota substituída:
   - Exemplo: "Substitui NFS-e nº XXXXX"

### Inutilizar Numeração (NF-e)

Se pulou números na sequência:

1. Acesse **Configurações** → **Notas Fiscais** → **Inutilizar Numeração**
2. Informe:
   - Série
   - Número inicial
   - Número final
   - Justificativa
3. Clique em **"Inutilizar"**
4. ✅ Números marcados como inutilizados na SEFAZ

---

## Solução de Problemas

### Erro: "Certificado digital expirado"

**Causa**: O certificado digital venceu.

**Solução:**
1. Renove o certificado junto à Autoridade Certificadora
2. Faça upload do novo certificado no sistema
3. Valide a instalação

### Erro: "Rejeição 203 - CNPJ do emitente inválido"

**Causa**: CNPJ configurado está incorreto ou inativo.

**Solução:**
1. Verifique o CNPJ nas configurações
2. Consulte situação na Receita Federal
3. Regularize cadastro se necessário
4. Atualize no sistema

### Erro: "Rejeição 539 - CNPJ do destinatário em duplicidade"

**Causa**: Tentativa de emitir duas notas iguais.

**Solução:**
1. Verifique se a nota não foi emitida anteriormente
2. Se foi emitida, use a existente
3. Se necessário, cancele a primeira e emita nova

### Erro: "Série/número já utilizado"

**Causa**: Conflito na numeração sequencial.

**Solução:**
1. Consulte última nota emitida
2. Acesse **Configurações** → **Notas Fiscais**
3. Ajuste o próximo número sequencial
4. Tente novamente

### Erro: "Gateway não responde"

**Causa**: Problema de conexão com provedor.

**Solução:**
1. Verifique conexão com internet
2. Teste credenciais do gateway
3. Consulte status do provedor
4. Entre em contato com suporte do gateway

### Erro: "Valor do ISS divergente"

**Causa**: Alíquota incorreta configurada.

**Solução:**
1. Verifique alíquota do seu município
2. Atualize nas configurações
3. Recalcule os impostos
4. Emita novamente

### Nota não chega por e-mail

**Verificar:**
1. E-mail do destinatário está correto?
2. Verificar caixa de spam
3. Reenviar manualmente pelo sistema
4. Verificar configurações de SMTP

### XML não valida na SEFAZ

**Causa**: Inconsistências no arquivo XML.

**Solução:**
1. Baixe novamente o XML do sistema
2. Valide em validador online da SEFAZ
3. Se persistir, entre em contato com suporte

---

## Boas Práticas

### 1. Emissão de Notas

✅ **Faça:**
- Emita notas imediatamente após o serviço/venda
- Confira todos os dados antes de autorizar
- Solicite CPF/CNPJ dos clientes
- Mantenha descrições claras e detalhadas
- Arquive XML e PDF de todas as notas

❌ **Evite:**
- Deixar para emitir notas em lote no fim do mês
- Emitir notas com dados incompletos
- Cancelar notas sem real necessidade
- Usar mesma descrição genérica para todos os serviços

### 2. Controle de Numeração

- Nunca altere manualmente números de notas autorizadas
- Inutilize numeração pulada imediatamente
- Mantenha séries separadas por tipo de operação
- Configure backup da sequência numeração

### 3. Gestão Tributária

- Revise regime tributário anualmente com contador
- Monitore mudanças na legislação municipal
- Acompanhe faturamento para transição de faixas (Simples)
- Separe impostos retidos para recolhimento

### 4. Backup e Segurança

📋 **Rotina recomendada:**
- Backup mensal de todos os XMLs
- Armazenar em nuvem e local físico
- Manter organização: `/ano/mes/tipo/`
- Prazo legal de guarda: 5 anos (mínimo)

### 5. Certificado Digital

- Renove 30 dias antes do vencimento
- Mantenha backup do certificado A1
- Proteja senha com cofre de senhas
- Não compartilhe certificado entre máquinas desnecessariamente

### 6. Relacionamento com Órgãos

- Mantenha cadastro atualizado na Receita Federal
- Regularize pendências imediatamente
- Consulte dúvidas na prefeitura/SEFAZ
- Guarde protocolos de atendimento

### 7. Integração Contábil

- Envie XMLs mensalmente ao contador
- Compartilhe relatórios do sistema
- Informe mudanças de procedimentos
- Alinhe classificações fiscais

### 8. Atendimento ao Cliente

- Explique tributos ao emitir nota
- Envie nota por e-mail prontamente
- Mantenha histórico de notas acessível
- Reemita documentos quando solicitado

### 9. Conformidade Legal

📖 **Consulte regularmente:**
- Legislação municipal (NFS-e)
- Portarias da SEFAZ (NF-e)
- Atualizações do Simples Nacional
- Orientações do CRM (aspectos médicos)

### 10. Auditoria e Controle

- Confira mensalmente notas emitidas vs. faturamento
- Valide retenções de impostos
- Cruze dados com extratos bancários
- Reconcilie com sistema contábil

---

## Suporte e Contato

### Documentação Adicional

- [Portal da NF-e](http://www.nfe.fazenda.gov.br/)
- [Simples Nacional](http://www8.receita.fazenda.gov.br/simplesnacional/)
- Consulte legislação municipal no site da sua prefeitura

### Suporte Técnico

- **E-mail**: suporte@omnicare.com.br
- **Telefone**: (11) 1234-5678
- **Horário**: Segunda a Sexta, 8h às 18h
- **Chat**: Disponível no sistema

### Suporte do Gateway

Entre em contato diretamente com seu provedor para questões de:
- Instabilidade na transmissão
- Mudanças em credenciais
- Atualizações de certificados no gateway

---

## Glossário

**CFOP**: Código Fiscal de Operações e Prestações  
**CNAE**: Classificação Nacional de Atividades Econômicas  
**CST**: Código de Situação Tributária  
**DANFE**: Documento Auxiliar da Nota Fiscal Eletrônica  
**ICMS**: Imposto sobre Circulação de Mercadorias e Serviços  
**ISS**: Imposto Sobre Serviços  
**NCM**: Nomenclatura Comum do Mercosul  
**SEFAZ**: Secretaria da Fazenda  
**XML**: Extensible Markup Language (formato do arquivo da nota)

---

**Versão do documento**: 1.0  
**Última atualização**: Janeiro 2025  
**Sistema**: Omni Care v2.0

---

💡 **Lembre-se**: Este guia é orientativo. Para questões específicas da sua operação, consulte sempre seu contador e os órgãos reguladores.
