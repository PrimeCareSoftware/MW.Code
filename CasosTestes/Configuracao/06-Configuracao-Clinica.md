# 06 - Configuração da Clínica

> **Objetivo:** Configurar completamente uma clínica desde o registro até estar pronta para operação  
> **Tempo estimado:** 20-30 minutos  
> **Pré-requisitos:** Nenhum (este é o ponto de partida)

## 📋 Índice

1. [Registro e Criação da Clínica](#1-registro-e-criação-da-clínica)
2. [Primeiro Acesso do Proprietário](#2-primeiro-acesso-do-proprietário)
3. [Configuração de Negócio](#3-configuração-de-negócio)
4. [Personalização Visual](#4-personalização-visual)
5. [Configuração de Módulos](#5-configuração-de-módulos)
6. [Informações Básicas da Clínica](#6-informações-básicas-da-clínica)
7. [Criação de Usuários](#7-criação-de-usuários)
8. [Verificação Final](#8-verificação-final)

---

## 1. Registro e Criação da Clínica

### 1.1. Acesso ao Site de Registro

**Passos:**
1. Acesse o site principal em:
   - **Desenvolvimento:** `http://localhost:5000`
   - **Produção:** `https://primecare.com.br`

2. Clique em **"Cadastre-se"** ou **"Começar Teste Grátis"**

### 1.2. Preencher Formulário de Registro

O formulário é dividido em 6 etapas:

#### **Etapa 1: Dados da Clínica**
```
✅ Nome da Clínica: "Clínica Saúde Total"
✅ CNPJ/CPF: "12.345.678/0001-90"
✅ Telefone: "(11) 98765-4321"
✅ Email: "contato@saudetotal.com.br"
```

**Verificações Automáticas:**
- Sistema valida se CNPJ já está cadastrado
- Email deve ser válido e único

#### **Etapa 2: Endereço da Clínica**
```
✅ CEP: "01310-100"
✅ Rua: "Av. Paulista"
✅ Número: "1578"
✅ Complemento: "Sala 203" (opcional)
✅ Bairro: "Bela Vista"
✅ Cidade: "São Paulo"
✅ Estado: "SP"
```

**Dica:** Ao preencher o CEP, os campos de endereço são preenchidos automaticamente.

#### **Etapa 3: Dados do Proprietário**
```
✅ Nome Completo: "Dr. João Silva"
✅ CPF: "123.456.789-00"
✅ Telefone: "(11) 99999-8888"
✅ Email: "joao.silva@saudetotal.com.br"
```

**Importante:** Este será o primeiro usuário com perfil de Proprietário (Owner).

#### **Etapa 4: Credenciais de Acesso**
```
✅ Nome de Usuário: "joao.silva"
✅ Senha: "SenhaForte@123"
✅ Confirmar Senha: "SenhaForte@123"
```

**Requisitos da Senha:**
- Mínimo 8 caracteres
- Pelo menos 1 letra maiúscula
- Pelo menos 1 letra minúscula
- Pelo menos 1 número
- Pelo menos 1 caractere especial (@, #, $, !, etc.)

#### **Etapa 5: Escolha do Plano**
```
Opções disponíveis:
✅ Básico - R$ 97/mês
✅ Profissional - R$ 197/mês
✅ Premium - R$ 397/mês
✅ Enterprise - R$ 697/mês
```

Escolha o plano de acordo com suas necessidades. Todos os planos incluem **14 dias de teste grátis**.

#### **Etapa 6: Confirmação e Termos**
```
✅ Li e aceito os Termos de Uso
✅ Li e aceito a Política de Privacidade
✅ Aceito receber comunicações por email
```

### 1.3. Confirmação do Registro

Após completar o registro, você verá uma tela de confirmação com:

```
🎉 Clínica Cadastrada com Sucesso!

Informações importantes:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Tenant ID: abc123-def456-ghi789
Nome de Usuário: joao.silva
Nome da Clínica: Clínica Saúde Total
Subdomínio: saudetotal.primecare.com.br
Período de Teste: 14 dias
Data de Vencimento: 15/02/2026
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

⚠️ IMPORTANTE: Anote estas informações!
```

**⚠️ GUARDE ESTAS INFORMAÇÕES EM LOCAL SEGURO:**
- **Tenant ID:** Identificador único da sua clínica
- **Nome de Usuário:** Será usado para fazer login
- **Subdomínio:** URL personalizada da sua clínica

---

## 2. Primeiro Acesso do Proprietário

### 2.1. Acessar a Aplicação

**Opção 1: Acesso pelo Subdomínio (Recomendado)**
```
URL: https://saudetotal.primecare.com.br
(ou http://localhost:4200 em desenvolvimento)
```

**Opção 2: Acesso Direto com Tenant ID**
```
URL: https://app.primecare.com.br
(ou http://localhost:4200 em desenvolvimento)
```

### 2.2. Fazer Login como Proprietário

**🚨 MUITO IMPORTANTE:** Proprietários devem marcar a opção **"Login como Proprietário"**

#### **Se usando Subdomínio:**
```
1. Usuário: joao.silva
2. Senha: SenhaForte@123
3. ✅ MARCAR: "Login como Proprietário"
4. Clicar em "Entrar"
```

#### **Se usando Tenant ID:**
```
1. Usuário: joao.silva
2. Senha: SenhaForte@123
3. Tenant ID: abc123-def456-ghi789
4. ✅ MARCAR: "Login como Proprietário"
5. Clicar em "Entrar"
```

### 2.3. Problema Comum: Erro de Login

**Sintoma:** "Usuário ou senha incorretos"

**Causa mais comum:** Não marcou "Login como Proprietário"

**Solução:**
1. ✅ Verifique se marcou a caixa "Login como Proprietário"
2. Tente novamente

**Resultado Esperado:**
- ✅ Login bem-sucedido
- ✅ Redirecionado para o Dashboard
- ✅ Menu lateral com opções de administração
- ✅ Mensagem de boas-vindas

---

## 3. Configuração de Negócio

### 3.1. Acessar Configurações de Negócio

**Passos:**
1. No menu lateral, clique em **"Configurações"**
2. Selecione **"Configuração de Negócio"**

### 3.2. Definir Tipo de Negócio

**Opções disponíveis:**

| Tipo | Descrição | Quando usar |
|------|-----------|-------------|
| **Profissional Solo** | Consultório individual | 1 profissional, atendimento solo |
| **Clínica Pequena** | 2-5 profissionais | Pequena equipe, 1-2 salas |
| **Clínica Média** | 6-15 profissionais | Equipe média, 3-5 salas |
| **Clínica Grande** | 16+ profissionais | Grande estrutura, 6+ salas |

**Exemplo:**
```
✅ Tipo de Negócio: Clínica Média
```

### 3.3. Definir Especialidade Principal

**Especialidades disponíveis:**
- Medicina Geral
- Odontologia
- Psicologia
- Nutrição
- Fisioterapia
- Fonoaudiologia
- Terapia Ocupacional
- Pediatria
- Ginecologia
- Cardiologia
- Dermatologia
- Ortopedia

**Exemplo:**
```
✅ Especialidade Principal: Medicina Geral
```

### 3.4. Funcionalidades Habilitadas Automaticamente

Baseado no **Tipo de Negócio** + **Especialidade**, o sistema habilita automaticamente:

#### **Funcionalidades Clínicas:**
- ✅ Prescrições Eletrônicas
- ✅ Prontuário Eletrônico
- ✅ Agendamento de Consultas
- ✅ Controle de Vacinas (se aplicável)
- ✅ Integração com Laboratórios
- ✅ Controle de Estoque

#### **Funcionalidades Administrativas:**
- ✅ Múltiplas Salas (exceto solo)
- ✅ Fila de Recepção (exceto solo)
- ✅ Módulo Financeiro
- ✅ Convênios Médicos

#### **Funcionalidades de Consulta:**
- ✅ Telemedicina
- ✅ Visita Domiciliar
- ✅ Sessões em Grupo (Psicologia)

#### **Funcionalidades de Marketing:**
- ✅ Perfil Público
- ✅ Agendamento Online
- ✅ Avaliações de Pacientes

#### **Funcionalidades Avançadas:**
- ✅ Relatórios de BI (médias/grandes)
- ✅ Acesso à API (médias/grandes)
- ✅ White Label (grandes)
- ✅ Pagamentos com Cartão

### 3.5. Ajustar Funcionalidades Individualmente

**Passos:**
1. Na mesma tela, role até **"Funcionalidades Disponíveis"**
2. Use os switches para habilitar/desabilitar cada funcionalidade
3. Clique em **"Salvar Configurações"**

**Exemplo:**
```
✅ Telemedicina: LIGADO
✅ Visita Domiciliar: DESLIGADO (não oferecemos)
✅ BI e Relatórios: LIGADO
✅ Agendamento Online: LIGADO
```

**Resultado Esperado:**
- ✅ Configuração salva com sucesso
- ✅ Mensagem de confirmação
- ✅ Funcionalidades refletidas no sistema

---

## 4. Personalização Visual

### 4.1. Acessar Personalização

**Passos:**
1. Menu **"Configurações"** → **"Personalização"**
2. Você verá 4 abas: **Cores**, **Logo**, **Imagem de Fundo**, **Preview**

### 4.2. Configurar Cores da Clínica

**Cores Principais:**

```
✅ Cor Primária: #0066CC (Azul)
   - Usada em botões, links, menus
   
✅ Cor Secundária: #28A745 (Verde)
   - Usada em destaques, ícones
   
✅ Cor da Fonte: #333333 (Cinza Escuro)
   - Usada em textos principais
```

**Como escolher:**
1. Clique no campo de cor
2. Use o seletor de cores ou digite o código hexadecimal
3. Veja o preview em tempo real
4. Clique em **"Salvar Cores"**

**Dicas:**
- Use cores que representem sua marca
- Evite cores muito vibrantes (prejudicam leitura)
- Teste o contraste (texto deve ser legível)

### 4.3. Upload do Logo

**Requisitos:**
- Formato: PNG, JPG ou SVG
- Tamanho máximo: 2 MB
- Dimensões recomendadas: 200x60 pixels
- Fundo transparente (PNG recomendado)

**Passos:**
1. Clique em **"Escolher Arquivo"**
2. Selecione o logo da sua clínica
3. Aguarde o upload
4. Visualize no preview
5. Clique em **"Salvar Logo"**

**Resultado:** Logo aparecerá no topo de todas as páginas.

### 4.4. Imagem de Fundo (Opcional)

**Uso:** Imagem de fundo na tela de login

**Requisitos:**
- Formato: JPG ou PNG
- Tamanho máximo: 5 MB
- Dimensões recomendadas: 1920x1080 pixels

**Passos:**
1. Clique em **"Escolher Arquivo"**
2. Selecione a imagem
3. Aguarde o upload
4. Visualize no preview
5. Clique em **"Salvar Imagem de Fundo"**

**Resultado Esperado:**
- ✅ Todas as personalizações salvas
- ✅ Preview atualizado
- ✅ Logout e login novamente para ver mudanças na tela de login

---

## 5. Configuração de Módulos

### 5.1. Acessar Gerenciamento de Módulos

**Passos:**
1. Menu **"Configurações"** → **"Módulos do Sistema"**
2. Você verá a lista de todos os módulos disponíveis

### 5.2. Módulos Disponíveis

| Módulo | Descrição | Planos |
|--------|-----------|--------|
| **WhatsApp** | Integração com WhatsApp Business | Profissional+ |
| **Relatórios Avançados** | Dashboards e relatórios de BI | Profissional+ |
| **TISS Export** | Exportação de guias TISS | Básico+ |
| **Telemedicina** | Consultas por vídeo | Profissional+ |
| **Portal do Paciente** | Área do paciente online | Básico+ |
| **Assinatura Digital** | Documentos com validade jurídica | Premium+ |
| **CRM** | Gestão de relacionamento | Profissional+ |
| **Analytics** | Análise de dados e métricas | Premium+ |
| **API Access** | Acesso à API REST | Premium+ |

### 5.3. Habilitar Módulos

**Passos:**
1. Localize o módulo desejado
2. Verifique se está disponível no seu plano
3. Clique no botão **"Habilitar"**
4. Configure parâmetros específicos (se aplicável)
5. Clique em **"Confirmar"**

**Exemplo: Habilitar WhatsApp**
```
1. Localizar "WhatsApp Integration"
2. Clicar em "Habilitar"
3. Configurar:
   ✅ Número do WhatsApp Business: +55 11 98765-4321
   ✅ Token da API: [gerado no WhatsApp Business API]
   ✅ Webhook URL: [gerado automaticamente]
4. Clicar em "Salvar e Ativar"
```

**Exemplo: Habilitar Portal do Paciente**
```
1. Localizar "Portal do Paciente"
2. Clicar em "Habilitar"
3. Configuração automática (sem parâmetros)
4. Clicar em "Confirmar"
```

### 5.4. Verificar Módulos Ativos

**Passos:**
1. Na mesma tela, veja a seção **"Módulos Ativos"**
2. Todos os módulos habilitados aparecem com status **"ATIVO"**

**Resultado Esperado:**
- ✅ Módulos habilitados conforme necessidade
- ✅ Status "ATIVO" visível
- ✅ Funcionalidades disponíveis no sistema

---

## 6. Informações Básicas da Clínica

### 6.1. Acessar Informações da Clínica

**Passos:**
1. Menu **"Administração"** → **"Informações da Clínica"**

### 6.2. Atualizar Informações Gerais

**Campos editáveis:**

```
✅ Nome Comercial: "Clínica Saúde Total"
✅ Razão Social: "Clínica Saúde Total Ltda"
✅ CNPJ: "12.345.678/0001-90" (não editável após cadastro)
✅ Inscrição Estadual: "123.456.789.012"
✅ Inscrição Municipal: "987654321"
✅ Telefone Principal: "(11) 3456-7890"
✅ WhatsApp: "(11) 98765-4321"
✅ Email: "contato@saudetotal.com.br"
✅ Site: "www.saudetotal.com.br"
```

### 6.3. Horários de Funcionamento

**Configurar horários de funcionamento:**

```
Segunda a Sexta:
✅ Abertura: 08:00
✅ Fechamento: 18:00
✅ Intervalo: 12:00 - 13:00

Sábado:
✅ Abertura: 08:00
✅ Fechamento: 12:00
✅ Sem intervalo

Domingo:
✅ Fechado
```

### 6.4. Configurações de Agendamento

```
✅ Duração Padrão da Consulta: 30 minutos
✅ Intervalo Mínimo entre Consultas: 0 minutos
✅ Antecedência Mínima para Agendamento: 2 horas
✅ Antecedência Máxima para Agendamento: 60 dias
✅ Permitir Agendamento Online: SIM
✅ Confirmação Automática: NÃO (requer aprovação)
```

### 6.5. Estrutura Física

```
✅ Número de Salas/Consultórios: 4
✅ Número de Leitos: 0 (se aplicável)
✅ Tem Estacionamento: SIM
✅ Tem Acessibilidade: SIM
```

**Resultado Esperado:**
- ✅ Informações atualizadas
- ✅ Horários de funcionamento definidos
- ✅ Configurações de agendamento salvas
- ✅ Mensagem de sucesso exibida

---

## 7. Criação de Usuários

### 7.1. Acessar Gerenciamento de Usuários

**Passos:**
1. Menu **"Administração"** → **"Gerenciar Usuários"**

### 7.2. Criar Primeiro Médico

**Passos:**
1. Clicar em **"+ Novo Usuário"**
2. Preencher formulário:

```
Informações Pessoais:
✅ Nome Completo: "Dra. Maria Santos"
✅ CPF: "987.654.321-00"
✅ Data de Nascimento: "15/03/1985"
✅ Telefone: "(11) 98888-7777"
✅ Email: "maria.santos@saudetotal.com.br"

Credenciais:
✅ Nome de Usuário: "maria.santos"
✅ Senha Inicial: "Senha@123"
✅ Confirmar Senha: "Senha@123"

Perfil Profissional:
✅ Perfil/Role: Doctor (Médico)
✅ Especialidade: Clínica Geral
✅ CRM: "123456"
✅ UF do CRM: "SP"

Configurações:
✅ Status: Ativo
✅ Pode fazer login: SIM
✅ Alterar senha no primeiro acesso: SIM
```

3. Clicar em **"Salvar"**

### 7.3. Criar Secretária/Recepcionista

**Passos:**
1. Clicar em **"+ Novo Usuário"**
2. Preencher formulário:

```
Informações Pessoais:
✅ Nome Completo: "Ana Costa"
✅ CPF: "111.222.333-44"
✅ Telefone: "(11) 97777-6666"
✅ Email: "ana.costa@saudetotal.com.br"

Credenciais:
✅ Nome de Usuário: "ana.costa"
✅ Senha Inicial: "Senha@123"

Perfil:
✅ Perfil/Role: Secretary (Secretária)
✅ Status: Ativo
✅ Alterar senha no primeiro acesso: SIM
```

3. Clicar em **"Salvar"**

### 7.4. Perfis/Roles Disponíveis

| Perfil | Permissões Principais | Quando usar |
|--------|----------------------|-------------|
| **Owner** | Todas as permissões | Proprietário da clínica |
| **Doctor** | Atendimento, prontuário, prescrições | Médicos e profissionais |
| **Secretary** | Agendamento, recepção, cadastros | Secretárias e recepcionistas |
| **Nurse** | Triagem, administração de medicamentos | Enfermeiros |
| **Admin** | Configurações, relatórios, financeiro | Administrador |
| **Receptionist** | Apenas recepção e agendamento | Recepcionista |

### 7.5. Verificar Usuários Criados

**Passos:**
1. Na tela de usuários, visualize a lista
2. Verifique status de cada usuário
3. Teste o login de um usuário secundário (opcional)

**Resultado Esperado:**
- ✅ Usuários criados com sucesso
- ✅ Todos os usuários visíveis na lista
- ✅ Status "Ativo" para usuários habilitados
- ✅ Credenciais funcionando

---

## 8. Verificação Final

### 8.1. Checklist de Configuração Completa

Verifique se todos os itens foram configurados:

```
✅ Clínica registrada e ativa
✅ Primeiro acesso do proprietário realizado
✅ Tipo de negócio definido
✅ Especialidade principal configurada
✅ Funcionalidades habilitadas/desabilitadas conforme necessidade
✅ Cores e logo personalizados
✅ Módulos necessários habilitados
✅ Informações da clínica completas
✅ Horários de funcionamento configurados
✅ Configurações de agendamento definidas
✅ Pelo menos 1 médico cadastrado
✅ Pelo menos 1 secretária cadastrada
```

### 8.2. Teste de Navegação

**Verifique:**
1. ✅ Logo aparece corretamente
2. ✅ Cores personalizadas aplicadas
3. ✅ Módulos habilitados aparecem no menu
4. ✅ Usuários conseguem fazer login

### 8.3. Próximos Passos

Após a configuração básica da clínica, prossiga para:

1. **[Configuração Financeiro](07-Configuracao-Financeiro.md)**
   - Formas de pagamento
   - Categorias de despesas
   - Contas bancárias

2. **[Configuração Fiscal](08-Configuracao-Fiscal.md)**
   - Regime tributário
   - Impostos
   - Notas fiscais

3. **[Cenário Completo de Setup](../CenariosTestesQA/09-Cenario-Completo-Setup-Clinica.md)**
   - Teste completo do zero à primeira consulta

---

## 🔧 Troubleshooting

### Problema: Não consigo fazer login como proprietário

**Soluções:**
1. ✅ Certifique-se de marcar "Login como Proprietário"
2. ✅ Verifique se está usando o usuário correto (não email)
3. ✅ Confirme o Tenant ID (se não usar subdomínio)
4. ✅ Tente resetar a senha

### Problema: Módulo não aparece na lista

**Soluções:**
1. ✅ Verifique se o módulo está disponível no seu plano
2. ✅ Considere fazer upgrade do plano
3. ✅ Entre em contato com suporte

### Problema: Logo não aparece após upload

**Soluções:**
1. ✅ Verifique o tamanho do arquivo (máx 2 MB)
2. ✅ Use formato PNG com fundo transparente
3. ✅ Limpe o cache do navegador (Ctrl+Shift+Delete)
4. ✅ Faça logout e login novamente

### Problema: Configuração não está sendo salva

**Soluções:**
1. ✅ Verifique sua conexão com internet
2. ✅ Verifique o console do navegador (F12) por erros
3. ✅ Tente novamente após alguns minutos
4. ✅ Entre em contato com suporte se persistir

---

## 📚 Documentação Relacionada

- [Configuração do Ambiente](01-Configuracao-Ambiente.md)
- [Configuração do Backend](02-Configuracao-Backend.md)
- [Configuração do Frontend](03-Configuracao-Frontend.md)
- [Configuração Financeiro](07-Configuracao-Financeiro.md)
- [Configuração Fiscal](08-Configuracao-Fiscal.md)
- [Guia de Primeiro Acesso](../../system-admin/guias/OWNER_FIRST_LOGIN_GUIDE.md)
- [Guia de Administração](../../system-admin/guias/CLINIC_ADMIN_GUIDE.md)

---

**Versão:** 1.0  
**Última Atualização:** Fevereiro 2026  
**Mantido por:** Equipe PrimeCare Software
