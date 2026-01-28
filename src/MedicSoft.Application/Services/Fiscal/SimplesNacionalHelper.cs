using System;
using System.Collections.Generic;

namespace MedicSoft.Application.Services.Fiscal
{
    /// <summary>
    /// Helper para cálculos de impostos do Simples Nacional
    /// Anexos III e V conforme Lei Complementar 123/2006
    /// </summary>
    public static class SimplesNacionalHelper
    {
        /// <summary>
        /// Faixas de receita e alíquotas do Anexo III (Serviços - FatorR >= 28%)
        /// </summary>
        private static readonly List<FaixaSimples> FaixasAnexoIII = new()
        {
            new FaixaSimples(0, 180000, 6.00m, 0, 0),
            new FaixaSimples(180000.01m, 360000, 11.20m, 9360, 180000),
            new FaixaSimples(360000.01m, 720000, 13.50m, 17640, 360000),
            new FaixaSimples(720000.01m, 1800000, 16.00m, 35640, 720000),
            new FaixaSimples(1800000.01m, 3600000, 21.00m, 125640, 1800000),
            new FaixaSimples(3600000.01m, 4800000, 33.00m, 648000, 3600000)
        };

        /// <summary>
        /// Faixas de receita e alíquotas do Anexo V (Serviços - FatorR < 28%)
        /// </summary>
        private static readonly List<FaixaSimples> FaixasAnexoV = new()
        {
            new FaixaSimples(0, 180000, 15.50m, 0, 0),
            new FaixaSimples(180000.01m, 360000, 18.00m, 4500, 180000),
            new FaixaSimples(360000.01m, 720000, 19.50m, 9900, 360000),
            new FaixaSimples(720000.01m, 1800000, 20.50m, 17100, 720000),
            new FaixaSimples(1800000.01m, 3600000, 23.00m, 62100, 1800000),
            new FaixaSimples(3600000.01m, 4800000, 30.50m, 540000, 3600000)
        };

        /// <summary>
        /// Distribuição de impostos no Anexo III (%)
        /// </summary>
        public static readonly Dictionary<string, decimal> DistribuicaoAnexoIII = new()
        {
            { "IRPJ", 4.00m },
            { "CSLL", 3.50m },
            { "COFINS", 12.74m },
            { "PIS", 2.76m },
            { "CPP", 43.40m },  // Contribuição Previdenciária Patronal
            { "ISS", 33.60m }
        };

        /// <summary>
        /// Distribuição de impostos no Anexo V (%)
        /// </summary>
        public static readonly Dictionary<string, decimal> DistribuicaoAnexoV = new()
        {
            { "IRPJ", 0.00m },
            { "CSLL", 1.19m },
            { "COFINS", 15.56m },
            { "PIS", 3.37m },
            { "ISS", 79.88m }
        };

        /// <summary>
        /// Calcula a alíquota efetiva do Simples Nacional com base na receita bruta acumulada de 12 meses
        /// </summary>
        /// <param name="receitaBruta12Meses">Receita bruta acumulada dos últimos 12 meses</param>
        /// <param name="anexo">Anexo III ou V</param>
        /// <returns>Alíquota efetiva em percentual</returns>
        public static decimal CalcularAliquotaEfetiva(decimal receitaBruta12Meses, Domain.Entities.Fiscal.AnexoSimplesNacional anexo)
        {
            var faixas = anexo == Domain.Entities.Fiscal.AnexoSimplesNacional.AnexoIII 
                ? FaixasAnexoIII 
                : FaixasAnexoV;

            foreach (var faixa in faixas)
            {
                if (receitaBruta12Meses >= faixa.LimiteInferior && receitaBruta12Meses <= faixa.LimiteSuperior)
                {
                    // Fórmula: Alíquota Efetiva = ((RBT12 × Aliq) - PD) / RBT12
                    // Onde: RBT12 = Receita Bruta Total 12 meses
                    //       Aliq = Alíquota nominal da faixa
                    //       PD = Parcela a deduzir
                    
                    if (receitaBruta12Meses == 0)
                        return faixa.Aliquota;

                    var aliquotaEfetiva = ((receitaBruta12Meses * faixa.Aliquota / 100) - faixa.ParcelaADeduzir) / receitaBruta12Meses * 100;
                    return Math.Max(aliquotaEfetiva, 0); // Não pode ser negativa
                }
            }

            // Se ultrapassar o limite, usar a última faixa
            var ultimaFaixa = faixas[faixas.Count - 1];
            return ultimaFaixa.Aliquota;
        }

        /// <summary>
        /// Calcula o valor de DAS (Documento de Arrecadação do Simples Nacional)
        /// </summary>
        /// <param name="receitaBruta12Meses">Receita bruta acumulada dos últimos 12 meses</param>
        /// <param name="faturamentoMesAtual">Faturamento do mês atual</param>
        /// <param name="anexo">Anexo III ou V</param>
        /// <returns>Valor do DAS a pagar</returns>
        public static decimal CalcularValorDAS(decimal receitaBruta12Meses, decimal faturamentoMesAtual, Domain.Entities.Fiscal.AnexoSimplesNacional anexo)
        {
            var aliquotaEfetiva = CalcularAliquotaEfetiva(receitaBruta12Meses, anexo);
            return faturamentoMesAtual * aliquotaEfetiva / 100;
        }

        /// <summary>
        /// Distribui o valor do DAS entre os impostos conforme o anexo
        /// </summary>
        /// <param name="valorDAS">Valor total do DAS</param>
        /// <param name="anexo">Anexo III ou V</param>
        /// <returns>Dicionário com o valor de cada imposto</returns>
        public static Dictionary<string, decimal> DistribuirImpostos(decimal valorDAS, Domain.Entities.Fiscal.AnexoSimplesNacional anexo)
        {
            var distribuicao = anexo == Domain.Entities.Fiscal.AnexoSimplesNacional.AnexoIII 
                ? DistribuicaoAnexoIII 
                : DistribuicaoAnexoV;

            var resultado = new Dictionary<string, decimal>();
            foreach (var item in distribuicao)
            {
                resultado[item.Key] = valorDAS * item.Value / 100;
            }

            return resultado;
        }

        /// <summary>
        /// Verifica se a receita está dentro dos limites do Simples Nacional
        /// </summary>
        /// <param name="receitaBruta12Meses">Receita bruta acumulada dos últimos 12 meses</param>
        /// <returns>True se está dentro do limite</returns>
        public static bool VerificarLimiteSimples(decimal receitaBruta12Meses)
        {
            return receitaBruta12Meses <= 4800000; // R$ 4,8 milhões
        }

        /// <summary>
        /// Representa uma faixa de receita do Simples Nacional
        /// </summary>
        private class FaixaSimples
        {
            public decimal LimiteInferior { get; }
            public decimal LimiteSuperior { get; }
            public decimal Aliquota { get; }
            public decimal ParcelaADeduzir { get; }
            public decimal BaseCalculo { get; }

            public FaixaSimples(decimal limiteInferior, decimal limiteSuperior, decimal aliquota, decimal parcelaADeduzir, decimal baseCalculo)
            {
                LimiteInferior = limiteInferior;
                LimiteSuperior = limiteSuperior;
                Aliquota = aliquota;
                ParcelaADeduzir = parcelaADeduzir;
                BaseCalculo = baseCalculo;
            }
        }
    }
}
