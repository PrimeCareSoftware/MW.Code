# 🎥 Telemedicina - Guia de Configuração e Testes

## 📌 Visão Geral

Este guia fornece instruções completas para configurar e testar o módulo de Telemedicina do Omni Care Software, incluindo videoconsultas, gravação de sessões, integração com Daily.co e conformidade com CFM 1821/2018.

## 🔧 Pré-requisitos

- Sistema iniciado (API + Frontend + Microserviço de Telemedicina)
- Usuário com perfil Medic logado
- Paciente cadastrado
- Conexão de internet estável (mínimo 5 Mbps)
- Navegador moderno com suporte a WebRTC
- Câmera e microfone funcionais
- Conta Daily.co configurada (ou similar)

## 📖 Índice

1. [Configuração Inicial](#configuração-inicial)
2. [Conformidade CFM 1821/2018](#conformidade-cfm-18212018)
3. [Cenários de Teste - Agendamento](#cenários-de-teste---agendamento)
4. [Cenários de Teste - Sala de Espera Virtual](#cenários-de-teste---sala-de-espera-virtual)
5. [Cenários de Teste - Videochamada](#cenários-de-teste---videochamada)
6. [Cenários de Teste - Recursos Avançados](#cenários-de-teste---recursos-avançados)
7. [Cenários de Teste - Gravação](#cenários-de-teste---gravação)
8. [Cenários de Teste - Prescrição Digital](#cenários-de-teste---prescrição-digital)
9. [API Testing](#api-testing)
10. [Troubleshooting](#troubleshooting)

---

## 🔧 Configuração Inicial

### 1. Configurar Integração Daily.co

**Passos:**
1. Acesse **"Configurações"** → **"Telemedicina"** → **"Provedores de Vídeo"**
2. Selecione **"Daily.co"**
3. Preencha credenciais:
   - **API Key:** seu_api_key_aqui
   - **Domain:** seu-dominio.daily.co
   - **Plano:** Free (10.000 min/mês) ou Pro

4. Configure opções:
   - [x] Gravação de sessões
   - [x] Qualidade HD (720p)
   - [x] Transcrição automática
   - [x] Sala de espera virtual

5. Teste conexão
6. Salve

**Resultado Esperado:**
- ✅ Conexão estabelecida
- ✅ Status: "Ativo"
- ✅ Pronto para criar salas

---

### 2. Configurar Certificado Digital

**Passos:**
1. Acesse **"Configurações"** → **"Telemedicina"** → **"Certificado Digital"**
2. Faça upload do certificado A3 ou A1
3. Informe senha
4. Valide certificado

**Resultado Esperado:**
- ✅ Certificado válido
- ✅ CPF do médico confirmado
- ✅ Prazo de validade verificado
- ✅ Pronto para assinar digitalmente

---

### 3. Definir Horários de Telemedicina

**Passos:**
1. Acesse **"Configurações"** → **"Agenda"** → **"Telemedicina"**
2. Configure disponibilidade:
   - **Segunda a Sexta:** 08:00 - 20:00
   - **Sábado:** 08:00 - 12:00
   - **Domingo:** Fechado

3. Defina duração padrão: 30 minutos
4. Configure intervalo: 10 minutos
5. Salve

**Resultado Esperado:**
- ✅ Horários configurados
- ✅ Disponíveis para agendamento online
- ✅ Sincronizados com agenda presencial

---

### 4. Configurar Consentimento do Paciente

**Passos:**
1. Acesse **"Configurações"** → **"Telemedicina"** → **"Termos"**
2. Revise termo de consentimento padrão
3. Personalize se necessário
4. Configure:
   - [x] Obrigatório aceitar antes da consulta
   - [x] Registrar aceite com data/hora
   - [x] Armazenar por 20 anos (LGPD)

5. Salve

**Resultado Esperado:**
- ✅ Termo configurado
- ✅ Será exibido ao paciente
- ✅ Aceite registrado legalmente

---

## ⚖️ Conformidade CFM 1821/2018

### Requisitos Obrigatórios Implementados

#### 1. Consentimento Livre e Esclarecido
- ✅ Termo de consentimento digital
- ✅ Aceite registrado com timestamp
- ✅ Revogável a qualquer momento

#### 2. Identificação do Médico
- ✅ Nome completo
- ✅ CRM e UF
- ✅ Especialidade registrada

#### 3. Identificação do Paciente
- ✅ Nome completo
- ✅ CPF
- ✅ Data de nascimento

#### 4. Prontuário Eletrônico
- ✅ Registro de todas as teleconsultas
- ✅ Data, hora e duração
- ✅ Anamnese e diagnóstico
- ✅ Prescrições digitais

#### 5. Segurança e Privacidade
- ✅ Criptografia ponta-a-ponta
- ✅ HTTPS/TLS
- ✅ Autenticação forte
- ✅ Auditoria de acesso

#### 6. Prescrição Digital
- ✅ Assinatura digital com certificado ICP-Brasil
- ✅ QR Code de validação
- ✅ Rastreabilidade completa

---

## 🧪 Cenários de Teste - Agendamento

### Cenário 1.1: Agendar Teleconsulta pelo Sistema

**Objetivo:** Médico/secretária agenda teleconsulta

**Passos:**
1. Acesse **"Agenda"** → **"Nova Consulta"**
2. Preencha:
   - **Paciente:** Maria Silva Santos
   - **Médico:** Dr. João Santos
   - **Data:** 25/01/2026
   - **Horário:** 14:00
   - **Modalidade:** ✅ **Telemedicina**
   - **Tipo:** Consulta de Retorno
   - **Duração:** 30 minutos

3. Marque [x] **"Enviar link por email/SMS"**
4. Confirme agendamento

**Resultado Esperado:**
- ✅ Consulta agendada
- ✅ Email enviado ao paciente com link
- ✅ SMS de confirmação enviado
- ✅ Sala de vídeo criada (mas não ativa)

---

### Cenário 1.2: Paciente Agenda pelo Portal

**Objetivo:** Autoagendamento de teleconsulta

**Perfil:** Paciente no Patient Portal

**Passos:**
1. Paciente acessa: https://portal.omnicare.com.br
2. Faz login
3. Clica em **"Agendar Consulta"**
4. Seleciona médico: Dr. João Santos
5. Escolhe modalidade: **Telemedicina**
6. Vê horários disponíveis
7. Seleciona: 25/01/2026 às 14:00
8. Confirma agendamento
9. Aceita termo de consentimento

**Resultado Esperado:**
- ✅ Agendamento confirmado
- ✅ Email com link da videochamada
- ✅ Lembrete 1 hora antes
- ✅ Termo de consentimento registrado

---

### Cenário 1.3: Reagendar Teleconsulta

**Objetivo:** Alterar data/hora

**Passos:**
1. Paciente ou médico acessa consulta agendada
2. Clica em **"Reagendar"**
3. Seleciona nova data: 26/01/2026 às 10:00
4. Confirma

**Resultado Esperado:**
- ✅ Data alterada
- ✅ Notificações enviadas
- ✅ Link continua o mesmo (sala reutilizada)

---

## 🧪 Cenários de Teste - Sala de Espera Virtual

### Cenário 2.1: Paciente Entra na Sala de Espera

**Objetivo:** Paciente aguarda médico online

**Perfil:** Paciente

**Passos:**
1. 10 minutos antes da consulta, clique no link recebido
2. Sistema verifica identidade
3. Solicita permissões:
   - Câmera
   - Microfone
   - Notificações

4. Testa áudio e vídeo
5. Entra na sala de espera
6. Visualiza mensagem: "Aguardando o médico..."

**Resultado Esperado:**
- ✅ Sala de espera carregada
- ✅ Vídeo e áudio funcionando
- ✅ Contador de tempo de espera
- ✅ Médico notificado da chegada

---

### Cenário 2.2: Médico Vê Paciente Aguardando

**Objetivo:** Médico é notificado da chegada

**Perfil:** Médico

**Passos:**
1. Sistema envia notificação
2. Médico acessa **"Teleconsultas de Hoje"**
3. Vê paciente com status "Aguardando"
4. Pode ver preview do paciente (opcional)
5. Clica em **"Iniciar Atendimento"**

**Resultado Esperado:**
- ✅ Notificação recebida
- ✅ Status do paciente visível
- ✅ Um clique para iniciar

---

## 🧪 Cenários de Teste - Videochamada

### Cenário 3.1: Iniciar Videochamada

**Objetivo:** Médico e paciente conectados

**Passos:**
1. Médico clica em **"Iniciar Atendimento"**
2. Sistema abre sala de vídeo
3. Conecta médico e paciente
4. Timer inicia automaticamente
5. Prontuário aparece ao lado do vídeo

**Resultado Esperado:**
- ✅ Vídeo e áudio bilateral funcionando
- ✅ Qualidade HD (720p)
- ✅ Latência < 300ms
- ✅ Timer rodando
- ✅ Prontuário acessível

---

### Cenário 3.2: Controles Durante a Chamada

**Objetivo:** Testar funcionalidades

**Controles Disponíveis:**
- 🎤 **Mudo/Ativar Microfone**
- 📹 **Ligar/Desligar Câmera**
- 🔊 **Ajustar Volume**
- 🖥️ **Compartilhar Tela**
- 💬 **Chat de Texto**
- 📄 **Compartilhar Documento**
- ⏸️ **Pausar Gravação**
- 📞 **Encerrar Chamada**

**Teste cada controle:**

**Teste A - Mudo:**
1. Clique em mudo
2. Fale
3. Paciente não deve ouvir
4. Reative

**Teste B - Câmera:**
1. Desligue câmera
2. Apenas voz ativa
3. Religue

**Teste C - Compartilhar Tela:**
1. Clique em compartilhar
2. Selecione tela/janela
3. Paciente vê sua tela
4. Pare compartilhamento

**Resultado Esperado:**
- ✅ Todos os controles funcionam
- ✅ Transições suaves
- ✅ Sem quedas de conexão

---

### Cenário 3.3: Chat de Texto Durante Consulta

**Objetivo:** Comunicação por texto paralela

**Passos:**
1. Durante videochamada, clique em **Chat**
2. Digite mensagem: "Aguarde um momento"
3. Envie
4. Paciente recebe notificação
5. Paciente responde

**Resultado Esperado:**
- ✅ Chat funcional
- ✅ Mensagens em tempo real
- ✅ Histórico salvo no prontuário

---

### Cenário 3.4: Compartilhar Documentos

**Objetivo:** Mostrar exames ao paciente

**Passos:**
1. Clique em **"Compartilhar Arquivo"**
2. Selecione exame: Hemograma.pdf
3. Sistema exibe documento para ambos
4. Médico pode apontar e anotar
5. Paciente acompanha

**Resultado Esperado:**
- ✅ Documento compartilhado
- ✅ Visualização sincronizada
- ✅ Ferramentas de anotação disponíveis

---

### Cenário 3.5: Qualidade de Vídeo Adaptativa

**Objetivo:** Testar em diferentes conexões

**Teste A - Conexão Boa (>10 Mbps):**
- Resultado: HD 720p, sem travamentos

**Teste B - Conexão Média (5-10 Mbps):**
- Resultado: SD 480p, fluido

**Teste C - Conexão Ruim (<5 Mbps):**
- Resultado: 360p ou apenas áudio
- Sistema sugere desligar vídeo

**Resultado Esperado:**
- ✅ Adaptação automática
- ✅ Consulta continua mesmo em baixa qualidade
- ✅ Alertas de qualidade exibidos

---

## 🧪 Cenários de Teste - Recursos Avançados

### Cenário 4.1: Sala de Espera com Múltiplos Pacientes

**Objetivo:** Vários pacientes aguardando

**Passos:**
1. 3 pacientes entram na sala de espera
2. Médico vê lista ordenada por horário
3. Atende na ordem
4. Outros aguardam

**Resultado Esperado:**
- ✅ Fila organizada
- ✅ Tempo de espera visível
- ✅ Pacientes podem sair e retornar

---

### Cenário 4.2: Convidar Acompanhante

**Objetivo:** Familiar participa da consulta

**Passos:**
1. Durante consulta, médico clica em **"Convidar Participante"**
2. Gera link temporário
3. Envia ao acompanhante via WhatsApp
4. Acompanhante entra
5. Agora são 3 na chamada

**Resultado Esperado:**
- ✅ Até 4 participantes simultâneos
- ✅ Link válido por 1 hora
- ✅ Controle de permissões

---

### Cenário 4.3: Transcrição Automática (Beta)

**Objetivo:** Converter fala em texto

**Pré-requisito:** Recurso ativado no plano

**Passos:**
1. Durante consulta, ative **"Transcrição"**
2. Sistema transcreve em tempo real
3. Médico vê transcrição ao lado
4. Pode editar e confirmar
5. Salva no prontuário

**Resultado Esperado:**
- ✅ Transcrição 80-90% precisa
- ✅ Editável pelo médico
- ✅ Facilita documentação

---

### Cenário 4.4: Tradução em Tempo Real (Beta)

**Objetivo:** Atender paciente estrangeiro

**Pré-requisito:** Recurso ativado

**Passos:**
1. Paciente fala inglês
2. Ative **"Tradução"**
3. Configure: Inglês → Português
4. Sistema traduz legenda em tempo real

**Resultado Esperado:**
- ✅ Legendas traduzidas
- ✅ Comunicação viável
- ✅ Útil para casos específicos

---

## 🧪 Cenários de Teste - Gravação

### Cenário 5.1: Gravar Teleconsulta

**Objetivo:** Registrar consulta em vídeo

**Requisitos Legais:**
- ⚖️ Consentimento do paciente obrigatório
- 📝 Finalidade médica ou educacional
- 🔒 Armazenamento seguro (20 anos)

**Passos:**
1. Antes de iniciar, peça consentimento ao paciente
2. Paciente aceita via checkbox
3. Inicie consulta
4. Sistema grava automaticamente
5. Indicador de gravação visível para ambos
6. Finalize consulta
7. Vídeo processado e salvo

**Resultado Esperado:**
- ✅ Consentimento registrado
- ✅ Gravação de áudio e vídeo
- ✅ Arquivo MP4 gerado
- ✅ Armazenado de forma segura
- ✅ Vinculado ao prontuário

---

### Cenário 5.2: Pausar Gravação Temporariamente

**Objetivo:** Parar gravação em momento sensível

**Passos:**
1. Durante consulta gravada
2. Médico clica em **"Pausar Gravação"**
3. Discute informação sensível
4. Clica em **"Retomar Gravação"**

**Resultado Esperado:**
- ✅ Gravação pausada
- ✅ Período não gravado
- ✅ Marcação no arquivo de onde parou

---

### Cenário 5.3: Acessar Gravação Posterior

**Objetivo:** Revisar consulta gravada

**Passos:**
1. Acesse prontuário do paciente
2. Aba **"Teleconsultas"**
3. Localize consulta de 25/01/2026
4. Clique em **"Assistir Gravação"**
5. Vídeo reproduz no navegador
6. Controles: play, pause, avançar, voltar

**Resultado Esperado:**
- ✅ Vídeo disponível por 20 anos
- ✅ Qualidade preservada
- ✅ Pode compartilhar com especialista (com autorização)

---

## 🧪 Cenários de Teste - Prescrição Digital

### Cenário 6.1: Prescrever Durante Teleconsulta

**Objetivo:** Receita digital válida

**Passos:**
1. Durante teleconsulta, clique em **"Nova Prescrição"**
2. Adicione medicamentos:
   - Dipirona 500mg - 6/6h - 3 dias
   - Amoxicilina 500mg - 8/8h - 7 dias

3. Adicione orientações
4. Assine digitalmente com certificado A3
5. Gera PDF com QR Code
6. Envia ao paciente por email/SMS

**Resultado Esperado:**
- ✅ Receita assinada digitalmente
- ✅ QR Code de validação
- ✅ Paciente pode usar em farmácia
- ✅ Conforme CFM 1821/2018

---

### Cenário 6.2: Validação de Receita Digital

**Objetivo:** Farmácia valida receita

**Perfil:** Farmacêutico

**Passos:**
1. Paciente apresenta receita digital (PDF ou QR Code)
2. Farmacêutico escaneia QR Code
3. Sistema valida:
   - Autenticidade da assinatura
   - Validade da receita (30 dias)
   - Médico habilitado
   - Não foi utilizada anteriormente

4. Se válido, dispensa medicamento

**Resultado Esperado:**
- ✅ Validação em tempo real
- ✅ Segurança contra fraudes
- ✅ Rastreabilidade completa

---

## 🔌 API Testing

### Endpoint: Criar Sala de Telemedicina

```bash
curl -X POST "http://localhost:5100/api/telemedicine/sessions" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "appointmentId": "appointment-uuid",
    "medicId": "medic-uuid",
    "patientId": "patient-uuid",
    "scheduledTime": "2026-01-25T14:00:00Z",
    "duration": 30,
    "enableRecording": true
  }'
```

**Resposta Esperada:**
```json
{
  "sessionId": "session-uuid",
  "roomUrl": "https://primecare.daily.co/session-uuid",
  "medicToken": "jwt_token_medico",
  "patientToken": "jwt_token_paciente",
  "expiresAt": "2026-01-25T14:45:00Z"
}
```

---

### Endpoint: Iniciar Sessão

```bash
curl -X POST "http://localhost:5100/api/telemedicine/sessions/{session_id}/start" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

---

### Endpoint: Finalizar Sessão

```bash
curl -X POST "http://localhost:5100/api/telemedicine/sessions/{session_id}/end" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "actualDuration": 28,
    "notes": "Consulta realizada com sucesso"
  }'
```

---

### Endpoint: Obter Gravação

```bash
curl -X GET "http://localhost:5100/api/telemedicine/sessions/{session_id}/recording" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

**Resposta:**
```json
{
  "recordingUrl": "https://cdn.daily.co/recordings/rec-123.mp4",
  "duration": 28,
  "size": 245000000,
  "format": "mp4"
}
```

---

## 🐛 Troubleshooting

### Problema 1: Câmera não funciona

**Causa:** Permissões não concedidas

**Solução:**
1. Verifique permissões do navegador
2. Acesse Configurações → Privacidade → Câmera
3. Autorize o site
4. Recarregue a página

---

### Problema 2: Áudio com eco

**Causa:** Múltiplos dispositivos ou fone/auto-falante

**Solução:**
1. Use fone de ouvido
2. Ou desligue alto-falante
3. Evite múltiplas abas abertas

---

### Problema 3: Vídeo travando

**Causa:** Conexão instável

**Solução:**
1. Verifique velocidade da internet
2. Feche outros aplicativos
3. Reduza qualidade (nas configurações)
4. Última opção: apenas áudio

---

### Problema 4: Link não abre

**Causa:** Link expirado ou inválido

**Solução:**
1. Verifique se é a data/hora correta
2. Links são válidos 15 min antes até 15 min depois
3. Solicite novo link ao médico

---

### Problema 5: Não consegue gravar

**Causa:** Plano não permite ou espaço cheio

**Solução:**
1. Verifique plano contratado
2. Libere espaço de armazenamento
3. Entre em contato com suporte

---

## ✅ Checklist de Validação Final

- [ ] Configurar integração Daily.co
- [ ] Configurar certificado digital
- [ ] Definir horários de telemedicina
- [ ] Configurar termo de consentimento
- [ ] Agendar teleconsulta pelo sistema
- [ ] Paciente agenda pelo portal
- [ ] Reagendar teleconsulta
- [ ] Entrar na sala de espera
- [ ] Médico notificado da chegada
- [ ] Iniciar videochamada
- [ ] Testar controles (mudo, câmera, etc.)
- [ ] Usar chat de texto
- [ ] Compartilhar documentos
- [ ] Testar qualidade adaptativa
- [ ] Fila de múltiplos pacientes
- [ ] Convidar acompanhante
- [ ] Gravar teleconsulta
- [ ] Pausar gravação
- [ ] Acessar gravação posterior
- [ ] Prescrever digitalmente
- [ ] Validar receita digital
- [ ] Testes de API

---

## 📚 Documentação Relacionada

- [Guia Rápido Telemedicina](../GUIA_RAPIDO_TELEMEDICINA.md)
- [Implementação Telemedicina](../TELEMEDICINA_IMPLEMENTATION_SUMMARY.md)
- [Integração Frontend](../../telemedicine/FRONTEND_INTEGRATION.md)
- [Análise de Serviços de Vídeo](../TELEMEDICINE_VIDEO_SERVICES_ANALYSIS.md)
- [Resolução CFM 1821/2018](../CFM_1821_IMPLEMENTACAO.md)

## 🔗 Links Úteis

- [Resolução CFM 1821/2018](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2018/1821)
- [Portal de Telemedicina CFM](https://portal.cfm.org.br/telemedicina/)
- [Daily.co Documentation](https://docs.daily.co/)
- [WebRTC Standards](https://webrtc.org/)
