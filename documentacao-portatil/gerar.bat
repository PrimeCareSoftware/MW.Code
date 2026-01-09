@echo off
:: Script para gerar documentação portátil do PrimeCare Software
:: Pode ser executado de qualquer lugar do repositório

chcp 65001 > nul
cls

echo ╔════════════════════════════════════════════════════════╗
echo ║  📱 Gerador de Documentação Portátil - PrimeCare Software  ║
echo ╚════════════════════════════════════════════════════════╝
echo.

cd /d "%~dp0"

:: Verificar se Node.js está instalado
where node >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ⚠️  Node.js não encontrado. Por favor, instale Node.js 18+ para continuar.
    pause
    exit /b 1
)

:: Verificar se dependências estão instaladas
if not exist "node_modules" (
    echo 📦 Instalando dependências...
    call npm install
    echo.
)

:: Executar o gerador
echo 🚀 Gerando documentação...
echo.
node gerar-documentacao.js

echo.
echo ✅ Concluído!
echo.
echo 📖 Arquivos gerados:
echo    - PrimeCare Software-Documentacao-Completa.md
echo    - PrimeCare Software-Documentacao-Completa.html
echo.
echo 💡 Próximos passos:
echo    1. Abra o arquivo HTML no navegador
echo    2. Para PDF: Ctrl+P ^> Salvar como PDF
echo    3. Para mobile: Transfira o HTML para seu celular
echo.

pause
