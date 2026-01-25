# Guia de Teste - Dados Mockados

## Objetivo

Este guia fornece instruções passo a passo para testar a funcionalidade de dados mockados implementada nos aplicativos frontend.

## Pré-requisitos

- Node.js instalado
- npm instalado
- Repositório clonado

## Passo 1: Preparação

### 1.1 Instalar Dependências

```bash
# PrimeCare Software App
cd frontend/medicwarehouse-app
npm install

# MW System Admin
cd ../mw-system-admin
npm install
```

## Passo 2: Habilitar Dados Mockados

### 2.1 PrimeCare Software App

Edite `frontend/medicwarehouse-app/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  enableDebug: true,
  useMockData: true, // ← Mude para true
  // ... resto do arquivo
};
```

### 2.2 MW System Admin

Edite `frontend/mw-system-admin/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  enableDebug: true,
  useMockData: true, // ← Mude para true
  // ... resto do arquivo
};
```

## Passo 3: Executar os Aplicativos

### 3.1 PrimeCare Software App

```bash
cd frontend/medicwarehouse-app
npm start
```

O aplicativo estará disponível em: `http://localhost:4200`

### 3.2 MW System Admin (em outro terminal)

```bash
cd frontend/mw-system-admin
npm start
```

O aplicativo estará disponível em: `http://localhost:4201`

## Passo 4: Cenários de Teste

### Cenário 1: Login no PrimeCare Software App

1. Acesse `http://localhost:4200/login`
2. Digite qualquer credencial (com mocks habilitados, qualquer credencial funciona):
   - Username: `admin`
   - Password: `qualquer-senha`
   - Tenant ID: `clinic1`
3. Clique em "Entrar"
4. **Resultado esperado**: Login bem-sucedido, redirecionamento para dashboard

### Cenário 2: Listar Pacientes

1. Após o login, navegue para a página de pacientes
2. **Resultado esperado**: 
   - Lista com 3 pacientes mockados:
     - João Silva (adulto com hipertensão)
     - Maria Santos (adulta com diabetes)
     - Pedro Oliveira (criança com resfriado, filho de Maria Santos)

### Cenário 3: Visualizar Detalhes do Paciente

1. Clique em um paciente da lista
2. **Resultado esperado**: 
   - Todos os dados do paciente são exibidos
   - Endereço completo
   - Histórico médico
   - Alergias

### Cenário 4: Agendamentos

1. Navegue para a página de agendamentos
2. **Resultado esperado**:
   - Lista com 3 agendamentos mockados
   - Diferentes status: Agendado, Confirmado, Atendido
   - Data: 2024-11-08

### Cenário 5: Agenda Diária

1. Na página de agendamentos, visualize a agenda do dia
2. **Resultado esperado**:
   - Agenda para 2024-11-08
   - 3 agendamentos
   - Horários disponíveis: 11:00, 11:30, 15:00, 15:30, 16:00

### Cenário 6: Procedimentos

1. Navegue para a página de procedimentos
2. **Resultado esperado**:
   - Lista com 4 procedimentos mockados:
     - Consulta Clínica Geral (R$ 150,00)
     - Hemograma Completo (R$ 80,00)
     - Eletrocardiograma (R$ 100,00)
     - Vacinação Gripe (R$ 50,00)

### Cenário 7: Solicitações de Exames

1. Navegue para a página de solicitações de exames
2. **Resultado esperado**:
   - Lista com 3 solicitações:
     - Hemograma Completo (Pendente)
     - Raio-X de Tórax (Agendado, Urgente)
     - Eletrocardiograma (Concluído)

### Cenário 8: Prontuários Médicos

1. Navegue para a página de prontuários
2. **Resultado esperado**:
   - Lista com 3 prontuários mockados
   - Diagnósticos, prescrições e notas completas

### Cenário 9: Fila de Espera

1. Navegue para a página de fila de espera
2. **Resultado esperado**:
   - 3 entradas na fila
   - Diferentes status: Aguardando, Chamado, Concluído
   - Prioridades: Normal, Alta
   - Tempo de espera estimado

### Cenário 10: Login no System Admin

1. Acesse `http://localhost:4201/login`
2. Digite qualquer credencial:
   - Username: `admin`
   - Password: `qualquer-senha`
   - Tenant ID: `system`
3. Clique em "Entrar"
4. **Resultado esperado**: Login bem-sucedido como administrador do sistema

### Cenário 11: Visualizar Clínicas (System Admin)

1. Após login no System Admin, navegue para a página de clínicas
2. **Resultado esperado**:
   - Lista com 3 clínicas mockadas:
     - Clínica Saúde Total (Ativa)
     - Clínica Bem Estar (Trial)
     - Clínica Vida Plena (Suspensa)

### Cenário 12: Analytics do Sistema (System Admin)

