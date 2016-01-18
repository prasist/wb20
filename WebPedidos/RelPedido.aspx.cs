using System;
using System.Linq;
using System.Web.UI.WebControls;
using WebPedidos.WSClasses;

public partial class RelPedido : System.Web.UI.Page
{
    UsuarioResumido u;
    ParametroResumido pr;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;

        UsuarioResumido u = (UsuarioResumido)Session["Usuario"];

        if (u == null)
        {
            Response.Redirect("Default.aspx");
        }

        if (!String.IsNullOrEmpty(Request.QueryString["id"]))
        {
            TextBoxNumeroPedido.Text = Request.QueryString["id"];
            LinkButtonPesquisar_Click(null, null);
        }

    }
    protected void LinkButtonPesquisar_Click(object sender, EventArgs e)
    {
        pr = (ParametroResumido)Session["Parametros"];
        u = (UsuarioResumido)Session["Usuario"];

        lbMsg.Text = "";

        PEDIDO p = ClassePedido.Pedido(Convert.ToInt32(TextBoxNumeroPedido.Text), Convert.ToInt32(Session["EmpresaCODEMP"]), Convert.ToInt32(Session["CodVend"]));
        
        if (p != null)
        {
            LabelCliente.Text = ClasseCliente.Cliente(Convert.ToInt32(p.CodCli)) == null ? "Cliente n&atilde;o encontrado" : ClasseCliente.Cliente(Convert.ToInt32(p.CodCli)).RazSoc;            
            LabelCondPagto.Text = ClasseFormaPagto.FormaPagto(Convert.ToInt32(p.CodFrmPgt)) == null ? "Forma de pagamento n&atilde;o encontrada" : ClasseFormaPagto.FormaPagto(Convert.ToInt32(p.CodFrmPgt)).DesFrmPgt;
            LabelNumeroPedido.Text = p.NumPed.ToString();
            LabelObservacao.Text = p.Obs;
            LB_SubTotal.Text = String.Format("R$ {0:" + Funcoes.Decimais(pr) + "}", p.VlrSubTot);
            lbPesoTotal.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", p.PesoBruto);
            LabelTotalPedido.Text = String.Format("R$ {0:" + Funcoes.Decimais(pr) + "}", p.Vlrtot);
            LB_DescPed.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", p.PercDes);
            
            if (ClasseUsuario.Usuario(Convert.ToInt32(p.CodEmp), Convert.ToInt32(u.CodUsu)).Count > 0)
            {
                LabelVendedor.Text = ClasseUsuario.Usuario(Convert.ToInt32(p.CodEmp), Convert.ToInt32(u.CodUsu)).First().NomUsu;
            }
            else
            {
                LabelVendedor.Text = "Vendedor n&atilde;o encontrado";
            }
            GridViewProdutos.DataSource = ClassePedido.ItensPedido(p, u, pr, Convert.ToInt16(pr.CodTipPrc), Convert.ToInt16(pr.CodTipPrz));
            GridViewProdutos.DataBind();
            PanelUnico.Visible = true;
        }
        else
        {
            PanelUnico.Visible = false;
            if (!ClientScript.IsClientScriptBlockRegistered("respostaScript"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('Não foi encontrado pedido com o número informado ou não pertence a esse vendedor')</script>");
            }
        }
    }
    protected void GridViewProdutos_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        /*52033*/
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[4].Text.Equals("0"))
            {
                e.Row.Cells[4].ForeColor = System.Drawing.Color.Red;
                lbMsg.Text = "Aviso, itens faturados parcialmente";                
            }
        }        
    }
}
