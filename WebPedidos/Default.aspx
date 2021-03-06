﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <asp:Panel ID="pnl1" runat="server">

        <div class="container">

            <div class="panel">
                <h1>Atalhos</h1>

                <div class="pull-left">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title text-center"><strong>Cadastro Pedidos</strong></h3>
                        </div>

                        <div class="panel-body">
                            <a href="Pedido.aspx">
                                <asp:Image ID="Image2" runat="server" Height="180px" ImageUrl="~/imagens/order_form_online_offer-512.png" Width="180px" /></a>
                        </div>

                    </div>
                </div>

                <div class="pull-left">
                    <div class="panel panel-default">

                        <div class="panel-heading">
                            <h3 class="panel-title text-center"><strong>Consulta Financeiro</strong></h3>
                        </div>

                        <div class="panel-body">
                            <a href="RelFinanceiro.aspx">
                                <asp:Image ID="Image3" runat="server" Height="180px" ImageUrl="~/imagens/finjance.png" Width="180px" Style="text-align: center" /></a>
                        </div>

                    </div>
                </div>

                <div class="pull-left">
                    <div class="panel panel-default">

                        <div class="panel-heading">
                            <h3 class="panel-title text-center"><strong>Importação Arquivos OFFLINE</strong></h3>
                        </div>

                        <div class="panel-body">
                            <a href="importacao.aspx">
                                <asp:Image ID="Image1" runat="server" Height="180px" ImageUrl="~/imagens/Folder-6-512.png" Width="180px" /></a>
                        </div>

                    </div>
                </div>

                <div class="pull-right">
                    <div class="panel panel-default">

                        <div class="panel-heading">
                            <h3 class="panel-title text-center"><strong>Mensagens</strong></h3>
                        </div>
                        <p>
                            <asp:Label ID="lbUltima" runat="server"></asp:Label>
                        </p>
                        <p>
                            Pedido(s) no dia :
                        <asp:Label ID="lbPedidos" runat="server" CssClass="texto_bold_gd" Text="Label"></asp:Label>
                        </p>
                        <p>
                            Peso Total :
                        <asp:Label ID="lbPeso" runat="server" CssClass="texto_bold_gd" Text="Label"></asp:Label>
                        </p>

                        <p>
                            <asp:Label ID="lbPedidosMsg" runat="server"></asp:Label>
                        </p>
                        <p><a href="instalar.aspx" target="_blank">ATUALIZAÇÃO DO APP OFFLINE</a> (vs 4.0.3)</p>
                    </div>
                </div>
            </div>
            
        </div>

    </asp:Panel>

    <br />
    <p></p>
</asp:Content>