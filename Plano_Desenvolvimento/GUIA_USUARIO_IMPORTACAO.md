# 📖 Guia do Usuário - Sistema de Importação de Dados

> **Documento Complementar ao:** [PLANO_IMPORTACAO_DADOS.md](PLANO_IMPORTACAO_DADOS.md)  
> **Data de Criação:** 29 de Janeiro de 2026  
> **Versão:** 1.0  
> **Público-Alvo:** Administradores de Clínicas e Usuários Finais

## 🎯 Introdução

Este guia explica como utilizar o Sistema de Importação de Dados do Omni Care Software para migrar seus dados de outros sistemas de gestão médica para nossa plataforma.

### O que você pode importar?

- ✅ **Pacientes** (dados cadastrais completos)
- ✅ **Histórico de Agendamentos** (consultas passadas)
- ✅ **Prontuários Médicos** (respeitando CFM 1.821)
- ✅ **Planos de Saúde** (convênios dos pacientes)
- ✅ **Exames e Resultados** (documentos médicos)
- ✅ **Histórico Financeiro** (pagamentos e recebimentos)

## 🚀 Primeiros Passos

### Pré-requisitos

Antes de iniciar a importação, certifique-se de que:

1. **Você tem permissão de administrador** no Omni Care Software
2. **Você exportou os dados** do seu sistema atual
3. **Os dados estão em formato compatível:**
   - CSV (Excel salvo como CSV)
   - Excel (XLSX ou XLS)
   - JSON
   - XML

4. **Você revisou os dados** para garantir qualidade:
   - CPFs válidos e sem duplicatas
   - Datas no formato correto
   - Telefones e emails válidos

### Formatos de Arquivo Aceitos

| Formato | Extensão | Tamanho Máximo | Observações |
|---------|----------|----------------|-------------|
| CSV | .csv | 100 MB | Codificação UTF-8 recomendada |
| Excel | .xlsx, .xls | 100 MB | Máximo 100.000 linhas |
| JSON | .json | 50 MB | Formato de array de objetos |
| XML | .xml | 50 MB | Estrutura validada por XSD |

## 📋 Passo a Passo: Importar Pacientes

### Passo 1: Preparar o Arquivo

#### Opção A: Exportar de outro sistema

1. Acesse o sistema atual (iClinic, Ninsaúde, etc.)
2. Vá em **Relatórios** ou **Exportação de Dados**
3. Selecione **Pacientes** e escolha formato **CSV** ou **Excel**
4. Baixe o arquivo

#### Opção B: Criar arquivo manualmente

Se você não tem um sistema anterior, pode criar um arquivo Excel com as seguintes colunas:

**Colunas Obrigatórias:**
- `Nome` - Nome completo do paciente
- `CPF` - CPF no formato 000.000.000-00 ou 00000000000
- `DataNascimento` - Data no formato DD/MM/AAAA ou AAAA-MM-DD
- `Genero` - Masculino, Feminino ou Outro

**Colunas Opcionais (mas recomendadas):**
- `Email` - Email do paciente
- `Telefone` - Telefone com DDD (11) 98765-4321
- `CEP` - CEP do endereço
- `Endereco` - Rua, número e complemento
- `Bairro` - Bairro
- `Cidade` - Cidade
- `Estado` - UF (SP, RJ, MG, etc.)
- `NomeMae` - Nome da mãe (recomendado pelo CFM)
- `Alergias` - Alergias conhecidas
- `HistoricoMedico` - Histórico médico resumido

**Exemplo de arquivo CSV:**

```csv
Nome,CPF,DataNascimento,Genero,Email,Telefone,CEP
João da Silva,123.456.789-00,15/03/1985,Masculino,joao@email.com,(11) 98765-4321,01310-100
Maria Santos,987.654.321-00,22/07/1990,Feminino,maria@email.com,(11) 91234-5678,04567-890
```

### Passo 2: Acessar o Sistema de Importação

1. Faça login no Omni Care Software
2. No menu principal, clique em **⚙️ Configurações**
3. Selecione **📥 Importação de Dados**
4. Clique no botão **+ Nova Importação**

### Passo 3: Upload do Arquivo

1. Clique em **Selecionar Arquivo** ou arraste o arquivo para a área indicada
2. Selecione o tipo de dados: **Pacientes**
3. Selecione o formato: **CSV**, **Excel**, **JSON** ou **XML**
4. Clique em **Próximo**

> ⏱️ **Tempo de Upload:** Depende do tamanho do arquivo. Um arquivo de 10.000 pacientes leva cerca de 30-60 segundos.

### Passo 4: Mapeamento de Colunas

