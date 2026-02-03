# 🏥 Guia do Usuário - Clínica

## Bem-vindo à Configuração de Módulos

Aprenda a gerenciar os módulos disponíveis para sua clínica de forma simples e eficiente.

---

## 🎯 O que são Módulos?

Módulos são **funcionalidades** do sistema Omni Care que você pode habilitar ou desabilitar conforme a necessidade da sua clínica.

### Benefícios

✨ **Personalize o sistema**
- Ative apenas as funcionalidades que você usa
- Interface mais limpa e focada
- Reduz curva de aprendizado da equipe

🎯 **Foco nas funcionalidades que você usa**
- Evite distrações com recursos não utilizados
- Menu mais organizado
- Navegação mais rápida

💰 **Otimize custos**
- Escolha o plano ideal para suas necessidades
- Pague apenas pelos recursos que usa
- Faça upgrade quando precisar crescer

### Como Funciona?

1. Cada módulo representa uma funcionalidade (ex: Gestão de Pacientes, WhatsApp)
2. Módulos podem estar disponíveis ou não, dependendo do seu plano
3. Você pode habilitar/desabilitar módulos disponíveis no seu plano
4. Módulos essenciais não podem ser desabilitados

---

## 📱 Acessar Módulos

### Passo a Passo

1. Faça **login** na área administrativa do Omni Care
2. No menu lateral, clique em **"Configurações"**
3. Selecione a opção **"Módulos"**
4. Você verá todos os módulos disponíveis para sua clínica

![Tela de Módulos](./screenshots/clinic-modules.png)

### Navegação

**Menu da Tela:**
- 🏠 Voltar para Dashboard
- 📋 Lista de todos os módulos
- 🔍 Busca rápida
- 🏷️ Filtros por categoria
- 📊 Status do seu plano

---

## ⚙️ Habilitar/Desabilitar Módulos

### Habilitar um Módulo

#### Passo a Passo

1. Localize o módulo que deseja habilitar
2. Clique no **toggle** (interruptor) do módulo
3. O toggle ficará verde/azul (ativo)
4. Aguarde a mensagem de confirmação: **"Módulo habilitado com sucesso!"**
5. Pronto! O módulo está ativo ✅

#### O que Acontece?

- O módulo aparece no menu principal
- Funcionalidades ficam disponíveis imediatamente
- Equipe pode começar a usar
- Mudança é registrada no histórico

**Exemplo:**
```
Antes:
Menu:
├── Dashboard
├── Pacientes
├── Agendamentos
└── Configurações

Depois de habilitar "Fila de Espera":
Menu:
├── Dashboard
├── Pacientes
├── Agendamentos
├── Fila de Espera ← Novo!
└── Configurações
```

### Desabilitar um Módulo

#### Passo a Passo

1. Localize o módulo habilitado (toggle ativo)
2. Clique no **toggle** para desligar
3. Uma janela de confirmação aparecerá
4. Leia o aviso sobre impactos
5. Clique em **"Confirmar"**
6. O toggle ficará cinza (inativo)
7. Aguarde a mensagem: **"Módulo desabilitado com sucesso!"**
8. O módulo está desabilitado 🚫

#### Avisos Importantes

⚠️ **Atenção ao desabilitar:**
- Menu item será removido
- Funcionalidades ficarão indisponíveis
- Dados não serão perdidos (podem ser acessados novamente ao reabilitar)
- Usuários não poderão acessar as features do módulo

**Exemplo de Confirmação:**
```
⚠️ Desabilitar Módulo?

Você está prestes a desabilitar "Gestão Financeira".

Impacto:
- Menu "Financeiro" será removido
- Relatórios financeiros ficarão indisponíveis
- Lançamentos existentes serão preservados

Tem certeza?

[Cancelar] [Confirmar]
```

### Módulos que Não Podem Ser Desabilitados

🌟 **Módulos Essenciais (Core):**

Alguns módulos são fundamentais e não podem ser desabilitados:

- **Gestão de Pacientes** - Cadastro e gestão de pacientes
- **Agendamento** - Agenda de consultas
- **Prontuários** - Prontuários médicos
- **Prescrições** - Prescrição de medicamentos
- **Gestão de Usuários** - Controle de acesso

