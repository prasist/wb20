<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<%@ Register Src="UserControls/login.ascx" TagName="login" TagPrefix="uc1" %>

<!--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">-->

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Web Pedidos 2.0</title>

    <link href="bootstrap/css/bootstrap.min.css" rel="stylesheet" media="screen" />
    <script src="https://code.jquery.com/jquery-1.10.2.min.js"></script>
    <script src="bootstrap/js/bootstrap.min.js"></script>

</head>
<body>


    <form id="form1" runat="server">

        <center>        
            <uc1:login ID="login1" runat="server" />
        </center>
    </form>


</body>
</html>
