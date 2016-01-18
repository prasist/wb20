<%@ Page Title="Importação Arquivos" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="importacao.aspx.cs" Inherits="importacao" %>

<%@ Register Assembly="System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.DynamicData" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">

    <script type="text/javascript" src="admin.js"></script>
    
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <asp:ScriptManager ID="ScriptManager1" runat="server" OnInit="ScriptManager1_Init">
    </asp:ScriptManager>


    <ul class="nav nav-tabs">
        <li class="active">
            <a href="#home" data-toggle="tab">IMPORTAÇÃO DO ARQUIVO</a></li>
        <li><a href="#itens" data-toggle="tab">HISTÓRICO DE IMPORTAÇÕES</a></li>
    </ul>


    <div id="myTabContent" class="tab-content">
        <div class="tab-pane fade active in" id="home">
            <asp:UpdatePanel ID="pnImportacao" runat="server">
                <Triggers>
                    <asp:PostBackTrigger ControlID="buImportar" />
                </Triggers>

                <ContentTemplate>
                                    
                    <div class="form-group">
                        <asp:CheckBox ID="ckPermite" runat="server" CssClass="checkbox" Text="Permitir importar o mesmo arquivo" />
                        <asp:Label ID="Label1" runat="server"></asp:Label>
                        <asp:Label ID="Label2" runat="server" Visible="False"></asp:Label>
                        <br />
                        <asp:Label ID="lbErros" runat="server" CssClass="text-danger h3" Visible="False"></asp:Label>
                    </div>

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title text-center"><strong>Passo 1 - Seleção do arquivo TXT</strong></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-group">
                                <asp:Label ID="lbPasso1" runat="server">Selecione o arquivo da pasta "ENVIAR" do seu Tablet</asp:Label>
                                <asp:FileUpload ID="FileUpload1" runat="server" OnDisposed="buDownload_Click" />
                            </div>
                        </div>

                    </div>

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title text-center"><strong>Passo 2 - Envio do Arquivo TXT para o servidor</strong></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-group">
                                <asp:Label ID="lbPasso2" runat="server">Clique no botão Importar após conclusão do Passo 1</asp:Label><br />
                                <asp:Button ID="buImportar" CssClass="btn btn-primary" runat="server" OnClick="buImportar_Click" Text="Importar" />
                            </div>

                        </div>

                    </div>

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title text-center"><strong>Passo 3 - Leitura do TXT e gravação do pedido no SIGMA</strong></h3>
                        </div>

                        <div class="panel-body">
                            <asp:Label ID="lbPasso3" runat="server">Após conclusão do Passo 2, clicar no botão abaixo</asp:Label>
                            <div class="form-group">
                                <asp:Button ID="buDownload" runat="server" CssClass="btn btn-primary" Visible="False" Text="GRAVAR PEDIDO(S) IMPORTADO(S)" OnClick="buDownload_Click" />
                            </div>
                        </div>
                    </div>


                </ContentTemplate>

            </asp:UpdatePanel>
        </div>

        <div class="tab-pane fade" id="itens">
            <asp:UpdatePanel ID="pnLog" runat="server">

                <ContentTemplate>

                    <div class="form-inline">
                        <p>&nbsp;</p>
                        <label for="tbDataEmissaoInicial" class="control-label">Data Importação</label>
                        <asp:TextBox ID="tbDataEmissaoInicial" runat="server" CssClass="form-control" MaxLength="10" onkeyup="Mascara('DATA',this,event);" Width="90px"></asp:TextBox>
                        <label for="tbDataEmissaoFinal" class="control-label">Até</label>
                        <asp:TextBox ID="tbDataEmissaoFinal" runat="server" CssClass="form-control" MaxLength="10" onkeyup="Mascara('DATA',this,event);" Width="90px"></asp:TextBox>
                        <asp:Button ID="LinkButtonPesquisar" runat="server" CssClass="btn btn-primary" OnClick="LinkButtonPesquisar_Click" Text="Pesquisar" />
                        <p>&nbsp;</p>
                    </div>

                    <div class="form-group">
                        <asp:GridView ID="gdLog" runat="server" AutoGenerateColumns="False" CssClass="table-responsive" OnRowDataBound="gdLog_RowDataBound">
                            <RowStyle />

                            <Columns>

                                <asp:TemplateField HeaderText="N. Pedido">
                                    <ItemTemplate>

                                        <asp:HyperLink
                                            ID="HyperLink2"
                                            runat="server"
                                            NavigateUrl='<%# String.Format("RelPedido.aspx?id={0}", Eval("NUMPED")) %>'
                                            onclick="javascript:w= window.open(this.href,'Pedido','toolbar=0,resizable=0');return false;"><%# Eval("NUMPED")%></asp:HyperLink>

                                    </ItemTemplate>

                                    <HeaderStyle />
                                    <ItemStyle Wrap="False" />
                                </asp:TemplateField>


                                <asp:BoundField DataField="CODCLI" HeaderText="Cliente" ItemStyle-HorizontalAlign="Left" SortExpression="CODCLI">
                                    <ItemStyle HorizontalAlign="Left" Width="200" />
                                </asp:BoundField>

                                <asp:BoundField DataField="DATA" HeaderText="Data" ItemStyle-HorizontalAlign="Left" SortExpression="DATA">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:BoundField DataField="VALOR" HeaderText="Valor" DataFormatString="{0:G}" ItemStyle-HorizontalAlign="Left" SortExpression="VALOR">
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:BoundField>


                                <asp:BoundField DataField="MSG" HeaderText="Mensagem" ItemStyle-Wrap="true" ItemStyle-VerticalAlign="Top" SortExpression="MSG">
                                    <ItemStyle VerticalAlign="Top" Width="250" />
                                </asp:BoundField>

                                <asp:BoundField DataField="STATUS" HeaderText="Status" ItemStyle-HorizontalAlign="Left" SortExpression="STATUS">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>

                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>

                                        <asp:HyperLink
                                            ID="HyperLink1"
                                            runat="server"
                                            NavigateUrl='<%# String.Format("itens_n_atendidos.aspx?id={0}", Eval("NUMPED_LOG")) %>'
                                            onclick="javascript:w= window.open(this.href,'Itens','left=40,top=40,width=750,height=600,toolbar=0,resizable=0');return false;"><img src='icones/itens.png' border='0' alt='Itens não atendidos' width='60' height='60' /></asp:HyperLink>

                                    </ItemTemplate>

                                    <HeaderStyle VerticalAlign="Top" />
                                    <ItemStyle VerticalAlign="Top" Wrap="False" />
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="#507CD1" ForeColor="White" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#D1DDF1" ForeColor="#333333" />
                            <HeaderStyle BackColor="#507CD1" ForeColor="White" />
                            <EditRowStyle BackColor="#2461BF" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>

                    </div>

                    <div class="form-inline">

                        <label for="lbTotal" class="control-label">Total : </label>
                        <strong>
                            <asp:Label ID="lbTotal" runat="server" Text="0,00"></asp:Label></strong>
                    </div>



                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

    </div>

</asp:Content>

