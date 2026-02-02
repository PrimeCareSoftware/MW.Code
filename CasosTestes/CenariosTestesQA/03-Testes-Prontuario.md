# 03 - Cenários de Testes de Prontuário Médico

> **Módulo:** Prontuário SOAP e Atendimento Médico  
> **Tempo estimado:** 40-50 minutos  
> **Pré-requisitos:** Sistema configurado, logado como médico

## 🎯 Objetivo dos Testes

Validar o módulo de prontuário eletrônico SOAP:
- ✅ Criar prontuário SOAP completo
- ✅ Anexar documentos e exames
- ✅ Gerar prescrições digitais
- ✅ Versionamento e histórico
- ✅ Assinatura digital (CFM 1.821/2007)
- ✅ Conformidade com CFM 1.638/2002

## 📝 Principais Casos de Teste

### CT-PRONT-001: Criar Prontuário SOAP

**Passos:**
1. Login como doctor@demo.com
2. Acesse consulta agendada
3. Clique em "Iniciar Atendimento"
4. Preencha SOAP:
   - **S (Subjetivo):** "Paciente relata dor de cabeça há 3 dias"
   - **O (Objetivo):** "PA: 120/80 mmHg, FC: 72 bpm, Tax: 36.5°C"
   - **A (Avaliação):** "Cefaleia tensional"
   - **P (Plano):** "Prescrever analgésico, retorno em 7 dias"
5. Salve o prontuário

**Resultado Esperado:**
- ✅ Prontuário criado com sucesso
- ✅ Data/hora registradas automaticamente
- ✅ Status da consulta: "Em Atendimento"
- ✅ Todos os campos SOAP salvos

**Prioridade:** 🔴 Crítica

---

### CT-PRONT-002: Adicionar Prescrição ao Prontuário

**Passos:**
1. No prontuário aberto, clique em "Nova Prescrição"
2. Adicione medicamento:
   - Nome: Paracetamol 500mg
   - Posologia: 1 comprimido a cada 6 horas
   - Duração: 5 dias
   - Via: Oral
3. Adicione instruções: "Tomar após refeições"
4. Salve a prescrição

**Resultado Esperado:**
- ✅ Prescrição vinculada ao prontuário
- ✅ Gerado PDF da prescrição
- ✅ QR Code incluído no PDF
- ✅ Assinatura digital do médico

**Prioridade:** 🔴 Crítica

---

### CT-PRONT-003: Anexar Exames e Documentos

**Passos:**
1. No prontuário, clique em "Anexar Documento"
2. Selecione arquivo: exame.pdf (resultado de exame de sangue)
3. Tipo: Exame Laboratorial
4. Upload do arquivo
5. Salve

**Resultado Esperado:**
- ✅ Documento anexado com sucesso
- ✅ Thumbnail ou ícone exibido
- ✅ Possível visualizar/baixar
- ✅ Metadata registrada (data, tipo, tamanho)

**Prioridade:** 🔴 Crítica

---

### CT-PRONT-004: Visualizar Histórico do Paciente

**Passos:**
1. Abra prontuário de paciente
2. Clique em "Histórico"
3. Visualize consultas anteriores

**Resultado Esperado:**
- ✅ Lista de todos os atendimentos
- ✅ Ordenado por data (mais recente primeiro)
- ✅ Possível expandir cada consulta
- ✅ Ver prescrições anteriores

**Prioridade:** 🟡 Média

---

### CT-PRONT-005: Editar Prontuário (Versionamento)

**Passos:**
1. Abra prontuário existente
2. Clique em "Editar"
3. Adicione informação no campo Objetivo
4. Salve alteração

**Resultado Esperado:**
- ✅ Nova versão criada
- ✅ Versão anterior preservada
- ✅ Log de alteração com usuário e timestamp
- ✅ Possível ver versões anteriores

**Prioridade:** 🔴 Crítica (CFM 1.638/2002)

---

### CT-PRONT-006: Assinatura Digital do Prontuário

**Passos:**
1. Complete o prontuário
2. Clique em "Assinar Digitalmente"
3. Confirme com senha do certificado digital (ou PIN)

**Resultado Esperado:**
- ✅ Prontuário assinado com certificado ICP-Brasil
- ✅ Hash gerado e armazenado
- ✅ Impossível editar após assinar
- ✅ Selo de assinatura visível

**Prioridade:** 🔴 Crítica (CFM 1.821/2007)

---

### CT-PRONT-007: Finalizar Atendimento

**Passos:**
1. Complete todos os campos do prontuário
2. Clique em "Finalizar Atendimento"
3. Confirme

**Resultado Esperado:**
- ✅ Status da consulta: "Concluída"
- ✅ Prontuário salvo definitivamente
- ✅ Possível agendar retorno
- ✅ Paciente notificado

**Prioridade:** 🔴 Crítica

---

### CT-PRONT-008: Buscar no Histórico

**Passos:**
1. Acesse histórico do paciente
2. Use busca: "cefaleia"
3. Visualize resultados

**Resultado Esperado:**
- ✅ Busca em todos os campos SOAP
- ✅ Destaca termo buscado
- ✅ Mostra data das ocorrências

**Prioridade:** 🟡 Média

---

### CT-PRONT-009: Exportar Prontuário em PDF

**Passos:**
1. Abra prontuário
2. Clique em "Exportar PDF"
3. Visualize/baixe arquivo

**Resultado Esperado:**
- ✅ PDF gerado com formatação correta
- ✅ Inclui logo da clínica
- ✅ Dados do paciente
- ✅ SOAP completo
- ✅ Assinatura digital

**Prioridade:** 🟡 Média

---

### CT-PRONT-010: Validar Campos Obrigatórios

**Passos:**
1. Tente salvar prontuário sem preencher campos obrigatórios

**Resultado Esperado:**
- ✅ Validação impede salvamento
- ✅ Campos obrigatórios destacados
- ✅ Mensagens de erro claras

**Prioridade:** 🟡 Média

---

## ✅ Critérios de Aceite

- [ ] Prontuário SOAP pode ser criado
- [ ] Prescrições podem ser adicionadas
- [ ] Documentos podem ser anexados
- [ ] Versionamento funciona (CFM 1.638/2002)
- [ ] Assinatura digital funciona (CFM 1.821/2007)
- [ ] Histórico é preservado
- [ ] Busca funciona
- [ ] Exportação PDF funciona

## 📚 Documentação Relacionada

- [CFM 1.638 User Guide](../../system-admin/cfm-compliance/CFM_1638_USER_GUIDE.md)
- [Digital Signature Guide](../../ASSINATURA_DIGITAL_GUIA_USUARIO.md)

## ⏭️ Próximos Passos

➡️ Vá para [04-Testes-LGPD.md](04-Testes-LGPD.md)
