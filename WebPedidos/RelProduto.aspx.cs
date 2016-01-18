using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using WebPedidos.WSClasses;

public partial class RelProduto : System.Web.UI.Page
{

    UsuarioResumido u;
    ParametroResumido pr;
    Int16 CodPrcTab;
    Int16 CodPrzTab;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;

        if (!IsPostBack)
        {
            pr = (ParametroResumido)Session["Parametros"];
            u = (UsuarioResumido)Session["Usuario"];

            if (u == null)
            {
                Response.Redirect("Default.aspx");
            }

            ClasseBanco csConectar = new ClasseBanco();
            String strSql = " select DISTINCT G.CodGru, DesGru from GRUPO G INNER JOIN SERVMERC S ON G.CodGru = S.CodGru WHERE S.Ativo = 'S' ORDER BY DesGru ";
            var r = csConectar.retornaQueryDataSet(strSql);

            DropDownListGrupo.DataSource = r;
            DropDownListGrupo.DataTextField = "DesGru";
            DropDownListGrupo.DataValueField = "CodGru";
            DropDownListGrupo.DataBind();
            DropDownListGrupo.Items.Insert(0, "Todos");
            DropDownListGrupo.Items.Insert(0, "");

            CodPrcTab = Convert.ToInt16(pr.CodTipPrc);
            CodPrzTab = Convert.ToInt16(pr.CodTipPrz);

            if (cbTabelas.SelectedIndex != -1)
            {
                CodPrcTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').First());
                CodPrzTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').Last());
            }

            BuscaTabelaPadrao();

        }
    }
    protected void LinkButtonPesquisar_Click(object sender, EventArgs e)
    {
        pr = (ParametroResumido)Session["Parametros"];   
        u = (UsuarioResumido)Session["Usuario"];

        if (cbTabelas.SelectedIndex != -1)
        {
            CodPrcTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').First());
            CodPrzTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').Last());
        }
        
        if (TextBoxCodServMerc.Text.Length > 0)
        {
            int codProduto = Convert.ToInt32(TextBoxCodServMerc.Text);
            //List<ProdutoResumido> prods = ClasseProdutos.Produto(u, codProduto, pr, Convert.ToInt16(pr.CodTipPrc), Convert.ToInt16(pr.CodTipPrz));
            List<ProdutoResumido> prods = ClasseProdutos.buscaProdutosPorCodigo(u, pr, codProduto, CodPrcTab, CodPrzTab);
            if (prods != null)
            {
                
                /*
                LabelCodigo.Text = prods[0].Codigo.ToString();
                LabelCodigoBarras.Text = "";
                LabelCodigoBarrasNumero.Text = "";
                LabelDescricao.Text = prods[0].Nome.Trim();
                LabelSaldo.Text = prods[0].Saldo.ToString();
                //LabelPreco.Text = String.Format("R$ {0:" + Funcoes.Decimais(pr) + "}", prods[0].Preco);
                TextBoxCodServMerc.Text = "";
                TextBoxDesServMerc.Text = "";
                DropDownListGrupo.SelectedIndex = 0;
                PanelUnico.Visible = true;
                PanelVarios.Visible = false;
                 */

                GridViewProdutos.DataSource = prods;

                BoundField bf;

                // formata o campo preço
                bf = (BoundField)GridViewProdutos.Columns[2]; // Preço base
                bf.DataFormatString = "{0:" + Funcoes.Decimais(pr) + "}";

                GridViewProdutos.DataBind();
                TextBoxCodServMerc.Text = "";
                DropDownListGrupo.SelectedIndex = 0;
                PanelVarios.Visible = true;
                PanelUnico.Visible = false;

            }
            else
            {
                PanelUnico.Visible = false;
                PanelVarios.Visible = false;
                TextBoxCodServMerc.Text = "";
                TextBoxDesServMerc.Text = "";
                DropDownListGrupo.SelectedIndex = 0;
                if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('Não foi encontrado produto com o número informado ou está inativo.')</script>");
                }
            }
        }
        else if (TextBoxDesServMerc.Text.Length > 0)
        {
            pr = (ParametroResumido)Session["Parametros"];
            //List<ProdutoResumido> prods = ClasseProdutos.buscaProdutosPorNome(u, pr, TextBoxDesServMerc.Text);
            var prods = ClasseProdutos.buscaProdutosPorNome(u, pr, TextBoxDesServMerc.Text, CodPrcTab, CodPrzTab);

            if (prods.Tables[0].Rows.Count>0)
            {
                GridViewProdutos.DataSource = prods;

                BoundField bf;

                // formata o campo preço
                bf = (BoundField)GridViewProdutos.Columns[2]; // Preço base
                bf.DataFormatString = "{0:" + Funcoes.Decimais(pr) + "}";

                GridViewProdutos.DataBind();
                TextBoxCodServMerc.Text = "";
                DropDownListGrupo.SelectedIndex = 0;
                PanelVarios.Visible = true;
                PanelUnico.Visible = false;
            }
            else
            {
                PanelUnico.Visible = false;
                PanelVarios.Visible = false;
                TextBoxCodServMerc.Text = "";
                DropDownListGrupo.SelectedIndex = 0;
                if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('Não foi encontrado produto com o texto informado')</script>");
                }
            }
        }
        else
        {
            PanelUnico.Visible = false;
            PanelVarios.Visible = false;
            if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('Digite um parâmetro para a pesquisa')</script>");
            }
        }
    }
    protected void DropDownListCategoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        pr = (ParametroResumido)Session["Parametros"];
        u = (UsuarioResumido)Session["Usuario"];

        //GridViewProdutos.Dispose();

        if (cbTabelas.SelectedIndex != -1)
        {
            CodPrcTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').First());
            CodPrzTab = Convert.ToInt16(cbTabelas.SelectedValue.Split('|').Last());
        }

        if (DropDownListGrupo.SelectedIndex > 1)
        {

            try
            {
                GridViewProdutos.DataSource = ClasseProdutos.buscarProdutosPorGrupo(u, pr, Convert.ToInt16(DropDownListGrupo.SelectedValue), CodPrcTab, CodPrzTab);

                BoundField bf;

                // formata o campo preço
                bf = (BoundField)GridViewProdutos.Columns[2]; // Preço base
                bf.DataFormatString = "{0:" + Funcoes.Decimais(pr) + "}";

                GridViewProdutos.DataBind();

            } catch (Exception exc){
                ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('" + exc.Message + "')</script>");
            }
            PanelUnico.Visible = false;
            PanelVarios.Visible = true;

        }
        else
        {
            if (DropDownListGrupo.SelectedIndex == 1)
            {
                GridViewProdutos.DataSource = ClasseProdutos.ListarProdutos((UsuarioResumido)Session["Usuario"], CodPrcTab, CodPrzTab);

                BoundField bf;

                // formata o campo preço
                bf = (BoundField)GridViewProdutos.Columns[2]; // Preço base
                bf.DataFormatString = "R$ {0:" + Funcoes.Decimais(pr) + "}";

                GridViewProdutos.DataBind();
                PanelUnico.Visible = false;
                PanelVarios.Visible = true;
            }
            else
            {
                PanelUnico.Visible = false;
                PanelVarios.Visible = false;
            }
        }
    }
    protected void GridViewProdutos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        ClasseBanco conn = new ClasseBanco();
        StringBuilder sObs = new StringBuilder();
        string strSql = "";

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //BUSCAR PRECO DO PRODUTO NA TABELA PADRAO
            strSql = "SELECT OBS_OBS FROM OBSERVACOES WHERE ISNULL(OBS_OBS,'') <> '' AND CODSERVMERC_OBS=" + e.Row.Cells[0].Text;

            var r = conn.Query(strSql);
            sObs.Length = 0;
            int iLinhas = 0;
            while (r.Read())
            {
                iLinhas++;
                if (iLinhas <= 3)
                {
                    sObs.Append("</br>" + r[0].ToString());
                }
            }
            r.Close();
    
            e.Row.Cells[1].Text = e.Row.Cells[1].Text + sObs.ToString();

            if (ckImagem.Checked)
            {
                e.Row.Cells[4].Visible = true;
            }
            else
            {
                e.Row.Cells[4].Visible = false;
                e.Row.Cells[4].Height = 0;
            }
        }
              
    }

    private void BuscaTabelaPadrao()
    {
        
        ClasseVendedorAtributos[] vend = ClasseVendedor.BuscaDados(Convert.ToInt16(Session["CodVend"].ToString()), null, null).ToArray();
        cbTabelas.Items.Clear();

        foreach (ClasseVendedorAtributos vn in vend)
        {
            cbTabelas.Items.Insert(0, new ListItem(vn.IdTabela.ToString() + " - " + vn.DesTipPrc + " ==> " + vn.CodTipPrz.ToString() + " - " + vn.DesTipPrz, vn.IdTabela.ToString() + "|" + vn.CodTipPrz));
        }

        if (cbTabelas.Items.Count <= 0)
        {
            if (pr.CodTipPrc.Equals(0)) /*Se nao tiver tabela de preco padronizada*/
            {
                string sTabela = Funcoes.BuscaCampoTabela("DESTIPPRC", "TIPO_PRECO", " AND CODTIPPRC = " + pr.CodTipPrc + "");
                string sPrazo  = Funcoes.BuscaCampoTabela("DESTIPPRZ", "TIPOPRAZO", " AND CODTIPPRZ = " + pr.CodTipPrz + "");

                cbTabelas.Items.Clear();
                cbTabelas.Items.Insert(0, new ListItem(sTabela.Trim() + " ==> " + sPrazo.Trim(), pr.CodTipPrc + "|" + pr.CodTipPrz));
            }
        }

        //cbTabelas.Items.Insert(0, new ListItem("(Selecione uma Tabela Válida)", "-1"));        

    }

    protected void cbTabelas_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownListCategoria_SelectedIndexChanged(null, null);
    }

}
