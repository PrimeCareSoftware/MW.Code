-- Script de inicialização do PostgreSQL para MedicWarehouse
-- Este script é executado automaticamente quando o container PostgreSQL inicia

-- Criar extensões úteis
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm"; -- Para busca de texto eficiente

-- Configurar timezone
SET timezone = 'America/Sao_Paulo';

-- Log de inicialização
DO $$
BEGIN
    RAISE NOTICE 'MedicWarehouse PostgreSQL inicializado com sucesso!';
    RAISE NOTICE 'Timezone: America/Sao_Paulo';
    RAISE NOTICE 'Aguardando migrations do Entity Framework Core...';
END $$;
