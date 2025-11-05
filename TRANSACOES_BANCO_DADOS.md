# Implementação de Gerenciamento de Transações de Banco de Dados

## Resumo Executivo

Este documento descreve as alterações implementadas para garantir que todos os métodos que realizam operações no banco de dados façam rollback automático em caso de erro, evitando dados incompletos e garantindo a integridade dos dados.

## Problema Identificado

A aplicação não tinha gerenciamento explícito de transações para operações que modificavam múltiplos registros no banco de dados. Isso poderia causar:

- **Dados incompletos**: Se um erro ocorresse após a primeira operação de banco de dados, a primeira operação seria confirmada enquanto as subsequentes falhariam
- **Inconsistência de dados**: Entidades relacionadas poderiam ficar dessincronizadas
- **Dificuldade de depuração**: Sem rollback adequado, era difícil rastrear o estado dos dados após operações falhadas

## Solução Implementada

### 1. Suporte a Transações no Repositório Base

Adicionados dois novos métodos na interface `IRepository<T>` e implementação `BaseRepository<T>`:

```csharp
Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation);
Task ExecuteInTransactionAsync(Func<Task> operation);
```

**Características Principais:**
- **Rollback automático**: Se qualquer exceção ocorrer, todas as alterações são revertidas
- **Commit automático**: Se a operação for bem-sucedida, todas as alterações são confirmadas
- **Suporte a transações aninhadas**: Se já existe uma transação ativa, chamadas aninhadas reutilizam a transação existente
- **Propagação de exceções**: Exceções são relançadas após o rollback para que os chamadores possam tratá-las

### 2. Command Handlers Atualizados

Os seguintes command handlers foram atualizados para usar transações:

#### Prontuários Médicos
- **CreateMedicalRecordCommandHandler**: Cria prontuário médico E atualiza status da consulta atomicamente
- **CompleteMedicalRecordCommandHandler**: Completa prontuário médico E finaliza a consulta atomicamente

#### Gerenciamento de Pacientes
- **LinkChildToGuardianCommandHandler**: Vincula criança ao responsável E atualiza ambos os registros atomicamente
- **LinkPatientToClinicCommandHandler**: Valida existência do paciente E cria/atualiza vínculo com clínica atomicamente

## Arquivos Modificados

### Código-Fonte

1. **src/MedicSoft.Domain/Interfaces/IRepository.cs**
   - Adicionados métodos de transação à interface

2. **src/MedicSoft.Repository/Repositories/BaseRepository.cs**
   - Implementação dos métodos de transação com suporte a:
     - Detecção de transação ativa
     - Rollback automático em caso de erro
     - Commit automático em caso de sucesso

3. **src/MedicSoft.Application/Handlers/Commands/MedicalRecords/CreateMedicalRecordCommandHandler.cs**
   - Envolvido operações de criação de prontuário em transação

4. **src/MedicSoft.Application/Handlers/Commands/MedicalRecords/CompleteMedicalRecordCommandHandler.cs**
   - Envolvido operações de finalização de prontuário em transação

5. **src/MedicSoft.Application/Handlers/Commands/Patients/LinkChildToGuardianCommandHandler.cs**
   - Envolvido operações de vinculação criança-responsável em transação

6. **src/MedicSoft.Application/Handlers/Commands/Patients/LinkPatientToClinicCommandHandler.cs**
   - Envolvido operações de vinculação paciente-clínica em transação

### Documentação

7. **DATABASE_TRANSACTION_IMPLEMENTATION.md** (Este documento)
   - Documentação completa em inglês sobre a implementação

8. **TRANSACOES_BANCO_DADOS.md** (Este documento)
   - Documentação em português sobre a implementação

## Exemplo de Uso

### Antes (Sem Transação)
```csharp
public async Task<MedicalRecordDto> Handle(CreateMedicalRecordCommand request, CancellationToken cancellationToken)
{
    // Faz check-in na consulta
    appointment.CheckIn();
    await _appointmentRepository.UpdateAsync(appointment); // Pode ter sucesso
    
    // Cria prontuário médico
    await _medicalRecordRepository.AddAsync(medicalRecord); // Pode falhar - deixando consulta com check-in!
    
    return dto;
}
```

