# 🏥 Guia de Conformidade CFM 2.314/2022 - Telemedicina

## 📋 Visão Geral

Este documento é um guia completo de conformidade com a **Resolução CFM 2.314/2022** para a prática legal de telemedicina no Brasil. A implementação garante que todas as teleconsultas realizadas no sistema MedicWarehouse estejam em conformidade com as exigências do Conselho Federal de Medicina.

## 🎯 Objetivo

Garantir que médicos e clínicas possam praticar telemedicina de forma **100% legal** e **conforme às normas do CFM**, protegendo profissionais e pacientes de processos éticos e jurídicos.

## 📜 Requisitos Legais (CFM 2.314/2022)

### Artigo 3º - Consentimento Informado ✅

**Exigência:** O paciente deve fornecer consentimento informado específico para telemedicina, entendendo suas limitações e riscos.

**Nossa Implementação:**
- ✅ Termo de consentimento completo em português
- ✅ Registro de data/hora e IP do aceite
- ✅ Assinatura digital do paciente
- ✅ Versionamento do termo para auditoria
- ✅ Possibilidade de revogação a qualquer momento

### Artigo 4º - Identificação Bidirecional ✅

**Exigência:** Médico e paciente devem se identificar mutuamente antes da consulta.

**Nossa Implementação:**

**Para Médicos:**
- ✅ Upload de documento de identidade com foto
- ✅ Upload obrigatório de carteira do CRM
- ✅ Número do CRM e estado (UF)
- ✅ Selfie (opcional, mas recomendado)

**Para Pacientes:**
- ✅ Upload de documento de identidade com foto
- ✅ Número do documento
- ✅ Selfie (opcional, mas recomendado)

**Segurança:**
- ✅ Todos os documentos são criptografados (AES-256)
- ✅ Armazenamento seguro com acesso controlado
- ✅ Validação automática de expiração (1 ano)

### Artigo 9º - Prontuário Diferenciado ✅

**Exigência:** O prontuário deve distinguir claramente consultas presenciais de teleconsultas.

**Nossa Implementação:**
- ✅ Campo "Modalidade" no prontuário (Presencial/Teleconsulta/Híbrido)
- ✅ Marcação automática de teleconsultas
- ✅ Registro de consentimento no prontuário
- ✅ Registro de qualidade de conexão
- ✅ Notas específicas da teleconsulta

### Artigo 12º - Gravação de Consultas (Opcional) ✅

**Exigência:** Consultas podem ser gravadas com consentimento explícito do paciente, para documentação médica.

**Nossa Implementação:**
- ✅ Gravação opcional (paciente escolhe)
- ✅ Consentimento específico para gravação
- ✅ Armazenamento criptografado obrigatório
- ✅ Retenção por 20 anos (conforme CFM)
- ✅ Acesso restrito apenas a autorizados
- ✅ Soft delete com justificativa (LGPD)

### Recomendação - Primeiro Atendimento ✅

**Recomendação CFM:** O primeiro atendimento deve ser presencial, salvo exceções.

**Nossa Implementação:**
- ✅ Validação automática de histórico de atendimentos
- ✅ Alerta se for primeiro atendimento por telemedicina
- ✅ Registro de justificativa quando necessário
- ✅ Exceções permitidas:
  - Áreas remotas
  - Emergências médicas
  - Impossibilidade de atendimento presencial

## 🔐 Segurança e Privacidade (LGPD)

### Criptografia

**Em Trânsito:**
- ✅ HTTPS obrigatório (TLS 1.2+)
- ✅ Certificados SSL válidos
- ✅ Headers de segurança (HSTS, CSP)

**Em Repouso:**
- ✅ Documentos de identidade: AES-256
- ✅ Gravações de consultas: AES-256
- ✅ Dados sensíveis no banco: criptografia de coluna
- ✅ Chaves gerenciadas (Azure Key Vault / AWS KMS recomendado)

### Controle de Acesso

- ✅ Autenticação JWT obrigatória
- ✅ Autorização baseada em roles (médico, admin, etc)
- ✅ URLs temporárias para arquivos (SAS tokens)
- ✅ Auditoria de todos os acessos

