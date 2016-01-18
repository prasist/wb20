using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebPedidos.WSClasses;

public partial class login : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        EmpresaResumido[] empresas = ClasseEmpresa.ListarEmpresas().ToArray();
        TBEmpresa.Focus();

        //Usado para logar automaticamente quando feito o envio do offline para a web.
        if (Session["integracao"] != null)
        {
            string[] sCampos = Session["integracao"].ToString().Split('|');

            TBEmpresa.Text  = sCampos[0];
            TBUsuario.Text  = sCampos[1];
            Password1.Text = sCampos[2];
            BU_Ok_Click1(null, null);            
        }
    }

    
    protected void BU_Ok_Click1(object sender, EventArgs e)
    {
        UsuarioResumido u = ClasseLogin.Login(Convert.ToInt32(TBEmpresa.Text), Convert.ToInt32(TBUsuario.Text), Password1.Text);
        if (u != null)
        {
            Session["Usuario"] = u;
            Session["Parametros"] = ClasseParametro.GetParametro(u.CodEmp)[0];
            Session["UsuarioNomUsu"] = u.NomUsu.Trim();
            Session["UsuarioCodUsu"] = u.CodUsu;
            Session["CodVend"] = u.CodVend;
            Session["NomVend"] = u.NomVend;
            EmpresaResumido[] er = ClasseEmpresa.GetEmpresa(u, Convert.ToInt32(TBEmpresa.Text)).ToArray();
            Session["Empresa"] = er[0].NOME;
            Session["EmpresaNOME"] = er[0].NOME;
            Session["EmpresaNOMEFAN"] = er[0].NOMEFAN;
            Session["EmpresaCGC"] = er[0].CGC;
            Session["EmpresaCODEMP"] = er[0].CODEMP;
            Session["EmpresaCODIGOPRASIST"] = er[0].CODIGOPRASIST;
            if (Request.QueryString["RedirectUrl"] == null)
            {
                Response.Redirect("Default.aspx");
            }
            else
            {
                Response.Redirect(Request.QueryString["RedirectUrl"]);
            }
        }
        else
        {
            Session["Usuario"] = null;
            TBEmpresa.Text = "";
            TBUsuario.Text = "";

            if (!Page.ClientScript.IsClientScriptBlockRegistered("respostaScript"))
            {
                String Retorno = String.Empty;

                Retorno += "Atenção! Ocorreu um erro de autenticação.";
                Retorno += "\\n";
                Retorno += "Possíveis causas do erro:";
                Retorno += "\\n\\n";
                Retorno += "- O Usuário, a senha ou a empresa estão incorretos.";
                Retorno += "\\n";
                Retorno += "- O Usuário está ativo porém sem um vendedor associado.";
                Retorno += "\\n";
                Retorno += "- O Usuário está inativo.";
                Retorno += "\\n\\n";
                Retorno += "Por favor, verifique estas informações e, em seguida, tente novamente.";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "respostaScript", "<script language = 'javascript'>alert('" + Retorno + "')</script>");
            }

        }
    }

    protected void TBEmpresa_TextChanged(object sender, EventArgs e)
    {

    }
}
