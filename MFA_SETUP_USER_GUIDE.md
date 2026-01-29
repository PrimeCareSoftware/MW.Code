# 📱 Guia de Configuração MFA - Autenticação de Dois Fatores

**Sistema:** PrimeCare System Admin  
**Versão:** 1.0  
**Atualizado:** Janeiro 2026

---

## 📋 O que é MFA?

MFA (Multi-Factor Authentication) ou Autenticação de Dois Fatores adiciona uma camada extra de segurança à sua conta. Além da senha, você precisará de um código temporário gerado por um aplicativo ou enviado por SMS.

### Por que usar MFA?

✅ **Proteção contra roubo de senha**  
✅ **Segurança enterprise-grade**  
✅ **Compliance LGPD/SOC 2**  
✅ **Detecção de acessos suspeitos**

---

## 🚀 Métodos Disponíveis

### 1. Aplicativo Autenticador (TOTP) - ⭐ Recomendado

**Vantagens:**
- Funciona offline
- Mais seguro que SMS
- Códigos mudam a cada 30 segundos
- Gratuito

**Aplicativos suportados:**
- 🟢 **Google Authenticator** (Android/iOS)
- 🔵 **Microsoft Authenticator** (Android/iOS)
- 🟠 **Authy** (Android/iOS/Desktop)
- 🔴 **1Password** (Multiplataforma, pago)
- 🟣 **Bitwarden** (Multiplataforma, freemium)

### 2. SMS

**Vantagens:**
- Simples de usar
- Não precisa instalar app

**Desvantagens:**
- Vulnerável a SIM swapping
- Requer conexão de celular
- Pode ter atraso

**Nota:** Use SMS apenas se não puder usar TOTP.

---

## 📝 Passo a Passo - Configuração TOTP

### Passo 1: Instale um Aplicativo Autenticador

Escolha um dos aplicativos recomendados e instale no seu smartphone:

