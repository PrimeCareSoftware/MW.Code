# MedicWarehouse - Versão Estática para GitHub Pages

Esta pasta contém versões estáticas de todos os aplicativos frontend do MedicWarehouse, configuradas para funcionar com dados mockados e sem necessidade de backend.

## 📦 Aplicações Incluídas

### 1. **medicwarehouse-app/** 
Aplicativo principal para gestão de clínicas médicas
- 👨‍⚕️ Dashboard da clínica
- 👥 Gestão de pacientes e prontuários
- 📅 Agendamentos e atendimentos
- 💊 Prescrições médicas
- 🧪 **Modo Mock ativado** - todos os dados são simulados

**URL de acesso:** `/MW.Code/front-static/medicwarehouse-app/`

### 2. **mw-site/**
Site institucional e de marketing
- 🌐 Landing page do produto
- 💰 Apresentação de planos e preços
- 📞 Formulário de contato
- ✨ Showcase de funcionalidades
- 🧪 **Modo Mock ativado** - formulários simulados

**URL de acesso:** `/MW.Code/front-static/mw-site/`

### 3. **mw-system-admin/**
Painel administrativo para system owners
- 🔧 Gestão de todas as clínicas do sistema
- 💰 Métricas financeiras (MRR, receitas, churn)
- 📊 Analytics globais
- ⚙️ Controle de assinaturas
- 🧪 **Modo Mock ativado** - dados administrativos simulados

**URL de acesso:** `/MW.Code/front-static/mw-system-admin/`

### 4. **mw-docs/**
Documentação completa do sistema
- 📚 Guias de uso e setup
- 🔧 Documentação técnica
- 📖 Guias de desenvolvimento
- 🎯 Tutoriais e exemplos

**URL de acesso:** `/MW.Code/front-static/mw-docs/`

### 5. **documentacao/**
Documentação consolidada em arquivo único
- 📄 `MedicWarehouse-Documentacao-Completa.html` - Versão HTML
- 📝 `MedicWarehouse-Documentacao-Completa.md` - Versão Markdown
- 💡 Ideal para consulta offline
- 📱 Otimizado para mobile

**URL de acesso:** `/MW.Code/front-static/documentacao/MedicWarehouse-Documentacao-Completa.html`

## 🔧 Como Foi Construído

### Configurações de Build

Todas as aplicações foram construídas com configurações especiais para deployment estático:

1. **Environment Files:** Criados arquivos `environment.static.ts` com:
   ```typescript
   {
     production: true,
     useMockData: true,  // Ativa interceptador de mocks
     enableDebug: false
   }
   ```

2. **Angular Configuration:** Adicionada configuração `static` no `angular.json`:
   ```json
   {
     "static": {
       "baseHref": "/MW.Code/front-static/{app-name}/",
       "fileReplacements": [
         {
           "replace": "src/environments/environment.ts",
           "with": "src/environments/environment.static.ts"
         }
       ]
     }
   }
   ```

3. **Mock Interceptors:** Interceptadores HTTP que capturam todas as chamadas de API e retornam dados mockados:
   - `medicwarehouse-app`: Mock completo de pacientes, agendamentos, prontuários
   - `mw-system-admin`: Mock de clínicas, métricas financeiras, assinaturas
   - `mw-site`: Mock de planos, registros e contatos

### Comando de Build

Para reconstruir todas as aplicações:

```bash
bash build-static.sh
```

O script:
1. Instala dependências (se necessário)
2. Builda cada app com configuração `--configuration=static`
3. Copia os arquivos buildados para `front-static/`
4. Gera a documentação consolidada

## 🚀 Hospedagem no GitHub Pages

### Configuração Necessária

Para hospedar no GitHub Pages:

1. Habilite GitHub Pages no repositório:
   - Vá em `Settings` → `Pages`
   - Configure **Source** como `GitHub Actions`

2. O workflow já está configurado em `.github/workflows/deploy-static.yml` (criar se não existir)

3. Faça commit e push da pasta `front-static/`