**Por quê?**
- São essenciais para o funcionamento básico
- Outros módulos dependem deles
- Removê-los quebraria o sistema

**Como Identificar:**
- Toggle aparece desabilitado
- Badge "ESSENCIAL" no card
- Mensagem ao passar o mouse: "Módulo essencial não pode ser desabilitado"

---

## 🎨 Categorias de Módulos

### 🌟 Essenciais (Core)

Módulos básicos que todo sistema precisa.

| Módulo | Descrição | Status |
|--------|-----------|--------|
| Gestão de Pacientes | Cadastro e histórico de pacientes | 🔒 Sempre ativo |
| Agendamento | Agenda de consultas e procedimentos | 🔒 Sempre ativo |
| Prontuários | Prontuários médicos eletrônicos | 🔒 Sempre ativo |
| Prescrições | Prescrição e controle de medicamentos | 🔒 Sempre ativo |
| Gestão de Usuários | Controle de acesso da equipe | 🔒 Sempre ativo |

**Características:**
- ✅ Incluídos em todos os planos
- ✅ Não podem ser desabilitados
- ✅ Sem custo adicional

### 🔧 Avançados (Advanced)

Funcionalidades extras para operação otimizada.

| Módulo | Descrição | Disponível em |
|--------|-----------|---------------|
| Gestão Financeira | Controle de receitas, despesas e faturamento | Standard+ |
| Fila de Espera | Gestão de fila digital para atendimento | Standard+ |
| Gestão de Estoque | Controle de materiais e medicamentos | Premium+ |
| Config. Campos Médico | Personalização de campos do prontuário | Standard+ |

**Características:**
- ⚙️ Otimizam operação do dia-a-dia
- ⚙️ Podem ser habilitados/desabilitados
- ⚙️ Incluídos em planos Standard ou superiores

### 💎 Premium

Recursos premium para diferenciação competitiva.

| Módulo | Descrição | Disponível em |
|--------|-----------|---------------|
| Integração WhatsApp | Envio de lembretes e confirmações via WhatsApp | Premium+ |
| Notificações SMS | Envio de SMS para pacientes | Premium+ |
| Exportação TISS | Exportação de guias no padrão TISS | Standard+ |

**Características:**
- 💎 Melhoram experiência do paciente
- 💎 Requerem plano Premium ou Enterprise
- 💎 Podem ter custos adicionais (ex: SMS)

### 📊 Analytics

Análises e relatórios para tomada de decisão.

| Módulo | Descrição | Disponível em |
|--------|-----------|---------------|
| Relatórios Avançados | Dashboards e relatórios customizados | Standard+ |

**Características:**
- 📊 Fornecem insights de negócio
- 📊 Suportam múltiplos formatos de exportação
- 📊 Incluídos em planos Standard ou superiores

---

## 🔧 Configurações Avançadas

Alguns módulos permitem configurações detalhadas para personalizar seu funcionamento.

### Acessar Configurações

1. Localize o módulo que deseja configurar
2. Clique no botão **"Configurar"** (ícone de engrenagem ⚙️)
3. Uma janela de configurações abrirá
4. Ajuste os parâmetros conforme necessário
5. Clique em **"Salvar"**
6. Aguarde confirmação

![Configurações Avançadas](./screenshots/module-config-dialog.png)

### Exemplos de Configurações

#### WhatsApp Integration

```json
{
  "enviarLembretes": true,
  "horasAntecedencia": 24,
  "mensagemPadrao": "Olá {paciente}, lembre-se da sua consulta amanhã às {hora} com Dr(a). {medico}.",
  "enviarConfirmacao": true,
  "permitirCancelamento": false
}
```

**Parâmetros:**
- `enviarLembretes`: Envia lembrete automático (sim/não)
- `horasAntecedencia`: Quantas horas antes enviar (número)
- `mensagemPadrao`: Template da mensagem (texto)
- `enviarConfirmacao`: Solicita confirmação do paciente (sim/não)
- `permitirCancelamento`: Permite cancelar via WhatsApp (sim/não)

#### SMS Notifications

```json
{
  "ativo": true,
  "eventos": ["lembrete_consulta", "confirmacao_agendamento", "resultado_exame"],
  "remetente": "Omni Care"
}
```

