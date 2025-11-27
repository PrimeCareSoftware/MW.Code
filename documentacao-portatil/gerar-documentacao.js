#!/usr/bin/env node

const fs = require('fs');
const path = require('path');
const { marked } = require('marked');

// Configurações
const REPO_ROOT = path.join(__dirname, '..');
const OUTPUT_DIR = __dirname;
const OUTPUT_MD_FILE = path.join(OUTPUT_DIR, 'MedicWarehouse-Documentacao-Completa.md');
const OUTPUT_HTML_FILE = path.join(OUTPUT_DIR, 'MedicWarehouse-Documentacao-Completa.html');

// Lista de arquivos markdown para incluir (baseado no documentation.service.ts)
const documentFiles = [
  // Raiz do projeto
  { path: 'README.md', title: '📚 README Principal', category: 'Início' },
  
  // Documentação essencial em docs/
  { path: 'docs/AUTHENTICATION_GUIDE.md', title: '🔐 Guia de Autenticação', category: 'Guias' },
  { path: 'docs/GLOSSARIO_TERMOS_EMPRESARIAIS.md', title: '📖 Glossário de Termos Empresariais', category: 'Referência' },
  { path: 'docs/DOCUMENTATION_INDEX.md', title: '📚 Índice de Documentação', category: 'Referência' },
  
  // Frontend mw-docs
  { path: 'frontend/mw-docs/src/assets/docs/README.md', title: '📖 README', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/GUIA_EXECUCAO.md', title: '🚀 Guia de Execução', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/SYSTEM_SETUP_GUIDE.md', title: '⚙️ Guia de Setup do Sistema', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/API_QUICK_GUIDE.md', title: '⚡ API Quick Guide', category: 'Guias' },
  
  // Documentação de interface
  { path: 'frontend/mw-docs/src/assets/docs/SCREENS_DOCUMENTATION.md', title: '📱 Documentação de Telas', category: 'Interface' },
  { path: 'frontend/mw-docs/src/assets/docs/docs/VISUAL_FLOW_SUMMARY.md', title: '🔄 Resumo Visual de Fluxos', category: 'Interface' },
  
  // Regras de negócio
  { path: 'frontend/mw-docs/src/assets/docs/BUSINESS_RULES.md', title: '📋 Regras de Negócio', category: 'Negócio' },
  
  // Implementação técnica
  { path: 'frontend/mw-docs/src/assets/docs/TECHNICAL_IMPLEMENTATION.md', title: '🔧 Implementação Técnica', category: 'Técnica' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION.md', title: '⚙️ Implementação', category: 'Técnica' },
  
  // CI/CD e Qualidade
  { path: 'frontend/mw-docs/src/assets/docs/CI_CD_DOCUMENTATION.md', title: '🔄 CI/CD', category: 'CI/CD' },
  { path: 'frontend/mw-docs/src/assets/docs/TEST_SUMMARY.md', title: '🧪 Resumo de Testes', category: 'CI/CD' },
  { path: 'frontend/mw-docs/src/assets/docs/SECURITY_VALIDATIONS.md', title: '🔒 Validações de Segurança', category: 'CI/CD' },
  { path: 'frontend/mw-docs/src/assets/docs/SONARCLOUD_SETUP.md', title: '📊 SonarCloud Setup', category: 'CI/CD' },
  
  // Resumos de implementação
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_SUMMARY.md', title: '📝 Resumo de Implementação', category: 'Implementação' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_NEW_FEATURES.md', title: '✨ Novas Funcionalidades', category: 'Implementação' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_SUMMARY_BUSINESS_RULES.md', title: '📋 Resumo de Regras de Negócio', category: 'Implementação' },
  { path: 'frontend/mw-docs/src/assets/docs/MIGRATION_IMPLEMENTATION_SUMMARY.md', title: '🔄 Resumo de Migrações', category: 'Implementação' },
  
  // Segurança
  { path: 'frontend/mw-docs/src/assets/docs/SECURITY_GUIDE.md', title: '🔐 Guia de Segurança', category: 'Segurança' },
  { path: 'frontend/mw-docs/src/assets/docs/SECURITY_IMPLEMENTATION_SUMMARY.md', title: '🔐 Resumo de Implementação de Segurança', category: 'Segurança' },
  
  // Pagamentos
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_PAYMENT_SYSTEM.md', title: '💰 Sistema de Pagamentos', category: 'Pagamentos' },
  { path: 'frontend/mw-docs/src/assets/docs/PAYMENT_FLOW.md', title: '💳 Fluxo de Pagamentos', category: 'Pagamentos' },
  
  // Financeiro
  { path: 'frontend/mw-docs/src/assets/docs/FINANCIAL_REPORTS_DOCUMENTATION.md', title: '📊 Gestão Financeira', category: 'Financeiro' },
  
  // Assinaturas
  { path: 'frontend/mw-docs/src/assets/docs/SUBSCRIPTION_SYSTEM.md', title: '💳 Sistema de Assinaturas', category: 'Assinaturas' },
  
  // WhatsApp AI
  { path: 'frontend/mw-docs/src/assets/docs/WHATSAPP_AI_AGENT_DOCUMENTATION.md', title: '🤖 WhatsApp AI Agent', category: 'WhatsApp AI' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_WHATSAPP_AI_AGENT.md', title: '🤖 Implementação WhatsApp AI', category: 'WhatsApp AI' },
  { path: 'frontend/mw-docs/src/assets/docs/WHATSAPP_AI_AGENT_SECURITY.md', title: '🔐 Segurança WhatsApp AI', category: 'WhatsApp AI' },
  
  // Notificações
  { path: 'frontend/mw-docs/src/assets/docs/NOTIFICATION_ROUTINES_DOCUMENTATION.md', title: '🔔 Documentação de Notificações', category: 'Notificações' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_NOTIFICATION_ROUTINES.md', title: '🔔 Implementação de Notificações', category: 'Notificações' },
  { path: 'frontend/mw-docs/src/assets/docs/NOTIFICATION_ROUTINES_EXAMPLE.md', title: '🔔 Exemplos de Notificações', category: 'Notificações' },
  
  // Recursos especiais
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_GUARDIAN_CHILD.md', title: '👨‍👩‍👧 Sistema Responsável/Dependente', category: 'Recursos' },
  
  // MW.Site
  { path: 'frontend/mw-docs/src/assets/docs/MW_SITE_DOCUMENTATION.md', title: '🌐 MW.Site Documentação', category: 'Marketing' },
  { path: 'frontend/mw-docs/src/assets/docs/MW_SITE_IMPLEMENTATION_SUMMARY.md', title: '🌐 MW.Site Implementação', category: 'Marketing' },
  
  // Índice
  { path: 'frontend/mw-docs/src/assets/docs/docs/INDEX.md', title: '📚 Índice', category: 'Referência' },
];

// Função para ler arquivo com fallback
function readFileWithFallback(filePath) {
  const fullPath = path.join(REPO_ROOT, filePath);
  try {
    if (fs.existsSync(fullPath)) {
      return fs.readFileSync(fullPath, 'utf8');
    } else {
      console.warn(`⚠️  Arquivo não encontrado: ${filePath}`);
      return null;
    }
  } catch (error) {
    console.error(`❌ Erro ao ler ${filePath}:`, error.message);
    return null;
  }
}

// Função para gerar o markdown consolidado
function gerarMarkdownConsolidado() {
  console.log('📝 Gerando documentação consolidada em Markdown...\n');
  
  let markdown = `# MedicWarehouse - Documentação Completa\n\n`;
  markdown += `> **Data de Geração:** ${new Date().toLocaleString('pt-BR')}\n\n`;
  markdown += `> Este documento contém toda a documentação do projeto MedicWarehouse consolidada em um único arquivo.\n\n`;
  markdown += `---\n\n`;
  
  // Gerar índice
  markdown += `## 📑 Índice\n\n`;
  let currentCategory = '';
  documentFiles.forEach((doc, index) => {
    if (doc.category !== currentCategory) {
      currentCategory = doc.category;
      markdown += `\n### ${currentCategory}\n\n`;
    }
    markdown += `${index + 1}. [${doc.title}](#doc-${index + 1})\n`;
  });
  markdown += `\n---\n\n`;
  
  // Adicionar conteúdo de cada arquivo
  documentFiles.forEach((doc, index) => {
    console.log(`📄 Processando: ${doc.title} (${doc.path})`);
    
    const content = readFileWithFallback(doc.path);
    if (content) {
      markdown += `<div id="doc-${index + 1}"></div>\n\n`;
      markdown += `# ${doc.title}\n\n`;
      markdown += `> **Categoria:** ${doc.category}\n`;
      markdown += `> **Arquivo:** \`${doc.path}\`\n\n`;
      markdown += `---\n\n`;
      markdown += content;
      markdown += `\n\n---\n\n`;
      markdown += `<div style="page-break-after: always;"></div>\n\n`;
    }
  });
  
  // Adicionar rodapé
  markdown += `\n\n---\n\n`;
  markdown += `## 📞 Informações de Contato\n\n`;
  markdown += `**MedicWarehouse**\n`;
  markdown += `- Email: contato@medicwarehouse.com\n`;
  markdown += `- GitHub: https://github.com/MedicWarehouse/MW.Code\n\n`;
  markdown += `---\n\n`;
  markdown += `*Documentação gerada automaticamente em ${new Date().toLocaleString('pt-BR')}*\n`;
  
  return markdown;
}

// Função para gerar HTML
function gerarHTML(markdownContent) {
  console.log('\n🌐 Gerando versão HTML...\n');
  
  const htmlContent = marked.parse(markdownContent);
  
  const html = `<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>MedicWarehouse - Documentação Completa</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
            background: #f5f5f5;
        }
        
        .container {
            background: white;
            padding: 40px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        h1 {
            color: #667eea;
            border-bottom: 3px solid #764ba2;
            padding-bottom: 10px;
            margin-bottom: 20px;
            font-size: 2.5em;
        }
        
        h2 {
            color: #667eea;
            margin-top: 30px;
            margin-bottom: 15px;
            font-size: 2em;
        }
        
        h3 {
            color: #764ba2;
            margin-top: 20px;
            margin-bottom: 10px;
            font-size: 1.5em;
        }
        
        h4 {
            color: #555;
            margin-top: 15px;
            margin-bottom: 8px;
        }
        
        blockquote {
            border-left: 4px solid #667eea;
            padding-left: 15px;
            margin: 20px 0;
            color: #666;
            background: #f8f9fa;
            padding: 10px 15px;
            border-radius: 4px;
        }
        
        code {
            background: #f4f4f4;
            padding: 2px 6px;
            border-radius: 3px;
            font-family: 'Courier New', monospace;
            font-size: 0.9em;
        }
        
        pre {
            background: #2d2d2d;
            color: #f8f8f2;
            padding: 15px;
            border-radius: 5px;
            overflow-x: auto;
            margin: 15px 0;
        }
        
        pre code {
            background: transparent;
            color: inherit;
            padding: 0;
        }
        
        table {
            border-collapse: collapse;
            width: 100%;
            margin: 20px 0;
        }
        
        th, td {
            border: 1px solid #ddd;
            padding: 12px;
            text-align: left;
        }
        
        th {
            background: #667eea;
            color: white;
        }
        
        tr:nth-child(even) {
            background: #f9f9f9;
        }
        
        a {
            color: #667eea;
            text-decoration: none;
        }
        
        a:hover {
            text-decoration: underline;
        }
        
        hr {
            border: none;
            border-top: 2px solid #e0e0e0;
            margin: 30px 0;
        }
        
        ul, ol {
            margin: 15px 0;
            padding-left: 30px;
        }
        
        li {
            margin: 8px 0;
        }
        
        img {
            max-width: 100%;
            height: auto;
            border-radius: 5px;
            margin: 15px 0;
        }
        
        .page-break {
            page-break-after: always;
        }
        
        @media print {
            body {
                background: white;
            }
            
            .container {
                box-shadow: none;
                padding: 0;
            }
            
            .page-break {
                page-break-after: always;
            }
        }
        
        @media (max-width: 768px) {
            body {
                padding: 10px;
            }
            
            .container {
                padding: 20px;
            }
            
            h1 {
                font-size: 2em;
            }
            
            h2 {
                font-size: 1.5em;
            }
            
            pre {
                font-size: 0.85em;
            }
        }
    </style>
</head>
<body>
    <div class="container">
        ${htmlContent}
    </div>
    
    <script>
        // Melhorar links internos
        document.addEventListener('DOMContentLoaded', function() {
            // Smooth scroll para âncoras
            document.querySelectorAll('a[href^="#"]').forEach(anchor => {
                anchor.addEventListener('click', function (e) {
                    e.preventDefault();
                    const target = document.querySelector(this.getAttribute('href'));
                    if (target) {
                        target.scrollIntoView({
                            behavior: 'smooth',
                            block: 'start'
                        });
                    }
                });
            });
        });
    </script>
</body>
</html>`;
  
  return html;
}

// Função principal
function main() {
  console.log('🚀 Iniciando geração da documentação consolidada...\n');
  console.log('=' .repeat(60));
  
  try {
    // Gerar markdown consolidado
    const markdownContent = gerarMarkdownConsolidado();
    fs.writeFileSync(OUTPUT_MD_FILE, markdownContent, 'utf8');
    console.log(`\n✅ Markdown gerado: ${OUTPUT_MD_FILE}`);
    
    // Gerar HTML
    const htmlContent = gerarHTML(markdownContent);
    fs.writeFileSync(OUTPUT_HTML_FILE, htmlContent, 'utf8');
    console.log(`✅ HTML gerado: ${OUTPUT_HTML_FILE}`);
    
    // Estatísticas
    const stats = fs.statSync(OUTPUT_MD_FILE);
    const htmlStats = fs.statSync(OUTPUT_HTML_FILE);
    
    console.log('\n' + '='.repeat(60));
    console.log('\n📊 Estatísticas:');
    console.log(`   - Documentos processados: ${documentFiles.length}`);
    console.log(`   - Tamanho Markdown: ${(stats.size / 1024).toFixed(2)} KB`);
    console.log(`   - Tamanho HTML: ${(htmlStats.size / 1024).toFixed(2)} KB`);
    
    console.log('\n✨ Geração concluída com sucesso!');
    console.log('\n📖 Para visualizar:');
    console.log(`   - Abra o arquivo HTML no navegador: ${OUTPUT_HTML_FILE}`);
    console.log('   - Para gerar PDF: Abra o HTML no navegador e use Ctrl+P (Imprimir > Salvar como PDF)');
    console.log('\n💡 O arquivo HTML é otimizado para leitura em celular e pode ser convertido em PDF facilmente.');
    
  } catch (error) {
    console.error('\n❌ Erro ao gerar documentação:', error.message);
    process.exit(1);
  }
}

// Executar
main();
