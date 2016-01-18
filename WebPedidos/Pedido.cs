using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebPedidos.WSClasses;
using System.IO;
using System.Text;

public partial class Pedido : System.Web.UI.Page
{
    UsuarioResumido u;
    ParametroResumido pr;
    ClasseBanco conn = new ClasseBanco();
    ClasseCliente csCliente = new ClasseCliente();
    Funcoes csFuncoes = new Funcoes();
    StringBuilder strSql = new StringBuilder();

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;

        u = (UsuarioResumido)Session["Usuario"];
        pr = (ParametroResumido)Session["Parametros"];

        if (!IsPostBack)
        {
            if (u == null)
            {
                Response.Redirect("Default.aspx");
            }

            conn.AbrirBanco();

            cbUnidadeVenda.Items.Insert(0, "Menor Unidade");
            cbUnidadeVenda.Items.Insert(1, "Maior Unidade");
            cbUnidadeVenda.SelectedIndex = (pr.para_unidadevenda == 2 ? 1 : 0);

            if (cbUnidadeVenda.SelectedValue == "Maior Unidade")
            {
                lbTipoPreco.Text = "Preço Maior Unidade";
            }
            else
            {
                lbTipoPreco.Text = "Preço Menor Unidade";
            }

            var rsMov = conn.retornaQueryDataSet("SELECT * FROM TMP_MOVIMENTACOES_WEB WHERE CODEMP = " + u.CodEmp + "");

            csMovimentacao.DataSource = rsMov;
            csMovimentacao.DataValueField = "Codigo";
            csMovimentacao.DataTextField = "Descricao";
            csMovimentacao.DataBind();
            csMovimentacao.Items.Insert(0, new ListItem("(Selecione uma Movimentação)", "-1"));
            csMovimentacao.SelectedValue = csMovimentacao.Items.FindByValue(pr.CodTipMov_Estadual.ToString()).Value; //pr.CodTipMov_Estadual

            #region Carrega Condicoes de Pagamento
            cbCliente.DataSource = ClasseCliente.ListarClientes(u, pr);
            cbCliente.DataValueField = "Codigo";
            cbCliente.DataTextField = "Nome";
            cbCliente.DataBind();
            cbCliente.Items.Insert(0, new ListItem("", "-1"));

            CarregaCondicoesPagamento(0, Convert.ToInt16(csMovimentacao.SelectedValue));

            #endregion

            //Busca Tabela de Preco Padrao ou do Vendedor
            BuscaTabelaPadrao();

            if (Session["Usuario"] != null)
            {
                LabelVendedor.Text = Session["NomVend"].ToString();
            }
            else
            {
                LabelVendedor.Text = String.Empty;
            }

            ClassePedido.RemoverReservaItensPedidoWeb(u);
            LabelNumPedido.Text = ClassePedido.ProximoPedidoWeb(u).ToString();
            LabelNumPedidoSigma.Text = ClassePedido.ProximoPedido(u).ToString();

        }