**Parâmetros:**
- `ativo`: Ativa/desativa envio de SMS (sim/não)
- `eventos`: Quais eventos disparam SMS (lista)
- `remetente`: Nome exibido como remetente (texto)

#### Reports (Relatórios)

```json
{
  "exportarPDF": true,
  "exportarExcel": true,
  "maxLinhasExportacao": 10000,
  "relatoriosAgendados": ["resumo_semanal", "faturamento_mensal"]
}
```

**Parâmetros:**
- `exportarPDF`: Permite exportar em PDF (sim/não)
- `exportarExcel`: Permite exportar em Excel (sim/não)
- `maxLinhasExportacao`: Limite de linhas (número)
- `relatoriosAgendados`: Relatórios automáticos (lista)

### Dicas para Configurações

✅ **Comece com configurações padrão**
- Sistema vem pré-configurado
- Funciona bem para maioria dos casos
- Ajuste conforme necessidade

✅ **Teste em pequena escala**
- Configure para um médico primeiro
- Valide o funcionamento
- Depois aplique para toda clínica

✅ **Documente suas configurações**
- Anote configurações customizadas
- Facilita replicação
- Útil para troubleshooting

✅ **Revise periodicamente**
- Configurações podem ficar desatualizadas
- Necessidades mudam com o tempo
- Otimize conforme feedback da equipe

---

## 🚀 Fazer Upgrade de Plano

Viu um módulo com **"UPGRADE NECESSÁRIO"** ou **"DISPONÍVEL NO PLANO PREMIUM"**?

### O que Significa?

Esse módulo está disponível apenas em planos superiores ao seu atual.

**Exemplo:**
- Você tem plano **Standard**
- Módulo **WhatsApp** requer plano **Premium**
- Para usar, precisa fazer upgrade

### Como Fazer Upgrade

#### Passo 1: Ver Planos Disponíveis

1. Clique no botão **"Fazer Upgrade"** no módulo
2. Você será direcionado para a página de planos
3. Compare os recursos de cada plano

#### Passo 2: Comparar Planos

| Recurso | Basic | Standard | Premium | Enterprise |
|---------|-------|----------|---------|------------|
| **Preço** | R$ 99/mês | R$ 199/mês | R$ 299/mês | Sob consulta |
| **Usuários** | 3 | 10 | 25 | Ilimitado |
| **Pacientes** | 500 | 2.000 | 10.000 | Ilimitado |
| **Módulos Core** | ✅ | ✅ | ✅ | ✅ |
| **Relatórios** | ❌ | ✅ | ✅ | ✅ |
| **TISS** | ❌ | ✅ | ✅ | ✅ |
| **WhatsApp** | ❌ | ❌ | ✅ | ✅ |
| **SMS** | ❌ | ❌ | ✅ | ✅ |
| **Estoque** | ❌ | ❌ | ✅ | ✅ |
| **Suporte** | Email | Email + Chat | Prioritário | Dedicado |

#### Passo 3: Solicitar Upgrade

1. Escolha o plano ideal
2. Clique em **"Solicitar Upgrade"**
3. Preencha o formulário de solicitação
4. Aguarde contato da equipe comercial

**Ou:**

📞 Entre em contato diretamente:
- Email: comercial@omnicare.com.br
- Telefone: (11) 1234-5678
- WhatsApp: (11) 98765-4321

#### Passo 4: Ativação

1. Equipe comercial entrará em contato
2. Detalhes sobre precificação e contrato
3. Assinatura do contrato (física ou digital)
4. Pagamento da primeira mensalidade
5. Upgrade ativado em até 24h

### Benefícios do Upgrade

**De Basic para Standard:**
- ➕ Relatórios Avançados
- ➕ Exportação TISS
- ➕ 7 usuários adicionais
- ➕ 1.500 pacientes adicionais
- ➕ Suporte via chat

**De Standard para Premium:**
- ➕ Integração WhatsApp
- ➕ Notificações SMS
- ➕ Gestão de Estoque
- ➕ 15 usuários adicionais
- ➕ 8.000 pacientes adicionais
- ➕ Suporte prioritário

