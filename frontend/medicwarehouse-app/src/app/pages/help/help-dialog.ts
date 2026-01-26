import { Component, Input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HelpContent } from '../../services/help.service';

@Component({
  selector: 'app-help-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './help-dialog.html',
  styleUrls: ['./help-dialog.scss']
})
export class HelpDialogComponent {
  @Input() content: HelpContent | null = null;
  isVisible = signal(false);

  open(content: HelpContent): void {
    this.content = content;
    this.isVisible.set(true);
  }

  close(): void {
    this.isVisible.set(false);
  }

  openInNewWindow(): void {
    if (!this.content) return;

    const windowFeatures = 'width=900,height=700,scrollbars=yes,resizable=yes';
    const newWindow = window.open('', '_blank', windowFeatures);
    
    if (newWindow) {
      newWindow.document.write(this.generateHelpHTML());
      newWindow.document.close();
    }
  }

  private generateHelpHTML(): string {
    if (!this.content) return '';

    let html = `
      <!DOCTYPE html>
      <html lang="pt-BR">
      <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>${this.content.title}</title>
        <style>
          * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
          }

          body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            background: #f5f5f5;
            padding: 2rem;
          }

          .help-container {
            max-width: 900px;
            margin: 0 auto;
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            padding: 2rem;
          }

          h1 {
            color: #2c5282;
            font-size: 2rem;
            margin-bottom: 1.5rem;
            padding-bottom: 1rem;
            border-bottom: 3px solid #4299e1;
          }

          h2 {
            color: #2d3748;
            font-size: 1.5rem;
            margin-top: 2rem;
            margin-bottom: 1rem;
            padding-left: 0.5rem;
            border-left: 4px solid #4299e1;
          }

          p {
            margin-bottom: 1rem;
            text-align: justify;
          }

          ul, ol {
            margin-left: 2rem;
            margin-bottom: 1rem;
          }

          li {
            margin-bottom: 0.5rem;
          }

          strong {
            color: #2d3748;
            font-weight: 600;
          }

          .test-data {
            background: #f7fafc;
            border: 1px solid #e2e8f0;
            border-radius: 6px;
            padding: 1rem;
            margin: 1rem 0;
          }

          .test-data h3 {
            color: #2c5282;
            font-size: 1.1rem;
            margin-bottom: 0.5rem;
          }

          .test-item {
            background: white;
            border-left: 3px solid #48bb78;
            padding: 0.75rem;
            margin: 0.75rem 0;
            border-radius: 4px;
          }

          .test-item-field {
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 0.25rem;
          }

          .test-item-example {
            color: #48bb78;
            font-family: 'Courier New', monospace;
            margin-bottom: 0.25rem;
          }

          .test-item-description {
            color: #718096;
            font-size: 0.9rem;
          }

          .section {
            margin-bottom: 2rem;
          }

          .print-button {
            background: #4299e1;
            color: white;
            border: none;
            padding: 0.75rem 1.5rem;
            border-radius: 6px;
            font-size: 1rem;
            cursor: pointer;
            margin-top: 2rem;
          }

          .print-button:hover {
            background: #3182ce;
          }

          @media print {
            body {
              background: white;
              padding: 0;
            }

            .help-container {
              box-shadow: none;
            }

            .print-button {
              display: none;
            }
          }
        </style>
      </head>
      <body>
        <div class="help-container">
          <h1>${this.content.title}</h1>
    `;

    this.content.sections.forEach(section => {
      html += `
          <div class="section">
            <h2>${section.title}</h2>
            ${section.content}
      `;

      if (section.testData && section.testData.length > 0) {
        html += `
            <div class="test-data">
              <h3>Exemplos de Dados Válidos para Testes</h3>
        `;
        
        section.testData.forEach(testItem => {
          html += `
              <div class="test-item">
                <div class="test-item-field">${testItem.field}</div>
                <div class="test-item-example">Exemplo: ${testItem.validExample}</div>
                <div class="test-item-description">${testItem.description}</div>
              </div>
          `;
        });
        
        html += `
            </div>
        `;
      }

      html += `
          </div>
      `;
    });

    html += `
          <button class="print-button" onclick="window.print()">Imprimir Ajuda</button>
        </div>
      </body>
      </html>
    `;

    return html;
  }
}
