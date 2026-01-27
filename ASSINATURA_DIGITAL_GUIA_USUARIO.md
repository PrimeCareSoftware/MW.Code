# 📋 Guia de Uso - Assinatura Digital ICP-Brasil

## 🎯 O que é a Assinatura Digital?

A assinatura digital é o equivalente eletrônico da assinatura manuscrita, com **validade jurídica** garantida pela ICP-Brasil. Ela assegura:

- ✅ **Autenticidade:** Confirma quem assinou o documento
- ✅ **Integridade:** Detecta qualquer alteração após a assinatura
- ✅ **Não-repúdio:** O assinante não pode negar que assinou
- ✅ **Conformidade CFM:** Atende CFM 1.821/2007 e CFM 1.638/2002

## 📱 Tipos de Certificados

### Certificado A1 (Software)
- 💾 Armazenado no computador
- ⏱️ Validade: 1 ano
- 💰 Custo: R$ 200-300/ano
- ✅ Mais prático para uso diário
- ⚠️ Requer backup e senha forte

### Certificado A3 (Token/Smartcard)
- 🔐 Armazenado em hardware físico (token USB ou cartão)
- ⏱️ Validade: 3-5 anos
- 💰 Custo: R$ 250-500 (certificado + token)
- ✅ Maior segurança
- ⚠️ Requer token conectado para assinar

## 🚀 Como Começar

### Passo 1: Adquirir Certificado Digital

1. **Escolha uma Autoridade Certificadora (AC) credenciada:**
   - Certisign
   - Serasa Experian
   - Valid
   - Soluti
   - Outras ACs ICP-Brasil

2. **Compre o certificado:**
   - Acesse o site da AC
   - Escolha tipo: A1 ou A3
   - Escolha tipo de pessoa: e-CPF (pessoa física) ou e-CNPJ
   - Para médicos, e-CPF é suficiente

3. **Valide sua identidade:**
   - Videoconferência (para alguns casos)
   - Presencial em posto de atendimento
   - Leve documentos: RG, CPF, comprovante de residência

4. **Receba o certificado:**
   - **A1:** Download de arquivo .pfx
   - **A3:** Token enviado pelo correio

### Passo 2: Configurar Certificado no Sistema

#### Para Certificado A1:

1. Acesse o sistema PrimeCare
2. Vá em **Configurações** → **Certificado Digital**
3. Clique em **Importar Certificado A1**
4. Selecione o arquivo `.pfx` baixado
5. Digite a senha do certificado
6. Clique em **Importar**

✅ **Pronto!** O certificado está configurado.

#### Para Certificado A3:

1. Conecte o token USB no computador
2. Instale os drivers do token (se necessário)
3. Acesse o sistema PrimeCare
4. Vá em **Configurações** → **Certificado Digital**
5. Clique em **Detectar Certificado A3**
6. Selecione o certificado na lista
7. Clique em **Registrar**

✅ **Pronto!** O certificado está registrado.

## ✍️ Como Assinar Documentos

### Assinar Prontuário Médico

1. Abra o prontuário do paciente
2. Preencha todas as informações necessárias
3. Clique no botão **Assinar Prontuário** 🔏
4. Confirme as informações exibidas:
   - Paciente
   - Data/Hora
   - Certificado a ser usado
5. **Para A1:** Digite a senha do certificado (se solicitado)
6. **Para A3:** Conecte o token e digite o PIN (se solicitado)
7. Marque a opção **"Incluir carimbo de tempo"** (recomendado)
8. Clique em **Assinar**

⏳ **Aguarde alguns segundos** - A assinatura está sendo processada.

✅ **Sucesso!** O prontuário foi assinado digitalmente.

### Assinar Receita Médica

1. Crie a receita no sistema
2. Adicione todos os medicamentos
3. Clique em **Assinar Receita** 🔏
4. Siga os mesmos passos da assinatura de prontuário

💡 **Dica:** Receitas assinadas digitalmente têm validade jurídica e podem ser enviadas eletronicamente ao paciente.

### Assinar Atestado Médico

1. Crie o atestado
2. Preencha data de início, fim, CID (opcional)
3. Clique em **Assinar Atestado** 🔏
4. Siga o processo de assinatura

## 🔍 Como Verificar Assinaturas

### Ver Detalhes de uma Assinatura

1. Abra o documento (prontuário, receita, atestado)
2. Clique no ícone 🔏 **Ver Assinatura**
3. O sistema exibe:
   - ✅ Status: **Assinatura Válida** ou ❌ **Assinatura Inválida**
   - 👤 Assinado por: Nome do médico + CRM
   - 📅 Data/Hora da assinatura
   - 📜 Certificado utilizado
   - ⏰ Carimbo de tempo (se houver)
   - 🔐 Hash SHA-256 do documento

### Revalidar Assinatura

Se quiser verificar novamente a validade:

1. Na tela de detalhes da assinatura
2. Clique em **Revalidar Assinatura**
3. O sistema verifica:
   - Integridade do documento
   - Validade do certificado
   - Carimbo de tempo (se houver)

