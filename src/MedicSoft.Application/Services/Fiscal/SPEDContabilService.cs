using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services.Fiscal
{
    /// <summary>
    /// Serviço para geração e validação de arquivos SPED Contábil (ECD - Escrituração Contábil Digital)
    /// </summary>
    public class SPEDContabilService : ISPEDContabilService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IConfiguracaoFiscalRepository _configuracaoFiscalRepository;
        private readonly IPlanoContasRepository _planoContasRepository;
        private readonly ILancamentoContabilRepository _lancamentoContabilRepository;

        public SPEDContabilService(
            IClinicRepository clinicRepository,
            IConfiguracaoFiscalRepository configuracaoFiscalRepository,
            IPlanoContasRepository planoContasRepository,
            ILancamentoContabilRepository lancamentoContabilRepository)
        {
            _clinicRepository = clinicRepository;
            _configuracaoFiscalRepository = configuracaoFiscalRepository;
            _planoContasRepository = planoContasRepository;
            _lancamentoContabilRepository = lancamentoContabilRepository;
        }

        public async Task<string> GerarSPEDContabilAsync(Guid clinicaId, DateTime inicio, DateTime fim, string tenantId)
        {
            var clinica = await _clinicRepository.GetByIdAsync(clinicaId, tenantId)
                ?? throw new ArgumentException("Clínica não encontrada", nameof(clinicaId));

            var configuracoesFiscais = await _configuracaoFiscalRepository.GetAllAsync(tenantId);
            var configuracaoFiscal = configuracoesFiscais.FirstOrDefault(cf => cf.ClinicaId == clinicaId);

            var sb = new StringBuilder();

            // Bloco 0: Abertura, Identificação e Referências
            GerarBloco0(sb, clinica, configuracaoFiscal, inicio, fim);

            // Bloco I: Lançamentos Contábeis
            await GerarBlocoIAsync(sb, clinicaId, inicio, fim, tenantId);

            // Bloco J: Demonstrações Contábeis
            await GerarBlocoJAsync(sb, clinicaId, inicio, fim, tenantId);

            // Bloco 9: Controle e Encerramento
            GerarBloco9(sb);

            return sb.ToString();
        }

        private void GerarBloco0(StringBuilder sb, Clinic clinica, ConfiguracaoFiscal? config, DateTime inicio, DateTime fim)
        {
            // Limpar CNPJ/CPF
            var documento = clinica.Document?.Replace(".", "").Replace("/", "").Replace("-", "") ?? "";
            
            // Extrair estado do endereço (por ora usar valor padrão)
            var estado = "SP"; // Valor padrão - pode ser configurado posteriormente
            
            // |0000| - Abertura do Arquivo Digital e Identificação da Pessoa Jurídica
            var tipoEscrituração = "G"; // G = Escrituração Completa
            var situacaoEspecial = "0"; // 0 = Situação Normal
            
            sb.AppendLine($"|0000|LECD|{inicio:ddMMyyyy}|{fim:ddMMyyyy}|{clinica.Name}|{documento}|{estado}||{tipoEscrituração}||{situacaoEspecial}|||A|1|");

            // |0001| - Abertura do Bloco 0
            sb.AppendLine("|0001|0|");

            // |0007| - Outras Inscrições Cadastrais da Pessoa Jurídica
            if (!string.IsNullOrEmpty(config?.InscricaoMunicipal))
            {
                sb.AppendLine($"|0007|01|{config.InscricaoMunicipal}|{estado}|");
            }

            // |0020| - Escrituração Contábil Descentralizada
            sb.AppendLine($"|0020|{clinica.Name}|{documento}|{inicio:ddMMyyyy}|{fim:ddMMyyyy}|");

            // |0150| - Tabela de Cadastro do Participante
            sb.AppendLine($"|0150|{documento}|{clinica.Name}|01|{clinica.Address ?? ""}|||{estado}||{clinica.Phone ?? ""}||");

            // |0990| - Encerramento do Bloco 0
            var totalLinhasBloco0 = sb.ToString().Split('\n').Count(l => l.StartsWith("|0"));
            sb.AppendLine($"|0990|{totalLinhasBloco0 + 1}|");
        }

        private async Task GerarBlocoIAsync(StringBuilder sb, Guid clinicaId, DateTime inicio, DateTime fim, string tenantId)
        {
            var clinica = await _clinicRepository.GetByIdAsync(clinicaId, tenantId);
            if (clinica == null) return;

            // |I001| - Abertura do Bloco I
            sb.AppendLine("|I001|0|");

            // |I010| - Identificação da Escrituração Contábil
            sb.AppendLine($"|I010|N|LIVRO DIÁRIO|{clinica.Name}|01|{inicio:ddMMyyyy}|{fim:ddMMyyyy}|N|");

            // Buscar plano de contas
            var todasContas = await _planoContasRepository.GetAllAsync(tenantId);
            var planoContas = todasContas
                .Where(pc => pc.ClinicaId == clinicaId && pc.Ativa)
                .OrderBy(pc => pc.Codigo)
                .ToList();

            // |I050| - Plano de Contas
            foreach (var conta in planoContas)
            {
                var natureza = conta.Tipo == TipoConta.Ativo || conta.Tipo == TipoConta.Despesa ? "01" : "02";
                var nivel = conta.Codigo.Split('.').Length;
                sb.AppendLine($"|I050|{inicio:ddMMyyyy}|{conta.Codigo}|{conta.Nome}|{natureza}|{nivel}|");
            }

            // Buscar lançamentos contábeis do período
            var todosLancamentos = await _lancamentoContabilRepository.GetAllAsync(tenantId);
            var lancamentos = todosLancamentos
                .Where(l => l.ClinicaId == clinicaId && l.DataLancamento >= inicio && l.DataLancamento <= fim)
                .OrderBy(l => l.DataLancamento)
                .ToList();

            // Agrupar lançamentos por data
            var lancamentosPorData = lancamentos
                .GroupBy(l => l.DataLancamento.Date)
                .OrderBy(g => g.Key);

            foreach (var grupo in lancamentosPorData)
            {
                // |I100| - Centro de Custos (Opcional - por ora não implementado)

                // |I150| - Abertura do Período de Apuração
                sb.AppendLine($"|I150|{grupo.Key:ddMMyyyy}|");

                var numeroLancamento = 1;
                foreach (var lancamento in grupo)
                {
                    // |I200| - Lançamento Contábil
                    sb.AppendLine($"|I200|{numeroLancamento}|{lancamento.Tipo}|{lancamento.Historico}|{lancamento.Valor:F2}|");

                    // |I250| - Partidas do Lançamento
                    var conta = planoContas.FirstOrDefault(pc => pc.Id == lancamento.PlanoContasId);
                    if (conta != null)
                    {
                        var tipoPartida = lancamento.Tipo == TipoLancamentoContabil.Debito ? "D" : "C";
                        sb.AppendLine($"|I250|{conta.Codigo}|{lancamento.Valor:F2}|{tipoPartida}|");
                    }

                    numeroLancamento++;
                }

                // |I990| - Encerramento do Período de Apuração (cada dia)
                var totalLinhasPeriodo = sb.ToString().Split('\n')
                    .Count(l => l.Contains($"|I150|{grupo.Key:ddMMyyyy}|") || l.StartsWith("|I200|") || l.StartsWith("|I250|"));
            }

            // |I990| - Encerramento do Bloco I (Total)
            var totalLinhasBlocoI = sb.ToString().Split('\n').Count(l => l.StartsWith("|I"));
            sb.AppendLine($"|I990|{totalLinhasBlocoI + 1}|");
        }

        private async Task GerarBlocoJAsync(StringBuilder sb, Guid clinicaId, DateTime inicio, DateTime fim, string tenantId)
        {
            // |J001| - Abertura do Bloco J
            sb.AppendLine("|J001|0|");

            // Por ora, gerar bloco J básico sem dados
            // Em uma implementação futura, buscar DRE e Balanço Patrimonial
            sb.AppendLine($"|J100|{fim:ddMMyyyy}|BALANÇO PATRIMONIAL|");
            sb.AppendLine($"|J150|{inicio:ddMMyyyy}|{fim:ddMMyyyy}|DEMONSTRAÇÃO DO RESULTADO DO EXERCÍCIO|");

            // |J990| - Encerramento do Bloco J
            var totalLinhasBlocoJ = sb.ToString().Split('\n').Count(l => l.StartsWith("|J"));
            sb.AppendLine($"|J990|{totalLinhasBlocoJ + 1}|");
        }

        private void GerarBloco9(StringBuilder sb)
        {
            // |9001| - Abertura do Bloco 9
            sb.AppendLine("|9001|0|");

            // |9900| - Registros do Arquivo
            var registros = new Dictionary<string, int>();
            var linhas = sb.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var linha in linhas)
            {
                if (linha.StartsWith("|"))
                {
                    var registro = linha.Split('|')[1];
                    if (!registros.ContainsKey(registro))
                        registros[registro] = 0;
                    registros[registro]++;
                }
            }

            foreach (var registro in registros.OrderBy(r => r.Key))
            {
                sb.AppendLine($"|9900|{registro.Key}|{registro.Value}|");
            }

            // |9990| - Encerramento do Bloco 9
            var totalLinhasBloco9 = sb.ToString().Split('\n').Count(l => l.StartsWith("|9"));
            sb.AppendLine($"|9990|{totalLinhasBloco9 + 2}|"); // +2 para incluir 9990 e 9999

            // |9999| - Encerramento do Arquivo Digital
            var totalLinhas = sb.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries).Count(l => l.StartsWith("|"));
            sb.AppendLine($"|9999|{totalLinhas + 1}|"); // +1 para incluir o próprio 9999
        }

        public async Task<SPEDValidationResult> ValidarSPEDContabilAsync(string conteudoSPED)
        {
            var result = new SPEDValidationResult
            {
                Valido = true
            };

            var erros = new List<string>();
            var avisos = new List<string>();

            if (string.IsNullOrWhiteSpace(conteudoSPED))
            {
                result.Valido = false;
                erros.Add("Conteúdo SPED vazio");
                result.Erros = erros.ToArray();
                result.Avisos = avisos.ToArray();
                return result;
            }

            var linhas = conteudoSPED.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var registros = new Dictionary<string, int>();
            var blocos = new HashSet<string>();

            // Validação básica de estrutura
            foreach (var linha in linhas.Select((l, i) => new { Linha = l, Index = i }))
            {
                if (!linha.Linha.StartsWith("|"))
                {
                    result.Valido = false;
                    erros.Add($"Linha {linha.Index + 1}: Linha não inicia com '|'");
                    continue;
                }

                if (!linha.Linha.EndsWith("|"))
                {
                    result.Valido = false;
                    erros.Add($"Linha {linha.Index + 1}: Linha não termina com '|'");
                }

                var campos = linha.Linha.Split('|');
                if (campos.Length < 2)
                {
                    result.Valido = false;
                    erros.Add($"Linha {linha.Index + 1}: Formato inválido");
                    continue;
                }

                var registro = campos[1];
                if (!registros.ContainsKey(registro))
                    registros[registro] = 0;
                registros[registro]++;

                // Identificar blocos
                if (registro.Length >= 1)
                    blocos.Add(registro[0].ToString());
            }

            result.TotalRegistros = linhas.Length;
            result.TotalBlocos = blocos.Count;

            // Validações obrigatórias para SPED Contábil
            if (!registros.ContainsKey("0000"))
            {
                result.Valido = false;
                erros.Add("Registro 0000 (abertura do arquivo) não encontrado");
            }

            if (!registros.ContainsKey("0001"))
            {
                result.Valido = false;
                erros.Add("Registro 0001 (abertura do bloco 0) não encontrado");
            }

            if (!registros.ContainsKey("0990"))
            {
                result.Valido = false;
                erros.Add("Registro 0990 (encerramento do bloco 0) não encontrado");
            }

            if (!registros.ContainsKey("I001"))
            {
                result.Valido = false;
                erros.Add("Registro I001 (abertura do bloco I) não encontrado");
            }

            if (!registros.ContainsKey("I010"))
            {
                result.Valido = false;
                erros.Add("Registro I010 (identificação da escrituração) não encontrado");
            }

            if (!registros.ContainsKey("9001"))
            {
                result.Valido = false;
                erros.Add("Registro 9001 (abertura do bloco 9) não encontrado");
            }

            if (!registros.ContainsKey("9990"))
            {
                result.Valido = false;
                erros.Add("Registro 9990 (encerramento do bloco 9) não encontrado");
            }

            if (!registros.ContainsKey("9999"))
            {
                result.Valido = false;
                erros.Add("Registro 9999 (encerramento do arquivo) não encontrado");
            }

            // Avisos
            if (!registros.ContainsKey("I050"))
            {
                avisos.Add("Registro I050 (Plano de Contas) não encontrado");
            }

            if (!registros.ContainsKey("J001"))
            {
                avisos.Add("Bloco J (Demonstrações Contábeis) não encontrado");
            }

            result.Erros = erros.ToArray();
            result.Avisos = avisos.ToArray();

            return result;
        }

        public async Task<string> ExportarSPEDContabilAsync(Guid clinicaId, DateTime inicio, DateTime fim, string caminhoArquivo, string tenantId)
        {
            var conteudo = await GerarSPEDContabilAsync(clinicaId, inicio, fim, tenantId);

            // Criar diretório se não existir
            var diretorio = Path.GetDirectoryName(caminhoArquivo);
            if (!string.IsNullOrEmpty(diretorio) && !Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            // Salvar arquivo
            await File.WriteAllTextAsync(caminhoArquivo, conteudo, Encoding.UTF8);

            return caminhoArquivo;
        }
    }
}
