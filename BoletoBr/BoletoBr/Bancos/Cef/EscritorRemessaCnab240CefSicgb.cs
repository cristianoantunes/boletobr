﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoletoBr.Dominio;

namespace BoletoBr.Bancos.Cef
{
    public class EscritorRemessaCnab240CefSicgb : IEscritorArquivoRemessa
    {
        #region Especificações Remessa CAIXA

        /*
         * O controle entre um grupo de segmentos para um mesmo título será peos campos 'Código do Movimento' e 'Número do Registro'.
         * O segmento 'P' é obrigatório;
         * O segmento 'Q' é obrigatório somente para o Código de Movimento '01' (Entrada de Títulos)
         * 
         * Código de Movimento Remessa = 31 (Alteração de Outros Dados)
         * Campos Alteráveis:
         * - Espécie do Título
         * - Aceite
         * - Data de Emissão do Título
         * - Juros
         * - Desconto
         * - Valor do IOF
         * - Abatimento
         * - Código/Prazo Protesto
         * - Código/Prazo Devolução
         * - Dados do Sacado
         * - Dados do Avalista
         * - Multa
         * - Mensagens
         * Obs.: Campos numéricos -> Quando estes campos não precisarem ser alterados devem ser preenchidos com brancos, excepcionalmente,
         * para caracterizar a falta de informação.
         * 
         * Mensagens nos Bloquetos:
         * Desc.: zz.zzz.zzz.zz9,99 até dd/mm/aaaa
         * Abatimento: zz.zzz.zzz.zz9,99
         * Juros: zz.zzz.zzz.zz9,99 ao dia
         * Multa: zz.zzz.zzz.zz9,99 após dd/mm/aaaa
         * Protestar com z9 dias
         * Não receber após z9 dias do vencimento
         */

        #endregion

        readonly Remessa _remessa = new Remessa()
        {
            Ambiente = Remessa.EnumTipoAmbiemte.Homologacao,
            CodigoOcorrencia = "01", // Entrada de Título
            TipoDocumento = "2" // 2 - Escritural (Fixo)

        };

        public List<string> EscreverArquivo(List<Boleto> boletosEscrever)
        {
            throw new NotImplementedException();
        }

        public void ValidarArquivoRemessa() { }

        public string EscreverHeader(Boleto boleto, int numeroRegistro)
        {
            var header = new string(' ', 240);
            try
            {
                header = header.PreencherValorNaLinha(1, 3, "104");
                header = header.PreencherValorNaLinha(4, 7, "0000"); 
                header = header.PreencherValorNaLinha(8, 8, "0");
                header = header.PreencherValorNaLinha(9, 17, string.Empty.PadLeft(9, ' '));
                header = header.PreencherValorNaLinha(18, 18, boleto.CedenteBoleto.CpfCnpj.Replace(".", "").Replace("/", "").Replace("-", "").Length == 11 ? "1" : "2");                          
                header = header.PreencherValorNaLinha(19, 32, boleto.CedenteBoleto.CpfCnpj.Replace(".", "").Replace("/", "").Replace("-", "").PadLeft(14, '0'));
                header = header.PreencherValorNaLinha(33, 52, string.Empty.PadLeft(20, '0'));
                header = header.PreencherValorNaLinha(53, 57, boleto.CedenteBoleto.ContaBancariaCedente.Agencia.PadLeft(5, '0')); 
                header = header.PreencherValorNaLinha(58, 58, boleto.CedenteBoleto.ContaBancariaCedente.DigitoConta);                                      
                header = header.PreencherValorNaLinha(59, 64, boleto.CedenteBoleto.CodigoCedente.PadLeft(6, '0'));
                header = header.PreencherValorNaLinha(65, 71, string.Empty.PadLeft(7, '0'));
                header = header.PreencherValorNaLinha(72, 72, "0"); 
                header = header.PreencherValorNaLinha(73, 102, boleto.CedenteBoleto.Nome.PadRight(30, ' '));
                header = header.PreencherValorNaLinha(103, 132, "CAIXA ECONOMICA FEDERAL".PadRight(30, ' '));                                                         
                header = header.PreencherValorNaLinha(133, 142, string.Empty.PadLeft(10, ' '));

                #region CÓDIGO REMESSA / RETORNO

                /* Código adotado pela FEBRABAN para qualificar o envio ou devoução de arquivo entre Empresa Cliente e o Banco prestador de serviços.
                 * Informar:
                 * 1 - Remessa (Cliente -> Banco)
                 * 2 - Retorno (Banco -> Cliente)
                 * 3 - Remessa Processada (Banco -> Cliente - Pré-Crítica)
                 * 4 - Remessa Processada Parcial (Banco -> Cliente - Pré-Crítica)
                 * 5 - Remessa Rejeitada (Banco -> Cliente - Pré-Crítica)
                 */

                #endregion

                // Código Remessa/Retorno padronizado para "1" no envio do arquivo
                header = header.PreencherValorNaLinha(143, 143, "1");                                                                             
                header = header.PreencherValorNaLinha(144, 151, DateTime.Now.ToString("d").Replace("/", ""));
                header = header.PreencherValorNaLinha(152, 157, DateTime.Now.ToString("T").Replace(":", ""));                                
                header = header.PreencherValorNaLinha(158, 163, numeroRegistro.ToString().PadLeft(6, '0'));
                header = header.PreencherValorNaLinha(164, 166, "050");
                header = header.PreencherValorNaLinha(167, 171, string.Empty.PadLeft(5, '0')); 
                header = header.PreencherValorNaLinha(172, 191, string.Empty.PadRight(20, ' '));
                header = header.PreencherValorNaLinha(192, 211, _remessa.Ambiente.Equals(Remessa.EnumTipoAmbiemte.Producao) ? 
                    "REMESSA-PRODUCAO".PadRight(20, ' ') :
                    "REMESSA-TESTE".PadRight(20, ' '));
                header = header.PreencherValorNaLinha(212, 215, string.Empty.PadRight(4, ' '));
                header = header.PreencherValorNaLinha(216, 240, string.Empty.PadRight(25, ' '));

                return header;
            }
            catch (Exception e)
            {
                throw new Exception(String.Concat("Falha na geração do HEADER do arquivo de REMESSA.",
                    Environment.NewLine, e));
            }
        }