## ⚙️ Gerenciar Certificados

### Ver Certificados Cadastrados

1. Acesse **Configurações** → **Certificado Digital**
2. Visualize a lista de certificados:
   - Tipo (A1/A3)
   - Validade
   - Dias para expiração
   - Total de assinaturas realizadas

### Trocar Certificado

Ao importar um novo certificado, o anterior é automaticamente desativado.

### Renovar Certificado

1. Adquira o novo certificado na Autoridade Certificadora
2. Importe/registre o novo certificado no sistema
3. O antigo será automaticamente desativado

⚠️ **Importante:** Documentos assinados com o certificado antigo permanecem válidos.

## ❓ Perguntas Frequentes

### 1. Preciso assinar todos os documentos?

**Sim.** A CFM 1.821/2007 exige assinatura digital em prontuários eletrônicos. Receitas e atestados também precisam de assinatura conforme CFM 1.638/2002.

### 2. Posso usar o mesmo certificado em vários computadores?

- **A1:** Sim, mas NÃO recomendado por segurança. Prefira usar em apenas um computador.
- **A3:** Sim, basta conectar o token no computador desejado.

### 3. O que acontece se meu certificado expirar?

- Documentos já assinados continuam válidos (principalmente com carimbo de tempo)
- Você precisa renovar o certificado para assinar novos documentos
- O sistema avisa quando o certificado está próximo do vencimento

### 4. Perdi meu token A3, e agora?

1. Entre em contato com a Autoridade Certificadora IMEDIATAMENTE
2. Solicite revogação do certificado
3. Adquira um novo certificado

### 5. Esqueci a senha do certificado A1

- Não há como recuperar a senha
- Você precisará adquirir um novo certificado

### 6. O que é o "carimbo de tempo" (timestamp)?

É uma prova inquestionável da data e hora da assinatura, fornecida por uma Autoridade de Carimbo de Tempo (TSA) confiável. **Sempre recomendamos incluir.**

### 7. Quanto tempo leva para assinar?

- Normalmente 3-10 segundos
- Depende de:
  - Tipo de certificado (A3 pode ser mais lento)
  - Disponibilidade da TSA (carimbo de tempo)
  - Conexão com internet

### 8. Posso assinar offline?

- **Sem carimbo de tempo:** Sim
- **Com carimbo de tempo:** Não (requer internet para acessar TSA)

### 9. O sistema funciona em Linux/Mac?

- **A1:** Sim, funciona em qualquer plataforma
- **A3:** Requer Windows ou drivers PKCS#11 específicos

### 10. Quanto custa?

**Custo do Certificado:**
- A1: R$ 200-300/ano
- A3: R$ 250-500 (certificado 3-5 anos) + R$ 50-100 (token)

**Sem custo adicional no sistema PrimeCare** - funcionalidade incluída.

## 🆘 Resolução de Problemas

### Erro: "Certificado ou senha inválidos"

✅ **Soluções:**
1. Verifique se está digitando a senha corretamente
2. Tente abrir o certificado no Windows para confirmar a senha
3. Confirme que o arquivo .pfx está íntegro

### Erro: "Token A3 não está conectado"

✅ **Soluções:**
1. Conecte o token USB
2. Instale os drivers do fabricante do token
3. Reinicie o navegador após conectar o token
4. Verifique no Windows se o certificado aparece em "Certificados do Usuário"

### Erro: "Certificado expirado"

✅ **Solução:**
- Renove o certificado junto à Autoridade Certificadora
- Importe/registre o novo certificado no sistema

### Erro: "Não foi possível obter carimbo de tempo"

✅ **Soluções:**
1. Verifique sua conexão com internet
2. Tente novamente (pode ser indisponibilidade temporária da TSA)
3. Se persistir, desmarque a opção "Incluir carimbo de tempo"

### Assinatura muito lenta

✅ **Possíveis causas:**
- Token A3 lento (normal, é processo criptográfico)
- TSA demorando para responder
- Conexão lenta com internet

**Não se preocupe:** É normal levar até 10 segundos.

## 📞 Suporte

**Dúvidas sobre certificados:**
- Entre em contato com sua Autoridade Certificadora

**Dúvidas sobre o sistema:**
- Contate o suporte técnico do PrimeCare
- Email: suporte@primecare.com.br
- Telefone: (XX) XXXX-XXXX

## 📚 Recursos Adicionais

- [CFM 1.821/2007](http://www.portalmedico.org.br/resolucoes/cfm/2007/1821_2007.htm) - Resolução sobre prontuários eletrônicos
- [ICP-Brasil](https://www.gov.br/iti/pt-br/assuntos/icp-brasil) - Site oficial da infraestrutura de chaves públicas
- [Autoridades Certificadoras Credenciadas](https://www.gov.br/iti/pt-br/assuntos/repositorio/autoridades-certificadoras) - Lista de ACs credenciadas

---

**Versão:** 1.0  
**Última atualização:** Janeiro 2026  
**Sistema:** PrimeCare Medical Warehouse
