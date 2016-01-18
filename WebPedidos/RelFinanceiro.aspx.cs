﻿using System;
using System.Web.UI.WebControls;
using WebPedidos.WSClasses;

public partial class RelFinanceiro : System.Web.UI.Page
{
    UsuarioResumido u;
    ParametroResumido pr;
    ClasseCliente csCliente = new ClasseCliente();

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
            else
            {
                dplClientes.DataSource = ClasseCliente.ListarClientes(u,pr);
                dplClientes.DataValueField = "Codigo";
                dplClientes.DataTextField = "Nome";
                dplClientes.DataBind();
                dplClientes.Items.Insert(0, new ListItem("", "-1"));

            }

        }

        

    }
    protected void LinkButtonPesquisar_Click(object sender, EventArgs e)
    {
       
    }
    protected void dplClientes_SelectedIndexChanged(object sender, EventArgs e)
    {

        DataClassesDataContext dcdc = new DataClassesDataContext();
        pr = (ParametroResumido)Session["Parametros"];
        ClasseBanco csBanco = new ClasseBanco();

        csBanco.AbrirBanco();

        csCliente.DadosCliente(Convert.ToInt32(dplClientes.SelectedValue));
        
        //Telefone e Email - 50732
        lbDados.Text = csCliente.EndCli + " - N.: " + csCliente.Numero + " - " + csCliente.CidCli + " - " + csCliente.EstCli + " - CNPJ : " + csCliente.CGC_CPF + " - Telefone : " + csCliente.TelCli + " - Email : " + csCliente.Email;


        LB_Total.Text = "";
        lbLimite.Text = "";
        lbDebitos.Text = "";
        lbSaldo.Text = "";

        var strSql = " SELECT ISNULL(SUM(VlrDoc),0) AS Valor " +
                         " FROM TITRECEB " +
                         " WHERE CODCLI = " + Convert.ToInt32(dplClientes.SelectedValue) + " AND TIPO = 'R' AND STATUS <> 'B' ";
        
        decimal saldo = 0;
        
        var rSaldo = csBanco.Query(strSql);
        
        if (rSaldo.Read())
        {
            saldo = rSaldo[0].ToString() == null ? 0 : Convert.ToDecimal(rSaldo[0].ToString());
        }
        rSaldo.Close();

        strSql = " SELECT NumDoc, NumPar, Serie, Status, VlrDoc, Saldo = (VlrDoc - VlrPago + AcrFin - (Desc1 + Desc2)), DtaVen, DtaEmi, QtdPar, Obs, Datediff(d,DtaVen,GETDATE()) AS atraso " +
                         " FROM TITRECEB " +
                         " WHERE CODCLI = " + Convert.ToInt32(dplClientes.SelectedValue) + " AND TIPO = 'R' AND STATUS <> 'B' ORDER BY DtaVen";

        var dados = csBanco.retornaQueryDataSet(strSql);

        if (dados.Tables[0].Rows.Count > 0)
        {
            //nothing...
        }
        else
        {
            PanelVarios.Visible = false ;
            PanelUnico.Visible = true;
            lbSaldo.Text = "Nenhum Registro Encontrado";
        }

        PanelVarios.Visible = true;
        PanelUnico.Visible = false;
        LB_Total.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", saldo);

        var r = csBanco.Query("SELECT LimCred, VlrDeb FROM FINANCLI WHERE CodCli = " + Convert.ToInt32(dplClientes.SelectedValue));

        if (r.Read())
        {

            var cLimite = r[0].ToString() == "" ? "0" : r[0].ToString();
            var cDebitos = r[1].ToString() == "" ? "0" : r[1].ToString(); ;
            var cSaldo = Convert.ToString((Convert.ToDecimal(cLimite) - Convert.ToDecimal(cDebitos)));

            
            lbLimite.Text  = String.Format("{0:" + Funcoes.Decimais(pr) + "}", cLimite);
            lbDebitos.Text = String.Format("{0:" + Funcoes.Decimais(pr) + "}", cDebitos);
            lbSaldo.Text   = String.Format("{0:" + Funcoes.Decimais(pr) + "}", cSaldo);
        }
        r.Close();

        GridViewTitulos.DataSource = dados;        
        GridViewTitulos.DataBind();
        
    }

    protected void buPesquisar_Click(object sender, EventArgs e)
    {
        UsuarioResumido u = (UsuarioResumido)Session["Usuario"];
        ParametroResumido pr = (ParametroResumido)Session["Parametros"];
        dplClientes.DataSource = csCliente.buscaCliente(tbCliente.Text, u,pr);
        dplClientes.DataValueField = "Codigo";
        dplClientes.DataTextField = "Nome";
        dplClientes.DataBind();
        dplClientes.Items.Insert(0, new ListItem("(Selecione um Cliente)", "-1"));
    }

    protected void tbCliente_TextChanged(object sender, EventArgs e)
    {

    }

    protected void GridViewTitulos_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (Convert.ToInt32(e.Row.Cells[10].Text)>0)
            {
                e.Row.Cells[0].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[4].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[6].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[7].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[8].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[9].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[10].ForeColor = System.Drawing.Color.Red;
            }
            else if (Convert.ToInt32(e.Row.Cells[10].Text) < 0)
            {
                e.Row.Cells[10].Text = "";
            }
        }
              

    }
    protected void buExportar_Click(object sender, EventArgs e)
    {
        //Response.ContentType = "application/pdf";
        //Response.AddHeader("content-disposition", "attachment;filename=Panel.pdf");
        //Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //StringWriter sw = new StringWriter();
        //HtmlTextWriter hw = new HtmlTextWriter(sw);
        //pnlPerson.RenderControl(hw);
        //StringReader sr = new StringReader(sw.ToString());
        //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
        //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //pdfDoc.Open();
        //htmlparser.Parse(sr);
        //pdfDoc.Close();
        //Response.Write(pdfDoc);
        //Response.End();
 
    }
}
