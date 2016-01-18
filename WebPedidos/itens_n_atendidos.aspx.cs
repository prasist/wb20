using System;
using WebPedidos.WSClasses;

public partial class Default2 : System.Web.UI.Page
{
    ClasseBanco conn = new ClasseBanco();

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!String.IsNullOrEmpty(Request.QueryString["id"]))
        {
            string sSql = "SELECT L.CODSERVMERC, S.DESSERVMERC, L.QTD, L.VALOR FROM LOG_ITENS_NAO_ATENDIDOS_WEB L INNER JOIN SERVMERC S ON L.CODSERVMERC = S.CODSERVMERC WHERE L.NUMPED = " + Request.QueryString["id"];
            var rs = conn.retornaQueryDataSet(sSql);
            gdItens.DataSource = rs;
            gdItens.DataBind();
        }
    }
    protected void buSair_Click(object sender, EventArgs e)
    {

    }
}