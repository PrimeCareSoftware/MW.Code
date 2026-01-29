# Guia do Usuário: Sistema SNGPC

**Versão:** 1.0  
**Data:** 29 de Janeiro de 2026  
**Público:** Médicos, Farmacêuticos, Recepcionistas, Equipe da Clínica

---

## 📚 Índice

1. [O que é SNGPC?](#o-que-é-sngpc)
2. [Por que devo me preocupar?](#por-que-devo-me-preocupar)
3. [Prescrição de Medicamentos Controlados](#prescrição-de-medicamentos-controlados)
4. [Dispensação de Medicamentos](#dispensação-de-medicamentos)
5. [Acompanhamento de Estoque](#acompanhamento-de-estoque)
6. [Balanço Mensal](#balanço-mensal)
7. [Dashboard SNGPC](#dashboard-sngpc)
8. [Alertas e Notificações](#alertas-e-notificações)
9. [Perguntas Frequentes](#perguntas-frequentes)

---

## O que é SNGPC?

**SNGPC** significa **Sistema Nacional de Gerenciamento de Produtos Controlados**. É um sistema da ANVISA (Agência Nacional de Vigilância Sanitária) que monitora a prescrição e dispensação de medicamentos controlados no Brasil.

### Medicamentos Controlados

São medicamentos que requerem receita especial e controle rigoroso, incluindo:

- **Lista A** - Entorpecentes (morfina, metadona, cannabis)
- **Lista B1** - Ansiolíticos e sedativos (clonazepam, diazepam, alprazolam)
- **Lista B2** - Anorexígenos (anfepramona, femproporex)
- **Lista C1** - Antidepressivos e outros (fluoxetina, amitriptilina)

### Objetivo

O SNGPC ajuda a:
- ✅ Prevenir uso indevido e desvio de medicamentos
- ✅ Monitorar prescrições e garantir uso responsável
- ✅ Combater a falsificação e comércio ilegal
- ✅ Proteger a saúde da população

---

## Por que devo me preocupar?

### Para Médicos

❗ **Você é responsável pela prescrição correta e legal de medicamentos controlados.**

- Cada prescrição é registrada no sistema
- Prescrições irregulares podem gerar alertas na ANVISA
- Excesso de prescrições pode ser investigado
- Não conformidade pode resultar em processo ético no CRM

### Para Farmacêuticos e Recepcionistas

❗ **Você é responsável pelo registro correto das movimentações.**

- Todas as dispensações devem ser registradas
- Estoque deve estar sempre correto
- Balanço mensal deve fechar sem divergências
- A clínica pode ser multada por registros incorretos

### Penalidades

- ⚠️ Advertência oficial
- 💰 Multas de R$ 2.000 a R$ 1.500.000
- 🔒 Suspensão de atividades
- ❌ Cancelamento de licença da clínica

---

## Prescrição de Medicamentos Controlados

### Como Prescrever

1. **Acesse o Atendimento do Paciente**
   - Navegue para **Atendimentos → Lista de Atendimentos**
   - Selecione o paciente e clique em **Iniciar Consulta**

2. **Vá para a Seção de Prescrição**
   - Clique na aba **Prescrição** ou **Receita**
   - O sistema detecta automaticamente se é medicamento controlado

3. **Preencha os Dados Obrigatórios**
   - **Nome completo do paciente** (sem abreviações)
   - **CPF do paciente** (obrigatório para controlados)
   - **Endereço completo do paciente**
   - **Medicamento** - Nome completo, concentração, forma farmacêutica
   - **Posologia** - "Tomar 1 comprimido a cada 8 horas por 5 dias"
   - **Quantidade** - Quantidade total a ser dispensada (número e por extenso)

4. **Revisão Automática**
   - O sistema verifica se todos os campos obrigatórios estão preenchidos
   - Alerta se falta alguma informação
   - Valida se a quantidade prescrita está dentro dos limites legais

5. **Finalizar Prescrição**
   - Clique em **Gerar Receita**
   - A receita é automaticamente registrada no SNGPC
   - Impressão disponível para entrega ao paciente

### Limites Legais

| Lista | Quantidade Máxima | Tratamento Máximo |
|-------|-------------------|-------------------|
| **A (Entorpecentes)** | 5 ampolas | 30 dias |
| **B1 (Ansiolíticos)** | 60 doses | 60 dias |
| **B2 (Anorexígenos)** | Variável | 60 dias |
| **C1 (Antidepressivos)** | Conforme necessidade | Sem limite específico |

### Dicas Importantes

✅ **Sempre preencha todos os campos** - Campos incompletos geram alertas  
✅ **Use nomenclatura correta** - "Clonazepam 2mg comprimido", não "Rivotril"  
✅ **Escreva posologia clara** - "Tomar 1 comprimido à noite", não "usar conforme orientação"  
✅ **Verifique CPF** - CPF incorreto impede dispensação  
✅ **Assinatura digital** - Use sua assinatura digital quando disponível

---

## Dispensação de Medicamentos

### Se sua clínica dispensa medicamentos controlados

1. **Registro da Dispensação**
   - Vá para **SNGPC → Dispensação**
   - Escaneie ou selecione a receita
   - Confirme os dados do paciente
   - Informe a data e hora da dispensação
   - Registre o lote e validade do medicamento

2. **Atualização Automática de Estoque**
   - O sistema automaticamente atualiza o estoque
   - Calcula o novo saldo
   - Registra a movimentação no livro de controle

3. **Validações**
   - ✅ Receita válida (dentro do prazo de validade)
   - ✅ Prescritor autorizado
   - ✅ Estoque disponível
   - ✅ CPF do paciente válido

---

## Acompanhamento de Estoque

### Visualizar Estoque Atual

1. **Acesse o Dashboard SNGPC**
   - Menu **SNGPC → Dashboard**
   - Visualize todos os medicamentos controlados

2. **Informações Disponíveis**
   - **Estoque atual** de cada medicamento
   - **Movimentações do mês** (entradas e saídas)
   - **Alertas** de estoque baixo ou negativo
   - **Histórico** de movimentações

### Entrada de Estoque (Compra)

1. **Registrar Compra**
   - Vá para **SNGPC → Movimentações → Nova Entrada**
   - Selecione o medicamento
   - Informe quantidade recebida
   - Anexe nota fiscal
   - Informe fornecedor, número da nota, data

2. **Validações Automáticas**
   - O sistema valida se o fornecedor é autorizado
   - Verifica se a nota fiscal é válida
   - Atualiza o estoque automaticamente

### Alerta de Estoque

O sistema gera alertas automáticos quando:
- 🟡 **Estoque baixo** - Abaixo de 10 unidades
- 🔴 **Estoque negativo** - CRÍTICO! Investigar imediatamente
- ⚠️ **Medicamento vencido** - Não pode ser dispensado

---

## Balanço Mensal

### O que é?

O **Balanço Mensal** é um resumo de todas as movimentações do mês, mostrando:
- Estoque inicial (do mês anterior)
- Total de entradas (compras, transferências)
- Total de saídas (dispensações)
- Estoque final (calculado)
- Estoque físico (contado fisicamente)
- Divergências (se houver)

### Como Fazer o Balanço

1. **Acesse Balanço Mensal**
   - Menu **SNGPC → Balanço Mensal**
   - O sistema calcula automaticamente o balanço

2. **Revisar Cálculos**
   - Verifique estoque inicial (deve bater com o final do mês anterior)
   - Confira total de entradas (somatória de compras)
   - Confira total de saídas (somatória de dispensações)
   - Estoque final = Inicial + Entradas - Saídas

3. **Inventário Físico**
   - Conte fisicamente os medicamentos
   - Registre a contagem real no sistema
   - O sistema compara: **Estoque Calculado** vs **Estoque Físico**

4. **Divergências**
   - Se houver diferença, o sistema solicita justificativa
   - Motivos comuns:
     - Vencimento de medicamento
     - Perda ou quebra
     - Erro de registro anterior
     - Roubo ou furto (comunicar autoridades)

5. **Fechar Balanço**
   - Após confirmar os dados, clique em **Fechar Balanço**
   - O balanço fica bloqueado para edição
   - Pode ser reabertoem caso de erro (com justificativa)

### Prazo Legal

⏰ **O balanço deve ser fechado até o dia 5 do mês seguinte**  
📤 **O relatório deve ser transmitido até o dia 10 do mês seguinte**

---

## Dashboard SNGPC

### Como Acessar

Menu **SNGPC → Dashboard**

### O que você vê

1. **Resumo do Mês Atual**
   - Total de prescrições emitidas
   - Total de dispensações realizadas
   - Medicamentos mais prescritos
   - Alertas ativos

2. **Estatísticas**
   - Gráfico de prescrições por médico
   - Gráfico de prescrições por medicamento
   - Comparativo mês a mês
   - Taxa de compliance (verde = 100%)

3. **Próximos Prazos**
   - Data limite para fechar balanço
   - Data limite para transmissão
   - Relatórios pendentes

4. **Alertas Ativos**
   - 🔴 Críticos (requerem ação imediata)
   - 🟡 Avisos (requerem atenção)
   - 🔵 Informativos

---

## Alertas e Notificações

### Tipos de Alertas

#### 🔴 Críticos (Ação Imediata)

- **Estoque Negativo** - Mais saídas do que entradas registradas
- **Relatório Vencido** - Prazo da ANVISA passou
- **Relatório Não Gerado** - Mês anterior sem relatório

#### 🟡 Avisos (Atenção)

- **Prazo Aproximando** - Faltam 5 dias para o prazo
- **Estoque Baixo** - Menos de 10 unidades
- **Divergência no Balanço** - Diferença entre calculado e físico

#### 🔵 Informativos

- **Relatório Transmitido** - Sucesso na transmissão
- **Balanço Fechado** - Confirmação de fechamento
- **Novo Medicamento** - Controlado adicionado ao sistema

### Como Lidar com Alertas

1. **Reconhecer Alerta**
   - Clique no alerta no dashboard
   - Leia a descrição completa
   - Clique em **Reconhecer** se você viu e está ciente

2. **Resolver Alerta**
   - Execute a ação necessária (ex: fechar balanço, corrigir estoque)
   - Clique em **Resolver**
   - Adicione uma descrição da resolução
   - O alerta é marcado como resolvido

3. **Histórico de Alertas**
   - Todos os alertas ficam registrados
   - Você pode ver quando foi criado, reconhecido, resolvido
   - Importante para auditorias

### Notificações por Email

Configure em **Configurações → Notificações**:
- ✅ Alertas críticos (recomendado)
- ✅ Prazos aproximando (recomendado)
- ⬜ Todos os alertas (pode ser muito)

---

## Perguntas Frequentes

### 1. O que fazer se eu prescrever errado?

**Resposta:** Se você ainda não finalizou a prescrição, pode editar. Se já finalizou:
- Entre em contato com o administrador do sistema
- Explique o erro
- O administrador pode fazer a correção ou cancelar a prescrição
- Uma nova prescrição correta deve ser feita

### 2. Esqueci de registrar uma dispensação. E agora?

**Resposta:**
- Registre assim que lembrar
- Informe a data/hora correta da dispensação
- Adicione uma observação explicando o atraso no registro
- Evite isso, pois pode gerar divergências no balanço

### 3. O estoque está negativo. O que fazer?

**Resposta:** Estoque negativo é **CRÍTICO**. Significa que houve mais saídas do que entradas.
- ✅ Verifique se alguma entrada não foi registrada
- ✅ Verifique se alguma dispensação foi registrada em duplicidade
- ✅ Conte fisicamente o estoque real
- ✅ Corrija os lançamentos com ajuda do administrador
- ❌ **NUNCA dispense medicamento com estoque negativo**

### 4. Posso prescrever mais de 60 dias de tratamento?

**Resposta:** Depende da lista:
- **Lista A** - Máximo 30 dias
- **Lista B1 e B2** - Máximo 60 dias
- **Lista C1** - Sem limite específico, use bom senso clínico
- Se precisar de mais, faça uma nova receita após o período

### 5. O paciente perdeu a receita. Posso fazer outra?

**Resposta:**
- ✅ Sim, você pode emitir uma segunda via
- ✅ Marque claramente "2ª VIA" na receita
- ✅ Informe ao paciente que só pode usar uma vez
- ✅ O sistema registra ambas, mas a farmácia deve verificar se já foi dispensada

### 6. Posso prescrever para mim mesmo ou familiares?

**Resposta:**
- ⚠️ O CRM desaconselha autoprescrição e prescrição para familiares diretos
- ⚠️ Para medicamentos controlados, isso é **fortemente desencorajado**
- ⚠️ Se for necessário, documente muito bem a justificativa
- ✅ Idealmente, consulte outro médico

### 7. Quando devo fazer o balanço mensal?

**Resposta:**
- 📅 Idealmente, no primeiro dia útil do mês seguinte
- 📅 Prazo legal: até dia 5 do mês seguinte
- 📅 Transmissão: até dia 10 do mês seguinte
- ⏰ Configure lembretes no sistema

### 8. O que acontece se passar do prazo?

**Resposta:**
- ⚠️ **1-3 dias de atraso:** Advertência (geralmente)
- 💰 **1 semana de atraso:** Multa de R$ 2.000 a R$ 10.000
- 💰 **1 mês de atraso:** Multa de R$ 10.000 a R$ 50.000
- 🔒 **Atraso recorrente:** Suspensão de atividades
- ❌ **Não envio sistemático:** Cancelamento de licença

### 9. Como funciona a transmissão para ANVISA?

**Resposta:**
- O administrador do sistema faz a transmissão
- O sistema gera um arquivo XML com todos os dados
- O arquivo é enviado via webservice da ANVISA
- A ANVISA retorna um protocolo de recebimento
- O status muda para "Transmitido" ✅

### 10. Posso ver minhas prescrições anteriores?

**Resposta:**
- ✅ Sim! Vá para **SNGPC → Minhas Prescrições**
- Filtre por período, paciente ou medicamento
- Veja estatísticas das suas prescrições
- Útil para auditorias e revisões

---

## 🔐 Segurança e Privacidade

### Acesso ao Sistema

- 🔒 Use senhas fortes e únicas
- 🔒 Nunca compartilhe sua senha
- 🔒 Use MFA/2FA quando disponível
- 🔒 Faça logout ao sair do computador

### Dados dos Pacientes

- 🔐 Dados de prescrição são **altamente sensíveis**
- 🔐 Acesso é **auditado** (tudo fica registrado)
- 🔐 Compartilhe apenas com quem precisa saber
- 🔐 Cumpra LGPD e sigilo médico

---

## 📞 Suporte

### Precisa de Ajuda?

**Suporte Técnico:**
- 📧 Email: suporte@primecare.com.br
- 📱 Telefone: (11) XXXX-XXXX
- 💬 Chat: Disponível no sistema (canto inferior direito)

**Dúvidas sobre ANVISA/SNGPC:**
- 🌐 Site da ANVISA: https://www.gov.br/anvisa/
- 📚 Manual SNGPC: Disponível no sistema em **Ajuda → Documentação**

**Problemas Técnicos:**
- Se o sistema está lento ou travado, recarregue a página
- Se não consegue acessar, verifique sua conexão com a internet
- Se continua com problemas, entre em contato com o suporte

---

## ✅ Checklist de Boas Práticas

### Diariamente

- [ ] Registrar todas as prescrições no sistema
- [ ] Registrar todas as dispensações imediatamente
- [ ] Verificar alertas no dashboard
- [ ] Conferir se não há estoque negativo

### Semanalmente

- [ ] Revisar prescrições da semana
- [ ] Verificar se há divergências no estoque
- [ ] Responder a alertas pendentes

### Mensalmente

- [ ] Fazer inventário físico (contar medicamentos)
- [ ] Fechar balanço mensal até dia 5
- [ ] Verificar se relatório foi transmitido até dia 10
- [ ] Revisar estatísticas e padrões de prescrição

### Anualmente

- [ ] Revisar treinamento SNGPC
- [ ] Atualizar certificado digital (se necessário)
- [ ] Revisar políticas internas de controle
- [ ] Fazer auditoria interna de compliance

---

**Versão:** 1.0  
**Última Atualização:** 29 de Janeiro de 2026  
**Próxima Revisão:** Julho de 2026  
**Documento aprovado por:** Equipe Técnica PrimeCare + Consultoria Jurídica