**Para Enterprise:**
- ➕ Todos os módulos
- ➕ Usuários ilimitados
- ➕ Pacientes ilimitados
- ➕ Customizações sob medida
- ➕ Suporte dedicado 24/7
- ➕ SLA garantido

---

## ⚠️ Restrições e Dependências

### Dependências entre Módulos

Alguns módulos precisam de outros para funcionar.

#### Exemplo 1: Fila de Espera

**Depende de:** Agendamento

**Por quê?**
- Fila precisa saber quais consultas foram agendadas
- Integração com horários disponíveis
- Notificação de chegada de paciente

**O que acontece?**
- Se tentar habilitar Fila sem Agendamento, verá erro
- Mensagem: "Para habilitar Fila de Espera, o módulo Agendamento deve estar ativo"

#### Exemplo 2: SMS Notifications

**Depende de:** Gestão de Pacientes

**Por quê?**
- Precisa de telefone dos pacientes
- Acesso ao cadastro para personalização

**O que acontece?**
- SMS depende de dados do paciente
- Sem Gestão de Pacientes, não há dados para enviar

### Tabela de Dependências

| Módulo | Depende de |
|--------|------------|
| Fila de Espera | Agendamento |
| SMS Notifications | Gestão de Pacientes |
| WhatsApp Integration | Gestão de Pacientes |
| Relatórios | Prontuários, Agendamento |
| Exportação TISS | Prontuários |

### Limites do Plano

Cada plano tem limites específicos:

#### Limite de Usuários

**O que é:** Número máximo de usuários (médicos, recepcionistas, etc.) que podem acessar o sistema.

**Por plano:**
- Basic: 3 usuários
- Standard: 10 usuários
- Premium: 25 usuários
- Enterprise: Ilimitado

**O que acontece ao atingir:**
- Não consegue adicionar novo usuário
- Mensagem: "Limite de usuários atingido. Faça upgrade do plano."

**Solução:**
- Desativar usuários inativos
- Fazer upgrade de plano

#### Limite de Pacientes

**O que é:** Número máximo de pacientes cadastrados no sistema.

**Por plano:**
- Basic: 500 pacientes
- Standard: 2.000 pacientes
- Premium: 10.000 pacientes
- Enterprise: Ilimitado

**O que acontece ao atingir:**
- Não consegue cadastrar novo paciente
- Mensagem: "Limite de pacientes atingido. Faça upgrade do plano."

**Solução:**
- Arquivar pacientes inativos (mantém dados, libera cota)
- Fazer upgrade de plano

#### Limite de Módulos

**O que é:** Quais módulos estão disponíveis no seu plano.

**Verificar seu plano:**
1. Menu → Configurações → Assinatura
2. Ver seção "Módulos Incluídos"

**Exemplo - Plano Standard:**
```
✅ Módulos Incluídos:
- Todos Core (Pacientes, Agendamento, Prontuários, Prescrições, Usuários)
- Gestão Financeira
- Fila de Espera
- Config. Campos Médico
- Relatórios
- TISS

❌ Módulos Não Incluídos:
- WhatsApp (requer Premium)
- SMS (requer Premium)
- Estoque (requer Premium)
```

### Verificar Limites

**Painel de Uso:**

Menu → Configurações → Assinatura → Uso Atual

```
📊 Uso do Plano Standard

Usuários: 7 / 10 (70%)
├─■■■■■■■□□□

Pacientes: 1.523 / 2.000 (76%)
├─■■■■■■■■□□

Módulos: 8 / 11 habilitados
├─■■■■■■■■□□□

Status: ✅ Dentro dos limites
```

---

## 💡 Dicas e Melhores Práticas

### Para Iniciantes

✅ **Habilite apenas o que você usa**
- Evita sobrecarga de informação
- Interface mais limpa
- Equipe aprende mais rápido

**Exemplo de configuração inicial:**
```
✅ Habilitar:
- Gestão de Pacientes (Core - já ativo)
- Agendamento (Core - já ativo)
- Prontuários (Core - já ativo)
- Fila de Espera (útil para organização)

❌ Deixar desabilitado inicialmente:
- Gestão Financeira (habilitar quando dominar o básico)
- Relatórios (habilitar após alguns meses de uso)
- Estoque (habilitar quando necessário)
```