Esta é a etapa mais importante! Aqui você conecta as colunas do seu arquivo aos campos do Omni Care.

#### Mapeamento Automático

O sistema tentará detectar automaticamente as colunas. Revise as sugestões:

| Coluna do Arquivo | → | Campo do Omni Care | Status |
|-------------------|---|-------------------|--------|
| Nome Completo | → | Nome | ✅ Correto |
| CPF | → | Documento (CPF) | ✅ Correto |
| Data Nasc. | → | Data de Nascimento | ✅ Correto |
| Sexo | → | Gênero | ⚠️ Requer transformação |

#### Ajustar Mapeamento

Se algo estiver incorreto:

1. Clique no campo mapeado incorretamente
2. Selecione o campo correto na lista suspensa
3. Repita para todas as colunas

#### Transformações de Valores

Para a coluna **Sexo/Gênero**, você pode precisar transformar os valores:

| Valor no Arquivo | → | Valor no Omni Care |
|------------------|---|-------------------|
| M | → | Masculino |
| F | → | Feminino |
| Masc | → | Masculino |
| Fem | → | Feminino |

**Como configurar:**
1. Clique em **Configurar Transformações**
2. Selecione o campo: **Gênero**
3. Adicione as regras de transformação
4. Clique em **Salvar**

#### Usar Templates

Se você já importou dados deste sistema antes, pode usar um template salvo:

1. Clique em **Usar Template**
2. Selecione o template (ex: "iClinic", "Ninsaúde Apolo")
3. O mapeamento será aplicado automaticamente

**Templates Disponíveis:**
- 📋 iClinic
- 📋 Ninsaúde Apolo
- 📋 ClinicWeb
- 📋 Softmed
- 📋 Amplimed
- 📋 CSV Padrão Omni Care

### Passo 5: Validação de Dados

Antes de importar, o sistema validará todos os dados:

1. Clique em **Validar Dados**
2. Aguarde a validação (pode levar alguns minutos)
3. Revise o relatório de validação

#### Interpretando o Relatório

**✅ Sucesso:** 847 de 850 registros válidos

**⚠️ Avisos:** 3 registros com avisos
- Linha 15: Telefone em formato não padrão (será normalizado)
- Linha 89: CEP não encontrado (endereço pode estar incompleto)
- Linha 203: Email inválido (será deixado em branco)

**❌ Erros:** 0 registros com erros críticos

#### O que fazer com erros?

**Avisos (⚠️):** Você pode continuar, mas recomendamos revisar

**Erros Críticos (❌):** Você **deve** corrigir antes de continuar

**Como corrigir:**
1. Clique em **Baixar Relatório de Erros** (arquivo CSV)
2. Abra o arquivo em Excel
3. Corrija os erros indicados
4. Salve o arquivo
5. Volte ao **Passo 3** e faça upload do arquivo corrigido

#### Erros Comuns e Soluções

| Erro | Causa | Solução |
|------|-------|---------|
| CPF inválido | CPF digitado incorretamente | Verifique e corrija o CPF |
| Data inválida | Formato de data incorreto | Use DD/MM/AAAA ou AAAA-MM-DD |
| Paciente duplicado | CPF já existe no sistema | Remova da importação ou atualize dados |
| Campo obrigatório vazio | Nome, CPF ou Data Nascimento faltando | Preencha os campos obrigatórios |

### Passo 6: Preview dos Dados

Antes de importar, você pode visualizar como os dados ficarão:

1. Clique em **Visualizar Dados**
2. Revise os primeiros 10 registros
3. Confirme que as informações estão corretas
4. Se algo estiver errado, volte ao **Passo 4** (Mapeamento)

**Exemplo de Preview:**

```
┌──────────────────┬──────────────────┬─────────────┬───────────────┐
│ Nome             │ CPF              │ Nascimento  │ Gênero        │
├──────────────────┼──────────────────┼─────────────┼───────────────┤
│ João da Silva    │ 123.456.789-00   │ 15/03/1985  │ Masculino     │
│ Maria Santos     │ 987.654.321-00   │ 22/07/1990  │ Feminino      │
│ Pedro Oliveira   │ 456.789.123-00   │ 10/11/1978  │ Masculino     │
└──────────────────┴──────────────────┴─────────────┴───────────────┘
```

### Passo 7: Confirmar e Executar Importação

1. Revise o resumo da importação:
   - **Total de registros:** 850
   - **Registros válidos:** 847
   - **Registros com aviso:** 3
   - **Registros com erro:** 0

2. Selecione o comportamento para duplicatas:
   - ⭕ **Pular** - Não importar registros duplicados
   - 🔄 **Atualizar** - Atualizar dados existentes com os novos
   - ❌ **Cancelar** - Cancelar importação se houver duplicatas