4. Acesse em: `https://{username}.github.io/MW.Code/front-static/`

### Estrutura de URLs

Com GitHub Pages ativo:
- Landing Page: `https://medicwarehouse.github.io/MW.Code/front-static/`
- App Principal: `https://medicwarehouse.github.io/MW.Code/front-static/medicwarehouse-app/`
- Site: `https://medicwarehouse.github.io/MW.Code/front-static/mw-site/`
- Admin: `https://medicwarehouse.github.io/MW.Code/front-static/mw-system-admin/`
- Docs: `https://medicwarehouse.github.io/MW.Code/front-static/mw-docs/`

## 🧪 Modo Mock - Como Funciona

### Interceptadores HTTP

Cada aplicação possui um interceptador que captura requisições HTTP:

```typescript
export const mockDataInterceptor: HttpInterceptorFn = (req, next) => {
  if (!environment.useMockData) {
    return next(req); // Passa para API real
  }
  
  // Retorna dados mockados
  return of(new HttpResponse({ 
    status: 200, 
    body: MOCK_DATA 
  })).pipe(delay(200)); // Simula latência
};
```

### Dados Mockados

Cada app tem sua própria coleção de dados mock em `src/app/mocks/`:
- `auth.mock.ts` - Usuários e autenticação
- `patient.mock.ts` - Pacientes
- `appointment.mock.ts` - Agendamentos
- `medical-record.mock.ts` - Prontuários
- E muitos outros...

### Sem Chamadas Reais à API

✅ **Garantido:** Nenhuma chamada HTTP real é feita quando `useMockData: true`

Todas as operações (GET, POST, PUT, DELETE) são interceptadas e retornam dados simulados.

## 📝 Manutenção e Atualização

### Atualizando Aplicações

1. Faça alterações no código fonte em `frontend/{app-name}/`
2. Execute o script de build:
   ```bash
   bash build-static.sh
   ```
3. Commit e push das alterações em `front-static/`

### Adicionando Novos Dados Mock

1. Edite os arquivos em `frontend/{app-name}/src/app/mocks/`
2. Reconstrua a aplicação
3. Os novos dados estarão disponíveis na versão estática

### Adicionando Novos Endpoints

1. Adicione o handler no `mock-data.interceptor.ts`:
   ```typescript
   if (url.includes('/new-endpoint') && method === 'GET') {
     return of(new HttpResponse({ 
       status: 200, 
       body: MOCK_NEW_DATA 
     })).pipe(delay(mockDelay));
   }
   ```
2. Reconstrua e redeploy

## 🔒 Considerações de Segurança

- ✅ Não há exposição de dados reais
- ✅ Não há conexão com banco de dados
- ✅ Não há chamadas à API backend
- ✅ Todo código roda no navegador do cliente
- ⚠️ Não usar em produção com dados reais
- ⚠️ Apenas para demonstração e desenvolvimento

## 📊 Tamanhos dos Bundles

Aplicações otimizadas para carregamento rápido:

- **medicwarehouse-app:** ~320 KB inicial
- **mw-site:** ~468 KB inicial
- **mw-system-admin:** ~323 KB inicial
- **mw-docs:** ~385 KB inicial

Todos os tamanhos são do bundle inicial (após compressão).

## 🛠️ Tecnologias Utilizadas

- **Angular 20** - Framework frontend
- **TypeScript** - Linguagem
- **RxJS** - Programação reativa
- **SCSS** - Estilização
- **Mock Interceptors** - Simulação de API

## 📞 Suporte

Para dúvidas ou problemas:
- Repositório: [github.com/MedicWarehouse/MW.Code](https://github.com/MedicWarehouse/MW.Code)
- Issues: [github.com/MedicWarehouse/MW.Code/issues](https://github.com/MedicWarehouse/MW.Code/issues)
- Documentação: Acesse a pasta `mw-docs/` ou o arquivo HTML consolidado

## 📜 Licença

Consulte o arquivo LICENSE no repositório principal.
