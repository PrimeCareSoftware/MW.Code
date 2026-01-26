# Sistema de Ajuda - MedicWarehouse App

Este documento descreve o sistema de ajuda implementado no MedicWarehouse App.

## Visão Geral

O sistema de ajuda fornece páginas de ajuda em HTML/Angular para cada módulo principal do sistema, ensinando os usuários a testar cada fluxo com dados válidos para validações.

## Componentes Implementados

### 1. HelpService (`src/app/services/help.service.ts`)

Serviço responsável por gerenciar todo o conteúdo de ajuda do sistema. Contém:

- **Estrutura de dados**: Define interfaces para `HelpContent`, `HelpSection` e `TestDataExample`
- **Conteúdo de ajuda**: Armazena ajuda detalhada para os seguintes módulos:
  - Pacientes
  - Agendamentos
  - Atendimento
  - Prontuários Médicos
  - Prescrições
  - Financeiro
  - TISS/TUSS
  - Telemedicina
  - Registros SOAP
  - Anamnese

Cada módulo inclui:
- Título da ajuda
- Múltiplas seções com conteúdo HTML
- Exemplos de dados válidos para testes

### 2. HelpDialogComponent (`src/app/pages/help/help-dialog.*`)

Componente modal que exibe o conteúdo de ajuda. Recursos:

- **Modal interativo**: Exibe ajuda em um diálogo sobreposto
- **Abrir em nova janela**: Botão para abrir ajuda em uma janela separada do navegador
- **HTML estilizado**: Conteúdo formatado com CSS profissional
- **Exemplos destacados**: Dados de teste exibidos em cards coloridos
- **Responsivo**: Adapta-se a diferentes tamanhos de tela

Arquivos:
- `help-dialog.ts`: Lógica do componente
- `help-dialog.html`: Template do modal
- `help-dialog.scss`: Estilos do modal

### 3. HelpButtonComponent (`src/app/shared/help-button/help-button.*`)

Botão flutuante (FAB) reutilizável que pode ser adicionado a qualquer página. Recursos:

- **Botão FAB**: Botão de ação flutuante no canto da tela
- **Posicionamento configurável**: Pode ser posicionado no topo ou parte inferior
- **Ícone de ajuda**: Ícone de interrogação intuitivo
- **Integração simples**: Adiciona-se com uma única linha de código

Arquivos:
- `help-button.ts`: Lógica do componente
- `help-button.html`: Template do botão
- `help-button.scss`: Estilos do botão FAB

## Como Usar

### Para Desenvolvedores

#### Adicionar ajuda a uma nova página:

1. Importe o componente no TypeScript da página:

```typescript
import { HelpButtonComponent } from '../../shared/help-button/help-button';

@Component({
  // ...
  imports: [CommonModule, HelpButtonComponent, /* outros imports */],
  // ...
})
```

2. Adicione o botão no HTML da página:

```html
<app-help-button module="nome-do-modulo"></app-help-button>
```

3. Se necessário, adicione novo conteúdo de ajuda no `help.service.ts`:

```typescript
this.helpContent.set('nome-do-modulo', {
  title: 'Ajuda - Módulo X',
  sections: [
    {
      title: 'Como usar',
      content: `<p>Instruções aqui...</p>`,
      testData: [
        {
          field: 'Nome do Campo',
          validExample: 'Exemplo válido',
          description: 'Descrição da validação'
        }
      ]
    }
  ]
});
```

### Para Usuários

1. **Acessar a ajuda**: Clique no botão "Ajuda" no canto inferior direito de qualquer tela
2. **Navegar o conteúdo**: Role para ver todas as seções de ajuda
3. **Abrir em nova janela**: Clique no ícone de janela para abrir em uma aba separada
4. **Imprimir**: Na janela separada, use o botão "Imprimir Ajuda" ou Ctrl+P

## Páginas com Ajuda Integrada

As seguintes páginas já possuem o botão de ajuda integrado:

- ✅ Listagem de Pacientes
- ✅ Calendário de Agendamentos
- ✅ Atendimento
- ✅ Prescrições
- ✅ Contas a Receber (Financeiro)
- ✅ Guias TISS
- ✅ Sessões de Telemedicina
- ✅ Registros SOAP
- ✅ Gerenciamento de Templates de Anamnese

## Estrutura de Dados de Teste

Cada módulo inclui exemplos de dados válidos para facilitar o teste:

- **Campo**: Nome do campo do formulário
- **Exemplo válido**: Valor de exemplo que passa na validação
- **Descrição**: Explicação das regras de validação

Exemplo:
- Campo: CPF
- Exemplo: 123.456.789-00
- Descrição: CPF válido no formato XXX.XXX.XXX-XX

## Tecnologias Utilizadas

- **Angular 20**: Framework principal
- **TypeScript**: Linguagem de programação
- **SCSS**: Pré-processador CSS para estilos
- **Standalone Components**: Componentes independentes do Angular
- **Signals**: API moderna do Angular para reatividade

## Notas Técnicas

- Todos os componentes são standalone (não requerem NgModule)
- O serviço usa `providedIn: 'root'` para ser singleton
- O HelpDialogComponent gera HTML completo para janelas separadas
- Estilos são responsivos e adaptam-se a mobile
- Z-index do modal é 9999 para ficar acima de outros elementos
- FAB tem z-index 1000 para não interferir com modais

## Manutenção

Para adicionar ou modificar conteúdo de ajuda:

1. Edite o arquivo `src/app/services/help.service.ts`
2. Localize o método `initializeHelpContent()`
3. Adicione ou modifique o conteúdo usando HTML
4. Use a estrutura de `TestDataExample` para dados de teste
5. Rebuild a aplicação

## Melhorias Futuras

Possíveis melhorias para o sistema:

- [ ] Adicionar busca no conteúdo de ajuda
- [ ] Permitir marcadores/favoritos em seções
- [ ] Adicionar vídeos tutoriais
- [ ] Implementar histórico de páginas visitadas
- [ ] Adicionar feedback do usuário sobre a ajuda
- [ ] Internacionalização (i18n) para múltiplos idiomas
- [ ] Tour guiado interativo pela aplicação
