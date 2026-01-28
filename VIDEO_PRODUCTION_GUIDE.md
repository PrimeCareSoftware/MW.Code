# Guia de Produção - Vídeo Demonstrativo PrimeCare

> **Documento Técnico**  
> **Para:** Equipe de Produção / Freelancers / Agência  
> **Data:** 28 de Janeiro de 2026

---

## 📑 Índice

1. [Preparação do Ambiente](#preparacao)
2. [Ferramentas Recomendadas](#ferramentas)
3. [Screen Recording Guidelines](#recording)
4. [Dados Demo para Gravação](#dados-demo)
5. [Edição e Pós-Produção](#edicao)
6. [Entrega e Formatos](#entrega)

---

<a name="preparacao"></a>
## 1. Preparação do Ambiente

### 1.1 Setup do Sistema para Gravação

**Antes de gravar, configure:**

1. **Resolução da Tela:**
   - Configurar monitor para 1920x1080 (Full HD)
   - Escala de interface: 100% (não usar zoom)

2. **Browser:**
   - Usar Chrome ou Edge (mais estáveis para gravação)
   - Abrir em modo anônimo (clean, sem extensões)
   - Fullscreen (F11) ou maximize window
   - Zoom 100%

3. **Sistema Operacional:**
   - Desativar notificações (Modo Foco no Windows/macOS)
   - Fechar aplicativos em background
   - Esconder barra de tarefas (auto-hide)
   - Cursor: usar cursor padrão (não personalizado)

4. **Dados Demo:**
   - Popular banco com dados fictícios realistas
   - Ver seção [Dados Demo](#dados-demo)

### 1.2 Ambiente de Trabalho

**Checklist pré-gravação:**
- [ ] Internet estável e rápida
- [ ] Sistema rodando localmente (não depender de cloud/APIs externas)
- [ ] Banco de dados populado com dados demo
- [ ] Backup do ambiente (caso algo dê errado)
- [ ] Testar fluxos antes de gravar

---

<a name="ferramentas"></a>
## 2. Ferramentas Recomendadas

### 2.1 Screen Recording

#### OBS Studio (Gratuito - Recomendado)
**Download:** https://obsproject.com

**Configurações OBS:**
```
Resolução: 1920x1080
FPS: 30
Encoder: x264
Bitrate: 8000 kbps
Preset: High Quality
Format: mp4
```

**Prós:**
- Gratuito e open-source
- Qualidade profissional
- Configurável

**Contras:**
- Curva de aprendizado inicial

#### Alternativas

**Camtasia (Pago - $249):**
- Interface intuitiva
- Editor integrado
- Biblioteca de assets
- https://www.techsmith.com/camtasia.html

**ScreenFlow (macOS - $169):**
- Específico para Mac
- Excelente integração macOS
- https://www.telestream.net/screenflow/

**Loom (Freemium):**
- Rápido e simples
- Limite de qualidade na versão free
- Ideal para drafts/testes

### 2.2 Edição de Vídeo

#### DaVinci Resolve (Gratuito - Recomendado)
**Download:** https://www.blackmagicdesign.com/products/davinciresolve

**Prós:**
- Gratuito
- Profissional (usado em Hollywood)
- Color grading excelente
- Effects e motion graphics

#### Alternativas

**Adobe Premiere Pro (Assinatura $20.99/mês):**
- Industry standard
- Integração Adobe (After Effects, Audition)

**Final Cut Pro (macOS - $299):**
- Apenas Mac
- Performance otimizada

**iMovie (macOS - Gratuito):**
- Simples
- Limitado para projetos profissionais

### 2.3 Áudio

#### Narração
**Audacity (Gratuito):**
- Gravação e edição de áudio
- https://www.audacityteam.org

**Adobe Audition (Assinatura):**
- Profissional
- Noise reduction avançado

#### Equipamento Mínimo
- **Microfone:** Blue Yeti, Audio-Technica AT2020, ou similar (~R$ 500-800)
- **Ambiente:** Sala silenciosa, com tratamento acústico básico (cortinas, tapetes)
- **Evitar:** Microfone de headset ou laptop (qualidade baixa)

### 2.4 Motion Graphics

**After Effects (Assinatura):**
- Para animações de texto/overlays elaborados

**Canva (Freemium):**
- Para overlays simples
- Templates prontos

---

<a name="recording"></a>
## 3. Screen Recording Guidelines

### 3.1 Técnicas de Gravação

**Movimentos do Mouse:**
- Movimentos **lentos e deliberados**
- Evitar movimentos erráticos
- Pausar cursor em elementos importantes (500ms)

**Cliques:**
- Destacar cliques com cursor highlight (configurar no OBS ou adicionar na edição)
- Aguardar 300-500ms após cada clique para visualização carregar

**Scrolling:**
- Scroll **muito lento** (mais lento que uso normal)
- Usar scroll suave (não scroll de mouse físico, usar arrow keys ou trackpad)

**Transições entre telas:**
- Aguardar 1-2 segundos em cada tela antes de transitar
- Não cortar abruptamente

### 3.2 Roteiro de Gravação por Feature

#### Feature 1: Agenda Inteligente

**Passos para gravar:**

1. **Cena inicial (3s):**
   - Mostrar dashboard inicial
   - Highlight do menu "Agenda"

2. **Navegação (2s):**
   - Clicar em "Agenda" no menu lateral
   - Transição para tela de agenda

3. **Visualização (5s):**
   - Mostrar agenda populada (semana atual)
   - Consultas coloridas por tipo/médico
   - Scroll leve de cima para baixo

4. **Criar agendamento (8s):**
   - Clicar em botão "Nova Consulta"
   - Modal abre
   - Selecionar paciente: "Maria Silva"
   - Selecionar data: próxima quarta-feira 14h
   - Selecionar tipo: "Consulta de Retorno"
   - Clicar "Confirmar"
   - Animação de sucesso
   - Agendamento aparece na agenda

5. **Recursos extras (2s):**
   - Hover em uma consulta (tooltip aparece)
   - Mostrar opções: Editar, Cancelar, Enviar Lembrete

**Total: ~20 segundos**

#### Feature 2: Prontuário Eletrônico

**Passos para gravar:**

1. **Navegação (2s):**
   - Da agenda, clicar em uma consulta existente
   - Ou ir para "Pacientes" > selecionar paciente

2. **Ficha do Paciente (3s):**
   - Mostrar cabeçalho com foto, nome, idade
   - Dados demográficos visíveis

3. **Prontuário (8s):**
   - Clicar em tab "Prontuário"
   - Scroll pelo histórico:
     - Consultas anteriores
     - Diagnósticos
     - Medicações prescritas
   - Highlight de seção "Alergias" e "Condições"

4. **Anexar Exame (4s):**
   - Clicar em "Anexar Exame"
   - Dialog de upload aparece
   - Selecionar arquivo (mockup: "Hemograma_Completo.pdf")
   - Confirmar
   - Exame aparece na lista

5. **Prescrição Digital (3s):**
   - Clicar em "Nova Prescrição"
   - Form rápido aparece
   - Mostrar campos (não preencher tudo, só visual)
   - Fechar modal

**Total: ~20 segundos**

#### Feature 3: Gestão Financeira

**Passos para gravar:**

1. **Dashboard Financeiro (5s):**
   - Navegar para "Financeiro"
   - Mostrar dashboard:
     - Card "Receita do Mês": R$ 45.000
     - Card "Pendências": R$ 8.500
     - Gráfico de barras (receita últimos 6 meses)

2. **Lançamento de Recebimento (8s):**
   - Clicar em "Novo Recebimento"
   - Form aparece
   - Selecionar paciente: "João Santos"
   - Valor: R$ 200,00
   - Forma: "Cartão de Crédito"
   - Clicar "Confirmar"
   - Toast de sucesso

3. **Lista de Transações (4s):**
   - Scroll pela lista de recebimentos
   - Highlight de filtros (por data, status)

4. **Relatório (3s):**
   - Clicar em "Relatórios"
   - Mostrar opção "Relatório Mensal"
   - Preview do PDF (não abrir, só mostrar ícone)

**Total: ~20 segundos**

#### Feature 4: Comunicação

**Passos para gravar:**

1. **Tela de Comunicações (3s):**
   - Navegar para "Comunicações" ou "Notificações"
   - Mostrar lista de mensagens enviadas

2. **Enviar Lembrete (8s):**
   - Voltar para Agenda
   - Clicar em consulta do dia seguinte
   - Botão "Enviar Lembrete"
   - Modal aparece com preview da mensagem:
     ```
     Olá, Maria! Lembrete: você tem consulta
     amanhã às 14h com Dr. Pedro. 
     Confirme sua presença respondendo SIM.
     ```
   - Checkbox "WhatsApp" selecionado
   - Clicar "Enviar"
   - Sucesso

3. **Confirmações (4s):**
   - Mostrar indicador de "Lido" e "Confirmado" na agenda

**Total: ~15 segundos**

#### Feature 5: Relatórios e Analytics

**Passos para gravar:**

1. **Dashboard BI (7s):**
   - Navegar para "Relatórios" ou "Analytics"
   - Mostrar dashboard com múltiplos KPIs:
     - Consultas realizadas (mês): 147
     - Taxa de ocupação: 78%
     - Novos pacientes: 23
     - Receita: R$ 45.000

2. **Gráfico Interativo (5s):**
   - Hover em gráfico de linha (consultas por dia)
   - Tooltip aparece com valores
   - Zoom in em um período

3. **Exportar (3s):**
   - Clicar em botão "Exportar"
   - Dropdown: Excel, PDF, CSV
   - Selecionar "PDF"
   - Toast "Relatório gerado com sucesso"

**Total: ~15 segundos**

#### Feature 6: Telemedicina (Bonus)

**Passos para gravar:**

1. **Tela de Telemedicina (5s):**
   - Navegar para "Telemedicina"
   - Mostrar lista de consultas online agendadas

2. **Sala de Espera (3s):**
   - Clicar em consulta
   - Mostrar "Sala de Espera Virtual"
   - Paciente aguardando (mockup)

3. **Iniciar Consulta (2s):**
   - Botão "Iniciar Consulta"
   - Transição para tela de vídeo (pode ser mockup/screenshot)

**Total: ~10 segundos**

---

<a name="dados-demo"></a>
## 4. Dados Demo para Gravação

### 4.1 Pacientes Fictícios

**Criar no mínimo 10 pacientes com dados realistas:**

| Nome | Idade | Telefone | Email | Última Consulta |
|------|-------|----------|-------|-----------------|
| Maria Silva | 34 anos | (11) 98765-4321 | maria.silva@email.com | 15/01/2026 |
| João Santos | 28 anos | (11) 97654-3210 | joao.santos@email.com | 10/01/2026 |
| Ana Costa | 45 anos | (11) 96543-2109 | ana.costa@email.com | 08/01/2026 |
| Pedro Oliveira | 52 anos | (11) 95432-1098 | pedro.oliveira@email.com | 20/01/2026 |
| Carla Souza | 29 anos | (11) 94321-0987 | carla.souza@email.com | 22/01/2026 |
| Roberto Lima | 41 anos | (11) 93210-9876 | roberto.lima@email.com | 18/01/2026 |
| Juliana Rocha | 37 anos | (11) 92109-8765 | juliana.rocha@email.com | 25/01/2026 |
| Fernando Alves | 60 anos | (11) 91098-7654 | fernando.alves@email.com | 12/01/2026 |
| Patrícia Mendes | 33 anos | (11) 90987-6543 | patricia.mendes@email.com | 14/01/2026 |
| Lucas Martins | 26 anos | (11) 89876-5432 | lucas.martins@email.com | 19/01/2026 |

### 4.2 Consultas Agendadas

**Criar agenda da semana atual:**

**Segunda-feira:**
- 09:00 - Maria Silva - Consulta de Rotina
- 10:00 - João Santos - Retorno
- 14:00 - Ana Costa - Primeira Consulta
- 15:30 - Pedro Oliveira - Check-up

**Terça-feira:**
- 08:30 - Carla Souza - Consulta
- 10:00 - Roberto Lima - Retorno
- 13:00 - Juliana Rocha - Consulta
- 16:00 - Fernando Alves - Check-up

**Quarta-feira:**
- 09:00 - Patrícia Mendes - Consulta
- 11:00 - Lucas Martins - Primeira Consulta
- 14:00 - (Horário vago - para demonstrar agendamento)
- 15:00 - Maria Silva - Retorno

### 4.3 Dados Financeiros

**Receitas do Mês (Janeiro 2026):**
- Total: R$ 45.000,00
- Recebido: R$ 36.500,00
- Pendente: R$ 8.500,00
- Consultas: 147
- Ticket Médio: R$ 306,12

**Últimas Transações:**
1. Maria Silva - R$ 250,00 - Cartão - 26/01/2026 ✓ Pago
2. João Santos - R$ 200,00 - Dinheiro - 25/01/2026 ✓ Pago
3. Ana Costa - R$ 300,00 - PIX - 24/01/2026 ✓ Pago
4. Pedro Oliveira - R$ 400,00 - Cartão - 23/01/2026 ⏳ Pendente
5. Carla Souza - R$ 250,00 - Boleto - 22/01/2026 ⏳ Pendente

### 4.4 Scripts SQL para Popular Dados Demo

```sql
-- Inserir pacientes demo
INSERT INTO Patients (Name, BirthDate, Phone, Email, CPF) VALUES
('Maria Silva', '1992-05-15', '11987654321', 'maria.silva@email.com', '123.456.789-00'),
('João Santos', '1996-08-22', '11976543210', 'joao.santos@email.com', '234.567.890-11'),
-- ... (adicionar todos)

-- Inserir consultas
INSERT INTO Appointments (PatientId, DoctorId, DateTime, Type, Status) VALUES
(1, 1, '2026-01-27 09:00:00', 'Consulta de Rotina', 'Scheduled'),
(2, 1, '2026-01-27 10:00:00', 'Retorno', 'Scheduled'),
-- ... (adicionar todas)

-- Inserir transações financeiras
INSERT INTO FinancialTransactions (PatientId, Amount, PaymentMethod, Status, Date) VALUES
(1, 250.00, 'CreditCard', 'Paid', '2026-01-26'),
(2, 200.00, 'Cash', 'Paid', '2026-01-25'),
-- ... (adicionar todas)
```

---

<a name="edicao"></a>
## 5. Edição e Pós-Produção

### 5.1 Timeline do Projeto

**DaVinci Resolve / Premiere Pro:**

```
Track 1 (Video): Screen recordings
Track 2 (Video): Overlays/Texto
Track 3 (Video): Logo/Watermark
Track 4 (Audio): Narração
Track 5 (Audio): Música de fundo
```

### 5.2 Transições

**Usar:**
- Crossfade (dissolve): 500ms entre cenas
- Cut direto: dentro da mesma feature
- Fade to black: apenas no início e fim

**Evitar:**
- Wipes, spins, 3D transitions (muito datado)
- Transições > 1 segundo (lento demais)

### 5.3 Overlays de Texto

**Template para textos animados:**

**Títulos de Feature:**
- Font: Inter Bold, 48px
- Color: #FFFFFF com shadow
- Animation: Slide in from left (300ms)
- Position: Top left ou centralizado

**Bullet Points (✓):**
- Font: Inter Regular, 24px
- Color: #FFFFFF
- Icon: Green checkmark (#10B981)
- Animation: Fade in + slight scale (200ms each, stagger 100ms)
- Position: Left third da tela

**Exemplo After Effects/Motion:**
```
Text 1: Fade In + Slide (0.0s - 0.3s)
Text 2: Fade In + Slide (0.1s - 0.4s)
Text 3: Fade In + Slide (0.2s - 0.5s)
Hold: 3-5 segundos
Fade Out: 0.3s
```

### 5.4 Color Grading

**Ajustes básicos:**
- Brightness: +5 a +10 (telas ficam mais vivas)
- Contrast: +10 a +15
- Saturation: +5 (não exagerar)
- Warm tone: +2 a +5 (mais convidativo)

**LUT (Look Up Table):**
- Usar LUT "Corporate" ou "Tech" (disponível em pacotes gratuitos)
- Intensidade: 50-70% (não 100%)

### 5.5 Música de Fundo

**Configurações:**
- Volume: -30dB a -40dB (bem abaixo da narração)
- Fade In: 2s no início
- Fade Out: 3s no final
- Ducking: Reduzir automaticamente quando narração acontece (usar compressor sidechain)

**Tempo de batida:**
- Sincronizar cortes com batidas quando possível (não obrigatório)

---

<a name="entrega"></a>
## 6. Entrega e Formatos

### 6.1 Formatos de Exportação

#### Formato Principal: MP4 (Web Optimized)

**Settings:**
```
Codec: H.264
Resolution: 1920x1080
Frame Rate: 30fps
Bitrate: 8 Mbps (CBR ou VBR de 2 passadas)
Audio: AAC, 192 kbps, Stereo
Profile: High
Level: 4.2
```

#### Formato Alternativo: WebM (Opcional)

**Para compatibilidade adicional:**
```
Codec: VP9
Resolution: 1920x1080
Frame Rate: 30fps
Bitrate: 5 Mbps
Audio: Opus, 128 kbps
```

### 6.2 Versões Adicionais

**Mobile-optimized (720p):**
```
Resolution: 1280x720
Bitrate: 3-4 Mbps
Tamanho: ~30-40 MB
```

**Thumbnail (Poster Image):**
- Extrair frame aos 5 segundos
- Resolução: 1920x1080
- Formato: JPG (quality 90%)
- Adicionar overlay: "▶ Assista ao vídeo"

### 6.3 Legendas (Subtitles)

**Formato: SRT**

**Exemplo:**
```srt
1
00:00:00,000 --> 00:00:05,000
Você perde horas organizando consultas manualmente?

2
00:00:05,000 --> 00:00:10,000
Papéis, planilhas confusas, pacientes esperando...

3
00:00:10,000 --> 00:00:13,000
Existe uma forma melhor.

4
00:00:15,000 --> 00:00:20,000
Com a Agenda Inteligente do PrimeCare,
agende consultas em segundos.
```

**Diretrizes:**
- Máximo 2 linhas por legenda
- Máximo 42 caracteres por linha
- Duração mínima: 1 segundo
- Duração máxima: 7 segundos
- Gap entre legendas: mínimo 100ms

**Formato alternativo: VTT (WebVTT)**
```vtt
WEBVTT

00:00:00.000 --> 00:00:05.000
Você perde horas organizando consultas manualmente?

00:00:05.000 --> 00:00:10.000
Papéis, planilhas confusas, pacientes esperando...
```

### 6.4 Metadados e Naming

**Nome do arquivo:**
```
primecare-video-demonstrativo-v1.0-1080p-pt-br.mp4
```

**Metadados a incluir (metadata):**
- Title: "PrimeCare Software - Vídeo Demonstrativo"
- Description: "Conheça o PrimeCare em 3 minutos..."
- Keywords: "gestão clínica, software médico, prontuário eletrônico"
- Author: "PrimeCare Software"
- Copyright: "© 2026 PrimeCare Software. Todos os direitos reservados."
- Creation Date: YYYY-MM-DD

### 6.5 Checklist Final de Entrega

- [ ] Video MP4 1080p (principal)
- [ ] Video MP4 720p (mobile)
- [ ] Thumbnail JPG
- [ ] Legendas SRT/VTT (PT-BR)
- [ ] Arquivo de projeto editável (.prproj / .dra / .fcpx)
- [ ] Assets separados (overlays, música, narração)
- [ ] Documento com especificações técnicas
- [ ] Link de preview (YouTube unlisted ou Vimeo)

---

## 📊 Cronograma de Produção

| Fase | Duração | Responsável |
|------|---------|-------------|
| **1. Preparação** | 2 dias | Dev Team |
| - Setup ambiente | 4h | |
| - Popular dados demo | 4h | |
| **2. Gravação** | 3 dias | Video Producer |
| - Screen recordings | 8h | |
| - Narração | 4h | Voice Artist |
| - B-roll extra | 4h | |
| **3. Edição** | 5 dias | Video Editor |
| - Montagem inicial | 8h | |
| - Adicionar narração | 4h | |
| - Motion graphics | 8h | Motion Designer |
| - Color grading | 4h | |
| - Áudio mixing | 4h | |
| **4. Revisão** | 2 dias | Stakeholders |
| - Feedback round 1 | 1 dia | |
| - Ajustes | 4h | |
| - Aprovação final | 1 dia | |
| **5. Entrega** | 1 dia | Video Producer |
| - Export final | 2h | |
| - Upload e testes | 2h | |
| - Integração site | 4h | Frontend Dev |
| **TOTAL** | **~15 dias úteis** | |

---

## 🎯 Critérios de Qualidade

### Checklist de Aprovação

**Visual:**
- [ ] Resolução nítida (1080p sem pixelação)
- [ ] Cores consistentes e profissionais
- [ ] Transições suaves
- [ ] Sem elementos de UI cortados/fora da tela
- [ ] Cursor visível e movimentos fluidos

**Áudio:**
- [ ] Narração clara e sem ruídos
- [ ] Volume balanceado (narração > música)
- [ ] Sem clipping ou distorção
- [ ] Música adequada e licenciada

**Conteúdo:**
- [ ] Segue o script aprovado
- [ ] Todas as features demonstradas
- [ ] Timing correto (2-3 minutos)
- [ ] CTAs claros
- [ ] Legendas sincronizadas

**Técnico:**
- [ ] Formato correto (MP4, H.264)
- [ ] Bitrate adequado (~8 Mbps)
- [ ] Metadata completo
- [ ] Thumbnail atrativo
- [ ] Testado em múltiplos devices

---

## 📞 Contatos e Suporte

**Dúvidas sobre o script:**
- Consultar: VIDEO_DEMONSTRATIVO_SCRIPT.md

**Dúvidas técnicas do sistema:**
- Equipe de desenvolvimento: dev@primecare.com.br

**Aprovações:**
- Product Manager: pm@primecare.com.br

**Fornecedores recomendados:**
- **Narração (PT-BR):** [Locutores Brasileiros](https://locutores.com.br)
- **Música Royalty-free:** Epidemic Sound, AudioJungle, Artlist
- **Motion Graphics:** Fiverr (Motion Designers Brasil)

---

**Documento preparado por:** GitHub Copilot Agent  
**Versão:** 1.0  
**Data:** 28 de Janeiro de 2026
