<%@ Page Title="Cadastro de Clientes (Prospect)" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Cliente.aspx.cs" Inherits="Cliente" %>

<%@ Register Assembly="System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.DynamicData" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">

    <script type="text/javascript" src="admin.js"></script>

</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager ID="ScriptManager1" runat="server" OnInit="ScriptManager1_Init">
    </asp:ScriptManager>

    <table class="table-responsive">

        <tr>
            <td>Tipo</td>
            <td>

                <asp:RadioButton GroupName="Tipo" ID="opFisica" runat="server" Text="Física"
                    CssClass="radio" OnCheckedChanged="opFisica_CheckedChanged"
                    AutoPostBack="True" />
                <asp:RadioButton GroupName="Tipo" ID="opJuridica" runat="server"
                    Text="Juridica" CssClass="radio" OnCheckedChanged="opJuridica_CheckedChanged"
                    AutoPostBack="True" />

                <asp:TextBox ID="tbTipoCliente" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>

            </td>

        </tr>

        <tr>
            <td>
                <asp:Label ID="lbNome" runat="server" Text="Razão Social"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbRazaoSocial" runat="server" MaxLength="50" CssClass="form-control" placeholder="Preenchimento Obrigatório"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Nome Fantasia
            </td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbNomeFantasia" runat="server" MaxLength="35"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lbTipo" runat="server" Text="CNPJ"></asp:Label>
            </td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbCNPF" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="18"></asp:TextBox>
                <asp:TextBox CssClass="form-control" ID="tbCPF" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="18" Visible="false"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lbRG" runat="server" Text="Insc. Est."></asp:Label>
            </td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRG" placeholder="Preenchimento Obrigatório" runat="server" MaxLength="15"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Endereço</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbEndereco" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="50"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Número</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbNumero" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="20"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Complemento</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbComplemento" runat="server" MaxLength="20"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Bairro</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbBairro" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="30"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Cep</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbCep" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="8"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Cidade</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbCidade" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="35"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Estado</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbEstado" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="2"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Contato</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbContato" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="100"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Telefone</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbTelefone" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="18"></asp:TextBox>
            </td>

        </tr>

        <tr>
            <td>Ramal</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRamal" runat="server" MaxLength="4"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Fax</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbFax" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="18"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Celular</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbCelular" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="18"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Email</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbEmail" runat="server" placeholder="Preenchimento Obrigatório" MaxLength="60"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Ref. Bancárias I</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRefBanc" runat="server" MaxLength="230"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Ref. Bancárias II</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRefBanc2" runat="server" MaxLength="230"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Ref. Bancárias III</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRefBanc3" runat="server" MaxLength="230"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Ref. Comerciais I</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRefCom" runat="server" MaxLength="230"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Ref. Comerciais II</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRefCom2" runat="server" MaxLength="230"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Ref. Comerciais III</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRefCom3" runat="server" MaxLength="230"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Ref. do Contador I</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRefContador" runat="server" MaxLength="230"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Ref. do Contador II</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRefContador2" runat="server" MaxLength="230"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Ref. do Contador III</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbRefContador3" runat="server" MaxLength="230"></asp:TextBox>

            </td>
        </tr>

        <tr>
            <td>Observações I</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbObs" runat="server" MaxLength="80"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Observações II</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbObs2" runat="server" MaxLength="80"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Observações III</td>
            <td>
                <asp:TextBox CssClass="form-control" ID="tbObs3" runat="server" MaxLength="80"></asp:TextBox>
            </td>
        </tr>


        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:RequiredFieldValidator ID="tipocliente" runat="server" ControlToValidate="tbTipoCliente"
                    ErrorMessage="Selecione o Tipo de Cliente" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="Nome" runat="server" ControlToValidate="tbRazaoSocial"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbNomeFantasia"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbRG"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>


                <asp:RequiredFieldValidator ID="endereco" runat="server" ControlToValidate="tbEndereco"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbNumero"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="tbBairro"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="tbCep"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RegularExpressionValidator ID="ValidaCep" runat="server"
                    ErrorMessage="Somente N&uacute;meros"
                    ControlToValidate="tbCep" Display="Dynamic"
                    ValidationExpression="\d*"></asp:RegularExpressionValidator>


                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                    ErrorMessage="Email inválido"
                    ControlToValidate="tbEmail" Display="Dynamic"
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="tbEmail"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="tbCidade"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="tbEstado"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="tbContato"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="telefone" runat="server" ControlToValidate="tbTelefone"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="tbCelular"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="tbFax"
                    ErrorMessage="Campo Obrigatório" InitialValue=""></asp:RequiredFieldValidator>



            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:Button ID="ButtonGravar" runat="server" Text="Gravar" OnClick="ButtonGravar_Click" CssClass="btn btn-primary" />
                <asp:Button ID="ButtonLimpar" runat="server" OnClick="ButtonLimpar_Click" CausesValidation="False" Text="Limpar" CssClass="btn btn-primary" />
            </td>
        </tr>

    </table>

</asp:Content>


