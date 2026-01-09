#!/usr/bin/env node

const fs = require('fs');
const path = require('path');
const { marked } = require('marked');

// Configurações
const REPO_ROOT = path.join(__dirname, '..');
const OUTPUT_DIR = __dirname;
const OUTPUT_MD_FILE = path.join(OUTPUT_DIR, 'PrimeCare Software-Documentacao-Completa.md');
const OUTPUT_HTML_FILE = path.join(OUTPUT_DIR, 'PrimeCare Software-Documentacao-Completa.html');

// Lista completa de arquivos markdown para incluir (todos os arquivos de documentação)
const documentFiles = [
  // Raiz do projeto
  { path: 'README.md', title: '📚 README Principal', category: 'Início' },
  { path: 'frontend/mw-docs/src/assets/docs/README.md', title: '🏠 README Frontend', category: 'Início' },
  
  // Guias (34 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/GUIA_EXECUCAO.md', title: '📖 Guia de Execução', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/GUIA_INICIO_RAPIDO_LOCAL.md', title: '📖 Guia Início Rápido Local', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/GUIA_DESENVOLVIMENTO_AUTH.md', title: '📖 Guia Desenvolvimento Auth', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/GUIA_MULTIPLATAFORMA.md', title: '📖 Guia Multiplataforma', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/GUIA_RAPIDO_TELEMEDICINA.md', title: '📖 Guia Rápido Telemedicina', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/GUIA_TESTES_PASSO_A_PASSO.md', title: '📖 Guia Testes Passo a Passo', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/GUIA_TRADUCAO_CODIGO.md', title: '📖 Guia Tradução Código', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/GUIA_MEDICO_CFM_1821.md', title: '📖 Guia Médico CFM 1821', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/SYSTEM_SETUP_GUIDE.md', title: '📖 System Setup Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/API_QUICK_GUIDE.md', title: '📖 API Quick Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/AUTHENTICATION_GUIDE.md', title: '📖 Authentication Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/POSTMAN_QUICK_GUIDE.md', title: '📖 Postman Quick Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/POSTMAN_IMPORT_GUIDE.md', title: '📖 Postman Import Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/SEEDER_GUIDE.md', title: '📖 Seeder Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/SEEDER_QUICK_REFERENCE.md', title: '📖 Seeder Quick Reference', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/QUICK_START_PRODUCTION.md', title: '📖 Quick Start Production', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/QUICK_REFERENCE_PERMISSIONS.md', title: '📖 Quick Reference Permissions', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/DATABASE_MIGRATION_GUIDE.md', title: '📖 Database Migration Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/DEPLOY_RAILWAY_GUIDE.md', title: '📖 Deploy Railway Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/DOCKER_POSTGRES_SETUP.md', title: '📖 Docker Postgres Setup', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/PODMAN_POSTGRES_SETUP.md', title: '📖 Podman Postgres Setup', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/DOCKER_TO_PODMAN_MIGRATION.md', title: '📖 Docker to Podman Migration', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/SESSION_MANAGEMENT_GUIDE.md', title: '📖 Session Management Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/SUBDOMAIN_LOGIN_GUIDE.md', title: '📖 Subdomain Login Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/SUBDOMAIN_DOMAIN_CONFIGURATION.md', title: '📖 Subdomain Domain Configuration', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/SUBDOMAIN_USAGE_EXAMPLES.md', title: '📖 Subdomain Usage Examples', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/MULTI_CLINIC_OWNERSHIP_GUIDE.md', title: '📖 Multi Clinic Ownership Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/MOBILE_APPS_GUIDE.md', title: '📖 Mobile Apps Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/HEALTH_INSURANCE_INTEGRATION_GUIDE.md', title: '📖 Health Insurance Integration Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/WAITING_QUEUE_GUIDE.md', title: '📖 Waiting Queue Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/MOCK_DATA_GUIDE.md', title: '📖 Mock Data Guide', category: 'Guias' },
  { path: 'frontend/mw-docs/src/assets/docs/TESTING_MOCK_DATA.md', title: '📖 Testing Mock Data', category: 'Guias' },
  
  // Interface (8 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/SCREENS_DOCUMENTATION.md', title: '📱 Screens Documentation', category: 'Interface' },
  { path: 'frontend/mw-docs/src/assets/docs/SCREENSHOTS_DOCUMENTATION.md', title: '📱 Screenshots Documentation', category: 'Interface' },
  { path: 'frontend/mw-docs/src/assets/docs/TELAS_COM_FLUXO.md', title: '📱 Telas com Fluxo', category: 'Interface' },
  { path: 'frontend/mw-docs/src/assets/docs/VISUAL_FLOW_SUMMARY.md', title: '📱 Visual Flow Summary', category: 'Interface' },
  { path: 'frontend/mw-docs/src/assets/docs/VISUAL_DOCUMENTATION_INDEX.md', title: '📱 Visual Documentation Index', category: 'Interface' },
  { path: 'frontend/mw-docs/src/assets/docs/INPUT_MASKS_DOCUMENTATION.md', title: '📱 Input Masks Documentation', category: 'Interface' },
  { path: 'frontend/mw-docs/src/assets/docs/RICH_TEXT_EDITOR_AUTOCOMPLETE.md', title: '📱 Rich Text Editor Autocomplete', category: 'Interface' },
  { path: 'frontend/mw-docs/src/assets/docs/APPLE_DESIGN_SYSTEM.md', title: '📱 Apple Design System', category: 'Interface' },
  
  // Negócio (3 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/BUSINESS_RULES.md', title: '📋 Business Rules', category: 'Negócio' },
  { path: 'frontend/mw-docs/src/assets/docs/ORDEM_CORRETA_CADASTRO.md', title: '📋 Ordem Correta Cadastro', category: 'Negócio' },
  { path: 'frontend/mw-docs/src/assets/docs/FLUXO_COMPLETO_SISTEMA.md', title: '📋 Fluxo Completo Sistema', category: 'Negócio' },
  
  // Técnica (6 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/TECHNICAL_IMPLEMENTATION.md', title: '🔧 Technical Implementation', category: 'Técnica' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION.md', title: '🔧 Implementation', category: 'Técnica' },
  { path: 'frontend/mw-docs/src/assets/docs/SERVICE_LAYER_ARCHITECTURE.md', title: '🔧 Service Layer Architecture', category: 'Técnica' },
  { path: 'frontend/mw-docs/src/assets/docs/BEFORE_AND_AFTER_ARCHITECTURE.md', title: '🔧 Before and After Architecture', category: 'Técnica' },
  { path: 'frontend/mw-docs/src/assets/docs/CODE_ANALYSIS_FINAL_REPORT.md', title: '🔧 Code Analysis Final Report', category: 'Técnica' },
  { path: 'frontend/mw-docs/src/assets/docs/ENTITY_DIAGRAM.md', title: '🔧 Entity Diagram', category: 'Técnica' },
  
  // CI/CD (7 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/CI_CD_DOCUMENTATION.md', title: '🔄 CI/CD Documentation', category: 'CI/CD' },
  { path: 'frontend/mw-docs/src/assets/docs/TEST_SUMMARY.md', title: '🔄 Test Summary', category: 'CI/CD' },
  { path: 'frontend/mw-docs/src/assets/docs/SECURITY_VALIDATIONS.md', title: '🔄 Security Validations', category: 'CI/CD' },
  { path: 'frontend/mw-docs/src/assets/docs/SONARCLOUD_SETUP.md', title: '🔄 SonarCloud Setup', category: 'CI/CD' },
  { path: 'frontend/mw-docs/src/assets/docs/SONARCLOUD_CONFIGURATION_ISSUES.md', title: '🔄 SonarCloud Configuration Issues', category: 'CI/CD' },
  { path: 'frontend/mw-docs/src/assets/docs/CHECKLIST_TESTES_COMPLETO.md', title: '🔄 Checklist Testes Completo', category: 'CI/CD' },
  { path: 'frontend/mw-docs/src/assets/docs/CARGA_INICIAL_TESTES.md', title: '🔄 Carga Inicial Testes', category: 'CI/CD' },
  
  // Implementação (6 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_SUMMARY.md', title: '✨ Implementation Summary', category: 'Implementação' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_NEW_FEATURES.md', title: '✨ Implementation New Features', category: 'Implementação' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_SUMMARY_BUSINESS_RULES.md', title: '✨ Implementation Summary Business Rules', category: 'Implementação' },
  { path: 'frontend/mw-docs/src/assets/docs/MIGRATION_IMPLEMENTATION_SUMMARY.md', title: '✨ Migration Implementation Summary', category: 'Implementação' },
  { path: 'frontend/mw-docs/src/assets/docs/FUNCIONALIDADES_IMPLEMENTADAS.md', title: '✨ Funcionalidades Implementadas', category: 'Implementação' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTACAO_FECHAMENTO_CONSULTA.md', title: '✨ Implementação Fechamento Consulta', category: 'Implementação' },
  
  // Segurança (6 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/SECURITY_GUIDE.md', title: '🔐 Security Guide', category: 'Segurança' },
  { path: 'frontend/mw-docs/src/assets/docs/SECURITY_IMPLEMENTATION_SUMMARY.md', title: '🔐 Security Implementation Summary', category: 'Segurança' },
  { path: 'frontend/mw-docs/src/assets/docs/SECURITY_CODE_QUALITY_ANALYSIS.md', title: '🔐 Security Code Quality Analysis', category: 'Segurança' },
  { path: 'frontend/mw-docs/src/assets/docs/SUGESTOES_MELHORIAS_SEGURANCA.md', title: '🔐 Sugestões Melhorias Segurança', category: 'Segurança' },
  { path: 'frontend/mw-docs/src/assets/docs/LGPD_COMPLIANCE_DOCUMENTATION.md', title: '🔐 LGPD Compliance Documentation', category: 'Segurança' },
  { path: 'frontend/mw-docs/src/assets/docs/ANALISE_SEGURANCA_CFM_1821.md', title: '🔐 Análise Segurança CFM 1821', category: 'Segurança' },
  
  // Pagamentos (2 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_PAYMENT_SYSTEM.md', title: '💰 Implementation Payment System', category: 'Pagamentos' },
  { path: 'frontend/mw-docs/src/assets/docs/PAYMENT_FLOW.md', title: '💰 Payment Flow', category: 'Pagamentos' },
  
  // Financeiro (2 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/FINANCIAL_REPORTS_DOCUMENTATION.md', title: '📊 Financial Reports Documentation', category: 'Financeiro' },
  { path: 'frontend/mw-docs/src/assets/docs/CALCULADORA_CUSTOS.md', title: '📊 Calculadora Custos', category: 'Financeiro' },
  
  // Assinaturas (2 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/SUBSCRIPTION_SYSTEM.md', title: '💳 Subscription System', category: 'Assinaturas' },
  { path: 'frontend/mw-docs/src/assets/docs/SUBSCRIPTION_PLANS_MANAGEMENT.md', title: '💳 Subscription Plans Management', category: 'Assinaturas' },
  
  // WhatsApp AI (3 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/WHATSAPP_AI_AGENT_DOCUMENTATION.md', title: '🤖 WhatsApp AI Agent Documentation', category: 'WhatsApp AI' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_WHATSAPP_AI_AGENT.md', title: '🤖 Implementation WhatsApp AI Agent', category: 'WhatsApp AI' },
  { path: 'frontend/mw-docs/src/assets/docs/WHATSAPP_AI_AGENT_SECURITY.md', title: '🤖 WhatsApp AI Agent Security', category: 'WhatsApp AI' },
  
  // Notificações (3 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/NOTIFICATION_ROUTINES_DOCUMENTATION.md', title: '🔔 Notification Routines Documentation', category: 'Notificações' },
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_NOTIFICATION_ROUTINES.md', title: '🔔 Implementation Notification Routines', category: 'Notificações' },
  { path: 'frontend/mw-docs/src/assets/docs/NOTIFICATION_ROUTINES_EXAMPLE.md', title: '🔔 Notification Routines Example', category: 'Notificações' },
  
  // Recursos (4 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/IMPLEMENTATION_GUARDIAN_CHILD.md', title: '⚡ Implementation Guardian Child', category: 'Recursos' },
  { path: 'frontend/mw-docs/src/assets/docs/PATIENT_HISTORY_API.md', title: '⚡ Patient History API', category: 'Recursos' },
  { path: 'frontend/mw-docs/src/assets/docs/PATIENT_CONSULTATION_IMPROVEMENTS.md', title: '⚡ Patient Consultation Improvements', category: 'Recursos' },
  { path: 'frontend/mw-docs/src/assets/docs/APPOINTMENT_CALENDAR_FEATURES.md', title: '⚡ Appointment Calendar Features', category: 'Recursos' },
  
  // Marketing (2 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/MW_SITE_DOCUMENTATION.md', title: '🌐 MW Site Documentation', category: 'Marketing' },
  { path: 'frontend/mw-docs/src/assets/docs/MW_SITE_IMPLEMENTATION_SUMMARY.md', title: '🌐 MW Site Implementation Summary', category: 'Marketing' },
  
  // Administração (7 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/SYSTEM_ADMIN_AREA_GUIDE.md', title: '👥 System Admin Area Guide', category: 'Administração' },
  { path: 'frontend/mw-docs/src/assets/docs/SYSTEM_ADMIN_DOCUMENTATION.md', title: '👥 System Admin Documentation', category: 'Administração' },
  { path: 'frontend/mw-docs/src/assets/docs/SYSTEM_OWNER_ACCESS.md', title: '👥 System Owner Access', category: 'Administração' },
  { path: 'frontend/mw-docs/src/assets/docs/SYSTEM_MAPPING.md', title: '👥 System Mapping', category: 'Administração' },
  { path: 'frontend/mw-docs/src/assets/docs/OWNER_DASHBOARD_PERMISSIONS.md', title: '👥 Owner Dashboard Permissions', category: 'Administração' },
  { path: 'frontend/mw-docs/src/assets/docs/OWNER_FLOW_DOCUMENTATION.md', title: '👥 Owner Flow Documentation', category: 'Administração' },
  { path: 'frontend/mw-docs/src/assets/docs/ACCESS_PROFILES_DOCUMENTATION.md', title: '👥 Access Profiles Documentation', category: 'Administração' },
  
  // Frontend (3 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/FRONTEND_INTEGRATION_GUIDE.md', title: '💻 Frontend Integration Guide', category: 'Frontend' },
  { path: 'frontend/mw-docs/src/assets/docs/FRONTEND_API_CONFIGURATION.md', title: '💻 Frontend API Configuration', category: 'Frontend' },
  { path: 'frontend/mw-docs/src/assets/docs/FRONTEND_TELEMEDICINE_INTEGRATION.md', title: '💻 Frontend Telemedicine Integration', category: 'Frontend' },
  
  // Telemedicina (2 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/TELEMEDICINE_COMPONENT_EXAMPLE.md', title: '🏥 Telemedicine Component Example', category: 'Telemedicina' },
  { path: 'frontend/mw-docs/src/assets/docs/TELEMEDICINE_VIDEO_SERVICES_ANALYSIS.md', title: '🏥 Telemedicine Video Services Analysis', category: 'Telemedicina' },
  
  // Infraestrutura (6 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/INFRA_DOCS_INDEX.md', title: '🏗️ Infra Docs Index', category: 'Infraestrutura' },
  { path: 'frontend/mw-docs/src/assets/docs/INFRA_PRODUCAO_BAIXO_CUSTO.md', title: '🏗️ Infra Produção Baixo Custo', category: 'Infraestrutura' },
  { path: 'frontend/mw-docs/src/assets/docs/GITHUB_PAGES_SETUP_REQUIRED.md', title: '🏗️ GitHub Pages Setup Required', category: 'Infraestrutura' },
  { path: 'frontend/mw-docs/src/assets/docs/MEDIATR_LICENSE_CONFIGURATION.md', title: '🏗️ MediatR License Configuration', category: 'Infraestrutura' },
  { path: 'frontend/mw-docs/src/assets/docs/MIGRACAO_POSTGRESQL.md', title: '🏗️ Migração PostgreSQL', category: 'Infraestrutura' },
  { path: 'frontend/mw-docs/src/assets/docs/TRANSACOES_BANCO_DADOS.md', title: '🏗️ Transações Banco Dados', category: 'Infraestrutura' },
  
  // Tickets (4 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/TICKETING_SYSTEM.md', title: '🎫 Ticketing System', category: 'Tickets' },
  { path: 'frontend/mw-docs/src/assets/docs/TICKET_API_DOCUMENTATION.md', title: '🎫 Ticket API Documentation', category: 'Tickets' },
  { path: 'frontend/mw-docs/src/assets/docs/TICKET_CLINIC_OWNER_ACCESS.md', title: '🎫 Ticket Clinic Owner Access', category: 'Tickets' },
  { path: 'frontend/mw-docs/src/assets/docs/TICKET_MIGRATION_GUIDE.md', title: '🎫 Ticket Migration Guide', category: 'Tickets' },
  
  // CFM 1821 (4 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/CFM_1821_IMPLEMENTACAO.md', title: '⚖️ CFM 1821 Implementação', category: 'CFM 1821' },
  { path: 'frontend/mw-docs/src/assets/docs/ESPECIFICACAO_CFM_1821.md', title: '⚖️ Especificação CFM 1821', category: 'CFM 1821' },
  { path: 'frontend/mw-docs/src/assets/docs/API_EXAMPLES_CFM_1821.md', title: '⚖️ API Examples CFM 1821', category: 'CFM 1821' },
  { path: 'frontend/mw-docs/src/assets/docs/API_CONTROLLERS_REPOSITORY_ACCESS_ANALYSIS.md', title: '⚖️ API Controllers Repository Access Analysis', category: 'CFM 1821' },
  
  // Planejamento (5 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/PLANO_DESENVOLVIMENTO.md', title: '📅 Plano Desenvolvimento', category: 'Planejamento' },
  { path: 'frontend/mw-docs/src/assets/docs/PLANO_DESENVOLVIMENTO_6_MESES.md', title: '📅 Plano Desenvolvimento 6 Meses', category: 'Planejamento' },
  { path: 'frontend/mw-docs/src/assets/docs/PENDING_TASKS.md', title: '📅 Pending Tasks', category: 'Planejamento' },
  { path: 'frontend/mw-docs/src/assets/docs/APPS_PENDING_TASKS.md', title: '📅 Apps Pending Tasks', category: 'Planejamento' },
  { path: 'frontend/mw-docs/src/assets/docs/PROMPTS_COPILOT_DESENVOLVIMENTO.md', title: '📅 Prompts Copilot Desenvolvimento', category: 'Planejamento' },
  
  // Análise (4 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/ANALISE_MELHORIAS_SISTEMA.md', title: '📊 Análise Melhorias Sistema', category: 'Análise' },
  { path: 'frontend/mw-docs/src/assets/docs/RESUMO_ANALISE_MELHORIAS.md', title: '📊 Resumo Análise Melhorias', category: 'Análise' },
  { path: 'frontend/mw-docs/src/assets/docs/RESUMO_SISTEMA_COMPLETO.md', title: '📊 Resumo Sistema Completo', category: 'Análise' },
  { path: 'frontend/mw-docs/src/assets/docs/DOCUMENTATION_CLEANUP_SUMMARY.md', title: '📊 Documentation Cleanup Summary', category: 'Análise' },
  
  // Referência (3 arquivos)
  { path: 'frontend/mw-docs/src/assets/docs/DOCUMENTATION_INDEX.md', title: '📚 Documentation Index', category: 'Referência' },
  { path: 'frontend/mw-docs/src/assets/docs/GLOSSARIO_TERMOS_EMPRESARIAIS.md', title: '📚 Glossário Termos Empresariais', category: 'Referência' },
  { path: 'frontend/mw-docs/src/assets/docs/PASSWORD_RECOVERY_FLOW.md', title: '📚 Password Recovery Flow', category: 'Referência' },
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
  
  let markdown = `# PrimeCare Software - Documentação Completa\n\n`;
  markdown += `> **Data de Geração:** ${new Date().toLocaleString('pt-BR')}\n\n`;
  markdown += `> Este documento contém toda a documentação do projeto PrimeCare Software consolidada em um único arquivo.\n\n`;
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
  markdown += `**PrimeCare Software**\n`;
  markdown += `- Email: contato@medicwarehouse.com\n`;
  markdown += `- GitHub: https://github.com/PrimeCare Software/MW.Code\n\n`;
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
    <title>PrimeCare Software - Documentação Completa</title>
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
