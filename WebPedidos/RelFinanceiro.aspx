<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RelFinanceiro.aspx.cs" Inherits="RelFinanceiro" %>

<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <fieldset>

        <div class="panel panel-default">
            <div class="panel-heading">
                <h3 class="panel-title"><strong>Cliente</strong></h3>
            </div>
            <div class="panel-body">

                <div class="form-group">
                    <asp:TextBox ID="tbCliente" runat="server" OnTextChanged="tbCliente_TextChanged" placeholder="(Digite o Código ou Razão Social ou Nome Fantasia do Cliente)" CssClass="form-control"></asp:TextBox>
                    <asp:Button CssClass="btn btn-default" ID="buPesquisar" runat="server" Text="Pesquisar..." OnClick="buPesquisar_Click" />
                    <asp:DropDownList ID="dplClientes" CssClass="form-control" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dplClientes_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:Label ID="lbDados" runat="server" CssClass="text-primary"></asp:Label>
                </div>

            </div>
        </div>

        <div class="form-group">
            <asp:Panel ID="PanelUnico" runat="server" Visible="false">
            </asp:Panel>
            <asp:Panel ID="PanelVarios" runat="server">
                <asp:GridView ID="GridViewTitulos" runat="server" AutoGenerateColumns="False" CssClass="table-condensed">
                    <RowStyle />
                    <Columns>
                        <asp:BoundField DataField="NumDoc" HeaderText="Num. Doc." ReadOnly="True" SortExpression="Num. Doc." Visible="true" />
                        <asp:BoundField DataField="NumPar" HeaderText="Parcela" SortExpression="Parcela" ReadOnly="True" />
                        <asp:BoundField DataField="Serie" HeaderText="Série" SortExpression="Serie" ReadOnly="True" />
                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" ReadOnly="True" />
                        <asp:BoundField DataField="VlrDoc" HeaderText="Valor" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" SortExpression="Valor" ReadOnly="True" DataFormatString="{0:0.00}" />
                        <asp:BoundField DataField="Saldo" HeaderText="Saldo" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" SortExpression="Saldo" ReadOnly="True" DataFormatString="{0:0.00}" />
                        <asp:BoundField DataField="DtaVen" HeaderText="Vencimento" SortExpression="Vencimento" ReadOnly="True" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="DtaEmi" HeaderText="Emissão" SortExpression="Emissao" ReadOnly="True" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="QtdPar" HeaderText="Qtd. Parcelas" SortExpression="Qtd. Parcelas" ReadOnly="True" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="Obs" HeaderText="Observação" SortExpression="Obs" ReadOnly="True" />
                        <asp:BoundField DataField="atraso" HeaderText="Dias Atraso" SortExpression="atraso" ReadOnly="True" />
                    </Columns>
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#2461BF" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>

                <div class="form-group">   
                    <asp:Button ID="buExportar" CssClass="btn btn-primary" Text="Exportar PDF" runat="server" OnClick="buExportar_Click" Visible="False" />                    
                </div>

                <div class="form-inline">
                    <label for="LB_Total" class="control-label">Total</label>
                    <strong>
                        <asp:Label ID="LB_Total" runat="server" Text="" Enabled="false" CssClass="form-control"></asp:Label></strong>
                    <label for="lbLimite" class="control-label">Limite Crédito</label>
                    <strong>
                        <asp:Label ID="lbLimite" runat="server" Text="" CssClass="form-control" Enabled="false"></asp:Label></strong>
                    <label for="lbDebitos" class="control-label">Débitos</label>
                    <strong>
                        <asp:Label ID="lbDebitos" runat="server" Text="" CssClass="form-control" Enabled="false"></asp:Label></strong>
                    <label for="lbDebitos" class="control-label">Saldo</label>
                    <strong>
                        <asp:Label ID="lbSaldo" runat="server" Text="" CssClass="form-control" Enabled="false"></asp:Label></strong>
                </div>

            </asp:Panel>

        </div>

        <div class="panel panel-default">
            <div class="panel-heading">
                <h3 class="panel-title"><strong>Legendas Status</strong></h3>
            </div>
            <div class="panel-body">

                <div class="form-inline">
                    <span class="style3">A</span><span> - Aberto,</span><span class="style3">B</span><span> - Baixado, </span>
                    <span class="style3">P</span><span> - Parcial, </span>
                    <span class="style3">Z</span><span> - Acertado</span>
                </div>
            </div>
        </div>


    </fieldset>

</asp:Content>