3. Marque a caixa: ☑️ **Eu confirmo que revisei e valido os dados**

4. Clique em **Executar Importação**

> ⚠️ **IMPORTANTE:** Esta ação não pode ser desfeita automaticamente. Certifique-se de que os dados estão corretos!

### Passo 8: Acompanhar o Progresso

A importação será processada em segundo plano. Você pode:

1. **Acompanhar em tempo real:**
   - Barra de progresso mostra o andamento
   - Estimativa de tempo restante é atualizada
   - Você pode fechar a tela e voltar depois

2. **Ver Histórico:**
   - Menu **Importação de Dados** → **Histórico**
   - Lista todas as importações (concluídas e em andamento)

**Exemplo de Progresso:**

```
📊 Importando Pacientes...
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 65%

✅ Processados: 551 de 847
✅ Sucesso: 549
⚠️ Avisos: 2
❌ Falhas: 0

⏱️ Tempo estimado: 2 minutos
```

### Passo 9: Revisar Resultados

Quando a importação for concluída:

1. Você receberá uma **notificação por email**
2. No sistema, vá em **Importação de Dados** → **Histórico**
3. Clique na importação concluída
4. Revise o **Relatório Final**

**Relatório Final:**

```
✅ Importação Concluída

📊 Resumo:
- Total de registros: 847
- Importados com sucesso: 845
- Pulados (duplicados): 2
- Falhas: 0

⏱️ Tempo total: 4 minutos e 32 segundos

📄 Baixar relatório detalhado (PDF)
```

### Passo 10: Verificar os Dados Importados

1. Vá em **Pacientes** no menu principal
2. Você verá os pacientes importados na lista
3. Abra alguns cadastros para verificar que os dados estão corretos
4. Se encontrar algum problema, você pode:
   - Editar manualmente os registros
   - Ou reverter a importação (veja seção **Reverter Importação**)

## 🔄 Cenários Avançados

### Importar Histórico de Agendamentos

Além dos pacientes, você pode importar agendamentos passados:

**Arquivo de Agendamentos (CSV):**

```csv
CPFPaciente,DataConsulta,HoraConsulta,NomeMedico,Tipo,Status
123.456.789-00,15/01/2024,14:30,Dr. Carlos Lima,Consulta,Realizada
123.456.789-00,20/02/2024,10:00,Dr. Carlos Lima,Retorno,Realizada
987.654.321-00,10/01/2024,09:00,Dra. Ana Costa,Consulta,Realizada
```

**Processo:**
1. Primeiro, importe os **Pacientes**
2. Depois, importe os **Agendamentos** (o sistema fará o matching por CPF)

### Importar com Sincronização Periódica

Se você ainda usa o sistema antigo temporariamente, pode configurar importação automática:

1. **Configurações** → **Importação de Dados** → **Agendamentos**
2. Clique em **Configurar Sincronização Automática**
3. Escolha a frequência:
   - ⭕ Diária (todo dia às 00:00)
   - ⭕ Semanal (toda segunda às 00:00)
   - ⭕ Mensal (primeiro dia do mês às 00:00)
4. Configure as credenciais de acesso ao sistema antigo
5. Salve

> ⚠️ **Nota:** Requer integração via API com o sistema de origem. Nem todos os sistemas suportam.

### Reverter uma Importação

Se você importou dados incorretos, pode reverter:

1. **Importação de Dados** → **Histórico**
2. Clique na importação que deseja reverter
3. Clique em **⚠️ Reverter Importação**
4. Confirme a ação digitando: **REVERTER**
5. Clique em **Confirmar**

> ⚠️ **ATENÇÃO:** 
> - Todos os registros importados serão **deletados permanentemente**
> - Registros que foram editados após a importação **não** serão revertidos
> - Esta ação **não pode ser desfeita**
> - Você tem até **7 dias** após a importação para reverter

## 🆘 Solução de Problemas

### Problemas Comuns

#### ❌ Upload falha com erro "Arquivo muito grande"

**Solução:**
- Divida o arquivo em partes menores (máximo 10.000 registros por arquivo)
- Ou comprima o arquivo em ZIP

#### ❌ "CPF inválido" para vários registros