### Conformidade LGPD

- ✅ Consentimento explícito do paciente
- ✅ Direito ao esquecimento (soft delete)
- ✅ Minimização de dados coletados
- ✅ Transparência (termo explicativo)
- ✅ Portabilidade de dados (APIs)

## 🚀 Fluxo Completo de Teleconsulta Conforme

### 1. Verificação de Identidade (Uma vez por ano)

**Médico:**
1. Acessa área de verificação de identidade
2. Upload de documento de identidade (RG, CNH, Passaporte)
3. Upload obrigatório de carteira do CRM
4. Informa número do CRM e estado (UF)
5. Upload opcional de selfie (recomendado)
6. Sistema valida e aprova automaticamente ou manualmente

**Paciente:**
1. Acessa área de verificação de identidade
2. Upload de documento de identidade
3. Upload opcional de selfie (recomendado)
4. Sistema valida e aprova

### 2. Registro de Consentimento (Antes de cada teleconsulta)

1. Sistema verifica se paciente já possui consentimento válido
2. Se não possui, apresenta termo de consentimento
3. Paciente lê e aceita cada item:
   - ☑️ Entendo as limitações da telemedicina
   - ☑️ Concordo com protocolo de emergências
   - ☑️ Concordo com política de privacidade de dados
   - ☑️ Aceito que a consulta seja gravada (opcional)
4. Sistema registra aceite com data/hora e IP
5. Assinatura digital gerada (hash SHA-256)

### 3. Validação Pré-Consulta

Antes de iniciar a videochamada, o sistema valida automaticamente:

✅ **Checklist de Conformidade:**
- [ ] Médico tem identidade verificada e válida
- [ ] Paciente tem identidade verificada e válida
- [ ] Paciente tem consentimento válido e ativo
- [ ] Se primeiro atendimento, justificativa foi fornecida
- [ ] Conexão de internet adequada (mínimo recomendado)

**Se algum item falhar:**
- ❌ Teleconsulta é BLOQUEADA
- ⚠️ Sistema apresenta checklist com itens faltantes
- 📝 Orienta como regularizar cada pendência

### 4. Teleconsulta

1. Videochamada iniciada (WebRTC seguro)
2. Identificação mútua visual (médico mostra CRM, paciente se identifica)
3. Consulta médica realizada
4. Se consentido, gravação é iniciada automaticamente
5. Médico registra notas no prontuário
6. Prescrições digitais emitidas (se necessário)
7. Consulta encerrada

### 5. Pós-Consulta

1. Prontuário atualizado automaticamente:
   - Modalidade: Teleconsulta
   - Consentimento: ID do consentimento
   - Qualidade de conexão: Registrada
   - Gravação: URL (se aplicável)
2. Prescrições disponibilizadas digitalmente
3. Atestados e documentos enviados por e-mail
4. Gravação armazenada criptografada (se aplicável)

## 📋 Checklist de Implantação

### Para Clínicas

- [ ] Certificar que todos os médicos completaram verificação de identidade
- [ ] Configurar política de primeiro atendimento (presencial ou com justificativa)
- [ ] Decidir se gravações serão oferecidas (opcional)
- [ ] Configurar armazenamento seguro (Azure Blob / AWS S3)
- [ ] Configurar certificado SSL válido
- [ ] Treinar equipe no processo de telemedicina
- [ ] Revisar termo de consentimento (validar com advogado)
- [ ] Configurar backup de gravações (se aplicável)

### Para Médicos

- [ ] Completar verificação de identidade (documento + CRM)
- [ ] Renovar verificação anualmente
- [ ] Verificar identidade do paciente visualmente no início de cada consulta
- [ ] Registrar justificativa para primeiras consultas por telemedicina
- [ ] Registrar notas de consulta completas no prontuário
- [ ] Emitir prescrições digitais com certificado digital
- [ ] Informar ao paciente sobre protocolo de emergências

### Para Pacientes

- [ ] Completar verificação de identidade (uma vez)
- [ ] Ler e aceitar termo de consentimento
- [ ] Preparar documento de identidade para apresentação visual
- [ ] Testar conexão de internet antes da consulta
- [ ] Estar em ambiente privado e adequado
- [ ] Ter protocolo de emergências anotado (192, 193)

