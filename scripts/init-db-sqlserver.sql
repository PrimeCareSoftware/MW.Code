-- Script de inicialização do banco de dados SQL Server para MedicWarehouse
-- Este script é executado automaticamente quando o container SQL Server é criado

USE master;
GO

-- Criar o banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MedicWarehouse')
BEGIN
    CREATE DATABASE MedicWarehouse;
    PRINT 'Database MedicWarehouse created successfully.';
END
ELSE
BEGIN
    PRINT 'Database MedicWarehouse already exists.';
END
GO

USE MedicWarehouse;
GO

-- Mensagem de confirmação
PRINT 'Database initialized for MedicWarehouse - Sistema de Gestão para Consultórios Médicos';
PRINT 'Ready to accept connections.';
GO
