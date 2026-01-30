# ⚠️ ATENÇÃO: Alterações de Configuração - SNGPC XML Validation

**Data:** 30 de Janeiro de 2026  
**Impacto:** Potencialmente Breaking Change  
**Categoria:** Configuração e Validação

---

## 🔴 Mudanças Importantes

### 1. Caminho do Schema XSD Alterado

**Antes:**
```json
"XsdSchemaBasePath": "docs/schemas"
```

**Depois:**
```json
"XsdSchemaBasePath": "wwwroot/schemas"
```

**Ação Necessária:**
- ✅ Schema já foi copiado para o novo local
- ⚠️ **IMPORTANTE:** Remover arquivo antigo em `docs/schemas/` se existir em deployments antigos
- ⚠️ Atualizar scripts de deployment se referenciarem o caminho antigo

---

### 2. Validação XSD Agora é Obrigatória

**Antes:**
```json
"RequireValidation": false  // Validação opcional, permitia XML inválido
```

**Depois:**
```json
"RequireValidation": true   // Validação obrigatória, bloqueia XML inválido
```

**Impacto:**
- ❌ XMLs que não passam validação XSD serão **REJEITADOS**
- ❌ Endpoint `/api/SNGPCReports/{id}/generate-xml` retornará erro se XML não for válido
- ❌ Transmissão para ANVISA será bloqueada

**Ação Necessária Antes de Deploy:**

1. **Testar XMLs Existentes:**
   ```bash
   # Regenerar todos os XMLs de relatórios existentes
   # Verificar se passam na validação
   ```

2. **Validar Dados de Teste:**
   - Gerar relatórios de teste com dados reais
   - Confirmar que XML passa na validação XSD
   - Verificar que não há campos obrigatórios faltando

3. **Plano de Rollback:**
   - Se houver problemas, reverter `RequireValidation` para `false` temporariamente
   - Corrigir dados que não passam validação
   - Re-habilitar validação

---

### 3. Schema XSD é Simplificado

**IMPORTANTE:** O schema incluído (`sngpc_v2.1.xsd`) é uma versão **simplificada** para validação básica.

**Implicações:**
- ✅ XML que passa nesta validação tem estrutura básica correta
- ⚠️ **MAS** pode ainda falhar na validação oficial da ANVISA
- Schema oficial completo pode ter validações adicionais não incluídas

**Recomendação:**
Antes de transmitir para ANVISA em **produção**, validar com schema oficial completo disponível em:
https://www.gov.br/anvisa/pt-br/assuntos/fiscalizacao-e-monitoramento/sngpc

**Opções:**
1. Baixar schema oficial completo da ANVISA e substituir o atual
2. Manter validação básica e confiar na validação da ANVISA ao transmitir
3. Implementar dupla validação (básica local + oficial antes de transmitir)

---

## 📋 Checklist de Deployment

Antes de fazer deploy em produção:

- [ ] Remover schema antigo de `docs/schemas/` (se existir)
- [ ] Confirmar que `wwwroot/schemas/sngpc_v2.1.xsd` existe no build
- [ ] Testar geração de XML com dados reais
- [ ] Verificar que XMLs passam validação XSD
- [ ] Atualizar documentação de deployment
- [ ] Preparar plano de rollback
- [ ] Considerar obter schema oficial completo da ANVISA

---

## 🔄 Rollback (Se Necessário)

Se houver problemas após deploy:

```json
// Reverter temporariamente em appsettings.json
"RequireValidation": false,
"XsdSchemaBasePath": "docs/schemas"  // Se schema antigo ainda existir
```

Então:
1. Investigar por que XMLs estão falhando validação
2. Corrigir dados ou lógica de geração
3. Re-habilitar validação após correção

---

## ⚠️ Avisos para Usuários

Se usuários reportarem erros ao gerar XML após esta atualização:

**Mensagem Típica:**
```
"XML validation failed against ANVISA schema"
```

**Causa Provável:**
- Dados de prescrição incompletos (faltam campos obrigatórios)
- Medicamento sem informações de substância controlada
- CPF/RG do paciente inválido ou faltando
- CRM do médico inválido

**Solução:**
1. Revisar dados da prescrição
2. Completar campos obrigatórios
3. Regenerar XML

---

**Documento Criado:** 30 de Janeiro de 2026  
**Próxima Revisão:** Após primeiro deployment em produção
