/**
 * Script de Auditoria de Acessibilidade
 * Gera relatório completo de conformidade WCAG 2.1 AA
 */

const { AxePuppeteer } = require('@axe-core/puppeteer');
const puppeteer = require('puppeteer');
const fs = require('fs');
const path = require('path');

const pagesToAudit = [
  { name: 'Home', url: 'http://localhost:4200' },
  { name: 'Dashboard', url: 'http://localhost:4200/dashboard' },
  { name: 'Patients List', url: 'http://localhost:4200/patients' },
  { name: 'Appointment Form', url: 'http://localhost:4200/appointments/new' },
  { name: 'Medical Records', url: 'http://localhost:4200/medical-records' },
  { name: 'Reports', url: 'http://localhost:4200/reports' }
];

async function auditAccessibility() {
  console.log('🔍 Iniciando auditoria de acessibilidade WCAG 2.1 AA...\n');
  
  const browser = await puppeteer.launch({ 
    headless: 'new',
    args: ['--no-sandbox', '--disable-setuid-sandbox']
  });
  
  const results = [];
  const reportDir = path.join(__dirname, '../../a11y-reports');
  
  // Criar diretório se não existir
  if (!fs.existsSync(reportDir)) {
    fs.mkdirSync(reportDir, { recursive: true });
  }
  
  for (const page of pagesToAudit) {
    console.log(`📄 Auditando: ${page.name} (${page.url})`);
    
    try {
      const browserPage = await browser.newPage();
      await browserPage.setBypassCSP(true);
      
      // Tentar acessar a página
      const response = await browserPage.goto(page.url, { 
        waitUntil: 'networkidle2',
        timeout: 10000 
      }).catch(err => {
        console.log(`   ⚠️  Não foi possível acessar ${page.url}: ${err.message}`);
        return null;
      });
      
      if (!response) {
        await browserPage.close();
        continue;
      }
      
      // Executar análise axe
      const axeResults = await new AxePuppeteer(browserPage)
        .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
        .analyze()
        .catch(err => {
          console.log(`   ⚠️  Erro na análise: ${err.message}`);
          return { violations: [] };
        });
      
      const summary = {
        page: page.name,
        url: page.url,
        violations: axeResults.violations.length,
        critical: axeResults.violations.filter(v => v.impact === 'critical').length,
        serious: axeResults.violations.filter(v => v.impact === 'serious').length,
        moderate: axeResults.violations.filter(v => v.impact === 'moderate').length,
        minor: axeResults.violations.filter(v => v.impact === 'minor').length,
        timestamp: new Date().toISOString()
      };
      
      results.push(summary);
      
      // Salvar detalhes completos
      const detailsFile = path.join(reportDir, `${page.name.replace(/\s/g, '-')}.json`);
      fs.writeFileSync(detailsFile, JSON.stringify(axeResults, null, 2));
      
      console.log(`   ✅ ${summary.violations} violações encontradas (${summary.critical} críticas, ${summary.serious} sérias)\n`);
      
      await browserPage.close();
    } catch (error) {
      console.error(`   ❌ Erro ao auditar ${page.name}:`, error.message, '\n');
    }
  }
  
  await browser.close();
  
  // Gerar relatório HTML
  generateHTMLReport(results, reportDir);
  
  // Gerar relatório JSON
  const summaryFile = path.join(reportDir, 'summary.json');
  fs.writeFileSync(summaryFile, JSON.stringify(results, null, 2));
  
  console.log('✅ Auditoria concluída!');
  console.log(`📊 Relatório HTML: ${path.join(reportDir, 'summary.html')}`);
  console.log(`📊 Relatório JSON: ${summaryFile}\n`);
  
  // Calcular totais
  const totalViolations = results.reduce((sum, r) => sum + r.violations, 0);
  const totalCritical = results.reduce((sum, r) => sum + r.critical, 0);
  const totalSerious = results.reduce((sum, r) => sum + r.serious, 0);
  
  console.log(`📈 Total de Violações: ${totalViolations}`);
  console.log(`   🔴 Críticas: ${totalCritical}`);
  console.log(`   🟠 Sérias: ${totalSerious}`);
  
  // Exit com código de erro se houver violações críticas ou sérias
  if (totalCritical > 0 || totalSerious > 0) {
    console.log('\n❌ Build failed: Critical or serious accessibility violations found');
    process.exit(1);
  }
}

