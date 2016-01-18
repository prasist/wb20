﻿using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebPedidos.WSClasses;
using System.Collections.Generic;
using System.IO;
using System.Text;

public partial class importacao : System.Web.UI.Page 
{
    UsuarioResumido u;
    ParametroResumido pr;
    ClasseBanco conn = new ClasseBanco();
    Funcoes csFuncoes = new Funcoes();

    private static string _mensagem;
    private static string _mensagem_desc;
    private static string _mensagem_prc;

    private static String MensagemLimitePreco
    {
        get { return _mensagem_prc; }
        set { _mensagem_prc = value; }
    }

    private static String MensagemLimiteDesconto
    {
        get { return _mensagem_desc; }
        set { _mensagem_desc = value; }
    }

    private static String MensagemItemNaoAtendido
    {
        get { return _mensagem; }
        set { _mensagem = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        lbErros.Text = "";
        lbErros.Visible = false;

        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        
        u = (UsuarioResumido)Session["Usuario"];
        pr = (ParametroResumido)Session["Parametros"];

        if (!IsPostBack)
        {
            tbDataEmissaoInicial.Text = String.Format("{0:dd/MM/yyyy}", DateTime.Today);
            tbDataEmissaoFinal.Text = String.Format("{0:dd/MM/yyyy}", DateTime.Today);
            buAba1_Click(null, null);

            if (u == null)
            {
                Response.Redirect("Default.aspx");
            }

            //Somente se vier com paramêtro para gravação automática pedidos
            if (!String.IsNullOrEmpty(Request.QueryString["flg"]))
            {
                try
                {
                    ImportacaoLote();
                }
                catch (Exception exc)
                {
                    lbErros.Text = exc.Message.ToString();
                    lbErros.Visible = true;
                }
            }

        }

    }

    
    private void AdicionarItem(String Codigo, String Qtd, String Valor, String lNumPedWeb, String lNumPed, String PercDesconto, Int16 CodPrcTab, Int16 CodPrzTab, int iUnidadeVenda)
    {

        if (CodPrcTab.Equals(0) && CodPrzTab.Equals(0))
        {
            CodPrcTab = (Int16)pr.CodTipPrc;
            CodPrzTab = (Int16)pr.CodTipPrz;
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


        try
        {
            //ClasseProdutos.Produto((UsuarioResumido)Session["Usuario"], Convert.ToInt32(Codigo), pr, CodPrcTab, CodPrzTab).First();
            ClasseProdutos.buscaProdutosPorCodigo((UsuarioResumido)Session["Usuario"], pr, Convert.ToInt32(Codigo), CodPrcTab, CodPrzTab).First();
        }
        catch (Exception) //Nao encontrou produto, inativo ou foi excluido...
        {
            //Insere na log dos itens
            conn.ExecutarComando("INSERT INTO LOG_ITENS_NAO_ATENDIDOS_WEB (NUMPED, CODSERVMERC, QTD, VALOR) VALUES (" + lNumPed + ", " + Codigo + ", '" + Qtd.Replace(",", ".") + "', '" + Valor.Replace(",", ".") + "')");

            if (String.IsNullOrEmpty(MensagemItemNaoAtendido))
            {
                MensagemItemNaoAtendido = "Produto Inativo ou não encontrado";
            }

            return;        
        }
        
        //ProdutoResumido pr1 = ClasseProdutos.Produto((UsuarioResumido)Session["Usuario"], Convert.ToInt32(Codigo), pr, CodPrcTab, CodPrzTab).First();
        ProdutoResumido pr1 = ClasseProdutos.buscaProdutosPorCodigo((UsuarioResumido)Session["Usuario"], pr, Convert.ToInt32(Codigo), CodPrcTab, CodPrzTab).First();
        //pr1.Desconto = CalculaDesconto(Codigo, Valor, Convert.ToString(pr1.Preco), pr1);
        pr1.Desconto = Convert.ToDecimal(PercDesconto == "" ? "0" : PercDesconto.Replace(".", ","));
        pr1.PrecoReal = pr1.Preco; //52557 - Preco de Tabela
        pr1.Preco    = Convert.ToDecimal(Valor.Replace(".", ",")); //Ajuste, nao estava gravando preco maior

        //pr1.Desconto = TB_Desconto.Text == "" ? 0 : Convert.ToDecimal(TB_Desconto.Text);
        pr1.TotalParcial = 0;
        pr1.Quantidade = Convert.ToDecimal(Qtd);
        var desconto = String.Format("{0:" + Funcoes.Decimais(pr) + "}", (pr1.Preco * (pr1.Desconto / 100)));        
        pr1.TotalParcial = ((pr1.Preco - Convert.ToDecimal(desconto)) * pr1.Quantidade);        
        pr1.preco_liquido = pr1.TotalParcial / pr1.Quantidade;
        pr1.UnidadeVenda = iUnidadeVenda.ToString();
        pr1.Itp_CodTabPrz = CodPrzTab;

        if (iUnidadeVenda.Equals(2))
        {
            pr1.Preco = Convert.ToDecimal(String.Format("{0:" + Funcoes.Decimais(pr) + "}", pr1.Preco)) * pr1.Quantidade;            
        }
        
        try
        {
            Boolean retorno = ClassePedido.ReservarItemPedidoWeb(u, Convert.ToInt32(lNumPed), Convert.ToInt32(Codigo), Convert.ToDecimal(Qtd), pr);
            if (retorno)
            {
                products.Add(pr1);
            }
        }
        catch (Exception)
        {            
                //Insere na log dos itens
                conn.ExecutarComando("INSERT INTO LOG_ITENS_NAO_ATENDIDOS_WEB (NUMPED, CODSERVMERC, QTD, VALOR) VALUES (" + lNumPed + ", " + Codigo + ", '" + Qtd.Replace(",", ".") + "', '" + Valor.Replace(",", ".") + "')");

                if (String.IsNullOrEmpty(MensagemItemNaoAtendido))
                {
                    MensagemItemNaoAtendido = "Saldo Indisponível";
                }
                
        }        
    }

    private Decimal CalculaDesconto(String PercDesconto, String Codigo, String ValorVenda, String ValorTabela, ProdutoResumido prm, Int16 CodPrcTab, Int16 CodPrzTab)
    {
        var cValorVenda = ValorVenda.Replace(".", ",");
        var cValorTabela = ValorTabela.Replace(".", ",");

        decimal cPerc = 0;

        if (String.IsNullOrEmpty(PercDesconto))
        {
            cPerc = Convert.ToDecimal(cValorVenda) * 100 / Convert.ToDecimal(Math.Round(Convert.ToDecimal(cValorTabela), 2));
        }
        else
        {
            cPerc = Convert.ToDecimal(PercDesconto);
        }

        var calculo_porcentagem = 100 - Convert.ToDecimal(Math.Round(cPerc));

        String cDesconto = String.Format("{0:0.00}", calculo_porcentagem);

        //List<ProdutoResumido> prs = ClasseProdutos.Produto(u, Convert.ToInt32(Codigo), pr, CodPrcTab, CodPrzTab);
        List<ProdutoResumido> prs = ClasseProdutos.buscaProdutosPorCodigo(u, pr, Convert.ToInt32(Codigo), CodPrcTab, CodPrzTab);
        ProdutoResumido pr1 = prs.First();

        if (Convert.ToDecimal(cDesconto) > 0) //SE DESCONTO FOR NEGATIVO, SIGNIFICA QUE FOI DADO ACRESCIMO
        {
            if (Convert.ToDecimal(cDesconto) > pr1.percentDesconto)
            {

                MensagemLimiteDesconto = " - Limite de desconto atingido. Preco alterado para valor de tabela.";
                
                prm.Preco = Convert.ToDecimal(cValorTabela);
                return 0;
            }
            else
            {
                //prm.Preco = Convert.ToDecimal(cValorVenda);
                return Convert.ToDecimal(cDesconto);                
            }
        }
        else if (Convert.ToDecimal(cDesconto) < 0)
        { //SE DESCONTO FOR NEGATIVO, SIGNIFICA QUE FOI DADO ACRESCIMO
            if (pr.PercBloqueio != null) //Se for null, nao validar percentual de bloqueio e permitir venda acima
            {
                if (Math.Abs(Convert.ToDecimal(cDesconto)) > pr.PercBloqueio) //Verifica percentual permitido
                {
                    MensagemLimitePreco = "Valor Ultrapassa percentual parametrizado para Preço acima da Tabela.";                    
                    prm.Preco = Convert.ToDecimal(cValorTabela);
                    return 0;
                }
                else
                {
                    //prm.Preco = Convert.ToDecimal(cValorVenda);
                    return Convert.ToDecimal(cDesconto);
                }
            }
            else
            {
                //prm.Preco = Convert.ToDecimal(cValorVenda);
                return Convert.ToDecimal(cDesconto);
            }
        }
        else
        {
            //prm.Preco = Convert.ToDecimal(cValorVenda);
            return Convert.ToDecimal(cDesconto);
        }
        
    }

    private void GravaPedido(Int32 Cliente, int Forma, int Prazo, string lNumPedWeb, string lNumPed, string Obs, int CodPrcTab, int CodPrzTab, int UnidadeVenda, int iCodTipMov, string sPedidoAtual)
    {

        try
        {
            List<ProdutoResumido> produtos = (List<ProdutoResumido>)ViewState["Produtos"];

            int resultado           = 0;
            string cValorPedido     = "0";
            string sMensagemLog     = "";
            decimal vlr_desconto    = 0;
            
            StringBuilder sObservacao = new StringBuilder(60);

            //49888
            var rsCliente = conn.Query("SELECT MOTIVO FROM FINANCLI WHERE CODCLI = " + Cliente + "");
            sObservacao.Length = 0;

            if (rsCliente.Read())
            {
                if (Obs != "")
                {
                    sObservacao.Append(Obs + " - " + rsCliente["motivo"].ToString().Trim());
                }
                else
                {
                    sObservacao.Append(rsCliente["motivo"].ToString().Trim());
                }

                //Se ultrapassou 60 caracteres 
                if (sObservacao.Length > 60)
                {
                    Obs = sObservacao.ToString().Substring(0, 60);
                }
                else
                {
                    Obs = sObservacao.ToString();
                }

            }

            rsCliente.Close();
            //49888

            resultado = ClassePedido.InserePedido((UsuarioResumido)Session["Usuario"],
                                                    (ParametroResumido)Session["Parametros"],
                                                    u.CodVend,
                                                    Cliente,
                                                    Obs,
                                                    Forma,
                                                    Prazo,
                                                    produtos,
                                                    (int)Session["EmpresaCODEMP"],
                                                    Convert.ToInt32(lNumPedWeb),
                                                    Convert.ToInt32(lNumPed),
                                                    vlr_desconto,
                                                    CodPrcTab,
                                                    CodPrzTab, UnidadeVenda, iCodTipMov);

            ClassePedido.RemoverReservaItensPedidoWeb(u);
            ClasseBloqueioFinancerio cls_BloqueioCliente = new ClasseBloqueioFinancerio();
            bool bNaoAtendido = false;

            //Pega total do pedido inserido
            //Se valor 
            var rs = conn.Query("SELECT VLRTOT FROM PEDIDO WHERE NUMPED = " + lNumPed + " AND CODEMP = " + u.CodEmp);

            if (rs.Read())
            {
                cValorPedido = rs[0].ToString();
                //Envia copia por email ao cliente
                EnviarEmail(lNumPed, Cliente.ToString(), produtos);
                rs.Close();
            }
            else
            {
                rs.Close();
                bNaoAtendido = true;
            }
            
            string resposta = cls_BloqueioCliente.BloqueioFinancerio(Cliente,
                Convert.ToInt32(resultado),
                Convert.ToInt16(CodPrcTab),
                Convert.ToInt16(CodPrzTab),
                Convert.ToInt16(pr.CodEmp), cValorPedido, "", 0, 0, 0);

            string sStatus = "";

            if (resposta != "OK")
            {
                sMensagemLog = resposta;
            }

            if (bNaoAtendido == true)
            {
                sStatus = "T";
            }
            else
            {
                sStatus = String.IsNullOrEmpty(MensagemItemNaoAtendido) == true ? "A" : "P";
            }

            string sMontaMensagens = sMensagemLog + Environment.NewLine +
                                     MensagemItemNaoAtendido + Environment.NewLine +
                                         MensagemLimitePreco + Environment.NewLine +
                                             MensagemLimiteDesconto;

            //Insere na log 
            conn.ExecutarComando("INSERT INTO LOG_IMPORTACAO_WEB (NUMPED, CODCLI, DATA, CODVEN, STATUS, VALOR, MSG, NUMERO_OFFLINE, NOMEARQUIVO) VALUES (" + lNumPed + ", " + Cliente + ", GETDATE(), " + u.CodVend + ", '" + sStatus + "', '" + cValorPedido.Replace(",", ".") + "', ' - " + sMontaMensagens + "', '" + sPedidoAtual + "', '" + Label1.Text + "')");
            ViewState["Produtos"] = null;
        }
        catch (Exception exc)
        {
            throw new Exception(exc.Message);      
        }
    }

    private void EnviarEmail(string resultado, string codCliente, List<ProdutoResumido> produtos)
    {

        //48426
        //Verifica se envia email ao cliente
        if (Funcoes.BuscaCampoTabela("EnviarEmailWeb", "PARAMETROS", " AND CODEMP = " + pr.CodEmp) == "1")
        {
            //busca email do cliente
            string sEmailCliente = Funcoes.BuscaCampoTabela("Email", "CLIENTE", " AND CODCLI = " + codCliente + "");

            if (sEmailCliente == "") //se nao tiver email sai da rotina...
            {
                return;
            }

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

                sTexto.Append("<tr><td>" + produto.Codigo + " - " + servMercadoria.DesServMerc.ToString() + "</td><td>" + produto.Quantidade + "</td><td>" + String.Format("{0:0.00}", produto.preco_liquido) + "</td><td>" + String.Format("{0:0.00}", cTotalItens) + "</td>");
            }

            sTexto.Append("<tr><td></td><td></td><td></td><td></td>");
            sTexto.Append("<tr><td></td><td></td><td></td><td>" + String.Format("{0:0.00}", cTotalGeral) + "</td>");
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
                    lbErros.Text = excEmail.Message;
                    lbErros.Visible = true;
                }
            }
        }
    }

    protected void buImportar_Click(object sender, EventArgs e)
    {
        
        if (FileUpload1.HasFile)
        {
            //verifica se o arquivo nao foi importado ainda
            if (System.IO.File.Exists(Server.MapPath(@"~\planilhas\importados\" + u.CodVend + "\\" + FileUpload1.FileName)) == false || ckPermite.Checked ==true)            
            {
                FileUpload1.SaveAs(Server.MapPath(@"~\planilhas\" + FileUpload1.FileName)); //SALVA ARQUIVO SELECIONADO NA PASTA LOCAL
                Label1.Text = Server.MapPath(@"~\planilhas\" + FileUpload1.FileName);
                Label2.Text = FileUpload1.FileName;

                if (!String.IsNullOrEmpty(Label2.Text))
                {
                    buDownload.Visible = true;
                    FileUpload1.Visible = false;
                    buImportar.Visible = false;

                    lbPasso1.Text = "Passo 1 concluído com sucesso!";
                    lbPasso2.Text = "Passo 2 concluído com sucesso!";
                    
                    //lbTexto.Visible = true;
                }
            }
            else
            {                
                lbErros.Text = "Arquivo " + FileUpload1.FileName + " já importado anteriormente. Para importá-lo novamente renomear com outro nome ou marcar a opção Permitir importar o mesmo arquivo.";
                lbErros.Visible=true;                
            }
        }
        
        FileUpload1.Dispose();
    }

    private void ImportacaoLote()
    {
        try
        {
            System.Collections.ArrayList arquivos = new System.Collections.ArrayList();

            //Verifica se foi recebido arquivos do vendedor logado
            var rsLeitura = conn.Query("SELECT * FROM LOG_ARQUIVOS_RECEBIDOS WHERE CODVEN = " + u.CodUsu);

            while (rsLeitura.Read())
            {
                //Pega path e nome do arquivo
                arquivos.Add(rsLeitura["ARQUIVO"].ToString() + "|" + rsLeitura["NOME_ARQUIVO"].ToString());
            }
            rsLeitura.Close();
            rsLeitura.Dispose();

            //Enquanto houverem arquivos do vendedor
            for (int contador = 0; contador != arquivos.Count; contador++)
            {
                string[] sColunas;

                sColunas = arquivos[contador].ToString().Split('|');

                Label1.Text = sColunas[0].ToString(); //Path completo e nome do arquivo
                Label2.Text = sColunas[1].ToString(); //Somente nome do arquivo

                //Dispara mesma rotina de importação como se estivesse fazendo importação manual
                ImportaArquivo(true);

                //Exclui arquivo do LOG 
                conn.ExecutarComando("DELETE FROM LOG_ARQUIVOS_RECEBIDOS WHERE CODVEN = " + u.CodUsu + " AND NOME_ARQUIVO = '" + Label2.Text + "'");

                //cria diretorio do vendedor se nao existir
                System.IO.Directory.CreateDirectory(Server.MapPath(@"~\planilhas\importados\" + u.CodVend));

                //move o arquivo importado para a pasta importados
                System.IO.File.Copy(Label1.Text, Server.MapPath(@"~\planilhas\importados\" + u.CodVend + "\\" + Label2.Text));

                //exclui o arquivo da pasta de recepção
                System.IO.File.Delete(Server.MapPath(@"~\planilhas\recebidos_offline\" + Label2.Text));
            }
        }
        catch (Exception e)
        {
            lbErros.Text = e.Message.ToString();
            lbErros.Visible = true;
        }

    }

    private bool VerificaImportacaoPedido(string sCliente, string sPedido, string sNomeArquivo, string sVendedor)
    {
        bool bRetorno = false;

        //var rsLeitura = conn.Query("SELECT 1 FROM LOG_IMPORTACAO_WEB WHERE CODCLI = " + sCliente + " AND CODVEN = " + sVendedor + " AND NUMERO_OFFLINE = " + sPedido + " AND CONVERT(CHAR(10), data , 112) = '" + Funcoes.RetornaDataQuery(sData) + "' ");
        var rsLeitura = conn.Query("SELECT 1 FROM LOG_IMPORTACAO_WEB WHERE CODCLI = " + sCliente + " AND CODVEN = " + sVendedor + " AND NUMERO_OFFLINE = " + sPedido + " AND NOMEARQUIVO = '" + sNomeArquivo + "' ");
        
            if (rsLeitura.Read())
            {
                bRetorno = true; //Encontrou, não importar novamente
            }
            else
            {
                bRetorno = false;
            }
                
        rsLeitura.Close();
        rsLeitura.Dispose();

        return bRetorno;
    }

    private void ImportaArquivo(bool bLote)
    {
               
        List<String> listagem = new List<string>();
        string[] arrDados = new string[3];
        char[]   separador      = { '|' };
        string   mensagem;        
        string   sPedidoAtual   = "";
        string   lNumPedWeb;
        string   lNumPed;
        string   sObs = "";
        string[,] valores = new string[999, 3]; //Limite 999 itens
        int?     iCodTipMov     = 0;
        int      linhas         = 0;
        int      iCliente       = 0;
        int      iForma         = 0;
        int      iPrazo         = 0;
        Int16    iCodPrcTab     = 0;
        Int16    iCodPrzTab     = 0;
        int      iUnidadeVenda  = 0;
        int      iPedidosJaGravados = 0;
        int      iTotalPedidos  = 0;
         
        MensagemItemNaoAtendido = "";

        try
        {
            if (Label1.Text.Substring((Label1.Text.Length - 4), 4) == ".txt")
            {
                //Estrutura novo arquivo:
                //PEDIDO | FORMA PAGTO | PRAZO PAGAMENTO | CLIENTE | PRODUTO | QTD | PRECO | DESC | OBS | CODPRCTAB | CODPRZTAB | UNIDVENDA | CODTIPMOV
                lNumPedWeb = ClassePedido.ProximoPedidoWeb(u).ToString();
                lNumPed = ClassePedido.ProximoPedido(u).ToString();

                using (StreamReader texto = new StreamReader(Label1.Text))
                {
                    while ((mensagem = texto.ReadLine()) != null)
                    {
                        arrDados = mensagem.Split(separador);

                        //Verifica se pedido já não foi importado
                        //if (VerificaImportacaoPedido(arrDados[3], arrDados[0], DateTime.Today.ToShortDateString(), u.CodVend.ToString()) == false)
                        if (VerificaImportacaoPedido(arrDados[3], arrDados[0], Label1.Text, u.CodVend.ToString()) == false)
                        {
                            if (sPedidoAtual == arrDados[0])
                            {
                                iCodPrcTab = Convert.ToInt16(arrDados[9]);
                                iCodPrzTab = Convert.ToInt16(arrDados[10]);
                                iUnidadeVenda = Convert.ToInt16(arrDados[11] == "Maior Unidade" ? 2 : 0);

                                if (arrDados.Length > 12)
                                {
                                    iCodTipMov = Convert.ToInt16(arrDados[12]);                                    
                                }

                                if (arrDados.Length > 13) //Somente se vier com ***
                                {
                                    
                                    if (Funcoes.BuscaCampoTabela("EstCli", "CLIENTE", " AND CODCLI = " + iCliente + "") !=
                                        Funcoes.BuscaCampoTabela("Estado", "EMPRESA", " AND CODEMP = " + u.CodEmp + ""))
                                    {
                                        iCodTipMov = Convert.ToInt16(Funcoes.BuscaCampoTabela("TipMovOrcPALMFE", "PARAMETROS", " AND CODEMP = " + u.CodEmp + ""));
                                    }
                                    else
                                    {
                                        iCodTipMov = Convert.ToInt16(Funcoes.BuscaCampoTabela("TipMovOrcPALM", "PARAMETROS", " AND CODEMP = " + u.CodEmp + ""));
                                    }
                                    
                                }

                                /*Se vier zerado, seta o padrao do sigma*/
                                if (iCodPrcTab == 0 && iCodPrzTab == 0)
                                {
                                    iCodPrcTab = (Int16)pr.CodTipPrc;
                                    iCodPrzTab = (Int16)pr.CodTipPrz;
                                }
                            }

                            if (string.IsNullOrEmpty(sPedidoAtual) || sPedidoAtual != arrDados[0])
                            {
                                if (string.IsNullOrEmpty(sPedidoAtual) == false)
                                {
                                    GravaPedido(iCliente, iForma, iPrazo, lNumPedWeb, lNumPed, sObs, iCodPrcTab, iCodPrzTab, iUnidadeVenda, (int)iCodTipMov, sPedidoAtual);
                                    lNumPedWeb = ClassePedido.ProximoPedidoWeb(u).ToString();
                                    lNumPed = ClassePedido.ProximoPedido(u).ToString();
                                    MensagemItemNaoAtendido = "";
                                    iTotalPedidos++;
                                }

                                sPedidoAtual = arrDados[0];
                                iCliente = Convert.ToInt32(arrDados[3]);
                                iForma = Convert.ToInt16(arrDados[1]);
                                iPrazo = Convert.ToInt16(arrDados[2]);

                                iCodPrcTab = Convert.ToInt16(arrDados[9]);
                                iCodPrzTab = Convert.ToInt16(arrDados[10]);
                                iUnidadeVenda = Convert.ToInt16(arrDados[11] == "Maior Unidade" ? 2 : 0);

                                if (arrDados.Length > 12)
                                {
                                    iCodTipMov = Convert.ToInt16(arrDados[12]);
                                }


                                if (arrDados.Length > 13) //Somente se vier com ***
                                {

                                    if (Funcoes.BuscaCampoTabela("EstCli", "CLIENTE", " AND CODCLI = " + iCliente + "") !=
                                        Funcoes.BuscaCampoTabela("Estado", "EMPRESA", " AND CODEMP = " + u.CodEmp + ""))
                                    {
                                        iCodTipMov = Convert.ToInt16(Funcoes.BuscaCampoTabela("TipMovOrcPALMFE", "PARAMETROS", " AND CODEMP = " + u.CodEmp + ""));
                                    }
                                    else
                                    {
                                        iCodTipMov = Convert.ToInt16(Funcoes.BuscaCampoTabela("TipMovOrcPALM", "PARAMETROS", " AND CODEMP = " + u.CodEmp + ""));
                                    }

                                }

                                /*Se vier zerado, seta o padrao do sigma*/
                                if (iCodPrcTab == 0 && iCodPrzTab == 0)
                                {
                                    iCodPrcTab = (Int16)pr.CodTipPrc;
                                    iCodPrzTab = (Int16)pr.CodTipPrz;
                                }

                                sObs = "";
                                if (arrDados.Length > 8)
                                {
                                    sObs = arrDados[8];
                                }

                                AdicionarItem(arrDados[4], arrDados[5].Replace(".", ","), arrDados[6], lNumPedWeb, lNumPed, arrDados[7], iCodPrcTab, iCodPrzTab, iUnidadeVenda);

                            }
                            else
                            {
                                AdicionarItem(arrDados[4], arrDados[5].Replace(".", ","), arrDados[6], lNumPedWeb, lNumPed, arrDados[7], iCodPrcTab, iCodPrzTab, iUnidadeVenda);
                            }
                            linhas++; //Incrementa Linha
                        }
                        else
                        {
                            iPedidosJaGravados++;
                        }

                    }

                    if (linhas > 0)
                    {
                        //Fim de Arquivo - Grava último pedido da variavel
                        GravaPedido(iCliente, iForma, iPrazo, lNumPedWeb, lNumPed, sObs, iCodPrcTab, iCodPrzTab, iUnidadeVenda, (int)iCodTipMov, sPedidoAtual);
                        iTotalPedidos++;

                        if (!String.IsNullOrEmpty(MensagemItemNaoAtendido))
                        {
                            lbErros.Visible = true;
                            //ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + MensagemItemNaoAtendido + "');", true);
                            lbErros.Text = MensagemItemNaoAtendido;
                        }
                        else
                        {
                            lbErros.Visible = true;                            
                            lbErros.Text = iTotalPedidos + " Pedido(s) Gravado(s) com Sucesso. Recomenda-se fechar o navegador caso a importação tenha sido feita automaticamento pelo processo OFFLINE / FTP.";                            
                            //ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + iTotalPedidos + " Pedido(s) Gravado(s) com Sucesso');", true);
                        }

                        
                    }
                    else if (iPedidosJaGravados>0)
                    {
                        lbErros.Visible = true;
                        lbErros.Text = iTotalPedidos + " Pedido(s) Gravado(s) com Sucesso. Recomenda-se fechar o navegador caso a importação tenha sido feita automaticamento pelo processo OFFLINE / FTP.";                            
                        Response.Write("<p class='texto_erro'>Foram encontrados " + iPedidosJaGravados + " pedidos já gravados em uma tentativa anterior e " + linhas + " pedidos novos gravados do arquivo : " + Label1.Text + "</p>");                        
                    }

                    texto.Close();

                    if (bLote == false)
                    {
                        buAba2_Click(null, null);
                        buDownload.Visible = false;
                        //lbTexto.Visible = false;
                    }
                }
            }
            else
            {
                lbErros.Visible = true;
                lbErros.Text = "Arquivo " + Label1.Text + " com extensão inválida. Selecionar apenas arquivos .TXT";
                //ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Arquivo " + Label1.Text + " com extensão inválida. Selecionar apenas arquivos .TXT');", true);
            }
        }
        catch (Exception e)
        {
            lbErros.Text = e.Message.ToString();
            lbErros.Visible = true;
        }

    }

    protected void ScriptManager1_Init(object sender, EventArgs e)
    {

    }

    protected void buAba1_Click(object sender, EventArgs e)
    {
        //pnImportacao.Visible    = true;
        //buAba1.CssClass         = "aba_imp_1";
        //buAba2.CssClass         = "aba_imp_4";
        //pnLog.Visible           = false;
    }

    protected void ItensNaoAtendidos_Click(object sender, EventArgs e)
    {
      
    }

    protected void buAba2_Click(object sender, EventArgs e)
    {
        //pnLog.Visible           = true;
        //pnImportacao.Visible    = false;
        //buAba1.CssClass         = "aba_imp_2";
        //buAba2.CssClass         = "aba_imp_3";
        tbDataEmissaoInicial.Focus();
        lbTotal.Text            = "0,00";

        StringBuilder sSql = new StringBuilder();

        sSql.Length = 0;
        sSql.Append(" SELECT ISNULL(PEDIDO.NumPed,'') AS NUMPED, RAZSOC AS CODCLI, DATA, VALOR, MSG, STATUS, L.NUMPED AS NUMPED_LOG ");
        sSql.Append(" FROM LOG_IMPORTACAO_WEB L INNER JOIN CLIENTE C ON L.CODCLI = C.CODCLI ");
        sSql.Append(" LEFT JOIN PEDIDO ON PEDIDO.NumPed = L.NUMPED ");
        sSql.Append(" WHERE ");
        sSql.Append(" PEDIDO.CODEMP = " + u.CodEmp + " AND ");
        sSql.Append(" L.CODVEN = " + u.CodVend + " ");
        
        if (!String.IsNullOrEmpty(tbDataEmissaoInicial.Text))
        {
            sSql.Append(" AND L.DATA >= '" + Funcoes.RetornaDataQuery(tbDataEmissaoInicial.Text) + " 00:00:00'");
        }

        if (!String.IsNullOrEmpty(tbDataEmissaoFinal.Text))
        {
            sSql.Append(" AND L.DATA <= '" + Funcoes.RetornaDataQuery(tbDataEmissaoFinal.Text) + " 23:59:59'");
        }

        sSql.Append(" ORDER BY L.DATA DESC ");
        //Pega total do pedido inserido
        //var rs = conn.Query("SELECT NUMPED  as 'N. PEDIDO', CODCLI  AS 'CLIENTE', DATA, MSG AS 'MENSAGEM', STATUS  FROM LOG_IMPORTACAO_WEB WHERE CODVEN = " + u.CodVend);
        var rs = conn.Query(sSql.ToString());

        gdLog.DataSource = rs;
        gdLog.DataBind();
        rs.Close();
        
    }

   
    protected void gdLog_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void gdLog_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            lbTotal.Text = (Convert.ToDecimal(lbTotal.Text) + Convert.ToDecimal(e.Row.Cells[3].Text)).ToString();
            
            
        }
        

    }

    protected void LinkButtonPesquisar_Click(object sender, EventArgs e)
    {
        buAba2_Click(null, null);
    }

    protected void buDownload_Click(object sender, EventArgs e)
    {
        buDownload.Enabled = false;

        try
        {
            ImportaArquivo(false);

            //cria diretorio do vendedor se nao existir
            System.IO.Directory.CreateDirectory(Server.MapPath(@"~\planilhas\importados\" + u.CodVend));

            //move o arquivo importado para a pasta importados
            System.IO.File.Copy(Server.MapPath(@"~\planilhas\" + Label2.Text), Server.MapPath(@"~\planilhas\importados\" + u.CodVend + "\\" + Label2.Text), true);

            //exclui o arquivo da pasta planilhas 
            System.IO.File.Delete(Server.MapPath(@"~\planilhas\" + Label2.Text));

            Label1.Text = "";
            Label2.Text = "";
            buImportar.Visible = true;
            FileUpload1.Visible = true;
            lbPasso1.Text = "Selecione o arquivo da pasta ENVIAR do seu Tablet";
            lbPasso2.Text = "Clique no botão Importar após conclusão do Passo 1";
            
        }
        catch (Exception exc)
        {
            lbErros.Text = exc.Message.ToString();
            lbErros.Visible = true;
        }

        buDownload.Enabled = true;
    }
}