### Para Uso Diário

✅ **Teste novos módulos gradualmente**
- Habilite um módulo por vez
- Treine a equipe antes de habilitar
- Valide funcionamento antes de próximo módulo

**Passo a passo:**
1. **Semana 1:** Habilitar Fila de Espera + treinar recepção
2. **Semana 3:** Habilitar Gestão Financeira + treinar administrativo
3. **Semana 5:** Habilitar Relatórios + treinar gerência
4. **Semana 7:** Habilitar WhatsApp (se Premium) + treinar todos

### Gestão Eficiente

✅ **Revise módulos periodicamente**
- Mensalmente, verifique o que está usando
- Desabilite módulos não utilizados
- Explore novos módulos disponíveis

**Checklist mensal:**
```
□ Verificar módulos habilitados
□ Identificar módulos não usados no último mês
□ Avaliar se vale a pena manter habilitado
□ Verificar novos módulos disponíveis no plano
□ Ler sobre atualizações de módulos existentes
```

✅ **Mantenha backup das configurações**
- Anote configurações personalizadas
- Tire prints de configurações importantes
- Facilita restauração se necessário

**O que documentar:**
```
Módulo: WhatsApp Integration
Configuração Atual:
- Lembrete: 24h antes
- Mensagem: [template personalizado]
- Confirmação: Sim
- Cancelamento: Não

Data da configuração: 15/01/2026
Responsável: Dr. João
```

✅ **Treine sua equipe**
- Novos módulos requerem adaptação
- Crie materiais de treinamento simples
- Tire dúvidas proativamente

**Dica:** Grave vídeos curtos mostrando como usar cada módulo

---

## 🆘 Problemas Comuns

### "Não consigo habilitar um módulo"

#### Causa 1: Módulo não disponível no seu plano

**Sintoma:** Botão "Upgrade Necessário" aparece no módulo

**Solução:**
1. Verificar seu plano atual: Menu → Configurações → Assinatura
2. Ver quais módulos estão incluídos
3. Se necessário, solicitar upgrade

**Exemplo:**
```
Seu plano: Standard
Módulo desejado: WhatsApp
Plano necessário: Premium

Ação: Fazer upgrade para Premium
```

#### Causa 2: Dependência não satisfeita

**Sintoma:** Mensagem "Módulo X precisa estar habilitado primeiro"

**Solução:**
1. Ler a mensagem de erro (indica qual dependência)
2. Habilitar o módulo dependente primeiro
3. Tentar habilitar o módulo desejado novamente

**Exemplo:**
```
Erro ao habilitar "Fila de Espera"
Mensagem: "Módulo Agendamento deve estar habilitado"

Solução:
1. Habilitar Agendamento
2. Tentar habilitar Fila de Espera novamente
```

#### Causa 3: Limite atingido

**Sintoma:** Mensagem "Limite de recursos do plano atingido"

**Solução:**
1. Verificar uso: Menu → Configurações → Assinatura → Uso
2. Identificar qual limite foi atingido
3. Desabilitar módulos não usados ou fazer upgrade

**Exemplo:**
```
Limite atingido: 10/10 módulos Advanced habilitados

Solução 1: Desabilitar um módulo não usado
Solução 2: Fazer upgrade para plano com mais módulos
```

### "Módulo habilitado não aparece no menu"

#### Solução 1: Recarregar página

1. Pressione **Ctrl + R** (Windows/Linux) ou **Cmd + R** (Mac)
2. Ou clique no botão de recarregar do navegador
3. Faça login novamente se necessário

#### Solução 2: Limpar cache

1. Pressione **Ctrl + Shift + Delete** (Windows/Linux)
2. Ou **Cmd + Shift + Delete** (Mac)
3. Selecione "Cache" e "Cookies"
4. Clique em "Limpar dados"
5. Feche e abra o navegador
6. Faça login novamente

#### Solução 3: Aguardar sincronização

- Mudanças podem levar alguns minutos para propagar
- Aguarde 5-10 minutos
- Tente fazer logout e login novamente

Se problema persistir após 30 minutos:
- Entre em contato com suporte
- Informe qual módulo foi habilitado
- Informe há quanto tempo habilitou

