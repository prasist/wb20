using System;

public partial class MasterPage : System.Web.UI.MasterPage
{    
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (Session["Usuario"] == null)
        {
            Response.Redirect("login.aspx?RedirectUrl=" + Request.FilePath);
        }
        else
        {
            LabelEmpresa.Text = "Empresa : " + Session["EmpresaCODEMP"] + " - " + Session["EmpresaNOMEFAN"].ToString();
            LabelUsuario.Text = Session["UsuarioNomUsu"].ToString();                        
        }
    }
    protected void LinkButtonSair_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Response.Redirect("login.aspx");
    }
}