### Depois (Com Transação)
```csharp
public async Task<MedicalRecordDto> Handle(CreateMedicalRecordCommand request, CancellationToken cancellationToken)
{
    return await _medicalRecordRepository.ExecuteInTransactionAsync(async () =>
    {
        // Faz check-in na consulta
        appointment.CheckIn();
        await _appointmentRepository.UpdateAsync(appointment);
        
        // Cria prontuário médico
        await _medicalRecordRepository.AddAsync(medicalRecord);
        
        // Ambas as operações têm sucesso ou ambas são revertidas!
        return dto;
    });
}
```

## Quando Usar Transações

Use `ExecuteInTransactionAsync` quando:

1. **Múltiplas operações** de banco de dados são realizadas e devem ser atômicas
2. **Entidades relacionadas** estão sendo modificadas e devem permanecer consistentes
3. **Regras de negócio** exigem que todas as operações tenham sucesso ou nenhuma
4. **Validação** acontece antes de múltiplas operações que podem falhar

**NÃO use transações para:**
- Operações únicas de banco de dados (já são atômicas por padrão)
- Operações somente de leitura
- Operações que não modificam o banco de dados

## Tratamento de Erros

Quando um erro ocorre:
1. A transação é revertida (rollback)
2. Todas as alterações no banco de dados dentro da transação são descartadas
3. A exceção é relançada para o chamador tratar
4. O DbContext permanece em um estado válido para operações subsequentes

## Testes

Todos os testes existentes (719 testes) continuam passando com a implementação de transações. O gerenciamento de transações é transparente para código existente que não o usa explicitamente.

### Recomendações de Testes Manuais

Para verificar o comportamento das transações:

1. **Testar rollback em erros de validação**: Provocar erros de validação após a primeira operação de banco de dados
2. **Testar rollback em erros de banco de dados**: Simular violações de constraints do banco
3. **Testar operações concorrentes**: Verificar que transações não causam deadlocks
4. **Testar transações aninhadas**: Chamar métodos que usam transações de dentro de outras transações

## Boas Práticas

1. **Mantenha transações curtas**: Inclua apenas operações necessárias no escopo da transação
2. **Evite chamadas externas**: Não chame APIs ou serviços externos dentro de uma transação
3. **Valide cedo**: Realize validações antes de iniciar operações caras de banco de dados
4. **Trate exceções apropriadamente**: Capture e registre exceções de operações transacionais
5. **Use para operações de negócio**: Aplique transações no nível da lógica de negócio (command handlers), não no nível do repositório para operações individuais

## Guia de Migração

Para desenvolvedores adicionando novos command handlers:

1. Identifique se seu handler realiza múltiplas operações de banco de dados
2. Se sim, envolva as operações em `ExecuteInTransactionAsync`:
   ```csharp
   return await _repository.ExecuteInTransactionAsync(async () => {
       // Suas operações de banco de dados aqui
       return resultado;
   });
   ```
3. Garanta que validações sejam feitas dentro da transação para evitar commits parciais
4. Teste o comportamento de rollback simulando erros

## Verificação de Build e Testes

✅ **Build**: Bem-sucedido sem erros ou avisos
✅ **Testes**: Todos os 719 testes passaram
✅ **Compatibilidade**: Nenhuma alteração breaking na API pública

## Melhorias Futuras

Possíveis aprimoramentos para o sistema de transações:

1. **Níveis de isolamento configuráveis**: Permitir especificar nível de isolamento por transação
2. **Logging de transações**: Adicionar logging detalhado de início, commit e rollback
3. **Monitoramento de performance**: Rastrear duração e frequência de rollbacks
4. **Transações distribuídas**: Suporte para operações que abrangem múltiplos bancos (se necessário)
5. **Padrão Saga**: Para operações de longa duração que não podem usar transações de banco

## Conclusão

A implementação de gerenciamento de transações fornece uma base robusta para garantir a consistência dos dados na aplicação MedicSoft. Ao envolver operações de múltiplas etapas em transações, garantimos que todas as alterações tenham sucesso juntas ou sejam revertidas completamente, prevenindo dados incompletos ou inconsistentes de serem persistidos no banco de dados.

## Suporte

Para questões ou suporte relacionado à implementação de transações, consulte:
- Este documento de documentação
- O documento em inglês `DATABASE_TRANSACTION_IMPLEMENTATION.md`
- O código-fonte em `BaseRepository.cs`