        public string EscreverHeaderDeLote(Boleto boleto, int numeroRemessa, int numeroLote, int numeroRegistro)
        {
            #region NOTAS EXPLICATIVAS HEADER DE LOTE

            #region 04.1 - G028 - TIPO DE OPERAÇÃO

            /* Tipo de Operação
             * Código adotado pela FEBRABAN para identificar a transação que será realizada com os registros detalhe do lote.
             * Domínio:
             * R = Arquivo Remessa
             * T = Arquivo Retorno
             */

            #endregion

            #region 05.1 - G025 - TIPO DE SERVIÇO

            /* Tipo de Serviço
             * Código adotado pela FEBRABAN para indicar o tipo de serviço / produto (processo) contido no arquivo / lote.
             * Domínio:
             * 01 = Cobrança Registrada
             * 02 = Cobrança Sem Registro / Serviços
             * 03 = Desconto de Títulos
             * 04 = Caução de Títulos
             */

            #endregion

            #endregion

            var headerLote = new string(' ', 240);
            try
            {
                headerLote = headerLote.PreencherValorNaLinha(1, 3, "104"); // Código do Banco na Compensação
                headerLote = headerLote.PreencherValorNaLinha(4, 7, numeroLote.ToString().PadLeft(4, '0')); // Lote de Serviço
                headerLote = headerLote.PreencherValorNaLinha(8, 8, "1"); // Tipo de Registro
                headerLote = headerLote.PreencherValorNaLinha(9, 9, "R");         
                headerLote = headerLote.PreencherValorNaLinha(10, 11, boleto.CarteiraCobranca.Codigo.Equals("RG") ? "01" : "02"); // Tipo de Serviço
                headerLote = headerLote.PreencherValorNaLinha(12, 13, "00"); // Uso Exclusivo FREBRABAN/CNAB
                headerLote = headerLote.PreencherValorNaLinha(14, 16, "030"); // Nº da versão do Layout do Lote
                headerLote = headerLote.PreencherValorNaLinha(17, 17, " "); // Uso Exclusivo FREBRABAN/CNAB
                headerLote = headerLote.PreencherValorNaLinha(18, 18, boleto.CedenteBoleto.CpfCnpj.Replace(".", "").Replace("/", "").Replace("-", "").Length == 11 ? "01" : "02");
                headerLote = headerLote.PreencherValorNaLinha(19, 33, boleto.CedenteBoleto.CpfCnpj.Replace(".", "").Replace("/", "").Replace("-", "").PadLeft(15, '0'));
                headerLote = headerLote.PreencherValorNaLinha(34, 39, boleto.CedenteBoleto.CodigoCedente.PadLeft(6, '0'));
                headerLote = headerLote.PreencherValorNaLinha(40, 53, string.Empty.PadLeft(14, '0'));
                headerLote = headerLote.PreencherValorNaLinha(54, 58, boleto.CedenteBoleto.ContaBancariaCedente.Agencia.PadLeft(5, '0'));
                headerLote = headerLote.PreencherValorNaLinha(59, 59, boleto.CedenteBoleto.ContaBancariaCedente.DigitoConta);
                headerLote = headerLote.PreencherValorNaLinha(60, 65, boleto.CedenteBoleto.CodigoCedente.PadLeft(6, '0'));
                headerLote = headerLote.PreencherValorNaLinha(66, 72, string.Empty.PadLeft(7, '0')); // Código do Modelo Personalizado
                headerLote = headerLote.PreencherValorNaLinha(73, 73, "0"); // Uso Exclusivo da CAIXA
                headerLote = headerLote.PreencherValorNaLinha(74, 103, boleto.CedenteBoleto.Nome.PadRight(30, ' '));
                headerLote = headerLote.PreencherValorNaLinha(104, 143, string.Empty.PadRight(40, ' ')); // Mensagem 1
                headerLote = headerLote.PreencherValorNaLinha(144, 183, string.Empty.PadRight(40, ' ')); // Mensagem 2

                // TODO: Analisar como vai controlar o número da remessa
                headerLote = headerLote.PreencherValorNaLinha(184, 191, numeroRemessa.ToString().PadLeft(8, '0')); // Número Remessa/Retorno

                headerLote = headerLote.PreencherValorNaLinha(192, 199, DateTime.Now.ToString("d").Replace("/", ""));
                headerLote = headerLote.PreencherValorNaLinha(200, 207, string.Empty.PadLeft(8, '0'));
                headerLote = headerLote.PreencherValorNaLinha(208, 240, string.Empty.PadRight(33, ' '));

                return headerLote;
            }
            catch (Exception e)
            {
                throw new Exception(
                    String.Format("<BoletoBr>{0}Falha na geração do HEADER DE LOTE do arquivo de REMESSA.",
                        Environment.NewLine), e);
            }
        }