### "Configurações não salvam"

#### Causa 1: Formato JSON inválido

**Sintoma:** Mensagem "Formato inválido" ou "Erro de sintaxe"

**Solução:**
- Verificar se JSON está correto
- Usar validador online: https://jsonlint.com
- Corrigir erros de sintaxe

**Erros comuns:**
```json
❌ Errado:
{
  "enviarLembretes": true,  ← Vírgula extra
}

✅ Correto:
{
  "enviarLembretes": true
}
```

```json
❌ Errado:
{
  "horas": "24"  ← Número como string
}

✅ Correto:
{
  "horas": 24
}
```

#### Causa 2: Sem permissão

**Sintoma:** Mensagem "Você não tem permissão para alterar configurações"

**Solução:**
- Verificar se você é administrador da clínica
- Solicitar permissões ao administrador
- Login como usuário com permissão adequada

#### Causa 3: Conexão perdida

**Sintoma:** Mensagem "Erro de conexão" ou timeout

**Solução:**
1. Verificar sua conexão com internet
2. Tentar novamente após alguns segundos
3. Se persistir, verificar status do sistema: https://status.omnicare.com.br

### "Módulo foi desabilitado sozinho"

#### Causa Possível: Ação administrativa

**O que aconteceu:**
- Administrador do sistema pode ter desabilitado globalmente
- Mudança no plano de assinatura
- Manutenção programada

**O que fazer:**
1. Verificar email (pode ter recebido notificação)
2. Contatar suporte para entender o motivo
3. Verificar se pode reabilitar

**Prevenção:**
- Mantenha contatos atualizados
- Leia notificações do sistema
- Mantenha pagamentos em dia

---

## 📞 Precisa de Ajuda?

### Suporte Técnico

**Canais de Atendimento:**
- 📧 Email: suporte@omnicare.com.br
- 📱 WhatsApp: (11) 98765-4321
- 💬 Chat: Clique no ícone de chat no canto inferior direito
- 📚 Base de Conhecimento: https://ajuda.omnicare.com.br

**Horário de Atendimento:**
- **Segunda a Sexta:** 8h às 18h
- **Sábado:** 8h às 12h
- **Emergências:** 24/7 (apenas casos críticos)

### Ao Entrar em Contato

Para atendimento mais rápido, tenha em mãos:

✅ **Informações básicas:**
- Nome da clínica
- Email de cadastro
- Plano contratado

✅ **Descrição do problema:**
- Qual módulo está relacionado
- O que você estava tentando fazer
- Mensagem de erro (se houver)
- Prints da tela (se possível)

**Exemplo de solicitação bem feita:**
```
Assunto: Não consigo habilitar módulo WhatsApp

Clínica: Clínica São Paulo
Email: contato@clinicasp.com.br
Plano: Premium

Problema:
Estou tentando habilitar o módulo "WhatsApp Integration",
mas quando clico no toggle, aparece a mensagem:
"Erro ao habilitar módulo".

Já tentei:
- Recarregar a página
- Limpar cache do navegador
- Fazer logout e login

Anexo: print da tela com erro

Aguardo retorno.
```

### SLA (Tempo de Resposta)

| Prioridade | Primeira Resposta | Resolução |
|------------|------------------|-----------|
| **Emergência** | 30 minutos | 4 horas |
| **Alta** | 2 horas | 8 horas |
| **Média** | 8 horas | 24 horas |
| **Baixa** | 24 horas | 72 horas |

**Emergência:**
- Sistema completamente fora do ar
- Perda de dados

**Alta:**
- Módulo crítico não funciona
- Impede operação da clínica

**Média:**
- Módulo opcional não funciona
- Dúvida sobre configuração

**Baixa:**
- Dúvida sobre uso
- Solicitação de melhoria

---

## 📺 Vídeo Tutoriais

🎥 **Aprenda em Vídeo:**

