using System;

namespace WebPedidos.WSClasses
{
    public class ClasseComissao
    {
        private static Decimal _ComissaoUsada = 0;
        private static Decimal _ComissaoUsadaTele = 0;
        private static Decimal _FatorDesconto = 0;


        public static Decimal FatorDesconto
        {
            get { return _FatorDesconto; }
            set { _FatorDesconto = value; }
        }

        public static Decimal ComissaoUsada
        {
            get { return _ComissaoUsada; }
            set { _ComissaoUsada = value; }
        }

        public static Decimal ComissaoUsadaTele
        {
            get { return _ComissaoUsadaTele; }
            set { _ComissaoUsadaTele = value; }
        }

        public static Decimal CalculaComissao(ParametroResumido pr, 
            Decimal Unitario, 
            Decimal Qtd, 
            Decimal QtdCan, 
            Decimal QtdCanAnt, 
            Decimal Comissao_Vendedor, 
            Decimal Comissao_Tabela, 
            Decimal Comissao_Mercadoria, 
            Decimal Comissao_Grupo, 
            String  AvisoComissao, 
            Decimal VP_cPercCom, 
            Decimal VP_cValorCom, 
            Decimal ComissaoCliente, 
            Decimal Comissao_Televendedor,
            Int16   IncideVendedor,
            Int16   IncideTelevendedor,
            Int32   Produto,
            Decimal cDescontoItem
            )

