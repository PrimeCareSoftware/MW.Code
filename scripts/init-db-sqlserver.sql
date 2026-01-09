-- Script de inicialização do banco de dados SQL Server para PrimeCare Software
-- Este script é executado automaticamente quando o container SQL Server é criado

USE master;
GO

-- Criar o banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PrimeCare Software')
BEGIN
    CREATE DATABASE PrimeCare Software;
    PRINT 'Database PrimeCare Software created successfully.';
END
ELSE
BEGIN
    PRINT 'Database PrimeCare Software already exists.';
END
GO

USE PrimeCare Software;
GO

-- Mensagem de confirmação
PRINT 'Database initialized for PrimeCare Software - Sistema de Gestão para Consultórios Médicos';
PRINT 'Ready to accept connections.';
GO
