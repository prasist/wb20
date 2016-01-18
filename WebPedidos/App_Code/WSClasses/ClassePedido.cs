﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Web.UI;
/*
 * Última alteração : 04/11/2014
 * N. Solicitação   : 51849
 * Descrição        : GeraXml - Implementação multi-empresa offline (Deve gerar os arquivos XML para todas as empresas)
 * Autor            : Fabiano  * 
 */
namespace WebPedidos.WSClasses
{
    public class ClassePedido
    {
        
        UsuarioResumido u;
        ParametroResumido pr;

        public static int InserePedido(
            UsuarioResumido u,  ParametroResumido pr, 
            int codVendedor,  int codCliente, 
            string observacao, int codFormaPagto,
            int codCondPagto, List<ProdutoResumido> produtos, 
            int codEmp, Int32 NumPedWeb, 
            Int32 NumPed,  decimal DescontoPedido, 
            int CodPrc_Tab, int CodPrz_Tab, int UnidadeVenda,  int iCodTipMov) 
        {

            Decimal TotalComissao = 0;
            Decimal TotalComissaoTelev = 0;

            ClasseBanco csBanco = new ClasseBanco();

            //verifica se tem itens na grid
            if (produtos.Count.ToString() == "0")            
            {
                //throw new Exception("Não há produtos digitados.");                
                return 0;
            }

            try
            {
                DataClassesDataContext dcdc = new DataClassesDataContext();
                
                decimal cPesoTotal = 0;
                decimal comissao_televend = 0;
                decimal comissao_vendedor = 0;
                int iIncideTelev = 0;
                int iIncideVend = 0;

                int CodTipMov = ValidaMovimentacao(codCliente, codEmp, iCodTipMov);

                if (CodTipMov == 0)
                {
                    throw new Exception("Não há Tipo de Movimentação Padrão cadastrada nos Parâmetros de Venda.");
                }
                
                /*51287*/
                string varCLIE_IPISuspenso = dcdc.CLIENTEs.FirstOrDefault(cli => cli.CodCli == codCliente).CLIE_IPISuspenso.ToString();

                PEDIDO p = new PEDIDO();
                
                p.CodEmp = (short)codEmp;

                //FABIANO - 12/10/2011 - N.42120
                //Buscar o segundo vendedor da VENDCLI e sua comissao. (TELEVENDEDOR)

                //49637 - Adicionado CODEMP na clausula where
                //var sSql = "SELECT CodVen, COMISSAOTELEV, INCIDE FROM VENDCLI INNER JOIN VENDEDOR ON VENDCLI.CodVen = VENDEDOR.CodVend" +
                           //" WHERE CodCli = " + codCliente + " AND CodVen <> " + codVendedor + "";

                var sSql = "SELECT CodVen, COMISSAOTELEV, INCIDE FROM VENDCLI INNER JOIN VENDEDOR ON VENDCLI.CodVen = VENDEDOR.CodVend" +
                           " WHERE CodCli = " + codCliente + " AND CodVen <> " + codVendedor + " AND CODEMP = " + p.CodEmp;
                
                var televendedor = csBanco.Query(sSql);
                
                p.TelVend_Pedido = null;
                
                if (televendedor.Read())
                {

                    if (televendedor[0].ToString() != "") //CODVEN
                    {
                        p.TelVend_Pedido = Convert.ToInt16(televendedor[0].ToString());
                    }

                    if (televendedor[1].ToString()!="") //COMISSAO
                    {
                        comissao_televend = Convert.ToDecimal(televendedor[1].ToString());
                    }

                    if (televendedor[2].ToString() != "") //INCIDE
                    {
                        iIncideTelev = Convert.ToInt16(televendedor[2].ToString());
                    }
                    
                }

                televendedor.Close();

                //Busca comissao do vendedor principal e o setor na vendcli
                sSql = "SELECT SETOR, Comissao, Incide" + 
                       " FROM " +
                       " VENDCLI INNER JOIN VENDEDOR ON VENDCLI.CodVen = VENDEDOR.CodVend " +
                       " WHERE 1=1 " +
                       " AND CodCli = " + codCliente + "" +
                       " AND CodVen = " + codVendedor + "";

                var r = csBanco.Query(sSql);

                if (r.Read())
                {
                    p.SETOR = r[0].ToString(); //43573

                    if (r[1].ToString()!="") //COMISSAO
                    {
                        comissao_vendedor = Convert.ToDecimal(r[1].ToString());
                    }

                    if (r[2].ToString() != "") //INCIDE
                    {
                        iIncideVend = Convert.ToInt16(r[2].ToString());
                    }
                                        
                }

                r.Close();

                //Busca parametro sistema
                r = csBanco.Query("SELECT conteudo FROM parametrosist WHERE descparametro = 'BloqPedConf' AND codemp = " + codEmp);

                if (r.Read())
                {
                    //Se Bloqueia por conferencia ou nao 
                    //Se = 'S' bloqueia com DIV (Divergencia), senao grava null
                    p.RESCONFER = r[0].ToString() == "S" ? "DIV" : null;
                }

                r.Close();


                //Validacao PEDIDO X PEDIDOWEB
                r = csBanco.Query("SELECT NUMPED FROM PEDIDO WHERE codemp = " + codEmp + " AND NUMPED = " + NumPedWeb + " AND NUMPEDWEB = " + NumPed + "");

                if (r.Read())
                {
                    throw new Exception("Pedido " + NumPed + " já cadastrado.");
                }

                r.Close();


               //Campos fixos
                p.Origem            = 'W';          //43573
               //p.StatusComercial  = "BCF";        // bloqueado para conferência (default)
                p.PesoBruto         = 0;
                p.Volume            = 0;
                p.Carga             = 1;
                p.SubCarga          = 1;                
                p.Comissao          = 0;                
                p.VlrFrete_Pedido   = 0;
                p.Origem_Pedido     = "VENDEDOR";
                p.Tipo_Pedido       = "PEDIDO";                
                p.USUCAD            = u.NomUsu;        //43573
                p.SitPed            = "ABE";
                p.HoraCad           = DateTime.Now;   //43573                
                p.Hora              = DateTime.Now;
                p.DtaEmi            = DateTime.Today; //DateTime.Now;
                p.DtaEnt            = DateTime.Today;
                p.DtaFat            = DateTime.Today;

                //Campos dinamicos
                p.PercDes           = DescontoPedido;
                p.CodVen            = (short)codVendedor;
                p.CodCli            = (int)codCliente;
                p.CodTipMov         = Convert.ToInt16(CodTipMov);
                p.NumPedWeb         = NumPedWeb;
                p.NumPed            = NumPed; //Agora buscando de forma diferente
                p.CodPgt            = (short)codCondPagto;
                p.CodFrmPgt         = (short)codFormaPagto;

                /*Alterado forma de pegar tabela de preco. Agora pega da combo, pois pode ser diferente do padrao configurado no sigma
                - Regras alteradas por solicitacao do cliente Bagetti 
                - 13/03/2013 - Fabiano                                    
                   p.CodigoTab         = Convert.ToInt16(pr.CodTipPrc);
                   p.CodPrzTab         = Convert.ToInt16(pr.CodTipPrz);                 
                */
                p.CodigoTab         = (short)CodPrc_Tab;
                p.CodPrzTab         = (short)CodPrz_Tab;

                p.Obs               = observacao;                
                p.Vlrtot            = (decimal)produtos.Sum(p2 => p2.TotalParcial) - ((decimal)produtos.Sum(p2 => p2.TotalParcial) * DescontoPedido/100);                                
                p.NomUsu            = u.NomUsu;                
                p.Usuario           = u.NomUsu;
                p.VlrSubTot         = (decimal)produtos.Sum(p2 => p2.TotalParcial);
                p.VlrDes1           = ((decimal)produtos.Sum(p2 => p2.TotalParcial) * DescontoPedido / 100);
                p.PesoLiq           = 0;

                //produtos.ForEach(prood => p.PesoLiq     += dcdc.SERVMERCs.SingleOrDefault(sm => sm.CodServMerc == prood.Codigo).PesoLiq * prood.Quantidade);
                //produtos.ForEach(prood => p.PesoLiq     += Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", dcdc.SERVMERCs.SingleOrDefault(sm => sm.CodServMerc == prood.Codigo).PesoLiq)) * prood.Quantidade);
                //produtos.ForEach(prood => p.PesoBruto   += dcdc.SERVMERCs.SingleOrDefault(sm => sm.CodServMerc == prood.Codigo).PesoBruto * prood.Quantidade);
                //produtos.ForEach(prood => p.PesoBruto   += Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", dcdc.SERVMERCs.SingleOrDefault(sm => sm.CodServMerc == prood.Codigo).PesoBruto)) * prood.Quantidade);

                string sValidaIPI = "", sIncidenciaDescBaseIpi="";

                /*51667 - BIERHOFF*/
                r = csBanco.Query("SELECT CALCULAIPI, CALCIPIDESC FROM NATOPER WHERE CODIGO = " + iCodTipMov);

                if (r.Read())
                {
                    sValidaIPI              = r["CALCULAIPI"].ToString();
                    sIncidenciaDescBaseIpi  = r["CALCIPIDESC"].ToString();
                }

                r.Close();

                //<<51667
                decimal cQtdTributavel  = 0;
                decimal cVlrTributavel  = 0;
                decimal cVlrIpi         = 0;
                decimal cVlrPis         = 0;  
                decimal cVlrCofins      = 0;
                decimal cQtdEmb         = 0;
                decimal cPercIpi        = 0;
                //<<51667
                                  
                dcdc.PEDIDOs.InsertOnSubmit(p);

                foreach (ProdutoResumido produto in produtos)
                {
                    ITENSPED itped          = new ITENSPED();

                    /*  Verifica qual unidade de venda
                     *  0 = Menor Unidade (Padrão)
                     *  2 = Maior Unidade (Selecionada pelo usuário)
                     */
                    int iUniVend = produto.UnidadeVenda == "0" ? 0 : 2;
                    
                    itped.Consignacao       = 'N';
                    itped.Itp_CodTabPrz     = produto.Itp_CodTabPrz; 
                    itped.Reserva           = Convert.ToChar(pr.SaldoPed);
                    itped.CodEmp            = (short)codEmp;
                    itped.Qtd               = (decimal)produto.Quantidade;
                    itped.Item              = (short)(produtos.IndexOf(produto) + 1);
                    itped.CodServMerc       = produto.Codigo;

                    //Comissao Tabela de Preco                
                    r = csBanco.Query("SELECT COMISSAO, FATOR FROM FATORES  WHERE CODSERVMERC = " + produto.Codigo + " AND CodTipPrc = " + CodPrc_Tab + " AND CodTipPrz = " + produto.Itp_CodTabPrz + "");
                    decimal dComissaoTabela=0; 
                    if (r.Read())
                    {
                        dComissaoTabela = Convert.ToDecimal(r["COMISSAO"].ToString());
                    }

                    r.Close();

                    itped.NumPed            = NumPed; 
                    //SERVMERC servMercadoria = dcdc.SERVMERCs.SingleOrDefault(sm => sm.CodServMerc == produto.Codigo);

                    r = csBanco.Query("SELECT QTDEMB, VALORPIS, VALORCOFINS,  IPI, SERV_VALORIPI, PercDesc, Aliquota, Saldo, PesoLiq, PesoBruto FROM SERVMERC WHERE CODSERVMERC = " + produto.Codigo + "");
                    
                    if (r.Read())
                    {
                        itped.DescMax = Convert.ToDecimal(r["PercDesc"].ToString());
                        itped.Aliquota = Convert.ToDecimal(r["Aliquota"].ToString());
                        itped.Saldo = (decimal)(Convert.ToDecimal(r["Saldo"].ToString()) - (decimal)produto.Quantidade);

                        //Bug com LinqToSql , tivemos que fazer calculo manualmente...
                        var cLiq = Convert.ToDecimal(r["PesoLiq"].ToString());
                        p.PesoLiq += cLiq * produto.Quantidade;

                        var cBruto = Convert.ToDecimal(r["PesoBruto"].ToString());
                        p.PesoBruto += cBruto * produto.Quantidade;

                        //<<51667
                        cPercIpi     = Convert.ToDecimal(r["IPI"] == null ? "0" : r["IPI"].ToString());
                        cVlrIpi      = Convert.ToDecimal(r["SERV_VALORIPI"] == null ? "0" : r["SERV_VALORIPI"].ToString());
                        cVlrPis      = Convert.ToDecimal(r["VALORPIS"].ToString() == "" ? "0" : r["VALORPIS"].ToString());
                        cVlrCofins   = Convert.ToDecimal(r["VALORCOFINS"].ToString() == "" ? "0" : r["VALORCOFINS"].ToString());
                        cQtdEmb      = Convert.ToDecimal(r["QTDEMB"].ToString() == "" ? "0" : r["QTDEMB"].ToString());
                        //<<51667
                    }
                    r.Close();

                    cPesoTotal              = cPesoTotal + produto.Peso;
                    itped.VlrProm           = 0;                    
                    itped.UniVenda          = (short)iUniVend;
                    
                    if (produto.QtdCaixa == 0)
                    {
                        produto.QtdCaixa = 1;
                    }
                    //Maior e menor unidade
                    decimal cQtdSolicitada  = (Convert.ToDecimal(produto.Quantidade) / Convert.ToDecimal(produto.QtdCaixa));

                    //String.Format("{0:" + Funcoes.Decimais(pr) + "}", products.Sum(p => p.TotalParcial));

                    //itped.VlrUni            = iUniVend == 2 ? (double)(Convert.ToDecimal(produto.Preco) / Convert.ToDecimal(produto.Quantidade)) : (double)produto.Preco;                    
                    //itped.VlrReal           = iUniVend == 2 ? (decimal)produto.Preco / cQtdSolicitada : (decimal)produto.Preco;
                    //itped.VLRUNIVENDA       = iUniVend == 2 ? (double)produto.Preco / (double)cQtdSolicitada : (double)produto.Preco;
                    //itped.QTDUNIVENDA       = iUniVend == 2 ? cQtdSolicitada : produto.Quantidade;

                    //if (iUniVend == 2)
                    //{
                    //    itped.VLRUNIVENDALIQ = ((double)produto.Preco / (double)cQtdSolicitada) - Convert.ToDouble(String.Format("{0:" + Funcoes.Decimais(pr) + "}", (((double)produto.Preco / (double)cQtdSolicitada) * ((double)produto.Desconto / 100))));
                    //}
                    //else
                    //{
                    //    itped.VLRUNIVENDALIQ = (double)produto.Preco - Convert.ToDouble(String.Format("{0:" + Funcoes.Decimais(pr) + "}", ((double)produto.Preco * (double)produto.Desconto / 100)));
                    //}


                    //49199
                    string sPrecoUnitario, sPrecoUnitarioReal, sPrecoUnitarioOriginal;
                    decimal dValorUnitarioLiq;
                    if (iUniVend == 2)
                    {
                        sPrecoUnitario = String.Format("{0:" + Funcoes.Decimais(pr) + "}", (double)(Convert.ToDecimal(produto.Preco) / Convert.ToDecimal(produto.Quantidade)));
                        sPrecoUnitarioReal = String.Format("{0:" + Funcoes.Decimais(pr) + "}", (decimal)produto.Preco / cQtdSolicitada);
                        sPrecoUnitarioOriginal = String.Format("{0:" + Funcoes.Decimais(pr) + "}", (decimal)produto.PrecoReal / cQtdSolicitada);
                    }
                    else
                    {
                        sPrecoUnitario = String.Format("{0:" + Funcoes.Decimais(pr) + "}", (double)produto.Preco);
                        sPrecoUnitarioReal = sPrecoUnitario;
                        sPrecoUnitarioOriginal = String.Format("{0:" + Funcoes.Decimais(pr) + "}", (double)produto.PrecoReal);
                    }

                    itped.ValorIpi = 0;
                    itped.BASEIPI = 0;
                    itped.ALIQIPI = 0;
                    itped.IPIUNIVENDA = 0;

                    /*51954*///Calcular o Valor unitario liquido antes do calculo da Qtd e Vlr tributavel IPI
                    if (iUniVend == 2)
                    {
                        //itped.VLRUNIVENDALIQ = ((double)produto.Preco / (double)cQtdSolicitada) - Convert.ToDouble(String.Format("{0:" + Funcoes.Decimais(pr) + "}", (((double)produto.Preco / (double)cQtdSolicitada) * ((double)produto.Desconto / 100))));
                        dValorUnitarioLiq = (Convert.ToDecimal(sPrecoUnitarioReal) - (Convert.ToDecimal(sPrecoUnitarioReal) * produto.Desconto / 100));
                    }
                    else
                    {
                        //itped.VLRUNIVENDALIQ = (double)produto.Preco - Convert.ToDouble(String.Format("{0:" + Funcoes.Decimais(pr) + "}", ((double)produto.Preco * (double)produto.Desconto / 100)));
                        dValorUnitarioLiq = (Convert.ToDecimal(sPrecoUnitario) - (Convert.ToDecimal(sPrecoUnitario) * produto.Desconto / 100));
                    }

                    dValorUnitarioLiq = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", dValorUnitarioLiq));

                    //>>51667
                    //51954 - Considerar o valor unitario liquido.
                    if (sValidaIPI == "S") { /* Se valida IPI */
                        if (cPercIpi <= 0)  /* Se percentual do IPI for zero, considerar valor*/
                        {
                            cVlrTributavel = Convert.ToDecimal(dValorUnitarioLiq) / (cQtdEmb == 0 ? 1 : cQtdEmb);
                            cQtdTributavel = (decimal)itped.Qtd * (cQtdEmb == 0 ? 1 : cQtdEmb);

                            /*51827*/
                            if (cVlrIpi > 0) /* Se houver valor de IPI no cadastro da mercadoria*/
                            {
                                //itped.ValorIpi = cQtdTributavel * cVlrIpi;
                                /*Tratar casas decimais*/
                                itped.ValorIpi = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", (cQtdTributavel * cVlrIpi)));

                                itped.BASEIPI = (decimal)itped.Qtd * Convert.ToDecimal(dValorUnitarioLiq);
                                itped.ALIQIPI  = 0;
                                itped.IPIUNIVENDA = itped.ValorIpi;
                            }
                        }
                        else if (cPercIpi > 0) /* Existe Percentual IPI */
                        {
                            cVlrTributavel = Convert.ToDecimal(dValorUnitarioLiq);
                            cQtdTributavel = (decimal)itped.Qtd;

                            /*51827*/
                            itped.BASEIPI = cQtdTributavel * Convert.ToDecimal(dValorUnitarioLiq);
                            
                            //itped.ValorIpi = ((itped.BASEIPI * cPercIpi) / 100);
                            /*Tratar casas decimais*/
                            itped.ValorIpi = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", ((itped.BASEIPI * cPercIpi) / 100)));

                            itped.IPIUNIVENDA = itped.ValorIpi;
                            /* Verificar incidencias sobre base IPI ?*/
                            itped.ALIQIPI = cPercIpi;                            
                        }

                        if (sIncidenciaDescBaseIpi.Equals("S"))
                        {
                            itped.BASEIPI = itped.BASEIPI - Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", (itped.BASEIPI * DescontoPedido / 100)));
                        }

                        /*51827*/
                        if (varCLIE_IPISuspenso.Equals("S")) /* Se cliente for suspenso IPI, zera valores calculados*/
                        {                            
                            itped.ValorIpi = 0;
                            itped.BASEIPI = 0;
                            itped.ALIQIPI = 0;
                            itped.IPIUNIVENDA = 0;
                        }

                    } /*Não validad IPI*/
                        else
                        {
                            if (cVlrPis > 0 || cVlrCofins > 0)
                            {
                                cVlrTributavel = Convert.ToDecimal(dValorUnitarioLiq) / (cQtdEmb == 0 ? 1 : cQtdEmb);
                                cQtdTributavel = (decimal)itped.Qtd * (cQtdEmb == 0 ? 1 : cQtdEmb);
                            }
                            else
                            {
                                cVlrTributavel = Convert.ToDecimal(dValorUnitarioLiq);
                                cQtdTributavel = (decimal)itped.Qtd;
                            }
                    }

                    itped.QTDTRIBUTAVEL = cQtdTributavel;
                    itped.VLRTRIBUTAVEL = cVlrTributavel;
                    //<<51667

                    itped.VlrUni = Convert.ToDouble(sPrecoUnitario);
                   
                  //itped.VlrReal = Convert.ToDecimal(sPrecoUnitarioReal);
                    itped.VlrReal = Convert.ToDecimal(sPrecoUnitarioOriginal);

                    itped.VLRUNIVENDA = Convert.ToDouble(sPrecoUnitarioReal);
                    itped.QTDUNIVENDA = iUniVend == 2 ? cQtdSolicitada : produto.Quantidade;

                    /*51954*/
                    //if (iUniVend == 2)
                    //{
                    //    //itped.VLRUNIVENDALIQ = ((double)produto.Preco / (double)cQtdSolicitada) - Convert.ToDouble(String.Format("{0:" + Funcoes.Decimais(pr) + "}", (((double)produto.Preco / (double)cQtdSolicitada) * ((double)produto.Desconto / 100))));
                    //    dValorUnitarioLiq    = (Convert.ToDecimal(sPrecoUnitarioReal) - (Convert.ToDecimal(sPrecoUnitarioReal) * produto.Desconto / 100));                        
                    //}
                    //else
                    //{
                    //    //itped.VLRUNIVENDALIQ = (double)produto.Preco - Convert.ToDouble(String.Format("{0:" + Funcoes.Decimais(pr) + "}", ((double)produto.Preco * (double)produto.Desconto / 100)));
                    //    dValorUnitarioLiq = (Convert.ToDecimal(sPrecoUnitario) - (Convert.ToDecimal(sPrecoUnitario) * produto.Desconto / 100));                        
                    //}

                    //dValorUnitarioLiq       = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", dValorUnitarioLiq));
                    itped.VLRUNIVENDALIQ    = Convert.ToDouble(dValorUnitarioLiq);
                                                           
                    itped.QtdCan            = 0;
                    itped.QTDCANUNIVENDA    = 0;
                    itped.QtdTroca          = 0;
                    itped.VlrDes            = produto.Desconto; //FABIANO - 07/10/2011
                    itped.ValorDesconto     = 0;
                    itped.VlrDescRateio     = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", (((decimal)itped.VLRUNIVENDALIQ * DescontoPedido / 100) * (decimal)itped.Qtd)));
                    itped.PerDescRateio     = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", DescontoPedido)); 
                    itped.PercDescGordura   = 0;
                    itped.VLRADICIONAL      = 0;
                    itped.VLREMBALAGEM      = 0;

                    itped.VlrComiss = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", (decimal)ClasseComissao.CalculaComissao(pr,
                                                                                    Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", iUniVend == 2 ? (double)(Convert.ToDecimal(itped.VLRUNIVENDALIQ) / Convert.ToDecimal(produto.QtdCaixa)) : (double)itped.VLRUNIVENDALIQ)), 
                                                                                    (decimal)itped.Qtd, 
                                                                                    0, 
                                                                                    0, 
                                                                                    comissao_vendedor,
                                                                                    dComissaoTabela, 
                                                                                    produto.Comissao, 
                                                                                    produto.Comissao, 
                                                                                    "N",
                                                                                    0, 
                                                                                    0, 
                                                                                    produto.Comissao, 
                                                                                    (decimal)comissao_televend,
                                                                                    (Int16)iIncideVend,
                                                                                    (Int16)iIncideTelev,
                                                                                    (Int32)produto.Codigo,
                                                                                    produto.Desconto)));

