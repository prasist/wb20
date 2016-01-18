<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="instalar.aspx.cs" Inherits="instalar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div>
        <p>PROCEDIMENTOS</p>
        
        <p>1 ) Clicar na imagem para fazer o download. Após a conclusão do download executar o arquivo Instalar.apk</p>
        
                
        <br />
        <a href="Instalar.apk" class="texto_bold_gd"><asp:Image ID="Image1" runat="server" ImageUrl="~/imagens/download.jpg" /></a>
        <br />
        
    </div>
</asp:Content>


