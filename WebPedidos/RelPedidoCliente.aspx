<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RelPedidoCliente.aspx.cs" Inherits="RelPedidoCliente" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">

    <script type="text/javascript" src="admin.js"></script>

</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <fieldset>

        <div class="form-group">
            <label class="control-label" for="tbCliente">Cliente</label>
            <asp:TextBox ID="tbCliente" runat="server" placeholder="(Digite o Código ou Razão Social ou Nome Fantasia do Cliente)" CssClass="form-control"></asp:TextBox>
            <asp:Button CssClass="btn btn-default" ID="buPesquisar" runat="server" Text="Pesquisar..." OnClick="buPesquisar_Click" />
            <asp:DropDownList CssClass="form-control" ID="DropDownListCliente" runat="server" AutoPostBack="True"></asp:DropDownList>

        </div>

        <div class="form-inline">
            <label class="control-label" for="tbDataEmissaoInicial">Data Emissão</label>
            <asp:TextBox ID="tbDataEmissaoInicial" onkeyup="Mascara('DATA',this,event);" CssClass="form-control" runat="server" Width="100px" MaxLength="10">

            </asp:TextBox>&nbsp;Até&nbsp;<asp:TextBox ID="tbDataEmissaoFinal" onkeyup="Mascara('DATA',this,event);" CssClass="form-control" runat="server" Width="100px" MaxLength="10"></asp:TextBox>
            <asp:Button ID="LinkButtonPesquisar" CssClass="btn btn-primary" runat="server" Text="Consultar" OnClick="LinkButtonPesquisar_Click" />
        </div>

        <asp:Panel ID="PanelUnico" runat="server" Visible="false">
            <table class="table-condensed">
                <tr>
                    <td class="texto" colspan="2">
                        <asp:GridView ID="GridViewProdutos" runat="server" AutoGenerateColumns="False" CssClass="table-condensed" OnRowDataBound="GridViewProdutos_RowDataBound">
                            <RowStyle />
                            <Columns>
                                <asp:TemplateField HeaderText="N. Pedido">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# String.Format("RelPedido.aspx?id={0}", Eval("NUMPED")) %>' onclick="javascript:w= window.open(this.href,'Pedido','toolbar=0,resizable=0');return false;"><%# Eval("NUMPED")%></asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle />
                                    <ItemStyle Wrap="False" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="RAZSOC" HeaderText="Cliente" ReadOnly="True" SortExpression="Item" />
                                <asp:BoundField DataField="DTAEMI" DataFormatString="{0: dd/MM/yyyy}" HeaderText="Data Emissão" ItemStyle-HorizontalAlign="Right" ReadOnly="True" SortExpression="DTAEMI">
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SITPED" HeaderText="St. Pedido" ReadOnly="True" SortExpression="SITPED" />
                                <asp:BoundField DataField="PesoBruto" DataFormatString="{0:0.00}" HeaderText="Peso" ItemStyle-HorizontalAlign="Right" SortExpression="PesoBruto">
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="VLRTOT" DataFormatString="{0:0.00}" HeaderText="Valor Pedido" ItemStyle-HorizontalAlign="Right" SortExpression="VLRTOT">
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="StatusPedido" HeaderText="St. Financ." ReadOnly="True" SortExpression="StatusPedido" />
                                <asp:BoundField DataField="STATUSCOMERCIAL" HeaderText="St. Comercial" ReadOnly="True" SortExpression="STATUSCOMERCIAL" />
                                <asp:BoundField DataField="Nome" HeaderText="Transportadora" ReadOnly="True" SortExpression="Nome" />
                                <asp:BoundField DataField="Fone" HeaderText="Telefone" ReadOnly="True" SortExpression="Fone" />
                                <asp:BoundField DataField="NUMPED" ReadOnly="True" />
                            </Columns>
                            <FooterStyle BackColor="#507CD1" ForeColor="White" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#D1DDF1" ForeColor="#333333" />
                            <HeaderStyle BackColor="#507CD1" ForeColor="White" />
                            <EditRowStyle BackColor="#2461BF" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </td>
                </tr>

                <tr>
                    <td colspan="2">
                        <div class="form-inline">
                            <label for="lbPesoTotal" class="control-label">Peso Total</label>
                            <strong>
                                <asp:Label ID="lbPesoTotal" runat="server" CssClass="form-control" Enabled="false"></asp:Label></strong>
                        </div>

                    </td>

                </tr>

                <tr>
                    <td colspan="2">
                        <div class="form-inline">
                            <label for="LabelTotalPedido" class="control-label">Total Pedidos</label>
                            <strong>
                                <asp:Label ID="LabelTotalPedido" runat="server" CssClass="form-control"></asp:Label></strong>
                        </div>

                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr class="bordas">
                    <td class="texto" colspan="2">
                        <b>Legendas Pedido</b>&nbsp;: <span class="style5"><b>
                            <br />
                            ABE</b> - Aberto, <b>EXE</b> - Executado (Faturado), <b>BAI</b> - Baixado, <strong>PAR</strong> - Parcial</span></td>
                </tr>
                <tr class="bordas">
                    <td class="texto" colspan="2">
                        <b>Legendas Financeiro </b>:<br />
                        <span class="style5"><b>BD</b> - Bloqueado por Débitos, <b>BL</b> - Bloqueado 
                            por Limite Excedido, <b>BI</b> - Bloqueado por Inadimplencia, <b>DVT</b> - Houve 
                            Devolução Total, <b>
                                <br />
                                DVP</b> - Houve Devolução Parcial, <b>BPF</b> - Bloqueado por Politica de Frete,
                            <b>BBF</b> - Bloqueado por Cliente Bloqueado no Financeiro, <b>
                                <br />
                                BCM</b> - Bloqueado por Cancelamento, <b>BOF</b> - Bloqueado Ocorrência 
                            Financeira</span></td>
                </tr>
                <tr class="bordas">
                    <td class="texto" colspan="2">
                        <b>Legendas Comercial </b>:<br />
                        <span class="style5"><b>BCP</b> - Bloqueado por Limite da Condição de Pagto. 
                            Insuficiente, <b>NAT</b> - Pedido Não Atendido, <b>BVM</b> - Bloqueado por Valor 
                            Mínimo, <b>
                                <br />
                                BMM</b> - Bloqueado por Margem Mínima, <b>BFP</b> - Bloqueado por Falta de Saldo 
                            em um ou mais Produtos, <b>BPT</b> - Bloqueado Palm / Política, <b>
                                <br />
                                BOC</b> - Bloqueado Ocorrência Comercial,<b> BCF</b> - Bloqueado Conferência</span></td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="PanelColecao" runat="server" Visible="false">
        </asp:Panel>
    </fieldset>

</asp:Content>