- **Google Authenticator**: [Android](https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2) | [iOS](https://apps.apple.com/app/google-authenticator/id388497605)
- **Microsoft Authenticator**: [Android](https://play.google.com/store/apps/details?id=com.azure.authenticator) | [iOS](https://apps.apple.com/app/microsoft-authenticator/id983156458)

### Passo 2: Acesse as Configurações de Segurança

1. Faça login no PrimeCare
2. Clique no seu perfil (canto superior direito)
3. Selecione **"Configurações"**
4. Clique na aba **"Segurança"**
5. Clique em **"Habilitar MFA"**

### Passo 3: Escolha o Método TOTP

1. Selecione **"Aplicativo Autenticador"**
2. Clique em **"Próximo"**

### Passo 4: Escaneie o QR Code

**Opção A - QR Code (recomendado):**
1. Abra o aplicativo autenticador no seu celular
2. Toque em "+" ou "Adicionar conta"
3. Escolha "Escanear QR Code"
4. Aponte a câmera para o QR Code na tela
5. O PrimeCare será adicionado automaticamente

**Opção B - Código Manual:**
1. Se não conseguir escanear, clique em "Digitar manualmente"
2. No aplicativo, escolha "Inserir chave de configuração"
3. Digite:
   - **Nome da conta:** PrimeCare
   - **Chave:** (copie da tela)
   - **Tipo:** Baseado em tempo
4. Salve

### Passo 5: Verifique o Código

1. O aplicativo mostrará um código de 6 dígitos
2. Digite o código no campo "Código de Verificação"
3. Clique em **"Verificar"**

✅ **Sucesso!** O MFA está configurado.

### Passo 6: Salve os Códigos de Backup

⚠️ **IMPORTANTE:** Esta é a parte mais importante!

1. Você verá 10 códigos de backup
2. **Salve estes códigos em local seguro:**
   - Gerenciador de senhas (1Password, Bitwarden)
   - Arquivo criptografado
   - Papel em cofre físico
3. Clique em **"Baixar Códigos"** para ter uma cópia
4. Marque ✅ "Eu salvei meus códigos de backup"
5. Clique em **"Concluir"**

**Exemplo de códigos:**
```
XXXX-1234
XXXX-5678
XXXX-9012
...
```

---

## 🔐 Como Usar MFA no Login

### Login com MFA Habilitado

1. Digite seu **email** e **senha** normalmente
2. Clique em **"Entrar"**
3. Se a senha estiver correta, você verá:
   - Tela de verificação MFA
4. Abra o aplicativo autenticador
5. Digite o código de 6 dígitos mostrado
6. Clique em **"Verificar"**
7. ✅ Você está logado!

### Login de Dispositivo Novo/Suspeito

Se você tentar fazer login de:
- Novo computador
- Novo navegador
- Nova localização
- Novo país

O sistema automaticamente **exigirá MFA**, mesmo que você não tenha habilitado.

**Por quê?** Para proteger sua conta de acessos não autorizados.

---

## 🆘 Perdeu o Acesso ao MFA?

### Opção 1: Use um Código de Backup

1. Na tela de verificação MFA, clique em **"Usar código de backup"**
2. Digite um dos 10 códigos salvos
3. ✅ Você está logado!

**Nota:** Cada código pode ser usado **apenas uma vez**.

### Opção 2: SMS (se configurado)

1. Na tela de verificação MFA, clique em **"Enviar código por SMS"**
2. Você receberá um SMS com código de 6 dígitos
3. Digite o código
4. ✅ Você está logado!

### Opção 3: Contate o Administrador

Se você não tem códigos de backup nem SMS configurado:

1. Contate o administrador da sua clínica
2. Ou envie email para: **suporte@primecare.com**
3. Será necessário:
   - Identificação
   - Comprovação de vínculo com a clínica
   - Pode levar até 24-48h

---

## 🔄 Gerenciar MFA

### Visualizar Códigos de Backup Restantes

1. Configurações → Segurança
2. Seção "Autenticação de Dois Fatores"
3. Ver quantidade de códigos não utilizados

### Regenerar Códigos de Backup

⚠️ **Atenção:** Isto invalidará todos os códigos antigos!

1. Configurações → Segurança
2. "Regenerar códigos de backup"
3. Confirme a ação
4. Salve os novos códigos em local seguro

**Quando regenerar:**
- Após usar mais de 5 códigos
- Se achar que os códigos foram comprometidos
- Anualmente (boa prática)

### Trocar de Aplicativo Autenticador

**Exemplo:** Google Authenticator → Microsoft Authenticator

1. **NÃO** remova a conta do aplicativo antigo ainda
2. Configurações → Segurança
3. "Desabilitar MFA"
4. "Habilitar MFA" novamente
5. Escaneie o novo QR Code no novo aplicativo
6. Verifique que funciona
7. Agora pode remover do aplicativo antigo

### Adicionar Método Secundário (SMS)

1. Configurações → Segurança
2. "Adicionar método secundário"
3. Digite seu número de telefone: `+55 (11) 99999-9999`
4. Clique em "Enviar código"
5. Digite o código recebido por SMS
6. ✅ SMS configurado como backup!

### Desabilitar MFA

⚠️ **Não recomendado** - Diminui significativamente a segurança

1. Configurações → Segurança
2. "Desabilitar MFA"
3. Digite sua senha para confirmar
4. Digite um código MFA válido
5. Confirme a desabilitação

---

## 🛡️ Dicas de Segurança

### ✅ Faça

- ✅ Use TOTP em vez de SMS
- ✅ Salve códigos de backup em gerenciador de senhas
- ✅ Configure método secundário (SMS)
- ✅ Mantenha backup dos QR Codes originais
- ✅ Use aplicativos com backup na nuvem (Authy, Microsoft)
- ✅ Regenere códigos anualmente

### ❌ Não Faça

- ❌ Não tire screenshot do QR Code e deixe em qualquer lugar
- ❌ Não compartilhe códigos de backup
- ❌ Não use o mesmo aplicativo autenticador para tudo em um celular não protegido
- ❌ Não ignore as notificações de login suspeito
- ❌ Não desabilite MFA sem motivo forte

### 🎯 Recomendações Avançadas

**Para máxima segurança:**

1. **Use hardware key** (YubiKey, Google Titan)
   - Requer implementação futura
   - Resistente a phishing

2. **Aplicativo com backup**
   - Authy (backup automático na nuvem)
   - Microsoft Authenticator (backup no OneDrive)
   - Evita perda total se celular quebrar

3. **Múltiplos dispositivos**
   - Configure MFA em tablet também
   - Use smartwatch como backup
   - Mais difícil perder todos ao mesmo tempo

---

## 📱 Screenshots e Exemplos Visuais

### Fluxo Completo de Configuração

```
1. Login → Configurações → Segurança
2. Habilitar MFA → Escolher método
3. Escanear QR Code
4. Verificar código
5. Salvar códigos de backup
6. ✅ Concluído!
```

### Tela de Login com MFA

```
┌─────────────────────────────────┐
│  🔐 Verificação de Dois Fatores │
├─────────────────────────────────┤
│                                 │
│  Digite o código de 6 dígitos   │
│  do seu aplicativo autenticador │
│                                 │
│  ┌─────────────────────────┐   │
│  │      [ _ _ _ _ _ _ ]    │   │
│  └─────────────────────────┘   │
│                                 │
│  [  Verificar  ]                │
│                                 │
│  Usar código de backup          │
│  Enviar código por SMS          │
│                                 │
└─────────────────────────────────┘
```

---

## 🆘 Suporte

### Perguntas Frequentes

**P: O código que digito não funciona!**  
R: Verifique se:
- O relógio do celular está sincronizado
- Você está digitando o código certo (muda a cada 30s)
- O código é do PrimeCare (não de outra conta)

**P: Perdi meu celular!**  
R: Use um código de backup ou contate o administrador imediatamente.

**P: Posso usar o mesmo aplicativo para várias contas?**  
R: Sim! Você pode adicionar quantas contas quiser no mesmo app.

**P: O MFA diminui a velocidade do login?**  
R: Adiciona apenas 5-10 segundos, mas aumenta muito a segurança.

**P: Preciso digitar o código toda vez?**  
R: Sim, mas em alguns casos você pode marcar "Confiar neste dispositivo por 30 dias".

### Contato

- **Email:** suporte@primecare.com
- **Telefone:** +55 (11) XXXX-XXXX
- **Chat:** Disponível das 8h às 18h

---

**Documento criado:** Janeiro 2026  
**Versão:** 1.0  
**Status:** ✅ Completo
