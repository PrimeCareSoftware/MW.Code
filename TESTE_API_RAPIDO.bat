@echo off
:: Script de Teste Rápido da API PrimeCare Software
:: Este script testa os endpoints principais da API

chcp 65001 > nul
setlocal enabledelayedexpansion

:: Configurações
set "API_URL=http://localhost:5000/api"
set "TENANT_ID=demo-clinic-001"

echo ╔════════════════════════════════════════════════════════╗
echo ║  PrimeCare Software - Teste Rápido de API                 ║
echo ╚════════════════════════════════════════════════════════╝
echo.

:: Verificar se curl está disponível
where curl >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ⚠️  curl não encontrado. Por favor, instale curl para usar este script.
    echo    Download: https://curl.se/windows/
    pause
    exit /b 1
)

:: 1. Verificar informações do seeder
echo [1/8] Verificando informações do seeder...
curl -s -X GET "%API_URL%/data-seeder/demo-info" -H "Content-Type: application/json"
echo.
echo.

:: 2. Popular dados demo
echo [2/8] Populando dados demo...
curl -s -X POST "%API_URL%/data-seeder/seed-demo" -H "Content-Type: application/json"
echo.
echo.

:: 3. Fazer login
echo [3/8] Fazendo login...
set "LOGIN_DATA={\"username\":\"admin\",\"password\":\"Admin@123\",\"tenantId\":\"demo-clinic-001\"}"
curl -s -X POST "%API_URL%/auth/login" -H "Content-Type: application/json" -d "%LOGIN_DATA%" > login_response.tmp

:: Extrair token (usando PowerShell para processar JSON)
for /f "delims=" %%i in ('powershell -Command "try { Get-Content login_response.tmp | ConvertFrom-Json | Select-Object -ExpandProperty token } catch { Write-Output '' }"') do set "TOKEN=%%i"

if "!TOKEN!"=="" (
    echo ✗ Falha no login! Resposta da API:
    type login_response.tmp
    del login_response.tmp
    echo.
    echo Verifique se a API está rodando em %API_URL%
    pause
    exit /b 1
) else (
    echo ✓ Login realizado com sucesso!
    echo Token: !TOKEN:~0,50!...
)
del login_response.tmp
echo.

:: 4. Listar pacientes
echo [4/8] Listando pacientes...
curl -s -X GET "%API_URL%/patients" ^
    -H "Content-Type: application/json" ^
    -H "Authorization: Bearer !TOKEN!" ^
    -H "X-Tenant-Id: %TENANT_ID%"
echo.
echo.

:: 5. Listar agendamentos
echo [5/8] Listando agendamentos...
curl -s -X GET "%API_URL%/appointments" ^
    -H "Content-Type: application/json" ^
    -H "Authorization: Bearer !TOKEN!" ^
    -H "X-Tenant-Id: %TENANT_ID%"
echo.
echo.

:: 6. Listar procedimentos
echo [6/8] Listando procedimentos...
curl -s -X GET "%API_URL%/procedures" ^
    -H "Content-Type: application/json" ^
    -H "Authorization: Bearer !TOKEN!" ^
    -H "X-Tenant-Id: %TENANT_ID%"
echo.
echo.

:: 7. Relatório financeiro
echo [7/8] Obtendo relatório financeiro...
curl -s -X GET "%API_URL%/reports/financial-summary?startDate=2024-01-01&endDate=2024-12-31" ^
    -H "Content-Type: application/json" ^
    -H "Authorization: Bearer !TOKEN!" ^
    -H "X-Tenant-Id: %TENANT_ID%"
echo.
echo.

:: 8. Listar medicamentos
echo [8/8] Listando medicamentos...
curl -s -X GET "%API_URL%/medications" ^
    -H "Content-Type: application/json" ^
    -H "Authorization: Bearer !TOKEN!" ^
    -H "X-Tenant-Id: %TENANT_ID%"
echo.
echo.

echo ╔════════════════════════════════════════════════════════╗
echo ║  Testes Concluídos!                                    ║
echo ╚════════════════════════════════════════════════════════╝
echo.
echo Dica: Acesse http://localhost:5000/swagger para testar mais endpoints!
echo Dica: Use o token acima para autenticar no Swagger (botão Authorize)
echo.
pause