        public string EscreverDetalheSegmentoP(Boleto boleto, int numeroLote, int numeroSequencialRegistroNoLote)
        {
            # region NOTAS EXPLICATIVAS REGISTRO DETALHE SEGMENTO P

            #region 04.3P - G038 - NÚMERO SEQUENCIAL DO REGISTRO NO LOTE

            /* Número Sequencial do Registro no Lote
             * Número para identificar a sequencia de registros encaminhados no lote.
             * Deve ser inicializado sempre em '1', em cada novo lote.
             */

            #endregion

            #endregion

            var segmentoP = new string(' ', 240);
            try
            {
                segmentoP = segmentoP.PreencherValorNaLinha(1, 3, "104"); // Código do Banco na Compensação
                segmentoP = segmentoP.PreencherValorNaLinha(4, 7, numeroLote.ToString().PadLeft(4, '0')); // Lote De Serviço
                segmentoP = segmentoP.PreencherValorNaLinha(8, 8, "3"); // Tipo de Registro
                segmentoP = segmentoP.PreencherValorNaLinha(9, 13, numeroSequencialRegistroNoLote.ToString().PadLeft(5, '0'));
                segmentoP = segmentoP.PreencherValorNaLinha(14, 14, "P"); // Cód. Segmento do Registro Detalhe
                segmentoP = segmentoP.PreencherValorNaLinha(15, 15, " ");
                // Padronizado para 01 - Entrada de Título
                segmentoP = segmentoP.PreencherValorNaLinha(16, 17, "01"); // Código de Movimento Remessa
                segmentoP = segmentoP.PreencherValorNaLinha(18, 22, boleto.CedenteBoleto.ContaBancariaCedente.Agencia.PadLeft(5, ' '));
                segmentoP = segmentoP.PreencherValorNaLinha(23, 23, boleto.CedenteBoleto.ContaBancariaCedente.DigitoAgencia);
                segmentoP = segmentoP.PreencherValorNaLinha(24, 29, boleto.CedenteBoleto.CodigoCedente.PadLeft(6, '0'));       
                segmentoP = segmentoP.PreencherValorNaLinha(30, 37, string.Empty.PadLeft(8, '0')); // Uso Exclusivo CAIXA
                segmentoP = segmentoP.PreencherValorNaLinha(38, 40, string.Empty.PadLeft(3, '0')); // Uso Exclusivo CAIXA
                segmentoP = segmentoP.PreencherValorNaLinha(41, 57, boleto.NossoNumeroFormatado.PadLeft(17, ' '));
                /* Código da Carteira
                 * '1' = Cobrança Simples
                 * '3' = Cobrança Caucionada
                 * '4' = Cobrança Descontada
                 */
                segmentoP = segmentoP.PreencherValorNaLinha(58, 58, "1");
                // 1 - Cobrança Registrada ou 2 - Cobrança Sem Registro
                segmentoP = segmentoP.PreencherValorNaLinha(59, 59, boleto.CarteiraCobranca.Codigo.Equals("RG") ? "1" : "2");
                // Fixo 2 - Escritural
                segmentoP = segmentoP.PreencherValorNaLinha(60, 60, "2"); // Tipo de Documento
                // '1' = Banco Emite
                segmentoP = segmentoP.PreencherValorNaLinha(61, 61, "1"); // Identificação da Emissão do Bloqueto
                // '1' = Sacado Via Correios
                segmentoP = segmentoP.PreencherValorNaLinha(62, 62, "1"); // Identificação da Entrega do Bloqueto
                segmentoP = segmentoP.PreencherValorNaLinha(63, 73, boleto.NumeroDocumento.PadLeft(11, '0'));
                segmentoP = segmentoP.PreencherValorNaLinha(74, 77, string.Empty.PadLeft(4, ' '));
                segmentoP = segmentoP.PreencherValorNaLinha(78, 85, boleto.DataVencimento.ToString("d").Replace("/", "").PadLeft(8, '0'));
                segmentoP = segmentoP.PreencherValorNaLinha(86, 100, boleto.ValorBoleto.ToString().Replace(".", "").Replace(",", "").PadLeft(13, '0'));
                segmentoP = segmentoP.PreencherValorNaLinha(101, 105, string.Empty.PadLeft(5, '0'));
                segmentoP = segmentoP.PreencherValorNaLinha(106, 106, "0"); // Dígito Verificador da Agência
                segmentoP = segmentoP.PreencherValorNaLinha(107, 108, boleto.Especie.Sigla.Equals("DM") ? "02" : boleto.Especie.Codigo.ToString()); // Espécia do Título 
                segmentoP = segmentoP.PreencherValorNaLinha(109, 109, boleto.Aceite.Equals("N") ? "N" : "A");
                segmentoP = segmentoP.PreencherValorNaLinha(110, 117, boleto.DataDocumento.ToString("d").Replace("/", "").PadLeft(8, '0'));
                /* Modalidade de cobrança de juros de mora
                 * 1 - Valor por dia
                 * 2 - Taxa Mensal
                 * 3 - Isento
                 */
                // TODO: Analisar como proceder com a cobrança de juros de mora
                segmentoP = segmentoP.PreencherValorNaLinha(118, 118, "3"); // Código do Juros de Mora
                segmentoP = segmentoP.PreencherValorNaLinha(119, 126, boleto.DataJurosMora.ToString("d").Replace("/", "").PadLeft(8, '0'));
                segmentoP = segmentoP.PreencherValorNaLinha(127, 141, boleto.JurosMora.ToString().Replace(".", "").Replace(",", "").PadLeft(13, '0'));
                /* Código do Desconto
                 * 0 - Sem desconto
                 * 1 - Valor fixo até a data informada
                 * 2 - Percentual até a data informada
                 * Obs.: Para os códigos '1' e '2' será obrigatório a informação da data.
                 */
                // TODO: Analisar como proceder com o desconto e outros abatimentos
                segmentoP = segmentoP.PreencherValorNaLinha(142, 142, "0"); // Código do Desconto 1
                segmentoP = segmentoP.PreencherValorNaLinha(143, 150, boleto.DataDesconto.ToString("d").Replace("/", "").PadLeft(8, '0'));
                segmentoP = segmentoP.PreencherValorNaLinha(151, 165, boleto.ValorDesconto.ToString().Replace(".", "").Replace(",", "").PadLeft(13, '0'));
                segmentoP = segmentoP.PreencherValorNaLinha(166, 180, boleto.Iof.ToString().Replace(".", "").Replace(",", "").PadLeft(13, '0'));
                segmentoP = segmentoP.PreencherValorNaLinha(181, 195, boleto.ValorAbatimento.ToString().Replace(".", "").Replace(",", "").PadLeft(13, '0'));
                segmentoP = segmentoP.PreencherValorNaLinha(196, 220, boleto.NumeroDocumento.PadRight(25, ' '));

                #region CÓDIGO PROTESTO

                /* Código para Protesto
                 * Código adotado pela FEBRABAN para identificar o tipo de prazo a ser considerado para protesto.
                 * 1 - Protestar
                 * 3 - Não Protestar
                 * 9 - Cancelamento Protesto Automático
                 * (Somente válido p/ Código Movimento Remessa = '31' - Alteração de Outros Dados)
                 */

                #endregion

                segmentoP = segmentoP.PreencherValorNaLinha(221, 221, "3"); // Código para Protesto
                segmentoP = segmentoP.PreencherValorNaLinha(222, 223, "00"); // Número de Dias para Protesto
                segmentoP = segmentoP.PreencherValorNaLinha(224, 224, "2"); // Código para Baixa/Devolução
                segmentoP = segmentoP.PreencherValorNaLinha(225, 227, "020"); // Número de Dias para Baixa/Devolução
                // Fixo 09 - REAL
                segmentoP = segmentoP.PreencherValorNaLinha(228, 229, "09"); // Código da Moeda
                segmentoP = segmentoP.PreencherValorNaLinha(230, 239, string.Empty.PadLeft(10, '0')); // Uso Exclusivo CAIXA
                segmentoP = segmentoP.PreencherValorNaLinha(240, 240, string.Empty.PadLeft(1, ' '));

                return segmentoP;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("<BoletoBr>{0}Falha na geração do DETALHE - Segmento T do arquivo de REMESSA.",
                    Environment.NewLine), e);
            }
        }