1. Navegue para a página de analytics
2. **Resultado esperado**:
   - Total de clínicas: 3
   - Clínicas ativas: 2
   - Receita mensal recorrente: R$ 1.098,00
   - Gráficos e métricas do sistema

### Cenário 13: System Owners (System Admin)

1. Navegue para a página de system owners
2. **Resultado esperado**:
   - Lista com 2 administradores:
     - Admin (último login hoje)
     - Suporte Técnico (último login ontem)

## Passo 5: Testar Operações CRUD

### Criar Novo Paciente

1. Na página de pacientes, clique em "Novo Paciente"
2. Preencha o formulário
3. Salve
4. **Resultado esperado**: 
   - Requisição mockada retorna sucesso
   - Novo paciente "criado" (mas não persiste após refresh)

### Atualizar Paciente

1. Clique em "Editar" em um paciente
2. Modifique algum campo
3. Salve
4. **Resultado esperado**:
   - Requisição mockada retorna sucesso
   - Alterações "salvas" (mas não persistem após refresh)

### Deletar Paciente

1. Clique em "Deletar" em um paciente
2. Confirme
3. **Resultado esperado**:
   - Requisição mockada retorna sucesso
   - Paciente "removido" (mas volta após refresh)

## Passo 6: Verificar Console do Navegador

### Com Mocks Habilitados

1. Abra o DevTools (F12)
2. Vá para a aba "Console"
3. Faça algumas operações
4. **Resultado esperado**:
   - Sem erros de rede (404, 500, etc.)
   - Mensagens de warning podem aparecer para endpoints não mockados
   - Mensagens como: `Mock data interceptor: No mock handler for...` (esperado para endpoints não implementados)

### Com Mocks Desabilitados

1. Mude `useMockData` para `false`
2. Recompile e recarregue
3. **Resultado esperado**:
   - Erros de conexão se o backend não estiver rodando
   - Requisições reais para `http://localhost:5000/api`

## Passo 7: Testar Latência Simulada

1. Com mocks habilitados, abra o Network tab no DevTools
2. Faça uma requisição (ex: carregar lista de pacientes)
3. **Resultado esperado**:
   - Delay de 200-500ms antes da resposta
   - Simula latência de rede real

## Passo 8: Verificar Build de Produção

### Build PrimeCare Software App

```bash
cd frontend/medicwarehouse-app
npm run build
```

**Resultado esperado**: Build bem-sucedido sem erros

### Build MW System Admin

```bash
cd frontend/mw-system-admin
npm run build
```

**Resultado esperado**: Build bem-sucedido sem erros

## Checklist de Validação

- [ ] Login funciona com qualquer credencial
- [ ] Lista de pacientes carrega 3 pacientes mockados
- [ ] Detalhes do paciente são exibidos corretamente
- [ ] Agendamentos carregam com diferentes status
- [ ] Agenda diária mostra horários disponíveis
- [ ] Procedimentos listam 4 itens com preços
- [ ] Solicitações de exames mostram diferentes status
- [ ] Prontuários médicos exibem diagnósticos e prescrições
- [ ] Fila de espera mostra entradas com prioridades
- [ ] System Admin lista clínicas corretamente
- [ ] Analytics do sistema mostram métricas
- [ ] System Owners listam administradores
- [ ] Operações CRUD retornam sucesso
- [ ] Console não mostra erros críticos
- [ ] Latência simulada funciona (200-500ms)
- [ ] Builds de produção completam sem erros

## Troubleshooting

### Problema: Dados não aparecem

**Solução**:
1. Verifique se `useMockData` está `true`
2. Limpe o cache do navegador (Ctrl+Shift+Del)
3. Recompile o aplicativo (`npm start`)

### Problema: Erro "No mock handler"

**Solução**: 
- Isso é esperado para endpoints não implementados
- Pode ser ignorado se não afetar sua funcionalidade

### Problema: Dados desaparecem após refresh

**Solução**:
- Comportamento esperado
- Mocks não persistem dados
- Para persistência, use o backend real

### Problema: Build falha

**Solução**:
1. Verifique se todas as dependências estão instaladas
2. Rode `npm install` novamente
3. Verifique erros de TypeScript no console

## Próximos Passos

Após validar que os mocks funcionam:

1. **Para Desenvolvimento**: Continue usando mocks ou desabilite conforme necessário
2. **Para Testes**: Use mocks para testes de UI
3. **Para Produção**: Certifique-se de que `useMockData` está `false` em `environment.prod.ts`

## Feedback

Se encontrar problemas ou tiver sugestões:
1. Verifique a documentação em `MOCK_DATA_GUIDE.md`
2. Consulte os arquivos de mock em `src/app/mocks/`
3. Revise o interceptor em `src/app/interceptors/mock-data.interceptor.ts`
