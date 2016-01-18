using System;
using WebPedidos.WSClasses;


public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;

        if (Request.QueryString["1"] != null)
        {
            string spwd = new Krypto().EncryptString(Request.QueryString["3"].ToString(), AcaoKrypto.cnDECRYPT);
            Session["integracao"] = Request.QueryString["1"].ToString() + "|" + Request.QueryString["2"].ToString() + "|" + spwd;
        }
    }
}

