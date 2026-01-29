# Guia do Usuário: Receitas Médicas Digitais

**Versão:** 1.0  
**Data:** 29 de Janeiro de 2026  
**Público:** Médicos, Enfermeiros, Administradores de Clínica

---

## 📚 Índice

1. [Introdução](#introdução)
2. [Como Criar uma Prescrição](#como-criar-uma-prescrição)
3. [Tipos de Receita](#tipos-de-receita)
4. [Medicamentos Controlados](#medicamentos-controlados)
5. [Impressão e PDF](#impressão-e-pdf)
6. [QR Code](#qr-code)
7. [SNGPC - Relatórios Mensais](#sngpc---relatórios-mensais)
8. [Perguntas Frequentes](#perguntas-frequentes)
9. [Resolução de Problemas](#resolução-de-problemas)

---

## Introdução

O sistema de **Receitas Médicas Digitais** permite criar prescrições eletrônicas em conformidade com o CFM 1.643/2002 e ANVISA 344/1998. 

### Benefícios
- ✅ **Conformidade Legal** - Atende CFM e ANVISA
- ✅ **Segurança** - QR Code para verificação de autenticidade
- ✅ **Praticidade** - PDF profissional para impressão
- ✅ **Rastreabilidade** - Histórico completo de prescrições
- ✅ **SNGPC Automático** - Relatórios mensais gerados automaticamente

---

## Como Criar uma Prescrição

### Passo 1: Acessar o Módulo de Prescrições

1. No menu lateral, clique em **"Prescrições"**
2. Clique no botão **"+ Nova Prescrição"**

### Passo 2: Selecionar o Tipo de Receita

Escolha o tipo apropriado de receita:

- **Receita Simples** - Para medicamentos não controlados
- **Controle Especial A** - Entorpecentes (Lista A)
- **Controle Especial B** - Psicotrópicos (Lista B)
- **Controle Especial C1** - Outros controlados (Lista C1)
- **Antimicrobiano** - Antibióticos e antimicrobianos

💡 **Dica:** O sistema exibe informações de compliance ao lado de cada tipo.

### Passo 3: Preencher Dados do Paciente

Os dados do paciente são preenchidos automaticamente se você estiver criando a prescrição a partir de um atendimento.

### Passo 4: Adicionar Medicamentos

1. Clique em **"+ Adicionar Medicamento"**
2. Digite o nome do medicamento (autocomplete disponível)
3. Preencha:
   - **Dosagem**: Ex: "2mg", "500mg", "10ml"
   - **Forma Farmacêutica**: Ex: "Comprimido", "Cápsula", "Xarope"
   - **Quantidade**: Número de unidades
   - **Frequência**: Ex: "2x ao dia", "8 em 8 horas"
   - **Duração**: Número de dias de tratamento
   - **Instruções**: Ex: "Tomar após as refeições"

⚠️ **Atenção:** Para receitas controladas, **1 medicamento por receita** é obrigatório por lei.

### Passo 5: Revisar e Finalizar

1. Clique em **"Preview"** para visualizar a prescrição
2. Verifique todos os dados
3. Clique em **"Finalizar Prescrição"**

### Passo 6: Baixar PDF

Após finalizar, você pode:
- **Visualizar** o PDF da receita
- **Baixar** para salvar ou enviar ao paciente
- **Imprimir** diretamente

---

## Tipos de Receita

### 1. Receita Simples

**Quando usar:** Medicamentos de venda livre e alguns medicamentos tarjados não controlados.

**Características:**
- ✅ Validade: 30 dias
- ✅ Múltiplos medicamentos permitidos
- ✅ Cor: Branco
- ❌ Não requer SNGPC

**Exemplos:** Dipirona, Paracetamol, Amoxicilina (sem receita especial)

---

### 2. Controle Especial A (Entorpecentes)

**Quando usar:** Substâncias da Lista A (A1, A2, A3) da ANVISA - Entorpecentes e substâncias de alta dependência.

**Características:**
- ✅ Validade: 30 dias
- ✅ 1 medicamento por receita
- ✅ Cor: Amarelo (notificação de receita "A")
- ✅ Numeração sequencial obrigatória
- ✅ Requer SNGPC

**Exemplos:** Morfina, Codeína, Cetamina, Metadona

**Avisos:**
- 🔴 Notificação de Receita Tipo A em papel amarelo
- 🔴 Prescrição deve ser em duas vias
- 🔴 Válida apenas na unidade federativa do emissor

---

### 3. Controle Especial B (Psicotrópicos)

**Quando usar:** Substâncias da Lista B (B1, B2) - Psicotrópicos e anorexígenos.

**Características:**
- ✅ Validade: 30 dias
- ✅ 1 medicamento por receita
- ✅ Cor: Azul (notificação de receita "B")
- ✅ Numeração sequencial obrigatória
- ✅ Requer SNGPC

**Exemplos:** Rivotril (Clonazepam), Diazepam, Alprazolam, Ritalina

**Avisos:**
- 🔵 Notificação de Receita Tipo B em papel azul
- 🔵 Prescrição em duas vias
- 🔵 Válida em todo território nacional

---

### 4. Controle Especial C1 (Outros Controlados)

**Quando usar:** Substâncias da Lista C1 - Outros controlados.

**Características:**
- ✅ Validade: 30 dias
- ✅ Até 3 medicamentos por receita
- ✅ Cor: Branco (receita de controle especial)
- ✅ Numeração sequencial obrigatória
- ✅ Requer SNGPC

**Exemplos:** Alguns antidepressivos, anticonvulsivantes

---

### 5. Antimicrobiano

**Quando usar:** Antibióticos e antimicrobianos (RDC 20/2011).

**Características:**
- ✅ Validade: 10 dias (mais curta!)
- ✅ Múltiplos medicamentos permitidos
- ✅ Cor: Branco
- ✅ Retenção da 2ª via pela farmácia
- ❌ Não requer SNGPC (mas tem registro especial)

**Exemplos:** Amoxicilina + Clavulanato, Azitromicina, Ciprofloxacino

**Avisos:**
- ⏰ **Validade de apenas 10 dias**
- 📋 A farmácia deve reter a segunda via
- ⚠️ "Venda sob prescrição médica - Só pode ser vendido com retenção da receita"

---

## Medicamentos Controlados

### Como Identificar

O sistema automaticamente identifica medicamentos controlados no autocomplete:

- 🔴 **Ícone vermelho** = Lista A (Entorpecentes)
- 🔵 **Ícone azul** = Lista B (Psicotrópicos)
- 🟡 **Ícone amarelo** = Lista C1 (Outros controlados)
- 🟢 **Ícone verde** = Antimicrobiano

### Alertas Automáticos

Ao selecionar um medicamento controlado, o sistema exibe:
- ✅ Tipo de controle (A, B ou C1)
- ✅ Necessidade de notificação de receita
- ✅ Limite de medicamentos por receita
- ✅ Requisitos de SNGPC

### Regras Importantes

1. **Lista A e B:** 1 medicamento por receita
2. **Lista C1:** Até 3 medicamentos por receita
3. **Numeração:** Sistema gera automaticamente número sequencial
4. **SNGPC:** Prescrição é automaticamente incluída no relatório mensal

---

## Impressão e PDF

### Gerando o PDF

1. Após finalizar a prescrição, clique em **"Baixar PDF"**
2. O sistema gera um PDF profissional com:
   - ✅ Dados da clínica (nome, endereço, telefone)
   - ✅ Dados do médico (nome, CRM/UF)
   - ✅ Dados do paciente
   - ✅ Medicamentos prescritos com posologia
   - ✅ QR Code para verificação
   - ✅ Assinatura médica (quando assinado digitalmente)

### Layouts por Tipo

**Receita Simples:**
- Layout limpo e profissional
- Múltiplos medicamentos listados
- QR Code no canto superior direito

**Receita Controlada:**
- Marca d'água "RECEITA CONTROLADA" em cinza claro
- Número de notificação em vermelho e destaque
- Tipo de controle (A/B/C1) especificado
- 1 medicamento por página

**Receita Antimicrobiana:**
- Marca d'água "USO SOB ORIENTAÇÃO MÉDICA"
- Header "RDC 20/2011 ANVISA"
- Box amarelo com avisos obrigatórios
- Validade de 10 dias destacada

### Impressão

💡 **Dica:** Configure sua impressora para:
- Tamanho: A4
- Orientação: Retrato
- Margens: Normais
- Escala: 100%

Para receitas controladas, imprima em:
- **Lista A:** Papel amarelo
- **Lista B:** Papel azul
- **Lista C1:** Papel branco (receituário especial)

---

## QR Code

### O que é?

Cada prescrição recebe um **QR Code único** que permite verificar sua autenticidade.

### Como Funciona

1. O paciente apresenta a receita na farmácia
2. O farmacêutico escaneia o QR Code
3. O sistema confirma:
   - ✅ Prescrição é autêntica
   - ✅ Dados do médico
   - ✅ Dados do paciente
   - ✅ Validade da receita
   - ✅ Status (ativa/expirada/usada)

### Verificação Manual

Você também pode verificar uma prescrição manualmente:

1. Acesse **"Prescrições" → "Verificar Receita"**
2. Digite o código de verificação (abaixo do QR Code)
3. Clique em **"Verificar"**

---

## SNGPC - Relatórios Mensais

### O que é SNGPC?

**Sistema Nacional de Gerenciamento de Produtos Controlados** - Sistema da ANVISA para controle de medicamentos das Listas A, B e C1.

### Relatório Automático

O sistema **gera automaticamente** os relatórios mensais:

1. No início de cada mês, o sistema cria o relatório do mês anterior
2. Todas as prescrições controladas são incluídas automaticamente
3. Você precisa apenas **revisar e transmitir**

### Como Acessar o Dashboard SNGPC

1. Menu **"SNGPC" → "Dashboard"**
2. Você verá:
   - 📊 **Estatísticas** - Total de prescrições, não reportadas, vencidas
   - 📋 **Relatórios** - Lista de relatórios mensais
   - ⚠️ **Alertas** - Prazos e pendências

### Transmitir para ANVISA

**Prazo:** Até o dia **10 do mês seguinte** ao período de referência.

**Passos:**

1. Acesse o relatório mensal
2. Clique em **"Gerar XML"**
3. Revise o arquivo XML gerado
4. Clique em **"Download XML"**
5. Acesse o portal da ANVISA
6. Faça upload do XML
7. Após confirmação, volte ao sistema
8. Clique em **"Marcar como Transmitido"**
9. Informe o número de protocolo ANVISA

### Alertas

O sistema gera alertas automáticos:

- 🟡 **5 dias antes** do prazo - Aviso amarelo
- 🔴 **Prazo vencido** - Alerta vermelho crítico
- ℹ️ **Relatório não criado** - Lembrete
- ⚠️ **Saldo negativo** - Inconsistência detectada

---

## Perguntas Frequentes

### 1. Posso editar uma prescrição depois de finalizada?

❌ **Não.** Por questões legais e de auditoria, prescrições finalizadas não podem ser editadas. Você precisa criar uma nova prescrição.

### 2. Como cancelar uma prescrição?

Clique em **"Desativar"** na prescrição. Ela permanecerá no histórico mas marcada como inativa.

### 3. Posso prescrever 2 medicamentos controlados na mesma receita?

Depende do tipo:
- **Lista A ou B:** ❌ Não, 1 medicamento por receita
- **Lista C1:** ✅ Sim, até 3 medicamentos

### 4. O que fazer se esqueci de transmitir o SNGPC no prazo?

1. Transmita o mais rápido possível
2. A ANVISA pode aplicar penalidades por atraso
3. Registre no sistema o protocolo de transmissão
4. Documente o motivo do atraso para auditoria

### 5. Como reimprimir uma receita?

1. Acesse **"Prescrições" → "Histórico"**
2. Busque a prescrição
3. Clique em **"Baixar PDF"**

### 6. Posso enviar a receita por email/WhatsApp?

✅ **Sim**, você pode enviar o PDF da receita digitalmente. O QR Code garante a autenticidade.

### 7. Quanto tempo as receitas ficam armazenadas?

**20 anos** - Conforme exigência do CFM para documentos médicos eletrônicos.

### 8. Como funciona a assinatura digital?

- **Preparado:** Sistema aceita certificados ICP-Brasil (A1/A3)
- **Uso:** Clique em "Assinar" e selecione seu certificado
- **PDF:** A assinatura aparece visualmente no PDF

### 9. Posso criar prescrições em lote?

Atualmente não, mas está planejado para versão futura.

### 10. Como sei se um medicamento é controlado?

Digite o nome no campo de medicamento - o autocomplete mostra ícones coloridos indicando o tipo de controle.

---

## Resolução de Problemas

### Problema: Não consigo finalizar a prescrição

**Possíveis causas:**
- ❌ Campos obrigatórios não preenchidos
- ❌ Medicamento controlado sem classificação
- ❌ Mais de 1 medicamento em receita tipo A/B

**Solução:**
1. Verifique os campos em vermelho
2. Complete todas as informações obrigatórias
3. Para controlados A/B, remova medicamentos extras

---

### Problema: PDF não está gerando

**Possíveis causas:**
- ❌ Prescrição não foi finalizada
- ❌ Erro temporário no servidor

**Solução:**
1. Verifique se a prescrição está finalizada
2. Aguarde 30 segundos e tente novamente
3. Se persistir, contate o suporte

---

### Problema: QR Code não funciona na farmácia

**Possíveis causas:**
- ❌ Qualidade de impressão ruim
- ❌ Leitor de QR Code desatualizado

**Solução:**
1. Reimprima a receita em qualidade alta
2. Use o código de verificação manual (abaixo do QR Code)
3. Farmácia pode acessar o portal e verificar manualmente

---

### Problema: Alerta de SNGPC vencido

**Solução:**
1. Acesse **"SNGPC" → "Dashboard"**
2. Localize o relatório vencido
3. Clique em **"Gerar XML"**
4. Transmita para ANVISA imediatamente
5. Registre o protocolo no sistema

---

### Problema: Numeração sequencial com erro

**Solução:**
1. **Não tente corrigir manualmente**
2. O sistema garante a numeração automática
3. Se detectar erro, contate imediatamente o suporte técnico
4. Não crie novas receitas controladas até resolver

---

## 📞 Suporte

> **Nota:** Os números de telefone abaixo são placeholders. Substitua pelos números reais da sua clínica antes de distribuir este guia.

**Email:** suporte@primecaresoftware.com  
**Telefone:** (11) XXXX-XXXX _(substituir com número real)_  
**Horário:** Segunda a Sexta, 8h às 18h

---

## 📖 Referências Legais

- **CFM 1.643/2002** - Prescrições médicas digitais
- **ANVISA Portaria 344/1998** - Substâncias controladas
- **ANVISA RDC 20/2011** - Antimicrobianos
- **ANVISA RDC 22/2014** - SNGPC

---

**Última Atualização:** 29 de Janeiro de 2026  
**Versão:** 1.0  
**Autor:** PrimeCare Software
