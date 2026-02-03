# 🔗 Guia de Integração - Assinatura Digital ICP-Brasil

**Data:** Janeiro 2026  
**Versão:** 1.0  
**Status:** Implementação Completa - Pronto para Integração

---

## 📋 Visão Geral

Este guia mostra como integrar a funcionalidade de **Assinatura Digital ICP-Brasil** com os módulos de documentos existentes no sistema Omni Care (prontuários, receitas, atestados, laudos, etc.).

A infraestrutura de assinatura digital está **100% implementada e funcional**. Os componentes foram projetados como **standalone** e podem ser facilmente importados e utilizados em qualquer módulo.

---

## ✅ O Que Está Pronto

### Backend (API REST)
- ✅ **9 endpoints REST** totalmente funcionais
- ✅ **CertificateManager** - Gerenciamento de certificados A1 e A3
- ✅ **AssinaturaDigitalService** - Assinatura e validação de documentos
- ✅ **TimestampService** - Carimbo de tempo RFC 3161
- ✅ **Database** - Migrations aplicadas, tabelas criadas

### Frontend (Angular)
- ✅ **4 componentes standalone** prontos para uso:
  - `GerenciarCertificadosComponent` - Lista e gerencia certificados
  - `ImportarCertificadoComponent` - Importa/registra certificados
  - `AssinarDocumentoComponent` - Dialog para assinar documentos
  - `VerificarAssinaturaComponent` - Visualiza e valida assinaturas
  
- ✅ **2 services HTTP** com todos os métodos necessários
- ✅ **Models TypeScript** completos e tipados

### Documentação
- ✅ Documentação técnica completa
- ✅ Guia do usuário
- ✅ Exemplos de código
- ✅ APIs documentadas

---

## 🎯 Módulos para Integração

Os seguintes módulos podem se beneficiar da assinatura digital:

| Módulo | Diretório | Tipo de Documento | Prioridade |
|--------|-----------|-------------------|------------|
| Prontuário | `frontend/medicwarehouse-app/src/app/pages/medical-records` | `TipoDocumento.Prontuario` | 🔥 Alta |
| Receitas | `frontend/medicwarehouse-app/src/app/pages/prescriptions` | `TipoDocumento.Receita` | 🔥 Alta |
| Atestados | *(a localizar)* | `TipoDocumento.Atestado` | 🔥 Alta |
| Laudos | *(a localizar)* | `TipoDocumento.Laudo` | ⚡ Média |
| SOAP Records | `frontend/medicwarehouse-app/src/app/pages/soap-records` | `TipoDocumento.Prontuario` | ⚡ Média |

---

## 🚀 Como Integrar - Passo a Passo

### Passo 1: Importar Models e Services

```typescript
// No componente do módulo de documentos (ex: prontuario-view.component.ts)

// 1. Importar models
import { 
  TipoDocumento, 
  AssinaturaDigital 
} from '@app/models/assinatura-digital.model';

// 2. Importar services
import { AssinaturaDigitalService } from '@app/services/assinatura-digital.service';
import { CertificadoDigitalService } from '@app/services/certificado-digital.service';

// 3. Importar componentes
import { AssinarDocumentoComponent } from '@app/pages/assinatura-digital/assinar-documento.component';
import { VerificarAssinaturaComponent } from '@app/pages/assinatura-digital/verificar-assinatura.component';

// 4. Injetar no construtor
constructor(
  private dialog: MatDialog,
  private assinaturaService: AssinaturaDigitalService,
  private certificadoService: CertificadoDigitalService
) {}
```

### Passo 2: Adicionar Botão de Assinatura