        {

            Decimal Retorno             = 0;
            Decimal Retorno_tele        = 0;
            Boolean VR_Nenhuma          = true;
            Decimal fator               = 1;
            Decimal fator_televendas    = 1;

            ClasseBanco csBanco = new ClasseBanco();

            //Busca parametro sistema
            var r = csBanco.Query("SELECT USADESCONTO FROM PARAMETROS WHERE codemp = " + pr.CodEmp);

            if (r.Read())
            {
                if (r[0].ToString()=="S") //usa desconto
                {

                    string valorDesconto = Convert.ToString(cDescontoItem);

                    string sSql = " SELECT PERCDE, PERCATE, FATOR  FROM DESCONTOS_MERC " +
                                  " WHERE " +
                                  " DtaValInicial <= '" + String.Format("{0:s}", DateTime.Today) + "' AND " +
                                  " DtaValFim >= '" + String.Format("{0:s}", DateTime.Today) + "' AND " +
                                  " CODSERVMERC = " + Produto + " AND PERCDE <= " + valorDesconto.Replace(",", ".") + " AND PERCATE >= " + valorDesconto.Replace(",", ".") + " ";

                    r.Close();

                    var t = csBanco.Query(sSql);


                    while (t.Read())
                    {
                          if (IncideTelevendedor == 1)
                            {
                                fator_televendas = Convert.ToDecimal(t[2].ToString());
                                _FatorDesconto = fator_televendas;
                            }

                            if (IncideVendedor == 1)
                            {
                                fator = Convert.ToDecimal(t[2].ToString());
                                _FatorDesconto = fator;
                            }
                       
                    }
                    t.Close();
                                        
                }

            }
            

            //aplica fator de desconto as comissoes
            Decimal Comissao_Mercadoria_Televendas = Comissao_Mercadoria;

            Comissao_Vendedor = (Comissao_Vendedor * fator);
            Comissao_Tabela = (Comissao_Tabela * fator);
            Comissao_Mercadoria = (Comissao_Mercadoria * fator);            
            Comissao_Grupo = (Comissao_Grupo * fator);
            ComissaoCliente = (ComissaoCliente * fator);
            Comissao_Televendedor = (Comissao_Televendedor * fator_televendas);
            Comissao_Mercadoria_Televendas = (Comissao_Mercadoria_Televendas * fator_televendas);

            // Desconto em valor da base da comissão
            Unitario -= VP_cValorCom;

            // Desconto do valor em percentual
            Unitario -= ((Unitario / 100) * VP_cPercCom);

            if (AvisoComissao.Equals("S"))
            {
                if ((Comissao_Vendedor == 0) && (Comissao_Tabela == 0) && (Comissao_Mercadoria == 0) && (Comissao_Grupo == 0) && (ComissaoCliente == 0))
                {
                    throw new Exception("Mercadoria sem Comissão cadastrada.");
                }
            }

            _ComissaoUsada = 0;
            _ComissaoUsadaTele = 0;

            // Verifica quem é a prioridade 1
            if ((pr.Prioridade_Tab == 1) && (Comissao_Tabela != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Tabela / 100);
                _ComissaoUsada = Comissao_Tabela;

                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Repr == 1) && (Comissao_Vendedor != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Vendedor / 100);
                _ComissaoUsada = Comissao_Vendedor;

               //prioridade comissao do produto para o televendedor 
                if (Comissao_Mercadoria_Televendas != 0)
                {
                    Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria_Televendas / 100);
                    _ComissaoUsadaTele = Comissao_Mercadoria_Televendas;
                }
                else
                {
                    Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Televendedor / 100);
                    _ComissaoUsadaTele = Comissao_Televendedor;
                }

                
                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Merc == 1) && (Comissao_Mercadoria != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria / 100);
                _ComissaoUsada = Comissao_Mercadoria;

                Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria_Televendas / 100);
                _ComissaoUsadaTele = Comissao_Mercadoria_Televendas;

                VR_Nenhuma = false;
            }

            if ((pr.PARA_PriorGrupo == 1) && (Comissao_Grupo != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Grupo / 100);
                _ComissaoUsada = Comissao_Grupo;
                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Cliente == 1) && (ComissaoCliente != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * ComissaoCliente / 100);
                _ComissaoUsada = ComissaoCliente;
                VR_Nenhuma = false;
            }

            if (!VR_Nenhuma)
            {
                if (AvisoComissao == "T")
                {
                    return Retorno_tele; //RETORNA COMISSAO TELEVENDEDOR
                }
                else
                {
                    return Retorno;     //RETORNA COMISSAO VENDEDOR
                }


            }

            // Verifica quem é a prioridade 2
            if ((pr.Prioridade_Tab == 2) && (Comissao_Tabela != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Tabela / 100);
                _ComissaoUsada = Comissao_Tabela;
                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Repr == 2) && (Comissao_Vendedor != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Vendedor / 100);
                _ComissaoUsada = Comissao_Vendedor;

                //prioridade comissao do produto para o televendedor 
                if (Comissao_Mercadoria_Televendas != 0)
                {
                    Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria_Televendas / 100);
                    _ComissaoUsadaTele = Comissao_Mercadoria_Televendas;
                }
                else
                {
                    Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Televendedor / 100);
                    _ComissaoUsadaTele = Comissao_Televendedor;
                }


                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Merc == 2) && (Comissao_Mercadoria != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria / 100);
                _ComissaoUsada = Comissao_Mercadoria;

                Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria_Televendas / 100);
                _ComissaoUsadaTele = Comissao_Mercadoria_Televendas;

                VR_Nenhuma = false;
            }

            if ((pr.PARA_PriorGrupo == 2) && (Comissao_Grupo != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Grupo / 100);
                _ComissaoUsada = Comissao_Grupo;
                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Cliente == 2) && (ComissaoCliente != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * ComissaoCliente / 100);
                _ComissaoUsada = ComissaoCliente;
                VR_Nenhuma = false;
            }

            if (!VR_Nenhuma)
            {
                if (AvisoComissao == "T")
                {
                    return Retorno_tele; //RETORNA COMISSAO TELEVENDEDOR
                }
                else
                {
                    return Retorno;     //RETORNA COMISSAO VENDEDOR
                }
            }

            // Verifica quem é a prioridade 3
            if ((pr.Prioridade_Tab == 3) && (Comissao_Tabela != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Tabela / 100);
                _ComissaoUsada = Comissao_Tabela;
                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Repr == 3) && (Comissao_Vendedor != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Vendedor / 100);
                _ComissaoUsada = Comissao_Vendedor;

                //prioridade comissao do produto para o televendedor 
                if (Comissao_Mercadoria_Televendas != 0)
                {
                    Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria_Televendas / 100);
                    _ComissaoUsadaTele = Comissao_Mercadoria_Televendas;
                }
                else
                {
                    Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Televendedor / 100);
                    _ComissaoUsadaTele = Comissao_Televendedor;
                }


                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Merc == 3) && (Comissao_Mercadoria != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria / 100);
                _ComissaoUsada = Comissao_Mercadoria;

                Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria_Televendas / 100);
                _ComissaoUsadaTele = Comissao_Mercadoria_Televendas;
                
                VR_Nenhuma = false;
            }

            if ((pr.PARA_PriorGrupo == 3) && (Comissao_Grupo != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Grupo / 100);
                _ComissaoUsada = Comissao_Grupo;
                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Cliente == 3) && (ComissaoCliente != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * ComissaoCliente / 100);
                _ComissaoUsada = ComissaoCliente;
                VR_Nenhuma = false;
            }

            if (!VR_Nenhuma)
            {
                if (AvisoComissao == "T")
                {
                    return Retorno_tele; //RETORNA COMISSAO TELEVENDEDOR
                }
                else
                {
                    return Retorno;     //RETORNA COMISSAO VENDEDOR
                }
            }

            // Verifica quem é a prioridade 4
            if ((pr.Prioridade_Tab == 4) && (Comissao_Tabela != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Tabela / 100);
                _ComissaoUsada = Comissao_Tabela;
                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Repr == 4) && (Comissao_Vendedor != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Vendedor / 100);
                _ComissaoUsada = Comissao_Vendedor;

                //prioridade comissao do produto para o televendedor 
                if (Comissao_Mercadoria_Televendas != 0)
                {
                    Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria_Televendas / 100);
                    _ComissaoUsadaTele = Comissao_Mercadoria_Televendas;
                }
                else
                {
                    Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Televendedor / 100);
                    _ComissaoUsadaTele = Comissao_Televendedor;
                }


                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Merc == 4) && (Comissao_Mercadoria != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria / 100);
                _ComissaoUsada = Comissao_Mercadoria;

                Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria_Televendas / 100);
                _ComissaoUsadaTele = Comissao_Mercadoria_Televendas;

                VR_Nenhuma = false;
            }

            if ((pr.PARA_PriorGrupo == 4) && (Comissao_Grupo != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Grupo / 100);
                _ComissaoUsada = Comissao_Grupo;
                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Cliente == 4) && (ComissaoCliente != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * ComissaoCliente / 100);
                _ComissaoUsada = ComissaoCliente;
                VR_Nenhuma = false;
            }

            if (!VR_Nenhuma)
            {
                if (AvisoComissao == "T")
                {
                    return Retorno_tele; //RETORNA COMISSAO TELEVENDEDOR
                }
                else
                {
                    return Retorno;     //RETORNA COMISSAO VENDEDOR
                }
            }

            // Verifica quem é a prioridade 5
            if ((pr.Prioridade_Tab == 5) && (Comissao_Tabela != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Tabela / 100);
                _ComissaoUsada = Comissao_Tabela;
                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Repr == 5) && (Comissao_Vendedor != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Vendedor / 100);
                _ComissaoUsada = Comissao_Vendedor;

                //prioridade comissao do produto para o televendedor 
                if (Comissao_Mercadoria_Televendas != 0)
                {
                    Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria_Televendas / 100);
                    _ComissaoUsadaTele = Comissao_Mercadoria_Televendas;
                }
                else
                {
                    Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Televendedor / 100);
                    _ComissaoUsadaTele = Comissao_Televendedor;
                }


                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Merc == 5) && (Comissao_Mercadoria != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria / 100);
                _ComissaoUsada = Comissao_Mercadoria;

                Retorno_tele = ((Unitario * (Qtd - QtdCan)) * Comissao_Mercadoria_Televendas / 100);
                _ComissaoUsadaTele = Comissao_Mercadoria_Televendas;

                VR_Nenhuma = false;
            }

            if ((pr.PARA_PriorGrupo == 5) && (Comissao_Grupo != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * Comissao_Grupo / 100);
                _ComissaoUsada = Comissao_Grupo;
                VR_Nenhuma = false;
            }

            if ((pr.Prioridade_Cliente == 5) && (ComissaoCliente != 0))
            {
                Retorno = ((Unitario * (Qtd - QtdCan)) * ComissaoCliente / 100);
                _ComissaoUsada = ComissaoCliente;
                VR_Nenhuma = false;
            }

            if (!VR_Nenhuma)
            {
                if (AvisoComissao == "T")
                {
                    return Retorno_tele; //RETORNA COMISSAO TELEVENDEDOR
                }
                else
                {
                    return Retorno;     //RETORNA COMISSAO VENDEDOR
                }
            }

            if (AvisoComissao == "T")
            {
                return Retorno_tele; //RETORNA COMISSAO TELEVENDEDOR
            }
            else
            {
                return Retorno;     //RETORNA COMISSAO VENDEDOR
            }
        }
    }
}