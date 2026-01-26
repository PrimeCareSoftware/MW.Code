import { Injectable } from '@angular/core';

export interface HelpContent {
  title: string;
  sections: HelpSection[];
}

export interface HelpSection {
  title: string;
  content: string;
  testData?: TestDataExample[];
}

export interface TestDataExample {
  field: string;
  validExample: string;
  description: string;
}

@Injectable({
  providedIn: 'root'
})
export class HelpService {
  private helpContent: Map<string, HelpContent> = new Map();

  constructor() {
    this.initializeHelpContent();
  }

  getHelpContent(module: string): HelpContent | undefined {
    return this.helpContent.get(module);
  }

  private initializeHelpContent(): void {
    // Patients Module Help
    this.helpContent.set('patients', {
      title: 'Ajuda - Módulo de Pacientes',
      sections: [
        {
          title: 'Como Cadastrar um Novo Paciente',
          content: `
            <p>Para cadastrar um novo paciente no sistema, siga os passos abaixo:</p>
            <ol>
              <li>Clique no botão <strong>"Novo Paciente"</strong> na tela de listagem</li>
              <li>Preencha os campos obrigatórios do formulário</li>
              <li>Verifique se todos os dados estão corretos</li>
              <li>Clique em <strong>"Salvar"</strong> para confirmar o cadastro</li>
            </ol>
          `,
          testData: [
            {
              field: 'Nome Completo',
              validExample: 'João da Silva Santos',
              description: 'Nome completo do paciente (mínimo 3 caracteres)'
            },
            {
              field: 'CPF',
              validExample: '123.456.789-00',
              description: 'CPF válido no formato XXX.XXX.XXX-XX'
            },
            {
              field: 'Data de Nascimento',
              validExample: '15/05/1985',
              description: 'Data válida no formato DD/MM/AAAA'
            },
            {
              field: 'Telefone',
              validExample: '(11) 98765-4321',
              description: 'Telefone celular no formato (XX) XXXXX-XXXX'
            },
            {
              field: 'Email',
              validExample: 'joao.silva@email.com',
              description: 'Email válido para contato'
            },
            {
              field: 'CEP',
              validExample: '01234-567',
              description: 'CEP válido no formato XXXXX-XXX'
            }
          ]
        },
        {
          title: 'Campos Obrigatórios',
          content: `
            <p>Os seguintes campos são obrigatórios para o cadastro de pacientes:</p>
            <ul>
              <li><strong>Nome Completo:</strong> Nome completo do paciente</li>
              <li><strong>CPF:</strong> Documento de identificação único</li>
              <li><strong>Data de Nascimento:</strong> Para cálculo de idade</li>
              <li><strong>Telefone:</strong> Para contato e confirmações</li>
            </ul>
          `
        },
        {
          title: 'Como Buscar Pacientes',
          content: `
            <p>Utilize a barra de pesquisa para encontrar pacientes por:</p>
            <ul>
              <li>Nome completo ou parte do nome</li>
              <li>CPF</li>
              <li>Telefone</li>
              <li>Email</li>
            </ul>
            <p>A busca é realizada em tempo real conforme você digita.</p>
          `
        },
        {
          title: 'Como Editar um Paciente',
          content: `
            <ol>
              <li>Localize o paciente na listagem</li>
              <li>Clique no botão <strong>"Editar"</strong> na linha do paciente</li>
              <li>Altere os dados necessários</li>
              <li>Clique em <strong>"Salvar"</strong> para confirmar as alterações</li>
            </ol>
          `
        }
      ]
    });

    // Appointments Module Help
    this.helpContent.set('appointments', {
      title: 'Ajuda - Módulo de Agendamentos',
      sections: [
        {
          title: 'Como Agendar uma Consulta',
          content: `
            <p>Para agendar uma nova consulta:</p>
            <ol>
              <li>Acesse o <strong>Calendário de Agendamentos</strong></li>
              <li>Clique no horário desejado ou use o botão <strong>"Novo Agendamento"</strong></li>
              <li>Selecione o paciente</li>
              <li>Escolha o profissional de saúde</li>
              <li>Defina a data e horário</li>
              <li>Informe o tipo de atendimento</li>
              <li>Clique em <strong>"Salvar"</strong></li>
            </ol>
          `,
          testData: [
            {
              field: 'Paciente',
              validExample: 'Selecione um paciente cadastrado',
              description: 'Paciente deve existir no sistema'
            },
            {
              field: 'Profissional',
              validExample: 'Dr. João Silva',
              description: 'Profissional de saúde cadastrado e ativo'
            },
            {
              field: 'Data',
              validExample: 'DD/MM/AAAA (data futura)',
              description: 'Data futura válida'
            },
            {
              field: 'Horário',
              validExample: '14:00',
              description: 'Horário dentro do expediente (08:00 - 18:00)'
            },
            {
              field: 'Duração',
              validExample: '30 minutos',
              description: 'Duração entre 15 e 120 minutos'
            },
            {
              field: 'Tipo',
              validExample: 'Consulta de Rotina',
              description: 'Selecione um tipo de atendimento válido'
            }
          ]
        },
        {
          title: 'Visualizações Disponíveis',
          content: `
            <p>O módulo oferece duas formas de visualizar os agendamentos:</p>
            <ul>
              <li><strong>Calendário:</strong> Visualização mensal com todos os agendamentos</li>
              <li><strong>Lista:</strong> Visualização em lista com filtros avançados</li>
            </ul>
          `
        },
        {
          title: 'Status dos Agendamentos',
          content: `
            <ul>
              <li><strong>Agendado:</strong> Consulta confirmada</li>
              <li><strong>Confirmado:</strong> Paciente confirmou presença</li>
              <li><strong>Em Atendimento:</strong> Paciente sendo atendido</li>
              <li><strong>Concluído:</strong> Atendimento finalizado</li>
              <li><strong>Cancelado:</strong> Agendamento cancelado</li>
              <li><strong>Faltou:</strong> Paciente não compareceu</li>
            </ul>
          `
        }
      ]
    });

    // Attendance Module Help
    this.helpContent.set('attendance', {
      title: 'Ajuda - Módulo de Atendimento',
      sections: [
        {
          title: 'Fluxo de Atendimento',
          content: `
            <p>O atendimento segue as seguintes etapas:</p>
            <ol>
              <li><strong>Check-in:</strong> Registro da chegada do paciente</li>
              <li><strong>Triagem:</strong> Coleta de sinais vitais</li>
              <li><strong>Atendimento:</strong> Consulta médica</li>
              <li><strong>Prescrição:</strong> Emissão de receitas e solicitações</li>
              <li><strong>Finalização:</strong> Conclusão do atendimento</li>
            </ol>
          `,
          testData: [
            {
              field: 'Peso',
              validExample: '70.5 kg',
              description: 'Peso entre 2 e 300 kg'
            },
            {
              field: 'Altura',
              validExample: '1.75 m',
              description: 'Altura entre 0.40 e 2.50 m'
            },
            {
              field: 'Pressão Arterial',
              validExample: '120/80 mmHg',
              description: 'Formato: XXX/XXX mmHg'
            },
            {
              field: 'Temperatura',
              validExample: '36.5°C',
              description: 'Temperatura entre 35.0 e 42.0°C'
            },
            {
              field: 'Frequência Cardíaca',
              validExample: '72 bpm',
              description: 'Entre 40 e 200 batimentos por minuto'
            }
          ]
        },
        {
          title: 'Registro de Sinais Vitais',
          content: `
            <p>Durante a triagem, registre os seguintes sinais vitais:</p>
            <ul>
              <li>Peso e altura (para cálculo do IMC)</li>
              <li>Pressão arterial</li>
              <li>Temperatura corporal</li>
              <li>Frequência cardíaca</li>
              <li>Saturação de oxigênio (se disponível)</li>
            </ul>
          `
        }
      ]
    });

    // Medical Records Module Help
    this.helpContent.set('medical-records', {
      title: 'Ajuda - Módulo de Prontuários',
      sections: [
        {
          title: 'Acessando o Prontuário',
          content: `
            <p>O prontuário médico eletrônico contém todo o histórico do paciente:</p>
            <ul>
              <li>Histórico de consultas</li>
              <li>Diagnósticos anteriores</li>
              <li>Prescrições e medicamentos</li>
              <li>Exames e resultados</li>
              <li>Alergias e observações importantes</li>
            </ul>
            <p>Todas as alterações são registradas com data, hora e usuário responsável.</p>
          `
        },
        {
          title: 'Adicionando Informações',
          content: `
            <ol>
              <li>Acesse o prontuário do paciente</li>
              <li>Selecione a aba desejada (Anamnese, Diagnóstico, etc.)</li>
              <li>Preencha as informações necessárias</li>
              <li>Clique em <strong>"Salvar"</strong></li>
            </ol>
            <p><strong>Importante:</strong> Todas as informações são protegidas pela LGPD e cada acesso é auditado.</p>
          `
        }
      ]
    });

    // Prescriptions Module Help
    this.helpContent.set('prescriptions', {
      title: 'Ajuda - Módulo de Prescrições',
      sections: [
        {
          title: 'Emitindo uma Receita Digital',
          content: `
            <p>Para emitir uma receita digital:</p>
            <ol>
              <li>Acesse o prontuário do paciente</li>
              <li>Clique em <strong>"Nova Prescrição"</strong></li>
              <li>Adicione os medicamentos necessários</li>
              <li>Preencha posologia e duração do tratamento</li>
              <li>Adicione orientações complementares</li>
              <li>Clique em <strong>"Gerar Receita"</strong></li>
            </ol>
          `,
          testData: [
            {
              field: 'Medicamento',
              validExample: 'Dipirona Sódica 500mg',
              description: 'Nome completo do medicamento com dosagem'
            },
            {
              field: 'Posologia',
              validExample: 'Tomar 1 comprimido a cada 6 horas',
              description: 'Instruções claras de como usar'
            },
            {
              field: 'Quantidade',
              validExample: '20 comprimidos',
              description: 'Quantidade total a ser dispensada'
            },
            {
              field: 'Duração',
              validExample: '7 dias',
              description: 'Tempo de duração do tratamento'
            }
          ]
        },
        {
          title: 'Medicamentos Controlados (SNGPC)',
          content: `
            <p>Para prescrição de medicamentos controlados:</p>
            <ul>
              <li>Certifique-se de que o CRM está válido e cadastrado</li>
              <li>A receita será automaticamente marcada como "controlada"</li>
              <li>Dados serão enviados para o sistema SNGPC</li>
              <li>Acompanhe o envio no <strong>Dashboard SNGPC</strong></li>
            </ul>
          `
        }
      ]
    });

    // Financial Module Help
    this.helpContent.set('financial', {
      title: 'Ajuda - Módulo Financeiro',
      sections: [
        {
          title: 'Contas a Receber',
          content: `
            <p>Gerencie os valores a receber de pacientes e convênios:</p>
            <ol>
              <li>Acesse <strong>Financeiro > Contas a Receber</strong></li>
              <li>Clique em <strong>"Nova Conta"</strong></li>
              <li>Preencha os dados da conta</li>
              <li>Defina a data de vencimento</li>
              <li>Salve o registro</li>
            </ol>
          `,
          testData: [
            {
              field: 'Descrição',
              validExample: 'Consulta - João Silva',
              description: 'Descrição clara da receita'
            },
            {
              field: 'Valor',
              validExample: 'R$ 150,00',
              description: 'Valor monetário válido'
            },
            {
              field: 'Vencimento',
              validExample: 'DD/MM/AAAA (data futura)',
              description: 'Data de vencimento futura'
            },
            {
              field: 'Categoria',
              validExample: 'Consultas',
              description: 'Categoria de receita cadastrada'
            },
            {
              field: 'Cliente',
              validExample: 'João da Silva',
              description: 'Cliente ou paciente cadastrado'
            }
          ]
        },
        {
          title: 'Contas a Pagar',
          content: `
            <p>Registre e controle as despesas da clínica:</p>
            <ul>
              <li>Fornecedores e seus dados</li>
              <li>Vencimentos e valores</li>
              <li>Categorias de despesas</li>
              <li>Status de pagamento</li>
            </ul>
          `
        },
        {
          title: 'Fluxo de Caixa',
          content: `
            <p>Acompanhe a movimentação financeira em tempo real:</p>
            <ul>
              <li><strong>Dashboard:</strong> Visão geral das finanças</li>
              <li><strong>Entradas:</strong> Receitas recebidas</li>
              <li><strong>Saídas:</strong> Despesas pagas</li>
              <li><strong>Saldo:</strong> Posição financeira atual</li>
            </ul>
          `
        },
        {
          title: 'Notas Fiscais Eletrônicas',
          content: `
            <p>Emita NFSe diretamente pelo sistema:</p>
            <ol>
              <li>Configure os dados fiscais em <strong>Configurações</strong></li>
              <li>Acesse <strong>Financeiro > Notas Fiscais</strong></li>
              <li>Clique em <strong>"Nova Nota"</strong></li>
              <li>Preencha os dados do serviço</li>
              <li>Clique em <strong>"Emitir"</strong></li>
            </ol>
          `
        }
      ]
    });

    // TISS Module Help
    this.helpContent.set('tiss', {
      title: 'Ajuda - Módulo TISS/TUSS',
      sections: [
        {
          title: 'Cadastro de Operadoras',
          content: `
            <p>Cadastre as operadoras de saúde (convênios):</p>
            <ol>
              <li>Acesse <strong>TISS > Operadoras</strong></li>
              <li>Clique em <strong>"Nova Operadora"</strong></li>
              <li>Preencha os dados da operadora</li>
              <li>Configure códigos e tabelas</li>
              <li>Salve o cadastro</li>
            </ol>
          `,
          testData: [
            {
              field: 'Registro ANS',
              validExample: '123456',
              description: 'Número de registro ANS válido (6 dígitos)'
            },
            {
              field: 'Razão Social',
              validExample: 'Operadora de Saúde S.A.',
              description: 'Nome completo da operadora'
            },
            {
              field: 'CNPJ',
              validExample: '12.345.678/0001-90',
              description: 'CNPJ válido no formato XX.XXX.XXX/XXXX-XX'
            }
          ]
        },
        {
          title: 'Guias TISS',
          content: `
            <p>Gere e envie guias TISS para as operadoras:</p>
            <ul>
              <li><strong>Guia de Consulta:</strong> Para consultas médicas</li>
              <li><strong>Guia SP/SADT:</strong> Para exames e procedimentos</li>
              <li><strong>Guia de Internação:</strong> Para internações hospitalares</li>
            </ul>
          `
        },
        {
          title: 'Lotes TISS',
          content: `
            <p>Organize as guias em lotes para envio:</p>
            <ol>
              <li>Acesse <strong>TISS > Lotes</strong></li>
              <li>Clique em <strong>"Novo Lote"</strong></li>
              <li>Selecione a operadora</li>
              <li>Adicione as guias ao lote</li>
              <li>Gere o arquivo XML</li>
              <li>Envie para a operadora</li>
            </ol>
          `
        }
      ]
    });

    // Telemedicine Module Help
    this.helpContent.set('telemedicine', {
      title: 'Ajuda - Módulo de Telemedicina',
      sections: [
        {
          title: 'Iniciando uma Teleconsulta',
          content: `
            <p>Para realizar uma consulta por vídeo:</p>
            <ol>
              <li>Agende a teleconsulta normalmente</li>
              <li>No horário marcado, acesse <strong>Telemedicina</strong></li>
              <li>Localize a sessão na lista</li>
              <li>Clique em <strong>"Entrar na Sala"</strong></li>
              <li>Aguarde o paciente conectar</li>
            </ol>
          `
        },
        {
          title: 'Requisitos Técnicos',
          content: `
            <p>Para usar a telemedicina, você precisa:</p>
            <ul>
              <li>Conexão de internet estável (mínimo 2 Mbps)</li>
              <li>Webcam e microfone funcionais</li>
              <li>Navegador atualizado (Chrome, Firefox ou Edge)</li>
              <li>Permitir acesso à câmera e microfone</li>
            </ul>
          `
        },
        {
          title: 'Termo de Consentimento',
          content: `
            <p>Antes da primeira teleconsulta, o paciente deve:</p>
            <ul>
              <li>Ler o termo de consentimento</li>
              <li>Concordar com as condições</li>
              <li>Assinar digitalmente</li>
            </ul>
            <p>O sistema armazena o termo assinado para fins legais.</p>
          `
        }
      ]
    });

    // SOAP Records Module Help
    this.helpContent.set('soap-records', {
      title: 'Ajuda - Módulo SOAP',
      sections: [
        {
          title: 'O que é SOAP?',
          content: `
            <p>SOAP é um método de documentação clínica estruturada:</p>
            <ul>
              <li><strong>S - Subjetivo:</strong> Queixa e sintomas relatados pelo paciente</li>
              <li><strong>O - Objetivo:</strong> Sinais clínicos e exame físico</li>
              <li><strong>A - Avaliação:</strong> Diagnóstico e análise do caso</li>
              <li><strong>P - Plano:</strong> Conduta, tratamento e orientações</li>
            </ul>
          `
        },
        {
          title: 'Preenchendo um Registro SOAP',
          content: `
            <ol>
              <li>Acesse o atendimento do paciente</li>
              <li>Clique em <strong>"Novo Registro SOAP"</strong></li>
              <li>Preencha cada seção (S, O, A, P)</li>
              <li>Adicione observações se necessário</li>
              <li>Salve o registro</li>
            </ol>
          `,
          testData: [
            {
              field: 'Subjetivo',
              validExample: 'Paciente relata dor de cabeça há 3 dias, pior pela manhã',
              description: 'Descreva os sintomas relatados pelo paciente'
            },
            {
              field: 'Objetivo',
              validExample: 'PA: 120/80 mmHg, FC: 72 bpm, afebril, consciente e orientado',
              description: 'Registre os achados do exame físico'
            },
            {
              field: 'Avaliação',
              validExample: 'Cefaleia tensional',
              description: 'Diagnóstico ou hipótese diagnóstica'
            },
            {
              field: 'Plano',
              validExample: 'Prescrever analgésico, orientar repouso, retorno se piora',
              description: 'Conduta e orientações ao paciente'
            }
          ]
        }
      ]
    });

    // Anamnesis Module Help
    this.helpContent.set('anamnesis', {
      title: 'Ajuda - Módulo de Anamnese',
      sections: [
        {
          title: 'Criando Templates de Anamnese',
          content: `
            <p>Crie questionários personalizados para diferentes especialidades:</p>
            <ol>
              <li>Acesse <strong>Anamnese > Gerenciar Templates</strong></li>
              <li>Clique em <strong>"Novo Template"</strong></li>
              <li>Defina nome e especialidade</li>
              <li>Adicione perguntas e categorias</li>
              <li>Configure tipos de resposta</li>
              <li>Salve o template</li>
            </ol>
          `
        },
        {
          title: 'Aplicando Anamnese',
          content: `
            <p>Durante o atendimento:</p>
            <ol>
              <li>Selecione o template apropriado</li>
              <li>Preencha as respostas com o paciente</li>
              <li>Revise as informações</li>
              <li>Salve no prontuário</li>
            </ol>
            <p>O histórico completo ficará disponível em consultas futuras.</p>
          `
        }
      ]
    });
  }
}
