using System;
using FluentAssertions;
using Xunit;
using MedicSoft.Application.Services.Fiscal;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Test.Services.Fiscal
{
    /// <summary>
    /// Testes unitários para SimplesNacionalHelper
    /// Baseados nas tabelas oficiais do Simples Nacional 2024/2025
    /// </summary>
    public class SimplesNacionalHelperTests
    {
        #region Anexo III - FatorR >= 28%

        [Theory]
        [InlineData(10000, 180000, 6.00)]     // Faixa 1: até R$ 180k
        [InlineData(10000, 180001, 11.20)]    // Faixa 2: de R$ 180k a R$ 360k
        [InlineData(10000, 360000, 11.20)]    // Faixa 2: de R$ 180k a R$ 360k
        [InlineData(10000, 360001, 13.50)]    // Faixa 3: de R$ 360k a R$ 720k
        [InlineData(10000, 720000, 13.50)]    // Faixa 3: de R$ 360k a R$ 720k
        [InlineData(10000, 720001, 16.00)]    // Faixa 4: de R$ 720k a R$ 1.8M
        [InlineData(10000, 1800000, 16.00)]   // Faixa 4: de R$ 720k a R$ 1.8M
        [InlineData(10000, 1800001, 21.00)]   // Faixa 5: de R$ 1.8M a R$ 3.6M
        [InlineData(10000, 3600000, 21.00)]   // Faixa 5: de R$ 1.8M a R$ 3.6M
        [InlineData(10000, 3600001, 33.00)]   // Faixa 6: acima de R$ 3.6M
        public void CalcularAliquotaEfetiva_DeveRetornarAliquotaCorreta_ParaAnexoIII(
            decimal valorNota,
            decimal receitaBruta12Meses,
            decimal aliquotaEsperada)
        {
            // Act
            var aliquota = SimplesNacionalHelper.CalcularAliquotaEfetiva(
                valorNota,
                receitaBruta12Meses,
                AnexoSimplesNacional.AnexoIII,
                30.00m // FatorR >= 28%
            );

            // Assert
            aliquota.Should().Be(aliquotaEsperada);
        }

        #endregion

        #region Anexo V - FatorR < 28%

        [Theory]
        [InlineData(10000, 180000, 15.50)]    // Faixa 1: até R$ 180k
        [InlineData(10000, 180001, 18.00)]    // Faixa 2: de R$ 180k a R$ 360k
        [InlineData(10000, 360000, 18.00)]    // Faixa 2
        [InlineData(10000, 360001, 19.50)]    // Faixa 3: de R$ 360k a R$ 720k
        [InlineData(10000, 720000, 19.50)]    // Faixa 3
        [InlineData(10000, 720001, 20.50)]    // Faixa 4: de R$ 720k a R$ 1.8M
        [InlineData(10000, 1800000, 20.50)]   // Faixa 4
        [InlineData(10000, 1800001, 23.00)]   // Faixa 5: de R$ 1.8M a R$ 3.6M
        [InlineData(10000, 3600000, 23.00)]   // Faixa 5
        [InlineData(10000, 3600001, 30.50)]   // Faixa 6: acima de R$ 3.6M
        public void CalcularAliquotaEfetiva_DeveRetornarAliquotaCorreta_ParaAnexoV(
            decimal valorNota,
            decimal receitaBruta12Meses,
            decimal aliquotaEsperada)
        {
            // Act
            var aliquota = SimplesNacionalHelper.CalcularAliquotaEfetiva(
                valorNota,
                receitaBruta12Meses,
                AnexoSimplesNacional.AnexoV,
                25.00m // FatorR < 28%
            );

            // Assert
            aliquota.Should().Be(aliquotaEsperada);
        }

        #endregion

        #region Cálculo de DAS

        [Theory]
        [InlineData(10000, 6.00, 600.00)]
        [InlineData(5000, 11.20, 560.00)]
        [InlineData(15000, 13.50, 2025.00)]
        [InlineData(20000, 16.00, 3200.00)]
        public void CalcularDAS_DeveCalcularCorretamente_ValorImposto(
            decimal valorNota,
            decimal aliquotaEfetiva,
            decimal dasEsperado)
        {
            // Act
            var das = SimplesNacionalHelper.CalcularDAS(valorNota, aliquotaEfetiva);

            // Assert
            das.Should().Be(dasEsperado);
        }

        [Fact]
        public void CalcularDAS_DeveRetornarZero_QuandoValorNotaZero()
        {
            // Act
            var das = SimplesNacionalHelper.CalcularDAS(0, 6.00m);

            // Assert
            das.Should().Be(0);
        }

        [Fact]
        public void CalcularDAS_DeveRetornarZero_QuandoAliquotaZero()
        {
            // Act
            var das = SimplesNacionalHelper.CalcularDAS(10000, 0);

            // Assert
            das.Should().Be(0);
        }

        #endregion

        #region Fator R

        [Theory]
        [InlineData(300000, 1000000, 30.00)] // 30% - Anexo III
        [InlineData(280000, 1000000, 28.00)] // 28% - Limite
        [InlineData(270000, 1000000, 27.00)] // 27% - Anexo V
        [InlineData(100000, 1000000, 10.00)] // 10% - Anexo V
        public void CalcularFatorR_DeveCalcularPercentualCorreto(
            decimal folhaSalarios,
            decimal receitaBruta,
            decimal fatorREsperado)
        {
            // Act
            var fatorR = SimplesNacionalHelper.CalcularFatorR(folhaSalarios, receitaBruta);

            // Assert
            fatorR.Should().Be(fatorREsperado);
        }

        [Fact]
        public void CalcularFatorR_DeveRetornarZero_QuandoReceitaBrutaZero()
        {
            // Act
            var fatorR = SimplesNacionalHelper.CalcularFatorR(100000, 0);

            // Assert
            fatorR.Should().Be(0);
        }

        [Fact]
        public void DeterminarAnexo_DeveRetornarAnexoIII_QuandoFatorRMaiorIgual28()
        {
            // Act
            var anexo = SimplesNacionalHelper.DeterminarAnexo(28.00m);

            // Assert
            anexo.Should().Be(AnexoSimplesNacional.AnexoIII);
        }

        [Fact]
        public void DeterminarAnexo_DeveRetornarAnexoV_QuandoFatorRMenor28()
        {
            // Act
            var anexo = SimplesNacionalHelper.DeterminarAnexo(27.99m);

            // Assert
            anexo.Should().Be(AnexoSimplesNacional.AnexoV);
        }

        #endregion

        #region Validações de Limite

        [Fact]
        public void ValidarLimiteReceita_DeveRetornarTrue_QuandoDentroDoLimite()
        {
            // Arrange
            var receitaBruta = 4800000m; // Limite é 4.8M

            // Act
            var resultado = SimplesNacionalHelper.ValidarLimiteReceita(receitaBruta);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void ValidarLimiteReceita_DeveRetornarFalse_QuandoAcimaDoLimite()
        {
            // Arrange
            var receitaBruta = 4800001m;

            // Act
            var resultado = SimplesNacionalHelper.ValidarLimiteReceita(receitaBruta);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void ObterFaixa_DeveRetornarFaixaCorreta_ParaCadaIntervaloReceita()
        {
            // Faixa 1: até 180k
            SimplesNacionalHelper.ObterFaixa(180000).Should().Be(1);
            
            // Faixa 2: 180k a 360k
            SimplesNacionalHelper.ObterFaixa(180001).Should().Be(2);
            SimplesNacionalHelper.ObterFaixa(360000).Should().Be(2);
            
            // Faixa 3: 360k a 720k
            SimplesNacionalHelper.ObterFaixa(360001).Should().Be(3);
            SimplesNacionalHelper.ObterFaixa(720000).Should().Be(3);
            
            // Faixa 4: 720k a 1.8M
            SimplesNacionalHelper.ObterFaixa(720001).Should().Be(4);
            SimplesNacionalHelper.ObterFaixa(1800000).Should().Be(4);
            
            // Faixa 5: 1.8M a 3.6M
            SimplesNacionalHelper.ObterFaixa(1800001).Should().Be(5);
            SimplesNacionalHelper.ObterFaixa(3600000).Should().Be(5);
            
            // Faixa 6: acima de 3.6M
            SimplesNacionalHelper.ObterFaixa(3600001).Should().Be(6);
        }

        #endregion

        #region Cálculos Progressivos

        [Fact]
        public void CalcularAliquotaEfetiva_DeveAplicarCalculoProgressivo_CorretamenteAnexoIII()
        {
            // Arrange: Receita de R$ 200.000 (Faixa 2)
            // Alíquota nominal: 11,20%
            // Parcela a deduzir: R$ 9.360,00
            // Alíquota efetiva = ((RBT12 × Aliq) - PD) / RBT12
            var valorNota = 10000m;
            var receitaBruta12Meses = 200000m;

            // Act
            var aliquota = SimplesNacionalHelper.CalcularAliquotaEfetiva(
                valorNota,
                receitaBruta12Meses,
                AnexoSimplesNacional.AnexoIII,
                30.00m
            );

            // Assert - Faixa 2 = 11.20%
            aliquota.Should().Be(11.20m);
        }

        [Fact]
        public void CalcularImpostoTotal_DeveCalcularCorretamente_CompletoFluxo()
        {
            // Arrange
            var receitaMensal = 50000m;
            var receitaBruta12Meses = 600000m; // Faixa 3
            var fatorR = 30.00m; // Anexo III
            
            // Act
            var aliquota = SimplesNacionalHelper.CalcularAliquotaEfetiva(
                receitaMensal,
                receitaBruta12Meses,
                AnexoSimplesNacional.AnexoIII,
                fatorR
            );
            var das = SimplesNacionalHelper.CalcularDAS(receitaMensal, aliquota);

            // Assert
            aliquota.Should().Be(13.50m); // Faixa 3 Anexo III
            das.Should().Be(6750.00m);    // 50000 * 13.5%
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void CalcularAliquotaEfetiva_DeveRetornarZero_QuandoReceitaBrutaZero()
        {
            // Act
            var aliquota = SimplesNacionalHelper.CalcularAliquotaEfetiva(
                10000,
                0,
                AnexoSimplesNacional.AnexoIII,
                30.00m
            );

            // Assert
            aliquota.Should().Be(0);
        }

        [Fact]
        public void CalcularAliquotaEfetiva_DeveRetornarAliquotaMinima_ParaReceitaMuitoBaixa()
        {
            // Act
            var aliquota = SimplesNacionalHelper.CalcularAliquotaEfetiva(
                1000,
                50000, // Muito baixo, Faixa 1
                AnexoSimplesNacional.AnexoIII,
                30.00m
            );

            // Assert
            aliquota.Should().Be(6.00m); // Alíquota mínima Anexo III
        }

        [Fact]
        public void CalcularAliquotaEfetiva_DeveRetornarAliquotaMaxima_ParaReceitaMuitoAlta()
        {
            // Act
            var aliquota = SimplesNacionalHelper.CalcularAliquotaEfetiva(
                100000,
                4800000, // Limite máximo
                AnexoSimplesNacional.AnexoIII,
                30.00m
            );

            // Assert
            aliquota.Should().Be(33.00m); // Alíquota máxima Anexo III (Faixa 6)
        }

        #endregion
    }
}
