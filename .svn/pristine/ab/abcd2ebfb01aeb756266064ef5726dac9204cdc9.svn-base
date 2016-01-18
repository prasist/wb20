using System;
using System.Web.UI.WebControls;
using WebPedidos.WSClasses;

public partial class RelPedidoCliente : System.Web.UI.Page
{    
    ClasseBanco conn = new ClasseBanco();
    ClasseCliente csCliente = new ClasseCliente();
    ParametroResumido pr;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        
        pr = (ParametroResumido)Session["Parametros"];        
        UsuarioResumido u = (UsuarioResumido)Session["Usuario"];
        if (!IsPostBack)
        {

            if (u == null)
            {
                Response.Redirect("Default.aspx");
            }

            conn.AbrirBanco();

            DropDownListCliente.DataSource = ClasseCliente.ListarClientes(u,pr);
            DropDownListCliente.DataValueField = "Codigo";
            DropDownListCliente.DataTextField = "Nome";
            DropDownListCliente.DataBind();
            DropDownListCliente.Items.Insert(0, new ListItem("", "-1"));
        }
    }

    protected void LinkButtonPesquisar_Click(object sender, EventArgs e)
    {
        conn.AbrirBanco();

        var sWhere = " WHERE 1 = 1 ";

        if (tbDataEmissaoInicial.Text != "" && tbDataEmissaoFinal.Text != "")
        {
        
            if (tbDataEmissaoInicial.Text != "" && tbDataEmissaoFinal.Text != "")
            {
                sWhere = sWhere + " AND CAST(P.DTAEMI AS DATETIME) >= '" + Funcoes.RetornaDataQuery(tbDataEmissaoInicial.Text) + "'" +
                         " AND CAST(P.DTAEMI AS DATETIME) <= '" + Funcoes.RetornaDataQuery(tbDataEmissaoFinal.Text) + "'";
            }
        }

        if (DropDownListCliente.Text != "-1") 
        {
            sWhere = sWhere + " AND P.CODCLI = " + DropDownListCliente.SelectedValue;
        }

        sWhere = sWhere + " AND P.CODVEN = " + Convert.ToInt32(Session["CodVend"]);
        sWhere = sWhere + " AND P.CODEMP = " + Convert.ToInt32(Session["EmpresaCODEMP"]);
        sWhere = sWhere + " AND P.SITPED <> 'CAN'"; 

        string strSql = " SELECT DISTINCT P.NUMPED, C.RAZSOC, P.DTAEMI, P.SITPED, P.VLRTOT, P.STATUSPEDIDO, " +
        " CASE WHEN P.STATUSCOMERCIAL <> 'ABE' THEN 'Bloqueado' ELSE '' END AS STATUSCOMERCIAL, T.NOME, T.FONE, P.PESOBRUTO " +
        " FROM PEDIDO P " +
        " INNER JOIN CLIENTE C ON P.CodCli = C.CodCli " +
        " LEFT JOIN NF N			ON N.NUMPED = P.NUMPED AND N.CODEMP = P.CODEMP AND N.CODCLI = P.CODCLI AND N.CODTIPMOV = P.CODTIPMOV " +
        " LEFT JOIN TRANSPORTADORA T ON T.CODIGO = N.CODTRA " +
        " " + sWhere + " AND ISNULL(N.SitNot,'') <> 'CAN' " +
        " ORDER BY P.NumPed, C.RazSoc, P.DtaEmi ";

        var dados = conn.retornaQueryDataSet(strSql);

        GridViewProdutos.DataSource = dados;
        GridViewProdutos.DataBind();

        //Total Consulta 
        strSql = " SELECT ISNULL(SUM(P.VLRTOT),0) AS TOTAL, SUM(P.PESOBRUTO) AS PESO  FROM PEDIDO P " + sWhere;
        var r = conn.Query(strSql);
        if (r.Read())
        {
            decimal valor = Convert.ToDecimal(r[0].ToString());
            LabelTotalPedido.Text   = Convert.ToString(String.Format("{0:" + Funcoes.Decimais(pr) + "}", valor));
            lbPesoTotal.Text        = Convert.ToString(String.Format("{0:" + Funcoes.Decimais(pr) + "}", r[1].ToString()));
        }
        r.Dispose();
        PanelUnico.Visible = true;
    }      

    protected void buPesquisar_Click(object sender, EventArgs e)
    {
        UsuarioResumido u = (UsuarioResumido)Session["Usuario"];
        DropDownListCliente.DataSource = csCliente.buscaCliente(tbCliente.Text, u, pr);
        DropDownListCliente.DataValueField = "Codigo";
        DropDownListCliente.DataTextField = "Nome";
        DropDownListCliente.DataBind();
        DropDownListCliente.Items.Insert(0, new ListItem("(Selecione um Cliente)", "-1"));
    }

    protected void GridViewProdutos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        /*52033*/
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[3].Text.Equals("EXE"))
            {

                var rs = conn.Query("SELECT CASE WHEN SUM(ISNULL(QTDFAT,0)) < SUM(QTD) THEN 'PAR' ELSE 'EXE' END AS SITPED FROM ITENSPED WHERE NUMPED= " + e.Row.Cells[10].Text + "");
                if (rs.Read())
                {
                    e.Row.Cells[3].Text = rs["SITPED"].ToString();
                }
                rs.Close();

            }
        }
        e.Row.Cells[10].Visible = false;

    }
}