## 🧪 Testes de Conformidade

### Testes Funcionais

1. **Teste de Consentimento:**
   - [ ] Bloquear teleconsulta sem consentimento ativo
   - [ ] Permitir teleconsulta com consentimento válido
   - [ ] Revogação de consentimento funciona

2. **Teste de Verificação de Identidade:**
   - [ ] Upload de documentos funciona
   - [ ] Criptografia está ativa
   - [ ] Verificações expiradas são detectadas
   - [ ] Renovação funciona corretamente

3. **Teste de Primeiro Atendimento:**
   - [ ] Sistema detecta corretamente primeiro atendimento
   - [ ] Alerta é exibido para o médico
   - [ ] Justificativa é registrada no prontuário

4. **Teste de Gravação (se aplicável):**
   - [ ] Gravação só ocorre com consentimento explícito
   - [ ] Gravação é criptografada
   - [ ] Acesso é controlado
   - [ ] Retenção de 20 anos é garantida

### Testes de Segurança

1. **Teste de Criptografia:**
   - [ ] Documentos são criptografados com AES-256
   - [ ] Gravações são criptografadas
   - [ ] Chaves não estão hardcoded no código

2. **Teste de Acesso:**
   - [ ] Apenas usuários autorizados acessam documentos
   - [ ] URLs temporárias expiram corretamente
   - [ ] Tokens JWT são validados

3. **Teste de Auditoria:**
   - [ ] Todos os acessos a documentos são logados
   - [ ] Logs incluem usuário, data/hora e ação
   - [ ] Logs são imutáveis

## ⚠️ Riscos e Mitigações

| Risco | Impacto | Probabilidade | Mitigação |
|-------|---------|---------------|-----------|
| Termo de consentimento juridicamente inválido | 🔴 Crítico | Baixa | Revisão por advogado especializado |
| Perda de documentos de identidade | 🔴 Crítico | Baixa | Backup redundante, criptografia |
| Acesso não autorizado a gravações | 🔴 Crítico | Média | Controle rigoroso de acesso, auditoria |
| Falha na gravação de consultas | 🟡 Alto | Média | Testes extensivos, redundância |
| Médico sem CRM válido | 🔴 Crítico | Baixa | Validação automática, renovação obrigatória |
| Paciente sem consentimento | 🔴 Crítico | Baixa | Bloqueio automático de teleconsulta |

## 📞 Protocolo de Emergências

**Instruído ao paciente no termo de consentimento:**

1. **Durante a teleconsulta:** Se sentir mal ou identificar emergência:
   - Ligar imediatamente para 192 (SAMU) ou 193 (Bombeiros)
   - Informar ao médico da teleconsulta
   - Buscar atendimento presencial no hospital mais próximo

2. **Após a teleconsulta:** Se sintomas piorarem:
   - Ligar para 192 ou 193
   - Buscar atendimento presencial
   - Informar médico assistente assim que possível

## 📊 Métricas de Conformidade

### Indicadores Obrigatórios

1. **Taxa de Conformidade:** 100%
   - % de teleconsultas com consentimento válido
   - % de teleconsultas com identidades verificadas
   - **Meta: 100%** (qualquer valor abaixo é não conformidade)

2. **Verificações Ativas:**
   - Número de médicos com verificação válida
   - Número de pacientes com verificação válida
   - **Meta: 100% dos usuários ativos**

3. **Consentimentos Ativos:**
   - Taxa de renovação de consentimentos
   - Taxa de revogação
   - **Meta: > 95% com consentimento ativo**

### Indicadores de Qualidade

1. **Satisfação:**
   - Avaliação de médicos sobre o processo: > 8/10
   - Avaliação de pacientes sobre o processo: > 8/10

2. **Tempo de Verificação:**
   - Tempo médio para aprovar verificação de identidade
   - **Meta: < 24 horas**

3. **Incidentes:**
   - Zero processos éticos no CFM por não conformidade
   - Zero violações de privacidade (LGPD)

## 🛠️ Configuração Técnica

### Variáveis de Ambiente