        public string EscreverDetalheSegQ(Boleto boleto, int numeroLote, int numeroSequencialRegistroNoLote)
        {
           string enderecoSacado = boleto.SacadoBoleto.EnderecoSacado.TipoLogradouro + " " +
                                    boleto.SacadoBoleto.EnderecoSacado.Logradouro + " " +
                                    boleto.SacadoBoleto.EnderecoSacado.Numero + " " +
                                    boleto.SacadoBoleto.EnderecoSacado.Complemento.PadRight(40, ' ');

            var segmentoQ = new string(' ', 240);

            try
            {
                segmentoQ = segmentoQ.PreencherValorNaLinha(1, 3, "104");
                segmentoQ = segmentoQ.PreencherValorNaLinha(4, 7, numeroLote.ToString().PadLeft(4, '0'));
                segmentoQ = segmentoQ.PreencherValorNaLinha(8, 8, "3");
                segmentoQ = segmentoQ.PreencherValorNaLinha(9, 13, numeroSequencialRegistroNoLote.ToString().PadLeft(5, '0'));
                segmentoQ = segmentoQ.PreencherValorNaLinha(14, 14, "Q");
                segmentoQ = segmentoQ.PreencherValorNaLinha(15, 15, " ");
                segmentoQ = segmentoQ.PreencherValorNaLinha(16, 17, "01");
                segmentoQ = segmentoQ.PreencherValorNaLinha(18, 18, boleto.SacadoBoleto.CpfCnpj.Replace(".", "").Replace("-", "").Replace("-", "").Length == 11 ? "01" : "02");
                segmentoQ = segmentoQ.PreencherValorNaLinha(19, 33, boleto.SacadoBoleto.CpfCnpj.Replace(".", "").Replace("/", "").Replace("-", "").PadLeft(15, '0'));
                segmentoQ = segmentoQ.PreencherValorNaLinha(34, 73, boleto.SacadoBoleto.Nome.PadRight(40, ' '));
                segmentoQ = segmentoQ.PreencherValorNaLinha(74, 128, enderecoSacado);
                segmentoQ = segmentoQ.PreencherValorNaLinha(129, 136, boleto.SacadoBoleto.EnderecoSacado.Cep.PadLeft(8, '0'));
                segmentoQ = segmentoQ.PreencherValorNaLinha(137, 151, boleto.SacadoBoleto.EnderecoSacado.Cidade.PadRight(15, ' '));
                segmentoQ = segmentoQ.PreencherValorNaLinha(152, 153, boleto.SacadoBoleto.EnderecoSacado.SiglaUf.PadRight(2, ' '));
                segmentoQ = segmentoQ.PreencherValorNaLinha(154, 154, boleto.SacadoBoleto.CpfCnpjAvalista.Replace(".", "").Replace("/", "").Replace("-", "").Length == 11 ? "01" : "02");
                segmentoQ = segmentoQ.PreencherValorNaLinha(155, 169, boleto.SacadoBoleto.CpfCnpjAvalista.Replace(".", "").Replace("/", "").Replace("-", "").PadLeft(15, '0'));
                segmentoQ = segmentoQ.PreencherValorNaLinha(170, 209, boleto.SacadoBoleto.NomeAvalista.PadRight(40, ' '));
                segmentoQ = segmentoQ.PreencherValorNaLinha(210, 212, string.Empty.PadLeft(3, ' '));
                segmentoQ = segmentoQ.PreencherValorNaLinha(213, 232, string.Empty.PadLeft(20, ' '));
                segmentoQ = segmentoQ.PreencherValorNaLinha(233, 240, string.Empty.PadLeft(8, ' '));

                return segmentoQ;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("<BoletoBr>{0}Falha na geração do DETALHE - Segmento Q do arquivo de REMESSA.",
                    Environment.NewLine), e);
            }
        }

        public string EscreverTrailerDeLote(int qtdTotalCobrancaSimples, decimal vlTotalCobrancaSimples, int qtdTotalCobrancaCaucionada, decimal vlTotalCobrancaCaucionada,
            int qtdTotalCobrancaDescontada, decimal vlTotalCobrancaDescontada, int numeroLote, int numeroRegistro)
        {
            var trailerLote = new string(' ', 240);
            try
            {
                trailerLote = trailerLote.PreencherValorNaLinha(1, 3, "104");
                trailerLote = trailerLote.PreencherValorNaLinha(4, 7, numeroLote.ToString().PadLeft(4, '0'));
                trailerLote = trailerLote.PreencherValorNaLinha(8, 8, "5");
                trailerLote = trailerLote.PreencherValorNaLinha(9, 17, string.Empty.PadLeft(9, ' '));
                trailerLote = trailerLote.PreencherValorNaLinha(18, 23, numeroRegistro.ToString().PadLeft(6, '0'));
                trailerLote = trailerLote.PreencherValorNaLinha(24, 29, qtdTotalCobrancaSimples.ToString().PadLeft(6, '0'));
                trailerLote = trailerLote.PreencherValorNaLinha(30, 46, vlTotalCobrancaSimples.ToString().Replace(".", "").Replace(",", "").PadLeft(15, '0'));
                trailerLote = trailerLote.PreencherValorNaLinha(47, 52, qtdTotalCobrancaCaucionada.ToString().PadLeft(6, '0'));
                trailerLote = trailerLote.PreencherValorNaLinha(53, 69, vlTotalCobrancaCaucionada.ToString().Replace(".", "").Replace(",", "").PadLeft(15, '0'));
                trailerLote = trailerLote.PreencherValorNaLinha(70, 75, qtdTotalCobrancaDescontada.ToString().PadLeft(6, '0'));
                trailerLote = trailerLote.PreencherValorNaLinha(76, 92, vlTotalCobrancaDescontada.ToString().Replace(".", "").Replace(",", "").PadLeft(15, '0'));
                trailerLote = trailerLote.PreencherValorNaLinha(93, 123, string.Empty.PadLeft(31, ' '));
                trailerLote = trailerLote.PreencherValorNaLinha(124, 240, string.Empty.PadLeft(117, ' '));

                return trailerLote;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("<BoletoBr>{0}Falha na geração do TRAILER DE LOTE do arquivo de REMESSA.",
                   Environment.NewLine), e);
            }
        }

        public string EscreverTrailer(int qtdLotes, int qtdRegistros)
        {
            var trailer = new string(' ', 240);
            try
            {
                trailer = trailer.PreencherValorNaLinha(1, 3, "104");
                trailer = trailer.PreencherValorNaLinha(4, 7, "9999");
                trailer = trailer.PreencherValorNaLinha(8, 8, "9");
                trailer = trailer.PreencherValorNaLinha(9, 17, string.Empty.PadLeft(9, ' '));
                trailer = trailer.PreencherValorNaLinha(18, 23, qtdLotes.ToString().PadLeft(6, '0'));
                trailer = trailer.PreencherValorNaLinha(24, 29, qtdRegistros.ToString().PadLeft(6, '0'));
                trailer = trailer.PreencherValorNaLinha(30, 35, string.Empty.PadLeft(6, ' '));
                trailer = trailer.PreencherValorNaLinha(36, 240, string.Empty.PadLeft(205, ' '));

                return trailer;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("<BoletoBr>{0}Falha na geração do TRAILER do arquivo de REMESSA.",
                    Environment.NewLine), e);
            }
        }
    }
}