```html
<!-- No template do visualizador de documentos -->

<!-- Exemplo: prontuario-view.component.html -->
<mat-card>
  <mat-card-header>
    <mat-card-title>Prontuário #{{ prontuario.id }}</mat-card-title>
    <mat-card-subtitle>
      Paciente: {{ prontuario.paciente.nome }}
    </mat-card-subtitle>
  </mat-card-header>

  <mat-card-content>
    <!-- Conteúdo do prontuário -->
    <div class="prontuario-content">
      {{ prontuario.conteudo }}
    </div>
  </mat-card-content>

  <mat-card-actions>
    <!-- Botões existentes -->
    <button mat-button color="primary" (click)="editar()">
      <mat-icon>edit</mat-icon>
      Editar
    </button>
    
    <button mat-button (click)="imprimir()">
      <mat-icon>print</mat-icon>
      Imprimir
    </button>

    <!-- ✨ NOVO: Botão de Assinatura Digital -->
    <button 
      mat-raised-button 
      color="accent" 
      (click)="assinarDigitalmente()"
      [disabled]="prontuario.assinado || !podeeAssinar()">
      <mat-icon>verified</mat-icon>
      {{ prontuario.assinado ? 'Assinado Digitalmente' : 'Assinar Digitalmente' }}
    </button>

    <!-- ✨ NOVO: Botão para Ver Assinaturas (se já assinado) -->
    <button 
      mat-button 
      (click)="verAssinaturas()"
      *ngIf="prontuario.assinado">
      <mat-icon>fact_check</mat-icon>
      Ver Assinaturas
    </button>
  </mat-card-actions>
</mat-card>

<!-- ✨ NOVO: Seção de assinaturas (se já assinado) -->
<mat-card *ngIf="assinaturas && assinaturas.length > 0" class="assinaturas-card">
  <mat-card-header>
    <mat-card-title>
      <mat-icon>verified_user</mat-icon>
      Assinaturas Digitais ({{ assinaturas.length }})
    </mat-card-title>
  </mat-card-header>
  
  <mat-card-content>
    <app-verificar-assinatura 
      *ngFor="let assinatura of assinaturas"
      [assinatura]="assinatura">
    </app-verificar-assinatura>
  </mat-card-content>
</mat-card>
```

### Passo 3: Implementar Métodos no Component

```typescript
// prontuario-view.component.ts

export class ProntuarioViewComponent implements OnInit {
  prontuario: Prontuario;
  assinaturas: AssinaturaDigital[] = [];
  
  ngOnInit() {
    // Código existente para carregar prontuário
    this.carregarProntuario();
    
    // ✨ NOVO: Carregar assinaturas se o documento já foi assinado
    if (this.prontuario.assinado) {
      this.carregarAssinaturas();
    }
  }
  
  /**
   * ✨ NOVO: Verifica se o usuário pode assinar (deve ser médico)
   */
  podeeAssinar(): boolean {
    const usuario = this.authService.getCurrentUser();
    return usuario && usuario.role === 'Medico';
  }
  
  /**
   * ✨ NOVO: Abre dialog para assinar digitalmente
   */
  async assinarDigitalmente() {
    try {
      // 1. Verificar se médico tem certificado cadastrado
      const certificados = await this.certificadoService
        .listarCertificados()
        .toPromise();
      
      if (!certificados || certificados.length === 0) {
        this.snackBar.open(
          'Você precisa importar um certificado digital antes de assinar. Acesse "Gerenciar Certificados".',
          'OK',
          { duration: 5000 }
        );
        return;
      }
      
      // 2. Gerar PDF do documento (se necessário)
      const pdfBytes = await this.gerarPdfProntuario();
      
      // 3. Abrir dialog de assinatura
      const dialogRef = this.dialog.open(AssinarDocumentoComponent, {
        width: '600px',
        data: {
          documentoId: this.prontuario.id,
          tipoDocumento: TipoDocumento.Prontuario,
          tipoDocumentoNome: 'Prontuário',
          documentoBytes: pdfBytes, // PDF em base64
          pacienteNome: this.prontuario.paciente.nome,
          data: this.prontuario.dataCriacao
        }
      });
      
      // 4. Aguardar resultado
      dialogRef.afterClosed().subscribe(async (resultado) => {
        if (resultado && resultado.assinatura) {
          // Documento foi assinado com sucesso!
          this.snackBar.open('Documento assinado com sucesso!', 'OK', {
            duration: 3000
          });
          
          // Atualizar status do documento
          this.prontuario.assinado = true;
          this.prontuario.dataAssinatura = new Date();
          
          // Recarregar assinaturas
          await this.carregarAssinaturas();
          
          // Atualizar no backend (se necessário)
          await this.marcarDocumentoComoAssinado();
        }
      });
      
    } catch (error) {
      console.error('Erro ao assinar documento:', error);
      this.snackBar.open(
        'Erro ao assinar documento. Tente novamente.',
        'OK',
        { duration: 5000 }
      );
    }
  }
  
  /**
   * ✨ NOVO: Carrega assinaturas do documento
   */
  async carregarAssinaturas() {
    try {
      this.assinaturas = await this.assinaturaService
        .obterAssinaturasPorDocumento(
          this.prontuario.id,
          TipoDocumento.Prontuario
        )
        .toPromise();
    } catch (error) {
      console.error('Erro ao carregar assinaturas:', error);
    }
  }
  
  /**
   * ✨ NOVO: Abre dialog para ver detalhes das assinaturas
   */
  verAssinaturas() {
    // Scroll suave até a seção de assinaturas
    const element = document.querySelector('.assinaturas-card');
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' });
    }
  }
  
  /**
   * ✨ NOVO: Gera PDF do prontuário para assinatura
   * NOTA: Você precisa implementar esta função de acordo com sua estrutura
   */
  private async gerarPdfProntuario(): Promise<string> {
    // Opção 1: Se você já tem geração de PDF no backend
    const response = await this.prontuarioService
      .gerarPdf(this.prontuario.id)
      .toPromise();
    return response.pdfBase64;
    
    // Opção 2: Se você gera PDF no frontend (pdfmake, jspdf, etc)
    // const pdfDocDefinition = this.criarDefinicaoPdf();
    // const pdf = pdfMake.createPdf(pdfDocDefinition);
    // return new Promise((resolve) => {
    //   pdf.getBase64((base64) => resolve(base64));
    // });
  }
  
  /**
   * ✨ NOVO: Marca documento como assinado no backend (opcional)
   */
  private async marcarDocumentoComoAssinado() {
    // Se seu backend tem um endpoint específico para marcar como assinado
    await this.prontuarioService
      .marcarComoAssinado(this.prontuario.id)
      .toPromise();
  }
}
```