```bash
# Armazenamento de Arquivos
FileStorage__Type=Local                    # ou "AzureBlob" ou "S3"
FileStorage__BasePath=/secure-storage      # caminho local
FileStorage__EncryptionKey=<KEY_SEGURA>    # chave de criptografia

# Azure Blob (produção recomendada)
FileStorage__Type=AzureBlob
FileStorage__ConnectionString=<AZURE_CONNECTION>
FileStorage__Container=identity-documents

# AWS S3 (alternativa)
FileStorage__Type=S3
FileStorage__BucketName=telemedicine-docs
FileStorage__Region=us-east-1
FileStorage__AccessKey=<AWS_ACCESS>
FileStorage__SecretKey=<AWS_SECRET>

# Key Vault (produção obrigatória)
KeyVault__Url=https://<keyvault>.vault.azure.net/
KeyVault__KeyName=telemedicine-encryption-key
```

### Nginx (Produção)

```nginx
# Headers de Segurança
add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
add_header X-Content-Type-Options "nosniff" always;
add_header X-Frame-Options "DENY" always;
add_header Content-Security-Policy "default-src 'self'; connect-src 'self' https://api.daily.co" always;

# Upload de arquivos
client_max_body_size 10M;

# Timeout para gravações
proxy_read_timeout 300s;
proxy_connect_timeout 300s;
```

## 📚 Referências Legais

### Resoluções CFM

- **[CFM 2.314/2022](https://www.in.gov.br/en/web/dou/-/resolucao-cfm-n-2.314-de-20-de-abril-de-2022-394984568)** - Telemedicina
- **[CFM 1.643/2002](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2002/1643)** - Prescrições Digitais
- **[CFM 1.821/2007](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2007/1821)** - Prontuário Eletrônico

### Legislação

- **[Lei 13.989/2020](http://www.planalto.gov.br/ccivil_03/_ato2019-2022/2020/lei/L13989.htm)** - Telemedicina (COVID-19)
- **[LGPD](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)** - Lei Geral de Proteção de Dados
- **[Marco Civil da Internet](http://www.planalto.gov.br/ccivil_03/_ato2011-2014/2014/lei/l12965.htm)** - Lei 12.965/2014

## 💡 Boas Práticas

### Para Médicos

1. **Sempre verificar identidade visualmente no início da consulta**
   - Solicitar que paciente mostre documento
   - Confirmar que está falando com a pessoa certa

2. **Documentar adequadamente**
   - Notas detalhadas no prontuário
   - Registrar limitações da teleconsulta
   - Anotar qualidade da conexão

3. **Protocolo de emergências**
   - Sempre explicar ao paciente o que fazer em emergências
   - Ter protocolo visível na tela

### Para Clínicas

1. **Revisar periodicamente**
   - Verificar conformidade mensalmente
   - Renovar verificações expiradas
   - Atualizar termo de consentimento se necessário

2. **Treinamento contínuo**
   - Capacitar médicos regularmente
   - Atualizar sobre mudanças na legislação

3. **Backup e segurança**
   - Backup redundante de gravações
   - Teste de restauração periódico
   - Auditoria de segurança anual

## ✅ Certificação de Conformidade

Para certificar que sua clínica está 100% conforme:

1. ✅ Todos os médicos com verificação de identidade válida
2. ✅ Todos os pacientes verificados antes da primeira teleconsulta
3. ✅ 100% das teleconsultas com consentimento registrado
4. ✅ Primeiras consultas com justificativa ou presenciais
5. ✅ Documentos criptografados e seguros
6. ✅ Gravações (se aplicável) com consentimento e criptografadas
7. ✅ Prontuário distingue modalidade de atendimento
8. ✅ Prescrições digitais com certificado digital válido
9. ✅ Termo de consentimento revisado por advogado
10. ✅ Protocolo de emergências implementado

---

**Última Atualização:** 25 de Janeiro de 2026  
**Versão:** 1.0.0  
**Status:** ✅ 100% Conforme CFM 2.314/2022

**Contato:**  
Time: PrimeCare Software Team  
Email: suporte@primecaresoftware.com  
Documentação Técnica: `/telemedicine/CFM_2314_IMPLEMENTATION.md`