        tbCliente.Focus();

    }

    private void CarregaCondicoesPagamento(int lCodigoCliente, int iCodTipMov)
    {
        bool bBuscaCondicoesCliente = false;
        bool bValidaMovimentacao = false;
        DropDownListCondPagto.Items.Clear();

        //53190
        if (iCodTipMov != 0)
        {
            strSql.Length = 0;
            strSql.Append(" SELECT * FROM CONDMOV ");
            strSql.Append(" WHERE CODTIPMOV = " + iCodTipMov + "");
            var rsLeitura = conn.Query(strSql.ToString());

            if (rsLeitura.Read())
            {
                bValidaMovimentacao = true;
            }
            rsLeitura.Dispose();
        }
        //53190

        if (lCodigoCliente != 0)
        {
            var rsTemp = conn.Query("SELECT 1 FROM CONDCLI WHERE CODCLI = " + lCodigoCliente + "");
            if (rsTemp.Read())
            {
                bBuscaCondicoesCliente = true;
            }
            rsTemp.Close();
        }

        strSql.Length = 0;
        strSql.Append("SELECT DISTINCT FORMAPAGTO.CodFrmPgt, FORMAPAGTO.DesFrmPgt, TIPOPRAZO.CodTipPrz, TIPOPRAZO.DesTipPrz, FORMAPAGTO.GeraParcelas ");
        strSql.Append("    FROM ITCONPAGTO, FORMAPAGTO, TIPOPRAZO ");

        if (bBuscaCondicoesCliente)
        {
            strSql.Append(", CONDCLI ");
        }

        //53190
        if (bValidaMovimentacao)
        {
            strSql.Append(", CONDMOV ");
        }


        strSql.Append("    WHERE ");
        strSql.Append("    ITCONPAGTO.CodFrmPgt = FORMAPAGTO.CodFrmPgt  AND ");
        strSql.Append("    ITCONPAGTO.CodTipPrz = TIPOPRAZO.CodTipPrz  AND ");

        if (bBuscaCondicoesCliente)
        {
            strSql.Append("    CONDCLI.CodFrmPgt = FORMAPAGTO.CodFrmPgt AND ");
            strSql.Append("    CONDCLI.CodTipPrz = TIPOPRAZO.CodTipPrz AND ");
            strSql.Append("    CONDCLI.CodCli = " + lCodigoCliente + " AND ");
        }

        //53190
        if (bValidaMovimentacao)
        {
            strSql.Append("    CONDMOV.CodFrmPgt = FORMAPAGTO.CodFrmPgt AND ");
            strSql.Append("    CONDMOV.CodTipPrz = TIPOPRAZO.CodTipPrz AND ");
            strSql.Append("    CONDMOV.CODTIPMOV = " + iCodTipMov + " AND ");
        }

        strSql.Append("    CODEMP               = " + u.CodEmp + " AND ");
        strSql.Append("    FILTROVENDA          = 1 ");
        strSql.Append("    AND ITCONPAGTO.PALM  = 1 ");
        //strSql.Append("    ORDER BY FORMAPAGTO.CodFrmPgt, TIPOPRAZO.CodTipPrz"); //
        strSql.Append("    ORDER BY FORMAPAGTO.CodFrmPgt, TIPOPRAZO.CodTipPrz"); //52688

        var dados = conn.Query(strSql.ToString());

        String vrGeraParcelas = "";
        String vrFormaPagto = "";
        var sCamposCondicao = "";
        var sDescricaoCombo = "";

        while (dados.Read())
        {
            if (vrGeraParcelas == "S" && dados[0].ToString() == vrFormaPagto)
            {
                vrGeraParcelas = dados[4].ToString();
            }
            else
            {
                sCamposCondicao = (dados[0].ToString() + "|" + dados[2].ToString().Trim()); ;
                sDescricaoCombo = (dados[0] + " - " + dados[1] + " ==> " + dados[2] + " - " + dados[3]);

                DropDownListCondPagto.Items.Insert(0, new ListItem(sDescricaoCombo, sCamposCondicao));

                vrFormaPagto = dados[0].ToString();
                vrGeraParcelas = dados[4].ToString();
            }
        }
        dados.Close();

        DropDownListCondPagto.Items.Insert(0, new ListItem("(Selecione uma condição de pagamento)", "-1"));
    }

    private void BuscaTabelaPadrao()
    {
        /*Se nao tiver tabela de preco padronizada*/
        ClasseVendedorAtributos[] vend = ClasseVendedor.BuscaDados(Convert.ToInt16(Session["CodVend"].ToString()), null, null).ToArray();
        cbTabelas.Items.Clear();
        cbPrazoTabela.Items.Clear();

        bool bAdicionar = true;

        foreach (ClasseVendedorAtributos vn in vend)
        {
            cbTabelas.Items.Insert(0, new ListItem(vn.IdTabela.ToString() + " - " + vn.DesTipPrc + " ==> " + vn.CodTipPrz.ToString() + " - " + vn.DesTipPrz, vn.IdTabela.ToString() + "|" + vn.CodTipPrz));

            for (int i = 0; i < cbPrazoTabela.Items.Count; i++) // Loop through List with for
            {
                cbPrazoTabela.SelectedIndex = i;

                if (vn.CodTipPrz == Convert.ToInt16(cbPrazoTabela.SelectedValue))
                {
                    bAdicionar = false;
                }
            }

            if (bAdicionar == true)
            {
                cbPrazoTabela.Items.Insert(0, new ListItem(vn.CodTipPrz.ToString() + " - " + vn.DesTipPrz, vn.CodTipPrz.ToString()));
            }

            bAdicionar = true;
        }
        cbPrazoTabela.SelectedIndex = -1;

        if (!pr.CodTipPrc.Equals(0))
        {
            if (cbTabelas.Items.Count <= 0)
            {
                string sTabela = Funcoes.BuscaCampoTabela("DESTIPPRC", "TIPO_PRECO", " AND CODTIPPRC = " + pr.CodTipPrc + "");
                string sPrazo = Funcoes.BuscaCampoTabela("DESTIPPRZ", "TIPOPRAZO", " AND CODTIPPRZ = " + pr.CodTipPrz + "");

                cbTabelas.Items.Clear();
                cbPrazoTabela.Items.Clear();

                cbTabelas.Items.Insert(0, new ListItem(sTabela.Trim() + " ==> " + sPrazo.Trim(), pr.CodTipPrc + "|" + pr.CodTipPrz));
                cbPrazoTabela.Items.Insert(0, new ListItem(sPrazo.Trim(), pr.CodTipPrz.ToString()));
            }
        }

        cbTabelas.Items.Insert(0, new ListItem("(Selecione uma Tabela Válida)", "-1"));
        cbPrazoTabela.Items.Insert(0, new ListItem("", "-1"));

    }

    protected void LinkButtonAdicionar_Click(object sender, EventArgs e)
    {

        Int16 CodPrcTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').First());
        Int16 CodPrzTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').Last());

        //51516
        if (pr.CondicaoTabLivreWeb == 1 && cbPrazoTabela.SelectedIndex != 0)
        {
            CodPrzTab = Convert.ToInt16(cbPrazoTabela.SelectedValue);
        }

        if (TextBoxQuantidade.Text == "" || TextBoxQuantidade.Text == "0")
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Informe a Quantidade.');", true);
            return;
        }

        if (TB_Preco.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Informe a Preco.');", true);
            return;
        }

        List<ProdutoResumido> products;
        if (ViewState["Produtos"] == null)
        {
            products = new List<ProdutoResumido>();
            ViewState["Produtos"] = products;
        }
        else
        {
            products = (List<ProdutoResumido>)ViewState["Produtos"];
        }

        //ProdutoResumido pr1 = ClasseProdutos.Produto((UsuarioResumido)Session["Usuario"], Convert.ToInt32(cbProdutos.SelectedValue), pr, CodPrcTab, CodPrzTab).First();
        ProdutoResumido pr1 = ClasseProdutos.buscaProdutosPorCodigo((UsuarioResumido)Session["Usuario"], pr, Convert.ToInt32(cbProdutos.SelectedValue), CodPrcTab, CodPrzTab).First();

        pr1.Desconto = TB_Desconto.Text == "" ? 0 : Convert.ToDecimal(TB_Desconto.Text);
        pr1.TotalParcial = 0;
        pr1.UnidadeVenda = cbUnidadeVenda.SelectedValue == "Menor Unidade" ? "0" : "2";

        if (products.Where(p => p.Codigo == pr1.Codigo).Count() > 0)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Produto ja adicionado. Para adicionar novamente remova o item existente.');", true);
            return;
        }
        else
        {
            //FABIANO - 07/10/2011
            if (cbUnidadeVenda.SelectedIndex.Equals(1))
            {
                pr1.Preco = Convert.ToDecimal(TB_Preco.Text) / pr1.QtdCaixa; //Convert.ToDecimal(tbPrecoLiq.Text);                                
                pr1.Quantidade = Convert.ToDecimal(TextBoxQuantidade.Text) * Convert.ToDecimal(pr1.QtdCaixa);
            }
            else
            {
                pr1.Preco = Convert.ToDecimal(TB_Preco.Text);
                pr1.Quantidade = Convert.ToDecimal(TextBoxQuantidade.Text);
            }

            //var desconto = String.Format("{0:" + Funcoes.Decimais(pr) + "}", (pr1.Preco * (pr1.Desconto / 100)));
            var desconto = (pr1.Preco * (pr1.Desconto / 100));

            pr1.TotalParcial = ((pr1.Preco - Convert.ToDecimal(desconto)) * pr1.Quantidade);
            pr1.preco_liquido = pr1.TotalParcial / pr1.Quantidade;
            pr1.Peso = Convert.ToDecimal(lbPeso.Text) * pr1.Quantidade;

            if (cbUnidadeVenda.SelectedIndex.Equals(1))
            {
                pr1.Preco = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", pr1.Preco)) * pr1.Quantidade;
            }
            pr1.Itp_CodTabPrz = CodPrzTab; //51516
        }

        try
        {
            Boolean retorno = ClassePedido.ReservarItemPedidoWeb(u, Convert.ToInt32(LabelNumPedido.Text), Convert.ToInt32(cbProdutos.SelectedValue), Convert.ToDecimal(pr1.Quantidade), pr);
            if (retorno)
            {
                products.Add(pr1);
            }
        }
        catch (Exception ex)
        {
            if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + ex.Message + "');", true);
                //lbTextoAtendidos.Visible = true;
                if (lbNaoAtendidos.Text != "")
                {
                    lbNaoAtendidos.Text = lbNaoAtendidos.Text + ", " + cbProdutos.SelectedValue;
                }
                else
                {
                    lbNaoAtendidos.Text = cbProdutos.SelectedValue;
                }
            }
        }

        cbProdutos.SelectedIndex = 0;
        lbMensagemPromocional.Text = String.Empty;
        TextBoxQuantidade.Text = String.Empty;
        LabelPreco.Text = String.Empty;
        TB_Preco.Text = String.Empty;
        LabelSaldo.Text = String.Empty;
        TB_Desconto.Text = String.Empty;
        tbPrecoLiq.Text = String.Empty;
        tbSubTotal.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", products.Sum(p => p.TotalParcial));
        tbTotalPedido.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", products.Sum(p => p.TotalParcial));
        tbPesoTotal.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", products.Sum(p => p.Peso));
        lbUnidade.Text = String.Empty;
        GridView1.DataSource = products;
        lbPeso.Text = String.Empty;
        BoundField bf;

        bf = (BoundField)GridView1.Columns[2]; // Preço base
        bf.DataFormatString = "R$ {0:" + Funcoes.Decimais(pr) + "}";

        bf = (BoundField)GridView1.Columns[5];
        bf.DataFormatString = "{0:" + Funcoes.Decimais(pr) + "}";

        bf = (BoundField)GridView1.Columns[7]; // Total parcial
        bf.DataFormatString = "R$ {0:" + Funcoes.Decimais(pr) + "}";

        GridView1.DataBind();
        txtPesqProduto.Focus();
        idItens.Text = GridView1.Rows.Count.ToString();
        TB_DescPed_TextChanged(sender, e);

        //51114        
        //NÃO PERMITIR ALTERAÇÃO DA TABELA , APÓS O INÍCIO DA DIGITAÇÃO DAS MERCADORIAS;
        //51516
        if (pr.CondicaoTabLivreWeb != 1)
        {
            cbTabelas.Enabled = false;
        }

        DropDownListCondPagto.Enabled = false;

    }

    protected void buPesquisar_Click(object sender, EventArgs e)
    {
        cbCliente.DataSource = csCliente.buscaCliente(tbCliente.Text, u, pr);
        cbCliente.DataValueField = "Codigo";
        cbCliente.DataTextField = "Nome";
        cbCliente.DataBind();
        cbCliente.Items.Insert(0, new ListItem("(Selecione um Cliente)", "-1"));
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Button cea = (Button)sender;
        List<ProdutoResumido> products = (List<ProdutoResumido>)ViewState["Produtos"];
        ProdutoResumido pr1 = products.Single(p => p.Codigo == Convert.ToInt32(cea.CommandArgument.ToString()));
        if (ClassePedido.RemoverReservaItemPedidoWeb(u, Convert.ToInt32(LabelNumPedido.Text), pr1.Codigo))
        {
            products.Remove(pr1);
        }

        tbSubTotal.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", products.Sum(p => p.TotalParcial));
        tbTotalPedido.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", products.Sum(p => p.TotalParcial));
        tbPesoTotal.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", products.Sum(p => p.Peso));
        idItens.Text = String.Empty;
        GridView1.DataSource = products;
        GridView1.DataBind();
        idItens.Text = GridView1.Rows.Count.ToString();

        if (GridView1.Rows.Count <= 0)
        {
            cbTabelas.Enabled = true;
            DropDownListCondPagto.Enabled = true;
        }

    }

    protected void ScriptManager1_Init(object sender, EventArgs e)
    {

    }

    protected void cbProdutos_SelectedIndexChanged(object sender, EventArgs e)
    {
        Int16 CodPrcTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').First());
        Int16 CodPrzTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').Last());

        //51516
        if (pr.CondicaoTabLivreWeb == 1 && cbPrazoTabela.SelectedIndex != 0)
        {
            CodPrzTab = Convert.ToInt16(cbPrazoTabela.SelectedValue);
        }

        if (cbProdutos.SelectedValue != "-1")
        {
            //List<ProdutoResumido> prs = ClasseProdutos.Produto((UsuarioResumido)Session["Usuario"], Convert.ToInt32(cbProdutos.SelectedValue), pr, CodPrcTab, CodPrzTab);
            List<ProdutoResumido> prs = ClasseProdutos.buscaProdutosPorCodigo((UsuarioResumido)Session["Usuario"], pr, Convert.ToInt32(cbProdutos.SelectedValue), CodPrcTab, CodPrzTab);

            if (prs.Count > 0)
            {
                ProdutoResumido pr1 = prs.First();

                if (pr1.Preco == 0)
                {
                    LabelPreco.Text = String.Empty;
                    TB_Preco.Text = String.Empty;
                    LabelSaldo.Text = String.Empty;
                    tbPrecoLiq.Text = String.Empty;
                    lbPeso.Text = String.Empty;
                    RangeValidator1.MinimumValue = "0";
                    RangeValidator1.MaximumValue = "0";
                }
                else
                {
                    lbMensagemPromocional.Text = pr1.MensagemPromocao;
                    LabelPreco.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", pr1.Preco);
                    TB_Preco.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", pr1.Preco);
                    lbUnidade.Text = pr1.Unidade;
                    tbPrecoLiq.Text = TB_Preco.Text;
                    lbPeso.Text = pr1.Peso.ToString();

                    /*Venda Maior unidade*/
                    if (cbUnidadeVenda.SelectedIndex.Equals(1))
                    {
                        lbUnidade.Text = pr1.M_UNIDADE;
                        //TB_Preco.Text   = String.Format("{0:" + Funcoes.Decimais(pr) + "}", (pr1.Preco * Convert.ToDecimal(pr1.QtdCaixa)));
                        TB_Preco.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", (Convert.ToDecimal(TB_Preco.Text) * Convert.ToDecimal(pr1.QtdCaixa)));
                    }

                    LabelSaldo.Text = String.Format("{0}", pr1.Saldo);
                    if (pr.SaldoPed == 'S')
                    {
                        RangeValidator1.MinimumValue = pr1.Saldo <= 0 ? "0" : "1";
                        RangeValidator1.MaximumValue = pr1.Saldo <= 0 ? "0" : pr1.Saldo.ToString();
                    }
                    else
                    {
                        RangeValidator1.MinimumValue = "1";
                        RangeValidator1.MaximumValue = "99999";
                    }
                }

            }
            else
            {
                LabelPreco.Text = String.Empty;
                TB_Preco.Text = String.Empty;
                LabelSaldo.Text = String.Empty;
                tbPrecoLiq.Text = String.Empty;
                lbPeso.Text = String.Empty;
                lbMensagemPromocional.Text = String.Empty;
                RangeValidator1.MinimumValue = "0";
                RangeValidator1.MaximumValue = "0";
            }
            TextBoxQuantidade.Focus();
        }
    }

    protected void ButtonGravar_Click(object sender, EventArgs e)
    {
        int codVendedor = Convert.ToInt32(Session["CodVend"]);
        int codCliente = Convert.ToInt32(cbCliente.SelectedValue);
        string observacao = TextBoxObservacao.Text.Trim();

        TB_DescPed_TextChanged(sender, e); //validacao desconto

        if (DropDownListCondPagto.SelectedValue == "-1")
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Selecione a condição de pagamento.');", true);
            return;
        }

        int codFormaPagto = Convert.ToInt32(DropDownListCondPagto.SelectedValue.Split('|').First());
        int codCondPagto = Convert.ToInt32(DropDownListCondPagto.SelectedValue.Split('|').Last());
        int CodPrcTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').First());
        int CodPrzTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').Last());

        List<ProdutoResumido> produtos = (List<ProdutoResumido>)ViewState["Produtos"];
        if (produtos != null)
        {
            if (produtos.Count < 1 && LabelPreco.Text == "")
            {
                if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
                {
                    if (LabelPreco.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Tabela de Preco nao cadastrada ou a mercadoria esta inativa');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Não há produtos adicionados a este pedido. Insira um ou mais produtos.');", true);
                    }
                }
            }
            else
            {
                foreach (ProdutoResumido prod in produtos)
                {
                    prod.Preco = prod.Preco;
                    prod.Desconto = prod.Desconto;
                    prod.TotalParcial = prod.TotalParcial;
                    prod.Quantidade = prod.Quantidade;
                    prod.M_UNIDADE = prod.M_UNIDADE;
                    prod.QtdCaixa = prod.QtdCaixa;
                    prod.Comissao = prod.Comissao;
                }
                int resultado = 0;

                try
                {

                    decimal vlr_desconto = TB_DescPed.Text == "" ? 0 : Convert.ToDecimal(TB_DescPed.Text);
                    resultado = ClassePedido.InserePedido((UsuarioResumido)Session["Usuario"],
                                                          (ParametroResumido)Session["Parametros"],
                                                          codVendedor,
                                                          codCliente,
                                                          observacao,
                                                          codFormaPagto,
                                                          codCondPagto,
                                                          produtos,
                                                          (int)Session["EmpresaCODEMP"],
                                                          Convert.ToInt32(LabelNumPedido.Text),
                                                          Convert.ToInt32(LabelNumPedidoSigma.Text),
                                                          vlr_desconto,
                                                          CodPrcTab,
                                                          CodPrzTab,
                                                          cbUnidadeVenda.SelectedIndex, Convert.ToInt16(csMovimentacao.SelectedValue));


                    ClassePedido.RemoverReservaItensPedidoWeb(u);
                    LabelNumPedido.Text = ClassePedido.ProximoPedidoWeb(u).ToString();
                    LabelNumPedidoSigma.Text = ClassePedido.ProximoPedido(u).ToString();

                    string cValorPedido = tbTotalPedido.Text.Replace(',', '.');


                    /* Verifica se houve bloqueio do pedido por regras do financeiro/comercial
                      
                        '-----------------------------------------------------------------------+
                        'BLOQUEIOS :  BL  - BLOQUEADO POR LIMITE                                +
                        '             BI  - BLOQUEADO POR INADIMPLENCIA                         +
                        '             BD  - BLOQUEADO POR DÉBITOS EM ABERTO                     +
                        '             BVM - BLOQUEADO POR VALOR MÍNIMO NÃO ATINGIDO             +
                        '             BMM - BLOQUEADO POR MARGEM MÍNIMA DO PEDIDO NÃO ATINGIDO  +
                        '             BBF - BLOQUEADO POR CLIENTE BLOQUEADO                     +
                        '-----------------------------------------------------------------------+
                      
                      SP_BloqueioPedidos
                      Cliente				
                      Pedido				
                      FormaPagto			
                      PrazoPagto			
                      Empresa				
                      ValorPedido			
                      DataInadimplente	- Esses 4 ultimos campos a exemplo do DIGPEDIDOS sao passados em branco
                      BloquearDebitos		
                      MargemMinimaPedido	
                      MargemLucro			                    
                   */

                    ClasseBloqueioFinancerio cls_BloqueioCliente = new ClasseBloqueioFinancerio();

                    string resposta = cls_BloqueioCliente.BloqueioFinancerio(codCliente,
                        Convert.ToInt32(resultado),
                        Convert.ToInt16(CodPrcTab),
                        Convert.ToInt16(CodPrzTab),
                        Convert.ToInt16(pr.CodEmp), cValorPedido, "", 0, 0, 0);

                    if (resposta != "OK")
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + resposta + "');", true);
                    }

                    if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('O pedido " + resultado + " foi cadastrado com sucesso.');", true);
                    }

                    //48426
                    //Verifica se envia email ao cliente
                    if (Funcoes.BuscaCampoTabela("EnviarEmailWeb", "PARAMETROS", " AND CODEMP = " + pr.CodEmp) == "1")
                    {
                        //busca email do cliente
                        string sEmailCliente = Funcoes.BuscaCampoTabela("Email", "CLIENTE", " AND CODCLI = " + codCliente + "");
                        string sRemetente = Funcoes.BuscaCampoTabela("UsuarioSmtp", "PARAMETROS", " AND CODEMP = " + pr.CodEmp + "");
                        string sAssunto = "Pedido de compra";
                        StringBuilder sTexto = new StringBuilder();

                        sTexto.Append("N. Pedido : " + resultado);
                        sTexto.Append("<br/>Data : " + DateTime.Today);
                        sTexto.Append("<br/><br/>");
                        sTexto.Append("<Table border=1>");
                        sTexto.Append("<tr><td>PRODUTO</td><td>QUANTIDADE</td><td>PREÇO</td><td>TOTAL</td>");
                        sTexto.Append("<tr><td></td><td></td><td></td><td></td>");

                        DataClassesDataContext dcdc = new DataClassesDataContext();
                        decimal cTotalGeral = 0;

                        foreach (ProdutoResumido produto in produtos)
                        {
                            SERVMERC servMercadoria = dcdc.SERVMERCs.SingleOrDefault(sm => sm.CodServMerc == produto.Codigo);
                            decimal cTotalItens = (produto.Quantidade * produto.preco_liquido);
                            cTotalGeral = cTotalGeral + cTotalItens;

                            sTexto.Append("<tr><td>" + produto.Codigo + " - " + servMercadoria.DesServMerc.ToString() + "</td><td>" + produto.Quantidade + "</td><td>" + produto.preco_liquido + "</td><td>" + cTotalItens + "</td>");
                        }

                        sTexto.Append("<tr><td></td><td></td><td></td><td></td>");
                        sTexto.Append("<tr><td></td><td></td><td></td><td>" + cTotalGeral + "</td>");
                        sTexto.Append("</table>");

                        if (sEmailCliente != "")
                        {
                            try
                            {
                                csFuncoes.EnviaEmail(pr, sEmailCliente.Trim(), sRemetente.Trim(), "", "", sAssunto, sTexto.ToString(), "", "");
                            }
                            catch (Exception excEmail)
                            {
                                //ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('" + excEmail.Message + "')</script>");
                                Response.Write("<p class='texto_erro'>" + excEmail.Message + "</p>");
                            }
                        }
                    }

                    ButtonLimpar_Click(null, null);

                }
                catch (Exception ex)
                {
                    if (!String.IsNullOrEmpty(ex.Message))
                    {
                        Response.Write("<p class='texto_erro'>" + ex.Message + "</p>");
                    }
                }

                if (resultado == -1)
                {
                    if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
                    {
                        if (ViewState["produtos"] != null)
                        {
                            if (((List<ProdutoResumido>)ViewState["produtos"]).Count == 0)
                            {
                                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Não foram encontrados produtos selecionados para este pedido. Favor informar um ou mais produtos para finalizar o pedido');", true);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Houve um problema na gravação do pedido. Verifique se os campos foram preenchidos corretamente');", true);
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Não foram encontrados produtos selecionados para este pedido. Favor informar um ou mais produtos para finalizar o pedido');", true);
                        }
                    }
                }
            }
        }
        else
        {
            if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Não há produtos adicionados a este pedido. Insira um ou mais produtos');", true);
            }
        }
    }

    protected void ButtonLimpar_Click(object sender, EventArgs e)
    {
        //Resolver problema time-out
        //EXEC sp_serveroption 'NOME SERVIDOR', 'query timeout', '1000';
        ClassePedido.RemoverReservaItensPedidoWeb(u);
        lbMensagem.Text = "";
        ViewState["Produtos"] = null;
        tbPesoTotal.Text = String.Empty;
        lbPeso.Text = String.Empty;
        Session["DESCONTO_CONDICAO"] = "";
        //lbTextoAtendidos.Visible = false;
        lbNaoAtendidos.Text = "";
        lbMensagemPromocional.Text = String.Empty;
        tbPrecoLiq.Text = String.Empty;
        TextBoxObservacao.Text = String.Empty;
        TextBoxQuantidade.Text = String.Empty;
        LabelPreco.Text = String.Empty;
        TB_Preco.Text = String.Empty;
        LabelSaldo.Text = String.Empty;
        tbTotalPedido.Text = String.Empty;
        tbSubTotal.Text = String.Empty;
        TB_DescPed.Text = String.Empty;
        TB_VlrDes.Text = String.Empty;
        idItens.Text = String.Empty;
        lbDados.Text = String.Empty;
        tbCliente.Text = String.Empty;
        cbCliente.SelectedIndex = 0;
        cbTabelas.Enabled = true; //49460 - Fabiano
        DropDownListCondPagto.Enabled = true;
        DropDownListCondPagto.SelectedIndex = 0;
        cbUnidadeVenda.SelectedIndex = 0;
        csMovimentacao.SelectedValue = csMovimentacao.Items.FindByValue(pr.CodTipMov_Estadual.ToString()).Value;
        BuscaTabelaPadrao();
        cbProdutos.Items.Clear();
        GridView1.DataBind();
        LinkButton2_Click(null, null);

    }

    protected void butPesqProdutos_Click(object sender, EventArgs e)
    {
        Int32 iCodProd = 0;
        Int32.TryParse(txtPesqProduto.Text, out iCodProd);
        Int16 CodPrcTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').First());
        Int16 CodPrzTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').Last());

        //51516
        if (pr.CondicaoTabLivreWeb == 1 && cbPrazoTabela.SelectedIndex != 0)
        {
            CodPrzTab = Convert.ToInt16(cbPrazoTabela.SelectedValue);
        }

        if (iCodProd == 0)
        {
            try
            {
                cbProdutos.DataSource = ClasseProdutos.buscaProdutosPorNome(u, pr, txtPesqProduto.Text, CodPrcTab, CodPrzTab);
            }
            catch (Exception erro)
            {
                cbProdutos.Items.Clear();
                //ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + erro.Message.ToString() + "');", true);
                Response.Write("<p class='texto_erro'>" + erro.Message + "</p>");
            }
        }
        else
        {
            try
            {
                cbProdutos.DataSource = ClasseProdutos.buscaProdutosPorCodigo(u, pr, iCodProd, CodPrcTab, CodPrzTab);
            }
            catch (Exception erro)
            {
                cbProdutos.Items.Clear();
                //ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + erro.Message.ToString() + "');", true);
                Response.Write("<p class='texto_erro'>" + erro.Message + "</p>");
            }
        }

        txtPesqProduto.Text = String.Empty;
        LabelPreco.Text = String.Empty;
        TB_Preco.Text = String.Empty;
        LabelSaldo.Text = String.Empty;

        RangeValidator1.MinimumValue = "0";
        RangeValidator1.MaximumValue = "0";

        cbProdutos.DataValueField = "Codigo";
        cbProdutos.DataTextField = "Nome";
        cbProdutos.DataBind();
        cbProdutos.Items.Insert(0, new ListItem("", "-1"));
        cbProdutos.Focus();

    }

    protected void TB_Preco_TextChanged(object sender, EventArgs e)
    {
        Int16 CodPrcTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').First());
        Int16 CodPrzTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').Last());

        //51516
        if (pr.CondicaoTabLivreWeb == 1 && cbPrazoTabela.SelectedIndex != 0)
        {
            CodPrzTab = Convert.ToInt16(cbPrazoTabela.SelectedValue);
        }

        TB_Desconto.Text = "0"; // Convert.ToString(0);
        if (LabelPreco.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Tabela de Preco nao cadastrada ou a mercadoria está inativa.');", true);
            TB_Preco.Text = String.Empty;
            tbPrecoLiq.Text = "";
        }
        else
        {
            //FABIANO - 07/10/2011
            if (TB_Preco.Text != "" && LabelPreco.Text != "")
            {
                var calculo_porcentagem = 100 - (Convert.ToDecimal(TB_Preco.Text) * 100) / Convert.ToDecimal(LabelPreco.Text);
                TB_Desconto.Text = String.Format("{0:0.00}", calculo_porcentagem);
            }

            //List<ProdutoResumido> prs = ClasseProdutos.Produto(u, Convert.ToInt32(cbProdutos.SelectedValue), pr, CodPrcTab, CodPrzTab);
            List<ProdutoResumido> prs = ClasseProdutos.buscaProdutosPorCodigo(u, pr, Convert.ToInt32(cbProdutos.SelectedValue), CodPrcTab, CodPrzTab);
            ProdutoResumido pr1 = prs.First();

            if (Convert.ToDecimal(TB_Desconto.Text) > 0) //SE DESCONTO FOR NEGATIVO, SIGNIFICA QUE FOI DADO ACRESCIMO
            {
                if (Convert.ToDecimal(TB_Desconto.Text) > pr1.percentDesconto)
                {

                    Control ctrl = (Control)sender;
                    if (ctrl.ID != "btnImportar")
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Limite de desconto atingido ( " + pr1.percentDesconto + "). Preco alterado para valor de tabela.');", true);
                    }

                    TB_Desconto.Text = String.Empty;
                    TB_Preco.Text = LabelPreco.Text;
                    tbPrecoLiq.Text = TB_Preco.Text;

                }
            }
            else if (Convert.ToDecimal(TB_Desconto.Text) < 0) //SE DESCONTO FOR NEGATIVO, SIGNIFICA QUE FOI DADO ACRESCIMO
            {
                if (pr.PercBloqueio != null) //Se for null, nao validar percentual de bloqueio e permitir venda acima
                {
                    if (Math.Abs(Convert.ToDecimal(TB_Desconto.Text)) > pr.PercBloqueio) //Verifica percentual permitido
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Valor Ultrapassa percentual parametrizado para Preço acima da Tabela.');", true);
                        TB_Desconto.Text = String.Empty;
                    }
                    else
                    {
                        TB_Desconto.Text = String.Empty;
                        TextBoxQuantidade.Focus();
                        return;
                    }
                }
                else
                {
                    TB_Desconto.Text = String.Empty;
                    TextBoxQuantidade.Focus();
                    return;
                }
            }
        }

        tbPrecoLiq.Text = TB_Preco.Text;
        TB_Preco.Text = LabelPreco.Text;
        TextBoxQuantidade.Focus();

    }

    protected void TextDesconto_TextChanged(object sender, EventArgs e)
    {

        Int16 CodPrcTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').First());
        Int16 CodPrzTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').Last());

        //51516
        if (pr.CondicaoTabLivreWeb == 1 && cbPrazoTabela.SelectedIndex != 0)
        {
            CodPrzTab = Convert.ToInt16(cbPrazoTabela.SelectedValue);
        }

        tbPrecoLiq.Text = LabelPreco.Text;

        //FABIANO - 12/10/2011
        //desconto do item
        if (TB_Desconto.Text != "" && LabelPreco.Text != "")
        {
            var calculo_desconto = Convert.ToDecimal(LabelPreco.Text) * (Convert.ToDecimal(TB_Desconto.Text) / 100);
            var resultado = Convert.ToDecimal(LabelPreco.Text) - calculo_desconto;
            //TB_Preco.Text = String.Format("{0:0.00}", resultado);
            tbPrecoLiq.Text = String.Format("{0:0.00}", resultado);
        }

        //List<ProdutoResumido> prs = ClasseProdutos.Produto(u, Convert.ToInt32(cbProdutos.SelectedValue), pr, CodPrcTab, CodPrzTab);
        List<ProdutoResumido> prs = ClasseProdutos.buscaProdutosPorCodigo(u, pr, Convert.ToInt32(cbProdutos.SelectedValue), CodPrcTab, CodPrzTab);
        ProdutoResumido pr1 = prs.First();

        if (TB_Desconto.Text != "")
        {
            if (Convert.ToDecimal(TB_Desconto.Text) > pr1.percentDesconto)
            {
                //ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('Limite de desconto atingido, favor verificar desconto')</script>");
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Limite de desconto atingido (" + pr1.percentDesconto + "), favor verificar desconto');", true);
                TB_Desconto.Text = "";
                //TB_Preco.Text = String.Empty;
                tbPrecoLiq.Text = "";
            }
        }

        TextBoxQuantidade.Focus();

    }

    private void ImportarArquivoTexto(object sender, EventArgs e, String arquivo)
    {

        ClasseBanco conn = new ClasseBanco();

        List<String> listagem = new List<string>();
        string[] arrDados = new string[3];
        char[] separador = { '|' };
        string mensagem;
        string campos = "";
        int linhas = 0;
        string[,] valores = new string[999, 3]; //limite 999 itens

        using (StreamReader texto = new StreamReader(arquivo))
        {
            while ((mensagem = texto.ReadLine()) != null)
            {

                if (mensagem == "")
                {
                    break;
                }

                arrDados = mensagem.Split(separador);
                valores[linhas, 0] = arrDados[0]; //codigo
                valores[linhas, 1] = arrDados[1]; //qtd
                valores[linhas, 2] = arrDados[2].Replace(".", ","); //preco

                if (campos.ToString() != "")
                {
                    campos = campos + ", " + arrDados[0];
                }
                else
                {
                    campos = arrDados[0];
                }
                linhas++; //incrementa linha

            }
        }

        var r = conn.retornaQueryDataSet("SELECT CODSERVMERC FROM SERVMERC WHERE CODSERVMERC IN (" + campos.ToString() + ")");

        cbProdutos.DataSource = r.Tables[0];
        cbProdutos.DataTextField = r.Tables[0].Columns[0].ColumnName.ToString();  //CAMPO CODIGO NA PLANILHA
        cbProdutos.DataValueField = r.Tables[0].Columns[0].ColumnName.ToString(); //CAMPO CODIGO NA PLANILHA                
        cbProdutos.DataBind();

        r.Dispose();

        for (int i = 0; i < valores.Length; i++) // Loop through List with for
        {
            if (valores[i, 0] == null)
            {
                break;
            }
            arrDados[0] = valores[i, 0];
            arrDados[1] = valores[i, 1];
            arrDados[2] = valores[i, 2];

            //arrDados[2].ToString().Replace(".", ",") //preco
            AdicionarItem(sender,
                      e,
                      arrDados[0].ToString(), //produto
                      arrDados[1].ToString().Replace(".", ","), //qtd                      
                      String.Format("{0:" + Funcoes.Decimais(pr) + "}", arrDados[2].ToString().Replace(".", ","))
                      );

        }

    }

    private void ImportarPlanilhas(object sender, EventArgs e)
    {

        ////ABRE PLANILHA VIA OLE DB
        //OleDbConnection conexao = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Label1.Text + ";Extended Properties='Excel 12.0 Xml;HDR=YES';");

        //var sNomePlan = "";
        //var iPosicaoCodigo=0;
        //var iPosicaoValor=2;
        //var iPosicaoQtd=1;

        //if (opPC.Checked)
        //{
        //    sNomePlan = "[Plan1$]";
        //    iPosicaoCodigo = 0;
        //    iPosicaoValor=2;
        //    iPosicaoQtd = 1;
        //}
        //else if (opTablet.Checked)
        //{
        //    sNomePlan = "[DIGITACAO$]";
        //    iPosicaoCodigo = 0;
        //    iPosicaoValor = 3;
        //    iPosicaoQtd = 2;
        //}

        ////LE TODA A PLANILHA (PLAN1)
        //OleDbDataAdapter adapter = new OleDbDataAdapter("select * from " + sNomePlan, conexao);

        //DataSet ds = new DataSet(); //SIMILAR A UM RECORDSET

        ////http://social.msdn.microsoft.com/Forums/pt/vscsharppt/thread/6259475a-a5ab-4a1a-a805-53a96b98ba2a

        //try
        //{
        //    conexao.Open();     //ABRE CONEXAO COM EXCEL
        //    adapter.Fill(ds);   //CRIA O RECORDSET

        //    cbProdutos.DataSource = ds.Tables[0];
        //    cbProdutos.DataTextField = ds.Tables[0].Columns[iPosicaoCodigo].ColumnName.ToString();  //CAMPO CODIGO NA PLANILHA
        //    cbProdutos.DataValueField = ds.Tables[0].Columns[iPosicaoCodigo].ColumnName.ToString(); //CAMPO CODIGO NA PLANILHA                
        //    cbProdutos.DataBind();

        //    var iContador = 0;

        //    //LE TODOS ITENS DA PLANILHA
        //    foreach (DataRow linha in ds.Tables[0].Rows)
        //    {
        //        iContador = iContador + 1;

        //        if (opTablet.Checked) //se for tablet, inicia os produtos na linha 11
        //        {
        //            if (iContador >= 10)
        //            {
        //                if (linha[iPosicaoCodigo].ToString() != "" && linha[iPosicaoCodigo].ToString() !="0")
        //                {

        //                    /*
        //                    cbProdutos.SelectedValue = linha[iPosicaoCodigo].ToString(); //CAMPO CODIGO NA PLANILHA
        //                    cbProdutos_SelectedIndexChanged(sender, e); //EXECUTA COMO SE ESTIVESSE CLICANDO NO PRODUTO

        //                    TB_Preco.Text = linha[iPosicaoValor].ToString().Replace(".", ",");
        //                    TB_Preco_TextChanged(sender, e); //VALIDACAO DESCONTO MAIOR PERMITIDO
        //                    TextBoxQuantidade.Text = linha[iPosicaoQtd].ToString(); //QUANTIDADE

        //                    //DISPARA EVENTO CLICK COMO SE USUÁRIO ESTIVESSE DIGITANDO O ITEM NORMALMENTE...                 
        //                    LinkButtonAdicionar_Click(sender, e);
        //                    */

        //                    AdicionarItem(sender,
        //                              e,
        //                              linha[iPosicaoCodigo].ToString(),                                          
        //                              linha[iPosicaoQtd].ToString(),
        //                              linha[iPosicaoValor].ToString().Replace(".", ",")
        //                              );

        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (linha[iPosicaoCodigo].ToString() != "" && linha[iPosicaoCodigo].ToString() != "0")
        //            {
        //                /*
        //                cbProdutos.SelectedValue = linha[iPosicaoCodigo].ToString(); //CAMPO CODIGO NA PLANILHA
        //                cbProdutos_SelectedIndexChanged(sender, e); //EXECUTA COMO SE ESTIVESSE CLICANDO NO PRODUTO

        //                TB_Preco.Text = linha[iPosicaoValor].ToString().Replace(".", ",");
        //                TB_Preco_TextChanged(sender, e); //VALIDACAO DESCONTO MAIOR PERMITIDO
        //                TextBoxQuantidade.Text = linha[iPosicaoQtd].ToString(); //QUANTIDADE

        //                //DISPARA EVENTO CLICK COMO SE USUÁRIO ESTIVESSE DIGITANDO O ITEM NORMALMENTE...                 
        //                LinkButtonAdicionar_Click(sender, e);
        //                 * */

        //                AdicionarItem(sender, 
        //                              e, 
        //                              linha[iPosicaoCodigo].ToString(),                                           
        //                              linha[iPosicaoQtd].ToString(),
        //                              linha[iPosicaoValor].ToString().Replace(".", ",") 
        //                              );

        //            }
        //        }
        //    }

        //    cbProdutos.Items.Clear(); //LIMPAR O DROPDOWN

        //}
        //catch (Exception ex)
        //{
        //    //throw new Exception("Erro ao acessar a planilha : " + ex.Message);
        //    //ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('Erro ao acessar a planilha : " + ex.Message + "')</script>");
        //    ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Erro ao acessar a planilha : " + ex.Message + "');", true);
        //}
        //finally
        //{
        //    conexao.Close();
        //}

    }

    private void AdicionarItem(object sender, EventArgs e, String Codigo, String Qtd, String Valor)
    {
        cbProdutos.SelectedValue = Convert.ToString(Convert.ToDouble(Codigo)); //CAMPO CODIGO NA PLANILHA
        cbProdutos_SelectedIndexChanged(sender, e); //EXECUTA COMO SE ESTIVESSE CLICANDO NO PRODUTO

        ClasseCliente csCliente = new ClasseCliente();

        TB_Preco.Text = Valor;
        TB_Preco_TextChanged(sender, e); //VALIDACAO DESCONTO MAIOR PERMITIDO
        TextBoxQuantidade.Text = Qtd;   //QUANTIDADE

        //DISPARA EVENTO CLICK COMO SE USUÁRIO ESTIVESSE DIGITANDO O ITEM NORMALMENTE...                 
        LinkButtonAdicionar_Click(sender, e);
    }

    protected void TextBoxQuantidade_TextChanged(object sender, EventArgs e)
    {

    }

    protected void DropDownListCliente_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (csMovimentacao.SelectedValue.ToString() != "-1")
        {
            try
            {
                int CodTipMov = ClassePedido.ValidaMovimentacao(Convert.ToInt32(cbCliente.SelectedValue), Convert.ToInt16(Session["EmpresaCODEMP"]), Convert.ToInt16(csMovimentacao.SelectedValue));

                if (CodTipMov.Equals(0))
                {
                    string rsp = "Não há Tipo de Movimentação Padrão cadastrada nos Parâmetros de Venda.";
                    ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + rsp + "');", true);
                }
            }
            catch (Exception exc)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + exc.Message + "');", true);
            }
        }

        lbMensagem.Text = "";
        csCliente.DadosCliente(Convert.ToInt32(cbCliente.SelectedValue));
        lbMensagem.Text = csCliente.MensagemBloqueio(Convert.ToInt32(cbCliente.SelectedValue));

        //Telefone e Email - 50732
        lbDados.Text = csCliente.EndCli + " - N.: " + csCliente.Numero + " - " + csCliente.CidCli + " - " + csCliente.EstCli + " - CNPJ : " + csCliente.CGC_CPF + " - Telefone : " + csCliente.TelCli + " - Email : " + csCliente.Email;


        //49888
        var rsCliente = conn.Query("SELECT MOTIVO FROM FINANCLI WHERE CODCLI = " + cbCliente.SelectedValue + "");
        TextBoxObservacao.Text = "";

        if (rsCliente.Read())
        {
            if (TextBoxObservacao.Text != "")
            {
                TextBoxObservacao.Text = TextBoxObservacao.Text + " - " + rsCliente["motivo"].ToString().Trim();
            }
            else
            {
                TextBoxObservacao.Text = rsCliente["motivo"].ToString().Trim();
            }
        }
        rsCliente.Close();
        //49888

        if (!csCliente.CLIE_TabPrecoPadrao.Equals(null) && !csCliente.CLIE_TabPrecoPadrao.Equals(0))
        {
            ClasseTabelaPrecos[] tab = ClasseTabelaPrecos.BuscaTabelaPreco(Convert.ToInt16(csCliente.CLIE_TabPrecoPadrao), 0).ToArray();

            cbTabelas.Items.Clear();
            cbPrazoTabela.Items.Clear();

            bool bAdicionar = true;

            foreach (ClasseTabelaPrecos tb in tab)
            {
                cbTabelas.Items.Insert(0, new ListItem(tb.CodTipPrc.ToString() + " - " + tb.DesTipPrc + " ==> " + tb.CodTipPrz.ToString() + " - " + tb.DesTipPrz, tb.CodTipPrc.ToString() + "|" + tb.CodTipPrz.ToString()));

                for (int i = 0; i < cbPrazoTabela.Items.Count; i++) // Loop through List with for
                {
                    cbPrazoTabela.SelectedIndex = i;

                    if (tb.CodTipPrz == Convert.ToInt16(cbPrazoTabela.SelectedValue))
                    {
                        bAdicionar = false;
                    }
                }

                if (bAdicionar == true)
                {
                    cbPrazoTabela.Items.Insert(0, new ListItem(tb.CodTipPrz.ToString() + " - " + tb.DesTipPrz, tb.CodTipPrz.ToString()));
                }

                bAdicionar = true;

            }

            cbTabelas.Items.Insert(0, new ListItem("(Selecione uma tabela Válida)", "-1"));
            cbPrazoTabela.Items.Insert(0, new ListItem("", "-1"));
            cbPrazoTabela.SelectedIndex = -1;

        }
        else
        {
            BuscaTabelaPadrao();
        }

        var resp = ClassePedido.VerificaCreditos(Convert.ToInt32(cbCliente.SelectedValue), Convert.ToInt16(Session["EmpresaCODEMP"]));

        if (!resp.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + resp + "');", true);
        }

        CarregaCondicoesPagamento(Convert.ToInt32(cbCliente.SelectedValue), Convert.ToInt16(csMovimentacao.SelectedValue));

    }

    protected void TB_DescPed_TextChanged(object sender, EventArgs e)
    {
        if (TB_DescPed.Text != "")
        {
            //calcula valor desconto 
            if (TB_DescPed.Text != "" && tbSubTotal.Text != "")
            {
                var _desconto = Convert.ToDecimal(tbSubTotal.Text) * (Convert.ToDecimal(TB_DescPed.Text) / 100);
                TB_VlrDes.Text = String.Format("{0:0.00}", _desconto);
            }

            DataClassesDataContext dcdc = new DataClassesDataContext();

            VENDEDOR vend = dcdc.VENDEDORs.SingleOrDefault(vnd => vnd.CodVend == Convert.ToInt32(Session["CodVend"]));
            var calculo_desconto = vend.VEND_DescMaximo == null ? 0 : vend.VEND_DescMaximo;

            if (Convert.ToDecimal(TB_DescPed.Text) > calculo_desconto && Session["DESCONTO_CONDICAO"].ToString() == "")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Limite de desconto do vendedor ultrapassado. Favor verificar');", true);
                TB_DescPed.Text = String.Empty;
                TB_VlrDes.Text = String.Empty;
                tbTotalPedido.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", tbSubTotal.Text);
                TB_DescPed.Focus();
            }
            else
            {
                if (tbSubTotal.Text != "")
                {
                    decimal desconto = Convert.ToDecimal(tbSubTotal.Text) - (Convert.ToDecimal(tbSubTotal.Text) * Convert.ToDecimal(TB_DescPed.Text) / 100);
                    tbTotalPedido.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", Convert.ToString(desconto));
                }

            }
        }
        else if (TB_DescPed.Text == "" && TB_VlrDes.Text == "")
        {
            tbTotalPedido.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", tbSubTotal.Text);
        }

    }

    protected void TB_VlrDes_TextChanged(object sender, EventArgs e)
    {
        //FABIANO - 04/11/2011
        if (TB_VlrDes.Text != "" && tbSubTotal.Text != "")
        {
            var sub_total = Convert.ToDecimal(tbSubTotal.Text) - Convert.ToDecimal(TB_VlrDes.Text);
            var calculo_porcentagem = 100 - (Convert.ToDecimal(sub_total) * 100) / Convert.ToDecimal(tbSubTotal.Text);

            if (calculo_porcentagem > 0)
            {
                TB_DescPed.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", calculo_porcentagem);
                tbTotalPedido.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", sub_total);
            }
        }
        else if (TB_DescPed.Text == "" && TB_VlrDes.Text == "")
        {
            tbTotalPedido.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", tbSubTotal.Text);
        }

        DataClassesDataContext dcdc = new DataClassesDataContext();

        VENDEDOR vend = dcdc.VENDEDORs.SingleOrDefault(vnd => vnd.CodVend == Convert.ToInt32(Session["CodVend"]));
        var calculo_desconto = vend.VEND_DescMaximo == null ? 0 : vend.VEND_DescMaximo;

        if (Convert.ToDecimal(TB_DescPed.Text == "" ? "0" : TB_DescPed.Text) > calculo_desconto)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Limite de desconto do vendedor ultrapassado. Favor verificar');", true);
            TB_DescPed.Text = String.Empty;
            TB_VlrDes.Text = String.Empty;
            tbTotalPedido.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", tbSubTotal.Text);
            TB_VlrDes.Focus();
        }

    }
    //52683
    private void LimpaCamposItens()
    {
        txtPesqProduto.Text = String.Empty;
        TB_Desconto.Text = String.Empty;
        cbProdutos.SelectedIndex = -1;
        TB_Preco.Text = String.Empty;
        tbPrecoLiq.Text = String.Empty;
        TextBoxQuantidade.Text = String.Empty;
        LabelPreco.Text = String.Empty;
        LabelSaldo.Text = String.Empty;
        lbPeso.Text = String.Empty;
        lbUnidade.Text = String.Empty;
    }

    protected void DropDownListCondPagto_SelectedIndexChanged(object sender, EventArgs e)
    {

        LimpaCamposItens();//52683

        if (DropDownListCondPagto.Text != "")
        {
            conn.AbrirBanco();

            Int32 codFormaPagto = Convert.ToInt32(DropDownListCondPagto.SelectedValue.Split('|').First());
            Int32 codCondPagto = Convert.ToInt32(DropDownListCondPagto.SelectedValue.Split('|').Last());

            strSql.Length = 0;
            strSql.Append(" SELECT ISNULL(PercDes,0) FROM CONDPAGTO ");
            strSql.Append(" WHERE CODTIPPRZ = " + codCondPagto + " AND CodFrmPgt = " + codFormaPagto + " AND Codemp = " + u.CodEmp + "");
            var dados = conn.Query(strSql.ToString());

            Session["DESCONTO_CONDICAO"] = "";

            if (dados.Read())
            {
                TB_DescPed.Text = dados[0].ToString();

                if (Convert.ToDecimal(TB_DescPed.Text) != 0)
                {
                    Session["DESCONTO_CONDICAO"] = "SIM";
                }

                TB_DescPed_TextChanged(sender, e);
            }
            dados.Dispose();

            bool bAchou = false;

            /*49780*/
            strSql.Length = 0;
            strSql.Append(" SELECT PRAZOTAB FROM ITCONPAGTO ");
            strSql.Append(" WHERE CODTIPPRZ = " + codCondPagto + " AND CodFrmPgt = " + codFormaPagto + " AND Codemp = " + u.CodEmp + "");
            dados = conn.Query(strSql.ToString());

            if (dados.Read())
            {
                codCondPagto = Convert.ToInt32(dados[0].ToString());
            }
            dados.Dispose();
            /*49780*/

            //51114
            //Não deixar mudar se estiver desabilitado o combo tabela, isto quer dizer que já tem venda em andamento
            if (cbTabelas.Enabled == true)
            {
                /*Procura na tabela de preco o prazo da condicao selecionada*/
                for (int proc = 0; proc < cbTabelas.Items.Count; proc++)
                {
                    cbTabelas.SelectedIndex = proc;

                    int Prazo = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').Last());

                    if (codCondPagto.Equals(Prazo))
                    {
                        bAchou = true;
                        //49560 - Fabiano
                        //cbTabelas.Enabled = false; 
                        break;
                    }
                }

                /*Se não encontrou prazo, seleciona tabela em branco*/
                if (bAchou.Equals(false))
                {
                    cbTabelas.SelectedIndex = -1;
                    //49560 - Fabiano
                    cbTabelas.Enabled = true;
                    DropDownListCondPagto.Enabled = true;
                }
            }
        }
    }

    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        UpdatePanel6.Visible = false;
        UpdatePanel5.Visible = true;
        //LinkButton2.CssClass = "abas_pedido";
        //LinkButton3.CssClass = "abas_branco";
        tbCliente.Focus();
    }

    protected void LinkButton3_Click(object sender, EventArgs e)
    {
        UpdatePanel5.Visible = false;
        UpdatePanel6.Visible = true;
        //LinkButton2.CssClass = "aba_branco_dados";
        //LinkButton3.CssClass = "aba_itens";
        txtPesqProduto.Focus();
    }


    protected void cbUnidadeVenda_SelectedIndexChanged(object sender, EventArgs e)
    {
        cbProdutos.SelectedIndex = -1;

        if (cbUnidadeVenda.SelectedValue == "Maior Unidade")
        {
            lbTipoPreco.Text = "Preço Maior Unidade";
        }
        else
        {
            lbTipoPreco.Text = "Preço Menor Unidade";
        }
    }
    protected void csMovimentacao_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            int CodTipMov = ClassePedido.ValidaMovimentacao(Convert.ToInt32(cbCliente.SelectedValue), Convert.ToInt16(Session["EmpresaCODEMP"]), Convert.ToInt16(csMovimentacao.SelectedValue));

            if (CodTipMov.Equals(0))
            {
                string rsp = "Não há Tipo de Movimentação Padrão cadastrada nos Parâmetros de Venda.";
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + rsp + "');", true);
            }
        }
        catch (Exception exc)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + exc.Message + "');", true);
        }


        CarregaCondicoesPagamento(0, Convert.ToInt16(csMovimentacao.SelectedValue));


    }
    protected void cbTabelas_SelectedIndexChanged(object sender, EventArgs e)
    {
        conn.AbrirBanco();

        LimpaCamposItens();//52683

        Int32 codFormaPagto = Convert.ToInt32(DropDownListCondPagto.SelectedValue.Split('|').First());
        Int32 codCondPagto = Convert.ToInt32(DropDownListCondPagto.SelectedValue.Split('|').Last());

        strSql.Length = 0;
        strSql.Append(" SELECT PRAZOTAB FROM ITCONPAGTO ");
        strSql.Append(" WHERE CODTIPPRZ = " + codCondPagto + " AND CodFrmPgt = " + codFormaPagto + " AND Codemp = " + u.CodEmp + "");
        var dados = conn.Query(strSql.ToString());

        if (dados.Read())
        {
            codCondPagto = Convert.ToInt32(dados[0].ToString());
        }
        dados.Dispose();

        Int32 iPrazoTabela = Convert.ToInt32(cbTabelas.SelectedValue.Split('|').Last());

        if (pr.CondicaoTabLivreWeb != 1) //51516
        {
            if (iPrazoTabela != codCondPagto)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Prazo selecionado da tabela de preço não é igual ao prazo da condição de pagamento. Selecione uma tabela válida ou altere a condição de pagamento.');", true);
                cbTabelas.SelectedIndex = -1;
            }
        }

    }
    protected void cbPrazoTabela_SelectedIndexChanged(object sender, EventArgs e)
    {
        TB_Preco.Text = "";
        if (cbProdutos.SelectedValue != "-1" && cbProdutos.SelectedValue != "")
        {
            cbProdutos_SelectedIndexChanged(null, null);
        }
    }
}