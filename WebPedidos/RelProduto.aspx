﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RelProduto.aspx.cs" Inherits="RelProduto" %>

<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <fieldset>

        <div class="form-inline">
            <label for="ckImagem" class="control-label">Exibir Imagem Produto</label>
            <asp:CheckBox ID="ckImagem" CssClass="checkbox" runat="server" />
        </div>

        <div class="form-group">
            <label for="cbTabelas" class="control-label">Tabela de Preço</label>
            <asp:DropDownList ID="cbTabelas" runat="server" AutoPostBack="True" CssClass="form-control" OnSelectedIndexChanged="cbTabelas_SelectedIndexChanged">
            </asp:DropDownList>
        </div>

        <div class="form-group">
            <label for="DropDownListGrupo" class="control-label">Grupo</label>
            <asp:DropDownList ID="DropDownListGrupo" CssClass="form-control" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="DropDownListCategoria_SelectedIndexChanged">
            </asp:DropDownList>
        </div>

        <div class="form-inline">
            <label for="TextBoxCodServMerc" class="control-label">Por Código</label>
            <asp:TextBox ID="TextBoxCodServMerc" CssClass="form-control" runat="server" MaxLength="9"></asp:TextBox>
            <label for="TextBoxDesServMerc" class="control-label">ou Descrição</label>
            <asp:TextBox ID="TextBoxDesServMerc" CssClass="form-control" runat="server"></asp:TextBox>
            <p>&nbsp;</p>
        </div>



        <div class="form-group">
            <asp:Button ID="LinkButtonPesquisar" runat="server" CssClass="btn btn-primary" Text="Pesquisar" OnClick="LinkButtonPesquisar_Click" />
        </div>

        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
            ControlToValidate="TextBoxCodServMerc" ErrorMessage="Campo num&amp;eacute;rico" ValidationExpression="\d*" Display="Dynamic"></asp:RegularExpressionValidator>

    </fieldset>


    <asp:Panel ID="PanelUnico" runat="server" Visible="false">
        <table class="table-condensed">
            <!--
            codigo descricao saldo preco codigoDeBarras
            -->
            <tr>
                <td class="texto">C&oacute;digo:</td>
                <td class="style1">
                    <asp:Label ID="LabelCodigo" CssClass="texto" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>
                <td class="texto">Descri&ccedil;&atilde;o:</td>
                <td class="style1">
                    <asp:Label ID="LabelDescricao" CssClass="texto" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="texto">Saldo:</td>
                <td class="style1">
                    <asp:Label ID="LabelSaldo" CssClass="texto" runat="server" Text=""></asp:Label></td>
            </tr>
            <!--
            <tr>
                <td class="texto">Pre&ccedil;o:</td>
                <td class="style1">
                    <asp:Label ID="LabelPreco" CssClass="texto" runat="server" Text=""></asp:Label></td>
            </tr>
            -->
            <tr>
                <td class="texto">C&oacute;digo de barras:</td>
                <td align="center" class="style1">
                    <asp:Label ID="LabelCodigoBarras" runat="server" Text="" CssClass="texto" Font-Names="CIA ITF Tall"></asp:Label></td>
            </tr>
            <tr>
                <td></td>
                <td align="center" class="style1">
                    <asp:Label ID="LabelCodigoBarrasNumero" runat="server" CssClass="texto" Text=""></asp:Label></td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="PanelVarios" runat="server">
        <asp:GridView ID="GridViewProdutos" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" Width="100%" CssClass="table-responsive" OnRowDataBound="GridViewProdutos_RowDataBound" EnableModelValidation="True">
            <RowStyle BackColor="#EFF3FB" />
            <Columns>
                <asp:BoundField DataField="Codigo" HeaderText="C&oacute;digo" ReadOnly="True" SortExpression="Codigo" Visible="true" />
                <asp:BoundField DataField="Nome" HeaderText="Descri&ccedil;&atilde;o" SortExpression="Nome" ReadOnly="True" />
                <asp:BoundField DataField="Preco" HeaderText="Pre&ccedil;o" ItemStyle-HorizontalAlign="Right" SortExpression="Preco" Visible="true" ReadOnly="True" DataFormatString="{0:0.00}">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>
                <asp:BoundField DataField="Saldo" HeaderText="Saldo" SortExpression="Saldo" ReadOnly="True" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.00}">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>
                                
                <asp:ImageField DataImageUrlField="CaminhoImagem" DataImageUrlFormatString="~/Produtos/Imagens/{0}" HeaderText="Imagem" ControlStyle-Height="100px" ControlStyle-Width="100px">

                </asp:ImageField>


            </Columns>
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#2461BF" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
    </asp:Panel>
</asp:Content>