### Passo 4: Adicionar Estilos (Opcional)

```scss
// prontuario-view.component.scss

.assinaturas-card {
  margin-top: 20px;
  
  mat-card-header {
    background: #f5f5f5;
    padding: 16px;
    margin: -16px -16px 16px -16px;
    
    mat-icon {
      color: #4caf50;
      vertical-align: middle;
      margin-right: 8px;
    }
  }
  
  app-verificar-assinatura {
    display: block;
    margin-bottom: 16px;
    
    &:last-child {
      margin-bottom: 0;
    }
  }
}

// Badge de status de assinatura
.status-assinado {
  display: inline-flex;
  align-items: center;
  padding: 4px 12px;
  background: #e8f5e9;
  color: #2e7d32;
  border-radius: 16px;
  font-size: 12px;
  font-weight: 500;
  
  mat-icon {
    font-size: 16px;
    width: 16px;
    height: 16px;
    margin-right: 4px;
  }
}
```

---

## 📦 Estrutura de Dados

### TipoDocumento Enum

```typescript
export enum TipoDocumento {
  Prontuario = 1,
  Receita = 2,
  Atestado = 3,
  Laudo = 4,
  Prescricao = 5,
  Encaminhamento = 6
}
```

### Interface AssinaturaDigital

```typescript
export interface AssinaturaDigital {
  id: string;
  documentoId: string;
  tipoDocumento: TipoDocumento;
  tipoDocumentoNome: string;
  medicoId: string;
  medicoNome: string;
  medicoCRM: string;
  certificadoId: string;
  dataHoraAssinatura: Date;
  hashDocumento: string;
  temTimestamp: boolean;
  dataTimestamp?: Date;
  valida: boolean;
  dataUltimaValidacao?: Date;
  localAssinatura: string;
  ipAssinatura: string;
  certificadoSubject: string;
  certificadoExpiracao: Date;
}
```

---

## 🔌 APIs Disponíveis

### Endpoints de Assinatura

```typescript
// 1. Assinar documento
POST /api/assinaturadigital/assinar
{
  "documentoId": "guid",
  "tipoDocumento": 1,
  "documentoBytes": "base64_pdf",
  "senhaCertificado": "opcional"
}

// 2. Validar assinatura
GET /api/assinaturadigital/{id}/validar

// 3. Listar assinaturas de um documento
GET /api/assinaturadigital/documento/{documentoId}?tipoDocumento=1
```

### Endpoints de Certificados

```typescript
// 1. Listar certificados do médico
GET /api/certificadodigital

// 2. Importar certificado A1
POST /api/certificadodigital/a1/importar
(multipart/form-data: arquivo + senha)

// 3. Registrar certificado A3
POST /api/certificadodigital/a3/registrar
{
  "thumbprint": "certificado_thumbprint"
}

// 4. Listar certificados A3 disponíveis
GET /api/certificadodigital/a3/disponiveis
```

---

## 🎨 Indicadores Visuais

### Badge de Status de Assinatura

```html
<!-- Em listagens de documentos -->
<mat-chip-listbox>
  <mat-chip *ngIf="documento.assinado" class="status-assinado">
    <mat-icon>verified</mat-icon>
    Assinado Digitalmente
  </mat-chip>
</mat-chip-listbox>
```

### Tooltip de Informações

```html
<!-- Mostrar informações rápidas ao passar o mouse -->
<button 
  mat-icon-button
  [matTooltip]="getTooltipAssinatura()"
  *ngIf="documento.assinado">
  <mat-icon color="accent">verified</mat-icon>
</button>
```

