<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RelPedido.aspx.cs" Inherits="RelPedido" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <fieldset>

        <div class="form-group">
            <label class="control-label" for="LabelNumPedido">Número Pedido : </label>
            <asp:TextBox ID="TextBoxNumeroPedido" CssClass="form-control" runat="server" MaxLength="7"></asp:TextBox>

            <asp:Button ID="LinkButtonPesquisar" CssClass="btn btn-primary" runat="server" Text="Pesquisar" OnClick="LinkButtonPesquisar_Click" />


            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                ErrorMessage="Somente N&uacute;meros"
                ControlToValidate="TextBoxNumeroPedido" Display="Dynamic"
                ValidationExpression="\d*"></asp:RegularExpressionValidator>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                ControlToValidate="TextBoxNumeroPedido" Display="Dynamic"
                ErrorMessage="Campo Obrigat&amp;oacute;rio"></asp:RequiredFieldValidator>

        </div>


        <asp:Panel ID="PanelUnico" runat="server" Visible="false">

            <div class="form-inline">
                <label for="LabelNumeroPedido" class="control-label">Número</label>
                <strong><asp:Label ID="LabelNumeroPedido" CssClass="form-control" Enabled="false" runat="server" Text=""></asp:Label></strong>
                <label for="LabelVendedor" class="control-label">Vendedor</label>
                <strong><asp:Label ID="LabelVendedor" runat="server" CssClass="form-control" Enabled="false" Text=""></asp:Label></strong>
                <label for="LabelCliente" class="control-label">Cliente</label>
                <strong><asp:Label ID="LabelCliente" runat="server" CssClass="form-control" Enabled="false" Text=""></asp:Label></strong>
                <label for="LabelObservacao" class="control-label">Observação</label>
                <strong><asp:Label ID="LabelObservacao" runat="server" CssClass="form-control" Enabled="false" Text=""></asp:Label></strong>
                <label for="LabelCondPagto" class="control-label">Cond. Pagto.</label>
                <strong><asp:Label ID="LabelCondPagto" runat="server" CssClass="form-control" Enabled="false" Text=""></asp:Label></strong>
                <strong><asp:Label ID="lbMsg" runat="server" CssClass="form-control" Enabled="false" Text=""></asp:Label></strong>
            </div>

            
            <div class="form-group">
                <label for="GridViewProdutos" class="control-label">Produtos</label>
                <asp:GridView ID="GridViewProdutos" runat="server" DataKeyNames="NumPed,Item,CodServMerc" AutoGenerateColumns="False" CssClass="table-condensed" OnRowDataBound="GridViewProdutos_RowDataBound">
                    <RowStyle />


                    <Columns>
                        <asp:BoundField DataField="NumPed" HeaderText="NumPed"
                            ReadOnly="True" SortExpression="NumPed" Visible="false" />
                        <asp:BoundField DataField="Item" HeaderText="Item" SortExpression="Item"
                            ReadOnly="True" />
                        <asp:BoundField DataField="CodServMerc" HeaderText="C&oacute;digo"
                            SortExpression="CodServMerc" ReadOnly="True"
                            ItemStyle-HorizontalAlign="Right">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DescricaoProduto" HeaderText="Descri&ccedil;&atilde;o"
                            SortExpression="DescricaoProduto" ReadOnly="True" />
                        <asp:BoundField DataField="Qtd" HeaderText="Qtd"
                            SortExpression="Qtd" />

                        <asp:BoundField DataField="PrecoProduto" HeaderText="Preço Unit."
                            SortExpression="PrecoProduto" DataFormatString="{0:0.00}"
                            ItemStyle-HorizontalAlign="Right">

                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Desconto" HeaderText="% Desc." ItemStyle-HorizontalAlign="Right"
                            SortExpression="Desconto">

                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>

                        <asp:BoundField DataField="PrecoLiquido" HeaderText="Preço Unit. Liq." ItemStyle-HorizontalAlign="Right"
                            SortExpression="PrecoLiquido" DataFormatString="{0:0.00}">

                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Peso" HeaderText="Peso" ItemStyle-HorizontalAlign="Right"
                            SortExpression="Peso" DataFormatString="{0:0.00}">

                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Total" HeaderText="Total"
                            SortExpression="Total" DataFormatString="{0:0.00}"
                            ItemStyle-HorizontalAlign="Right">

                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>

                    </Columns>
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#2461BF" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>
            </div>

            <div class="form-inline">
                <label for="LB_SubTotal" class="control-label">SubTotal</label>
                <strong><asp:Label ID="LB_SubTotal" runat="server" CssClass="form-control" Enabled="false"></asp:Label></strong>
                <label for="lbPesoTotal" class="control-label">Peso Total</label>
                <strong><asp:Label ID="lbPesoTotal" runat="server" CssClass="form-control" Enabled="false" Text="Label"></asp:Label></strong>
                <label for="LB_DescPed" class="control-label">% Desc. Pedido</label>
                <strong><asp:Label ID="LB_DescPed" runat="server" CssClass="form-control" Enabled="false"></asp:Label></strong>
                <label for="LabelTotalPedido" class="control-label">Total do Pedido</label>
                <strong><asp:Label ID="LabelTotalPedido" runat="server" Text="" CssClass="form-control" Enabled="false"></asp:Label></strong>
            </div>

        </asp:Panel>
        <asp:Panel ID="PanelColecao" runat="server" Visible="false">
        </asp:Panel>
    </fieldset>


</asp:Content>
