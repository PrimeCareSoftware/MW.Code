# 👨‍⚕️ Guia do Usuário - CFM 1.638/2002 Versionamento de Prontuários

**Versão:** 1.0  
**Última Atualização:** Janeiro 2026  
**Audiência:** Médicos, Enfermeiros e Equipe Clínica

---

## 📑 Índice

1. [Introdução](#introducao)
2. [O que é CFM 1.638/2002?](#o-que-e)
3. [Versionamento de Prontuários](#versionamento)
4. [Fechamento de Prontuários](#fechamento)
5. [Reabertura de Prontuários](#reabertura)
6. [Histórico de Versões](#historico-versoes)
7. [Auditoria de Acessos](#auditoria)
8. [Perguntas Frequentes](#faq)

---

## 📖 Introdução {#introducao}

Este guia explica como utilizar o sistema de versionamento de prontuários eletrônicos implementado em conformidade com a **Resolução CFM 1.638/2002** do Conselho Federal de Medicina.

### Por que isso é importante?

- ✅ **Segurança Jurídica**: Prontuários versionados são aceitos em processos legais
- ✅ **Rastreabilidade**: Todo acesso e modificação é registrado
- ✅ **Imutabilidade**: Prontuários fechados não podem ser alterados sem justificativa
- ✅ **Conformidade Legal**: Atende exigências do CFM e LGPD

---

## 🏥 O que é CFM 1.638/2002? {#o-que-e}

A **Resolução CFM 1.638/2002** estabelece requisitos de segurança e confiabilidade para prontuários eletrônicos:

### Principais Requisitos

1. **Versionamento Completo**
   - Cada alteração no prontuário gera uma nova versão
   - Versões anteriores nunca são deletadas
   - Histórico completo disponível para consulta

2. **Imutabilidade**
   - Prontuários fechados não podem ser editados
   - Alterações após fechamento viram adendos (nova versão)
   - Reabertura só com justificativa documentada

3. **Auditoria de Acessos**
   - Todo acesso é registrado (quem, quando, de onde)
   - Logs mantidos por 20+ anos
   - Disponíveis para fiscalização

4. **Assinatura Digital** (preparado)
   - Estrutura pronta para certificados ICP-Brasil
   - Hash SHA-256 de cada versão
   - Garantia de integridade

---

## 📝 Versionamento de Prontuários {#versionamento}

### Como Funciona?

Toda vez que você **salva** um prontuário, o sistema automaticamente:

1. ✅ Cria uma nova versão
2. ✅ Preserva a versão anterior
3. ✅ Registra quem fez a alteração
4. ✅ Armazena data e hora exatas
5. ✅ Calcula hash SHA-256 para integridade

### Exemplo Visual

```
Versão 1 (Criação)
├─ Anamnese: "Paciente relata dor no joelho..."
├─ Criado por: Dr. João Silva
├─ Data: 29/01/2026 10:00
└─ Status: Aberto

Versão 2 (Atualização)
├─ Anamnese: "Paciente relata dor no joelho direito há 3 dias..."
├─ Exame Físico: "Edema em joelho D..."
├─ Modificado por: Dr. João Silva
├─ Data: 29/01/2026 10:30
└─ Status: Aberto

Versão 3 (Fechamento)
├─ [Todos os campos anteriores]
├─ Diagnóstico: "CID M25.5 - Dor articular"
├─ Conduta: "Anti-inflamatório + Fisioterapia"
├─ Fechado por: Dr. João Silva
├─ Data: 29/01/2026 11:00
└─ Status: Fechado ✅
```

### O que é versionado?

- ✅ Anamnese
- ✅ Exame físico
- ✅ Hipóteses diagnósticas
- ✅ Prescrições
- ✅ Exames solicitados
- ✅ Atestados e documentos
- ✅ Evolução clínica
- ✅ Condutas terapêuticas

---

## 🔒 Fechamento de Prontuários {#fechamento}

### Quando Fechar?

Você deve **fechar o prontuário** quando:

- ✅ Atendimento está concluído
- ✅ Todas as informações foram preenchidas
- ✅ Prescrições e exames foram registrados
- ✅ Não há mais alterações a fazer

### Como Fechar?

**Passo 1:** Na tela do prontuário, clique em **"Concluir Atendimento"**

**Passo 2:** Revise todas as informações

**Passo 3:** Confirme o fechamento

### ⚠️ ATENÇÃO

Uma vez fechado, o prontuário:
- ❌ NÃO pode ser editado diretamente
- ❌ NÃO pode ter informações deletadas
- ✅ Pode ser reaberto com justificativa
- ✅ Alterações viram nova versão (adendo)

### Exemplo de Tela de Fechamento

```
┌─────────────────────────────────────────┐
│ ⚠️  Concluir Atendimento                 │
├─────────────────────────────────────────┤
│                                         │
│ Você está prestes a FECHAR este         │
│ prontuário. Após o fechamento:          │
│                                         │
│ ✓ O prontuário ficará imutável          │
│ ✓ Uma versão final será criada          │
│ ✓ Alterações requerem reabertura        │
│                                         │
│ Tem certeza que deseja continuar?      │
│                                         │
│ [ Cancelar ]  [ Confirmar Fechamento ]  │
└─────────────────────────────────────────┘
```

---

## 🔓 Reabertura de Prontuários {#reabertura}

### Por que reabrir?

Motivos válidos para reabertura:
- ✅ Esqueceu de registrar informação importante
- ✅ Precisa adicionar exame complementar
- ✅ Paciente retornou com nova queixa
- ✅ Erro na prescrição precisa ser corrigido

### Como Reabrir?

**Passo 1:** No prontuário fechado, clique em **"Reabrir Prontuário"**

**Passo 2:** **OBRIGATÓRIO** - Digite justificativa com no mínimo 20 caracteres

**Passo 3:** Sistema cria nova versão com sua justificativa

**Passo 4:** Faça as alterações necessárias

**Passo 5:** Feche novamente quando concluir

### Exemplo de Tela de Reabertura

```
┌─────────────────────────────────────────┐
│ 🔓 Reabrir Prontuário                    │
├─────────────────────────────────────────┤
│                                         │
│ Justificativa (mínimo 20 caracteres):  │
│ ┌─────────────────────────────────────┐ │
│ │ Necessário corrigir dosagem da      │ │
│ │ prescrição de dipirona de 500mg     │ │
│ │ para 750mg conforme indicação       │ │
│ │ clínica                             │ │
│ └─────────────────────────────────────┘ │
│                                         │
│ ⚠️  Atenção: A reabertura será          │
│    registrada no histórico              │
│                                         │
│ [ Cancelar ]  [ Confirmar Reabertura ]  │
└─────────────────────────────────────────┘
```

### ⚠️ Boas Práticas

✅ **Seja específico na justificativa:**
- ❌ Ruim: "Preciso alterar"
- ✅ Bom: "Necessário corrigir dosagem de dipirona de 500mg para 750mg"

✅ **Reabra apenas se realmente necessário**

✅ **Feche novamente após fazer as alterações**

---

## 📜 Histórico de Versões {#historico-versoes}

### Como Visualizar?

No prontuário do paciente:

1. Clique em **"Histórico de Versões"** ou ícone 📜
2. Veja lista completa de versões
3. Clique em qualquer versão para visualizar

### Informações Disponíveis

Para cada versão, você vê:

- 📅 **Data e hora** da alteração
- 👤 **Profissional** que fez a alteração
- 📝 **Tipo de alteração**: Criação, Atualização, Fechamento, Reabertura
- 📄 **Resumo das mudanças**
- 🔐 **Hash de integridade** (garante que não foi adulterado)

### Exemplo de Histórico

```
┌────────────────────────────────────────────────────┐
│ 📜 Histórico de Versões - Prontuário #12345        │
├────────────────────────────────────────────────────┤
│                                                    │
│ ✅ Versão 3 - FECHADO                              │
│    Fechado por Dr. João Silva                     │
│    29/01/2026 11:00                               │
│    Adicionado: Diagnóstico, Conduta               │
│    [ Visualizar ]                                 │
│                                                    │
│ 🔄 Versão 2 - ATUALIZADO                           │
│    Modificado por Dr. João Silva                  │
│    29/01/2026 10:30                               │
│    Adicionado: Exame Físico, Hipóteses Diag.      │
│    [ Visualizar ]                                 │
│                                                    │
│ 🆕 Versão 1 - CRIADO                               │
│    Criado por Dr. João Silva                      │
│    29/01/2026 10:00                               │
│    Criação inicial do prontuário                  │
│    [ Visualizar ]                                 │
│                                                    │
└────────────────────────────────────────────────────┘
```

### Comparar Versões

Você pode comparar duas versões para ver exatamente o que mudou:

1. Selecione versão mais antiga
2. Selecione versão mais recente
3. Clique em **"Comparar"**
4. Sistema mostra diferenças destacadas

```
Versão 1 → Versão 2
═══════════════════

Anamnese:
- Antes: "Paciente relata dor no joelho..."
+ Depois: "Paciente relata dor no joelho direito há 3 dias..."

Exame Físico:
+ Adicionado: "Edema em joelho D, dor à palpação..."
```

---

## 🔍 Auditoria de Acessos {#auditoria}

### O que é registrado?

TODOS os acessos ao prontuário são registrados:

- 👁️ **Visualização** - Quando alguém abre o prontuário
- ✏️ **Edição** - Quando salva alterações
- 🔒 **Fechamento** - Quando conclui atendimento
- 🔓 **Reabertura** - Quando reabre prontuário fechado
- 🖨️ **Impressão** - Quando imprime documento
- 📤 **Exportação** - Quando exporta PDF

### Informações Registradas

- 👤 Usuário que acessou
- 📅 Data e hora exatas
- 🌐 Endereço IP
- 💻 Dispositivo utilizado (navegador)
- 📝 Tipo de ação realizada

### Como Visualizar Logs de Auditoria?

**Para Médicos:**
- Você pode ver **seus próprios** acessos
- No prontuário, clique em **"Meus Acessos"**

**Para Administradores:**
- Podem ver todos os acessos
- Relatório completo de auditoria
- Exportação para análise

### Exemplo de Log de Auditoria

```
┌────────────────────────────────────────────────────┐
│ 🔍 Logs de Auditoria - Prontuário #12345           │
├────────────────────────────────────────────────────┤
│                                                    │
│ 📤 Exportação                                       │
│    Dr. João Silva                                 │
│    29/01/2026 14:30                               │
│    IP: 192.168.1.100                              │
│    Navegador: Chrome 120                          │
│                                                    │
│ 🔒 Fechamento                                       │
│    Dr. João Silva                                 │
│    29/01/2026 11:00                               │
│    IP: 192.168.1.100                              │
│                                                    │
│ ✏️ Edição                                           │
│    Dr. João Silva                                 │
│    29/01/2026 10:30                               │
│    IP: 192.168.1.100                              │
│                                                    │
│ 👁️ Visualização                                     │
│    Dr. João Silva                                 │
│    29/01/2026 10:00                               │
│    IP: 192.168.1.100                              │
│                                                    │
└────────────────────────────────────────────────────┘
```

---

## ❓ Perguntas Frequentes {#faq}

### 1. Por que não consigo editar um prontuário?

**R:** Provavelmente o prontuário está **fechado**. Você precisa **reabrir** com uma justificativa antes de fazer alterações.

---

### 2. Esqueci de adicionar uma informação. E agora?

**R:** 
1. Reabra o prontuário com justificativa clara
2. Adicione a informação
3. Feche novamente
4. A reabertura ficará registrada no histórico

---

### 3. Posso deletar uma versão antiga?

**R:** ❌ **NÃO**. As versões são permanentes e não podem ser deletadas. Isso garante rastreabilidade e conformidade legal.

---

### 4. E se eu cometer um erro?

**R:** 
- Se prontuário está **aberto**: Corrija e salve normalmente
- Se prontuário está **fechado**: Reabra, corrija e feche novamente

---

### 5. Quanto tempo os logs são mantidos?

**R:** Os logs de auditoria são mantidos por **20+ anos**, conforme exigido pela CFM 1.638/2002.

---

### 6. Quem pode ver o histórico de versões?

**R:**
- ✅ Médico que criou o prontuário
- ✅ Médicos autorizados do mesmo paciente
- ✅ Administradores do sistema
- ❌ Outros usuários sem permissão

---

### 7. O paciente pode ver o histórico?

**R:** Sim, se o sistema tiver Portal do Paciente ativado. O paciente pode:
- Ver versões do seu prontuário
- Ver quem acessou (nome do profissional e data)
- Não pode editar ou deletar

---

### 8. O que acontece se eu esquecer de fechar?

**R:** O prontuário permanece **aberto** e pode ser editado normalmente. Porém:
- ⚠️ Prontuários abertos não têm garantia de imutabilidade
- ⚠️ Podem ser contestados juridicamente
- ✅ Feche sempre que concluir o atendimento

---

### 9. Posso imprimir versões antigas?

**R:** ✅ Sim! Você pode visualizar e imprimir qualquer versão do histórico.

---

### 10. Como sei se alguém acessou o prontuário?

**R:** Verifique os **Logs de Auditoria**. Eles mostram todos os acessos com data, hora e usuário.

---

## 📞 Suporte

### Precisa de ajuda?

- 📧 **Email:** suporte@primecare.com.br
- 📱 **Telefone:** (11) 3000-0000
- 💬 **Chat:** Disponível no sistema (canto inferior direito)
- 📚 **Documentação:** [Documentação Técnica CFM 1.638](./CFM-1638-VERSIONING-README.md)

### Treinamento

Oferecemos treinamento presencial e online sobre:
- ✅ Uso correto do versionamento
- ✅ Boas práticas de fechamento
- ✅ Interpretação de logs de auditoria
- ✅ Conformidade legal

---

## 📚 Referências Legais

- **CFM 1.638/2002** - Prontuário Eletrônico
- **CFM 1.821/2007** - Campos Obrigatórios do Prontuário
- **CFM 2.218/2018** - Telemedicina (atualizada CFM 2.314/2022)
- **LGPD (Lei 13.709/2018)** - Proteção de Dados

---

## 🎓 Dicas de Uso

### ✅ Boas Práticas

1. **Feche o prontuário imediatamente** após concluir o atendimento
2. **Revise antes de fechar** - após fechado, reabrir gera registro
3. **Use justificativas claras** ao reabrir
4. **Consulte o histórico** quando tiver dúvidas sobre alterações
5. **Não compartilhe** seu login - cada acesso é rastreado

### ❌ O que evitar

1. ❌ Deixar prontuários abertos indefinidamente
2. ❌ Reabrir sem necessidade real
3. ❌ Usar justificativas genéricas ("preciso alterar")
4. ❌ Tentar deletar ou ocultar versões
5. ❌ Acessar prontuários sem necessidade clínica

---

## 🎯 Checklist do Atendimento

Use este checklist para garantir conformidade:

- [ ] Preenchi todos os campos obrigatórios
- [ ] Revisei anamnese e exame físico
- [ ] Registrei hipóteses diagnósticas
- [ ] Adicionei prescrições necessárias
- [ ] Solicitei exames complementares
- [ ] Orientei o paciente (registrado)
- [ ] Defini conduta terapêutica
- [ ] **Fechei o prontuário**

---

**Última Atualização:** Janeiro 2026  
**Versão do Documento:** 1.0  
**Próxima Revisão:** Julho 2026

---

*Este guia é parte integrante do sistema MedicSoft e está em conformidade com as resoluções do CFM e legislação brasileira vigente.*