```typescript
getTooltipAssinatura(): string {
  if (!this.documento.assinatura) return '';
  
  return `Assinado digitalmente por ${this.documento.assinatura.medicoNome}
          em ${this.documento.assinatura.dataHoraAssinatura | date:'dd/MM/yyyy HH:mm'}
          Certificado: ${this.documento.assinatura.certificadoSubject}`;
}
```

---

## ✅ Checklist de Integração

Para cada módulo que você integrar:

- [ ] Importar models e services
- [ ] Adicionar botão "Assinar Digitalmente" no visualizador
- [ ] Implementar método `assinarDigitalmente()`
- [ ] Implementar geração de PDF do documento
- [ ] Adicionar seção de visualização de assinaturas
- [ ] Implementar carregamento de assinaturas existentes
- [ ] Adicionar indicadores visuais (badges, ícones)
- [ ] Testar fluxo completo de assinatura
- [ ] Testar validação de assinaturas
- [ ] Atualizar documentação do módulo

---

## 🧪 Testes

### Cenários de Teste

1. **Assinar documento sem certificado**
   - Deve exibir mensagem pedindo para cadastrar certificado

2. **Assinar documento com certificado A1**
   - Deve solicitar senha
   - Deve assinar com sucesso
   - Deve exibir confirmação

3. **Assinar documento com certificado A3**
   - Deve detectar token conectado
   - Deve assinar com sucesso
   - Pode solicitar PIN do token

4. **Visualizar assinaturas**
   - Deve listar todas as assinaturas do documento
   - Deve exibir status (válida/inválida)
   - Deve mostrar informações do assinante

5. **Revalidar assinatura**
   - Deve validar novamente a assinatura
   - Deve atualizar status

---

## 🚨 Considerações Importantes

### 1. Geração de PDF

**CRÍTICO:** Para que a assinatura digital funcione corretamente, você precisa:

- Gerar PDF do documento de forma **consistente e reproduzível**
- O mesmo documento deve sempre gerar o **mesmo PDF** (byte por byte)
- Caso contrário, a validação de integridade falhará

**Recomendações:**
- Use bibliotecas de geração de PDF determinísticas
- Evite timestamps ou dados dinâmicos no PDF
- Teste a geração múltiplas vezes para garantir consistência

### 2. Armazenamento de Documentos

A validação de assinatura requer acesso ao documento original:

```typescript
// Você pode precisar implementar:
interface IDocumentStorageService {
  getDocumentoBytes(documentoId: string, tipoDocumento: TipoDocumento): Promise<byte[]>;
}
```

Atualmente, a validação verifica a estrutura PKCS#7 e certificado, mas não recalcula o hash do documento armazenado (veja limitações na documentação técnica).

### 3. Permissões

- Apenas **médicos** devem poder assinar documentos
- Verifique permissões antes de exibir o botão de assinatura
- Use guards de rota se necessário

### 4. Estado do Documento

- Considere bloquear edição de documentos já assinados
- Adicione campo `assinado: boolean` no modelo do documento
- Atualize este campo após assinatura bem-sucedida

---

## 📞 Suporte

### Documentação Adicional
- [Documentação Técnica](./ASSINATURA_DIGITAL_DOCUMENTACAO_TECNICA.md)
- [Guia do Usuário](./ASSINATURA_DIGITAL_GUIA_USUARIO.md)
- [Resumo da Implementação](./RESUMO_IMPLEMENTACAO_ASSINATURA_DIGITAL.md)

### Exemplos Completos
Os componentes em `frontend/medicwarehouse-app/src/app/pages/assinatura-digital/` servem como exemplos completos de como usar os services e models.

### Dúvidas?
Consulte os arquivos de service para ver todos os métodos disponíveis:
- `frontend/medicwarehouse-app/src/app/services/assinatura-digital.service.ts`
- `frontend/medicwarehouse-app/src/app/services/certificado-digital.service.ts`

---

## 🎉 Conclusão

A infraestrutura de assinatura digital está pronta e aguardando integração. Os componentes foram projetados para serem:

- ✅ **Standalone** - Podem ser importados em qualquer módulo
- ✅ **Reutilizáveis** - Mesma lógica para todos os tipos de documentos
- ✅ **Fáceis de integrar** - API simples e clara
- ✅ **Totalmente documentados** - Exemplos e guias completos

Basta seguir este guia e adaptar para cada módulo específico!

---

**Versão:** 1.0  
**Data:** 27 de Janeiro de 2026  
**Autor:** Omni Care Software Team
