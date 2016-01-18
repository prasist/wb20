<%@ Control Language="C#" AutoEventWireup="true" CodeFile="login.ascx.cs" Inherits="login" %>

<!--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">-->
<script language="javascript" type="text/javascript">
</script>

<div id="log" class="logar">

    <div class="panel panel-primary">
        <div class="panel-heading">
            <h3 class="panel-title">Acesso Restrito</h3>
        </div>
        <div class="panel-body">
            <div class="form-inline">
                <label for="TBEmpresa" class="control-label">Empresa : </label>
                <asp:TextBox ID="TBEmpresa" runat="server" ValidationGroup="Login" CssClass="form-control" Width="205px" OnTextChanged="TBEmpresa_TextChanged"></asp:TextBox>
            </div>

            <div class="form-inline">
                <label for="TBUsuario" class="control-label">Usuário : </label>
                <asp:TextBox ID="TBUsuario" runat="server" ValidationGroup="Login" CssClass="form-control" Width="205px"></asp:TextBox>
            </div>

            <div class="form-inline">
                <label for="Password1" class="control-label">Senha : </label>                
                <asp:TextBox ID="Password1" runat="server" ValidationGroup="Login" CssClass="form-control" Width="205px" TextMode="Password"></asp:TextBox>
            </div>

            <div class="form-group">
                <p>&nbsp;</p>
                <asp:Button ID="LinkButtonOk" runat="server" CssClass="btn btn-primary" OnClick="BU_Ok_Click1" ValidationGroup="Login" Text="OK" Width="120px" />
            </div>


            <asp:RegularExpressionValidator ID="RegularExpressionValidatorEmpresa" runat="server"
                ErrorMessage="Somente n&uacute;meros" ControlToValidate="TBEmpresa"
                ValidationExpression="^[0-9]+$" Display="Dynamic"></asp:RegularExpressionValidator>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorEmpresa" runat="server"
                ErrorMessage="Campo Obrigat&oacute;rio" ControlToValidate="TBEmpresa"
                ValidationGroup="Login" Display="Dynamic"></asp:RequiredFieldValidator>

            <asp:RegularExpressionValidator ID="RegularExpressionValidatorUsuario" runat="server"
                ErrorMessage="Somente n&uacute;meros" ValidationExpression="^[0-9]+$"
                ControlToValidate="TBUsuario" Display="Dynamic"></asp:RegularExpressionValidator>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorUsuario" runat="server"
                ErrorMessage="Campo Obrigat&oacute;rio" ControlToValidate="TBUsuario"
                ValidationGroup="Login" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorSenha" runat="server"
                ErrorMessage="Campo Obrigat&oacute;rio" ControlToValidate="Password1"
                ValidationGroup="Login" Display="Dynamic"></asp:RequiredFieldValidator>

        </div>
    </div>




</div>
