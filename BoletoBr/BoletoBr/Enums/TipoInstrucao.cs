﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoletoBr.Enums
{
    public enum EnumTipoInstrucao
    {
        Protestar,
        NaoProtestar,
        NaoReceberAposOVencimento,
        ProtestarAposNDiasCorridos,
        ProtestarAposNDiasUteis,
        NaoReceberAposNDiasCorridos,
        NaoReceberAposNDiasUteis,
        DevolverAposNDias,
        MultaVencimento,
        JurosdeMora,
        DescontoPorDia,
        CobrarJurosApos7DiasVencimento,

        #region Itaú

        ImportanciaPorDiaDeAtrasoAPartirDeDDMMAA,
        NoVencimentoPagavelEmQualquerAgencia,

        #endregion

        #region Santander

        NaoHaInstrucoes,
        BaixarAposQuinzeDiasDoVencto,
        BaixarAposTrintaDiasDoVencto,
        NaoBaixar,
        NaoCobrarJurosDeMora,

        #endregion

        #region HSBC

        MultaPercentualVencimento,
        MultaPorDiaVencimento,
        MultaPorDiaCorrido,
        MultaPorDiaUtil,
        JurosSoAposData,
        ConcederAbatimento,
        AposVencimentoMulta10PorCento,
        ConcederDescontoPagoAposVencimento,
        NaoReceberAntesDoVencimento,
        NaoReceberAntesdoVencimentoOu10DiasApos,
        AposVencimentoMulta20PorCentoMaisMora,
        AbatimentoDesconto,
        TituloSujeitoAProtestoAposVencimento,
        AposVencimentoMulta2PorCento,
        MultaDeVPorCentoAposNDiasCorridos,
        MultaDeVPorCentoAposNDiasUteis,

        // Instruções que não geram mensagens nos boletos
        ProtestarAposNDiasCorridosNGM,
        ProtestarAposNDiasUteisNGM,

        #endregion

        #region Bradesco

        CobrarEncargosApos5DiaVencimento,
        CobrarEncargosApos10DiaVencimento,
        CobrarEncargosApos15DiaVencimento,

        #endregion

        #region CAIXA

        ProtestoFinsFalimentares,

        #endregion
    }
}