1. **[Introdução aos Módulos](https://youtube.com/...)** - 3 min
   - O que são módulos
   - Benefícios
   - Como acessar

2. **[Habilitar/Desabilitar Módulos](https://youtube.com/...)** - 5 min
   - Passo a passo prático
   - Dicas de uso
   - Erros comuns

3. **[Configurações Avançadas](https://youtube.com/...)** - 4 min
   - Como configurar módulos
   - Exemplos práticos
   - JSON básico

4. **[Upgrade de Plano](https://youtube.com/...)** - 3 min
   - Quando fazer upgrade
   - Como solicitar
   - Benefícios

5. **[Módulos Premium em Ação](https://youtube.com/...)** - 8 min
   - WhatsApp na prática
   - SMS em ação
   - TISS passo a passo

---

## 📋 Checklist de Configuração

### Primeiros Passos (Primeira Semana)

- [ ] Acessar tela de módulos
- [ ] Revisar módulos disponíveis no seu plano
- [ ] Identificar módulos essenciais para sua clínica
- [ ] Habilitar módulos prioritários
- [ ] Testar funcionalidades básicas

### Segundo Momento (Segundo Mês)

- [ ] Avaliar uso dos módulos habilitados
- [ ] Desabilitar módulos não utilizados
- [ ] Explorar novos módulos disponíveis
- [ ] Configurar parâmetros avançados
- [ ] Treinar equipe nos novos módulos

### Rotina (Mensal)

- [ ] Revisar módulos habilitados
- [ ] Verificar uso de cada módulo
- [ ] Atualizar configurações se necessário
- [ ] Explorar atualizações de módulos
- [ ] Coletar feedback da equipe

### Evolução (Trimestral)

- [ ] Avaliar necessidade de upgrade de plano
- [ ] Identificar módulos desejados não disponíveis
- [ ] Calcular ROI de upgrade
- [ ] Planejar expansão de funcionalidades
- [ ] Revisar estratégia de uso do sistema

---

## 🌟 Casos de Sucesso

### Clínica Odontológica - São Paulo

**Contexto:**
- Clínica com 5 dentistas
- Plano: Standard
- 800 pacientes cadastrados

**Módulos Habilitados:**
- Core (todos)
- Fila de Espera
- Gestão Financeira
- Relatórios
- SMS Notifications

**Resultado:**
- 📊 Redução de 40% no tempo de espera
- 💰 Aumento de 25% no faturamento (melhor controle financeiro)
- 😊 NPS de 85 (satisfação do paciente)

**Depoimento:**
> "A Fila de Espera revolucionou nossa recepção. Antes, era caos. 
> Agora, tudo organizado e os pacientes adoram!"
> 
> *- Dra. Maria, proprietária*

### Clínica Multiespecialidade - Rio de Janeiro

**Contexto:**
- Clínica com 20 médicos
- Plano: Premium
- 4.500 pacientes cadastrados

**Módulos Habilitados:**
- Core (todos)
- Todos Advanced
- WhatsApp Integration
- SMS Notifications
- Relatórios

**Resultado:**
- 📱 90% dos lembretes via WhatsApp (redução de custo com ligações)
- 📉 Redução de 60% em faltas sem aviso
- ⭐ Aumento de 35% em avaliações positivas online

**Depoimento:**
> "O WhatsApp foi game changer. Pacientes confirmam pelo app, 
> reduzimos faltas drasticamente!"
> 
> *- Dr. Carlos, diretor clínico*

---

## 📖 Glossário

**Core (Essencial):** Módulos fundamentais do sistema que não podem ser desabilitados.

**Toggle:** Interruptor virtual para habilitar/desabilitar módulos.

**JSON:** Formato de texto para configurações avançadas (JavaScript Object Notation).

**Dependência:** Quando um módulo precisa de outro para funcionar corretamente.

**Upgrade:** Migração para plano superior com mais recursos.

**Downgrade:** Migração para plano inferior (menos recursos).

**SLA:** Service Level Agreement - Acordo de nível de serviço (tempo de atendimento).

**NPS:** Net Promoter Score - Métrica de satisfação do cliente.

**Cache:** Memória temporária do navegador (pode guardar informações antigas).

---

*Última atualização: 29 de Janeiro de 2026*

**Versão:** 1.0  
**Autor:** Omni Care Software - Customer Success Team

---

**Dúvidas?** Acesse nossa [Central de Ajuda](https://ajuda.omnicare.com.br) ou [abra um ticket](https://suporte.omnicare.com.br)
