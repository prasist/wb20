<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="~/Pedido.cs" Inherits="Pedido" %>

<%@ Register Assembly="System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.DynamicData" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">

    <script type="text/javascript">
    <!--
    function openListElement(elementRef) {
        elementRef.size = elementRef.options.length;
    }
    function closeListElement(elementRef) {
        elementRef.size = 1;
    }
    // -->    
    </script>

    <style type="text/css">
        .auto-style1 {
            font-size: xx-large;
            color: #000000;
            background-color: #FFCC00;
            text-align: center;
        }
    </style>

</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <asp:ScriptManager ID="ScriptManager1" runat="server" OnInit="ScriptManager1_Init">
    </asp:ScriptManager>

    <ul class="nav nav-tabs">
        <li class="active"><a href="#home" data-toggle="tab">DADOS PEDIDO</a></li>
        <li><a href="#itens" data-toggle="tab">ITENS</a></li>
    </ul>

    <div id="myTabContent" class="tab-content">

        <!--Dados Pedido-->
        <div class="tab-pane fade active in" id="home">
            <asp:UpdatePanel ID="UpdatePanel5" runat="server">

                <ContentTemplate>
                    <fieldset>

                        <div class="form-inline">
                            <p>&nbsp;</p>
                            <label class="control-label" for="LabelNumPedido">N. Pedido Web</label>
                            <strong>
                                <asp:Label ID="LabelNumPedido" runat="server" Text="Label" CssClass="form-control" Enabled="false" Width="100px"></asp:Label></strong>
                            <label class="control-label" for="LabelNumPedidoSigma">N. Pedido SIGMA</label>
                            <strong>
                                <asp:Label ID="LabelNumPedidoSigma" runat="server" Text="Label" CssClass="form-control" Enabled="false" Width="100px"></asp:Label></strong>
                            <label class="control-label" for="LabelVendedor">Vendedor</label>
                            <strong>
                                <asp:Label ID="LabelVendedor" runat="server" CssClass="form-control" Text="Label" Enabled="false"></asp:Label></strong>
                            <p>&nbsp;</p>
                        </div>

                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><strong>Cliente</strong></h3>
                            </div>
                            <div class="panel-body">

                                <div class="form-group">
                                    <asp:TextBox ID="tbCliente" runat="server" placeholder="(Digite o Código,  Razão Social, Nome Fantasia ou CNPJ / CPF  do Cliente e clique no botão PESQUISAR" CssClass="form-control"></asp:TextBox>
                                    <asp:Button ID="buPesquisar" runat="server" Text="Pesquisar..." OnClick="buPesquisar_Click" CssClass="btn btn-default" />
                                    <asp:DropDownList CssClass="form-control" ID="cbCliente" placeholder="Selecione um Cliente" runat="server" OnSelectedIndexChanged="DropDownListCliente_SelectedIndexChanged" AutoPostBack="True">
                                    </asp:DropDownList>
                                    <asp:Label ID="lbMensagem" runat="server" CssClass="text-danger"></asp:Label>
                                    <asp:Label ID="lbDados" runat="server" CssClass="text-info"></asp:Label>
                                </div>

                            </div>
                        </div>

                        <div class="form-group">
                            <label for="TextBoxObservacao" class="control-label">Observação</label>
                            <asp:TextBox CssClass="form-control" ID="TextBoxObservacao" runat="server" placeholder="Máximo 60 caracteres" MaxLength="60"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="csMovimentacao" class="control-label">Movimentação</label>
                            <asp:DropDownList CssClass="form-control" ID="csMovimentacao" runat="server"
                                AutoPostBack="True" OnSelectedIndexChanged="csMovimentacao_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label for="DropDownListCondPagto" class="control-label">Cond. Pagto.</label>
                            <asp:DropDownList CssClass="form-control" ID="DropDownListCondPagto" runat="server"
                                OnSelectedIndexChanged="DropDownListCondPagto_SelectedIndexChanged"
                                AutoPostBack="True">
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label for="cbTabelas" class="control-label">Tabela Preço</label>
                            <asp:DropDownList ID="cbTabelas" runat="server" AutoPostBack="True" CssClass="form-control" OnSelectedIndexChanged="cbTabelas_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>

                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!--Itens-->
        <div class="tab-pane fade" id="itens">
            <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                <ContentTemplate>

                    <div class="form-inline">

                        <label for="cbUnidadeVenda" class="control-label">Unid. Venda</label>
                        <asp:DropDownList ID="cbUnidadeVenda" runat="server" AutoPostBack="True" CssClass="form-control" OnSelectedIndexChanged="cbUnidadeVenda_SelectedIndexChanged">
                        </asp:DropDownList>

                        <%
                            WebPedidos.WSClasses.ParametroResumido pr;
                            pr = (WebPedidos.WSClasses.ParametroResumido)Session["Parametros"];

                            if (pr.CondicaoTabLivreWeb == 1)
                            {   
                        %>
                        <div class="form-group">
                            <label for="cbPrazoTabela" class="control-label">Prazo Tabela</label>
                            <asp:DropDownList ID="cbPrazoTabela" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="cbPrazoTabela_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <% } %>
                        <p>&nbsp;</p>
                    </div>

                    <!--PRODUTOS-->
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><strong>Produtos</strong></h3>
                        </div>
                        <div class="panel-body">

                            <div class="form-group">
                                <asp:TextBox ID="txtPesqProduto" runat="server" CssClass="form-control" placeholder="Digite o Código ou Descrição do Produto e clique em PESQUISAR"></asp:TextBox>
                                <asp:UpdateProgress ID="UpdateProgress1" DisplayAfter="1" runat="server" AssociatedUpdatePanelID="UpdatePanel6">
                                    <ProgressTemplate>
                                        <div class="text-center">
                                            <strong><span class="btn btn-warning btn-lg">Aguarde...Carregando </span></strong>
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <asp:Button ID="butPesqProdutos" runat="server" CssClass="btn btn-default" OnClick="butPesqProdutos_Click" Text="Pesquisar..." />
                                <asp:DropDownList ID="cbProdutos" CssClass="form-control" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cbProdutos_SelectedIndexChanged" ValidationGroup="Prod" onfocus="openListElement(this);">
                                </asp:DropDownList>
                            </div>

                            <div class="form-inline">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <label for="LabelPreco" class="control-label">Preço : </label>
                                        <asp:Label ID="LabelPreco" runat="server" CssClass="text-info text-center" Enabled="false" Width="80px" Text=""></asp:Label>

                                        <label for="LabelSaldo" class="control-label">Saldo : </label>
                                        <asp:Label ID="LabelSaldo" runat="server" CssClass="text-info text-center" Enabled="false" Width="80px" Text=""></asp:Label>

                                        <label for="lbPeso" class="control-label">Peso : </label>
                                        <asp:Label ID="lbPeso" runat="server" CssClass="text-info text-center" Enabled="false" Width="80px" Text=""></asp:Label>

                                        <label for="lbUnidade" class="control-label">Unidade Medida : </label>
                                        <asp:Label ID="lbUnidade" runat="server" CssClass="text-info text-center" Enabled="false" Width="80px"></asp:Label>

                                        <asp:Label ID="lbMensagemPromocional" runat="server" Enabled="false" CssClass="text-info"></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                                <p>&nbsp;</p>
                                <asp:Label ID="lbTipoPreco" CssClass="control-label" runat="server" Text="Preço"></asp:Label>
                                <asp:TextBox ID="TB_Preco" CssClass="form-control text-center" runat="server" OnTextChanged="TB_Preco_TextChanged" Width="90px" AutoPostBack="True"></asp:TextBox>

                                <label for="TB_Desconto" class="control-label">% Desconto</label>
                                <asp:TextBox ID="TB_Desconto" runat="server" CssClass="form-control text-center" OnTextChanged="TextDesconto_TextChanged" Width="90px" AutoPostBack="True"></asp:TextBox>

                                <label for="tbPrecoLiq" class="control-label">Preço Líquido</label>
                                <asp:TextBox ID="tbPrecoLiq" runat="server" CssClass="form-control text-center" ReadOnly="True" Width="90px"></asp:TextBox>

                                <label for="TextBoxQuantidade" class="control-label">Quantidade</label>
                                <asp:TextBox ID="TextBoxQuantidade" runat="server" CausesValidation="True" CssClass="form-control text-center" Width="90px" OnTextChanged="TextBoxQuantidade_TextChanged" ValidationGroup="Prod"></asp:TextBox>

                                <asp:Button ID="LinkButtonAdicionar0" runat="server" CssClass="btn btn-primary btn-sm" OnClick="LinkButtonAdicionar_Click" Text="Adicionar Item" />

                            </div>

                        </div>
                    </div>

                    <div class="form-group">
                        <asp:RequiredFieldValidator ID="RangeValidator2" runat="server" ControlToValidate="TextBoxQuantidade" ErrorMessage="Valor Tabela de Preço Zerada." ValidationGroup="Prod"></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBoxQuantidade" Display="Dynamic" ErrorMessage="Campo Obrigatório" ValidationGroup="Prod"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TextBoxQuantidade" ErrorMessage="Apenas Números" ValidationExpression="^\d+$" Display="Dynamic" ValidationGroup="Prod"></asp:RegularExpressionValidator>
                        <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="TextBoxQuantidade" ErrorMessage="Quantidade inválida" SetFocusOnError="True" ValidationGroup="Prod" Type="Double"></asp:RangeValidator>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="cbProdutos" ErrorMessage="Campo Obrigatório" InitialValue="-1" SetFocusOnError="True" ValidationGroup="Prod"></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="rfvPesqProdutos" runat="server" ControlToValidate="txtPesqProduto" ErrorMessage="Digite o texto a ser pesquisado." ValidationGroup="PesqProduto" />
                    </div>

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <ul class="nav nav-pills">
                                <li class="active"><a href="#">Itens <span class="badge">
                                    <asp:Label ID="idItens" runat="server" CssClass="texto_bold"></asp:Label></span></a></li>

                                <li class="active"><a href="#">Itens não atendidos <span class="badge">
                                    <asp:Label ID="lbNaoAtendidos" runat="server" CssClass="texto_atencao"></asp:Label>
                                </span></a>
                                </li>

                            </ul>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">

                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="table-condensed">
                                            <RowStyle />
                                            <Columns>
                                                <asp:BoundField DataField="Codigo" HeaderText="Código"
                                                    ItemStyle-HorizontalAlign="Right" SortExpression="Codigo">
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                                                <asp:BoundField DataField="Preco" DataFormatString="R${0:0.00}"
                                                    HeaderText="Preço Unit." ItemStyle-HorizontalAlign="Right"
                                                    SortExpression="Preco">
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Quantidade" HeaderText="Qtde."
                                                    ItemStyle-HorizontalAlign="Right" SortExpression="Quantidade">
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Desconto" HeaderText="% Desc."
                                                    ItemStyle-HorizontalAlign="Right" SortExpression="Desconto">
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="preco_liquido" DataFormatString="R${0:0.00}"
                                                    HeaderText="Preço Unit. Liq." ItemStyle-HorizontalAlign="Right"
                                                    SortExpression="preco_liquido">
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="Peso" DataFormatString="{0:0.00}"
                                                    HeaderText="Peso Parcial" ItemStyle-HorizontalAlign="Right"
                                                    SortExpression="Peso Parcial">
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="TotalParcial" DataFormatString="R${0:0.00}"
                                                    HeaderText="Total Parcial" ItemStyle-HorizontalAlign="Right"
                                                    SortExpression="TotalParcial">
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:BoundField>

                                                <asp:TemplateField ShowHeader="False">
                                                    <ItemTemplate>
                                                        <asp:Button ID="LinkButton1" runat="server" CausesValidation="False"
                                                            CommandArgument='<%# Eval("Codigo") %>' CommandName="Delete" CssClass="btn btn-danger btn-xs"
                                                            OnClick="LinkButton1_Click" Text="Remover" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>

                                            <FooterStyle BackColor="#507CD1" ForeColor="White" />
                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#D1DDF1" ForeColor="#333333" />
                                            <HeaderStyle BackColor="#336699" ForeColor="White" />
                                            <EditRowStyle BackColor="#2461BF" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </div>
                        </div>
                    </div>

                    <div class="panel panel-success">
                        <div class="panel-heading">
                            <h3 class="panel-title">Totais</h3>
                        </div>
                        <div class="panel-body">

                            <div class="form-inline">

                                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                    <ContentTemplate>
                                        <label for="tbSubTotal" class="control-label">Sub Total</label>
                                        <asp:TextBox ID="tbSubTotal" runat="server" AutoPostBack="True" CssClass="form-control text-center" OnTextChanged="TB_DescPed_TextChanged" Width="86px" ReadOnly="True"></asp:TextBox>

                                        <label for="tbPesoTotal" class="control-label">Peso Total</label>
                                        <asp:TextBox ID="tbPesoTotal" runat="server" AutoPostBack="True" CssClass="form-control text-center" OnTextChanged="TB_DescPed_TextChanged" Width="86px" ReadOnly="True"></asp:TextBox>

                                        <label for="TB_DescPed" class="control-label">% Desconto</label>
                                        <asp:TextBox ID="TB_DescPed" runat="server" AutoPostBack="True" CssClass="form-control text-center" OnTextChanged="TB_DescPed_TextChanged" Width="86px"></asp:TextBox>

                                        <label for="TB_VlrDes" class="control-label">Vlr. Desc.</label>
                                        <asp:TextBox ID="TB_VlrDes" runat="server" AutoPostBack="True" CssClass="form-control text-center" OnTextChanged="TB_VlrDes_TextChanged" Width="86px"></asp:TextBox>

                                        <label for="tbTotalPedido" class="control-label">Total Pedido</label>

                                        <asp:TextBox ID="tbTotalPedido" runat="server" AutoPostBack="True" Width="86px" CssClass="form-control text-center" OnTextChanged="TB_DescPed_TextChanged" ReadOnly="True"></asp:TextBox>

                                    </ContentTemplate>
                                </asp:UpdatePanel>


                            </div>


                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>


        </div>

        <!--Botões-->
        <div class="form-group">

            <asp:Button ID="ButtonGravar" runat="server" Text="Gravar" OnClick="ButtonGravar_Click" CssClass="btn btn-primary btn-lg" />
            <asp:Button ID="ButtonLimpar" runat="server" OnClick="ButtonLimpar_Click" CausesValidation="False" Text="Limpar" CssClass="btn btn-primary btn-lg" />

        </div>

    </div>

</asp:Content>