function generateHTMLReport(results, reportDir) {
  const totalViolations = results.reduce((sum, r) => sum + r.violations, 0);
  const totalCritical = results.reduce((sum, r) => sum + r.critical, 0);
  const totalSerious = results.reduce((sum, r) => sum + r.serious, 0);
  const totalModerate = results.reduce((sum, r) => sum + r.moderate, 0);
  const totalMinor = results.reduce((sum, r) => sum + r.minor, 0);
  
  const html = `
    <!DOCTYPE html>
    <html lang="pt-BR">
    <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>Relatório de Acessibilidade WCAG 2.1 AA - Omni Care</title>
      <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { 
          font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, sans-serif;
          margin: 40px;
          background: #f5f5f5;
          color: #333;
        }
        .container { 
          max-width: 1200px; 
          margin: 0 auto;
          background: white;
          padding: 40px;
          border-radius: 8px;
          box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        h1 { 
          color: #1976d2; 
          margin-bottom: 10px;
          font-size: 2.5rem;
        }
        .subtitle {
          color: #666;
          margin-bottom: 30px;
          font-size: 1.1rem;
        }
        .summary-cards {
          display: grid;
          grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
          gap: 20px;
          margin: 30px 0;
        }
        .card {
          padding: 20px;
          border-radius: 8px;
          border: 2px solid #ddd;
        }
        .card h3 {
          font-size: 0.9rem;
          color: #666;
          text-transform: uppercase;
          margin-bottom: 10px;
        }
        .card .value {
          font-size: 2.5rem;
          font-weight: bold;
        }
        .card.total { border-color: #1976d2; background: #e3f2fd; }
        .card.total .value { color: #1976d2; }
        .card.critical { border-color: #d32f2f; background: #ffebee; }
        .card.critical .value { color: #d32f2f; }
        .card.serious { border-color: #f57c00; background: #fff3e0; }
        .card.serious .value { color: #f57c00; }
        .card.moderate { border-color: #fbc02d; background: #fffde7; }
        .card.moderate .value { color: #fbc02d; }
        .card.minor { border-color: #7cb342; background: #f1f8e9; }
        .card.minor .value { color: #7cb342; }
        table { 
          border-collapse: collapse; 
          width: 100%; 
          margin-top: 30px;
          box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        th, td { 
          border: 1px solid #ddd; 
          padding: 16px; 
          text-align: left; 
        }
        th { 
          background-color: #1976d2; 
          color: white;
          font-weight: 600;
          text-transform: uppercase;
          font-size: 0.85rem;
          letter-spacing: 0.5px;
        }
        tr:nth-child(even) { background-color: #f9f9f9; }
        tr:hover { background-color: #f5f5f5; }
        .critical-text { color: #d32f2f; font-weight: bold; }
        .serious-text { color: #f57c00; font-weight: bold; }
        .moderate-text { color: #fbc02d; font-weight: bold; }
        .minor-text { color: #7cb342; }
        .timestamp {
          color: #999;
          font-size: 0.9rem;
          margin-top: 30px;
          padding-top: 20px;
          border-top: 1px solid #ddd;
        }
        .status {
          display: inline-block;
          padding: 8px 16px;
          border-radius: 4px;
          font-weight: bold;
          margin: 20px 0;
        }
        .status.pass { background: #4caf50; color: white; }
        .status.fail { background: #d32f2f; color: white; }
        a { color: #1976d2; text-decoration: none; }
        a:hover { text-decoration: underline; }
      </style>
    </head>
    <body>
      <div class="container">
        <h1>📊 Relatório de Acessibilidade</h1>
        <p class="subtitle">WCAG 2.1 Level AA - Omni Care Software</p>
        
        ${totalCritical === 0 && totalSerious === 0 
          ? '<div class="status pass">✅ CONFORMIDADE APROVADA</div>'
          : '<div class="status fail">❌ CONFORMIDADE NÃO ATENDIDA</div>'
        }
        
        <div class="summary-cards">
          <div class="card total">
            <h3>Total de Violações</h3>
            <div class="value">${totalViolations}</div>
          </div>
          <div class="card critical">
            <h3>Críticas</h3>
            <div class="value">${totalCritical}</div>
          </div>
          <div class="card serious">
            <h3>Sérias</h3>
            <div class="value">${totalSerious}</div>
          </div>
          <div class="card moderate">
            <h3>Moderadas</h3>
            <div class="value">${totalModerate}</div>
          </div>
          <div class="card minor">
            <h3>Menores</h3>
            <div class="value">${totalMinor}</div>
          </div>
        </div>
        
        <h2 style="margin-top: 40px; color: #333;">Resultados por Página</h2>
        <table>
          <thead>
            <tr>
              <th>Página</th>
              <th>Total</th>
              <th>Críticas</th>
              <th>Sérias</th>
              <th>Moderadas</th>
              <th>Menores</th>
              <th>Detalhes</th>
            </tr>
          </thead>
          <tbody>
            ${results.map(r => `
              <tr>
                <td><strong>${r.page}</strong><br><small style="color: #666;">${r.url}</small></td>
                <td><strong>${r.violations}</strong></td>
                <td class="critical-text">${r.critical}</td>
                <td class="serious-text">${r.serious}</td>
                <td class="moderate-text">${r.moderate}</td>
                <td class="minor-text">${r.minor}</td>
                <td><a href="${r.page.replace(/\s/g, '-')}.json" target="_blank">Ver JSON</a></td>
              </tr>
            `).join('')}
          </tbody>
        </table>
        
        <div class="timestamp">
          <strong>Gerado em:</strong> ${new Date().toLocaleString('pt-BR', { 
            dateStyle: 'long', 
            timeStyle: 'medium' 
          })}
        </div>
        
        <div style="margin-top: 30px; padding: 20px; background: #f5f5f5; border-radius: 8px;">
          <h3 style="margin-bottom: 10px;">📚 Referências</h3>
          <ul style="line-height: 2;">
            <li><a href="https://www.w3.org/WAI/WCAG21/quickref/" target="_blank">WCAG 2.1 Quick Reference</a></li>
            <li><a href="https://www.acessibilidade.gov.pt/" target="_blank">Acessibilidade Digital</a></li>
            <li><a href="https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2015/lei/l13146.htm" target="_blank">Lei Brasileira de Inclusão (LBI)</a></li>
          </ul>
        </div>
      </div>
    </body>
    </html>
  `;
  
  const reportFile = path.join(reportDir, 'summary.html');
  fs.writeFileSync(reportFile, html);
}

// Executar auditoria
auditAccessibility().catch(error => {
  console.error('❌ Erro fatal:', error);
  process.exit(1);
});
