<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="validatpage.aspx.cs" Inherits="DYComGetKeyService.validatpage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="tb_msg" runat="server" Text="Label" Font-Bold="True" 
            Font-Italic="False" Font-Size="20pt" ForeColor="Red"></asp:Label>
        <br />
        <br />
        <asp:Image ID="Image1" runat="server" ImageUrl="~/imgs/dycom.png" />
        <asp:Image ID="Image2" runat="server" ImageUrl="~/imgs/logo.png" />
    </div>
    </form>
</body>
</html>
