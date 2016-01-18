<%@ Page Language="C#" AutoEventWireup="true" CodeFile="itens_n_atendidos.aspx.cs" Inherits="Default2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />    

</head>
<body>
    
    <form id="form1" runat="server">
    <div class="texto_grid">
        <table class="bordas">
            <tr>
                <td>
                    <asp:GridView ID="gdItens" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False"
                        Width="730px" CssClass="texto_grid">
                        <RowStyle BackColor="#EFF3FB" />
                        <Columns>
                            <asp:BoundField DataField="CodServMerc" HeaderText="Código" ReadOnly="True" SortExpression="cod" Visible="true" />
                            <asp:BoundField DataField="DesServMerc" HeaderText="Descrição" SortExpression="descr" ReadOnly="True" />
                            <asp:BoundField DataField="Qtd" HeaderText="Qtd" SortExpression="qtd" ReadOnly="True" />                            
                            <asp:BoundField DataField="Valor" HeaderText="Preço" SortExpression="valor" ReadOnly="True" />                            
                        </Columns>
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <EditRowStyle BackColor="#2461BF" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                </td>
             </tr>
            <tr>
                <td class="texto_bold_gd_branco"><a class="link_grid" href="javascript:window.close();">Fechar</a></td>
            </tr>
        </table>        
        </div>
    </form>
    
</body>
</html>
