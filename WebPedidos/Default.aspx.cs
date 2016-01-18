﻿using System;
using System.IO;
using System.Web.UI;
using WebPedidos.WSClasses;

public partial class _Default : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        ClasseBanco conn = new ClasseBanco();


        if (Session["Usuario"] != null)
        {
            Control ctrl = (Control)Session["ctrl"];
            
            //Verifica ultima geracao do XML
            string sCaminhodoArquivo = System.Web.HttpContext.Current.Server.MapPath(@"~\xml\produtos.xml");
            FileInfo fi = new FileInfo(sCaminhodoArquivo);

            DateTime dHoje = DateTime.Now;
            DateTime dArquivo = fi.LastWriteTime;

            if (dArquivo.AddMinutes(2) < dHoje)
            {
                try
                {
                    ClassePedido.GeraXml((UsuarioResumido)Session["Usuario"], (ParametroResumido)Session["Parametros"]);
                    lbUltima.Text = "Última atualização XML : <strong>" + String.Format("{0:G}", dHoje) + "</strong>";
                }
                catch (Exception exc)
                {
                    //ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + exc.Message  + ".');", true)                    ;
                    Response.Write("<p class='texto_erro'>" + exc.Message + "</p>");
                }

                //if (Funcoes.isMobileBrowser())
                //{
                    lbUltima.Text = "Última atualização XML : <strong>" + String.Format("{0:G}", dHoje) + "</strong>";
                //}
            }
            else
            {
               lbUltima.Text = "Última atualização XML : <strong>" + String.Format("{0:G}", dArquivo) + "</strong>";
            }

            //var dados = conn.Query("SELECT COUNT(*) AS TOT, SUM(ISNULL(PESOBRUTO,0)) AS PESO FROM PEDIDO WHERE CODEMP = " + Session["EmpresaCODEMP"] + " AND CODVEN = " + Session["CodVend"] + " AND DTAEMI >= CONVERT(VARCHAR(10),GETDATE(),110) AND ORIGEM = 'W'");
            var dados = conn.Query("SELECT COUNT(*) AS TOT, SUM(ISNULL(PESOBRUTO,0)) AS PESO FROM PEDIDO WHERE CODEMP = " + Session["EmpresaCODEMP"] + " AND CODVEN = " + Session["CodVend"] + " AND DTAEMI >= '" + Funcoes.RetornaDataQuery(DateTime.Now.ToString())  + "' AND ORIGEM = 'W'");
            
            if (dados.Read())
            {
                lbPedidos.Text = dados["tot"].ToString();
                lbPeso.Text = dados["peso"].ToString();
            }
            dados.Close();

            //Se houver pedidos recebidos automaticamente do offline no FTP do cliente.
            GravaPedidosRecebidos();

        }        
    }

    private void GravaPedidosRecebidos()
    {
        ParametroResumido pr = (ParametroResumido)Session["Parametros"];
        UsuarioResumido u = (UsuarioResumido)Session["Usuario"];
        ClasseBanco conn = new ClasseBanco();
        ClasseFtp clsFtp = new ClasseFtp();

        string sPathDestino = Server.MapPath(@"~\planilhas\recebidos_offline");

        //Verifica se foi configurado endereco do FTP
        if (pr.HostFtp != null && pr.HostFtp!="")
        {
            //Verifica se tem arquivos na pasta do FTP e copia para o servidor
            clsFtp.VerificarArquivos(sPathDestino, pr.HostFtp, pr.PastaServidor, pr.FtpUsuario, pr.FtpSenha);

            //Se houver erro no FTP
            if (clsFtp.sErro != "" && clsFtp.sErro!=null)
            {
                Response.Write("<p class='texto_erro'>" + clsFtp.sErro + "</p>");
            }

            ////Verifica se tem arquivos recebidos do vendedor logado
            //var rsLeitura = conn.Query("SELECT 1 FROM LOG_ARQUIVOS_RECEBIDOS WHERE CODVEN =" + u.CodUsu);
            //if (rsLeitura.Read())
            //{
            //    Response.Redirect("importacao.aspx?flg=Ok"); //Redireciona enviando parametro para gravar arquivos recebidos do vendedor
            //}
            //rsLeitura.Close();
            //rsLeitura.Dispose();
        }

        //Verifica se tem arquivos recebidos do vendedor logado
        var rsLeitura = conn.Query("SELECT 1 FROM LOG_ARQUIVOS_RECEBIDOS WHERE CODVEN =" + u.CodUsu);
        if (rsLeitura.Read())
        {
            Response.Redirect("importacao.aspx?flg=Ok"); //Redireciona enviando parametro para gravar arquivos recebidos do vendedor
        }
        else
        {
            lbPedidosMsg.Text = "Não há pedidos pendentes!";
        }
        rsLeitura.Close();
        rsLeitura.Dispose();
    }

    protected void buExporta_Click(object sender, ImageClickEventArgs e)
    {
        ClassePedido.GeraXml((UsuarioResumido)Session["Usuario"], (ParametroResumido)Session["Parametros"]);
    }   

    protected void buImportar_Click(object sender, ImageClickEventArgs e)
    {
        
    }

    protected void buValidar_Click(object sender, EventArgs e)
    {
        //Se houver pedidos recebidos automaticamente do offline no FTP do cliente.
        GravaPedidosRecebidos();
    }
}