                    itped.ComissaoUsada = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", (decimal)ClasseComissao.ComissaoUsada)); 
                    
                    //VALOR DA COMISSAO TELEVENDEDOR 
                    itped.COMISSAOTELEV         = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", (decimal)ClasseComissao.CalculaComissao(pr,
                                                                                    Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", iUniVend == 2 ? (double)(Convert.ToDecimal(itped.VLRUNIVENDALIQ) / Convert.ToDecimal(produto.QtdCaixa)) : (double)itped.VLRUNIVENDALIQ)), 
                                                                                    (decimal)itped.Qtd, 
                                                                                    0, 
                                                                                    0, 
                                                                                    comissao_vendedor,
                                                                                    dComissaoTabela,
                                                                                    produto.ComissaoTel, 
                                                                                    produto.Comissao, 
                                                                                    "T", 
                                                                                    0, 
                                                                                    0, 
                                                                                    produto.Comissao, 
                                                                                    (decimal)comissao_televend,
                                                                                    (Int16)iIncideVend,
                                                                                    (Int16)iIncideTelev,
                                                                                    (Int32)produto.Codigo,
                                                                                    produto.Desconto)));

                    itped.COMISSAOUSADATELEV = (decimal)ClasseComissao.ComissaoUsadaTele;
                    itped.FatorComiss        = (decimal)ClasseComissao.FatorDesconto;

                    TotalComissao       += (decimal)itped.VlrComiss;
                    TotalComissaoTelev  += (decimal)itped.COMISSAOTELEV;

                    dcdc.ITENSPEDs.InsertOnSubmit(itped);
                }

                p.Comissao              = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", TotalComissao));        //Valor da comissao VENDEDOR
                p.COMISSAOTELEV         = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", TotalComissaoTelev));   //Valor da comissao do TELEVENDEDOR (OU SEGUNDO VENDEDOR)
                //p.PesoBruto             = cPesoTotal;
                //p.PesoLiq               = cPesoTotal;
                dcdc.SubmitChanges();
                dcdc.Dispose();
                return NumPed;
                

            }
            catch (Exception ex)
            {
                if (!String.IsNullOrEmpty(ex.Message))
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    return -1;
                }
            }
        }

        public static string VerificaCreditos(int codCliente, int codEmp)
        {
              string sSql;
              string sRetorna="Créditos do Cliente : \\n";
              decimal cValor = 0;

              ClasseBanco csBanco = new ClasseBanco();

              sSql = " SELECT NFDEV, VLRCRED, OBS " +
                     " FROM CREDCLI" +
                     " Where CodCLi = " + codCliente + 
                     " And CODEMP = " + codEmp +
                     " AND DTABAIXA IS NULL";

              var r = csBanco.Query(sSql);
              while (r.Read())
              {
                 sRetorna = sRetorna + "\\n" + r[0].ToString() + " - " + r[1].ToString() + " - " + r[2].ToString().Trim();
                 cValor = cValor + Convert.ToDecimal(r[1].ToString());
              }

              r.Close();
  
              sRetorna = sRetorna + "\\n \\nTotal :   " + Convert.ToString(cValor) + "\\n";

              if (cValor != 0)
              {
                  return sRetorna;
              }
              else
              {
                  return "";
              }  
        }

        public static int ValidaMovimentacao(int codCliente, int codEmp, int iMovimentacao)
        {
            ClasseBanco csBanco = new ClasseBanco();

            if (codCliente.Equals(-1))
            {
                return 0;
            }

            /* Busca UF do Cliente e da Empresa para ver qual tipo de movimentacao utilizar */
                var sUFCliente = "";
                var sUFEmpresa = "";

                var r = csBanco.Query("SELECT EstCli FROM CLIENTE WHERE CODCLI = " + codCliente);
                if (r.Read()){
                    sUFCliente = r[0].ToString();
                }                
                r.Close();

                r = csBanco.Query("SELECT Estado FROM EMPRESA WHERE CODEMP = " + codEmp);
                if (r.Read())
                {
                    sUFEmpresa = r[0].ToString();
                }
                r.Close();
                /* ------------------------------------------------------------------------------ */

                if (sUFCliente == "")
                {
                    throw new Exception("UF do Cliente não cadastrado.");
                }

                if (sUFEmpresa == "")
                {
                    throw new Exception("UF da Empresa não cadastrado.");
                }

                ////Busca tipo de movimentacao parametrizada para dentro e fora do estado
                //r = csBanco.Query("SELECT TipMovPedPALMFE, TipMovPedPALM FROM PARAMETROS WHERE CODEMP = " + codEmp + "");
                //if (r.Read())
                //{
                //    if (sUFCliente != sUFEmpresa) //fora do estado
                //    {
                //        return Convert.ToInt16(r[0].ToString());  //retorna o campo TipMovPedPALMFE
                //    }
                //    else //dentro do estado
                //    {
                //        return Convert.ToInt16(r[1].ToString()); //retorna o campo TipMovPedPALM
                //    }
                    
                //}

                
                r = csBanco.Query("SELECT TIPO FROM TMP_MOVIMENTACOES_WEB WHERE CODIGO = " + iMovimentacao + "");
                if (r.Read())
                {
                    if (sUFCliente != sUFEmpresa) //fora do estado
                    {
                        if (r[0].ToString() == "F")
                        {
                            return iMovimentacao;
                        }
                        else
                        {
                            throw new Exception("Movimentação Selecionada é inválida para vendas fora do estado.");                            
                        }
                        
                    }
                    else //dentro do estado
                    {
                        if (r[0].ToString() == "D")
                        {
                            return iMovimentacao;
                        }
                        else
                        {
                            throw new Exception("Movimentação Selecionada é inválida para vendas dentro do estado.");
                        }
                    }

                }

            r.Close();                    
            return 0;
    
        }

        public static PEDIDO Pedido(int NumPed, int CodEmp, int CodVen)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            PEDIDO p = null;

            p = dcdc.PEDIDOs.FirstOrDefault(ped => ped.NumPed == NumPed && ped.CodVen == CodVen && ped.CodEmp == CodEmp);

            return p;
        }

        public static List<PEDIDO> Pedidos()
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            return dcdc.PEDIDOs.ToList();
        }

        public static List<ItemPedidoResumido> ItensPedido(PEDIDO pedido, UsuarioResumido u, ParametroResumido pr, Int16 CodPrcTab, Int16 CodPrzTab)
        {
            
            List<ItemPedidoResumido> itens = new List<ItemPedidoResumido>();
            List<ITENSPED> itensPed = new DataClassesDataContext().ITENSPEDs.Where(i => i.NumPed == pedido.NumPed && i.CodEmp == u.CodEmp).ToList();
            foreach (ITENSPED item in itensPed)
            {
                ItemPedidoResumido i = new ItemPedidoResumido();
                i.CodServMerc = item.CodServMerc;
                i.Item = item.Item;
                i.NumPed = item.NumPed;

                /*52033*/
                if (pedido.SitPed.Equals("EXE"))
                {
                    i.Qtd = item.QtdFat == null ? 0 : (decimal)item.QtdFat; 
                }
                else
                {
                    i.Qtd = (decimal)item.Qtd;
                }
                                
                if (item.UniVenda == 2)
                {
                    i.PrecoLiquido = (decimal)item.VlrUni;
                    i.PrecoLiquido = (decimal)item.VLRUNIVENDALIQ / i.Qtd;
                    i.PrecoProduto = (decimal)item.VlrUni;
                }
                else
                {
                    i.PrecoLiquido = item.VLRUNIVENDALIQ == null ? 0 : (decimal)item.VLRUNIVENDALIQ;
                    i.PrecoProduto = ((decimal)item.VLRUNIVENDA); //preco total
                }

                i.Desconto = (decimal)item.VlrDes;
                

                if (item.UniVenda==2)
                {
                    i.Total = (item.VLRUNIVENDALIQ == null ? 0 : (decimal)item.VLRUNIVENDALIQ * (decimal)item.QTDUNIVENDA);
                }
                else
                {
                    i.Total = (item.VLRUNIVENDALIQ == null ? 0 : (decimal)item.VLRUNIVENDALIQ * i.Qtd);
                }

                try
                {
                    //List<ProdutoResumido> lpr = ClasseProdutos.Produto(u, item.CodServMerc, pr, CodPrcTab, CodPrzTab);
                    List<ProdutoResumido> lpr = ClasseProdutos.buscaProdutosPorCodigo(u, pr, item.CodServMerc, CodPrcTab, CodPrzTab);
                    if (lpr != null)
                    {
                        i.Saldo = (int)(lpr.Count < 1 ? 0 : lpr[0].Saldo / 1);
                        //i.DescricaoProduto = ClasseProdutos.Produto(u, item.CodServMerc, pr, CodPrcTab, CodPrzTab).Count < 1 ? "" : ClasseProdutos.Produto(u, item.CodServMerc, pr, CodPrcTab, CodPrzTab)[0].Nome;
                        i.DescricaoProduto = ClasseProdutos.buscaProdutosPorCodigo(u, pr, item.CodServMerc, CodPrcTab, CodPrzTab).Count < 1 ? "" : ClasseProdutos.buscaProdutosPorCodigo(u, pr, item.CodServMerc, CodPrcTab, CodPrzTab)[0].Nome;
                        i.Peso = Convert.ToDecimal(lpr[0].Peso.ToString()) * i.Qtd;

                    }
                    else
                    {
                        i.Saldo = 0;
                        i.Peso = 0;
                        i.DescricaoProduto = "PRODUTO NÃO ENCONTRADO OU INATIVO";
                    }
                }
                catch (Exception excp)
                {
                    i.Saldo = 0;
                    i.Peso = 0;
                    i.DescricaoProduto = "PRODUTO NÃO ENCONTRADO OU INATIVO";
                }
                
                itens.Add(i);
            }
            return itens;
        }

        public static Int32 ProximoPedido(UsuarioResumido u)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            ClasseBanco csBanco = new ClasseBanco();

            //pega ultimo pedido disponivel na PARAMETROS
            int numPedido = (int)dcdc.PARAMETROs.Single(prm => prm.CodEmp == (short)u.CodEmp).NumPedIni;

            //Proximo Pedido
            numPedido++;

            //Verifica se o pedido existe
            var r = csBanco.Query("SELECT 1 FROM PEDIDO WHERE CODEMP  = " + (short)u.CodEmp + " AND NUMPED = " + numPedido + "");
            if (r.Read())
            {
                r.Dispose();

                var rTemp = csBanco.Query("SELECT MAX(NUMPED) FROM PEDIDO WHERE CODEMP  = " + (short)u.CodEmp + "");
                if (rTemp.Read())
                {
                    numPedido = Convert.ToInt32(rTemp[0].ToString()) + 1;
                }
                rTemp.Dispose();
            }
            else
            {
                r.Dispose();
            }


            //Atualiza PARAMETROS  
            dcdc.PARAMETROs.Single(prm => prm.CodEmp == (short)u.CodEmp).NumPedIni = numPedido;

            //Atualiza NUMPED            
            csBanco.ExecutarComando("UPDATE NUMPED SET NUMPED = " + (numPedido) + "");

            //Commit
            dcdc.SubmitChanges();

            return numPedido;
        }

        public static Int32 ProximoPedidoWeb(UsuarioResumido u)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();

            Int32 NumPedWeb = dcdc.NUMPEDs.SingleOrDefault(np => np.CodEmp.Equals(u.CodEmp)).NumPedWeb;
            NumPedWeb++;

            dcdc.NUMPEDs.SingleOrDefault(np => np.CodEmp.Equals(u.CodEmp)).NumPedWeb = NumPedWeb;
            dcdc.SubmitChanges();

            return NumPedWeb;
        }

        public static Int32 QuantidadePedidos(Int32 codVend, Int32 codEmp)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();

            var var_resultado = dcdc.PEDIDOs.Where(vd => vd.CodVen == codVend)
                .Where(vd => vd.CodEmp == codEmp)
                .Where(vd => vd.NumPedWeb > 0 )
                .Where(vd => vd.SitPed == "ABE").Count();

            return var_resultado;
        }

        public static Boolean ReservarItemPedidoWeb(UsuarioResumido Usuario, Int32 NumPedWeb, Int32 CodServMerc, Decimal Quantidade, ParametroResumido pr)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();

            //Decimal? Saldo = Convert.ToDecimal(dcdc.SERVMERCs.SingleOrDefault(sm => sm.CodServMerc.Equals((Int32)CodServMerc)).Saldo);
            //Decimal? QtdRes = Convert.ToDecimal(dcdc.SERVMERCs.SingleOrDefault(sm => sm.CodServMerc.Equals(CodServMerc)).QtdRes);
            Decimal? Saldo = 0;
            Decimal? QtdRes = 0;
            
            ClasseBanco csBanco = new ClasseBanco();

            var r = csBanco.Query("SELECT Saldo, QtdRes FROM SERVMERC WHERE CODSERVMERC = " + CodServMerc + "");
            if (r.Read())
            {
                Saldo = Convert.ToDecimal(r["SALDO"].ToString());
                QtdRes = Convert.ToDecimal(r["QtdRes"].ToString());
            }
            r.Close();
            r.Dispose();
            

            // verifica se a quantidade da mercadoria solicitada está disponível em estoque //
            if (pr.SaldoPed == 'S')
            {
                if ((Saldo - (QtdRes <= 0 ? 0 : QtdRes)) < Quantidade)
                {
                    throw new Exception("Quantidade indisponível da(s) mercadoria(s) solicitada(s). Se foi feita importação de Planilha/Arquivo OffLine, favor verificar a listagem de itens não atendidos.");
                }
            }

            // retorna os itens da mercadoria selecionada em outros pedidos  //
            var Itens = (from o in dcdc.ITPEDIDOWEBs
                         where (o.CODSERVMERC == CodServMerc)
                         where (o.CODEMP != Usuario.CodEmp || o.CODUSU != Usuario.CodUsu)
                         select new
                         {
                             o.CODSERVMERC,
                             o.QTDE
                         }).ToList();

            // soma a quantidade dos itens retornados
            var Soma = (Decimal)Itens.Select(c => c.QTDE).Sum();


            /* FABIANO -------------------------------------------------------------------------------
             Verifica saldo - reserva do produto - reserva de produto via web (ITPEDIDOWEB)
            ------------------------------------------------------------------------------------------*/

            if (pr.SaldoPed == 'S')
            {
                // se a soma dos itens em reserva for maior que a quantidade //
                if (((Saldo - QtdRes) - Soma) < Quantidade)
                {
                    throw new Exception("Quantidade solicitada é superior ao saldo disponível da mercadoria selecionada.");
                }
            }

            // ajusta as propriedades do Item
            ITPEDIDOWEB Item = new ITPEDIDOWEB();
            Item.NUMPEDWEB = NumPedWeb;
            Item.CODSERVMERC = CodServMerc;
            Item.CODEMP = Usuario.CodEmp;
            Item.CODUSU = Usuario.CodUsu;
            Item.QTDE = (Decimal)Quantidade;

            // grava o item na tabela //
            dcdc.ITPEDIDOWEBs.InsertOnSubmit(Item);
            dcdc.SubmitChanges();
            //dcdc.Dispose();

            //FABIANO
            //GRAVAR NA SERVMERC A QTD. RESERVADA NO MOMENTO DA INCLUSAO DO ITEM
            //SERVMERC prod = dcdc.SERVMERCs.Single(p => p.CodServMerc == CodServMerc);
            //prod.QtdRes = (prod.QtdRes == null ? (Convert.ToDouble(Quantidade)) : (prod.QtdRes + Convert.ToDouble(Quantidade)));
            //dcdc.SubmitChanges(); 
            String sQuantidade = Quantidade.ToString();//51747
            csBanco.ExecutarComando(" UPDATE SERVMERC SET QTDRES = ISNULL(QTDRES,0) + " + sQuantidade.Replace(',','.') + " WHERE CODSERVMERC = " + CodServMerc + "");

            dcdc.Dispose();

            return true;

        }

        public static Boolean RemoverReservaItemPedidoWeb(UsuarioResumido Usuario, Int32 NumPedWeb, Int32 CodServMerc)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            var Item = dcdc.ITPEDIDOWEBs.FirstOrDefault(p => p.NUMPEDWEB == NumPedWeb && p.CODSERVMERC == CodServMerc && p.CODEMP == Usuario.CodEmp && p.CODUSU == Usuario.CodUsu);

            dcdc.ITPEDIDOWEBs.DeleteOnSubmit(Item);

            //EXECUTA PROCEDURE PARA CORRIGIR QTD RESERVADA SE O PEDIDO NAO FOI GRAVADO.
            dcdc.sp_corrige_reserva_webpedido();

            dcdc.SubmitChanges();
            dcdc.Dispose();

            return true;
        }

        public static Boolean RemoverReservaItensPedidoWeb(UsuarioResumido Usuario)
        {
            DataClassesDataContext dcdc = new DataClassesDataContext();
            var Itens = dcdc.ITPEDIDOWEBs.Where(p => p.CODEMP == Usuario.CodEmp && p.CODUSU == Usuario.CodUsu);

            dcdc.ITPEDIDOWEBs.DeleteAllOnSubmit(Itens);

            //EXECUTA PROCEDURE PARA CORRIGIR QTD RESERVADA SE O PEDIDO NAO FOI GRAVADO.
            dcdc.sp_corrige_reserva_webpedido();

            dcdc.SubmitChanges();
            dcdc.Dispose();

            return true;
        }

        public static void GeraXml(UsuarioResumido u, ParametroResumido pr )
        {
                        
            ClasseBanco conn    = new ClasseBanco();
            ClasseBanco conn2   = new ClasseBanco();
            ClasseBanco conn3 = new ClasseBanco();
            StringBuilder sCmd  = new StringBuilder();

            conn.AbrirBanco();
            conn2.AbrirBanco();
            conn3.AbrirBanco();

            String vrGeraParcelas   = "";
            String vrFormaPagto     = "";
            var sCamposCondicao     = "";
            var sDescricaoCombo     = "";
            StringBuilder strSql    = new StringBuilder();

            sCmd.Length     = 0;
            strSql.Length   = 0;

            /*
             * "Mercadoria"                                         = 0
               "Mercadoria + Unidade"                               = 1
               "Mercadoria + Preço"                                 = 2
               "Mercadoria + Qtd. Emb/Cx. Emb"                      = 3
               "Mercadoria + Qtd. Emb/Cx. Emb + Unidade + Preço"    = 4
             */
            try
            {

                #region CondMov
                //53190
                strSql.Length = 0;

                String sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\condmov.xml");

                strSql.Append(" SELECT CODTIPMOV, CODFRMPGT, CODTIPPRZ ");
                strSql.Append(" FROM CONDMOV ");
                
                var r = conn.retornaQueryDataSet(strSql.ToString());

                //Criando o arquivo XML
                using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                {
                    //Escrevendo no documento
                    r.WriteXml(xmlDoc);
                }

                r.Dispose();

                #endregion      

                #region Produtos

                strSql.Length = 0;

                sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\produtos.xml");

                //strSql.Append(" SELECT replicate(0, 9 - Len(CAST(S.CODSERVMERC AS VARCHAR))) + CAST(S.CODSERVMERC AS VARCHAR) as CODSERVMERC,  ");
                //strSql.Append(" (rtrim(S.DesServMerc) + ' ' + rtrim(upper(Isnull(CodSec_ServMerc,''))) + ' - Qt. Emb.' + convert(varchar,Isnull(convert(int,S.QtdEmb),0))) + ' / Cx. Emb.' + convert(varchar,Isnull(convert(int,S.QtdCAIXA),0)) AS DESSERVMERC, ");
                //strSql.Append(" ISNULL(T.PRECO,0) AS Preco,  ");
                //strSql.Append(" (ISNULL(SALDO,0) - ISNULL(QTDRES,0) ) AS SALDO, ");
                //strSql.Append(" UNIDADE, ISNULL(M_UNIDADE,'') AS M_UNIDADE, ISNULL(QTDCAIXA,0) AS QTDCAIXA, ");
                //strSql.Append(" CASE WHEN ISNULL(DESCCLIENTE,0) > 0 THEN DESCCLIENTE ELSE percdesc END AS PERCDESC ");
                //strSql.Append(" FROM  SERVMERC S INNER JOIN TABPRECO T ON S.CODSERVMERC = T.CODSERVMERC  LEFT JOIN REGRAS_PRODUTO R ON S.CODSERVMERC = R.CODPRO  ");
                //strSql.Append(" WHERE ISNULL(R.REGRA1,0) = 0 AND T.CODEMP = " + u.CodEmp + " ");

                //if (pr.CodTipPrc != 0 && pr.CodTipPrz != 0)
                //{
                //    strSql.Append(" AND T.CodTipPrc = " + pr.CodTipPrc + " AND T.CodTipPrz = " + pr.CodTipPrz + " ");
                //}
                //strSql.Append(" AND S.ATIVO = 'S' ORDER BY DESSERVMERC ");


                strSql.Append(" SELECT replicate(0, 9 - Len(CAST(S.CODSERVMERC AS VARCHAR))) + CAST(S.CODSERVMERC AS VARCHAR) as CODSERVMERC,  ");

                if (pr.LayoutCombo == 0 || pr.LayoutCombo == 1 || pr.LayoutCombo == 2)
                {
                    strSql.Append(" rtrim(S.DesServMerc) AS DESSERVMERC, ");
                }
                else if (pr.LayoutCombo == 3 || pr.LayoutCombo == 4)
                {
                    strSql.Append(" (rtrim(S.DesServMerc) + ' ' + rtrim(upper(Isnull(CodSec_ServMerc,''))) + ' - Qt. Emb.' + convert(varchar,Isnull(convert(int,S.QtdEmb),0))) + ' / Cx. Emb.' + convert(varchar,Isnull(convert(int,S.QtdCAIXA),0)) AS DESSERVMERC, ");
                }
                
                strSql.Append(" ISNULL(S.PRECOBASE,0) AS Preco,  ");
                strSql.Append(" (ISNULL(SALDO,0) - ISNULL(QTDRES,0) ) AS SALDO, ");
                strSql.Append(" UNIDADE, ISNULL(M_UNIDADE,'') AS M_UNIDADE, ISNULL(QTDCAIXA,0) AS QTDCAIXA, ");
                strSql.Append(" CASE WHEN ISNULL(DESCCLIENTE,0) > 0 THEN DESCCLIENTE ELSE ISNULL(percdesc,0) END AS PERCDESC, S.CODEMP_SERVMERC, ISNULL(S.CodSec_ServMerc,'') AS CodSec_ServMerc");
                strSql.Append(" FROM  SERVMERC S LEFT JOIN REGRAS_PRODUTO R ON S.CODSERVMERC = R.CODPRO  ");
                strSql.Append(" WHERE ISNULL(R.REGRA1,0) = 0 ");
                strSql.Append(" AND S.ATIVO = 'S' ORDER BY DESSERVMERC ");

                r = conn.retornaQueryDataSet(strSql.ToString());

                //Criando o arquivo XML
                using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                {
                    //Escrevendo no documento
                    r.WriteXml(xmlDoc);
                }

                r.Dispose();
                #endregion

                #region Parametros

                strSql.Length = 0;

                sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\parametros.xml");

                strSql.Append(" SELECT CodEmp, CondicaoTabLivreWeb, ExibirRazaoSocial, PARA_UNIDADEVENDA, IPExterno, IPInterno, HostFtp, FtpUsuario, FtpSenha, PastaServidor, LayoutCombo, MostraTodosProdutos ");
                strSql.Append(" FROM PARAMETROS ");
                /*51849 - Enviar dados de todas as empresas*/
                //strSql.Append(" WHERE 1=1 AND  ");
                //strSql.Append(" CODEMP = " + u.CodEmp);

                r = conn.retornaQueryDataSet(strSql.ToString());

                //Criando o arquivo XML
                using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                {
                    //Escrevendo no documento
                    r.WriteXml(xmlDoc);
                }

                r.Dispose();

                #endregion                

                #region Condições de Pagamento
                //Cria temporaria para as condicoes
                sCmd.Append("if exists (select * From dbo.sysobjects where id = object_id(N'[dbo].[TMP_CONDPAGTO]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)");
                sCmd.Append("   DROP TABLE dbo.TMP_CONDPAGTO");
                conn.ExecutarComando(sCmd.ToString());

                sCmd.Length = 0; //Reset variavel

                sCmd.Append(" CREATE TABLE dbo.TMP_CONDPAGTO ( ");
                sCmd.Append(" CODEMP         int          NOT NULL ,"); /*51849*/
                sCmd.Append(" CODFRMPGT      int          NOT NULL ,");
                sCmd.Append(" DESFRMPGT      varchar(100) NOT NULL ,");
                sCmd.Append(" CODTIPPRZ      int          NOT NULL ,");
                sCmd.Append(" PRAZOTAB       int              NULL , ");
                sCmd.Append(" DESTIPPRZ      varchar(100) NOT NULL ) ");

                conn.ExecutarComando(sCmd.ToString());

                sCmd.Length = 0; //Reset variavel

                strSql.Length = 0;
                //Condicao de Pagamento
                /*51849 - Acrescentado CODEMP*/
                strSql.Append(" SELECT DISTINCT FORMAPAGTO.CodFrmPgt, FORMAPAGTO.DesFrmPgt, TIPOPRAZO.CodTipPrz, TIPOPRAZO.DesTipPrz, FORMAPAGTO.GeraParcelas, ITCONPAGTO.PRAZOTAB, CODEMP ");
                strSql.Append(" FROM ITCONPAGTO, FORMAPAGTO, TIPOPRAZO ");
                strSql.Append(" WHERE ");
                strSql.Append(" ITCONPAGTO.CodFrmPgt = FORMAPAGTO.CodFrmPgt  AND ");
                strSql.Append(" ITCONPAGTO.CodTipPrz = TIPOPRAZO.CodTipPrz  AND ");
                //strSql.Append(" Codemp = " + u.CodEmp + " AND "); /*51849*/
                strSql.Append(" FILTROVENDA = 1 ");
                strSql.Append(" AND ITCONPAGTO.PALM = 1 ");
                //strSql.Append(" ORDER BY FORMAPAGTO.CodFrmPgt DESC, TIPOPRAZO.CodTipPrz DESC"); //52688
                strSql.Append(" ORDER BY FORMAPAGTO.CodFrmPgt, TIPOPRAZO.CodTipPrz");
                var rsCondicao = conn.Query(strSql.ToString());

                while (rsCondicao.Read())
                {
                    //Verificacao para nao ficar repetindo as parcelas
                    if (vrGeraParcelas == "S" && rsCondicao[0].ToString() == vrFormaPagto)
                    {
                        vrGeraParcelas = rsCondicao[4].ToString();
                    }
                    else
                    {
                        sCamposCondicao = (rsCondicao[0].ToString() + "|" + rsCondicao[2].ToString().Trim()); ;
                        sDescricaoCombo = (rsCondicao[0] + " - " + rsCondicao[1] + " ==> " + rsCondicao[2] + " - " + rsCondicao[3]);

                        vrFormaPagto = rsCondicao[0].ToString();
                        vrGeraParcelas = rsCondicao[4].ToString();

                        /*51849 - Acrescentado CODEMP*/
                        sCmd.Append(" INSERT INTO TMP_CONDPAGTO (CODFRMPGT, DESFRMPGT, CODTIPPRZ, DESTIPPRZ, PRAZOTAB, CODEMP) ");
                        sCmd.Append(" VALUES ( " + rsCondicao[0].ToString() + ", ");
                        sCmd.Append(" '" + rsCondicao[1].ToString().Trim() + "', ");
                        sCmd.Append(" " + rsCondicao[2].ToString() + ", ");
                        sCmd.Append(" '" + rsCondicao[3].ToString().Trim() + "', " + rsCondicao[5].ToString() + ", " + rsCondicao[6].ToString() + " ); ");

                    }
                }
                rsCondicao.Dispose();

                if (sCmd.ToString() == "")
                {
                    throw new Exception("Não há condições de pagamento parametrizadas para uso na WEB/PALM");
                }
                else
                {
                    conn.ExecutarComando(sCmd.ToString()); //Grava na temporaria 
                }

                r = conn.retornaQueryDataSet("SELECT * FROM TMP_CONDPAGTO ORDER BY CODFRMPGT");

                sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\condpagto.xml");

                //Criando o arquivo XML
                using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                {
                    r.WriteXml(xmlDoc); //Escrevendo no documento
                }

                r.Dispose();

                #endregion

                #region Clientes e Condições de Pagamento do Cliente

                var rsVendedores = conn2.Query("SELECT CODUSU, CODVEND FROM VENDEDOR WHERE STATUS = 'A' AND (CodUsu IS NOT NULL AND CodUsu <> 0)");

                string sCampo;

                //51516
                if (pr.ExibirRazaoSocial == 1)
                {
                    sCampo = "C.RAZSOC";
                }
                else
                {
                    sCampo = "C.NOMFAN";
                }

                while (rsVendedores.Read())
                {
                    //Gera Clientes por vendedor
                    //o arquivo será por exemplo 514clientes.xml (Para o vendedor 514)
                    
                    /*51849 - Acrescentado CODEMP*/
                    sCmd.Length = 0;
                    sCmd.Append(" SELECT " + sCampo + " AS RAZSOC, C.CODCLI, RTRIM(ISNULL(C.STATUSCLIENTE,'LB')) AS STATUSCLIENTE, ESTCLI AS UF, C.CGC_CPF AS CNPJ, ISNULL(CLIE_TABPRECOPADRAO,0) AS TAB_PADRAO, V.CODEMP ");
                    sCmd.Append(" , F.LimCred, F.VlrDeb "); //Limite de crédito (se houver)
                    sCmd.Append(" FROM CLIENTE C ");
                    sCmd.Append(" INNER JOIN VENDCLI V on C.CodCli = V.CodCli");
                    sCmd.Append(" INNER JOIN VENDEDOR VE ON VE.CodVend = V.CodVen");
                    sCmd.Append(" LEFT JOIN FINANCLI F ON F.CODCLI = C.CODCLI");
                    sCmd.Append(" WHERE C.Ativo != 'N'");
                    sCmd.Append(" AND c.Prospect != 'S'");
                    sCmd.Append(" AND VE.Status = 'A'");
                    sCmd.Append(" AND VE.CodUsu = " + rsVendedores["CODUSU"] + "");
                    //sCmd.Append(" AND V.CodEmp = " + u.CodEmp + ""); /*51849 - Enviar dados de todas as empresas*/
                    sCmd.Append(" ORDER BY " + sCampo + "");

                    r = conn.retornaQueryDataSet(sCmd.ToString());

                    sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\" + rsVendedores["CODUSU"] + "clientes.xml");

                    //Criando o arquivo XML
                    using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                    {
                        r.WriteXml(xmlDoc);
                    }
                    r.Dispose();

                    //CONDICOES DE PAGAMENTO DO CLIENTE
                    /*51849 - Acrescentado CODEMP*/
                    strSql.Length = 0;
                    strSql.Append("SELECT DISTINCT CONDCLI.CodCli, CONDCLI.CodFrmPgt AS CODTIPPRC, CONDCLI.CodTipPrz, VENDCLI.CODEMP ");
                    strSql.Append("FROM CONDCLI ");
                    strSql.Append("INNER JOIN CLIENTE ON CONDCLI.CodCli = CLIENTE.CodCli ");
                    strSql.Append("INNER JOIN VENDCLI ON VENDCLI.CodCli = CONDCLI.CODCLI ");
                    strSql.Append("WHERE CLIENTE.Ativo = 'S' AND VENDCLI.CODVEN=" + rsVendedores["CODVEND"] + "");

                    sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\" + rsVendedores["CODUSU"] + "condcli.xml");
                    r = conn.retornaQueryDataSet(strSql.ToString());

                    //Criando o arquivo XML
                    using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                    {
                        r.WriteXml(xmlDoc);
                    }
                    r.Dispose();


                    //Enviar titulos do cliente do vendedor                    
                    //TITULOS DO CLIENTE                    
                    sCmd.Length = 0;
                    sCmd.Append(" SELECT T.CodEmp, T.CodCli, T.NumDoc, T.NumPar, T.Serie, T.Status, T.VlrDoc, Saldo = (T.VlrDoc - T.VlrPago + T.AcrFin - (T.Desc1 + T.Desc2)), CONVERT(char,T.DtaVen,103) as DtaVen, CONVERT(char,T.DtaEmi,103) as DtaEmi, T.QtdPar, T.Obs, Datediff(d,T.DtaVen,GETDATE()) AS atraso ");                    
                    sCmd.Append(" FROM TITRECEB T ");
                    sCmd.Append(" INNER JOIN CLIENTE C on C.CodCli = T.CodCli");
                    sCmd.Append(" INNER JOIN VENDCLI V on C.CodCli = V.CodCli");
                    sCmd.Append(" INNER JOIN VENDEDOR VE ON VE.CodVend = V.CodVen");
                    sCmd.Append(" WHERE C.Ativo != 'N'");
                    sCmd.Append(" AND C.Prospect != 'S'");
                    sCmd.Append(" AND VE.Status = 'A'");
                    sCmd.Append(" AND VE.CodUsu = " + rsVendedores["CODUSU"] + "");
                    sCmd.Append(" AND T.TIPO = 'R' AND T.STATUS <> 'B'");

                    sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\" + rsVendedores["CODUSU"] + "titulos.xml");
                    r = conn.retornaQueryDataSet(sCmd.ToString());

                    //Criando o arquivo XML
                    using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                    {
                        r.WriteXml(xmlDoc);
                    }
                    r.Dispose();
                    
                }
                rsVendedores.Close();
                #endregion

                #region Tabelas de Preço
                rsVendedores = conn2.Query("SELECT CODUSU, CODVEND FROM VENDEDOR WHERE STATUS = 'A' AND (CodUsu IS NOT NULL AND CodUsu <> 0)");

                while (rsVendedores.Read())
                {
                    /*Verifica se há tabelas de preço especifica para o vendedor*/
                    var temp = conn.retornaQueryDataSet("SELECT 1 FROM TABVENDEDOR WHERE IDTABVENDEDOR = " + rsVendedores["CODVEND"] + "");

                    if (temp.Tables[0].Rows.Count <= 0) //Se não existir, gera tabela de preço padrão
                    {                        

                        /*51489 - Acrescentado tratamento para levar todas empresas. Gerar arquivos separados por questões de performance*/
                        var rsEmpresas = conn3.Query("SELECT CODEMP FROM EMPRESA ");

                        while (rsEmpresas.Read())
                        {
                            sCmd.Length = 0;
                            sCmd.Append("  SELECT DISTINCT " + rsVendedores["CODUSU"] + " AS CODVEND, 0 AS IDTABVENDEDOR, P.CODTIPPRC, P.CODTIPPRZ, rtrim(TP.DESTIPPRC) AS DESTIPPRC, rtrim(TPZ.DESTIPPRZ) AS DESTIPPRZ, replicate(0, 9 - Len(CAST(TABPRECO.CODSERVMERC AS VARCHAR))) + CAST(TABPRECO.CODSERVMERC AS VARCHAR) as CODSERVMERC, TABPRECO.PRECO, ISNULL(TABPRECO.PercDescMax,0) AS PERCMAX, TABPRECO.CODEMP ");
                            sCmd.Append("  FROM PARAMETROS P ");
                            sCmd.Append("  INNER JOIN TIPO_PRECO TP ON TP.CodTipPrc = P.CODTIPPRC ");
                            sCmd.Append("  INNER JOIN TIPOPRAZO TPZ ON TPZ.CodTipPrz = P.CodTipPrz ");
                            sCmd.Append("  INNER JOIN TABPRECO ON TABPRECO.CodTipPrc = P.CODTIPPRC AND TABPRECO.CodTipPrz = P.CodTipPrz AND TABPRECO.CODEMP = P.CODEMP"); /*51849 - Acrescentado CODEMP*/
                            sCmd.Append("  INNER JOIN SERVMERC S ON S.CODSERVMERC = TABPRECO.CODSERVMERC ");
                            sCmd.Append("  WHERE S.ATIVO = 'S' AND TABPRECO.PRECO IS NOT NULL AND TABPRECO.CODEMP = " + rsEmpresas["CODEMP"].ToString() + ""); 

                            temp = conn.retornaQueryDataSet(sCmd.ToString());

                            sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\" + rsVendedores["CODUSU"] + "_" + rsEmpresas["CODEMP"].ToString() + "_tabpreco.xml");

                            //Criando o arquivo XML
                            using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                            {
                                temp.WriteXml(xmlDoc);
                            }
                            temp.Dispose();
                        }
                        rsEmpresas.Close();
                        rsEmpresas.Dispose();
                        
                    }
                    else //caso contrário, leva as tabelas especificas...
                    {                        
                        /*51489 - Acrescentado tratamento para levar todas empresas. Gerar arquivos separados por questões de performance*/
                        var rsEmpresas = conn3.Query("SELECT CODEMP FROM EMPRESA ");

                        while (rsEmpresas.Read())
                        {
                            sCmd.Length = 0;

                            /*Tabela de Preco*/
                            sCmd.Append(" SELECT DISTINCT " + rsVendedores["CODUSU"] + " AS CODVEND, TV.IDTABVENDEDOR, TV.IDTABELA AS CODTIPPRC, TV.CODTIPPRZ, rtrim(TP.DESTIPPRC) AS DESTIPPRC , rtrim(TPZ.DESTIPPRZ) AS DESTIPPRZ, replicate(0, 9 - Len(CAST(TABPRECO.CODSERVMERC AS VARCHAR))) + CAST(TABPRECO.CODSERVMERC AS VARCHAR) as CODSERVMERC, TABPRECO.PRECO, ISNULL(TABPRECO.PercDescMax,0) AS PERCMAX, TABPRECO.CODEMP  ");
                            sCmd.Append("  FROM TABVENDEDOR TV ");
                            sCmd.Append("  INNER JOIN TIPO_PRECO TP ON TP.CodTipPrc = TV.IDTABELA ");
                            sCmd.Append("  INNER JOIN TIPOPRAZO TPZ ON TPZ.CodTipPrz = TV.CodTipPrz ");
                            sCmd.Append("  INNER JOIN TABPRECO ON TABPRECO.CodTipPrc = TV.IDTABELA AND TABPRECO.CodTipPrz = TV.CodTipPrz");
                            sCmd.Append("  INNER JOIN SERVMERC S ON S.CODSERVMERC = TABPRECO.CODSERVMERC ");
                            sCmd.Append("  WHERE S.ATIVO = 'S' AND TABPRECO.PRECO IS NOT NULL AND TV.IDTABVENDEDOR = " + rsVendedores["CODVEND"] + " AND TABPRECO.CODEMP = " + rsEmpresas["CODEMP"].ToString() + " ");  /*51849 - Retirado filtro CODEMP, enviar dados de todas empresas*/

                            temp = conn.retornaQueryDataSet(sCmd.ToString());

                            sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\" + rsVendedores["CODUSU"] + "_" + rsEmpresas["CODEMP"] + "_tabpreco.xml");

                            //Criando o arquivo XML
                            using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                            {
                                temp.WriteXml(xmlDoc);
                            }
                            temp.Dispose();
                        }
                        rsEmpresas.Close();
                        rsEmpresas.Dispose();
                        
                    }
                }

                rsVendedores.Close();
                #endregion

                #region Tipos de Movimentacao

                //string sUFEmpresa = "";

                /*51849 - Retirado, pois sera tratado na query abaixo */
                /*
                var rsEmpresa = conn.Query("SELECT Estado FROM EMPRESA WHERE CODEMP = " + u.CodEmp);
                if (rsEmpresa.Read())
                {
                    sUFEmpresa = rsEmpresa[0].ToString();
                }
                rsEmpresa.Close();
                */

                //Cria temporaria para as condicoes
                sCmd.Length = 0; //Reset variavel
                sCmd.Append("if exists (select * From dbo.sysobjects where id = object_id(N'[dbo].[TMP_MOVIMENTACOES_WEB]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)");
                sCmd.Append("   DROP TABLE dbo.TMP_MOVIMENTACOES_WEB");
                conn.ExecutarComando(sCmd.ToString());

                sCmd.Length = 0; //Reset variavel
                sCmd.Append(" CREATE TABLE dbo.TMP_MOVIMENTACOES_WEB ( ");
                sCmd.Append(" CODEMP        int          NOT NULL ,");
                sCmd.Append(" CODIGO        int          NOT NULL ,");
                sCmd.Append(" DESCRICAO     varchar(255) NULL , ");
                sCmd.Append(" TIPO          char(1)      NULL , ");
                sCmd.Append(" GERA_VERBA    char(1)      NULL , ");                
                sCmd.Append(" UF_EMPRESA    varchar(2)   NULL ) ");
                conn.ExecutarComando(sCmd.ToString());

                sCmd.Length = 0;
                sCmd.Append(" SELECT P.CODEMP, ");
                sCmd.Append(" ISNULL(N1.GERAVERBA,'N') AS GERA_VERBA1, ");
                sCmd.Append(" ISNULL(N2.GERAVERBA,'N') AS GERA_VERBA2, ");
                sCmd.Append(" ISNULL(N3.GERAVERBA,'N') AS GERA_VERBA3, ");
                sCmd.Append(" ISNULL(N4.GERAVERBA,'N') AS GERA_VERBA4, ");
                sCmd.Append(" ISNULL(N5.GERAVERBA,'N') AS GERA_VERBA5, ");
                sCmd.Append(" ISNULL(N6.GERAVERBA,'N') AS GERA_VERBA6, ");
                sCmd.Append(" ISNULL(N7.GERAVERBA,'N') AS GERA_VERBA7, ");
                sCmd.Append(" ISNULL(N8.GERAVERBA,'N') AS GERA_VERBA8, ");
                sCmd.Append(" ISNULL(N9.GERAVERBA,'N') AS GERA_VERBA9, ");
                sCmd.Append(" ISNULL(N10.GERAVERBA,'N') AS GERA_VERBA10, ");
                sCmd.Append(" ISNULL(N11.GERAVERBA,'N') AS GERA_VERBA11, ");
                sCmd.Append(" ISNULL(N12.GERAVERBA,'N') AS GERA_VERBA12, ");
                sCmd.Append(" ISNULL(N13.GERAVERBA,'N') AS GERA_VERBA13, ");
                sCmd.Append(" ISNULL(N14.GERAVERBA,'N') AS GERA_VERBA14, ");
                sCmd.Append(" ISNULL(N15.GERAVERBA,'N') AS GERA_VERBA15, ");
                sCmd.Append(" ISNULL(N16.GERAVERBA,'N') AS GERA_VERBA16, ");
                sCmd.Append(" TipMovOrcPALM	AS COD1,	CASE ISNULL(N1.DESNATOPER_SUBST,'') WHEN '' THEN N1.DESNATOPE ELSE N1.DESNATOPER_SUBST END AS DESCRICAO1, 'D' AS TIPO1,   ");
                sCmd.Append(" TipMovPedPALM   AS COD2, CASE ISNULL(N2.DESNATOPER_SUBST,'') WHEN '' THEN N2.DESNATOPE ELSE N2.DESNATOPER_SUBST END  AS DESCRICAO2, 'D' AS TIPO2,  ");
                sCmd.Append(" TipMovBonificaPALM   AS COD3, CASE ISNULL(N3.DESNATOPER_SUBST,'') WHEN '' THEN N3.DESNATOPE ELSE N3.DESNATOPER_SUBST END  AS DESCRICAO3,'D' AS TIPO3,  ");
                sCmd.Append(" TipMovPropostaPALM   AS COD4, CASE ISNULL(N4.DESNATOPER_SUBST,'') WHEN '' THEN N4.DESNATOPE ELSE N4.DESNATOPER_SUBST END  AS DESCRICAO4,'D' AS TIPO4,  ");
                sCmd.Append(" TipMovTrocaPALM      AS COD5, CASE ISNULL(N5.DESNATOPER_SUBST,'') WHEN '' THEN N5.DESNATOPE ELSE N5.DESNATOPER_SUBST END  AS DESCRICAO5,'D' AS TIPO5,  ");
                sCmd.Append(" TipMovBonificaPALM2  AS COD6, CASE ISNULL(N6.DESNATOPER_SUBST,'') WHEN '' THEN N6.DESNATOPE ELSE N6.DESNATOPER_SUBST END  AS DESCRICAO6,'D' AS TIPO6,  ");
                sCmd.Append(" TipMovPropostaPALM2  AS COD7, CASE ISNULL(N7.DESNATOPER_SUBST,'') WHEN '' THEN N7.DESNATOPE ELSE N7.DESNATOPER_SUBST END  AS DESCRICAO7,'D' AS TIPO7,  ");
                sCmd.Append(" TipMovBonificaPALM3  AS COD8, CASE ISNULL(N8.DESNATOPER_SUBST,'') WHEN '' THEN N8.DESNATOPE ELSE N8.DESNATOPER_SUBST END  AS DESCRICAO8,'D' AS TIPO8,  ");
                sCmd.Append(" TipMovOrcPALMFE      AS COD9, CASE ISNULL(N9.DESNATOPER_SUBST,'') WHEN '' THEN N9.DESNATOPE ELSE N9.DESNATOPER_SUBST END  AS DESCRICAO9,'F' AS TIPO9,  ");
                sCmd.Append(" TipMovPedPALMFE      AS COD10, CASE ISNULL(N10.DESNATOPER_SUBST,'') WHEN '' THEN N10.DESNATOPE ELSE N10.DESNATOPER_SUBST END  AS DESCRICAO10,'F' AS TIPO10, ");
                sCmd.Append(" TipMovBonificaPALMFE AS COD11, CASE ISNULL(N11.DESNATOPER_SUBST,'') WHEN '' THEN N11.DESNATOPE ELSE N11.DESNATOPER_SUBST END  AS DESCRICAO11,'F' AS TIPO11,  ");
                sCmd.Append(" TipMovPropostaPALMFE AS COD12, CASE ISNULL(N12.DESNATOPER_SUBST,'') WHEN '' THEN N12.DESNATOPE ELSE N12.DESNATOPER_SUBST END  AS DESCRICAO12,'F' AS TIPO12,  ");
                sCmd.Append(" TipMovTrocaPALMFE    AS COD13, CASE ISNULL(N13.DESNATOPER_SUBST,'') WHEN '' THEN N13.DESNATOPE ELSE N13.DESNATOPER_SUBST END  AS DESCRICAO13,'F' AS TIPO13,  ");
                sCmd.Append(" TipMovBonificaPALMFE2 AS COD14 ,CASE ISNULL(N14.DESNATOPER_SUBST,'') WHEN '' THEN N14.DESNATOPE ELSE N14.DESNATOPER_SUBST END  AS DESCRICAO14,'F' AS TIPO14,  ");
                sCmd.Append(" TipMovPropostaPALMFE2 AS COD15 ,CASE ISNULL(N15.DESNATOPER_SUBST,'') WHEN '' THEN N15.DESNATOPE ELSE N15.DESNATOPER_SUBST END  AS DESCRICAO15,'F' AS TIPO15,  ");
                sCmd.Append(" TipMovBonificaPALMFE3 AS COD16 ,CASE ISNULL(N16.DESNATOPER_SUBST,'') WHEN '' THEN N16.DESNATOPE ELSE N16.DESNATOPER_SUBST END  AS DESCRICAO16, 'F' AS TIPO16, EMPRESA.Estado as UF_Empresa"); 
                sCmd.Append(" FROM  ");
                sCmd.Append(" PARAMETROS P INNER JOIN EMPRESA ON EMPRESA.CODEMP = P.CODEMP "); /*51849 - Acrescentado CODEMP*/
                sCmd.Append(" LEFT JOIN NATOPER N1 ON P.TipMovOrcPALM = N1.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N2 ON P.TipMovPedPALM = N2.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N3 ON P.TipMovBonificaPALM = N3.CODIGO ");
                sCmd.Append(" LEFT JOIN NATOPER N4 ON P.TipMovPropostaPALM = N4.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N5 ON P.TipMovTrocaPALM = N5.CODIGO	   ");
                sCmd.Append(" LEFT JOIN NATOPER N6 ON P.TipMovBonificaPALM2 = N6.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N7 ON P.TipMovPropostaPALM2 = N7.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N8 ON P.TipMovBonificaPALM3 = N8.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N9 ON P.TipMovOrcPALMFE = N9.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N10 ON P.TipMovPedPALMFE = N10.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N11 ON P.TipMovBonificaPALMFE = N11.CODIGO ");
                sCmd.Append(" LEFT JOIN NATOPER N12 ON P.TipMovPropostaPALMFE = N12.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N13 ON P.TipMovTrocaPALMFE = N13.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N14 ON P.TipMovBonificaPALMFE2 = N14.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N15 ON P.TipMovPropostaPALMFE2 = N15.CODIGO  ");
                sCmd.Append(" LEFT JOIN NATOPER N16 ON P.TipMovBonificaPALMFE3 = N16.CODIGO  ");
                //sCmd.Append(" WHERE CodEmp=" + u.CodEmp + "");/*51849 - Retirado filtro CODEMP*/

                var rsMov = conn.Query(sCmd.ToString());

                while (rsMov.Read())
                {
                    for (int i = 1; i <= 16; i++)
                    {
                        if (rsMov["COD" + Convert.ToString(i).ToString()].ToString() != "0")
                        {
                            /*51849 - Acrescentado CODEMP e UF_EMPRESA*/
                            conn2.ExecutarComando(" INSERT INTO TMP_MOVIMENTACOES_WEB (CODEMP, CODIGO, DESCRICAO, TIPO, UF_EMPRESA, GERA_VERBA) VALUES (" + rsMov["CODEMP"] + ", " + rsMov["COD" + Convert.ToString(i).ToString()] + ", '" + rsMov["DESCRICAO" + Convert.ToString(i).ToString()] + "', '" + rsMov["TIPO" + Convert.ToString(i).ToString()] + "', '" + rsMov["UF_EMPRESA"].ToString() + "', '" + rsMov["GERA_VERBA" + Convert.ToString(i).ToString()] + "')");
                        }
                    }
                }
                rsMov.Close();
                rsMov.Dispose();

                /*51849 - Acrescentado CODEMP*/
                var dsTemp = conn.retornaQueryDataSet("SELECT CODEMP, CODIGO, DESCRICAO, TIPO, UF_EMPRESA, '" + pr.CodTipMov_Estadual.ToString() + "' AS PADRAO, GERA_VERBA FROM TMP_MOVIMENTACOES_WEB ORDER BY DESCRICAO");
                sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\movimentacoes.xml");

                //Criando o arquivo XML
                using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                {
                    dsTemp.WriteXml(xmlDoc);
                }
                dsTemp.Dispose();

                #endregion

                #region Vendedores

                /*51849 - Acrescentado CODEMP*/
                r = conn.retornaQueryDataSet("SELECT U.CODEMP, U.CODUSU, SENHA FROM VENDEDOR V INNER JOIN USUARIOS U ON V.CODUSU = U.CODUSU WHERE V.STATUS = 'A' AND (V.CodUsu IS NOT NULL AND V.CodUsu <> 0)");

                sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\tabvend.xml");

                //Criando o arquivo XML
                using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                {
                    r.WriteXml(xmlDoc);
                }
                r.Dispose();
                
                rsVendedores.Close();
                #endregion

                #region Promoções Mercadorias

                strSql.Length = 0;

                sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\promocao.xml");

                /*51849 - Acrescentado CODEMP*/
                strSql.Append(" SELECT CODEMP, CODSERVMERC, CONVERT(CHAR, CONVERT(DateTime,DtaFim,103)) AS DtaFim, CONVERT(CHAR, CONVERT(DateTime,DtaIni,103)) AS DtaIni, PercProm, PrecoProm,Comissao ");
                strSql.Append(" FROM ITPROMOCAO ");
                strSql.Append(" WHERE 1=1 AND  ");
                strSql.Append(" Convert(DateTime, Convert(VarChar, GetDate(), 101)) >= Convert(DateTime, Convert(VarChar, DtaIni, 101)) AND  ");
                strSql.Append(" Convert(DateTime, Convert(VarChar, GetDate(), 101)) <= Convert(DateTime, Convert(VarChar, DtaFim, 101)) ");

                r = conn.retornaQueryDataSet(strSql.ToString());

                //Criando o arquivo XML
                using (StreamWriter xmlDoc = new StreamWriter(sCaminhodoArquivo))
                {
                    //Escrevendo no documento
                    r.WriteXml(xmlDoc);
                }

                r.Dispose();

                #endregion

            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
        }
      
    }
     
}