**Solução:**
- Verifique se os CPFs estão no formato correto
- Remova pontos e hífens se necessário
- Use a ferramenta online: [Validador de CPF](https://www.4devs.com.br/validador_cpf)

#### ❌ "Data de nascimento inválida"

**Solução:**
- Use formato DD/MM/AAAA ou AAAA-MM-DD
- Verifique se não há datas futuras
- Certifique-se de que a idade do paciente é realista (0-120 anos)

#### ⚠️ "Codificação de caracteres incorreta"

**Solução:**
- Salve o CSV com codificação **UTF-8**
- No Excel: **Salvar Como** → **CSV UTF-8 (delimitado por vírgulas)**

#### ❌ Importação trava em 50%

**Solução:**
- Aguarde mais tempo (arquivos grandes podem demorar)
- Se travar por mais de 30 minutos, entre em contato com o suporte
- Não recarregue a página, isso pode cancelar a importação

### Logs de Erro

Para ver logs detalhados de erro:

1. **Importação de Dados** → **Histórico**
2. Clique na importação com erro
3. Clique em **Ver Logs Detalhados**
4. Você pode copiar os logs e enviar para o suporte

## 📞 Suporte

Se você precisar de ajuda:

**📧 Email:** suporte@omnicaresoftware.com.br  
**📱 WhatsApp:** (11) 9999-9999  
**💬 Chat:** Disponível no sistema (canto inferior direito)  
**📖 Central de Ajuda:** https://ajuda.omnicaresoftware.com.br

**Horário de Atendimento:**
- Segunda a Sexta: 8h às 18h
- Sábado: 8h às 12h
- Domingo e Feriados: Apenas email (resposta em até 24h úteis)

## ✅ Checklist de Importação

Antes de começar, certifique-se de:

- [ ] Exportei os dados do sistema atual
- [ ] Arquivo está em formato compatível (CSV, Excel, JSON ou XML)
- [ ] Revisei os dados para garantir qualidade
- [ ] CPFs estão válidos e sem duplicatas
- [ ] Datas estão no formato correto
- [ ] Telefones e emails estão válidos
- [ ] Fiz backup dos dados originais
- [ ] Li este guia completamente
- [ ] Tenho permissão de administrador no Omni Care

Durante a importação:

- [ ] Mapeei corretamente todas as colunas
- [ ] Configurei transformações de valores quando necessário
- [ ] Revisei o relatório de validação
- [ ] Corrigi todos os erros críticos
- [ ] Visualizei o preview dos dados
- [ ] Selecionei o comportamento para duplicatas
- [ ] Confirmei que revisei os dados

Após a importação:

- [ ] Revisei o relatório final
- [ ] Verifiquei alguns cadastros aleatoriamente
- [ ] Baixei o relatório detalhado (PDF)
- [ ] Arquivei o arquivo original e os relatórios

## 🎓 Melhores Práticas

### Antes de Importar

1. **Faça um teste com poucos registros primeiro**
   - Importe 10-20 pacientes inicialmente
   - Verifique se tudo está correto
   - Depois importe o restante

2. **Limpe os dados antes de importar**
   - Remova duplicatas
   - Corrija erros de digitação
   - Padronize formatos

3. **Documente seu mapeamento**
   - Salve o template de mapeamento
   - Anote transformações especiais
   - Isso facilita futuras importações

### Durante a Importação

1. **Não feche o navegador** durante o upload
2. **Revise cuidadosamente** o mapeamento de colunas
3. **Leia todos os avisos** antes de continuar

### Após a Importação

1. **Verifique a qualidade** dos dados importados
2. **Treine sua equipe** nos novos cadastros
3. **Arquive os relatórios** para auditoria

## 📚 Apêndice

### Glossário

- **Mapeamento:** Processo de conectar colunas do arquivo aos campos do sistema
- **Validação:** Verificação automática da qualidade dos dados
- **Transformação:** Conversão de valores (ex: "M" → "Masculino")
- **Template:** Configuração salva de mapeamento
- **Duplicata:** Registro que já existe no sistema (mesmo CPF)
- **Rollback:** Reverter uma importação

### Atalhos de Teclado

- `Ctrl + U` - Upload de arquivo
- `Ctrl + S` - Salvar template de mapeamento
- `Ctrl + Enter` - Executar importação
- `Esc` - Cancelar ação atual

### Formatos de Data Aceitos

- DD/MM/AAAA (ex: 15/03/1985)
- AAAA-MM-DD (ex: 1985-03-15)
- DD-MM-AAAA (ex: 15-03-1985)
- MM/DD/AAAA (ex: 03/15/1985) - Apenas se claramente especificado

### Formatos de Telefone Aceitos

- (11) 98765-4321
- 11 98765-4321
- (11) 3456-7890
- 11987654321
- +55 11 98765-4321

---

> **Versão:** 1.0  
> **Data:** 29 de Janeiro de 2026  
> **Elaborado por:** GitHub Copilot  
> **Última Revisão:** 29 de Janeiro de 2026
